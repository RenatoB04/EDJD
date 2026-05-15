import SpriteKit

class ShieldPickupNode: SKShapeNode {

    override init() {
        super.init()
        let radius: CGFloat = 14

        path = CGPath(ellipseIn: CGRect(x: -radius, y: -radius, width: radius * 2, height: radius * 2),
                      transform: nil)
        fillColor   = SKColor(red: 0.25, green: 0.75, blue: 1.0, alpha: 1.0)
        strokeColor = SKColor.white
        lineWidth   = 2.0
        name        = NodeNames.shieldPickup

        physicsBody = SKPhysicsBody(circleOfRadius: radius * 0.9)
        physicsBody?.isDynamic = false
        physicsBody?.categoryBitMask = PhysicsCategory.shieldPickup
        physicsBody?.contactTestBitMask = PhysicsCategory.player
        physicsBody?.collisionBitMask = PhysicsCategory.none

        let pulse = SKAction.sequence([
            SKAction.scale(to: 1.18, duration: 0.4),
            SKAction.scale(to: 1.0,  duration: 0.4)
        ])
        run(SKAction.repeatForever(pulse))
    }

    required init?(coder aDecoder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }
}

class ShieldAuraNode: SKShapeNode {

    override init() {
        super.init()
        let radius: CGFloat = 32

        path = CGPath(ellipseIn: CGRect(x: -radius, y: -radius, width: radius * 2, height: radius * 2),
                      transform: nil)
        fillColor   = SKColor(red: 0.4, green: 0.85, blue: 1.0, alpha: 0.25)
        strokeColor = SKColor(red: 0.4, green: 0.85, blue: 1.0, alpha: 0.9)
        lineWidth   = 2.0
        zPosition   = -1
        name        = NodeNames.shieldAura

        let pulse = SKAction.sequence([
            SKAction.fadeAlpha(to: 0.6, duration: 0.4),
            SKAction.fadeAlpha(to: 1.0, duration: 0.4)
        ])
        run(SKAction.repeatForever(pulse))
    }

    required init?(coder aDecoder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }
}
