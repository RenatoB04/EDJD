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
    $dataSize  = $Samples.Length * 2
    $byteRate  = $sampleRate * $channels * ($bitsPerSample / 8)
    $blockAlign = $channels * ($bitsPerSample / 8)

    $stream = New-Object System.IO.FileStream($Path, [System.IO.FileMode]::Create)
    $writer = New-Object System.IO.BinaryWriter($stream)

    # RIFF header
    $writer.Write([System.Text.Encoding]::ASCII.GetBytes("RIFF"))
    $writer.Write([uint32](36 + $dataSize))
    $writer.Write([System.Text.Encoding]::ASCII.GetBytes("WAVE"))

    # fmt chunk
    $writer.Write([System.Text.Encoding]::ASCII.GetBytes("fmt "))
    $writer.Write([uint32]16)
    $writer.Write([uint16]1)              # PCM
    $writer.Write([uint16]$channels)
    $writer.Write([uint32]$sampleRate)
    $writer.Write([uint32]$byteRate)
    $writer.Write([uint16]$blockAlign)
    $writer.Write([uint16]$bitsPerSample)

    # data chunk
    $writer.Write([System.Text.Encoding]::ASCII.GetBytes("data"))
    $writer.Write([uint32]$dataSize)
    foreach ($s in $Samples) { $writer.Write([int16]$s) }

    $writer.Close(); $stream.Close()
}

function To-Int16 {
    param([double]$v)
    $clamped = [Math]::Max(-1.0, [Math]::Min(1.0, $v))
    return [int16]([Math]::Round($clamped * 32000))
}

# --- Coin: sweep up two short blips ---
function Gen-Coin {
    $duration = 0.22
    $n = [int]($sampleRate * $duration)
    $samples = New-Object int16[] $n
    for ($i = 0; $i -lt $n; $i++) {
        $t = $i / [double]$sampleRate
        $progress = $i / [double]$n
        $freq = if ($progress -lt 0.45) { 880 } else { 1320 }
        $env = if ($progress -lt 0.45) {
            [Math]::Exp(-12 * $progress)
        } elseif ($progress -lt 0.5) {
            0.0
        } else {
            [Math]::Exp(-10 * ($progress - 0.5))
        }
        $v = [Math]::Sin(2 * [Math]::PI * $freq * $t) * $env * 0.55
        $samples[$i] = (To-Int16 $v)
    }
    Write-Wav -Samples $samples -Path (Join-Path $OutDir "coin.wav")
}

# --- Hit: noise burst + low thud ---
function Gen-Hit {
    $duration = 0.45
    $n = [int]($sampleRate * $duration)
    $samples = New-Object int16[] $n
    $rng = New-Object System.Random(42)
    for ($i = 0; $i -lt $n; $i++) {
        $t = $i / [double]$sampleRate
        $progress = $i / [double]$n
        $envNoise = [Math]::Exp(-8 * $progress)
        $noise = (($rng.NextDouble() * 2) - 1) * $envNoise * 0.6
        $envThud = [Math]::Exp(-5 * $progress)
        $thud = [Math]::Sin(2 * [Math]::PI * 80 * $t) * $envThud * 0.8
        $v = ($noise + $thud) * 0.7
        $samples[$i] = (To-Int16 $v)
    }
    Write-Wav -Samples $samples -Path (Join-Path $OutDir "hit.wav")
}

# --- Button tap: short high click ---
function Gen-Button {
    $duration = 0.06
    $n = [int]($sampleRate * $duration)
    $samples = New-Object int16[] $n
    for ($i = 0; $i -lt $n; $i++) {
        $t = $i / [double]$sampleRate
        $progress = $i / [double]$n
        $env = [Math]::Exp(-30 * $progress)
        $v = [Math]::Sin(2 * [Math]::PI * 1800 * $t) * $env * 0.5
        $samples[$i] = (To-Int16 $v)
    }
    Write-Wav -Samples $samples -Path (Join-Path $OutDir "button.wav")
}

