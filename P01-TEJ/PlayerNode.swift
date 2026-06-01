import SpriteKit

// Nó do jogador: é o foguete que o utilizador controla.
class PlayerNode: SKSpriteNode {

    // Chama visual simples que aparece quando há impulso.
    private let flame = SKShapeNode()

    init() {
        let texture = SKTexture(imageNamed: AssetNames.player)
        super.init(texture: texture, color: .clear, size: PlayerConfig.size)

        name = NodeNames.player
        setupPhysics()
        setupFlame()
    }

    required init?(coder aDecoder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }

    // Configura a física do jogador para detectar obstáculos e moedas.
    private func setupPhysics() {
        let bodySize = CGSize(width: size.width * 0.7, height: size.height * 0.6)
        physicsBody = SKPhysicsBody(rectangleOf: bodySize)
        physicsBody?.categoryBitMask = PhysicsCategory.player
        physicsBody?.contactTestBitMask = PhysicsCategory.obstacle | PhysicsCategory.coin
        physicsBody?.collisionBitMask = PhysicsCategory.none
        physicsBody?.affectedByGravity = true
        physicsBody?.allowsRotation = false
    }

    // A chama é um triângulo desenhado por código atrás do foguete.
    private func setupFlame() {
        let path = CGMutablePath()
        path.move(to: CGPoint(x: -size.width * 0.45, y: 0))
        path.addLine(to: CGPoint(x: -size.width * 0.8, y: 9))
        path.addLine(to: CGPoint(x: -size.width * 0.8, y: -9))
        path.closeSubpath()

        flame.path = path
        flame.fillColor = .orange
        flame.strokeColor = .clear
        flame.isHidden = true
        flame.zPosition = -1
        addChild(flame)
    }

    // Mostra ou esconde a chama consoante o jogador esteja a tocar no ecrã.
    func setThrusting(_ thrusting: Bool) {
        flame.isHidden = !thrusting
    }
}
