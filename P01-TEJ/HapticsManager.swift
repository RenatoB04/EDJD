import UIKit

final class HapticsManager {
    static let shared = HapticsManager()

    private let lightImpact  = UIImpactFeedbackGenerator(style: .light)
    private let mediumImpact = UIImpactFeedbackGenerator(style: .medium)
    private let heavyImpact  = UIImpactFeedbackGenerator(style: .heavy)
    private let notification = UINotificationFeedbackGenerator()

    private init() {
        lightImpact.prepare()
        mediumImpact.prepare()
        heavyImpact.prepare()
        notification.prepare()
    }

    var isEnabled: Bool {
        get { UserDefaults.standard.object(forKey: StorageKeys.hapticsEnabled) as? Bool ?? true }
        set { UserDefaults.standard.set(newValue, forKey: StorageKeys.hapticsEnabled) }
    }

    func coinCollected() {
        guard isEnabled else { return }
        lightImpact.impactOccurred()
    }

    func shieldHit() {
        guard isEnabled else { return }
        mediumImpact.impactOccurred()
    }

    func playerDied() {
        guard isEnabled else { return }
        heavyImpact.impactOccurred()
        notification.notificationOccurred(.error)
    }

    func buttonTap() {
        guard isEnabled else { return }
        lightImpact.impactOccurred(intensity: 0.6)
    }
}
