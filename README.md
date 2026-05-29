# Astro Drift

Endless runner em estilo Jetpack Joyride, ambientado no espaco.
Toca para subir, larga para descer, evita asteroides e lasers, e tenta bater o recorde.

**Plataforma:** iOS - SpriteKit + Swift
**Sessao tipica:** 30-90 segundos
**Input:** um toque (tap/hold = empuxo, release = queda)

## Como correr

1. Abrir `P01-TEJ.xcodeproj` no Xcode (15+)
2. Selecionar um simulador iOS ou device
3. Run

> Se houver erros apos pull, faz **Product > Clean Build Folder** (`Shift+Cmd+K`).

## Features

### Core
- Voo com gravidade + empuxo
- Obstaculos a entrar pela direita
- Score por distancia
- Moedas colecionaveis com carteira persistente
- Continue uma vez por run por 20 moedas
- Recorde persistente com `UserDefaults`
- Menu, retry e game over

### Polish
- Asteroides rotativos
- Lasers como segundo obstaculo
- Rampa gradual de dificuldade
- Fundo simples com estrelas
- Chama simples no jetpack
- Screen shake e flash na morte
- Musica de fundo e efeitos sonoros simples
- Pausa com botao na HUD

## Estrutura

```text
P01-TEJ/
|-- AppDelegate.swift
|-- GameViewController.swift
|-- MenuScene.swift
|-- GameScene.swift
|-- GameOverOverlay.swift
|-- PauseOverlay.swift
|-- PlayerNode.swift
|-- CoinNode.swift
|-- CoinSpawner.swift
|-- ObstacleSpawner.swift
|-- LaserSpawner.swift
|-- DifficultyManager.swift
|-- StarField.swift
|-- AudioManager.swift
|-- Effects.swift
|-- Constants.swift
|-- Assets.xcassets
`-- Sounds/
```
