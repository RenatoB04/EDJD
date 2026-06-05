import SpriteKit

// Cena principal do jogo.
// Controla input, física, score, spawn de obstáculos/moedas, pausa e game over.
class GameScene: SKScene, SKPhysicsContactDelegate {

    // Nós e classes auxiliares usados durante a run.
    let player = PlayerNode()
    let asteroidSpawner = ObstacleSpawner()
    let laserSpawner = LaserSpawner()
    let coinSpawner = CoinSpawner()
    let difficultyManager = DifficultyManager()

    // Estados simples para saber se o jogador está a subir, se perdeu ou se pausou.
    var isThrusting = false
    var isGameOver = false
    private var isManuallyPaused = false

    // Score e moedas pertencem apenas à run actual.
    private var score: Int = 0
    private var rawScore: Double = 0
    private var coinsThisRun: Int = 0
    // O continue começa em 10 moedas e duplica sempre que é usado na mesma run.
    private var currentContinueCost = CoinConfig.continueCost

    // Guardam o tempo entre frames para calcular movimento e score de forma estável.
    private var lastUpdateTime: TimeInterval = 0
    private var elapsedTime: TimeInterval = 0

    // Elementos fixos da interface durante o jogo.
    private let scoreLabel = SKLabelNode(fontNamed: "AvenirNext-Bold")
    private let coinLabel = SKLabelNode(fontNamed: "AvenirNext-Bold")
    private var pauseButton: SKShapeNode!

    // O worldNode contém os objectos do jogo. O HUD fica fora dele.
    // Assim podemos aplicar screen shake só ao mundo, sem abanar os textos.
    private var starField: StarField?
    private var worldNode: SKNode!

    // Referências fracas aos overlays para os poder remover quando já não são necessários.
    private weak var gameOverOverlay: GameOverOverlay?
    private weak var pauseOverlay: PauseOverlay?

    // Chaves para identificar acções SpriteKit e evitar duplicar loops de spawn.
    private static let spawnLoopKey = "spawnLoop"
    private static let coinSpawnLoopKey = "coinSpawnLoop"

    // Chamado quando a cena aparece no SKView.
    override func didMove(to view: SKView) {
        backgroundColor = .black
        size = view.bounds.size

        // Gravidade negativa: o jogador cai quando não está a tocar no ecrã.
        physicsWorld.gravity = CGVector(dx: 0, dy: GameConfig.gravity)
        physicsWorld.contactDelegate = self

        worldNode = SKNode()
        addChild(worldNode)

        setupStarField()

        player.position = CGPoint(x: size.width * 0.2, y: size.height / 2)
        worldNode.addChild(player)

        setupHUD()
        updateCoinLabel()
        AudioManager.shared.startMusic()
        startGameplay()
    }

    // Cria o fundo de estrelas dentro do mundo do jogo.
    private func setupStarField() {
        let field = StarField(sceneSize: size)
        worldNode.addChild(field)
        starField = field
    }

    // Cria score, moedas e botão de pausa.
    private func setupHUD() {
        scoreLabel.text = "0 m"
        scoreLabel.fontSize = 28
        scoreLabel.fontColor = .white
        scoreLabel.horizontalAlignmentMode = .right
        scoreLabel.position = CGPoint(x: size.width - 20, y: size.height - 50)
        scoreLabel.zPosition = 100
        addChild(scoreLabel)

        coinLabel.fontSize = 20
        coinLabel.fontColor = SKColor(red: 1.0, green: 0.82, blue: 0.0, alpha: 1.0)
        coinLabel.horizontalAlignmentMode = .right
        coinLabel.position = CGPoint(x: size.width - 20, y: size.height - 78)
        coinLabel.zPosition = 100
        addChild(coinLabel)

        let pauseSize = CGSize(width: 44, height: 44)
        let rect = CGRect(x: -pauseSize.width / 2, y: -pauseSize.height / 2, width: pauseSize.width, height: pauseSize.height)
        pauseButton = SKShapeNode(rect: rect, cornerRadius: pauseSize.height * 0.3)
        pauseButton.position = CGPoint(x: 30, y: size.height - 50)
        pauseButton.name = NodeNames.pauseButton
        pauseButton.fillColor = SKColor(white: 0.2, alpha: 0.7)
        pauseButton.strokeColor = SKColor.white.withAlphaComponent(0.25)
        pauseButton.lineWidth = 1.5
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

    // Começa os loops de spawn da run.
    private func startGameplay() {
        scheduleNextSpawn()
        scheduleNextCoinSpawn()
    }

    // Agenda o próximo obstáculo. No fim chama-se a si própria para continuar o ciclo.
    private func scheduleNextSpawn() {
        removeAction(forKey: GameScene.spawnLoopKey)

        let wait = SKAction.wait(forDuration: difficultyManager.currentSpawnRate())
        let spawn = SKAction.run { [weak self] in
            guard let self = self, !self.isGameOver, !self.isManuallyPaused else { return }

            let duration = self.difficultyManager.currentObstacleDuration()
            // Há uma pequena probabilidade de sair laser em vez de asteroide.
            if CGFloat.random(in: 0...1) < SpawnConfig.laserChance {
                self.laserSpawner.spawn(in: self.worldNode, moveDuration: duration, sceneSize: self.size)
            } else {
                self.asteroidSpawner.spawn(in: self.worldNode, moveDuration: duration, sceneSize: self.size)
            }

            self.scheduleNextSpawn()
        }

        run(SKAction.sequence([wait, spawn]), withKey: GameScene.spawnLoopKey)
    }

    // Agenda moedas de forma parecida aos obstáculos.
    private func scheduleNextCoinSpawn() {
        removeAction(forKey: GameScene.coinSpawnLoopKey)

        let wait = SKAction.wait(forDuration: CoinConfig.spawnRate)
        let spawn = SKAction.run { [weak self] in
            guard let self = self, !self.isGameOver, !self.isManuallyPaused else { return }

            self.coinSpawner.spawn(
                in: self.worldNode,
                moveDuration: self.difficultyManager.currentObstacleDuration(),
                sceneSize: self.size
            )
            self.scheduleNextCoinSpawn()
        }

        run(SKAction.sequence([wait, spawn]), withKey: GameScene.coinSpawnLoopKey)
    }

    // Primeiro tratamos toques em overlays/botões. Se não houver botão, o toque activa o impulso.
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
        AudioManager.shared.startThrust()
    }

