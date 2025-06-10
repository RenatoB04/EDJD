using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P02_TDV;
using System.Collections.Generic;

namespace P02_TDV.GameObjects
{
    internal class FirstBoss : Enemy
    {
        public Bullet bullet;
        public float fireTimer = 0f;

        private float delayInvincible = 0f;
        public bool isInvincible = true;

        public enum Direction
        {
            Left,
            Right
        }
        public Direction MovingDirection;

        public float Speed;
        public float MovedDistance;

        private Vector2 PositionHud;

        public FirstBoss(Texture2D texture) : base(texture)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(State.Instance.font, String.Format("{0}", Life), PositionHud, Color.Red);

            if (MovingDirection == Direction.Left)
            {
                spriteBatch.Draw(_texture, Position, Viewport, Color.White);
            }
            else
            {
                spriteBatch.Draw(_texture, Position, Viewport, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0f);
            }
            base.Draw(spriteBatch);
        }

        public override void Reset()
        {
            Life = 10;
            MovedDistance = 0;
            Speed = 250;
            MovingDirection = Direction.Left;
            base.Reset();
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects)
        {
            PositionHud.X = Position.X + 16;
            PositionHud.Y = Position.Y - 16;

            if (Life <= 0)
            {
                IsActive = false;

                State.Instance.isBoss1Dead = true;
            }

            delayInvincible += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (delayInvincible >= 10)
            {
                delayInvincible = 0f;
                isInvincible = !isInvincible;
                if (isInvincible)
                {
                    Viewport = new Rectangle(0, 608, 47, 48);
                }
                else
                {
                    Viewport = new Rectangle(0, 348, 32, 36);
                }
            }

            Vector2 direction = State.Instance.Player.Position - Position;

            direction.Normalize();

            Vector2 rightReference = new Vector2(1, 0);

            float dotProduct = Vector2.Dot(direction, rightReference);

            if (dotProduct > 0)
            {
                MovingDirection = Direction.Right;
            }
            else if (dotProduct < 0)
            {
                MovingDirection = Direction.Left;
            }

            Velocity = direction * Speed;
            bullet.Velocity = direction * 100;

            fireTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (fireTimer >= 2f)
            {
                var newBullet = bullet.Clone() as Bullet;
                newBullet.Position = new Vector2(Rectangle.Width / 2 + Position.X - newBullet.Rectangle.Width / 2, Position.Y);
                newBullet.Reset();
                gameObjects.Add(newBullet);
                fireTimer = 0f;
            }

            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime, gameObjects);
        }
    }
}
