import SpriteKit

class GameScene: SKScene, SKPhysicsContactDelegate {

    let player = PlayerNode()
    let asteroidSpawner = ObstacleSpawner()
    let laserSpawner = LaserSpawner()
    let coinSpawner = CoinSpawner()
    let shieldSpawner = ShieldSpawner()
    let difficultyManager = DifficultyManager()

    var isThrusting = false
    var isGameOver = false
    private var isManuallyPaused = false

    private var score: Int = 0
    private var rawScore: Double = 0
    private var coinCount: Int = 0
    private var coinStreak: Int = 0
    private var scoreMultiplier: CGFloat = 1.0
    private var streakRemaining: TimeInterval = 0

    private var lastUpdateTime: TimeInterval = 0
    private var elapsedTime: TimeInterval = 0

    private let scoreLabel = SKLabelNode(fontNamed: "AvenirNext-Bold")
    private let coinLabel  = SKLabelNode(fontNamed: "AvenirNext-Bold")
    private let multiplierLabel = SKLabelNode(fontNamed: "AvenirNext-Bold")
    private var pauseButton: SKSpriteNode!

    private var starField: StarField?
    private var worldNode: SKNode!

    private weak var gameOverOverlay: GameOverOverlay?
    private weak var pauseOverlay: PauseOverlay?

    private static let spawnLoopKey       = "spawnLoop"
    private static let coinSpawnLoopKey   = "coinSpawnLoop"
    private static let shieldSpawnLoopKey = "shieldSpawnLoop"

    override func didMove(to view: SKView) {
        self.backgroundColor = .black
        self.size = view.bounds.size

        self.physicsWorld.gravity = CGVector(dx: 0, dy: GameConfig.gravity)
        self.physicsWorld.contactDelegate = self

        worldNode = SKNode()
        addChild(worldNode)

        setupStarField(view: view)

        player.position = CGPoint(x: size.width * 0.2, y: size.height / 2)
        worldNode.addChild(player)
        player.trail.targetNode = worldNode

        setupHUD()

        AudioManager.shared.startMusic()
        startGameplay()

        NotificationCenter.default.addObserver(self,
                                               selector: #selector(handleEnterBackground),
                                               name: UIApplication.willResignActiveNotification,
                                               object: nil)
    }

    deinit {
        NotificationCenter.default.removeObserver(self)
    }

    private func setupStarField(view: SKView) {
        let field = StarField(sceneSize: self.size, view: view)
        worldNode.addChild(field)
        self.starField = field
    }

    private func setupHUD() {
        scoreLabel.text = "0 m"
        scoreLabel.fontSize = 28
        scoreLabel.fontColor = .white
        scoreLabel.horizontalAlignmentMode = .right
        scoreLabel.position = CGPoint(x: size.width - 20, y: size.height - 50)
        scoreLabel.zPosition = 100
        addChild(scoreLabel)

        coinLabel.text = "Moedas: 0"
        coinLabel.fontSize = 22
        coinLabel.fontColor = SKColor(red: 1.0, green: 0.82, blue: 0.0, alpha: 1.0)
        coinLabel.horizontalAlignmentMode = .right
        coinLabel.position = CGPoint(x: size.width - 20, y: size.height - 80)
        coinLabel.zPosition = 100
        addChild(coinLabel)

        multiplierLabel.text = nil
        multiplierLabel.fontSize = 18
        multiplierLabel.fontColor = .systemOrange
        multiplierLabel.horizontalAlignmentMode = .right
        multiplierLabel.position = CGPoint(x: size.width - 20, y: size.height - 105)
        multiplierLabel.zPosition = 100
        addChild(multiplierLabel)

        pauseButton = SKSpriteNode(color: SKColor(white: 0.2, alpha: 0.7), size: CGSize(width: 44, height: 44))
        pauseButton.position = CGPoint(x: 30, y: size.height - 50)
        pauseButton.name = NodeNames.pauseButton
        pauseButton.zPosition = 100

        let pauseGlyph = SKLabelNode(fontNamed: "AvenirNext-Bold")
        pauseGlyph.text = "II"
        pauseGlyph.fontSize = 18
        pauseGlyph.verticalAlignmentMode = .center
        pauseGlyph.horizontalAlignmentMode = .center
        pauseGlyph.name = NodeNames.pauseButton
        pauseButton.addChild(pauseGlyph)
        addChild(pauseButton)
    }

    private func startGameplay() {
        scheduleNextSpawn()
        scheduleNextCoinSpawn()
        scheduleNextShieldSpawn()
    }

