import SpriteKit

class GameOverOverlay: SKNode {

    private let sceneSize: CGSize

    init(sceneSize: CGSize, score: Int, highScore: Int, isNewRecord: Bool, coinsThisRun: Int) {
        self.sceneSize = sceneSize
        super.init()

        self.name = NodeNames.gameOverOverlay
        self.zPosition = 1000

        setupBackground()
        setupLabels(score: score, highScore: highScore, isNewRecord: isNewRecord, coinsThisRun: coinsThisRun)
        setupButtons()
    }

    required init?(coder aDecoder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }

    private func setupBackground() {
        let background = SKSpriteNode(color: .black, size: sceneSize)
        background.alpha = 0.65
        background.position = CGPoint(x: sceneSize.width / 2, y: sceneSize.height / 2)
        background.zPosition = 0
        addChild(background)
    }

    private func setupLabels(score: Int, highScore: Int, isNewRecord: Bool, coinsThisRun: Int) {
        let title = SKLabelNode(fontNamed: "AvenirNext-Bold")
        title.text = "Fim de Jogo"
        title.fontSize = 44
        title.fontColor = .white
        title.position = CGPoint(x: sceneSize.width / 2, y: sceneSize.height * 0.72)
        title.zPosition = 1
        addChild(title)

        let scoreLabel = SKLabelNode(fontNamed: "AvenirNext-Bold")
        scoreLabel.text = "Distância: \(score) m"
        scoreLabel.fontSize = 26
        scoreLabel.fontColor = .white
        scoreLabel.position = CGPoint(x: sceneSize.width / 2, y: sceneSize.height * 0.62)
        scoreLabel.zPosition = 1
        addChild(scoreLabel)

        let highLabel = SKLabelNode(fontNamed: "AvenirNext-Bold")
        if isNewRecord {
            highLabel.text = "✨ Novo Recorde! ✨"
            highLabel.fontColor = .yellow
        } else {
            highLabel.text = "Recorde: \(highScore) m"
            highLabel.fontColor = .lightGray
        }
        highLabel.fontSize = 20
        highLabel.position = CGPoint(x: sceneSize.width / 2, y: sceneSize.height * 0.55)
        highLabel.zPosition = 1
        addChild(highLabel)

        let coinLabel = SKLabelNode(fontNamed: "AvenirNext-Bold")
        coinLabel.text = "🪙 \(coinsThisRun) moedas"
        coinLabel.fontSize = 18
        coinLabel.fontColor = SKColor(red: 1.0, green: 0.82, blue: 0.0, alpha: 1.0)
        coinLabel.position = CGPoint(x: sceneSize.width / 2, y: sceneSize.height * 0.49)
        coinLabel.zPosition = 1
        addChild(coinLabel)
    }

    private func setupButtons() {
        let buttonWidth: CGFloat = 180
        let buttonHeight: CGFloat = 50
        let buttonY = sceneSize.height * 0.33

        let retryButton = makeButton(
            text: "Jogar de Novo",
            size: CGSize(width: buttonWidth, height: buttonHeight),
            color: .systemGreen,
            name: NodeNames.retryButton
        )
        retryButton.position = CGPoint(x: sceneSize.width / 2, y: buttonY)
        addChild(retryButton)

        let menuButton = makeButton(
            text: "Menu",
            size: CGSize(width: buttonWidth, height: buttonHeight),
            color: .systemGray,
            name: NodeNames.menuButton
        )
        menuButton.position = CGPoint(x: sceneSize.width / 2, y: buttonY - 70)
        addChild(menuButton)
    }

    private func makeButton(text: String, size: CGSize, color: SKColor, name: String) -> SKSpriteNode {
        let button = SKSpriteNode(color: color, size: size)
        button.name = name
        button.zPosition = 1

        let label = SKLabelNode(fontNamed: "AvenirNext-Bold")
        label.text = text
        label.fontSize = 18
        label.fontColor = .white
        label.verticalAlignmentMode = .center
        label.horizontalAlignmentMode = .center
        label.position = .zero
        label.zPosition = 1
        label.name = name
        button.addChild(label)

        return button
    }

    func buttonName(at location: CGPoint) -> String? {
        for node in nodes(at: location) {
            if node.name == NodeNames.retryButton || node.name == NodeNames.menuButton {
                return node.name
            }
        }
        return nil
    }
}
