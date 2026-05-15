import Foundation
import SpriteKit

class CoinNode: SKShapeNode {

    override init() {
        super.init()

        let radius = CoinConfig.radius

        self.path = CGPath(ellipseIn: CGRect(x: -radius, y: -radius,
                                             width: radius * 2, height: radius * 2),
                           transform: nil)
        self.fillColor   = SKColor(red: 1.0, green: 0.82, blue: 0.0, alpha: 1.0)
        self.strokeColor = SKColor(red: 0.9, green: 0.65, blue: 0.0, alpha: 1.0)
        self.lineWidth   = 2.0
        self.name        = NodeNames.coin

        setupPhysics(radius: radius)
        addSpinAnimation()
    }

    required init?(coder aDecoder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }

    private func setupPhysics(radius: CGFloat) {
        physicsBody = SKPhysicsBody(circleOfRadius: radius * 0.85)
        physicsBody?.isDynamic         = false
        physicsBody?.categoryBitMask   = PhysicsCategory.coin
        physicsBody?.contactTestBitMask = PhysicsCategory.player
        physicsBody?.collisionBitMask  = PhysicsCategory.none
    }

    private func addSpinAnimation() {
        let scaleDown  = SKAction.scaleX(to: 0.15, duration: 0.45)
        let scaleUp    = SKAction.scaleX(to: 1.0,  duration: 0.45)
        let flip       = SKAction.sequence([scaleDown, scaleUp])
        run(SKAction.repeatForever(flip))
    }
}

