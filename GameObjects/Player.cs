using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using P02_TDV.GameObjects;
using System;
using System.Collections.Generic;

namespace P02_TDV
{
    class Player : GameObject
    {
        public Keys Left, Right, Fire, Up;
        public Bullet bullet;

        private float jumpVelocity = -8000f;
        private float gravity = 10000f;

        public bool isInvincible = false;
        private float delayInvincible = 0f;
        private bool isVisible = true;
        private float blinkTimer = 0f;

        public bool isHint = false;
        private bool swap = false;

        public int posX, posY;

        public Vector2 PositionHud;
        public enum Direction
        {
            Left,
            Right
        }
        public Direction MovingDirection;

        private float previousBottom;

        private SoundEffect deathSound;

        public bool IsOnGround
        {
            get { return isOnGround; }
        }
        bool isOnGround;

        public Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, Viewport.Width, Viewport.Height);
            }
        }

        public Player(Texture2D texture) : base(texture)
        {
        }

        public Player(Texture2D texture, SoundEffect soundEffect) : base(texture, soundEffect)
        {
            deathSound = soundEffect;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            
            if (isVisible)
            {
                if (MovingDirection == Direction.Right)
                {
                    spriteBatch.Draw(_texture, Position, Viewport, Color.White);
                }
                else
                {
                    spriteBatch.Draw(_texture, Position, Viewport, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0f);
                }
            }

            if (isHint)
            {
                spriteBatch.Draw(_texture, PositionHud, new Rectangle(32, 912, 32, 32), Color.White);
            }

            base.Draw(spriteBatch);
        }

        public override void Reset()
        {
            State.Instance.life = 3;

            MovingDirection = Direction.Right;
            isInvincible = false;

            base.Reset();
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects)
        {
            isHint = false;

            PositionHud.X = Position.X + 16;
            PositionHud.Y = Position.Y - 16;

            if (isInvincible)
            {
                delayInvincible += (float)gameTime.ElapsedGameTime.TotalSeconds;
                blinkTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (blinkTimer >= 0.2f)
                {
                    isVisible = !isVisible;
                    blinkTimer = 0f;
                }

                if (delayInvincible >= 3)
                {
                    isInvincible = false;
                    delayInvincible = 0f;
                    isVisible = true;

                    deathSound.Play();
                }
            }

            if (State.Instance.CurrentKey.IsKeyDown(Left))
            {
                Velocity.X = -500;
                MovingDirection = Direction.Left;
                bullet.Velocity.X = -100f;
            }
            if (State.Instance.CurrentKey.IsKeyDown(Right))
            {
                Velocity.X = 500;
                MovingDirection = Direction.Right;
                bullet.Velocity.X = 100f;
            }

            if (State.Instance.CurrentKey.IsKeyDown(Up) && isOnGround)
            {
                Velocity.Y = jumpVelocity;
            }

            if (State.Instance.CurrentKey.IsKeyDown(Fire) && State.Instance.CurrentKey != State.Instance.PreviousKey)
            {
                Viewport = new Rectangle(64, 348, 36, 36);
                var newBullet = bullet.Clone() as Bullet;
                newBullet.Position = new Vector2(Rectangle.Width / 2 + Position.X - newBullet.Rectangle.Width / 2, Position.Y);
                newBullet.Reset();
                gameObjects.Add(newBullet);

                SoundEffect.Play();
            }

            if (State.Instance.isCunning && State.Instance.CurrentKey.IsKeyDown(Keys.H) && State.Instance.CurrentKey != State.Instance.PreviousKey)
            {
                swap = !swap;
            }

            if (State.Instance.isDig && State.Instance.CurrentKey.IsKeyDown(Keys.Q) && State.Instance.PreviousKey != State.Instance.CurrentKey)
            {
                State.Instance.level.tiles[posX, posY] = new Tile(null, TileCollision.Passable);
            }

            Velocity.Y += gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            State.Instance.Player.Position = Position;

            Velocity = Vector2.Zero;

            HandleCollisions();

            hasPlayerEnterto(gameTime);
            BossLevelHandler();

            base.Update(gameTime, gameObjects);
        }

        #region Collision
        private void HandleCollisions()
        {
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

            isOnGround = false;

            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    TileCollision collision = State.Instance.level.GetCollision(x, y);
                    if (collision != TileCollision.Passable)
                    {
                        Rectangle tileBounds = State.Instance.level.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            if (absDepthY < absDepthX || collision == TileCollision.Platform)
                            {
                                if (previousBottom <= tileBounds.Top)
                                    isOnGround = true;

                                if (collision == TileCollision.Impassable || IsOnGround)
                                {

                                    posX = x;
                                    posY = y;

                                    Position = new Vector2(Position.X, Position.Y + depth.Y);

                                    bounds = BoundingRectangle;
                                }
                            }
                            else if (collision == TileCollision.Impassable)
                            {
                                Position = new Vector2(Position.X + depth.X, Position.Y);

                                bounds = BoundingRectangle;
                            }
                        }
                    }
                }
            }

            previousBottom = bounds.Bottom;
        }
        #endregion

        #region Level Handele In map By Player
        public void Level2WarpPoint()
        {
            if (Vector2.Distance(Position, new Vector2(40, 418)) < 20)
            {
                Position = new Vector2(983, 2370);
            }

            if (Vector2.Distance(Position, new Vector2(806, 2146)) < 20)
            {
                Position = new Vector2(760, 1697);
            }

            if (Vector2.Distance(Position, new Vector2(1120, 1570)) < 20)
            {
                Position = new Vector2(2087, 1697);
            }

            if (Vector2.Distance(Position, new Vector2(2445, 1570)) < 20)
            {
                Position = new Vector2(2480, 418);
            }

            if (Vector2.Distance(Position, new Vector2(2445, 1697)) < 20)
            {
                Position = new Vector2(2126, 2274);
            }

            if (Vector2.Distance(Position, new Vector2(1163, 2274)) < 20)
            {
                Position = new Vector2(2126, 2274);
            }

            if (Vector2.Distance(Position, new Vector2(2126, 2145)) < 20)
            {
                Position = new Vector2(760, 1697);
            }

            if (Vector2.Distance(Position, new Vector2(1170, 1697)) < 20)
            {
                Position = new Vector2(2126, 2274);
            }

            if (Vector2.Distance(Position, new Vector2(2485, 2274)) < 20)
            {
                Position = new Vector2(806, 2274);
            }

            if (Vector2.Distance(Position, new Vector2(760, 1569)) < 20)
            {
                Position = new Vector2(806, 2274);
            }

            if (Vector2.Distance(Position, new Vector2(1170, 1697)) < 20)
            {
                Position = new Vector2(2126, 2274);
            }
        }

        public void hasPlayerEnterto(GameTime gameTime)
        {
            switch (State.Instance.levelIndex)
            {
                case 0:
                    if ((Vector2.Distance(Position, new Vector2(1400, 700)) < 50) || ((Vector2.Distance(Position, new Vector2(2600, 630)) < 100)) || ((Vector2.Distance(Position, new Vector2(2761, 800)) < 100)))
                    {
                        State.Instance.life = 0;
                    }

                    if (State.Instance.levelIndex == 0)
                    {
                        if (swap && Vector2.Distance(Position, new Vector2(3120, 387)) < 5)
                        {
                            State.Instance.messageLog.AddMessage("Elimina todos os inimigos para continuar", gameTime);
                        }
                    }

                    break;
                case 1:
                    if (Vector2.Distance(Position, new Vector2(800, 222)) < 20)
                    {
                        Position = new Vector2(977, 862);
                    }

                    if (State.Instance.isBoss1Dead && Vector2.Distance(Position, new Vector2(3048, 797)) < 10)
                    {
                        {
                            State.Instance.level.tiles[44, 25] = new Tile(null, TileCollision.Passable);

                            State.Instance.level.tiles[76, 25] = new Tile(null, TileCollision.Passable);

                            State.Instance.messageLog.AddMessage("A porta secreta foi destrancada" +
                                "" +
                                "", gameTime);
                        }
                    }

                    if (State.Instance.isBoss1Dead && Vector2.Distance(Position, new Vector2(398, 830)) < 5)
                    {

                        {
                            State.Instance.isDig = true;
                            State.Instance.level.tiles[10, 26] = new Tile(null, TileCollision.Passable);
                            State.Instance.messageLog.AddMessage("Adquiriste a habilidade de escavar", gameTime);
                            State.Instance.messageLog.AddMessage("Pressiona Q para escavar", gameTime);
                        }
                    }

                    {
                        if (Vector2.Distance(Position, new Vector2(1440, 835)) < 4)
                        {
                            State.Instance.messageLog.AddMessage("Por finalizar", gameTime);
                        }
                        if (Vector2.Distance(Position, new Vector2(1200, 835)) < 4)
                        {
                            State.Instance.messageLog.AddMessage("Por finalizar", gameTime);
                        }
                        if (Vector2.Distance(Position, new Vector2(968, 835)) < 4)
                        {
                            State.Instance.messageLog.AddMessage("Por finalizar", gameTime);
                        }
                        if (Vector2.Distance(Position, new Vector2(720, 835)) < 4)
                        {
                            State.Instance.messageLog.AddMessage("Por finalizar", gameTime);
                        }
                    }

                    {
                        if (!State.Instance.isSkillDefected && Vector2.Distance(Position, new Vector2(1000, 2000)) < 1000)
                        {
                            State.Instance.messageLog.AddMessage("Ainda nao podes acessar esta zona", gameTime);
                            State.Instance.messageLog.AddMessage("Precisas de desbloquear a habilidade da perfeicao primeiro", gameTime);
                            State.Instance.life = 0;
                        }
                    }

                    {
                        if (!State.Instance.isBoss1Dead && Vector2.Distance(Position, new Vector2(1800, 803)) < 5)
                        {
                            State.Instance.messageLog.AddMessage("Encontra a chave neste nivel", gameTime);
                            isHint = true;
                        }

                        if (Vector2.Distance(Position, new Vector2(520, 867)) < 5)
                        {
                            isHint = true;
                        }

                        if (swap && Vector2.Distance(Position, new Vector2(520, 867)) < 5)
                        {
                            State.Instance.messageLog.AddMessage("Tenta saltar na parede", gameTime);
                        }
                    }

                    {
                        if (!State.Instance.isBoss1Dead && Vector2.Distance(Position, new Vector2(2968, 799)) < 5)
                        {
                            State.Instance.messageLog.AddMessage("Elimina o Boss primeiro", gameTime);
                        }
                    }

                    {
                        if (Vector2.Distance(Position, new Vector2(1150, 1600)) < 10)
                        {
                            State.Instance.messageLog.AddMessage("Acabou, provaste ser digno.", gameTime);
                            State.Instance.CurrentGameState = State.GameState.GameWin;
                        }
                    }

                    break;
                case 2:
                    Level2WarpPoint();

                    {
                        if (Vector2.Distance(Position, new Vector2(1487, 1055)) < 5)
                        {
                            State.Instance.messageLog.AddMessage("Adquiriste a habilidade da perfeicao", gameTime);
                            State.Instance.isSkillDefected = true;
                            State.Instance.level.tiles[37, 33] = new Tile(null, TileCollision.Passable);
                        }
                    }

                    {
                        if (swap && Vector2.Distance(Position, new Vector2(2480, 415)) < 10)
                        {
                            State.Instance.messageLog.AddMessage("Tenta escavar", gameTime);
                        }
                        if (Vector2.Distance(Position, new Vector2(2480, 415)) < 5)
                        {
                            isHint = true;
                        }
                    }

                    break;
                case 3:
                    {
                        if (Vector2.Distance(Position, new Vector2(3000, 0)) < 10)
                        {
                            State.Instance.messageLog.AddMessage("Desbloqueaste a habilidade de obter dicas", gameTime);
                            State.Instance.messageLog.AddMessage("Pressiona H quando estiveres num ? amarelo", gameTime);
                            State.Instance.isCunning = true;
                            State.Instance.level.tiles[75, 0] = new Tile(null, TileCollision.Passable);
                        }
                    }

                    break;
                case 4:
                    if (Vector2.Distance(Position, new Vector2(880, 2018)) < 5)
                    {
                        State.Instance.CurrentGameState = State.GameState.GameWin;
                    }

                    if (State.Instance.isColorSight == false && State.Instance.isBoss1Dead)
                    {
                        State.Instance.level.tiles[68, 35] = new Tile(null, TileCollision.Passable);

                        State.Instance.messageLog.AddMessage("A porta abriu-se", gameTime);

                        State.Instance.messageLog.AddMessage("Adquiriste o Olho das Cores", gameTime);
                        State.Instance.messageLog.AddMessage("Pressiona TAB para ativar", gameTime);

                        State.Instance.isColorSight = true;

                        State.Instance.messageLog.AddMessage("O Boss foi eliminado", gameTime);
                    }

                    break;
                default: break;
            }
        }

        public void BossLevelHandler()
        {
            if (State.Instance.isBoss1Dead)
            {
                if (State.Instance.levelIndex == 1)
                {
                    State.Instance.level.tiles[75, 25] = new Tile(null, TileCollision.Passable); 
                }
            }
        }
        #endregion
    }
}
