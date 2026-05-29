import SpriteKit

class MenuScene: SKScene {

    var lastScore: Int? = nil

    override func didMove(to view: SKView) {
        backgroundColor = SKColor(red: 0.05, green: 0.05, blue: 0.12, alpha: 1.0)
        AudioManager.shared.startMusic()

        let title = SKLabelNode(text: "Astro Drift")
        title.fontName = "AvenirNext-Bold"
        title.fontSize = 44
        title.fontColor = .white
        title.position = CGPoint(x: size.width / 2, y: size.height * 0.78)
        addChild(title)

        let highScore = UserDefaults.standard.integer(forKey: StorageKeys.highScore)

        let highLabel = SKLabelNode(fontNamed: "AvenirNext-Bold")
        highLabel.text = "Recorde: \(highScore) m"
        highLabel.fontSize = 22
        highLabel.fontColor = .yellow
        highLabel.position = CGPoint(x: size.width / 2, y: size.height * 0.68)
        addChild(highLabel)

        let wallet = UserDefaults.standard.integer(forKey: StorageKeys.coinWallet)
        let walletLabel = SKLabelNode(fontNamed: "AvenirNext-Bold")
        walletLabel.text = "Moedas: \(wallet)"
        walletLabel.fontSize = 20
        walletLabel.fontColor = SKColor(red: 1.0, green: 0.82, blue: 0.0, alpha: 1.0)
        walletLabel.position = CGPoint(x: size.width / 2, y: size.height * 0.62)
        addChild(walletLabel)

        if let lastScore = lastScore {
            let lastLabel = SKLabelNode(fontNamed: "AvenirNext")
            lastLabel.text = "Ultima: \(lastScore) m"
            lastLabel.fontSize = 18
            lastLabel.fontColor = .white
            lastLabel.position = CGPoint(x: size.width / 2, y: size.height * 0.56)
            addChild(lastLabel)
        }

        let preview = SKSpriteNode(texture: SKTexture(imageNamed: AssetNames.player))
        preview.size = CGSize(width: 70, height: 70)
        preview.position = CGPoint(x: size.width / 2, y: size.height * 0.46)
        addChild(preview)

        let float = SKAction.sequence([
            SKAction.moveBy(x: 0, y: 6, duration: 1.0),
            SKAction.moveBy(x: 0, y: -6, duration: 1.0)
        ])
        preview.run(SKAction.repeatForever(float))

        addChild(makeButton(text: "Jogar",
                            color: .systemGreen,
                            position: CGPoint(x: size.width / 2, y: size.height * 0.30),
                            size: CGSize(width: 220, height: 50),
                            name: NodeNames.playButton))
    }

    private func makeButton(text: String, color: SKColor, position: CGPoint, size: CGSize, name: String) -> SKShapeNode {
        let cornerRadius = size.height * 0.3
        let rect = CGRect(x: -size.width / 2, y: -size.height / 2, width: size.width, height: size.height)

        let button = SKShapeNode(rect: rect, cornerRadius: cornerRadius)
        button.position = position
        button.name = name
        button.fillColor = color
        button.strokeColor = SKColor.white.withAlphaComponent(0.25)
        button.lineWidth = 1.5

        let label = SKLabelNode(fontNamed: "AvenirNext-Bold")
        label.text = text
        label.fontSize = 20
        label.fontColor = .white
        label.verticalAlignmentMode = .center
        label.horizontalAlignmentMode = .center
        label.name = name
        button.addChild(label)

        return button
    }

    override func touchesBegan(_ touches: Set<UITouch>, with event: UIEvent?) {
        guard let touch = touches.first else { return }
        let location = touch.location(in: self)
        let names = nodes(at: location).compactMap { $0.name }

        if names.contains(NodeNames.playButton) {
            startGame()
        }
    }

    private func startGame() {
        run(SKAction.playSoundFileNamed("Sounds/button.wav", waitForCompletion: false))

        let game = GameScene(size: size)
        game.scaleMode = .aspectFill
        view?.presentScene(game, transition: .crossFade(withDuration: 0.6))
    }
}
