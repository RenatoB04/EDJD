# Astro Drift - Plan

## Concept

Endless horizontal runner in space. Tap to fly up, release to fall. Dodge asteroids and lasers, collect coins, grab shields. Beat your high score.

- **Genre:** Endless runner (Jetpack Joyride style)
- **Platform:** iOS - SpriteKit + Swift
- **Session length:** 30-90 seconds
- **Input:** One touch (tap = thrust up, release = fall)

## Status

**v1.0 - MVP (concluido)**
Player, obstaculos, score, recorde, menu, game over.

**v1.1 - Visual / Audio polish (concluido)**
Parallax 3-camadas, moedas, dificuldade logaritmica, assets para player/asteroide.

**v1.2 - Full content drop (concluido)**
Game feel (particles, shake), audio (musica + SFX procedurais), lasers, escudo, multiplicador, loja com 4 skins, definicoes e pausa.

## Proximas iteracoes

### v1.3 - Social / progressao (futuro)
- Game Center leaderboard
- Achievements
- Daily mission

### v1.4 - Polish externo
- SFX e musica profissionais
- Localizacao EN/PT
- Texture atlases para performance

## Stack tecnico

- **Engine:** SpriteKit (built into iOS)
- **Physics:** `SKPhysicsBody` para gravidade + colisoes via `contactTestBitMask`
- **Obstaculos:** `SKAction.sequence` para spawn -> move -> remove
- **Score:** acumulador `rawScore: Double` com `scoreMultiplier`
- **Audio:** `AVAudioPlayer` para musica/thrust loop, `SKAction.playSoundFileNamed` para SFX one-shot
- **Persistencia:** `UserDefaults` (recorde, carteira, skins, settings)
- **Assets:** PNG @2x/@3x gerados programaticamente via `tools/*.ps1` (PowerShell + System.Drawing)

## Decisoes de design

- **Colisoes trigger-only** (`collisionBitMask = .none`) - sem fisica resolvida, so notificacao
- **`worldNode` separado** dos HUD/overlays para o screen shake afectar so o mundo
- **Singleton** para `AudioManager` - settings globais de audio
- **Inventario em UserDefaults** - sem servidor, sem cloud sync
- **SFX procedurais** geradas em PowerShell - facilita iterar sem precisar de assets externos; substituiveis sem mexer no codigo (`Sounds/coin.wav` etc.)
