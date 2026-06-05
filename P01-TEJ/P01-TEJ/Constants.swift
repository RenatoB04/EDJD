import SpriteKit

// Máscaras usadas pelo SpriteKit para distinguir tipos de colisão.
// Cada categoria usa um bit diferente para podermos combinar valores.
struct PhysicsCategory {
    static let none: UInt32     = 0
    static let player: UInt32   = 0b0001
    static let obstacle: UInt32 = 0b0010
    static let coin: UInt32     = 0b0100
}

// Valores principais da jogabilidade.
// Estão juntos para ser mais fácil ajustar dificuldade e sensação de voo.
struct GameConfig {
    static let gravity: CGFloat = -7.0
    static let thrustForce: CGFloat = 140.0
    static let maxVelocity: CGFloat = 300.0
    static let spawnRate: TimeInterval = 1.5
    static let scoreMultiplier: CGFloat = 10.0
}

// Chaves usadas no UserDefaults para guardar dados simples entre sessões.
struct StorageKeys {
    static let highScore = "astroDrift.highScore"
}

// Nomes atribuídos aos nós para detectar toques e limpar objectos da cena.
struct NodeNames {
    static let player = "player"
    static let obstacle = "obstacle"

    static let retryButton = "retryButton"
    static let menuButton = "menuButton"
    static let playButton = "playButton"
    static let pauseButton = "pauseButton"
    static let resumeButton = "resumeButton"
    static let continueButton = "continueButton"
    static let coin = "coin"

    static let gameOverOverlay = "gameOverOverlay"
    static let pauseOverlay = "pauseOverlay"
}

// Nomes dos assets no Assets.xcassets.
struct AssetNames {
    static let player = "player"
    static let asteroid = "asteroid"
}

// Tamanho visual do jogador.
struct PlayerConfig {
    static let size = CGSize(width: 44, height: 44)
}

// Configuração dos asteroides.
struct ObstacleConfig {
    static let size = CGSize(width: 36, height: 36)
    static var physicsRadius: CGFloat { size.width * 0.45 }
}

// Configuração do fundo de estrelas.
struct StarFieldConfig {
    static let speed: CGFloat = 45.0
    static let starCount: Int = 70
}

// Controla a forma como a dificuldade sobe com o tempo.
struct DifficultyConfig {
    static let rampDuration: TimeInterval = 45.0
    static let rampIntensity: CGFloat = 1.2
    static let maxDifficulty: CGFloat = 2.5
}

// Configuração de spawn e movimento dos obstáculos.
struct SpawnConfig {
    static let baseObstacleDuration: TimeInterval = 3.5
    static let minSpawnRate: TimeInterval = 0.45
    static let minObstacleDuration: TimeInterval = 1.4
    static let laserChance: CGFloat = 0.25
}

// Configuração das moedas e do custo inicial para continuar.
struct CoinConfig {
    static let radius: CGFloat = 10.0
    static let spawnRate: TimeInterval = 2.0
    static let continueCost: Int = 10
}