    private func scheduleNextSpawn() {
        self.removeAction(forKey: GameScene.spawnLoopKey)
        let wait = SKAction.wait(forDuration: difficultyManager.currentSpawnRate())
        let spawn = SKAction.run { [weak self] in
            guard let self = self, !self.isGameOver else { return }
            let duration = self.difficultyManager.currentObstacleDuration()
            if CGFloat.random(in: 0...1) < SpawnConfig.laserChance {
                self.laserSpawner.spawn(in: self.worldNode, moveDuration: duration, sceneSize: self.size)
            } else {
                self.asteroidSpawner.spawn(in: self.worldNode, moveDuration: duration, sceneSize: self.size)
            }
            self.scheduleNextSpawn()
        }
        run(SKAction.sequence([wait, spawn]), withKey: GameScene.spawnLoopKey)
    }

    private func scheduleNextCoinSpawn() {
        self.removeAction(forKey: GameScene.coinSpawnLoopKey)
        let wait = SKAction.wait(forDuration: difficultyManager.currentCoinSpawnRate())
        let spawn = SKAction.run { [weak self] in
            guard let self = self, !self.isGameOver else { return }
            let duration = self.difficultyManager.currentObstacleDuration()
            self.coinSpawner.spawn(in: self.worldNode, moveDuration: duration, sceneSize: self.size)
            self.scheduleNextCoinSpawn()
        }
        run(SKAction.sequence([wait, spawn]), withKey: GameScene.coinSpawnLoopKey)
    }

    private func scheduleNextShieldSpawn() {
        self.removeAction(forKey: GameScene.shieldSpawnLoopKey)
        let wait = SKAction.wait(forDuration: ShieldConfig.baseSpawnRate)
        let spawn = SKAction.run { [weak self] in
            guard let self = self, !self.isGameOver else { return }
            let duration = self.difficultyManager.currentObstacleDuration()
            self.shieldSpawner.spawn(in: self.worldNode, moveDuration: duration, sceneSize: self.size)
            self.scheduleNextShieldSpawn()
        }
        run(SKAction.sequence([wait, spawn]), withKey: GameScene.shieldSpawnLoopKey)
    }

    override func touchesBegan(_ touches: Set<UITouch>, with event: UIEvent?) {
        guard let touch = touches.first else { return }
        let location = touch.location(in: self)

        if isGameOver {
            handleGameOverTouch(at: location)
            return
        }

        if isManuallyPaused {
            handlePauseTouch(at: location)
            return
        }

        let names = nodes(at: location).compactMap { $0.name }
        if names.contains(NodeNames.pauseButton) {
            pauseGame()
            return
        }

        isThrusting = true
        player.setThrusting(true)
        AudioManager.shared.startThrustLoop()
    }

    override func touchesEnded(_ touches: Set<UITouch>, with event: UIEvent?) {
        isThrusting = false
        player.setThrusting(false)
        AudioManager.shared.stopThrustLoop()
    }

    override func touchesCancelled(_ touches: Set<UITouch>, with event: UIEvent?) {
        isThrusting = false
        player.setThrusting(false)
        AudioManager.shared.stopThrustLoop()
    }

    private func handleGameOverTouch(at location: CGPoint) {
        guard let overlay = gameOverOverlay,
              let buttonName = overlay.buttonName(at: location) else { return }

        AudioManager.shared.playSFX(.button, on: self)

        switch buttonName {
        case NodeNames.retryButton: restartGame()
        case NodeNames.menuButton:  goToMenu()
        default: break
        }
    }

    private func handlePauseTouch(at location: CGPoint) {
        guard let overlay = pauseOverlay,
              let buttonName = overlay.buttonName(at: location) else { return }

        AudioManager.shared.playSFX(.button, on: self)

        switch buttonName {
        case NodeNames.resumeButton: resumeGame()
        case NodeNames.menuButton:   goToMenu()
        default: break
        }
    }

    private func pauseGame() {
        guard !isManuallyPaused, !isGameOver else { return }
        isManuallyPaused = true
        removeAction(forKey: GameScene.spawnLoopKey)
        removeAction(forKey: GameScene.coinSpawnLoopKey)
        removeAction(forKey: GameScene.shieldSpawnLoopKey)
        worldNode.isPaused = true
        physicsWorld.speed = 0
        AudioManager.shared.stopThrustLoop()
        AudioManager.shared.pauseAll()

        let overlay = PauseOverlay(sceneSize: self.size)
        addChild(overlay)
        pauseOverlay = overlay
    }

