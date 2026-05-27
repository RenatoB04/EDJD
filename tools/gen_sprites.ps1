Add-Type -AssemblyName System.Drawing

$assetsRoot = Join-Path $PSScriptRoot "..\P01-TEJ\Assets.xcassets"

function New-Imageset {
    param([string]$Name)
    $dir = Join-Path $assetsRoot "$Name.imageset"
    if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }

    $contents = @"
{
  "images" : [
    {
      "idiom" : "universal",
      "filename" : "$Name@2x.png",
      "scale" : "2x"
    },
    {
      "idiom" : "universal",
      "filename" : "$Name@3x.png",
      "scale" : "3x"
    },
    {
      "idiom" : "universal",
      "scale" : "1x"
    }
  ],
  "info" : {
    "author" : "xcode",
    "version" : 1
  }
}
"@
    Set-Content -Path (Join-Path $dir "Contents.json") -Value $contents -Encoding UTF8
    return $dir
}

function Save-PNG {
    param([System.Drawing.Bitmap]$Bitmap, [string]$Path)
    $Bitmap.Save($Path, [System.Drawing.Imaging.ImageFormat]::Png)
}

function New-PlayerSprite {
    param([int]$Size, [string]$Path)
    $bmp = New-Object System.Drawing.Bitmap($Size, $Size)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.Clear([System.Drawing.Color]::Transparent)

    $s = [double]$Size

    # Flame exhaust (left side, behind rocket - rocket points right)
    $flameOuter = New-Object System.Drawing.Drawing2D.GraphicsPath
    [System.Drawing.PointF[]]$flameOuterPts = @(
        (New-Object System.Drawing.PointF([single]($s*0.00),[single]($s*0.50))),
        (New-Object System.Drawing.PointF([single]($s*0.22),[single]($s*0.30))),
        (New-Object System.Drawing.PointF([single]($s*0.18),[single]($s*0.50))),
        (New-Object System.Drawing.PointF([single]($s*0.22),[single]($s*0.70)))
    )
    $flameOuter.AddPolygon($flameOuterPts)
    $flameBrushOuter = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255,255,140,0))
    $g.FillPath($flameBrushOuter, $flameOuter)

    $flameInner = New-Object System.Drawing.Drawing2D.GraphicsPath
    [System.Drawing.PointF[]]$flameInnerPts = @(
        (New-Object System.Drawing.PointF([single]($s*0.08),[single]($s*0.50))),
        (New-Object System.Drawing.PointF([single]($s*0.22),[single]($s*0.38))),
        (New-Object System.Drawing.PointF([single]($s*0.20),[single]($s*0.50))),
        (New-Object System.Drawing.PointF([single]($s*0.22),[single]($s*0.62)))
    )
    $flameInner.AddPolygon($flameInnerPts)
    $flameBrushInner = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255,255,235,80))
    $g.FillPath($flameBrushInner, $flameInner)

    # Wings (top + bottom triangles)
    $wingBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255,40,90,140))
    $wingTop = New-Object System.Drawing.Drawing2D.GraphicsPath
    [System.Drawing.PointF[]]$wingTopPts = @(
        (New-Object System.Drawing.PointF([single]($s*0.30),[single]($s*0.34))),
        (New-Object System.Drawing.PointF([single]($s*0.55),[single]($s*0.34))),
        (New-Object System.Drawing.PointF([single]($s*0.45),[single]($s*0.18)))
    )
    $wingTop.AddPolygon($wingTopPts)
    $g.FillPath($wingBrush, $wingTop)
    $wingBot = New-Object System.Drawing.Drawing2D.GraphicsPath
    [System.Drawing.PointF[]]$wingBotPts = @(
        (New-Object System.Drawing.PointF([single]($s*0.30),[single]($s*0.66))),
        (New-Object System.Drawing.PointF([single]($s*0.55),[single]($s*0.66))),
        (New-Object System.Drawing.PointF([single]($s*0.45),[single]($s*0.82)))
    )
    $wingBot.AddPolygon($wingBotPts)
    $g.FillPath($wingBrush, $wingBot)

    # Rocket body (rounded rectangle pointing right)
    $bodyRect = [System.Drawing.RectangleF]::new([single]($s*0.22),[single]($s*0.32),[single]($s*0.55),[single]($s*0.36))
    $bodyBrush = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
        $bodyRect,
        [System.Drawing.Color]::FromArgb(255,140,220,255),
        [System.Drawing.Color]::FromArgb(255,40,130,200),
        [System.Drawing.Drawing2D.LinearGradientMode]::Vertical
    )
    $bodyPath = New-Object System.Drawing.Drawing2D.GraphicsPath
    $r = [single]($s*0.18)
    $bodyPath.AddArc([single]($s*0.22),[single]($s*0.32),$r,$r,180,90)
    $bodyPath.AddArc([single]($s*0.59),[single]($s*0.32),$r,$r,270,90)
    $bodyPath.AddArc([single]($s*0.59),[single]($s*0.50),$r,$r,0,90)
    $bodyPath.AddArc([single]($s*0.22),[single]($s*0.50),$r,$r,90,90)
    $bodyPath.CloseFigure()
    $g.FillPath($bodyBrush, $bodyPath)

    # Nose cone (triangle on right)
    $noseBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255,230,80,80))
    $nose = New-Object System.Drawing.Drawing2D.GraphicsPath
    [System.Drawing.PointF[]]$nosePts = @(
        (New-Object System.Drawing.PointF([single]($s*0.72),[single]($s*0.32))),
        (New-Object System.Drawing.PointF([single]($s*0.72),[single]($s*0.68))),
        (New-Object System.Drawing.PointF([single]($s*0.95),[single]($s*0.50)))
    )
    $nose.AddPolygon($nosePts)
    $g.FillPath($noseBrush, $nose)

    # Cockpit/window
    $windowBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255,230,250,255))
    $g.FillEllipse($windowBrush, [single]($s*0.55), [single]($s*0.42), [single]($s*0.14), [single]($s*0.16))
    $windowRim = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(255,30,80,130), [single]($s*0.02))
    $g.DrawEllipse($windowRim, [single]($s*0.55), [single]($s*0.42), [single]($s*0.14), [single]($s*0.16))

    # Highlight stripe on body
    $highlightBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(120,255,255,255))
    $g.FillRectangle($highlightBrush, [single]($s*0.30), [single]($s*0.38), [single]($s*0.40), [single]($s*0.04))

    Save-PNG -Bitmap $bmp -Path $Path
    $g.Dispose(); $bmp.Dispose()
}

