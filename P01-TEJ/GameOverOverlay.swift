import SpriteKit

class GameOverOverlay: SKNode {

    private let sceneSize: CGSize

    init(sceneSize: CGSize, score: Int, highScore: Int, isNewRecord: Bool) {
        self.sceneSize = sceneSize
        super.init()

        name = NodeNames.gameOverOverlay
        zPosition = 1000

        setupBackground()
        setupLabels(score: score, highScore: highScore, isNewRecord: isNewRecord)
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

    private func setupLabels(score: Int, highScore: Int, isNewRecord: Bool) {
        let title = SKLabelNode(fontNamed: "AvenirNext-Bold")
        title.text = "Fim de Jogo"
        title.fontSize = 44
        title.fontColor = .white
        title.position = CGPoint(x: sceneSize.width / 2, y: sceneSize.height * 0.72)
        title.zPosition = 1
        addChild(title)

        let scoreLabel = SKLabelNode(fontNamed: "AvenirNext-Bold")
        scoreLabel.text = "Distancia: \(score) m"
        scoreLabel.fontSize = 26
        scoreLabel.fontColor = .white
        scoreLabel.position = CGPoint(x: sceneSize.width / 2, y: sceneSize.height * 0.62)
        scoreLabel.zPosition = 1
        addChild(scoreLabel)

        let highLabel = SKLabelNode(fontNamed: "AvenirNext-Bold")
        highLabel.text = isNewRecord ? "Novo Recorde!" : "Recorde: \(highScore) m"
        highLabel.fontSize = 20
        highLabel.fontColor = isNewRecord ? .yellow : .lightGray
        highLabel.position = CGPoint(x: sceneSize.width / 2, y: sceneSize.height * 0.55)
        highLabel.zPosition = 1
        addChild(highLabel)
    }

    private func setupButtons() {
        let retryButton = makeButton(
            text: "Jogar de Novo",
            size: CGSize(width: 180, height: 50),
            color: .systemGreen,
            name: NodeNames.retryButton
        )
        retryButton.position = CGPoint(x: sceneSize.width / 2, y: sceneSize.height * 0.34)
        addChild(retryButton)

        let menuButton = makeButton(
            text: "Menu",
            size: CGSize(width: 180, height: 50),
            color: .systemGray,
            name: NodeNames.menuButton
        )
        menuButton.position = CGPoint(x: sceneSize.width / 2, y: sceneSize.height * 0.22)
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
