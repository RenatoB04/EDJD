import SpriteKit

class GameScene: SKScene, SKPhysicsContactDelegate {
    let player = PlayerNode()
    let spawner = ObstacleSpawner()
    let coinSpawner = CoinSpawner()
    let difficultyManager = DifficultyManager()
    
    var isThrusting = false
    var isGameOver = false
    
    private var score: Int = 0
    private var coinCount: Int = 0
    private var lastUpdateTime: TimeInterval = 0
    private var elapsedTime: TimeInterval = 0
    private let scoreLabel = SKLabelNode(fontNamed: "AvenirNext-Bold")
    private let coinLabel  = SKLabelNode(fontNamed: "AvenirNext-Bold")
    
    private var starField: StarField?
    
    private weak var gameOverOverlay: GameOverOverlay?
    
    override func didMove(to view: SKView) {
        self.backgroundColor = .black
        self.size = view.bounds.size
        
        self.physicsWorld.gravity = CGVector(dx: 0, dy: GameConfig.gravity)
        self.physicsWorld.contactDelegate = self
        
        setupStarField(view: view)
        
        player.position = CGPoint(x: size.width * 0.2, y: size.height / 2)
        
        if player.parent == nil {
            addChild(player)
        }
        
        setupHUD()
        scheduleNextSpawn()
        scheduleNextCoinSpawn()
    }
    
    private func setupStarField(view: SKView) {
        let field = StarField(sceneSize: self.size, view: view)
        addChild(field)
        self.starField = field
    }
    
    private func setupHUD() {
        scoreLabel.text = "0 m"
        scoreLabel.fontSize = 28
        scoreLabel.fontColor = .white
        scoreLabel.horizontalAlignmentMode = .right
        scoreLabel.position = CGPoint(x: size.width - 20, y: size.height - 50)
        scoreLabel.zPosition = 100
        if scoreLabel.parent == nil {
            addChild(scoreLabel)
        }

        coinLabel.text = "🪙 0"
        coinLabel.fontSize = 22
        coinLabel.fontColor = SKColor(red: 1.0, green: 0.82, blue: 0.0, alpha: 1.0)
        coinLabel.horizontalAlignmentMode = .right
        coinLabel.position = CGPoint(x: size.width - 20, y: size.height - 80)
        coinLabel.zPosition = 100
        if coinLabel.parent == nil {
            addChild(coinLabel)
        }
    }
    
    private func scheduleNextSpawn() {
        self.removeAction(forKey: "spawnLoop")
        
        let waitDuration = difficultyManager.currentSpawnRate()
        let wait = SKAction.wait(forDuration: waitDuration)
        let spawn = SKAction.run { [weak self] in
            guard let self = self, !self.isGameOver else { return }
            let duration = self.difficultyManager.currentObstacleDuration()
            self.spawner.spawn(in: self, moveDuration: duration)
            self.scheduleNextSpawn()
        }
        
        let sequence = SKAction.sequence([wait, spawn])
        run(sequence, withKey: "spawnLoop")
    }
    
    private func scheduleNextCoinSpawn() {
        self.removeAction(forKey: "coinSpawnLoop")

        let rate = max(CoinConfig.baseSpawnRate / TimeInterval(difficultyManager.currentDifficulty),
                       CoinConfig.baseSpawnRate * 0.45)
        let wait  = SKAction.wait(forDuration: rate)
        let spawn = SKAction.run { [weak self] in
            guard let self = self, !self.isGameOver else { return }
            let duration = self.difficultyManager.currentObstacleDuration()
            self.coinSpawner.spawn(in: self, moveDuration: duration)
            self.scheduleNextCoinSpawn()
        }
        run(SKAction.sequence([wait, spawn]), withKey: "coinSpawnLoop")
    }
    
    override func touchesBegan(_ touches: Set<UITouch>, with event: UIEvent?) {
        if isGameOver {
            handleGameOverTouch(touches)
        } else {
            isThrusting = true
        }
    }
    
    override func touchesEnded(_ touches: Set<UITouch>, with event: UIEvent?) {
        isThrusting = false
    }
    
    override func touchesCancelled(_ touches: Set<UITouch>, with event: UIEvent?) {
        isThrusting = false
    }
    
    private func handleGameOverTouch(_ touches: Set<UITouch>) {
        guard let touch = touches.first, let overlay = gameOverOverlay else { return }
        let location = touch.location(in: self)
        
        guard let buttonName = overlay.buttonName(at: location) else { return }
        
        switch buttonName {
        case NodeNames.retryButton:
            restartGame()
        case NodeNames.menuButton:
            goToMenu()
        default:
            break
        }
    }
    