    private func resumeGame() {
        isManuallyPaused = false
        worldNode.isPaused = false
        
        player.physicsBody?.velocity = .zero
        physicsWorld.speed = 1
        lastUpdateTime = 0
        startGameplay()
        
        AudioManager.shared.resumeAll()
        pauseOverlay?.removeFromParent()
        pauseOverlay = nil
    }

    @objc private func handleEnterBackground() {
        if !isGameOver && !isManuallyPaused {
            pauseGame()
        }
    }

    override func update(_ currentTime: TimeInterval) {
        guard !isGameOver, !isManuallyPaused else { return }

        if lastUpdateTime == 0 { lastUpdateTime = currentTime }
        let dt = currentTime - lastUpdateTime
        lastUpdateTime = currentTime

        elapsedTime += dt
        difficultyManager.update(elapsedTime: elapsedTime)

        starField?.update(deltaTime: dt, speedMultiplier: difficultyManager.currentParallaxMultiplier())

        rawScore += dt * Double(GameConfig.scoreMultiplier) * Double(scoreMultiplier)
        score = Int(rawScore)
        scoreLabel.text = "\(score) m"

        if streakRemaining > 0 {
            streakRemaining -= dt
            if streakRemaining <= 0 {
                resetMultiplier()
            }
        }

        if isThrusting {
            player.physicsBody?.applyForce(CGVector(dx: 0, dy: GameConfig.thrustForce))
            if let vy = player.physicsBody?.velocity.dy, vy > GameConfig.maxVelocity {
                player.physicsBody?.velocity.dy = GameConfig.maxVelocity
            }
        }

        if player.position.y > size.height - 20 {
            player.position.y = size.height - 20
            player.physicsBody?.velocity.dy = 0
        }
        if player.position.y < 20 {
            player.position.y = 20
            player.physicsBody?.velocity.dy = 0
        }
    }

    func didBegin(_ contact: SKPhysicsContact) {
        guard !isGameOver else { return }

        let a = contact.bodyA.categoryBitMask
        let b = contact.bodyB.categoryBitMask
        let combined = a | b

        if combined == (PhysicsCategory.player | PhysicsCategory.coin) {
            let coinBody = (a == PhysicsCategory.coin) ? contact.bodyA : contact.bodyB
            collectCoin(node: coinBody.node)
        } else if combined == (PhysicsCategory.player | PhysicsCategory.shieldPickup) {
            let pickupBody = (a == PhysicsCategory.shieldPickup) ? contact.bodyA : contact.bodyB
            collectShield(node: pickupBody.node)
        } else if combined == (PhysicsCategory.player | PhysicsCategory.obstacle) {
            if player.hasShield {
                let obstacleBody = (a == PhysicsCategory.obstacle) ? contact.bodyA : contact.bodyB
                consumeShield(obstacleNode: obstacleBody.node)
            } else {
                triggerGameOver()
            }
        }
    }

    private func collectCoin(node: SKNode?) {
        guard let coin = node, coin.parent != nil else { return }

        coinCount += 1
        coinStreak += 1
        coinLabel.text = "Moedas: \(coinCount)"

        if coinStreak >= ScoreConfig.coinsForStreak {
            increaseMultiplier()
            coinStreak = 0
        }

        let collect = SKAction.group([
            SKAction.moveBy(x: 0, y: 25, duration: 0.25),
            SKAction.fadeOut(withDuration: 0.25)
        ])
        coin.run(SKAction.sequence([collect, SKAction.removeFromParent()]))

        coinLabel.run(SKAction.sequence([
            SKAction.scale(to: 1.35, duration: 0.08),
            SKAction.scale(to: 1.0,  duration: 0.12)
        ]))

        AudioManager.shared.playSFX(.coin, on: self)
    }

    private func collectShield(node: SKNode?) {
        guard let pickup = node, pickup.parent != nil else { return }
        pickup.removeFromParent()

        player.hasShield = true
        player.run(SKAction.sequence([
            SKAction.wait(forDuration: ShieldConfig.durationAfterPickup),
            SKAction.run { [weak self] in self?.player.hasShield = false }
        ]), withKey: "shieldTimer")

        AudioManager.shared.playSFX(.shield, on: self)
    }

    private func consumeShield(obstacleNode: SKNode?) {
        player.hasShield = false
        player.removeAction(forKey: "shieldTimer")

        obstacleNode?.removeAllActions()
        obstacleNode?.run(SKAction.sequence([
            SKAction.fadeOut(withDuration: 0.2),
            SKAction.removeFromParent()
        ]))

        Effects.flash(in: self, color: SKColor(red: 0.4, green: 0.85, blue: 1.0, alpha: 1.0), peakAlpha: 0.4)
        AudioManager.shared.playSFX(.shield, on: self)
    }

