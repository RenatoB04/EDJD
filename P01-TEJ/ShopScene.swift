import SpriteKit

class ShopScene: SKScene {

    private var walletLabel: SKLabelNode!
    private var statusLabel: SKLabelNode!
    private var skinNodes: [String: SKSpriteNode] = [:]

    override func didMove(to view: SKView) {
        self.backgroundColor = SKColor(red: 0.05, green: 0.05, blue: 0.12, alpha: 1.0)

        let title = SKLabelNode(fontNamed: "AvenirNext-Bold")
        title.text = "Loja"
        title.fontSize = 38
        title.fontColor = .white
        title.position = CGPoint(x: size.width / 2, y: size.height * 0.88)
        addChild(title)

        walletLabel = SKLabelNode(fontNamed: "AvenirNext-Bold")
        walletLabel.fontSize = 22
        walletLabel.fontColor = SKColor(red: 1.0, green: 0.82, blue: 0.0, alpha: 1.0)
        walletLabel.position = CGPoint(x: size.width / 2, y: size.height * 0.82)
        addChild(walletLabel)

        statusLabel = SKLabelNode(fontNamed: "AvenirNext")
        statusLabel.fontSize = 16
        statusLabel.fontColor = .lightGray
        statusLabel.position = CGPoint(x: size.width / 2, y: size.height * 0.18)
        addChild(statusLabel)

        layoutSkins()
        addBackButton()
        refresh()
    }

    private func layoutSkins() {
        let skins = SkinCatalog.all
        let columns = 2
        let spacingX = size.width * 0.42
        let spacingY: CGFloat = 130
        let originX = size.width / 2 - spacingX / 2
        let originY = size.height * 0.65

        for (index, skin) in skins.enumerated() {
            let col = index % columns
            let row = index / columns
            let position = CGPoint(
                x: originX + CGFloat(col) * spacingX,
                y: originY - CGFloat(row) * spacingY
            )
            let card = makeSkinCard(skin: skin)
            card.position = position
            addChild(card)
            skinNodes[skin.id] = card
        }
    }

    private func makeSkinCard(skin: Skin) -> SKSpriteNode {
        let card = SKSpriteNode(color: SKColor(white: 0.15, alpha: 1.0),
                                size: CGSize(width: 150, height: 110))
        card.name = "skinCard_\(skin.id)"

        let sprite = SKSpriteNode(texture: SKTexture(imageNamed: skin.assetName))
        sprite.size = CGSize(width: 60, height: 60)
        sprite.position = CGPoint(x: 0, y: 18)
        sprite.name = card.name
        card.addChild(sprite)

        let nameLabel = SKLabelNode(fontNamed: "AvenirNext-Bold")
        nameLabel.text = skin.displayName
        nameLabel.fontSize = 13
        nameLabel.fontColor = .white
        nameLabel.position = CGPoint(x: 0, y: -22)
        nameLabel.name = card.name
        card.addChild(nameLabel)

        let priceLabel = SKLabelNode(fontNamed: "AvenirNext-Bold")
        priceLabel.fontSize = 13
        priceLabel.position = CGPoint(x: 0, y: -42)
        priceLabel.name = card.name
        card.addChild(priceLabel)

        return card
    }

    private func addBackButton() {
        let back = SKSpriteNode(color: .systemGray, size: CGSize(width: 160, height: 46))
        back.position = CGPoint(x: size.width / 2, y: size.height * 0.08)
        back.name = NodeNames.backButton

        let label = SKLabelNode(fontNamed: "AvenirNext-Bold")
        label.text = "Voltar"
        label.fontSize = 18
        label.fontColor = .white
        label.verticalAlignmentMode = .center
        label.horizontalAlignmentMode = .center
        label.name = NodeNames.backButton
        back.addChild(label)

        addChild(back)
    }

    private func refresh() {
        walletLabel.text = "🪙 \(PlayerInventory.walletBalance())"

        let equipped = PlayerInventory.equippedSkinId

        for skin in SkinCatalog.all {
            guard let card = skinNodes[skin.id] else { continue }

            let priceLabel = card.children.compactMap { $0 as? SKLabelNode }
                .first(where: { $0.position.y < -30 })

            if !PlayerInventory.isOwned(skin.id) {
                priceLabel?.text = "🪙 \(skin.price)"
                priceLabel?.fontColor = SKColor(red: 1.0, green: 0.82, blue: 0.0, alpha: 1.0)
                card.color = SKColor(white: 0.15, alpha: 1.0)
            } else if skin.id == equipped {
                priceLabel?.text = "EQUIPADA"
                priceLabel?.fontColor = .systemGreen
                card.color = SKColor(red: 0.1, green: 0.3, blue: 0.15, alpha: 1.0)
            } else {
                priceLabel?.text = "TOCA P/ EQUIPAR"
                priceLabel?.fontColor = .white
                card.color = SKColor(white: 0.22, alpha: 1.0)
            }
        }
    }

    override func touchesBegan(_ touches: Set<UITouch>, with event: UIEvent?) {
        guard let touch = touches.first else { return }
        let location = touch.location(in: self)
        let names = nodes(at: location).compactMap { $0.name }

        if names.contains(NodeNames.backButton) {
            AudioManager.shared.playSFX(.button, on: self)
            HapticsManager.shared.buttonTap()
            let menu = MenuScene(size: self.size)
            menu.scaleMode = .aspectFill
            view?.presentScene(menu, transition: .fade(withDuration: 0.4))
            return
        }

        for name in names where name.hasPrefix("skinCard_") {
            let id = String(name.dropFirst("skinCard_".count))
            handleSkinTap(id: id)
            return
        }
    }

    private func handleSkinTap(id: String) {
        let skin = SkinCatalog.skin(forId: id)

        if PlayerInventory.isOwned(skin.id) {
            PlayerInventory.equip(skin.id)
            statusLabel.text = "Equipado: \(skin.displayName)"
            AudioManager.shared.playSFX(.button, on: self)
            HapticsManager.shared.buttonTap()
        } else if PlayerInventory.purchase(skin) {
            PlayerInventory.equip(skin.id)
            statusLabel.text = "Comprado e equipado!"
            AudioManager.shared.playSFX(.shield, on: self)
            HapticsManager.shared.shieldHit()
        } else {
            statusLabel.text = "Moedas insuficientes (\(skin.price))"
            HapticsManager.shared.playerDied()
        }

        refresh()
    }
}
