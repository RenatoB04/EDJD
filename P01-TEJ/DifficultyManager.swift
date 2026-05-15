import SpriteKit

class DifficultyManager {

    private(set) var currentDifficulty: CGFloat = 1.0

    func update(elapsedTime: TimeInterval) {
        let raw = 1.0 + log(1.0 + CGFloat(elapsedTime) / CGFloat(DifficultyConfig.rampDuration)) * DifficultyConfig.rampIntensity
        currentDifficulty = min(raw, DifficultyConfig.maxDifficulty)
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

    func currentCoinSpawnRate() -> TimeInterval {
        let scaled = CoinConfig.baseSpawnRate / TimeInterval(currentDifficulty)
        let floor  = CoinConfig.baseSpawnRate * TimeInterval(CoinConfig.minSpawnRateFactor)
        return max(scaled, floor)
    }

    func currentParallaxMultiplier() -> CGFloat {
        return currentDifficulty
    }
}
