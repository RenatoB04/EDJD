import SpriteKit

class PlayerNode: SKSpriteNode {

    private(set) var trail: SKEmitterNode!
    private(set) var skin: Skin

    var hasShield: Bool = false {
        didSet { refreshShieldAura() }
    }

    private weak var shieldAura: ShieldAuraNode?

    init() {
        let skin = PlayerInventory.equippedSkin
        self.skin = skin
        let texture = SKTexture(imageNamed: skin.assetName)
        super.init(texture: texture, color: .clear, size: PlayerConfig.size)

        self.name = NodeNames.player
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
        physicsBody?.contactTestBitMask = PhysicsCategory.obstacle | PhysicsCategory.coin | PhysicsCategory.shieldPickup
        physicsBody?.collisionBitMask = PhysicsCategory.none
        physicsBody?.affectedByGravity = true
        physicsBody?.allowsRotation = false
    }

    private func setupTrail() {
        trail = Effects.makeJetpackTrail()
        Effects.tintTrail(trail, color: skin.trailColor)
        trail.position = CGPoint(x: -size.width * 0.4, y: 0)
        trail.particleBirthRate = 0
        addChild(trail)
    }

    func setThrusting(_ thrusting: Bool) {
        trail.particleBirthRate = thrusting ? 140 : 0
    }

    private func refreshShieldAura() {
        if hasShield {
            guard shieldAura == nil else { return }
            let aura = ShieldAuraNode()
            addChild(aura)
            shieldAura = aura
        } else {
            shieldAura?.removeFromParent()
            shieldAura = nil
        }
    }

    func reloadSkin() {
        self.skin = PlayerInventory.equippedSkin
        self.texture = SKTexture(imageNamed: skin.assetName)
        Effects.tintTrail(trail, color: skin.trailColor)
    }
}