    // Quando o jogador larga o ecrã, o foguete deixa de subir.
    override func touchesEnded(_ touches: Set<UITouch>, with event: UIEvent?) {
        stopThrust()
    }

    override func touchesCancelled(_ touches: Set<UITouch>, with event: UIEvent?) {
        stopThrust()
    }

    // Centraliza tudo o que acontece ao parar o impulso.
    private func stopThrust() {
        isThrusting = false
        player.setThrusting(false)
        AudioManager.shared.stopThrust()
    }

    // Trata os botões disponíveis no fim de jogo.
    private func handleGameOverTouch(at location: CGPoint) {
        guard let overlay = gameOverOverlay,
              let buttonName = overlay.buttonName(at: location) else { return }

        run(SKAction.playSoundFileNamed("Sounds/button.wav", waitForCompletion: false))

        switch buttonName {
        case NodeNames.continueButton: continueGame()
        case NodeNames.retryButton: restartGame()
        case NodeNames.menuButton: goToMenu()
        default: break
        }
    }

    // Trata os botões do menu de pausa.
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

    // Pausar significa parar física, spawns, mundo e som de impulso.
    private func pauseGame() {
        guard !isManuallyPaused, !isGameOver else { return }

        isManuallyPaused = true
        removeAction(forKey: GameScene.spawnLoopKey)
        removeAction(forKey: GameScene.coinSpawnLoopKey)
        worldNode.isPaused = true
        physicsWorld.speed = 0
        stopThrust()
        AudioManager.shared.pauseAll()

        let overlay = PauseOverlay(sceneSize: size)
        addChild(overlay)
        pauseOverlay = overlay
    }

    // Ao retomar, reiniciamos o tempo para evitar um salto grande no update.
    private func resumeGame() {
        isManuallyPaused = false
        worldNode.isPaused = false
        player.physicsBody?.velocity = .zero
        physicsWorld.speed = 1
        lastUpdateTime = 0
        startGameplay()

        AudioManager.shared.resumeMusic()
        pauseOverlay?.removeFromParent()
        pauseOverlay = nil
    }

    // Loop chamado pelo SpriteKit a cada frame.
    override func update(_ currentTime: TimeInterval) {
        guard !isGameOver, !isManuallyPaused else { return }

        if lastUpdateTime == 0 { lastUpdateTime = currentTime }
        let dt = currentTime - lastUpdateTime
        lastUpdateTime = currentTime

        elapsedTime += dt
        difficultyManager.update(elapsedTime: elapsedTime)
        starField?.update(deltaTime: dt, speedMultiplier: difficultyManager.currentParallaxMultiplier())

        // O score aumenta com o tempo, simulando distância percorrida.
        rawScore += dt * Double(GameConfig.scoreMultiplier)
        score = Int(rawScore)
        scoreLabel.text = "\(score) m"

        // Enquanto o toque está activo, aplicamos força para cima.
        if isThrusting {
            player.physicsBody?.applyForce(CGVector(dx: 0, dy: GameConfig.thrustForce))
            if let vy = player.physicsBody?.velocity.dy, vy > GameConfig.maxVelocity {
                player.physicsBody?.velocity.dy = GameConfig.maxVelocity
            }
        }

        // Sair completamente por cima ou por baixo também conta como morte.
        let margin = player.size.height / 2
        if player.position.y > size.height + margin || player.position.y < -margin {
            triggerGameOver()
        }
    }

