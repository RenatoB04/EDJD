import SpriteKit

class DifficultyManager {

    private(set) var currentDifficulty: CGFloat = 1.0

    func update(elapsedTime: TimeInterval) {
        let progress = CGFloat(elapsedTime / DifficultyConfig.rampDuration)
        currentDifficulty = min(1.0 + progress * DifficultyConfig.rampIntensity, DifficultyConfig.maxDifficulty)
    }

    func reset() {
        currentDifficulty = 1.0
    }

    func currentSpawnRate() -> TimeInterval {
        let scaled = GameConfig.spawnRate / TimeInterval(currentDifficulty)
        return max(scaled, SpawnConfig.minSpawnRate)
    }

    func currentObstacleDuration() -> TimeInterval {
        let scaled = SpawnConfig.baseObstacleDuration / TimeInterval(currentDifficulty)
        return max(scaled, SpawnConfig.minObstacleDuration)
    }

    func currentParallaxMultiplier() -> CGFloat {
        return currentDifficulty
    }
}
