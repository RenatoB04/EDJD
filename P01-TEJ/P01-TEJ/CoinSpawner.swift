import SpriteKit

// Cria moedas fora do lado direito do ecrã e move-as para a esquerda.
// A GameScene chama este spawner de tempos a tempos.
class CoinSpawner {

    func spawn(in parent: SKNode, moveDuration: TimeInterval, sceneSize: CGSize) {
        let coin = CoinNode()
        // A posição Y é aleatória para obrigar o jogador a ajustar a altura.
        coin.position = CGPoint(
            x: sceneSize.width + 30,
            y: CGFloat.random(in: 60...(sceneSize.height - 60))
        )

        parent.addChild(coin)

        // A moeda atravessa o ecrã e é removida quando sai pela esquerda.
        let moveAction = SKAction.moveTo(x: -30, duration: moveDuration)
        coin.run(SKAction.sequence([moveAction, SKAction.removeFromParent()]))
    }
}
