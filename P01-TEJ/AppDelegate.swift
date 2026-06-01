import UIKit

// Ponto de entrada da aplicação iOS.
// Neste projecto quase toda a lógica está em SpriteKit, por isso o AppDelegate fica simples.
@main
class AppDelegate: UIResponder, UIApplicationDelegate {

    var window: UIWindow?


    func application(_ application: UIApplication, didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey: Any]?) -> Bool {
        // Devolvemos true para indicar que a aplicação arrancou normalmente.
        return true
    }

    func applicationWillResignActive(_ application: UIApplication) {
        // Método disponível caso seja preciso reagir quando a app deixa de estar activa.
    }

    func applicationDidEnterBackground(_ application: UIApplication) {
        // Método disponível caso seja preciso guardar estado ao ir para segundo plano.
    }

    func applicationWillEnterForeground(_ application: UIApplication) {
        // Método disponível caso seja preciso preparar o regresso da app.
    }

    func applicationDidBecomeActive(_ application: UIApplication) {
        // Método disponível caso seja preciso retomar alguma tarefa.
    }
}	

