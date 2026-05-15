import Foundation
import SpriteKit

class CoinSpawner {

    func spawn(in parent: SKNode, moveDuration: TimeInterval, sceneSize: CGSize) {
        let coin = CoinNode()

        let randomY = CGFloat.random(in: 60...(sceneSize.height - 60))
        coin.position = CGPoint(x: sceneSize.width + 30, y: randomY)

        parent.addChild(coin)

        let moveAction = SKAction.moveTo(x: -30, duration: moveDuration)
        coin.run(SKAction.sequence([moveAction, SKAction.removeFromParent()]))
    }
}
