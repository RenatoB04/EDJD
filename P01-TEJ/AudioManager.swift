import AVFoundation

final class AudioManager {
    static let shared = AudioManager()

    private var musicPlayer: AVAudioPlayer?
    private var thrustPlayer: AVAudioPlayer?

    private init() {}

    func startMusic() {
        if musicPlayer == nil {
            musicPlayer = makePlayer(fileName: "music", volume: 0.25)
            musicPlayer?.numberOfLoops = -1
        }
        musicPlayer?.play()
    }

    func startThrust() {
        if thrustPlayer == nil {
            thrustPlayer = makePlayer(fileName: "thrust", volume: 0.35)
            thrustPlayer?.numberOfLoops = -1
        }
        thrustPlayer?.play()
    }

    func stopThrust() {
        thrustPlayer?.stop()
        thrustPlayer?.currentTime = 0
    }

    func pauseAll() {
        musicPlayer?.pause()
        thrustPlayer?.pause()
    }

    func resumeMusic() {
        musicPlayer?.play()
    }

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
