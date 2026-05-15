import SpriteKit

class ObstacleSpawner {

    private let texture: SKTexture = {
        let t = SKTexture(imageNamed: AssetNames.asteroid)
        t.filteringMode = .nearest
        return t
    }()

    func spawn(in parent: SKNode, moveDuration: TimeInterval, sceneSize: CGSize) {
        let obstacle = SKSpriteNode(texture: texture, color: .clear, size: ObstacleConfig.size)
        obstacle.name = NodeNames.obstacle

        let randomY = CGFloat.random(in: 50...sceneSize.height - 50)
        obstacle.position = CGPoint(x: sceneSize.width + ObstacleConfig.size.width, y: randomY)

        obstacle.physicsBody = SKPhysicsBody(circleOfRadius: ObstacleConfig.physicsRadius)
        obstacle.physicsBody?.isDynamic = false
        obstacle.physicsBody?.categoryBitMask = PhysicsCategory.obstacle
        obstacle.physicsBody?.contactTestBitMask = PhysicsCategory.player
        obstacle.physicsBody?.collisionBitMask = PhysicsCategory.none

        let spin = SKAction.rotate(byAngle: .pi * 2, duration: TimeInterval.random(in: 2.5...5.0))
        obstacle.run(SKAction.repeatForever(spin))

        parent.addChild(obstacle)

        let moveAction = SKAction.moveTo(x: -ObstacleConfig.size.width, duration: moveDuration)
        obstacle.run(SKAction.sequence([moveAction, SKAction.removeFromParent()]))
    }
}
