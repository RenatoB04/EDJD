# Astro Drift

Endless runner em estilo Jetpack Joyride, ambientado no espaco.
Toca para subir, larga para descer, foge aos asteroides e lasers, apanha moedas e escudos.

**Plataforma:** iOS - SpriteKit + Swift
**Sessao tipica:** 30-90 segundos
**Input:** um toque (tap/hold = empuxo, release = queda)

## Como correr

1. Abrir `P01-TEJ.xcodeproj` no Xcode (15+)
2. Selecionar um simulador iOS (ou device)
3. Run

> Se houver erros apos pull, faz **Product > Clean Build Folder** (`Shift+Cmd+K`).

## Features

### Core
- Voo com gravidade + empuxo, limites no topo/fundo do ecra
- Score por distancia + recorde persistente (UserDefaults)
- Rampa de dificuldade logaritmica
- Parallax de 3 camadas de estrelas

### Gameplay
- **Asteroides** rotativos (asset)
- **Lasers** verticais pulsantes (25% chance no spawn)
- **Moedas** com carteira persistente
- **Escudo** apanhavel (5s) - consome 1 colisao
- **Multiplicador de score** por streak de moedas (5 seguidas -> +1x ate x3)

### Polish
- Particulas de jetpack (cor depende da skin)
- Screen shake + flash + explosao na morte
- SFX procedurais (moeda, hit, escudo, botao) + musica ambient em loop

### Meta
- **Loja** com 4 skins (3 compraveis: vermelho 50, dourado 200, neon 500)
- **Definicoes** com toggles de musica, SFX + reset de progresso
- **Pausa** com botao na HUD + auto-pausa quando app vai para background

## Estrutura

```text
P01-TEJ/
|-- AppDelegate.swift
|-- GameViewController.swift
|
|-- Scenes
|-- MenuScene.swift
|-- GameScene.swift
|-- ShopScene.swift
|-- SettingsScene.swift
|
|-- Overlays
|-- GameOverOverlay.swift
|-- PauseOverlay.swift
|
|-- Gameplay nodes
|-- PlayerNode.swift
|-- CoinNode.swift / CoinSpawner.swift
|-- ObstacleSpawner.swift
|-- LaserSpawner.swift
|-- ShieldNode.swift / ShieldSpawner.swift
|
|-- Systems
|-- DifficultyManager.swift
|-- StarField.swift
|-- AudioManager.swift
|-- Effects.swift
|
|-- Domain
|-- Skin.swift
|-- Constants.swift
|
|-- Assets.xcassets
`-- Sounds/
```

## Ferramentas

- `tools/gen_sprites.ps1` - gera os PNG base do `player` e `asteroid`
- `tools/gen_skins.ps1` - gera 3 variantes de skins (vermelha, dourada, neon)
- `tools/gen_sounds.ps1` - gera 6 .wav procedurais (SFX + musica ambient)

Todos correm em PowerShell + System.Drawing/.NET (Windows).

## Roadmap futuro

- Game Center: leaderboard de distancia + moedas totais
- Achievements ("Primeiras 100 moedas", "1km", "Sobreviveu 1 min")
- Daily mission com bonus
- Localizacao EN/PT (tudo PT hardcoded por agora)
- Substituir SFX procedurais por sons "de verdade" (freesound.org / sfxr)
