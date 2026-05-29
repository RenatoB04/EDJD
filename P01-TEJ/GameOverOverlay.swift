import SpriteKit

class GameOverOverlay: SKNode {

    private let sceneSize: CGSize
    private let canContinue: Bool
    private let continueCost: Int

    init(sceneSize: CGSize, score: Int, highScore: Int, isNewRecord: Bool, coinsThisRun: Int, canContinue: Bool, continueCost: Int) {
        self.sceneSize = sceneSize
        self.canContinue = canContinue
        self.continueCost = continueCost
        super.init()

        name = NodeNames.gameOverOverlay
        zPosition = 1000

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
        addChild(background)
    }

    private func setupLabels(score: Int, highScore: Int, isNewRecord: Bool, coinsThisRun: Int) {
        let title = SKLabelNode(fontNamed: "AvenirNext-Bold")
        title.text = "Fim de Jogo"
        title.fontSize = 44
        title.fontColor = .white
        title.position = CGPoint(x: sceneSize.width / 2, y: sceneSize.height * 0.76)
        title.zPosition = 1
        addChild(title)

        let scoreLabel = SKLabelNode(fontNamed: "AvenirNext-Bold")
        scoreLabel.text = "Distancia: \(score) m"
        scoreLabel.fontSize = 25
        scoreLabel.fontColor = .white
        scoreLabel.position = CGPoint(x: sceneSize.width / 2, y: sceneSize.height * 0.66)
        scoreLabel.zPosition = 1
        addChild(scoreLabel)

        let highLabel = SKLabelNode(fontNamed: "AvenirNext-Bold")
        highLabel.text = isNewRecord ? "Novo Recorde!" : "Recorde: \(highScore) m"
        highLabel.fontSize = 20
        highLabel.fontColor = isNewRecord ? .yellow : .lightGray
        highLabel.position = CGPoint(x: sceneSize.width / 2, y: sceneSize.height * 0.59)
        highLabel.zPosition = 1
        addChild(highLabel)

        let coinsLabel = SKLabelNode(fontNamed: "AvenirNext-Bold")
        coinsLabel.text = "Moedas: \(coinsThisRun)"
        coinsLabel.fontSize = 18
        coinsLabel.fontColor = SKColor(red: 1.0, green: 0.82, blue: 0.0, alpha: 1.0)
        coinsLabel.position = CGPoint(x: sceneSize.width / 2, y: sceneSize.height * 0.52)
        coinsLabel.zPosition = 1
        addChild(coinsLabel)
    }

    private func setupButtons() {
        var buttonY = sceneSize.height * 0.39

        if canContinue {
            let continueButton = makeButton(
                text: "Continuar: -\(continueCost)",
                size: CGSize(width: 190, height: 48),
                color: .systemBlue,
                name: NodeNames.continueButton
            )
            continueButton.position = CGPoint(x: sceneSize.width / 2, y: buttonY)
            addChild(continueButton)
            buttonY -= 62
        }

        let retryButton = makeButton(
            text: "Jogar de Novo",
            size: CGSize(width: 190, height: 48),
            color: .systemGreen,
            name: NodeNames.retryButton
        )
        retryButton.position = CGPoint(x: sceneSize.width / 2, y: buttonY)
        addChild(retryButton)

        let menuButton = makeButton(
            text: "Menu",
            size: CGSize(width: 190, height: 48),
            color: .systemGray,
            name: NodeNames.menuButton
        )
        menuButton.position = CGPoint(x: sceneSize.width / 2, y: buttonY - 62)
        addChild(menuButton)
    }

    private func makeButton(text: String, size: CGSize, color: SKColor, name: String) -> SKShapeNode {
        let rect = CGRect(x: -size.width / 2, y: -size.height / 2, width: size.width, height: size.height)
        let button = SKShapeNode(rect: rect, cornerRadius: size.height * 0.3)
        button.name = name
        button.fillColor = color
        button.strokeColor = SKColor.white.withAlphaComponent(0.25)
        button.lineWidth = 1.5
        button.zPosition = 1

        let label = SKLabelNode(fontNamed: "AvenirNext-Bold")
        label.text = text
        label.fontSize = 18
        label.fontColor = .white
        label.verticalAlignmentMode = .center
        label.horizontalAlignmentMode = .center
        label.name = name
        button.addChild(label)

        return button
    }

    func buttonName(at location: CGPoint) -> String? {
        for node in nodes(at: location) {
            if node.name == NodeNames.continueButton || node.name == NodeNames.retryButton || node.name == NodeNames.menuButton {
                return node.name
            }
        }
        return nil
    }
}
