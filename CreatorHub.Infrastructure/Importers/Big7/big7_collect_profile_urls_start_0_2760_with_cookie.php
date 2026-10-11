<?php
/*
    Big7 Profil-URL-Sammler
    PHP 5.6 kompatibel

    Zweck:
    - ruft Big7-Suchergebnisseiten start=0 bis start=2760 ab
    - Schrittweite: 60
    - funktioniert mit Login-Cookie
    - extrahiert Profil-URLs nach dem Muster:
      https://www.big7.com/account/341226/beauty-of-pain.html
    - speichert alles als CSV
    - KEINE Dubletten entfernen
    - zeigt am Ende Download-Link

    Ergebnis:
    - big7_profile_urls.csv
    - big7_profile_urls_log.txt

    Wichtig:
    - $cookieHeader eintragen, sonst sieht Big7 die eingeloggten Suchseiten nicht.
*/

set_time_limit(0);
ini_set('memory_limit', '512M');
ini_set('display_errors', 1);
error_reporting(E_ALL);

// ------------------------------------------------------------
// Einstellungen
// ------------------------------------------------------------

$csvFileBasename = 'big7_profile_urls.csv';
$logFileBasename = 'big7_profile_urls_log.txt';

$csvFilename = __DIR__ . '/' . $csvFileBasename;
$logFilename = __DIR__ . '/' . $logFileBasename;

// Login-Cookie hier eintragen.
// Beispiel:
// $cookieHeader = 'PHPSESSID=abc123; anderer_cookie=xyz';
$cookieHeader = 'B7FVISIT=1750089232; CookieConsent={stamp:\'vBrciIC1pLthHeRd3iQ3DGCM/bWxJ5qH9GG7/jtyFyfBUlFvdzMR3w==\',necessary:true,preferences:true,statistics:true,marketing:true,method:\'explicit\',ver:1,utc:1782215929952,region:\'pl\'}; B7WM=0; B7WMB=0; B7WMS=0; _gcl_au=1.1.364687915.1782215929; AdultContentNoteShown=1; B7SID=ldnf4kp4da9mp3ed3uncaaks78; accepted-cookies=1; g_state={"i_l":0,"i_ll":1783419750050,"i_e":{"enable_itp_optimization":24},"i_et":1783419750050}; B7DE=N; B7SC=2fe1e3fe8bc7c5ee0f426319e00b9073; B7L=a=Y&u=8366973&m=1e3b216df6c72fd13d51e2bf873c7f64; B7_IMGT=hd; B7P=12da2432fced225f09281eca865a511b; _gid=GA1.2.254526800.1783421091; _b7_volume=off; _ga=GA1.1.1828903268.1782215929; _ga_RC2F8PDT0J=GS2.1.s1783422931$o6$g1$t1783424278$j59$l0$h0';

// Pagination
$startMin = 0;
$startMax = 2760;
$startStep = 60;

$baseSearchUrl = 'https://www.big7.com/suche_profile.html?do=single&q=&search_type=amateur';

// Pausen
$requestDelayMicroseconds = 500000; // 0,5 Sekunden
$pauseEveryPages = 10;
$pauseSeconds = 5;

// ------------------------------------------------------------
// Funktionen
// ------------------------------------------------------------

function logMessage($message)
{
    global $logFilename;

    $line = date('Y-m-d H:i:s') . ' - ' . $message . PHP_EOL;

    echo htmlspecialchars($line, ENT_QUOTES, 'UTF-8') . "<br>\n";
    @ob_flush();
    @flush();

    file_put_contents($logFilename, $line, FILE_APPEND);
}

function absoluteBig7Url($href)
{
    $href = html_entity_decode(trim($href), ENT_QUOTES, 'UTF-8');

    if ($href === '') {
        return '';
    }

    if (strpos($href, 'http://') === 0 || strpos($href, 'https://') === 0) {
        return $href;
    }

    if (strpos($href, '//') === 0) {
        return 'https:' . $href;
    }

    if (strpos($href, '/') === 0) {
        return 'https://www.big7.com' . $href;
    }

    return 'https://www.big7.com/' . $href;
}

function buildSearchUrl($baseSearchUrl, $start)
{
    return $baseSearchUrl . '&start=' . intval($start);
}

