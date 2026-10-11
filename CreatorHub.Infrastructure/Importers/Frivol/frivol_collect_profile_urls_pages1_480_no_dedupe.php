<?php
/*
    Frivol.com Profil-URL-Sammler
    PHP 5.6 kompatibel

    Anforderungen:
    - Start mit &page=1
    - Ende mit &page=480
    - keine Dubletten entfernen
    - am Ende Download-Link zur CSV anzeigen

    Ergebnis:
    - frivol_profile_urls.csv
    - frivol_profile_urls_log.txt
*/

set_time_limit(0);
ini_set('memory_limit', '512M');
ini_set('display_errors', 1);
error_reporting(E_ALL);

$startPage = 1;
$endPage = 480;

$pauseEveryPages = 20;
$pauseSeconds = 5;

$csvFileBasename = 'frivol_profile_urls.csv';
$logFileBasename = 'frivol_profile_urls_log.txt';

$csvFilename = __DIR__ . '/' . $csvFileBasename;
$logFilename = __DIR__ . '/' . $logFileBasename;

$baseSearchUrl = 'https://www.frivol.com/amateure/finden?member_search_form%5Bterm%5D=&member_search_form%5BmemberType%5D=amateur&member_search_form%5BmemberSex%5D=all&member_search_form%5Bregion%5D=&member_search_form%5Bonline%5D=all&member_search_form%5BprofilePhotoOnly%5D=1&member_search_form%5BminAge%5D=18&member_search_form%5BmaxAge%5D=127&member_search_form%5Bsubmit%5D=&member_search_form%5B_t%5D=a35a16dda95ea803289555d794.mVJB7Vk3Y7bWLaghU_c56-_XmHmFiU5943pupqhDZ2Y.qxwqgG1vLumTf-54Z5tNkYqF4kv23TlOriAd9vJyHQ2hahjdNwE2gJNY4Q';

function logMessage($message)
{
    global $logFilename;

    $line = date('Y-m-d H:i:s') . ' - ' . $message . PHP_EOL;

    echo htmlspecialchars($line, ENT_QUOTES, 'UTF-8') . "<br>\n";
    @ob_flush();
    @flush();

    file_put_contents($logFilename, $line, FILE_APPEND);
}

function fetchUrl($url, &$httpCode)
{
    $httpCode = 0;

    if (function_exists('curl_init')) {
        $ch = curl_init();

        curl_setopt($ch, CURLOPT_URL, $url);
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ch, CURLOPT_FOLLOWLOCATION, true);
        curl_setopt($ch, CURLOPT_CONNECTTIMEOUT, 30);
        curl_setopt($ch, CURLOPT_TIMEOUT, 120);
        curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, true);

        curl_setopt($ch, CURLOPT_HTTPHEADER, array(
            'User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120 Safari/537.36',
            'Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8',
            'Accept-Language: de-DE,de;q=0.9,en;q=0.8',
            'Referer: https://www.frivol.com/',
            'Connection: keep-alive'
        ));

        $response = curl_exec($ch);

        if ($response === false) {
            $error = curl_error($ch);
            curl_close($ch);
            return array(false, $error);
        }

        $httpCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
        curl_close($ch);

        return array($response, null);
    }

    $context = stream_context_create(array(
        'http' => array(
            'method' => 'GET',
            'timeout' => 120,
            'header' =>
                "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64)\r\n" .
                "Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8\r\n" .
                "Accept-Language: de-DE,de;q=0.9,en;q=0.8\r\n" .
                "Referer: https://www.frivol.com/\r\n"
        )
    ));

    $response = @file_get_contents($url, false, $context);

    if (isset($http_response_header) && is_array($http_response_header)) {
        foreach ($http_response_header as $header) {
            if (preg_match('/HTTP\/\S+\s+(\d+)/', $header, $matches)) {
                $httpCode = intval($matches[1]);
                break;
            }
        }
    }

    if ($response === false) {
        return array(false, 'file_get_contents fehlgeschlagen');
    }

    return array($response, null);
}

function buildPageUrl($baseSearchUrl, $page)
{
    return $baseSearchUrl . '&page=' . intval($page);
}

