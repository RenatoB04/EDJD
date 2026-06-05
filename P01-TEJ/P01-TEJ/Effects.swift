import SpriteKit

// Pequenos efeitos visuais usados quando o jogador morre.
// Estão separados para a GameScene ficar mais fácil de ler.
enum Effects {

    // Abana ligeiramente o mundo do jogo para dar impacto à colisão.
    static func screenShake(on node: SKNode) {
        let shake = SKAction.sequence([
            SKAction.moveBy(x: 8, y: 0, duration: 0.04),
            SKAction.moveBy(x: -16, y: 0, duration: 0.08),
            SKAction.moveBy(x: 8, y: 0, duration: 0.04)
        ])
        node.run(shake)
    }

    // Cria um flash branco rápido por cima da cena.
    static func flash(in scene: SKScene) {
        let flash = SKSpriteNode(color: .white, size: scene.size)
        flash.position = CGPoint(x: scene.size.width / 2, y: scene.size.height / 2)
        flash.alpha = 0.5
        flash.zPosition = 900
        scene.addChild(flash)

        flash.run(SKAction.sequence([
            SKAction.fadeOut(withDuration: 0.25),
            SKAction.removeFromParent()
        ]))
    }

    // Faz o jogador aumentar e desaparecer no momento da morte.
    static func playerDeathAnimation(on player: SKSpriteNode) {
        player.run(SKAction.group([
            SKAction.scale(to: 1.5, duration: 0.25),
            SKAction.fadeOut(withDuration: 0.25)
        ]))
    }
}
