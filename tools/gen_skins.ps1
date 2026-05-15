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

function New-PlayerSpriteVariant {
    param(
        [int]$Size,
        [string]$Path,
        [System.Drawing.Color]$BodyTop,
        [System.Drawing.Color]$BodyBottom,
        [System.Drawing.Color]$Wing,
        [System.Drawing.Color]$Nose,
        [System.Drawing.Color]$FlameOuter,
        [System.Drawing.Color]$FlameInner,
        [System.Drawing.Color]$WindowFill
    )
    $bmp = New-Object System.Drawing.Bitmap($Size, $Size)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.Clear([System.Drawing.Color]::Transparent)

    $s = [double]$Size

    # Flame
    $pathFlameOuter = New-Object System.Drawing.Drawing2D.GraphicsPath
    [System.Drawing.PointF[]]$pts1 = @(
        (New-Object System.Drawing.PointF([single]($s*0.00),[single]($s*0.50))),
        (New-Object System.Drawing.PointF([single]($s*0.22),[single]($s*0.30))),
        (New-Object System.Drawing.PointF([single]($s*0.18),[single]($s*0.50))),
        (New-Object System.Drawing.PointF([single]($s*0.22),[single]($s*0.70)))
    )
    $pathFlameOuter.AddPolygon($pts1)
    $g.FillPath((New-Object System.Drawing.SolidBrush($FlameOuter)), $pathFlameOuter)

    $pathFlameInner = New-Object System.Drawing.Drawing2D.GraphicsPath
    [System.Drawing.PointF[]]$pts2 = @(
        (New-Object System.Drawing.PointF([single]($s*0.08),[single]($s*0.50))),
        (New-Object System.Drawing.PointF([single]($s*0.22),[single]($s*0.38))),
        (New-Object System.Drawing.PointF([single]($s*0.20),[single]($s*0.50))),
        (New-Object System.Drawing.PointF([single]($s*0.22),[single]($s*0.62)))
    )
    $pathFlameInner.AddPolygon($pts2)
    $g.FillPath((New-Object System.Drawing.SolidBrush($FlameInner)), $pathFlameInner)

    # Wings
    $brushWing = New-Object System.Drawing.SolidBrush($Wing)
    $pathWingTop = New-Object System.Drawing.Drawing2D.GraphicsPath
    [System.Drawing.PointF[]]$pts3 = @(
        (New-Object System.Drawing.PointF([single]($s*0.30),[single]($s*0.34))),
        (New-Object System.Drawing.PointF([single]($s*0.55),[single]($s*0.34))),
        (New-Object System.Drawing.PointF([single]($s*0.45),[single]($s*0.18)))
    )
    $pathWingTop.AddPolygon($pts3)
    $g.FillPath($brushWing, $pathWingTop)

    $pathWingBot = New-Object System.Drawing.Drawing2D.GraphicsPath
    [System.Drawing.PointF[]]$pts4 = @(
        (New-Object System.Drawing.PointF([single]($s*0.30),[single]($s*0.66))),
        (New-Object System.Drawing.PointF([single]($s*0.55),[single]($s*0.66))),
        (New-Object System.Drawing.PointF([single]($s*0.45),[single]($s*0.82)))
    )
    $pathWingBot.AddPolygon($pts4)
    $g.FillPath($brushWing, $pathWingBot)

    # Body
    $bodyRect = [System.Drawing.RectangleF]::new([single]($s*0.22),[single]($s*0.32),[single]($s*0.55),[single]($s*0.36))
    $brushBody = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
        $bodyRect, $BodyTop, $BodyBottom,
        [System.Drawing.Drawing2D.LinearGradientMode]::Vertical
    )
    $pathBody = New-Object System.Drawing.Drawing2D.GraphicsPath
    $r = [single]($s*0.18)
    $pathBody.AddArc([single]($s*0.22),[single]($s*0.32),$r,$r,180,90)
    $pathBody.AddArc([single]($s*0.59),[single]($s*0.32),$r,$r,270,90)
    $pathBody.AddArc([single]($s*0.59),[single]($s*0.50),$r,$r,0,90)
    $pathBody.AddArc([single]($s*0.22),[single]($s*0.50),$r,$r,90,90)
    $pathBody.CloseFigure()
    $g.FillPath($brushBody, $pathBody)

    # Nose
    $brushNose = New-Object System.Drawing.SolidBrush($Nose)
    $pathNose = New-Object System.Drawing.Drawing2D.GraphicsPath
    [System.Drawing.PointF[]]$pts5 = @(
        (New-Object System.Drawing.PointF([single]($s*0.72),[single]($s*0.32))),
        (New-Object System.Drawing.PointF([single]($s*0.72),[single]($s*0.68))),
        (New-Object System.Drawing.PointF([single]($s*0.95),[single]($s*0.50)))
    )
    $pathNose.AddPolygon($pts5)
    $g.FillPath($brushNose, $pathNose)

    # Window
    $brushWindow = New-Object System.Drawing.SolidBrush($WindowFill)
    $g.FillEllipse($brushWindow, [single]($s*0.55), [single]($s*0.42), [single]($s*0.14), [single]($s*0.16))
    $penWindowRim = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(255,30,30,30), [single]($s*0.02))
    $g.DrawEllipse($penWindowRim, [single]($s*0.55), [single]($s*0.42), [single]($s*0.14), [single]($s*0.16))

    # Highlight stripe
    $brushHighlight = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(120,255,255,255))
    $g.FillRectangle($brushHighlight, [single]($s*0.30), [single]($s*0.38), [single]($s*0.40), [single]($s*0.04))

    $bmp.Save($Path, [System.Drawing.Imaging.ImageFormat]::Png)
    $g.Dispose(); $bmp.Dispose()
}

