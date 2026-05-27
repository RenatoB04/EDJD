import SpriteKit

struct PhysicsCategory {
    static let none: UInt32          = 0
    static let player: UInt32        = 0b00001
    static let obstacle: UInt32      = 0b00010
    static let coin: UInt32          = 0b00100
    static let shieldPickup: UInt32  = 0b01000
}

struct GameConfig {
    static let gravity: CGFloat = -7.0
    static let thrustForce: CGFloat = 140.0
    static let maxVelocity: CGFloat = 300.0
    static let spawnRate: TimeInterval = 1.5
    static let scoreMultiplier: CGFloat = 10.0
}

struct StorageKeys {
    static let highScore       = "astroDrift.highScore"
    static let coinWallet      = "astroDrift.coinWallet"
    static let musicEnabled    = "astroDrift.musicEnabled"
    static let sfxEnabled      = "astroDrift.sfxEnabled"
    static let ownedSkins      = "astroDrift.ownedSkins"
    static let equippedSkin    = "astroDrift.equippedSkin"
}

struct NodeNames {
    static let player           = "player"
    static let obstacle         = "obstacle"
    static let coin             = "coin"
    static let shieldPickup     = "shieldPickup"
    static let shieldAura       = "shieldAura"

    static let retryButton      = "retryButton"
    static let menuButton       = "menuButton"
    static let playButton       = "playButton"
    static let shopButton       = "shopButton"
    static let settingsButton   = "settingsButton"
    static let pauseButton      = "pauseButton"
    static let resumeButton     = "resumeButton"
    static let backButton       = "backButton"
    static let resetButton      = "resetButton"

    static let musicToggle      = "musicToggle"
    static let sfxToggle        = "sfxToggle"

    static let gameOverOverlay  = "gameOverOverlay"
    static let pauseOverlay     = "pauseOverlay"
}

struct AssetNames {
    static let player   = "player"
    static let asteroid = "asteroid"
}

struct PlayerConfig {
    static let size = CGSize(width: 44, height: 44)
}

struct ObstacleConfig {
    static let size = CGSize(width: 36, height: 36)
    static var physicsRadius: CGFloat { size.width * 0.45 }
}

struct StarFieldConfig {
    static let farLayerSpeed: CGFloat = 20.0
    static let midLayerSpeed: CGFloat = 50.0
    static let nearLayerSpeed: CGFloat = 100.0

    static let farStarCount: Int = 40
    static let midStarCount: Int = 25
    static let nearStarCount: Int = 15

    static let farStarSize: CGFloat = 1.0
    static let midStarSize: CGFloat = 2.0
    static let nearStarSize: CGFloat = 3.0
}

struct DifficultyConfig {
    static let rampDuration: TimeInterval = 15.0
    static let rampIntensity: CGFloat = 0.6
    static let maxDifficulty: CGFloat = 2.5
}

struct SpawnConfig {
    static let baseObstacleDuration: TimeInterval = 3.5
    static let minSpawnRate: TimeInterval = 0.45
    static let minObstacleDuration: TimeInterval = 1.4
    static let laserChance: CGFloat = 0.25
}

struct CoinConfig {
    static let radius: CGFloat = 10.0
    static let baseSpawnRate: TimeInterval = 2.2
    static let minMoveDuration: TimeInterval = 1.4
    static let minSpawnRateFactor: CGFloat = 0.45
}

struct ShieldConfig {
    static let baseSpawnRate: TimeInterval = 14.0
    static let durationAfterPickup: TimeInterval = 5.0
}

struct ScoreConfig {
    static let coinsForStreak: Int = 5
    static let streakDuration: TimeInterval = 10.0
    static let maxMultiplier: CGFloat = 3.0
}
