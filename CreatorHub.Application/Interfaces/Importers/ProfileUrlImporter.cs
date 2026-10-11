using System.Net;
using System.Text.RegularExpressions;
using CreatorHub.Application.Interfaces.Importers;
using CreatorHub.Application.Interfaces.Repositories;
using CreatorHub.Domain.Entities;

namespace CreatorHub.Application.Importers;

public class ProfileUrlImporter : IProfileUrlImporter
{
    private readonly HttpClient _httpClient;
    private readonly IProfileUrlRepository _profileUrlRepository;

    private const int StartPage = 1;
    private const int EndPage = 480;

    private const string BaseSearchUrl =
        "https://www.frivol.com/amateure/finden" +
        "?member_search_form%5Bterm%5D=" +
        "&member_search_form%5BmemberType%5D=amateur" +
        "&member_search_form%5BmemberSex%5D=all" +
        "&member_search_form%5Bregion%5D=" +
        "&member_search_form%5Bonline%5D=all" +
        "&member_search_form%5BprofilePhotoOnly%5D=1" +
        "&member_search_form%5BminAge%5D=18" +
        "&member_search_form%5BmaxAge%5D=127" +
        "&member_search_form%5Bsubmit%5D=" +
        "&member_search_form%5B_t%5D=a35a16dda95ea803289555d794.mVJB7Vk3Y7bWLaghU_c56-_XmHmFiU5943pupqhDZ2Y.qxwqgG1vLumTf-54Z5tNkYqF4kv23TlOriAd9vJyHQ2hahjdNwE2gJNY4Q";

    public ProfileUrlImporter(
        HttpClient httpClient,
        IProfileUrlRepository profileUrlRepository)
    {
        _httpClient = httpClient;
        _profileUrlRepository = profileUrlRepository;
    }

    public async Task ImportAsync(CancellationToken cancellationToken = default)
    {
        var totalFound = 0;

        for (var page = StartPage; page <= EndPage; page++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var url = $"{BaseSearchUrl}&page={page}";

            Console.WriteLine(
                $"Fetching Frivol search page {page}/{EndPage}...");

            try
            {
                using var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    url);

                request.Headers.UserAgent.ParseAdd(
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120 Safari/537.36");

                request.Headers.Accept.ParseAdd(
                    "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

                request.Headers.AcceptLanguage.ParseAdd(
                    "de-DE,de;q=0.9,en;q=0.8");

                request.Headers.Referrer =
                    new Uri("https://www.frivol.com/");

                using var response =
                    await _httpClient.SendAsync(
                        request,
                        cancellationToken);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine(
                        $"Page {page} returned {(int)response.StatusCode}.");

                    continue;
                }

                var html =
                    await response.Content.ReadAsStringAsync(
                        cancellationToken);

                var profiles = ExtractProfileUrls(html, page);

                var entities = profiles.Select(profile =>
                    new ProfileUrl
                    {
                        Id = Guid.NewGuid(),
                        Username = profile.Username,
                        Url = profile.ProfileUrl,
                        Source = "frivol",
                        SourcePage = profile.SourcePage,
                        IsProcessed = false,
                        CreatedAt = DateTime.UtcNow
                    }).ToList();

                if (entities.Count > 0)
                {
                    await _profileUrlRepository.AddRangeAsync(entities);
                    await _profileUrlRepository.SaveChangesAsync();
                }

                totalFound += entities.Count;

                Console.WriteLine(
                    $"Profiles on page {page}: {entities.Count} | Total: {totalFound}");

                if (page % 20 == 0)
                {
                    await Task.Delay(
                        TimeSpan.FromSeconds(5),
                        cancellationToken);
                }
                else
                {
                    await Task.Delay(
                        TimeSpan.FromMilliseconds(300),
                        cancellationToken);
                }
            }
            catch (Exception ex) when (
                ex is HttpRequestException ||
                ex is TaskCanceledException)
            {
                Console.WriteLine(
                    $"Error on page {page}: {ex.Message}");
            }
        }

        Console.WriteLine(
            $"Profile URL import finished. Total: {totalFound}");
    }

    private static List<ProfileUrlResult> ExtractProfileUrls(
        string html,
        int sourcePage)
    {
        var profiles = new List<ProfileUrlResult>();

        var matches = Regex.Matches(
            html,
            @"href=[""']([^""']*/profil/[^""']+)[""']",
            RegexOptions.IgnoreCase);

        foreach (Match match in matches)
        {
            var href =
                WebUtility.HtmlDecode(
                    match.Groups[1].Value);

            var profileUrl =
                NormalizeProfileUrl(href);

            if (!profileUrl.StartsWith(
                    "https://www.frivol.com/profil/",
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            profileUrl =
                Regex.Replace(
                    profileUrl,
                    @"[#?].*$",
                    string.Empty);

            var username =
                ExtractUsername(profileUrl);

            if (string.IsNullOrWhiteSpace(username))
                continue;

            profiles.Add(
                new ProfileUrlResult(
                    username,
                    profileUrl,
                    sourcePage));
        }

        return profiles;
    }

    private static string NormalizeProfileUrl(string href)
    {
        if (href.StartsWith("http://") ||
            href.StartsWith("https://"))
        {
            return href;
        }

        if (href.StartsWith("/"))
        {
            return $"https://www.frivol.com{href}";
        }

        return $"https://www.frivol.com/{href}";
    }

    private static string ExtractUsername(string url)
    {
        if (!Uri.TryCreate(
                url,
                UriKind.Absolute,
                out var uri))
        {
            return string.Empty;
        }

        var parts =
            uri.AbsolutePath
                .Trim('/')
                .Split('/');

        if (parts.Length >= 2 &&
            parts[0].Equals(
                "profil",
                StringComparison.OrdinalIgnoreCase))
        {
            return Uri.UnescapeDataString(parts[1]);
        }

        return string.Empty;
    }

    private sealed record ProfileUrlResult(
        string Username,
        string ProfileUrl,
        int SourcePage);
}