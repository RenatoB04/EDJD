# Astro Drift

Endless runner em estilo Jetpack Joyride, ambientado no espaço.
Toca para subir, larga para descer, foge aos asteróides e lasers, apanha moedas e escudos.

**Plataforma:** iOS — SpriteKit + Swift
**Sessão típica:** 30–90 segundos
**Input:** um toque (tap/hold = empuxo, release = queda)

## Como correr

1. Abrir `P01-TEJ.xcodeproj` no Xcode (15+)
2. Selecionar um simulador iOS (ou device)
3. ⌘R

> Se houver erros após pull, faz **Product → Clean Build Folder** (`Shift+Cmd+K`).

## Features

### Core
- Voo com gravidade + empuxo, limites no topo/fundo do ecrã
- Score por distância + recorde persistente (UserDefaults)
- Rampa de dificuldade logarítmica
- Parallax de 3 camadas de estrelas

### Gameplay
- **Asteróides** rotativos (asset)
- **Lasers** verticais pulsantes (25% chance no spawn)
- **Moedas** com carteira persistente
- **Escudo** apanhável (5s) — consome 1 colisão
- **Multiplicador de score** por streak de moedas (5 seguidas → +1x até x3)

### Polish
- Partículas de jetpack (cor depende da skin)
- Screen shake + flash + explosão na morte
- Haptic feedback (moeda / escudo / morte / botões)
- SFX procedurais (moeda, hit, escudo, botão) + música ambient em loop

### Meta
- **Loja** com 4 skins (3 compráveis: vermelho 50, dourado 200, neon 500)
- **Definições** com toggles de música, SFX, vibração + reset de progresso
- **Tutorial** na primeira sessão
- **Pausa** com botão na HUD + auto-pausa quando app vai para background

## Estrutura

```
P01-TEJ/
├── AppDelegate.swift
├── GameViewController.swift     # entry point
│
│ Scenes
├── MenuScene.swift              # menu principal
├── GameScene.swift              # loop de jogo
├── ShopScene.swift              # loja de skins
├── SettingsScene.swift          # toggles + reset
│
│ Overlays
├── GameOverOverlay.swift
├── PauseOverlay.swift
├── TutorialOverlay.swift
│
│ Gameplay nodes
├── PlayerNode.swift
├── CoinNode.swift / CoinSpawner.swift
├── ObstacleSpawner.swift        # asteróides
├── LaserSpawner.swift           # 2º tipo de obstáculo
├── ShieldNode.swift / ShieldSpawner.swift
│
│ Systems
├── DifficultyManager.swift      # rampa logarítmica
├── StarField.swift              # parallax background
├── AudioManager.swift           # música + SFX
├── HapticsManager.swift         # taptic engine
├── Effects.swift                # partículas, shake, flash
│
│ Domain
├── Skin.swift                   # Skin + SkinCatalog + PlayerInventory
├── Constants.swift              # config + IDs
│
├── Assets.xcassets              # sprites (4 skins + asteroide)
└── Sounds/                      # .wav SFX + music
```

## Ferramentas

- `tools/gen_sprites.ps1` — gera os PNG base do `player` e `asteroid`
- `tools/gen_skins.ps1`   — gera 3 variantes de skins (vermelha, dourada, neon)
- `tools/gen_sounds.ps1`  — gera 6 .wav procedurais (SFX + música ambient)

Todos correm em PowerShell + System.Drawing/.NET (Windows).

## Roadmap futuro

- Game Center: leaderboard de distância + moedas totais
- Achievements ("Primeiras 100 moedas", "1km", "Sobreviveu 1 min")
- Daily mission com bónus
- Localização EN/PT (tudo PT hardcoded por agora)
- Substituir SFX procedurais por sons "de verdade" (freesound.org / sfxr)