function normalizeProfileUrl($href)
{
    $href = html_entity_decode($href, ENT_QUOTES, 'UTF-8');

    if (strpos($href, 'http://') === 0 || strpos($href, 'https://') === 0) {
        return $href;
    }

    if (strpos($href, '/') === 0) {
        return 'https://www.frivol.com' . $href;
    }

    return 'https://www.frivol.com/' . $href;
}

function extractUsernameFromUrl($url)
{
    $path = parse_url($url, PHP_URL_PATH);

    if (!$path) {
        return '';
    }

    $parts = explode('/', trim($path, '/'));

    if (count($parts) >= 2 && $parts[0] === 'profil') {
        return urldecode($parts[1]);
    }

    return '';
}

function extractProfileUrls($html)
{
    $profiles = array();

    preg_match_all('/href=["\']([^"\']*\/profil\/[^"\']+)["\']/i', $html, $matches);

    if (!isset($matches[1])) {
        return $profiles;
    }

    foreach ($matches[1] as $href) {
        $url = normalizeProfileUrl($href);

        if (strpos($url, 'https://www.frivol.com/profil/') !== 0) {
            continue;
        }

        $url = preg_replace('/[#?].*$/', '', $url);
        $username = extractUsernameFromUrl($url);

        if ($username === '') {
            continue;
        }

        // Keine Dubletten entfernen: jeder Treffer wird übernommen.
        $profiles[] = array(
            'username' => $username,
            'profile_url' => $url
        );
    }

    return $profiles;
}

function getCurrentScriptUrl()
{
    $https = false;

    if (isset($_SERVER['HTTPS']) && $_SERVER['HTTPS'] !== 'off') {
        $https = true;
    }

    $scheme = $https ? 'https://' : 'http://';
    $host = isset($_SERVER['HTTP_HOST']) ? $_SERVER['HTTP_HOST'] : '';
    $scriptName = isset($_SERVER['SCRIPT_NAME']) ? $_SERVER['SCRIPT_NAME'] : '';

    $dir = rtrim(dirname($scriptName), '/\\');

    return $scheme . $host . $dir . '/';
}

logMessage('Frivol Profil-URL-Sammlung gestartet');

$csvHandle = fopen($csvFilename, 'w');

if (!$csvHandle) {
    die('CSV-Datei konnte nicht geöffnet werden.');
}

fwrite($csvHandle, "\xEF\xBB\xBF");
fputcsv($csvHandle, array('username', 'profile_url', 'source_page'), ';');

$totalFound = 0;

for ($page = $startPage; $page <= $endPage; $page++) {
    logMessage('Rufe Suchseite ' . $page . ' von ' . $endPage . ' ab');

    $url = buildPageUrl($baseSearchUrl, $page);

    $httpCode = 0;
    $result = fetchUrl($url, $httpCode);
    $html = $result[0];
    $error = $result[1];

    if ($html === false) {
        logMessage('Fehler auf Seite ' . $page . ': ' . $error);
        continue;
    }

    if ($httpCode != 200) {
        logMessage('HTTP ' . $httpCode . ' auf Seite ' . $page);
        continue;
    }

    $profiles = extractProfileUrls($html);
    $foundOnPage = 0;

    foreach ($profiles as $profile) {
        fputcsv($csvHandle, array(
            $profile['username'],
            $profile['profile_url'],
            $page
        ), ';');

        $totalFound++;
        $foundOnPage++;
    }

    fflush($csvHandle);

    logMessage('Profile auf Seite ' . $page . ': ' . $foundOnPage . ' | Gesamt: ' . $totalFound);

    if ($page % $pauseEveryPages == 0) {
        logMessage($pauseEveryPages . ' Seiten erreicht - ' . $pauseSeconds . ' Sekunden Pause...');
        sleep($pauseSeconds);
    } else {
        usleep(300000);
    }
}

fclose($csvHandle);

$downloadUrl = getCurrentScriptUrl() . $csvFileBasename;

logMessage('Fertig. Gespeicherte Profileinträge: ' . $totalFound);
logMessage('CSV-Datei: ' . basename($csvFilename));

echo '<hr>';
echo '<strong>Download:</strong> ';
echo '<a href="' . htmlspecialchars($downloadUrl, ENT_QUOTES, 'UTF-8') . '" target="_blank">';
echo htmlspecialchars($csvFileBasename, ENT_QUOTES, 'UTF-8');
echo '</a>';

?>