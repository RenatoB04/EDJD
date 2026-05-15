import SpriteKit

class TutorialOverlay: SKNode {

    init(sceneSize: CGSize) {
        super.init()
        self.name = NodeNames.tutorialOverlay
        self.zPosition = 800

        let bg = SKSpriteNode(color: .black, size: sceneSize)
        bg.alpha = 0.55
        bg.position = CGPoint(x: sceneSize.width / 2, y: sceneSize.height / 2)
        addChild(bg)

        let title = SKLabelNode(fontNamed: "AvenirNext-Bold")
        title.text = "Como Jogar"
        title.fontSize = 34
        title.fontColor = .white
        title.position = CGPoint(x: sceneSize.width / 2, y: sceneSize.height * 0.68)
        addChild(title)

        let lines = [
            "🚀 Toca e mantém para subir",
            "🪙 Apanha moedas pelo caminho",
            "🛡️ Apanha escudos para sobreviver",
            "☄️ Foge aos asteróides e lasers"
        ]

        for (i, text) in lines.enumerated() {
            let label = SKLabelNode(fontNamed: "AvenirNext")
            label.text = text
            label.fontSize = 18
            label.fontColor = .white
            label.position = CGPoint(x: sceneSize.width / 2,
                                     y: sceneSize.height * 0.55 - CGFloat(i) * 32)
            addChild(label)
        }

        let cta = SKLabelNode(fontNamed: "AvenirNext-Bold")
        cta.text = "Toca para começar"
        cta.fontSize = 22
        cta.fontColor = .yellow
        cta.position = CGPoint(x: sceneSize.width / 2, y: sceneSize.height * 0.22)
        addChild(cta)

        let pulse = SKAction.sequence([
            SKAction.fadeAlpha(to: 0.4, duration: 0.6),
            SKAction.fadeAlpha(to: 1.0, duration: 0.6)
        ])
        cta.run(SKAction.repeatForever(pulse))
    }

    required init?(coder aDecoder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }
}
