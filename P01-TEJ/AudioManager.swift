import SpriteKit
import AVFoundation

enum SoundEffect: String {
    case hit = "Sounds/hit.wav"
    case button = "Sounds/button.wav"
}

final class AudioManager {
    static let shared = AudioManager()

    private var musicPlayer: AVAudioPlayer?
    private var thrustPlayer: AVAudioPlayer?

    private init() {}

    func playSFX(_ effect: SoundEffect, on scene: SKScene) {
        scene.run(SKAction.playSoundFileNamed(effect.rawValue, waitForCompletion: false))
    }

    func startMusic() {
        if musicPlayer?.isPlaying == true { return }
        musicPlayer = makePlayer(named: "music", volume: 0.35, loops: -1)
        musicPlayer?.play()
    }

    func startThrustLoop() {
        if thrustPlayer?.isPlaying == true { return }
        thrustPlayer = makePlayer(named: "thrust", volume: 0.28, loops: -1)
        thrustPlayer?.play()
    }

    func stopThrustLoop() {
        thrustPlayer?.stop()
        thrustPlayer = nil
    }

    func pauseAll() {
        musicPlayer?.pause()
        thrustPlayer?.pause()
    }

    func resumeAll() {
        musicPlayer?.play()
        thrustPlayer?.play()
    }

    private func makePlayer(named name: String, volume: Float, loops: Int) -> AVAudioPlayer? {
        guard let url = Bundle.main.url(forResource: name, withExtension: "wav", subdirectory: "Sounds") else {
            return nil
        }

        do {
            let player = try AVAudioPlayer(contentsOf: url)
            player.volume = volume
            player.numberOfLoops = loops
            player.prepareToPlay()
            return player
        } catch {
            return nil
        }
    }
}