function New-AsteroidSprite {
    param([int]$Size, [string]$Path)
    $bmp = New-Object System.Drawing.Bitmap($Size, $Size)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.Clear([System.Drawing.Color]::Transparent)

    $s = [double]$Size
    $cx = $s * 0.5
    $cy = $s * 0.5

    # Irregular rock silhouette (10-sided with jitter)
    $points = New-Object System.Collections.Generic.List[System.Drawing.PointF]
    $sides = 10
    $rng = New-Object System.Random(7)
    for ($i=0; $i -lt $sides; $i++) {
        $angle = ($i / $sides) * [Math]::PI * 2
        $jitter = 0.78 + ($rng.NextDouble() * 0.18)
        $r = ($s * 0.46) * $jitter
        $px = $cx + [Math]::Cos($angle) * $r
        $py = $cy + [Math]::Sin($angle) * $r
        $points.Add((New-Object System.Drawing.PointF([single]$px,[single]$py)))
    }

    $rockPath = New-Object System.Drawing.Drawing2D.GraphicsPath
    $rockPath.AddPolygon($points.ToArray())

    # Base fill - radial gradient for shading
    $rockBrush = New-Object System.Drawing.Drawing2D.PathGradientBrush($rockPath)
    $rockBrush.CenterColor = [System.Drawing.Color]::FromArgb(255,170,160,150)
    $rockBrush.SurroundColors = @([System.Drawing.Color]::FromArgb(255,85,78,72))
    $rockBrush.CenterPoint = New-Object System.Drawing.PointF([single]($cx - $s*0.10),[single]($cy - $s*0.10))
    $g.FillPath($rockBrush, $rockPath)

    # Outline
    $outline = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(255,45,40,38), [single]($s*0.025))
    $outline.LineJoin = [System.Drawing.Drawing2D.LineJoin]::Round
    $g.DrawPath($outline, $rockPath)

    # Craters
    $craterBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255,70,64,58))
    $craterRim = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255,200,190,180))
    $craters = @(
        @{ x=0.32; y=0.36; r=0.10 },
        @{ x=0.62; y=0.58; r=0.08 },
        @{ x=0.48; y=0.72; r=0.06 },
        @{ x=0.68; y=0.30; r=0.05 }
    )
    foreach ($c in $craters) {
        $cxC = [single]($s*$c.x); $cyC = [single]($s*$c.y); $cR = [single]($s*$c.r)
        # rim highlight
        $g.FillEllipse($craterRim, $cxC - $cR, $cyC - $cR, $cR*2, $cR*2)
        # inner dark
        $inset = [single]($s*0.015)
        $g.FillEllipse($craterBrush, $cxC - $cR + $inset, $cyC - $cR + $inset, ($cR*2) - ($inset*2), ($cR*2) - ($inset*2))
    }

    Save-PNG -Bitmap $bmp -Path $Path
    $g.Dispose(); $bmp.Dispose()
}

# Generate Player
$playerDir = New-Imageset -Name "player"
New-PlayerSprite -Size 80  -Path (Join-Path $playerDir "player@2x.png")
New-PlayerSprite -Size 120 -Path (Join-Path $playerDir "player@3x.png")

# Generate Asteroid
$astDir = New-Imageset -Name "asteroid"
New-AsteroidSprite -Size 60 -Path (Join-Path $astDir "asteroid@2x.png")
New-AsteroidSprite -Size 90 -Path (Join-Path $astDir "asteroid@3x.png")

Write-Output "Generated sprites:"
Get-ChildItem -Recurse -Path $assetsRoot -Filter *.png | ForEach-Object { Write-Output "  $($_.FullName)" }
