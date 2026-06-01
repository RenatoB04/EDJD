import UIKit
import SpriteKit

// Controlador principal criado pelo storyboard.
// A sua função é apresentar a primeira cena SpriteKit.
class GameViewController: UIViewController {
    override func viewDidLoad() {
        super.viewDidLoad()
        
        if let view = self.view as? SKView {
            // Começamos pelo menu principal.
            let scene = MenuScene(size: view.bounds.size)
            scene.scaleMode = .aspectFill
            
            view.presentScene(scene)
            
            // Optimização normal em SpriteKit.
            view.ignoresSiblingOrder = true
            #if DEBUG
            // Informação útil enquanto estamos a testar no simulador.
            view.showsFPS = true
            view.showsNodeCount = true
            #endif
        }
    }

    // Esconde a barra de estado para o jogo ocupar o ecrã todo.
    override var prefersStatusBarHidden: Bool {
        return true
    }
}
