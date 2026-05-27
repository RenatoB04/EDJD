import SpriteKit

struct Skin {
    let id: String
    let displayName: String
    let assetName: String
    let trailColor: SKColor
    let price: Int
}

enum SkinCatalog {
    static let defaultSkinId = "default"

    static let all: [Skin] = [
        Skin(
            id: "default",
            displayName: "Foguetao",
            assetName: AssetNames.player,
            trailColor: .orange,
            price: 0
        ),
        Skin(
            id: "red",
            displayName: "Falcao Vermelho",
            assetName: "player_red",
            trailColor: SKColor(red: 1.0, green: 0.4, blue: 0.2, alpha: 1.0),
            price: 50
        ),
        Skin(
            id: "gold",
            displayName: "Dourado",
            assetName: "player_gold",
            trailColor: SKColor(red: 1.0, green: 0.85, blue: 0.2, alpha: 1.0),
            price: 200
        ),
        Skin(
            id: "neon",
            displayName: "Neon",
            assetName: "player_neon",
            trailColor: SKColor(red: 1.0, green: 0.3, blue: 0.9, alpha: 1.0),
            price: 500
        )
    ]

    static func skin(forId id: String) -> Skin {
        return all.first(where: { $0.id == id }) ?? all[0]
    }
}

enum PlayerInventory {

    static var equippedSkinId: String {
        get {
            UserDefaults.standard.string(forKey: StorageKeys.equippedSkin) ?? SkinCatalog.defaultSkinId
        }
        set {
            UserDefaults.standard.set(newValue, forKey: StorageKeys.equippedSkin)
        }
    }

    static var equippedSkin: Skin {
        return SkinCatalog.skin(forId: equippedSkinId)
    }

    static var ownedSkinIds: Set<String> {
        get {
            let list = UserDefaults.standard.array(forKey: StorageKeys.ownedSkins) as? [String] ?? []
            var set = Set(list)
            set.insert(SkinCatalog.defaultSkinId)
            return set
        }
        set {
            UserDefaults.standard.set(Array(newValue), forKey: StorageKeys.ownedSkins)
        }
    }

    static func isOwned(_ skinId: String) -> Bool {
        return ownedSkinIds.contains(skinId)
    }

    static func purchase(_ skin: Skin) -> Bool {
        let wallet = walletBalance()
        guard wallet >= skin.price else { return false }
        guard !isOwned(skin.id) else { return false }

        setWalletBalance(wallet - skin.price)
        var owned = ownedSkinIds
        owned.insert(skin.id)
        ownedSkinIds = owned
        return true
    }

    static func equip(_ skinId: String) {
        guard isOwned(skinId) else { return }
        equippedSkinId = skinId
    }

    static func walletBalance() -> Int {
        return UserDefaults.standard.integer(forKey: StorageKeys.coinWallet)
    }

    static func setWalletBalance(_ value: Int) {
        UserDefaults.standard.set(value, forKey: StorageKeys.coinWallet)
    }
}
