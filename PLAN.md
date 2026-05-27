# Astro Drift — Plan

## Concept

Endless horizontal runner in space. Tap to fly up, release to fall. Dodge asteroids. Beat your high score.

- **Genre:** Endless runner (Jetpack Joyride style)
- **Platform:** iOS — SpriteKit + Swift
- **Session length:** 30–90 seconds
- **Input:** One touch (tap = thrust up, release = fall)

## Gameplay Loop

1. Player taps Play
2. Astronaut drifts forward, asteroids scroll from the right
3. Tap/hold to rise, release to fall — dodge asteroids
4. Speed increases gradually
5. Hit asteroid → Game Over → see score → retry or menu

## Technical Approach

- **Engine:** SpriteKit (built into iOS, no dependencies)
- **Physics:** `SKPhysicsBody` for gravity + collisions
- **Obstacles:** `SKAction.sequence` for spawn → move → remove
- **Scoring:** Distance counter in `update()`, saved with `UserDefaults`
- **Art:** Colored shapes (no external assets needed)
- **Scenes:** `MenuScene`, `GameScene`, Game Over overlay (`SKNode`)

### File Structure (7 files)

```
GameViewController.swift   — loads first scene
MenuScene.swift            — title, play button, high score
GameScene.swift            — all gameplay
PlayerNode.swift           — astronaut + physics body
ObstacleSpawner.swift      — asteroid spawning logic
Constants.swift            — physics categories, config values
Assets.xcassets            — app icon only
```

## MVP Scope (4 weeks)

| Week | Goal | Done When |
|------|------|-----------|
| 1 | Player movement | Square flies up/down with touch |
| 2 | Obstacles + death | Dodge circles, die on contact, retry |
| 3 | Score + menu | HUD, high score, MenuScene, full loop |
| 4 | Polish | Star background, speed ramp, particles |

**Weeks 5–6:** Bug fixes, playtesting, difficulty tuning.

## Future Ideas (only after MVP)

- Sound effect on death
- Screen shake
- Asteroid rotation animation
- Jetpack particle trail
- Second obstacle type (tall rectangle)
- Background music