    private func increaseMultiplier() {
        scoreMultiplier = min(scoreMultiplier + 1.0, ScoreConfig.maxMultiplier)
        streakRemaining = ScoreConfig.streakDuration
        multiplierLabel.text = "x\(Int(scoreMultiplier)) (\(Int(streakRemaining))s)"
        multiplierLabel.run(SKAction.sequence([
            SKAction.scale(to: 1.5, duration: 0.1),
            SKAction.scale(to: 1.0, duration: 0.15)
        ]))
    }

    private func resetMultiplier() {
        scoreMultiplier = 1.0
        streakRemaining = 0
        multiplierLabel.text = nil
    }

    private func triggerGameOver() {
        isGameOver = true
        isThrusting = false
        player.setThrusting(false)

        self.removeAction(forKey: GameScene.spawnLoopKey)
        self.removeAction(forKey: GameScene.coinSpawnLoopKey)
        self.removeAction(forKey: GameScene.shieldSpawnLoopKey)

        player.physicsBody?.velocity = .zero
        player.physicsBody?.affectedByGravity = false

        AudioManager.shared.stopThrustLoop()
        AudioManager.shared.playSFX(.hit, on: self)

        Effects.flash(in: self)
        Effects.screenShake(on: worldNode)
        Effects.deathExplosion(at: player.position, in: self)
        Effects.playerDeathAnimation(on: player)

        let fade = SKAction.sequence([
            SKAction.fadeOut(withDuration: 0.3),
            SKAction.removeFromParent()
        ])
        worldNode.enumerateChildNodes(withName: NodeNames.obstacle) { node, _ in
            node.removeAllActions()
            node.run(fade)
        }
        worldNode.enumerateChildNodes(withName: NodeNames.coin) { node, _ in
            node.removeAllActions()
            node.run(fade)
        }
        worldNode.enumerateChildNodes(withName: NodeNames.shieldPickup) { node, _ in
            node.removeAllActions()
            node.run(fade)
        }

        let defaults = UserDefaults.standard
        let previousHigh = defaults.integer(forKey: StorageKeys.highScore)
        let isNewRecord = score > previousHigh
        if isNewRecord {
            defaults.set(score, forKey: StorageKeys.highScore)
        }
        let highScoreToShow = max(previousHigh, score)

        PlayerInventory.setWalletBalance(PlayerInventory.walletBalance() + coinCount)

        let overlay = GameOverOverlay(
            sceneSize: self.size,
            score: score,
            highScore: highScoreToShow,
            isNewRecord: isNewRecord,
            coinsThisRun: coinCount
        )
        overlay.alpha = 0
        addChild(overlay)
        overlay.run(SKAction.sequence([
            SKAction.wait(forDuration: 0.6),
            SKAction.fadeIn(withDuration: 0.4)
        ]))
        gameOverOverlay = overlay
    }

    private func restartGame() {
        gameOverOverlay?.removeFromParent()
        gameOverOverlay = nil

        worldNode.enumerateChildNodes(withName: NodeNames.obstacle)     { n, _ in n.removeFromParent() }
        worldNode.enumerateChildNodes(withName: NodeNames.coin)         { n, _ in n.removeFromParent() }
        worldNode.enumerateChildNodes(withName: NodeNames.shieldPickup) { n, _ in n.removeFromParent() }

        player.alpha = 1.0
        player.setScale(1.0)
        player.color = .clear
        player.colorBlendFactor = 0
        player.position = CGPoint(x: size.width * 0.2, y: size.height / 2)
        player.physicsBody?.velocity = .zero
        player.physicsBody?.affectedByGravity = true
        player.hasShield = false
        player.reloadSkin()

        score = 0
        rawScore = 0
        coinCount = 0
        coinStreak = 0
        elapsedTime = 0
        lastUpdateTime = 0
        resetMultiplier()
        scoreLabel.text = "0 m"
        coinLabel.text  = "Moedas: 0"
        isGameOver = false
        isThrusting = false

        difficultyManager.reset()
        startGameplay()
    }

    private func goToMenu() {
        AudioManager.shared.stopThrustLoop()
        let menu = MenuScene(size: self.size)
        menu.scaleMode = .aspectFill
        menu.lastScore = score
        view?.presentScene(menu, transition: .doorway(withDuration: 0.7))
    }
}
