import AVFoundation

// Classe simples para controlar os sons que precisam de tocar em loop.
// Os efeitos curtos, como botões e moedas, são reproduzidos directamente nas cenas com SKAction.
final class AudioManager {
    static let shared = AudioManager()

    // Guardamos uma referência aos players para o som não parar imediatamente.
    private var musicPlayer: AVAudioPlayer?
    private var thrustPlayer: AVAudioPlayer?

    private init() {}

    // Começa a música de fundo e deixa-a em loop.
    func startMusic() {
        if musicPlayer == nil {
            musicPlayer = makePlayer(fileName: "music", volume: 0.1)
            musicPlayer?.numberOfLoops = -1
        }
        musicPlayer?.play()
    }

    // Começa o som do impulso enquanto o jogador mantém o toque no ecrã.
    func startThrust() {
        if thrustPlayer == nil {
            thrustPlayer = makePlayer(fileName: "thrust", volume: 0.35)
            thrustPlayer?.numberOfLoops = -1
        }
        thrustPlayer?.play()
    }

    // Pára e rebobina o som do impulso para ficar pronto para o próximo toque.
    func stopThrust() {
        thrustPlayer?.stop()
        thrustPlayer?.currentTime = 0
    }

    // Usado quando o jogo entra em pausa.
    func pauseAll() {
        musicPlayer?.pause()
        thrustPlayer?.pause()
    }

    // Ao sair da pausa retomamos apenas a música; o impulso só volta com novo toque.
    func resumeMusic() {
        musicPlayer?.play()
    }

    // Cria um AVAudioPlayer a partir dos ficheiros .wav dentro da pasta Sounds.
    private func makePlayer(fileName: String, volume: Float) -> AVAudioPlayer? {
        guard let url = Bundle.main.url(forResource: fileName, withExtension: "wav", subdirectory: "Sounds") else {
            return nil
        }

        let player = try? AVAudioPlayer(contentsOf: url)
        player?.volume = volume
        player?.prepareToPlay()
        return player
    }
}
