import SpriteKit

class LaserSpawner {

    func spawn(in parent: SKNode, moveDuration: TimeInterval, sceneSize: CGSize) {
        let laserHeight = CGFloat.random(in: sceneSize.height * 0.35...sceneSize.height * 0.55)
        let laserWidth: CGFloat = 14

        let placeTop = Bool.random()
        let yPos = placeTop
            ? sceneSize.height - laserHeight / 2
            : laserHeight / 2

        let laser = SKSpriteNode(color: SKColor(red: 1.0, green: 0.25, blue: 0.45, alpha: 1.0),
                                 size: CGSize(width: laserWidth, height: laserHeight))
        laser.name = NodeNames.obstacle
        laser.position = CGPoint(x: sceneSize.width + laserWidth, y: yPos)

        laser.physicsBody = SKPhysicsBody(rectangleOf: laser.size)
        laser.physicsBody?.isDynamic = false
        laser.physicsBody?.categoryBitMask = PhysicsCategory.obstacle
        laser.physicsBody?.contactTestBitMask = PhysicsCategory.player
        laser.physicsBody?.collisionBitMask = PhysicsCategory.none

        let glow = SKAction.sequence([
            SKAction.fadeAlpha(to: 0.55, duration: 0.25),
            SKAction.fadeAlpha(to: 1.0,  duration: 0.25)
        ])
        laser.run(SKAction.repeatForever(glow))

        parent.addChild(laser)

        let moveAction = SKAction.moveTo(x: -laserWidth, duration: moveDuration)
        laser.run(SKAction.sequence([moveAction, SKAction.removeFromParent()]))
    }
}
