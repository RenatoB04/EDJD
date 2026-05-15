import SpriteKit

class PauseOverlay: SKNode {

    private let sceneSize: CGSize

    init(sceneSize: CGSize) {
        self.sceneSize = sceneSize
        super.init()

        self.name = NodeNames.pauseOverlay
        self.zPosition = 950

        let bg = SKSpriteNode(color: .black, size: sceneSize)
        bg.alpha = 0.6
        bg.position = CGPoint(x: sceneSize.width / 2, y: sceneSize.height / 2)
        addChild(bg)

        let title = SKLabelNode(fontNamed: "AvenirNext-Bold")
        title.text = "Em Pausa"
        title.fontSize = 40
        title.fontColor = .white
        title.position = CGPoint(x: sceneSize.width / 2, y: sceneSize.height * 0.66)
        addChild(title)

        addChild(makeButton(text: "Continuar",
                            position: CGPoint(x: sceneSize.width / 2, y: sceneSize.height * 0.48),
                            color: .systemGreen,
                            name: NodeNames.resumeButton))

        addChild(makeButton(text: "Menu",
                            position: CGPoint(x: sceneSize.width / 2, y: sceneSize.height * 0.36),
                            color: .systemGray,
                            name: NodeNames.menuButton))
    }

    required init?(coder aDecoder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }

    private func makeButton(text: String, position: CGPoint, color: SKColor, name: String) -> SKSpriteNode {
        let button = SKSpriteNode(color: color, size: CGSize(width: 180, height: 50))
        button.position = position
        button.name = name

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
            if node.name == NodeNames.resumeButton || node.name == NodeNames.menuButton {
                return node.name
            }
        }
        return nil
    }
}
