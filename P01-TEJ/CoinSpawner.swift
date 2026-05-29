import SpriteKit

class CoinSpawner {

    func spawn(in parent: SKNode, moveDuration: TimeInterval, sceneSize: CGSize) {
        let coin = CoinNode()
        coin.position = CGPoint(
            x: sceneSize.width + 30,
            y: CGFloat.random(in: 60...(sceneSize.height - 60))
        )

        parent.addChild(coin)

        let moveAction = SKAction.moveTo(x: -30, duration: moveDuration)
        coin.run(SKAction.sequence([moveAction, SKAction.removeFromParent()]))
    }
}
