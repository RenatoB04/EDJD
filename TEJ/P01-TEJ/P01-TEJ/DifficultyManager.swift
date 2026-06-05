import SpriteKit

// Responsável por aumentar gradualmente a dificuldade durante a run.
// A GameScene pergunta a esta classe a velocidade de spawn e movimento.
class DifficultyManager {

    private(set) var currentDifficulty: CGFloat = 1.0

    // A dificuldade cresce de forma linear até ao máximo definido nas constantes.
    func update(elapsedTime: TimeInterval) {
        let progress = CGFloat(elapsedTime / DifficultyConfig.rampDuration)
        currentDifficulty = min(1.0 + progress * DifficultyConfig.rampIntensity, DifficultyConfig.maxDifficulty)
    }

    // Chamado quando a run reinicia.
    func reset() {
        currentDifficulty = 1.0
    }

    // Quanto maior a dificuldade, menor o tempo entre obstáculos.
    func currentSpawnRate() -> TimeInterval {
        let scaled = GameConfig.spawnRate / TimeInterval(currentDifficulty)
        return max(scaled, SpawnConfig.minSpawnRate)
    }

    // Quanto maior a dificuldade, mais depressa os obstáculos atravessam o ecrã.
    func currentObstacleDuration() -> TimeInterval {
        let scaled = SpawnConfig.baseObstacleDuration / TimeInterval(currentDifficulty)
        return max(scaled, SpawnConfig.minObstacleDuration)
    }

    // As estrelas acompanham a sensação de velocidade do jogo.
    func currentParallaxMultiplier() -> CGFloat {
        return currentDifficulty
    }
}
