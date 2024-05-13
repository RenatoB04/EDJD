using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P02_TDV.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P02_TDV
{
    internal class Enemy : GameObject
    {
        public int Life;
        private float delay = 0f;
        private bool delayActive = false;

        public bool IsOnGround
        {
            get { return isOnGround; }
        }
        bool isOnGround;

        public bool IsHitXAxis
        {
            get { return isHitXAxis; }
        }
        private bool isHitXAxis;

        public Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, Viewport.Width, Viewport.Height);
            }
        }

        private float previousBottom;

        public Enemy(Texture2D texture) : base(texture)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects)
        {
            #region begin deplay feature
            if (Life <= 0 && !delayActive)
            {
                delayActive = true;
                delay = 1f;

                Viewport = new Rectangle(0, 672, 32, 32);
                double randomNumber = State.Instance.random.NextDouble();

                if (randomNumber < 0.25 && State.Instance.life < 3)
                {
                    State.Instance.life += 1;
                    State.Instance.messageLog.AddMessage("Ganhaste uma vida", gameTime);
                }
            }

            if (delayActive)
            {
                delay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (delay <= 0)
                {
                    IsActive = false;
                    delayActive = false;
                }

                Position.Y -= 10;

            }
            #endregion

            foreach (GameObject s in gameObjects)
            {
                if (this is Enemy)
                {
                    if (IsTouching(s) && (s.Name.Equals("Player")) && !(s as Player).isInvincible)
                    {
                        State.Instance.life -= 1;
                        (s as Player).isInvincible = true;
                        break;
                    }
                }
            }


            HandleCollisions();

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
            isHitXAxis = false;

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
                                    isHitXAxis = true;

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
    }
}
