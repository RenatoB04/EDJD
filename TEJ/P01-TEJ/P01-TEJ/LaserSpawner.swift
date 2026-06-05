import SpriteKit

// Segundo tipo de obstáculo: um laser vertical que pode aparecer em cima ou em baixo.
class LaserSpawner {

    func spawn(in parent: SKNode, moveDuration: TimeInterval, sceneSize: CGSize) {
        // A altura varia para tornar cada laser ligeiramente diferente.
        let laserHeight = CGFloat.random(in: sceneSize.height * 0.35...sceneSize.height * 0.55)
        let laserWidth: CGFloat = 14

        // O laser fica colado ao topo ou ao fundo, deixando uma zona livre no lado oposto.
        let placeTop = Bool.random()
        let yPos = placeTop
            ? sceneSize.height - laserHeight / 2
            : laserHeight / 2

        let laser = SKSpriteNode(color: SKColor(red: 1.0, green: 0.25, blue: 0.45, alpha: 1.0),
                                 size: CGSize(width: laserWidth, height: laserHeight))
        laser.name = NodeNames.obstacle
        laser.position = CGPoint(x: sceneSize.width + laserWidth, y: yPos)

        // Tal como os asteroides, só precisamos de detectar contacto com o jogador.
        laser.physicsBody = SKPhysicsBody(rectangleOf: laser.size)
        laser.physicsBody?.isDynamic = false
        laser.physicsBody?.categoryBitMask = PhysicsCategory.obstacle
        laser.physicsBody?.contactTestBitMask = PhysicsCategory.player
        laser.physicsBody?.collisionBitMask = PhysicsCategory.none

        // Animação simples para o laser parecer activo.
        let glow = SKAction.sequence([
            SKAction.fadeAlpha(to: 0.55, duration: 0.25),
            SKAction.fadeAlpha(to: 1.0,  duration: 0.25)
        ])
        laser.run(SKAction.repeatForever(glow))

        parent.addChild(laser)

        // O laser entra pela direita e desaparece quando sai pela esquerda.
        let moveAction = SKAction.moveTo(x: -laserWidth, duration: moveDuration)
        laser.run(SKAction.sequence([moveAction, SKAction.removeFromParent()]))
    }
}
