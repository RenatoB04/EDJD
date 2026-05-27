param(
    [string]$OutDir = (Join-Path $PSScriptRoot "..\P01-TEJ\Sounds")
)

if (-not (Test-Path $OutDir)) { New-Item -ItemType Directory -Path $OutDir -Force | Out-Null }

$sampleRate = 44100
$bitsPerSample = 16
$channels = 1

function Write-Wav {
    param(
        [Parameter(Mandatory)] [int16[]]$Samples,
        [Parameter(Mandatory)] [string]$Path
    )

    $dataSize = $Samples.Length * 2
    $byteRate = $sampleRate * $channels * ($bitsPerSample / 8)
    $blockAlign = $channels * ($bitsPerSample / 8)

    $stream = New-Object System.IO.FileStream($Path, [System.IO.FileMode]::Create)
    $writer = New-Object System.IO.BinaryWriter($stream)

    $writer.Write([System.Text.Encoding]::ASCII.GetBytes("RIFF"))
    $writer.Write([uint32](36 + $dataSize))
    $writer.Write([System.Text.Encoding]::ASCII.GetBytes("WAVE"))
    $writer.Write([System.Text.Encoding]::ASCII.GetBytes("fmt "))
    $writer.Write([uint32]16)
    $writer.Write([uint16]1)
    $writer.Write([uint16]$channels)
    $writer.Write([uint32]$sampleRate)
    $writer.Write([uint32]$byteRate)
    $writer.Write([uint16]$blockAlign)
    $writer.Write([uint16]$bitsPerSample)
    $writer.Write([System.Text.Encoding]::ASCII.GetBytes("data"))
    $writer.Write([uint32]$dataSize)
    foreach ($sample in $Samples) { $writer.Write([int16]$sample) }

    $writer.Close()
    $stream.Close()
}

function To-Int16 {
    param([double]$Value)

    $clamped = [Math]::Max(-1.0, [Math]::Min(1.0, $Value))
    return [int16]([Math]::Round($clamped * 32000))
}

function Gen-Button {
    $duration = 0.06
    $n = [int]($sampleRate * $duration)
    $samples = New-Object int16[] $n

    for ($i = 0; $i -lt $n; $i++) {
        $t = $i / [double]$sampleRate
        $progress = $i / [double]$n
        $env = [Math]::Exp(-30 * $progress)
        $samples[$i] = To-Int16 ([Math]::Sin(2 * [Math]::PI * 1800 * $t) * $env * 0.5)
    }

    Write-Wav -Samples $samples -Path (Join-Path $OutDir "button.wav")
}

function Gen-Hit {
    $duration = 0.45
    $n = [int]($sampleRate * $duration)
    $samples = New-Object int16[] $n
    $rng = New-Object System.Random(42)

    for ($i = 0; $i -lt $n; $i++) {
        $t = $i / [double]$sampleRate
        $progress = $i / [double]$n
        $noise = (($rng.NextDouble() * 2) - 1) * [Math]::Exp(-8 * $progress) * 0.6
        $thud = [Math]::Sin(2 * [Math]::PI * 80 * $t) * [Math]::Exp(-5 * $progress) * 0.8
        $samples[$i] = To-Int16 (($noise + $thud) * 0.7)
    }

    Write-Wav -Samples $samples -Path (Join-Path $OutDir "hit.wav")
}

function Gen-Thrust {
    $duration = 0.3
    $n = [int]($sampleRate * $duration)
    $samples = New-Object int16[] $n
    $rng = New-Object System.Random(7)
    $prev = 0.0

    for ($i = 0; $i -lt $n; $i++) {
        $noise = ($rng.NextDouble() * 2) - 1
        $prev = $prev + 0.06 * ($noise - $prev)
        $samples[$i] = To-Int16 ($prev * 0.4)
    }

    Write-Wav -Samples $samples -Path (Join-Path $OutDir "thrust.wav")
}

function Gen-Music {
    $duration = 8.0
    $n = [int]($sampleRate * $duration)
    $samples = New-Object int16[] $n
    $notes = @(65.41, 155.56, 196.00, 233.08)

    for ($i = 0; $i -lt $n; $i++) {
        $t = $i / [double]$sampleRate
        $sum = 0.0
        foreach ($f in $notes) {
            $sum += [Math]::Sin(2 * [Math]::PI * $f * $t)
        }
        $lfo = 0.55 + 0.25 * [Math]::Sin(2 * [Math]::PI * 0.125 * $t)
        $samples[$i] = To-Int16 (($sum / $notes.Length) * $lfo * 0.32)
    }

    Write-Wav -Samples $samples -Path (Join-Path $OutDir "music.wav")
}

Gen-Button
Gen-Hit
Gen-Thrust
Gen-Music

Write-Output "Generated sounds:"
Get-ChildItem -Path $OutDir -Filter *.wav | ForEach-Object { Write-Output "  $($_.Name) ($($_.Length) bytes)" }
