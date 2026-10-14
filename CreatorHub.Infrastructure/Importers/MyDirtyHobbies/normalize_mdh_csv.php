
<?php

$inputPath = __DIR__ . '/mydirtyhobby_amateurs_Profiles.csv';
$outputPath = __DIR__ . '/mydirtyhobby_amateurs_Profiles_normalized.csv';

if (!file_exists($inputPath)) {
    exit("ERROR: Original CSV was not found.\n");
}

/**
 * Parse one physical line from the original MDH file.
 * The profile data is semicolon-delimited inside an outer wrapper.
 */
function parseMdhLine(string $line, bool $isHeader = false): array
{
    $line = rtrim($line, "\r\n");

    if ($isHeader) {
        // The header has trailing commas that are not real columns.
        $line = rtrim($line, ',');
        return array_map('trim', explode(';', $line));
    }

    // Remove the outer opening quote.
    if (str_starts_with($line, '"')) {
        $line = substr($line, 1);
    }

    // Remove the outer closing quote and trailing comma padding.
    $line = preg_replace('/",*$/', '', $line);

    // Undo one layer of doubled-quote escaping.
    $line = str_replace('""', '"', $line);

    // Inner fields are separated by semicolons.
    return explode(';', $line);
}

$input = fopen($inputPath, 'r');

if (!$input) {
    exit("ERROR: Could not open original CSV.\n");
}

$headerLine = fgets($input);

if ($headerLine === false) {
    fclose($input);
    exit("ERROR: Original CSV is empty.\n");
}

$headers = parseMdhLine($headerLine, true);
$expectedColumns = count($headers);

if ($expectedColumns === 0 || !in_array('url', $headers, true)) {
    fclose($input);
    exit("ERROR: Could not identify the header or URL column.\n");
}

// Inspect all records and determine the largest field count.
// No profile records are discarded.
$dataRows = 0;
$blankRows = 0;
$maxColumns = $expectedColumns;

while (($line = fgets($input)) !== false) {
    if (trim($line) === '') {
        $blankRows++;
        continue;
    }

    $fields = parseMdhLine($line);
    $maxColumns = max($maxColumns, count($fields));
    $dataRows++;
}

fclose($input);

// Preserve unexpected extra fields rather than silently dropping them.
for ($i = $expectedColumns; $i < $maxColumns; $i++) {
    $headers[] = 'extra_column_' . ($i - $expectedColumns + 1);
}

$urlIndex = array_search('url', $headers, true);

$input = fopen($inputPath, 'r');
if (!$input) {
    exit("ERROR: Could not reopen original CSV.\n");
}

// Skip the original header.
fgets($input);

$output = fopen($outputPath, 'w');
if (!$output) {
    fclose($input);
    exit("ERROR: Could not create normalized CSV.\n");
}

fputcsv($output, $headers, ',', '"', '\\');

$writtenRows = 0;
$rowsWithUrl = 0;
$rowsWithoutUrl = 0;
$rowsPadded = 0;

while (($line = fgets($input)) !== false) {
    if (trim($line) === '') {
        continue;
    }

    $fields = parseMdhLine($line);

    // Pad shorter records so every row matches the output header.
    if (count($fields) < count($headers)) {
        $fields = array_pad($fields, count($headers), '');
        $rowsPadded++;
    }

    // Extra columns have already been included in the header.
    if (count($fields) > count($headers)) {
        $headersNeeded = count($fields) - count($headers);

        for ($i = 1; $i <= $headersNeeded; $i++) {
            $headers[] = 'extra_column_' . (count($headers) - $expectedColumns + 1);
        }

        // Avoid silently dropping values if an unexpected long row appears.
        fputcsv($output, $fields, ',', '"', '\\');
        $writtenRows++;

        $url = trim($fields[$urlIndex] ?? '');
        if (preg_match('/^https?:\/\//i', $url)) {
            $rowsWithUrl++;
        } else {
            $rowsWithoutUrl++;
        }

        continue;
    }

    fputcsv($output, $fields, ',', '"', '\\');
    $writtenRows++;

    $url = trim($fields[$urlIndex] ?? '');

    if (preg_match('/^https?:\/\//i', $url)) {
        $rowsWithUrl++;
    } else {
        $rowsWithoutUrl++;
    }
}

fclose($input);
fclose($output);

echo "MDH normalization completed.\n";
echo "Original data rows: $dataRows\n";
echo "Rows written: $writtenRows\n";
echo "Output columns: " . count($headers) . "\n";
echo "Rows with profile URLs: $rowsWithUrl\n";
echo "Rows without profile URLs: $rowsWithoutUrl\n";
echo "Rows padded with blank fields: $rowsPadded\n";
echo "Blank lines ignored: $blankRows\n";
echo "Output file: $outputPath\n";
