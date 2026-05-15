import SpriteKit

enum Effects {

    static func makeJetpackTrail() -> SKEmitterNode {
        let emitter = SKEmitterNode()
        emitter.particleTexture = SKTexture(image: dotImage(diameter: 8))
        emitter.particleBirthRate = 120
        emitter.particleLifetime = 0.45
        emitter.particleLifetimeRange = 0.15
        emitter.particlePositionRange = CGVector(dx: 4, dy: 6)
        emitter.particleSpeed = 80
        emitter.particleSpeedRange = 30
        emitter.emissionAngle = .pi
        emitter.emissionAngleRange = 0.25
        emitter.particleAlpha = 0.85
        emitter.particleAlphaRange = 0.2
        emitter.particleAlphaSpeed = -1.8
        emitter.particleScale = 0.7
        emitter.particleScaleRange = 0.3
        emitter.particleScaleSpeed = -0.9
        emitter.particleColor = SKColor.orange
        emitter.particleColorBlendFactor = 1.0
        emitter.particleBlendMode = .add
        emitter.zPosition = -1
        emitter.targetNode = nil
        return emitter
    }

    static func tintTrail(_ emitter: SKEmitterNode, color: SKColor) {
        emitter.particleColor = color
    }

    static func screenShake(on node: SKNode, magnitude: CGFloat = 10, duration: TimeInterval = 0.35) {
        let steps = 8
        let stepDuration = duration / TimeInterval(steps)
        var actions: [SKAction] = []
        for _ in 0..<steps {
            let dx = CGFloat.random(in: -magnitude...magnitude)
            let dy = CGFloat.random(in: -magnitude...magnitude)
            actions.append(SKAction.moveBy(x: dx, y: dy, duration: stepDuration / 2))
            actions.append(SKAction.moveBy(x: -dx, y: -dy, duration: stepDuration / 2))
        }
        node.run(SKAction.sequence(actions))
    }

    static func flash(in scene: SKScene, color: SKColor = .white, peakAlpha: CGFloat = 0.55) {
        let flash = SKSpriteNode(color: color, size: scene.size)
        flash.position = CGPoint(x: scene.size.width / 2, y: scene.size.height / 2)
        flash.alpha = 0
        flash.zPosition = 900
        flash.blendMode = .add
        scene.addChild(flash)

        let appear  = SKAction.fadeAlpha(to: peakAlpha, duration: 0.06)
        let vanish  = SKAction.fadeOut(withDuration: 0.32)
        flash.run(SKAction.sequence([appear, vanish, SKAction.removeFromParent()]))
    }

    static func deathExplosion(at position: CGPoint, in scene: SKScene) {
        let emitter = SKEmitterNode()
        emitter.particleTexture = SKTexture(image: dotImage(diameter: 10))
        emitter.position = position
        emitter.particleBirthRate = 600
        emitter.numParticlesToEmit = 60
        emitter.particleLifetime = 0.6
        emitter.particleLifetimeRange = 0.25
        emitter.particleSpeed = 180
        emitter.particleSpeedRange = 80
        emitter.emissionAngleRange = .pi * 2
        emitter.particleAlpha = 1.0
        emitter.particleAlphaSpeed = -1.5
        emitter.particleScale = 1.0
        emitter.particleScaleRange = 0.4
        emitter.particleScaleSpeed = -1.2
        emitter.particleColor = SKColor.orange
        emitter.particleColorBlendFactor = 1.0
        emitter.particleBlendMode = .add
        emitter.zPosition = 800

        scene.addChild(emitter)
        emitter.run(SKAction.sequence([
            SKAction.wait(forDuration: 1.2),
            SKAction.removeFromParent()
        ]))
    }

    static func playerDeathAnimation(on player: SKSpriteNode) {
        let colorize = SKAction.colorize(with: .red, colorBlendFactor: 0.8, duration: 0.1)
        let scaleUp  = SKAction.scale(to: 1.6, duration: 0.2)
        let fadeOut  = SKAction.fadeOut(withDuration: 0.3)
        player.run(SKAction.sequence([
            colorize,
            SKAction.group([scaleUp, fadeOut])
        ]))
    }

    private static func dotImage(diameter: CGFloat) -> UIImage {
        let size = CGSize(width: diameter, height: diameter)
        let renderer = UIGraphicsImageRenderer(size: size)
        return renderer.image { ctx in
            let rect = CGRect(origin: .zero, size: size)
            ctx.cgContext.setFillColor(UIColor.white.cgColor)
            ctx.cgContext.fillEllipse(in: rect)
        }
    }
}
