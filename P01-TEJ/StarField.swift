import SpriteKit

class StarField: SKNode {

    private let sceneSize: CGSize

    init(sceneSize: CGSize) {
        self.sceneSize = sceneSize
        super.init()

        zPosition = -100

        for _ in 0..<StarFieldConfig.starCount {
            let size = CGFloat.random(in: 1...3)
            let star = SKSpriteNode(color: .white, size: CGSize(width: size, height: size))
            star.alpha = CGFloat.random(in: 0.4...1.0)
            star.position = CGPoint(
                x: CGFloat.random(in: 0...sceneSize.width),
                y: CGFloat.random(in: 0...sceneSize.height)
            )
            addChild(star)
        }
    }

    required init?(coder aDecoder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }

    func update(deltaTime: TimeInterval, speedMultiplier: CGFloat) {
        let movement = StarFieldConfig.speed * speedMultiplier * CGFloat(deltaTime)

        for star in children {
            star.position.x -= movement

            if star.position.x < 0 {
                star.position.x = sceneSize.width
                star.position.y = CGFloat.random(in: 0...sceneSize.height)
            }
        }
    }
}
