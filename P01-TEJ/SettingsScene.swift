import SpriteKit

class SettingsScene: SKScene {

    private var musicToggle: SKShapeNode!
    private var sfxToggle: SKShapeNode!
    private var hapticsToggle: SKShapeNode!

    override func didMove(to view: SKView) {
        self.backgroundColor = SKColor(red: 0.05, green: 0.05, blue: 0.12, alpha: 1.0)

        let title = SKLabelNode(fontNamed: "AvenirNext-Bold")
        title.text = "Definições"
        title.fontSize = 36
        title.fontColor = .white
        title.position = CGPoint(x: size.width / 2, y: size.height * 0.85)
        addChild(title)

        musicToggle   = makeToggleRow(label: "Música",    y: size.height * 0.71,  name: NodeNames.musicToggle)
        sfxToggle     = makeToggleRow(label: "Efeitos",   y: size.height * 0.6,  name: NodeNames.sfxToggle)
        hapticsToggle = makeToggleRow(label: "Vibração",  y: size.height * 0.49,  name: NodeNames.hapticsToggle)

        addChild(makeButton(text: "Apagar Progresso",
                            fontSize: 16,
                            color: .systemRed,
                            position: CGPoint(x: size.width / 2, y: size.height * 0.32),
                            size: CGSize(width: 220, height: 46),
                            name: NodeNames.resetButton))

        addChild(makeButton(text: "Voltar",
                            fontSize: 18,
                            color: .systemGray,
                            position: CGPoint(x: size.width / 2, y: size.height * 0.18),
                            size: CGSize(width: 160, height: 46),
                            name: NodeNames.backButton))

        refresh()
    }

    private func makeToggleRow(label: String, y: CGFloat, name: String) -> SKShapeNode {
        let labelNode = SKLabelNode(fontNamed: "AvenirNext-Bold")
        labelNode.text = label
        labelNode.fontSize = 22
        labelNode.fontColor = .white
        labelNode.horizontalAlignmentMode = .left
        labelNode.verticalAlignmentMode = .center
        labelNode.position = CGPoint(x: size.width * 0.2, y: y)
        addChild(labelNode)

        let toggleSize = CGSize(width: 90, height: 40)
        let cornerRadius = toggleSize.height / 2
        let rect = CGRect(x: -toggleSize.width / 2, y: -toggleSize.height / 2, width: toggleSize.width, height: toggleSize.height)
        
        let toggle = SKShapeNode(rect: rect, cornerRadius: cornerRadius)
        toggle.position = CGPoint(x: size.width * 0.8, y: y)
        toggle.name = name
        toggle.fillColor = .systemGray
        toggle.strokeColor = SKColor.white.withAlphaComponent(0.25)
        toggle.lineWidth = 1.5

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

    private func makeButton(text: String, fontSize: CGFloat, color: SKColor, position: CGPoint, size: CGSize, name: String) -> SKShapeNode {
        let cornerRadius = size.height * 0.3
        let rect = CGRect(x: -size.width / 2, y: -size.height / 2, width: size.width, height: size.height)
        
        let button = SKShapeNode(rect: rect, cornerRadius: cornerRadius)
        button.position = position
        button.name = name
        button.fillColor = color
        button.strokeColor = SKColor.white.withAlphaComponent(0.25)
        button.lineWidth = 1.5

        let label = SKLabelNode(fontNamed: "AvenirNext-Bold")
        label.text = text
        label.fontSize = fontSize
        label.fontColor = .white
        label.verticalAlignmentMode = .center
        label.horizontalAlignmentMode = .center
        label.name = name
        button.addChild(label)
        return button
    }

    private func refresh() {
        applyToggleState(musicToggle,   isOn: AudioManager.shared.isMusicEnabled)
        applyToggleState(sfxToggle,     isOn: AudioManager.shared.isSfxEnabled)
        applyToggleState(hapticsToggle, isOn: HapticsManager.shared.isEnabled)
    }

    private func applyToggleState(_ node: SKShapeNode, isOn: Bool) {
        node.fillColor = isOn ? .systemGreen : .systemGray
        node.strokeColor = isOn
            ? SKColor.white.withAlphaComponent(0.4)
            : SKColor.white.withAlphaComponent(0.2)
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
