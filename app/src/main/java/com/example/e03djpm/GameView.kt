package com.example.e03djpm

import android.content.Context
import android.graphics.Canvas
import android.graphics.Color
import android.graphics.Paint
import android.graphics.Rect
import android.os.Handler
import android.os.Looper
import android.util.AttributeSet
import android.view.MotionEvent
import android.view.SurfaceHolder
import android.view.SurfaceView

class GameView : SurfaceView, Runnable {

    var playing = false
    var gameThread: Thread? = null
    lateinit var surfaceHolder: SurfaceHolder
    lateinit var canvas: Canvas

    lateinit var paint: Paint
    var stars = arrayListOf<Star>()
    var enemies = arrayListOf<Enemy>()
    lateinit var player: Player
    lateinit var boom: Boom

    var lives = 3
    var onGameOver: () -> Unit = {}

    // Projétil simplificado
    var projectileX = -1
    var projectileY = -1
    var projectileActive = false
    val projectileSpeed = 20

    private fun init(context: Context, width: Int, height: Int) {
        surfaceHolder = holder
        paint = Paint()

        for (i in 0..100) {
            stars.add(Star(width, height))
        }

        for (i in 0..2) {
            enemies.add(Enemy(context, width, height))
        }

        player = Player(context, width, height)
        boom = Boom(context, width, height)
    }

    constructor(context: Context?, width: Int, height: Int) : super(context) {
        init(context!!, width, height)
    }

    constructor(context: Context?, attrs: AttributeSet?) : super(context, attrs) {
        init(context!!, 0, 0)
    }

    constructor(context: Context?, attrs: AttributeSet?, defStyleAttr: Int) : super(
        context,
        attrs,
        defStyleAttr
    ) {
        init(context!!, 0, 0)
    }

    fun resume() {
        playing = true
        gameThread = Thread(this)
        gameThread?.start()
    }

    override fun run() {
        while (playing) {
            update()
            draw()
            control()
        }
    }

    fun update() {
        boom.x = -300
        boom.y = -300

        for (s in stars) {
            s.update(player.speed)
        }

        for (e in enemies) {
            e.update(player.speed)
            if (Rect.intersects(player.detectCollision, e.detectCollision)) {
                boom.x = e.x
                boom.y = e.y
                e.x = -300
                lives -= 1
            }
        }

        player.update()

        // Atualizar o projétil
        if (projectileActive) {
            projectileX += projectileSpeed
            if (projectileX > width) {
                projectileActive = false
            }

            // Verificar colisões com inimigos
            for (enemy in enemies) {
                if (Rect.intersects(
                        Rect(projectileX, projectileY, projectileX + 20, projectileY + 20),
                        enemy.detectCollision
                    )
                ) {
                    enemies.remove(enemy)
                    projectileActive = false
                    break
                }
            }
        }
    }

    fun draw() {
        if (surfaceHolder.surface.isValid) {
            canvas = surfaceHolder.lockCanvas()
            canvas.drawColor(Color.BLACK)

            paint.color = Color.YELLOW
            for (star in stars) {
                paint.strokeWidth = star.starWidth.toFloat()
                canvas.drawPoint(star.x.toFloat(), star.y.toFloat(), paint)
            }

            canvas.drawBitmap(player.bitmap, player.x.toFloat(), player.y.toFloat(), paint)
            for (enemy in enemies) {
                canvas.drawBitmap(enemy.bitmap, enemy.x.toFloat(), enemy.y.toFloat(), paint)
            }
            canvas.drawBitmap(boom.bitmap, boom.x.toFloat(), boom.y.toFloat(), paint)

            // Desenhar o projétil
            if (projectileActive) {
                paint.color = Color.RED
                canvas.drawRect(
                    projectileX.toFloat(),
                    projectileY.toFloat(),
                    (projectileX + 20).toFloat(),
                    (projectileY + 20).toFloat(),
                    paint
                )
            }

            paint.textSize = 42f
            canvas.drawText("Lives: $lives", 10f, 100f, paint)

            surfaceHolder.unlockCanvasAndPost(canvas)
        }
    }

    fun control() {
        Thread.sleep(17)
        if (lives == 0) {
            playing = false
            Handler(Looper.getMainLooper()).post {
                onGameOver()
                gameThread?.join()
            }
        }
    }

    override fun onTouchEvent(event: MotionEvent?): Boolean {
        when (event?.action) {
            MotionEvent.ACTION_DOWN -> {
                if (event.x < width / 2) {
                    // Lado esquerdo: mover a nave
                    player.boosting = true
                } else {
                    // Lado direito: disparar projétil
                    if (!projectileActive) {
                        projectileX = player.x + player.bitmap.width
                        projectileY = player.y + player.bitmap.height / 2
                        projectileActive = true
                    }
                }
            }
            MotionEvent.ACTION_UP -> {
                if (event.x < width / 2) {
                    // Parar o movimento da nave
                    player.boosting = false
                }
            }
        }
        return true
    }
}