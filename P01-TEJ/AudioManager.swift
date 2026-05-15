import SpriteKit
import AVFoundation

enum SoundEffect: String {
    case coin   = "Sounds/coin.wav"
    case hit    = "Sounds/hit.wav"
    case button = "Sounds/button.wav"
    case shield = "Sounds/shield.wav"
}

final class AudioManager {
    static let shared = AudioManager()

    private var musicPlayer: AVAudioPlayer?
    private var thrustPlayer: AVAudioPlayer?

    private init() {
        configureSession()
    }

    private func configureSession() {
        try? AVAudioSession.sharedInstance().setCategory(.ambient, mode: .default, options: [.mixWithOthers])
        try? AVAudioSession.sharedInstance().setActive(true)
    }

    var isMusicEnabled: Bool {
        get { UserDefaults.standard.object(forKey: StorageKeys.musicEnabled) as? Bool ?? true }
        set {
            UserDefaults.standard.set(newValue, forKey: StorageKeys.musicEnabled)
            if newValue { musicPlayer?.play() } else { musicPlayer?.pause() }
        }
    }

    var isSfxEnabled: Bool {
        get { UserDefaults.standard.object(forKey: StorageKeys.sfxEnabled) as? Bool ?? true }
        set { UserDefaults.standard.set(newValue, forKey: StorageKeys.sfxEnabled) }
    }

    func playSFX(_ effect: SoundEffect, on scene: SKScene) {
        guard isSfxEnabled else { return }
        scene.run(SKAction.playSoundFileNamed(effect.rawValue, waitForCompletion: false))
    }

    func startMusic() {
        guard isMusicEnabled else { return }
        if musicPlayer?.isPlaying == true { return }

        guard let url = soundURL(named: "music") else { return }
        do {
            let player = try AVAudioPlayer(contentsOf: url)
            player.numberOfLoops = -1
            player.volume = 0.35
            player.prepareToPlay()
            player.play()
            musicPlayer = player
        } catch {
            musicPlayer = nil
        }
    }

    func stopMusic() {
        musicPlayer?.stop()
        musicPlayer = nil
    }

    func startThrustLoop() {
        guard isSfxEnabled else { return }
        if thrustPlayer?.isPlaying == true { return }

        guard let url = soundURL(named: "thrust") else { return }
        do {
            let player = try AVAudioPlayer(contentsOf: url)
            player.numberOfLoops = -1
            player.volume = 0.28
            player.prepareToPlay()
            player.play()
            thrustPlayer = player
        } catch {
            thrustPlayer = nil
        }
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
        if isMusicEnabled { musicPlayer?.play() }
        thrustPlayer?.play()
    }

    private func soundURL(named name: String) -> URL? {
        if let url = Bundle.main.url(forResource: name, withExtension: "wav", subdirectory: "Sounds") {
            return url
        }
        return Bundle.main.url(forResource: name, withExtension: "wav")
    }
}