# --- Shield pickup: rising arp ---
function Gen-Shield {
    $duration = 0.35
    $n = [int]($sampleRate * $duration)
    $samples = New-Object int16[] $n
    $freqs = @(523.25, 659.25, 783.99, 1046.5)
    for ($i = 0; $i -lt $n; $i++) {
        $t = $i / [double]$sampleRate
        $progress = $i / [double]$n
        $step = [Math]::Min($freqs.Length - 1, [int]($progress * $freqs.Length))
        $freq = $freqs[$step]
        $envStep = $progress * $freqs.Length - $step
        $env = [Math]::Exp(-3 * $envStep)
        $v = [Math]::Sin(2 * [Math]::PI * $freq * $t) * $env * 0.45
        $samples[$i] = (To-Int16 $v)
    }
    Write-Wav -Samples $samples -Path (Join-Path $OutDir "shield.wav")
}

# --- Thrust loop: filtered noise rumble (300ms loopable) ---
function Gen-Thrust {
    $duration = 0.3
    $n = [int]($sampleRate * $duration)
    $samples = New-Object int16[] $n
    $rng = New-Object System.Random(7)
    $prev = 0.0
    for ($i = 0; $i -lt $n; $i++) {
        $noise = ($rng.NextDouble() * 2) - 1
        # 1-pole lowpass for rumble
        $prev = $prev + 0.06 * ($noise - $prev)
        $v = $prev * 0.4
        $samples[$i] = (To-Int16 $v)
    }
    # Apply 5ms fade in/out at edges for clean looping
    $fadeSamples = [int]($sampleRate * 0.005)
    for ($i = 0; $i -lt $fadeSamples; $i++) {
        $factor = $i / [double]$fadeSamples
        $samples[$i] = [int16]([Math]::Round($samples[$i] * $factor))
        $idx = $n - 1 - $i
        $samples[$idx] = [int16]([Math]::Round($samples[$idx] * $factor))
    }
    Write-Wav -Samples $samples -Path (Join-Path $OutDir "thrust.wav")
}

# --- Music: ambient 8s loop (slow chord pad) ---
function Gen-Music {
    $duration = 8.0
    $n = [int]($sampleRate * $duration)
    $samples = New-Object int16[] $n
    # C minor pad: C2, Eb3, G3, Bb3 - slow LFO on amplitude
    $notes = @(65.41, 155.56, 196.00, 233.08)
    for ($i = 0; $i -lt $n; $i++) {
        $t = $i / [double]$sampleRate
        $sum = 0.0
        foreach ($f in $notes) {
            $sum += [Math]::Sin(2 * [Math]::PI * $f * $t)
        }
        $sum = $sum / $notes.Length
        # slow LFO
        $lfo = 0.55 + 0.25 * [Math]::Sin(2 * [Math]::PI * 0.125 * $t)
        # gentle filtering via averaging would help; simple shape
        $v = $sum * $lfo * 0.32
        $samples[$i] = (To-Int16 $v)
    }
    # 50ms crossfade at edges for seamless loop
    $fadeSamples = [int]($sampleRate * 0.05)
    for ($i = 0; $i -lt $fadeSamples; $i++) {
        $factor = $i / [double]$fadeSamples
        $samples[$i] = [int16]([Math]::Round($samples[$i] * $factor))
        $idx = $n - 1 - $i
        $samples[$idx] = [int16]([Math]::Round($samples[$idx] * $factor))
    }
    Write-Wav -Samples $samples -Path (Join-Path $OutDir "music.wav")
}

Gen-Coin
Gen-Hit
Gen-Button
Gen-Shield
Gen-Thrust
Gen-Music

Write-Output "Generated sounds:"
Get-ChildItem -Path $OutDir -Filter *.wav | ForEach-Object { Write-Output "  $($_.Name) ($($_.Length) bytes)" }
