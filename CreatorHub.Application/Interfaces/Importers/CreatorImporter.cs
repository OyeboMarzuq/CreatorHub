using CreatorHub.Application.Interfaces.Importers;
using CreatorHub.Application.Interfaces.Repositories;
using CreatorHub.Domain.Entities;
using System.Net;
using System.Text.RegularExpressions;

namespace CreatorHub.Application.Importers;

public class CreatorImporter : ICreatorImporter
{
    private const int DefaultBatchSize = 10;

    private const string PlatformSlug = "frivol";
    private const string PlatformName = "Frivol";
    private const string PlatformBaseUrl = "https://www.frivol.com";

    private readonly HttpClient _httpClient;
    private readonly IProfileUrlRepository _profileUrlRepository;
    private readonly ICreatorRepository _creatorRepository;

    public CreatorImporter(
        HttpClient httpClient,
        IProfileUrlRepository profileUrlRepository,
        ICreatorRepository creatorRepository)
    {
        _httpClient = httpClient;
        _profileUrlRepository = profileUrlRepository;
        _creatorRepository = creatorRepository;

        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
            "(KHTML, like Gecko) Chrome/154.0.0.0 Safari/537.36");
    }

    public async Task ImportAsync(
        CancellationToken cancellationToken = default)
    {
        await ImportAsync(
            DefaultBatchSize,
            cancellationToken);
    }

    public async Task ImportAsync(
        int batchSize,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Creator importer started.");

        if (batchSize < 1)
            batchSize = DefaultBatchSize;

        if (batchSize > 100)
            batchSize = 100;

        var platform =
            await _creatorRepository.GetPlatformBySlugAsync(PlatformSlug);

        if (platform == null)
        {
            platform = new Platform
            {
                Id = Guid.NewGuid(),
                Name = PlatformName,
                Slug = PlatformSlug,
                BaseUrl = PlatformBaseUrl,
                IsActive = true
            };

            await _creatorRepository.AddPlatformAsync(platform);
            await _creatorRepository.SaveChangesAsync();

            Console.WriteLine("Created Frivol platform.");
        }

        var profileUrls =
            await _profileUrlRepository.GetUnprocessedAsync(
                batchSize,
                cancellationToken);

        Console.WriteLine(
            $"Found {profileUrls.Count} unprocessed profile URLs.");

        foreach (var profileUrl in profileUrls)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                Console.WriteLine(
                    $"Importing: {profileUrl.Username}");

                var html = await DownloadProfileAsync(
                    profileUrl.Url,
                    cancellationToken);

                var profile = ParseProfile(
                    html,
                    profileUrl);

                if (profile == null)
                {
                    Console.WriteLine(
                        $"Could not extract profile: {profileUrl.Url}");

                    continue;
                }

                await SaveProfileAsync(
                    platform,
                    profile,
                    cancellationToken);

                profileUrl.IsProcessed = true;
                profileUrl.ImageUrl = profile.ProfileImageUrl;
                profileUrl.ProcessedAt = DateTime.UtcNow;

                await _profileUrlRepository.SaveChangesAsync();

                Console.WriteLine(
                    $"Imported successfully: {profile.Username}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Failed: {profileUrl.Url}");

                Console.WriteLine(ex.Message);
            }

            await Task.Delay(
                TimeSpan.FromMilliseconds(300),
                cancellationToken);
        }

        Console.WriteLine("Creator importer finished.");
    }

    private async Task<string> DownloadProfileAsync(
        string url,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            url);

        request.Headers.Accept.ParseAdd(
            "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

        request.Headers.AcceptLanguage.ParseAdd(
            "de-DE,de;q=0.9,en-US;q=0.8,en;q=0.7");

        request.Headers.Referrer =
            new Uri(PlatformBaseUrl);

        using var response =
            await _httpClient.SendAsync(
                request,
                cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(
            cancellationToken);
    }

    private async Task SaveProfileAsync(
        Platform platform,
        ParsedProfile profile,
        CancellationToken cancellationToken)
    {
        var existingPlatformProfile =
            await _creatorRepository.GetPlatformProfileByUsernameAsync(
                platform.Id,
                profile.Username);

        if (existingPlatformProfile != null)
        {
            existingPlatformProfile.ProfileUrl = profile.ProfileUrl;
            existingPlatformProfile.ExternalId = profile.ExternalId;
            existingPlatformProfile.ProfileImageUrl =
                profile.ProfileImageUrl;
            existingPlatformProfile.IsActive = true;
            existingPlatformProfile.LastImportedAt =
                DateTime.UtcNow;

            await _creatorRepository.SaveChangesAsync();

            return;
        }

        var slug = CreateSlug(profile.Username);

        var creator =
            await _creatorRepository.GetBySlugAsync(slug);

        if (creator == null)
        {
            creator = new Creator
            {
                Id = Guid.NewGuid(),
                DisplayName = profile.DisplayName,
                Slug = slug,
                Age = profile.Age,
                Gender = profile.Gender,
                Country = profile.Country,
                Region = profile.Region,
                Bio = profile.Bio,
                ProfileImageUrl = profile.ProfileImageUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _creatorRepository.AddAsync(creator);

            await _creatorRepository.SaveChangesAsync();
        }
        else
        {
            creator.DisplayName = profile.DisplayName;
            creator.Age = profile.Age;
            creator.Gender = profile.Gender;
            creator.Country = profile.Country;
            creator.Region = profile.Region;
            creator.Bio = profile.Bio;
            creator.ProfileImageUrl =
                profile.ProfileImageUrl;
            creator.UpdatedAt = DateTime.UtcNow;

            await _creatorRepository.UpdateAsync(creator);
            await _creatorRepository.SaveChangesAsync();
        }

        var platformProfile = new PlatformProfile
        {
            Id = Guid.NewGuid(),
            CreatorId = creator.Id,
            PlatformId = platform.Id,
            Username = profile.Username,
            ProfileUrl = profile.ProfileUrl,
            ExternalId = profile.ExternalId,
            ProfileImageUrl = profile.ProfileImageUrl,
            IsActive = true,
            LastImportedAt = DateTime.UtcNow
        };

        await _creatorRepository.AddPlatformProfileAsync(
            platformProfile);

        await _creatorRepository.SaveChangesAsync();

        cancellationToken.ThrowIfCancellationRequested();
    }

    private static ParsedProfile? ParseProfile(
        string html,
        ProfileUrl profileUrl)
    {
        var username =
            ExtractUsername(html)
            ?? profileUrl.Username;

        if (string.IsNullOrWhiteSpace(username))
            return null;

        return new ParsedProfile
        {
            Username = WebUtility.HtmlDecode(username).Trim(),

            DisplayName =
                WebUtility.HtmlDecode(username).Trim(),

            Age = ExtractInt(
                html,
                @"(?:Alter|Age)\s*</[^>]+>\s*([^<]+)"),

            Gender =
                ExtractValue(
                    html,
                    "Geschlecht",
                    "Gender"),

            Country =
                ExtractValue(
                    html,
                    "Land",
                    "Country"),

            Region =
                ExtractValue(
                    html,
                    "PLZ / Region",
                    "PLZ/Region",
                    "Region"),

            Bio =
                ExtractTextByClass(
                    html,
                    "profile-description",
                    "description",
                    "profile-about"),

            ProfileImageUrl = ExtractProfileImage(html, profileUrl.Url),

            ExternalId =
                ExtractAttribute(
                    html,
                    "data-frivol-id"),

            ProfileUrl = profileUrl.Url
        };
    }

    private static string? ExtractUsername(string html)
    {
        var patterns = new[]
        {
            @"<h1[^>]*>(.*?)</h1>",
            @"<title[^>]*>(.*?)</title>"
        };

        foreach (var pattern in patterns)
        {
            var match = Regex.Match(
                html,
                pattern,
                RegexOptions.IgnoreCase |
                RegexOptions.Singleline);

            if (!match.Success)
                continue;

            var value =
                StripHtml(match.Groups[1].Value);

            value = WebUtility.HtmlDecode(value).Trim();

            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }

        return null;
    }

    private static int? ExtractInt(
        string html,
        string pattern)
    {
        var match = Regex.Match(
            html,
            pattern,
            RegexOptions.IgnoreCase |
            RegexOptions.Singleline);

        if (!match.Success)
            return null;

        var value =
            StripHtml(match.Groups[1].Value).Trim();

        var number =
            Regex.Match(value, @"\d+");

        if (!number.Success)
            return null;

        return int.TryParse(
            number.Value,
            out var result)
            ? result
            : null;
    }

    private static string? ExtractValue(
        string html,
        params string[] labels)
    {
        foreach (var label in labels)
        {
            var pattern =
                $@"{Regex.Escape(label)}\s*</[^>]+>\s*<[^>]+>\s*(.*?)</";

            var match = Regex.Match(
                html,
                pattern,
                RegexOptions.IgnoreCase |
                RegexOptions.Singleline);

            if (!match.Success)
                continue;

            var value =
                StripHtml(match.Groups[1].Value);

            value =
                WebUtility.HtmlDecode(value).Trim();

            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }

        return null;
    }

    private static string? ExtractTextByClass(
        string html,
        params string[] classNames)
    {
        foreach (var className in classNames)
        {
            var pattern =
                $@"<[^>]*class\s*=\s*[""'][^""']*\b{Regex.Escape(className)}\b[^""']*[""'][^>]*>(.*?)</[^>]+>";

            var match = Regex.Match(
                html,
                pattern,
                RegexOptions.IgnoreCase |
                RegexOptions.Singleline);

            if (!match.Success)
                continue;

            var value =
                StripHtml(match.Groups[1].Value);

            value =
                WebUtility.HtmlDecode(value).Trim();

            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }

        return null;
    }

    //private static string? ExtractProfileImage(
    //    string html)
    //{
    //    var pattern =
    //        @"<[^>]*id\s*=\s*[""']profile-header-container[""'][^>]*>.*?<img[^>]+src\s*=\s*[""']([^""']+)[""']";

    //    var match = Regex.Match(
    //        html,
    //        pattern,
    //        RegexOptions.IgnoreCase |
    //        RegexOptions.Singleline);

    //    if (!match.Success)
    //        return null;

    //    return WebUtility.HtmlDecode(
    //        match.Groups[1].Value).Trim();
    //}

    private static string? ExtractProfileImage(
    string html,
    string profileUrl)
    {
        var imagePatterns = new[]
        {
        // Preferred: image specified for previews and sharing.
        @"<meta[^>]+property\s*=\s*[""']og:image[""'][^>]+content\s*=\s*[""']([^""']+)[""']",
        @"<meta[^>]+content\s*=\s*[""']([^""']+)[""'][^>]+property\s*=\s*[""']og:image[""']",

        @"<meta[^>]+name\s*=\s*[""']twitter:image[""'][^>]+content\s*=\s*[""']([^""']+)[""']",
        @"<meta[^>]+content\s*=\s*[""']([^""']+)[""'][^>]+name\s*=\s*[""']twitter:image[""']",

        // Profile header image.
        @"id\s*=\s*[""']profile-header-container[""'][^>]*>[\s\S]*?<img\b[^>]*?\bsrc\s*=\s*[""']([^""']+)[""']",

        // Lazy-loaded images used by some websites.
        @"<img\b[^>]*?\bdata-src\s*=\s*[""']([^""']+)[""']",
        @"<img\b[^>]*?\bsrc\s*=\s*[""']([^""']+)[""']"
    };

        foreach (var pattern in imagePatterns)
        {
            var match = Regex.Match(
                html,
                pattern,
                RegexOptions.IgnoreCase |
                RegexOptions.Singleline);

            if (!match.Success)
                continue;

            var imageUrl = WebUtility.HtmlDecode(
                match.Groups[1].Value.Trim());

            if (!Uri.TryCreate(
                    new Uri(profileUrl),
                    imageUrl,
                    out var absoluteUri))
            {
                continue;
            }

            if (absoluteUri.Scheme != Uri.UriSchemeHttps &&
                absoluteUri.Scheme != Uri.UriSchemeHttp)
            {
                continue;
            }

            // Ignore embedded data and JavaScript URLs.
            if (imageUrl.StartsWith(
                    "data:",
                    StringComparison.OrdinalIgnoreCase) ||
                imageUrl.StartsWith(
                    "javascript:",
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            return absoluteUri.AbsoluteUri;
        }

        return null;
    }

    private static string? ExtractAttribute(
        string html,
        string attributeName)
    {
        var pattern =
            $@"{Regex.Escape(attributeName)}\s*=\s*[""']([^""']+)[""']";

        var match = Regex.Match(
            html,
            pattern,
            RegexOptions.IgnoreCase);

        return match.Success
            ? WebUtility.HtmlDecode(
                match.Groups[1].Value).Trim()
            : null;
    }

    private static string StripHtml(
        string value)
    {
        return Regex.Replace(
            value,
            "<.*?>",
            string.Empty);
    }

    private static string CreateSlug(
        string username)
    {
        return Regex.Replace(
                username.Trim().ToLowerInvariant(),
                @"[^a-z0-9]+",
                "-")
            .Trim('-');
    }

    private sealed class ParsedProfile
    {
        public string Username { get; set; } = null!;
        public string DisplayName { get; set; } = null!;

        public int? Age { get; set; }
        public string? Gender { get; set; }
        public string? Country { get; set; }
        public string? Region { get; set; }
        public string? Bio { get; set; }

        public string? ProfileImageUrl { get; set; }
        public string? ExternalId { get; set; }

        public string ProfileUrl { get; set; } = null!;
    }
}