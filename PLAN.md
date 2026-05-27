# Astro Drift - Plan

## Concept

Endless horizontal runner in space. Tap to fly up, release to fall. Dodge asteroids. Beat your high score.

- **Genre:** Endless runner (Jetpack Joyride style)
- **Platform:** iOS - SpriteKit + Swift
- **Session length:** 30-90 seconds
- **Input:** One touch (tap = thrust up, release = fall)

## Gameplay Loop

1. Player taps Play.
2. The ship moves forward while obstacles scroll from the right.
3. Tap and hold to rise, release to fall.
4. Speed increases gradually.
5. Hit an obstacle to get Game Over.
6. See score, then retry or return to menu.

## Technical Approach

- **Engine:** SpriteKit (built into iOS, no dependencies)
- **Physics:** `SKPhysicsBody` for gravity and contact detection
- **Obstacles:** `SKAction.sequence` for spawn -> move -> remove
- **Scoring:** Distance counter in `update()`, saved with `UserDefaults`
- **Art:** Static PNG assets in `Assets.xcassets`
- **Scenes:** `MenuScene`, `GameScene`, `GameOverOverlay`

## File Structure

```text
GameViewController.swift   - loads the first scene
MenuScene.swift            - title, play button, high score
GameScene.swift            - gameplay loop
PlayerNode.swift           - player sprite and physics body
ObstacleSpawner.swift      - asteroid spawning logic
LaserSpawner.swift         - second obstacle type
Constants.swift            - physics categories and config values
GameOverOverlay.swift      - retry/menu UI
PauseOverlay.swift         - pause UI
DifficultyManager.swift    - gradual speed ramp
StarField.swift            - simple star background
AudioManager.swift         - simple background music and thrust sound
Effects.swift              - simple flash and shake effects
Assets.xcassets            - app icon and game sprites
```

## MVP Scope

| Week | Goal | Done When |
| --- | --- | --- |
| 1 | Player movement | Ship flies up/down with touch |
| 2 | Obstacles + death | Dodge asteroids, die on contact, retry |
| 3 | Score + menu | HUD, high score, menu, full loop |
| 4 | Polish | Star background, speed ramp, simple effects |

## Extra Polish

- Sound effect on death
- Screen shake
- Asteroid rotation animation
- Jetpack flame
- Background music
- Second obstacle type
