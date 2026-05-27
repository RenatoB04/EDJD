import SpriteKit

class GameScene: SKScene, SKPhysicsContactDelegate {

    let player = PlayerNode()
    let asteroidSpawner = ObstacleSpawner()
    let laserSpawner = LaserSpawner()
    let difficultyManager = DifficultyManager()

    var isThrusting = false
    var isGameOver = false
    private var isManuallyPaused = false

    private var score: Int = 0
    private var rawScore: Double = 0

    private var lastUpdateTime: TimeInterval = 0
    private var elapsedTime: TimeInterval = 0

    private let scoreLabel = SKLabelNode(fontNamed: "AvenirNext-Bold")
    private var pauseButton: SKSpriteNode!

    private var starField: StarField?
    private var worldNode: SKNode!

    private weak var gameOverOverlay: GameOverOverlay?
    private weak var pauseOverlay: PauseOverlay?

    private static let spawnLoopKey = "spawnLoop"

    override func didMove(to view: SKView) {
        backgroundColor = .black
        size = view.bounds.size

        physicsWorld.gravity = CGVector(dx: 0, dy: GameConfig.gravity)
        physicsWorld.contactDelegate = self

        worldNode = SKNode()
        addChild(worldNode)

        setupStarField()

        player.position = CGPoint(x: size.width * 0.2, y: size.height / 2)
        worldNode.addChild(player)

        setupHUD()
        startGameplay()
    }

    private func setupStarField() {
        let field = StarField(sceneSize: size)
        worldNode.addChild(field)
        starField = field
    }

    private func setupHUD() {
        scoreLabel.text = "0 m"
        scoreLabel.fontSize = 28
        scoreLabel.fontColor = .white
        scoreLabel.horizontalAlignmentMode = .right
        scoreLabel.position = CGPoint(x: size.width - 20, y: size.height - 50)
        scoreLabel.zPosition = 100
        addChild(scoreLabel)

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
    }

    private func scheduleNextSpawn() {
        removeAction(forKey: GameScene.spawnLoopKey)

        let wait = SKAction.wait(forDuration: difficultyManager.currentSpawnRate())
        let spawn = SKAction.run { [weak self] in
            guard let self = self, !self.isGameOver, !self.isManuallyPaused else { return }

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
    }

    override func touchesEnded(_ touches: Set<UITouch>, with event: UIEvent?) {
        stopThrust()
    }

    override func touchesCancelled(_ touches: Set<UITouch>, with event: UIEvent?) {
        stopThrust()
    }

    private func stopThrust() {
        isThrusting = false
        player.setThrusting(false)
    }

    private func handleGameOverTouch(at location: CGPoint) {
        guard let overlay = gameOverOverlay,
              let buttonName = overlay.buttonName(at: location) else { return }

        run(SKAction.playSoundFileNamed("Sounds/button.wav", waitForCompletion: false))

        switch buttonName {
        case NodeNames.retryButton: restartGame()
        case NodeNames.menuButton: goToMenu()
        default: break
        }
    }

    private func handlePauseTouch(at location: CGPoint) {
        guard let overlay = pauseOverlay,
              let buttonName = overlay.buttonName(at: location) else { return }

        run(SKAction.playSoundFileNamed("Sounds/button.wav", waitForCompletion: false))

        switch buttonName {
        case NodeNames.resumeButton: resumeGame()
        case NodeNames.menuButton: goToMenu()
        default: break
        }
    }

    private func pauseGame() {
        guard !isManuallyPaused, !isGameOver else { return }

        isManuallyPaused = true
        removeAction(forKey: GameScene.spawnLoopKey)
        worldNode.isPaused = true
        physicsWorld.speed = 0
        stopThrust()

        let overlay = PauseOverlay(sceneSize: size)
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

        pauseOverlay?.removeFromParent()
        pauseOverlay = nil
    }

    override func update(_ currentTime: TimeInterval) {
        guard !isGameOver, !isManuallyPaused else { return }

        if lastUpdateTime == 0 { lastUpdateTime = currentTime }
        let dt = currentTime - lastUpdateTime
        lastUpdateTime = currentTime

        elapsedTime += dt
        difficultyManager.update(elapsedTime: elapsedTime)
        starField?.update(deltaTime: dt, speedMultiplier: difficultyManager.currentParallaxMultiplier())

        rawScore += dt * Double(GameConfig.scoreMultiplier)
        score = Int(rawScore)
        scoreLabel.text = "\(score) m"

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

        let combined = contact.bodyA.categoryBitMask | contact.bodyB.categoryBitMask
        if combined == (PhysicsCategory.player | PhysicsCategory.obstacle) {
            triggerGameOver()
        }
    }

    private func triggerGameOver() {
        isGameOver = true
        stopThrust()
        removeAction(forKey: GameScene.spawnLoopKey)

        player.physicsBody?.velocity = .zero
        player.physicsBody?.affectedByGravity = false

        run(SKAction.playSoundFileNamed("Sounds/hit.wav", waitForCompletion: false))

        Effects.flash(in: self)
        Effects.screenShake(on: worldNode)
        Effects.playerDeathAnimation(on: player)

        let fade = SKAction.sequence([
            SKAction.fadeOut(withDuration: 0.3),
            SKAction.removeFromParent()
        ])
        worldNode.enumerateChildNodes(withName: NodeNames.obstacle) { node, _ in
            node.removeAllActions()
            node.run(fade)
        }

        let defaults = UserDefaults.standard
        let previousHigh = defaults.integer(forKey: StorageKeys.highScore)
        let isNewRecord = score > previousHigh
        if isNewRecord {
            defaults.set(score, forKey: StorageKeys.highScore)
        }

        let overlay = GameOverOverlay(
            sceneSize: size,
            score: score,
            highScore: max(previousHigh, score),
            isNewRecord: isNewRecord
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

        worldNode.enumerateChildNodes(withName: NodeNames.obstacle) { node, _ in
            node.removeFromParent()
        }

        player.alpha = 1.0
        player.setScale(1.0)
        player.color = .clear
        player.colorBlendFactor = 0
        player.position = CGPoint(x: size.width * 0.2, y: size.height / 2)
        player.physicsBody?.velocity = .zero
        player.physicsBody?.affectedByGravity = true

        score = 0
        rawScore = 0
        elapsedTime = 0
        lastUpdateTime = 0
        scoreLabel.text = "0 m"
        isGameOver = false
        isThrusting = false

        difficultyManager.reset()
        startGameplay()
    }

    private func goToMenu() {
        let menu = MenuScene(size: size)
        menu.scaleMode = .aspectFill
        menu.lastScore = score
        view?.presentScene(menu, transition: .doorway(withDuration: 0.7))
    }
}
