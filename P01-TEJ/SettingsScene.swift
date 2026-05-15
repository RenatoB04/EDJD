import SpriteKit

class SettingsScene: SKScene {

    private var musicToggle: SKSpriteNode!
    private var sfxToggle: SKSpriteNode!
    private var hapticsToggle: SKSpriteNode!

    override func didMove(to view: SKView) {
        self.backgroundColor = SKColor(red: 0.05, green: 0.05, blue: 0.12, alpha: 1.0)

        let title = SKLabelNode(fontNamed: "AvenirNext-Bold")
        title.text = "Definições"
        title.fontSize = 36
        title.fontColor = .white
        title.position = CGPoint(x: size.width / 2, y: size.height * 0.85)
        addChild(title)

        musicToggle   = makeToggleRow(label: "Música",    y: size.height * 0.7,  name: NodeNames.musicToggle)
        sfxToggle     = makeToggleRow(label: "Efeitos",   y: size.height * 0.6,  name: NodeNames.sfxToggle)
        hapticsToggle = makeToggleRow(label: "Vibração",  y: size.height * 0.5,  name: NodeNames.hapticsToggle)

        let reset = SKSpriteNode(color: .systemRed, size: CGSize(width: 220, height: 46))
        reset.position = CGPoint(x: size.width / 2, y: size.height * 0.32)
        reset.name = NodeNames.resetButton

        let resetLabel = SKLabelNode(fontNamed: "AvenirNext-Bold")
        resetLabel.text = "Apagar Progresso"
        resetLabel.fontSize = 16
        resetLabel.fontColor = .white
        resetLabel.verticalAlignmentMode = .center
        resetLabel.horizontalAlignmentMode = .center
        resetLabel.name = NodeNames.resetButton
        reset.addChild(resetLabel)
        addChild(reset)

        let back = SKSpriteNode(color: .systemGray, size: CGSize(width: 160, height: 46))
        back.position = CGPoint(x: size.width / 2, y: size.height * 0.18)
        back.name = NodeNames.backButton

        let backLabel = SKLabelNode(fontNamed: "AvenirNext-Bold")
        backLabel.text = "Voltar"
        backLabel.fontSize = 18
        backLabel.fontColor = .white
        backLabel.verticalAlignmentMode = .center
        backLabel.horizontalAlignmentMode = .center
        backLabel.name = NodeNames.backButton
        back.addChild(backLabel)
        addChild(back)

        refresh()
    }

    private func makeToggleRow(label: String, y: CGFloat, name: String) -> SKSpriteNode {
        let labelNode = SKLabelNode(fontNamed: "AvenirNext-Bold")
        labelNode.text = label
        labelNode.fontSize = 22
        labelNode.fontColor = .white
        labelNode.horizontalAlignmentMode = .left
        labelNode.verticalAlignmentMode = .center
        labelNode.position = CGPoint(x: size.width * 0.2, y: y)
        addChild(labelNode)

        let toggle = SKSpriteNode(color: .systemGray, size: CGSize(width: 90, height: 40))
        toggle.position = CGPoint(x: size.width * 0.8, y: y)
        toggle.name = name

        let state = SKLabelNode(fontNamed: "AvenirNext-Bold")
        state.fontSize = 14
        state.fontColor = .white
        state.verticalAlignmentMode = .center
        state.horizontalAlignmentMode = .center
        state.name = name
        toggle.addChild(state)

        addChild(toggle)
        return toggle
    }

    private func refresh() {
        applyToggleState(musicToggle,   isOn: AudioManager.shared.isMusicEnabled)
        applyToggleState(sfxToggle,     isOn: AudioManager.shared.isSfxEnabled)
        applyToggleState(hapticsToggle, isOn: HapticsManager.shared.isEnabled)
    }

    private func applyToggleState(_ node: SKSpriteNode, isOn: Bool) {
        node.color = isOn ? .systemGreen : .systemGray
        if let label = node.children.first as? SKLabelNode {
            label.text = isOn ? "ON" : "OFF"
        }
    }

    override func touchesBegan(_ touches: Set<UITouch>, with event: UIEvent?) {
        guard let touch = touches.first else { return }
        let location = touch.location(in: self)
        let names = nodes(at: location).compactMap { $0.name }

        if names.contains(NodeNames.musicToggle) {
            AudioManager.shared.isMusicEnabled.toggle()
            HapticsManager.shared.buttonTap()
        } else if names.contains(NodeNames.sfxToggle) {
            AudioManager.shared.isSfxEnabled.toggle()
            AudioManager.shared.playSFX(.button, on: self)
            HapticsManager.shared.buttonTap()
        } else if names.contains(NodeNames.hapticsToggle) {
            HapticsManager.shared.isEnabled.toggle()
            HapticsManager.shared.buttonTap()
        } else if names.contains(NodeNames.resetButton) {
            resetProgress()
        } else if names.contains(NodeNames.backButton) {
            AudioManager.shared.playSFX(.button, on: self)
            HapticsManager.shared.buttonTap()
            let menu = MenuScene(size: self.size)
            menu.scaleMode = .aspectFill
            view?.presentScene(menu, transition: .fade(withDuration: 0.4))
            return
        }
        refresh()
    }

    private func resetProgress() {
        let defaults = UserDefaults.standard
        defaults.removeObject(forKey: StorageKeys.highScore)
        defaults.removeObject(forKey: StorageKeys.coinWallet)
        defaults.removeObject(forKey: StorageKeys.ownedSkins)
        defaults.removeObject(forKey: StorageKeys.equippedSkin)
        defaults.removeObject(forKey: StorageKeys.tutorialSeen)
        HapticsManager.shared.playerDied()
    }
}