    // Chamado automaticamente pelo SpriteKit quando dois corpos com contacto se tocam.
    func didBegin(_ contact: SKPhysicsContact) {
        guard !isGameOver else { return }

        let combined = contact.bodyA.categoryBitMask | contact.bodyB.categoryBitMask
        // Usamos a combinação das categorias para saber que tipo de colisão aconteceu.
        if combined == (PhysicsCategory.player | PhysicsCategory.coin) {
            let coinBody = contact.bodyA.categoryBitMask == PhysicsCategory.coin ? contact.bodyA : contact.bodyB
            collectCoin(coinBody.node)
        } else if combined == (PhysicsCategory.player | PhysicsCategory.obstacle) {
            triggerGameOver()
        }
    }

    // Recolhe a moeda, aumenta o contador da run e faz uma pequena animação de saída.
    private func collectCoin(_ node: SKNode?) {
        guard let coin = node, coin.parent != nil else { return }

        coinsThisRun += 1
        updateCoinLabel()
        run(SKAction.playSoundFileNamed("Sounds/coin.wav", waitForCompletion: false))

        coin.removeAllActions()
        coin.physicsBody = nil
        coin.run(SKAction.sequence([
            SKAction.group([
                SKAction.moveBy(x: 0, y: 20, duration: 0.2),
                SKAction.fadeOut(withDuration: 0.2)
            ]),
            SKAction.removeFromParent()
        ]))
    }

    // Estado de morte: para spawns, actualiza recorde e mostra o overlay.
    private func triggerGameOver() {
        isGameOver = true
        stopThrust()
        removeAction(forKey: GameScene.spawnLoopKey)
        removeAction(forKey: GameScene.coinSpawnLoopKey)

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
        worldNode.enumerateChildNodes(withName: NodeNames.coin) { node, _ in
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
            isNewRecord: isNewRecord,
            coinsThisRun: coinsThisRun,
            canContinue: canContinue(),
            continueCost: currentContinueCost
        )
        overlay.alpha = 0
        addChild(overlay)
        overlay.run(SKAction.sequence([
            SKAction.wait(forDuration: 0.6),
            SKAction.fadeIn(withDuration: 0.4)
        ]))
        gameOverOverlay = overlay
    }

    // Só é possível continuar se houver moedas suficientes para o custo actual.
    private func canContinue() -> Bool {
        return coinsThisRun >= currentContinueCost
    }

    // Continua a mesma run: mantém score e dificuldade, mas limpa perigos próximos.
    private func continueGame() {
        guard canContinue() else { return }

        coinsThisRun -= currentContinueCost
        currentContinueCost *= 2
        updateCoinLabel()

        gameOverOverlay?.removeFromParent()
        gameOverOverlay = nil

        worldNode.enumerateChildNodes(withName: NodeNames.obstacle) { node, _ in
            node.removeFromParent()
        }
        worldNode.enumerateChildNodes(withName: NodeNames.coin) { node, _ in
            node.removeFromParent()
        }

        player.alpha = 1.0
        player.setScale(1.0)
        player.position = CGPoint(x: size.width * 0.2, y: size.height / 2)
        player.physicsBody?.velocity = .zero
        player.physicsBody?.affectedByGravity = true

        lastUpdateTime = 0
        isGameOver = false
        isThrusting = false
        startGameplay()
    }

    // Reinicia tudo o que pertence à run.
    private func restartGame() {
        gameOverOverlay?.removeFromParent()
        gameOverOverlay = nil

        worldNode.enumerateChildNodes(withName: NodeNames.obstacle) { node, _ in
            node.removeFromParent()
        }
        worldNode.enumerateChildNodes(withName: NodeNames.coin) { node, _ in
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
        coinsThisRun = 0
        currentContinueCost = CoinConfig.continueCost
        elapsedTime = 0
        lastUpdateTime = 0
        scoreLabel.text = "0 m"
        updateCoinLabel()
        isGameOver = false
        isThrusting = false

        difficultyManager.reset()
        startGameplay()
    }

    // Actualiza a label das moedas apanhadas na run actual.
    private func updateCoinLabel() {
        coinLabel.text = "Moedas: \(coinsThisRun)"
    }

    // Volta ao menu e passa a última pontuação para ser mostrada lá.
    private func goToMenu() {
        AudioManager.shared.stopThrust()

        let menu = MenuScene(size: size)
        menu.scaleMode = .aspectFill
        menu.lastScore = score
        view?.presentScene(menu, transition: .doorway(withDuration: 0.7))
    }
}
