import SpriteKit

class ShieldSpawner {

    func spawn(in parent: SKNode, moveDuration: TimeInterval, sceneSize: CGSize) {
        let pickup = ShieldPickupNode()
        let randomY = CGFloat.random(in: 70...(sceneSize.height - 70))
        pickup.position = CGPoint(x: sceneSize.width + 30, y: randomY)

        parent.addChild(pickup)

        let moveAction = SKAction.moveTo(x: -30, duration: moveDuration)
        pickup.run(SKAction.sequence([moveAction, SKAction.removeFromParent()]))
    }
}
