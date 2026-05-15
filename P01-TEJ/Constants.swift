import SpriteKit

struct PhysicsCategory {
    static let none: UInt32     = 0
    static let player: UInt32   = 0b001
    static let obstacle: UInt32 = 0b010
    static let coin: UInt32     = 0b100
}

struct GameConfig {
    static let gravity: CGFloat = -7.0
    static let thrustForce: CGFloat = 140.0
    static let maxVelocity: CGFloat = 300.0
    static let spawnRate: TimeInterval = 1.5
    static let scoreMultiplier: CGFloat = 10.0
}

struct StorageKeys {
    static let highScore = "astroDrift.highScore"
}

struct NodeNames {
    static let retryButton      = "retryButton"
    static let menuButton       = "menuButton"
    static let gameOverOverlay  = "gameOverOverlay"
    static let coin             = "coin"
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
}

struct CoinConfig {
    static let radius: CGFloat = 10.0
    static let baseSpawnRate: TimeInterval = 2.2
    static let minMoveDuration: TimeInterval = 1.4
}
