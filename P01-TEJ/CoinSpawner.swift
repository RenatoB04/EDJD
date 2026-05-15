import Foundation
import SpriteKit

class CoinSpawner {

    func spawn(in scene: SKScene, moveDuration: TimeInterval) {
        let coin = CoinNode()

        let randomY = CGFloat.random(in: 60...(scene.size.height - 60))
        coin.position = CGPoint(x: scene.size.width + 30, y: randomY)

        scene.addChild(coin)

        let moveAction   = SKAction.moveTo(x: -30, duration: moveDuration)
        let removeAction = SKAction.removeFromParent()
        coin.run(SKAction.sequence([moveAction, removeAction]))
    }
}

