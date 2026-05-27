import SpriteKit

class PlayerNode: SKSpriteNode {

    private(set) var trail: SKEmitterNode!

    init() {
        let texture = SKTexture(imageNamed: AssetNames.player)
        super.init(texture: texture, color: .clear, size: PlayerConfig.size)

        name = NodeNames.player
        setupPhysics()
        setupTrail()
    }

    required init?(coder aDecoder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }

    private func setupPhysics() {
        let bodySize = CGSize(width: size.width * 0.7, height: size.height * 0.6)
        physicsBody = SKPhysicsBody(rectangleOf: bodySize)
        physicsBody?.categoryBitMask = PhysicsCategory.player
        physicsBody?.contactTestBitMask = PhysicsCategory.obstacle
        physicsBody?.collisionBitMask = PhysicsCategory.none
        physicsBody?.affectedByGravity = true
        physicsBody?.allowsRotation = false
    }

    private func setupTrail() {
        trail = Effects.makeJetpackTrail()
        trail.position = CGPoint(x: -size.width * 0.4, y: 0)
        trail.particleBirthRate = 0
        addChild(trail)
    }

    func setThrusting(_ thrusting: Bool) {
        trail.particleBirthRate = thrusting ? 140 : 0
    }
}