    override func update(_ currentTime: TimeInterval) {
        guard !isGameOver else { return }
        
        if lastUpdateTime == 0 {
            lastUpdateTime = currentTime
        }
        let dt = currentTime - lastUpdateTime
        lastUpdateTime = currentTime
        
        elapsedTime += dt
        difficultyManager.update(elapsedTime: elapsedTime)
        
        starField?.update(
            deltaTime: dt,
            speedMultiplier: difficultyManager.currentParallaxMultiplier()
        )
        
        score = Int(elapsedTime * TimeInterval(GameConfig.scoreMultiplier))
        scoreLabel.text = "\(score) m"
        
        if isThrusting {
            player.physicsBody?.applyForce(CGVector(dx: 0, dy: GameConfig.thrustForce))
            
            if let velocityY = player.physicsBody?.velocity.dy, velocityY > GameConfig.maxVelocity {
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

        let categoryA = contact.bodyA.categoryBitMask
        let categoryB = contact.bodyB.categoryBitMask
        let combined  = categoryA | categoryB

        if combined == (PhysicsCategory.player | PhysicsCategory.coin) {
            let coinBody = (categoryA == PhysicsCategory.coin) ? contact.bodyA : contact.bodyB
            collectCoin(node: coinBody.node)
        } else if combined == (PhysicsCategory.player | PhysicsCategory.obstacle) {
            triggerGameOver()
        }
    }

    private func collectCoin(node: SKNode?) {
        guard let coin = node, coin.parent != nil else { return }

        coinCount += 1
        coinLabel.text = "🪙 \(coinCount)"

        let moveUp  = SKAction.moveBy(x: 0, y: 25, duration: 0.25)
        let fadeOut = SKAction.fadeOut(withDuration: 0.25)
        let collect = SKAction.group([moveUp, fadeOut])
        let remove  = SKAction.removeFromParent()
        coin.run(SKAction.sequence([collect, remove]))

        let scaleUp   = SKAction.scale(to: 1.35, duration: 0.08)
        let scaleDown = SKAction.scale(to: 1.0,  duration: 0.12)
        coinLabel.run(SKAction.sequence([scaleUp, scaleDown]))
    }
    
    private func triggerGameOver() {
        isGameOver = true
        isThrusting = false
        
        self.removeAction(forKey: "spawnLoop")
        self.removeAction(forKey: "coinSpawnLoop")
        player.physicsBody?.velocity = .zero
        player.physicsBody?.affectedByGravity = false
        
        self.enumerateChildNodes(withName: "obstacle") { node, _ in
            node.removeAllActions()
        }
        self.enumerateChildNodes(withName: NodeNames.coin) { node, _ in
            node.removeAllActions()
        }
        
        let defaults = UserDefaults.standard
        let previousHigh = defaults.integer(forKey: StorageKeys.highScore)
        let isNewRecord = score > previousHigh
        if isNewRecord {
            defaults.set(score, forKey: StorageKeys.highScore)
        }
        let highScoreToShow = max(previousHigh, score)
        
        let overlay = GameOverOverlay(
            sceneSize: self.size,
            score: score,
            highScore: highScoreToShow,
            isNewRecord: isNewRecord
        )
        overlay.alpha = 0
        addChild(overlay)
        overlay.run(SKAction.fadeIn(withDuration: 0.4))
        gameOverOverlay = overlay
    }
    
    private func restartGame() {
        gameOverOverlay?.removeFromParent()
        gameOverOverlay = nil
        self.enumerateChildNodes(withName: "obstacle") { node, _ in
            node.removeFromParent()
        }
        self.enumerateChildNodes(withName: NodeNames.coin) { node, _ in
            node.removeFromParent()
        }
        
        player.position = CGPoint(x: size.width * 0.2, y: size.height / 2)
        player.physicsBody?.velocity = .zero
        player.physicsBody?.affectedByGravity = true
        
        score = 0
        coinCount = 0
        elapsedTime = 0
        lastUpdateTime = 0
        scoreLabel.text = "0 m"
        coinLabel.text  = "🪙 0"
        isGameOver = false
        isThrusting = false
        
        difficultyManager.reset()
        
        scheduleNextSpawn()
        scheduleNextCoinSpawn()
    }
    
    private func goToMenu() {
        let menu = MenuScene(size: self.size)
        menu.scaleMode = .aspectFill
        menu.lastScore = score
        let transition = SKTransition.doorway(withDuration: 0.8)
        self.view?.presentScene(menu, transition: transition)
    }
}
