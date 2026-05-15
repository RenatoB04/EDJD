# Astro Drift — Plan

## Concept

Endless horizontal runner in space. Tap to fly up, release to fall. Dodge asteroids and lasers, collect coins, grab shields. Beat your high score.

- **Genre:** Endless runner (Jetpack Joyride style)
- **Platform:** iOS — SpriteKit + Swift
- **Session length:** 30–90 seconds
- **Input:** One touch (tap = thrust up, release = fall)

## Status

✅ **v1.0 — MVP (concluído)**
Player, obstáculos, score, recorde, menu, game over.

✅ **v1.1 — Visual / Audio polish (concluído)**
Parallax 3-camadas, moedas, dificuldade logarítmica, assets para player/asteróide.

✅ **v1.2 — Full content drop (concluído)**
Game feel (particles, shake, haptics), audio (música + SFX procedurais), 2º tipo de obstáculo (lasers), escudo, multiplicador, loja com 4 skins, definições, pausa, tutorial.

## Próximas iterações

### v1.3 — Social / progressão (futuro)
- Game Center leaderboard
- Achievements
- Daily mission

### v1.4 — Polish externo
- SFX e música profissionais
- Localização EN/PT
- Texture atlases para performance

## Stack técnico

- **Engine:** SpriteKit (built into iOS)
- **Physics:** `SKPhysicsBody` para gravidade + colisões via `contactTestBitMask`
- **Obstáculos:** `SKAction.sequence` para spawn → move → remove
- **Score:** acumulador `rawScore: Double` com `scoreMultiplier`
- **Áudio:** `AVAudioPlayer` para música/thrust loop, `SKAction.playSoundFileNamed` para SFX one-shot
- **Persistência:** `UserDefaults` (recorde, carteira, skins, settings)
- **Assets:** PNG @2x/@3x gerados programaticamente via `tools/*.ps1` (PowerShell + System.Drawing)

## Decisões de design

- **Coelisões trigger-only** (`collisionBitMask = .none`) — sem física resolvida, só notificação
- **`worldNode` separado** dos HUD/overlays para o screen shake afectar só o "mundo"
- **Singletons** para `AudioManager`, `HapticsManager` — settings globais
- **Inventário em UserDefaults** — sem servidor, sem cloud sync
- **SFX procedurais** geradas em PowerShell — facilita iterar sem precisar de assets externos; substituíveis sem mexer no código (`Sounds/coin.wav` etc.)