# Skin: Red Falcon
$skin = "player_red"
$dir = New-Imageset -Name $skin
New-PlayerSpriteVariant -Size 80  -Path (Join-Path $dir "$skin@2x.png") `
    -BodyTop ([System.Drawing.Color]::FromArgb(255,255,140,140)) `
    -BodyBottom ([System.Drawing.Color]::FromArgb(255,180,30,30)) `
    -Wing ([System.Drawing.Color]::FromArgb(255,90,20,20)) `
    -Nose ([System.Drawing.Color]::FromArgb(255,60,60,60)) `
    -FlameOuter ([System.Drawing.Color]::FromArgb(255,255,140,0)) `
    -FlameInner ([System.Drawing.Color]::FromArgb(255,255,235,80)) `
    -WindowFill ([System.Drawing.Color]::FromArgb(255,255,230,200))
New-PlayerSpriteVariant -Size 120 -Path (Join-Path $dir "$skin@3x.png") `
    -BodyTop ([System.Drawing.Color]::FromArgb(255,255,140,140)) `
    -BodyBottom ([System.Drawing.Color]::FromArgb(255,180,30,30)) `
    -Wing ([System.Drawing.Color]::FromArgb(255,90,20,20)) `
    -Nose ([System.Drawing.Color]::FromArgb(255,60,60,60)) `
    -FlameOuter ([System.Drawing.Color]::FromArgb(255,255,140,0)) `
    -FlameInner ([System.Drawing.Color]::FromArgb(255,255,235,80)) `
    -WindowFill ([System.Drawing.Color]::FromArgb(255,255,230,200))

# Skin: Gold
$skin = "player_gold"
$dir = New-Imageset -Name $skin
New-PlayerSpriteVariant -Size 80  -Path (Join-Path $dir "$skin@2x.png") `
    -BodyTop ([System.Drawing.Color]::FromArgb(255,255,235,140)) `
    -BodyBottom ([System.Drawing.Color]::FromArgb(255,200,150,30)) `
    -Wing ([System.Drawing.Color]::FromArgb(255,120,80,10)) `
    -Nose ([System.Drawing.Color]::FromArgb(255,180,30,30)) `
    -FlameOuter ([System.Drawing.Color]::FromArgb(255,255,200,40)) `
    -FlameInner ([System.Drawing.Color]::FromArgb(255,255,255,200)) `
    -WindowFill ([System.Drawing.Color]::FromArgb(255,255,250,220))
New-PlayerSpriteVariant -Size 120 -Path (Join-Path $dir "$skin@3x.png") `
    -BodyTop ([System.Drawing.Color]::FromArgb(255,255,235,140)) `
    -BodyBottom ([System.Drawing.Color]::FromArgb(255,200,150,30)) `
    -Wing ([System.Drawing.Color]::FromArgb(255,120,80,10)) `
    -Nose ([System.Drawing.Color]::FromArgb(255,180,30,30)) `
    -FlameOuter ([System.Drawing.Color]::FromArgb(255,255,200,40)) `
    -FlameInner ([System.Drawing.Color]::FromArgb(255,255,255,200)) `
    -WindowFill ([System.Drawing.Color]::FromArgb(255,255,250,220))

# Skin: Neon Purple
$skin = "player_neon"
$dir = New-Imageset -Name $skin
New-PlayerSpriteVariant -Size 80  -Path (Join-Path $dir "$skin@2x.png") `
    -BodyTop ([System.Drawing.Color]::FromArgb(255,220,140,255)) `
    -BodyBottom ([System.Drawing.Color]::FromArgb(255,110,30,180)) `
    -Wing ([System.Drawing.Color]::FromArgb(255,60,10,90)) `
    -Nose ([System.Drawing.Color]::FromArgb(255,255,60,180)) `
    -FlameOuter ([System.Drawing.Color]::FromArgb(255,255,60,200)) `
    -FlameInner ([System.Drawing.Color]::FromArgb(255,255,180,255)) `
    -WindowFill ([System.Drawing.Color]::FromArgb(255,200,255,255))
New-PlayerSpriteVariant -Size 120 -Path (Join-Path $dir "$skin@3x.png") `
    -BodyTop ([System.Drawing.Color]::FromArgb(255,220,140,255)) `
    -BodyBottom ([System.Drawing.Color]::FromArgb(255,110,30,180)) `
    -Wing ([System.Drawing.Color]::FromArgb(255,60,10,90)) `
    -Nose ([System.Drawing.Color]::FromArgb(255,255,60,180)) `
    -FlameOuter ([System.Drawing.Color]::FromArgb(255,255,60,200)) `
    -FlameInner ([System.Drawing.Color]::FromArgb(255,255,180,255)) `
    -WindowFill ([System.Drawing.Color]::FromArgb(255,200,255,255))

Write-Output "Generated skin variants:"
Get-ChildItem -Recurse -Path $assetsRoot -Filter "player_*.png" | ForEach-Object { Write-Output "  $($_.Name)" }
