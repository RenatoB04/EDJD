import SpriteKit

class CoinNode: SKShapeNode {

    override init() {
        super.init()

        let radius = CoinConfig.radius
        path = CGPath(ellipseIn: CGRect(x: -radius, y: -radius, width: radius * 2, height: radius * 2), transform: nil)
        fillColor = SKColor(red: 1.0, green: 0.82, blue: 0.0, alpha: 1.0)
        strokeColor = SKColor(red: 0.9, green: 0.6, blue: 0.0, alpha: 1.0)
        lineWidth = 2
        name = NodeNames.coin

        physicsBody = SKPhysicsBody(circleOfRadius: radius)
        physicsBody?.isDynamic = false
        physicsBody?.categoryBitMask = PhysicsCategory.coin
        physicsBody?.contactTestBitMask = PhysicsCategory.player
        physicsBody?.collisionBitMask = PhysicsCategory.none
    }

    required init?(coder aDecoder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }
}