function fetchUrl($url, &$httpCode)
{
    global $cookieHeader;

    $httpCode = 0;

    if (function_exists('curl_init')) {
        $ch = curl_init();

        $headers = array(
            'User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120 Safari/537.36',
            'Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8',
            'Accept-Language: de-DE,de;q=0.9,en;q=0.8',
            'Referer: https://www.big7.com/',
            'Connection: keep-alive'
        );

        if (trim($cookieHeader) !== '') {
            $headers[] = 'Cookie: ' . $cookieHeader;
        }

        curl_setopt($ch, CURLOPT_URL, $url);
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ch, CURLOPT_FOLLOWLOCATION, true);
        curl_setopt($ch, CURLOPT_CONNECTTIMEOUT, 30);
        curl_setopt($ch, CURLOPT_TIMEOUT, 120);
        curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, true);
        curl_setopt($ch, CURLOPT_HTTPHEADER, $headers);

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

    $header =
        "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64)\r\n" .
        "Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8\r\n" .
        "Accept-Language: de-DE,de;q=0.9,en;q=0.8\r\n" .
        "Referer: https://www.big7.com/\r\n";

    if (trim($cookieHeader) !== '') {
        $header .= "Cookie: " . $cookieHeader . "\r\n";
    }

    $context = stream_context_create(array(
        'http' => array(
            'method' => 'GET',
            'timeout' => 120,
            'header' => $header
        )
    ));

    $response = @file_get_contents($url, false, $context);

    if (isset($http_response_header) && is_array($http_response_header)) {
        foreach ($http_response_header as $h) {
            if (preg_match('/HTTP\/\S+\s+(\d+)/', $h, $m)) {
                $httpCode = intval($m[1]);
                break;
            }
        }
    }

    if ($response === false) {
        return array(false, 'file_get_contents fehlgeschlagen');
    }

    return array($response, null);
}

function extractBig7ProfileDataFromUrl($url)
{
    $data = array(
        'profile_id' => '',
        'username_slug' => '',
        'profile_url' => $url
    );

    $path = parse_url($url, PHP_URL_PATH);

    if (!$path) {
        return $data;
    }

    // Muster: /account/341226/beauty-of-pain.html
    if (preg_match('#/account/([0-9]+)/([^/]+)\.html#i', $path, $m)) {
        $data['profile_id'] = $m[1];
        $data['username_slug'] = preg_replace('/\.html$/i', '', $m[2]);
    }

    return $data;
}

function extractProfileUrls($html)
{
    $profiles = array();

    preg_match_all('/href=["\']([^"\']+)["\']/i', $html, $matches);

    if (!isset($matches[1])) {
        return $profiles;
    }

    foreach ($matches[1] as $href) {
        $url = absoluteBig7Url($href);

        // Query und Anker entfernen
        $url = preg_replace('/[#?].*$/', '', $url);

        if (preg_match('#^https://www\.big7\.com/account/[0-9]+/[^/]+\.html$#i', $url)) {
            $profiles[] = extractBig7ProfileDataFromUrl($url);
        }
    }

    return $profiles;
}

function getCurrentScriptBaseUrl()
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

// ------------------------------------------------------------
// Start
// ------------------------------------------------------------

logMessage('Big7 Profil-URL-Sammlung gestartet');

$csvHandle = fopen($csvFilename, 'w');

if (!$csvHandle) {
    die('CSV-Datei konnte nicht geöffnet werden.');
}

fwrite($csvHandle, "\xEF\xBB\xBF");
fputcsv($csvHandle, array('profile_id', 'username_slug', 'profile_url', 'source_start'), ';');

$totalFound = 0;
$pageCounter = 0;

for ($start = $startMin; $start <= $startMax; $start += $startStep) {
    $pageCounter++;

    $url = buildSearchUrl($baseSearchUrl, $start);

    logMessage('Rufe Suchseite start=' . $start . ' ab: ' . $url);

    $httpCode = 0;
    $result = fetchUrl($url, $httpCode);
    $html = $result[0];
    $error = $result[1];

    if ($html === false) {
        logMessage('Fehler bei start=' . $start . ': ' . $error);
        continue;
    }

    if ($httpCode != 200) {
        logMessage('HTTP ' . $httpCode . ' bei start=' . $start);
        continue;
    }

    $profiles = extractProfileUrls($html);
    $foundOnPage = 0;

    foreach ($profiles as $profile) {
        fputcsv($csvHandle, array(
            $profile['profile_id'],
            $profile['username_slug'],
            $profile['profile_url'],
            $start
        ), ';');

        $totalFound++;
        $foundOnPage++;
    }

    fflush($csvHandle);

    logMessage('Profile auf start=' . $start . ': ' . $foundOnPage . ' | Gesamt: ' . $totalFound);

    if ($pageCounter % $pauseEveryPages == 0) {
        logMessage($pauseEveryPages . ' Suchseiten erreicht - ' . $pauseSeconds . ' Sekunden Pause...');
        sleep($pauseSeconds);
    } else {
        usleep($requestDelayMicroseconds);
    }
}

fclose($csvHandle);

$downloadUrl = getCurrentScriptBaseUrl() . $csvFileBasename;

logMessage('Fertig. Gespeicherte Profileinträge: ' . $totalFound);
logMessage('CSV-Datei: ' . $csvFileBasename);

echo '<hr>';
echo '<strong>Download:</strong> ';
echo '<a href="' . htmlspecialchars($downloadUrl, ENT_QUOTES, 'UTF-8') . '" target="_blank">';
echo htmlspecialchars($csvFileBasename, ENT_QUOTES, 'UTF-8');
echo '</a>';

?>