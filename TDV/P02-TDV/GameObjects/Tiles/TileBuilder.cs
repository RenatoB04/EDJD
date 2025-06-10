using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Text.RegularExpressions;
using P02_TDV;

namespace P02_TDV.GameObjects
{

    class TileBuilder : IDisposable
    {
        public Tile[,] tiles;

        List<string> lines = new List<string>();
        char tileType;
     
        private Point exit = InvalidPosition;
        private static readonly Point InvalidPosition = new Point(-1, -1);

        private Random random = new Random(354668);

        public bool ReachedExit
        {
            get { return reachedExit; }
        }
        bool reachedExit;
    
        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        #region Loading

        public TileBuilder(IServiceProvider serviceProvider, Stream fileStream)
        {
            content = new ContentManager(serviceProvider, "Content");

            LoadTiles(fileStream);

        }

        private void LoadTiles(Stream fileStream)
        {
            int width;

            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine();
                width = line.Length;
                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != width)
                        throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                    line = reader.ReadLine();
                }
            }

            tiles = new Tile[width, lines.Count];

            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    tileType = lines[y][x];
                    tiles[x, y] = LoadTile(tileType, x, y);
                }
            }
        }

        private Tile LoadTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                case '.':
                    return new Tile(null, TileCollision.Passable);

                case 'X':
                    return LoadExitTile(x, y);

                case '-':
                    return LoadTile("Platform", TileCollision.Platform);

                case 'L':
                    return LoadTile("LockDoor", TileCollision.Impassable);

                case 'K':
                    return LoadTile("Key", TileCollision.Passable);

                case 'S':
                    return LoadTile("DigSkill", TileCollision.Passable);

                case 'G':
                    return LoadTile("God", TileCollision.Passable);

                case '(':
                    return LoadTile("Invisible", TileCollision.Impassable);

                case ')':
                    return LoadVarietyTile("BlockA", 7, TileCollision.Passable);

                case '~':
                    return LoadVarietyTile("BlockB", 2, TileCollision.Impassable);

                case ':':
                    return LoadVarietyTile("BlockB", 2, TileCollision.Passable);

                case '#':
                    return LoadVarietyTile("BlockA", 7, TileCollision.Impassable);

                case 'W':
                    return LoadVarietyTile("WarpA", 4, TileCollision.Passable);

                case '9':
                    return LoadTile("InDefectSkill", TileCollision.Passable);

                case '8':
                    return LoadTile("cunning", TileCollision.Passable);

                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", tileType, x, y));
            }
        }

        private Tile LoadTile(string name, TileCollision collision)
        {
            return new Tile(Content.Load<Texture2D>("Tiles/" + name), collision);
        }

        private Tile LoadVarietyTile(string baseName, int variationCount, TileCollision collision)
        {
            int index = random.Next(variationCount);
            return LoadTile(baseName + index, collision);
        }

        private Tile LoadExitTile(int x, int y)
        {
            if (exit != InvalidPosition)
                throw new NotSupportedException("A level may only have one exit.");

            exit = GetBounds(x, y).Center;

            return LoadTile("Exit", TileCollision.Passable);
        }

        public void Dispose()
        {
            Content.Unload();
        }

        #endregion

        #region Bounds and collision

        public TileCollision GetCollision(int x, int y)
        {
            if (x < 0 || x >= Width)
                return TileCollision.Impassable;
            if (y < 0 || y >= Height)
                return TileCollision.Passable;

            return tiles[x, y].Collision;
        }


        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }


        public int Width
        {
            get { return tiles.GetLength(0); }
        }


        public int Height
        {
            get { return tiles.GetLength(1); }
        }

        #endregion

        #region Update

        public void Update()
        {
            reachedExit = false;

            if (Vector2.Distance(State.Instance.Player.Position, new Vector2(exit.X, exit.Y)) < 20)
            {
                OnExitReached();
            }

        }


        private void OnExitReached()
        {
            reachedExit = true;
        }

        #endregion

        #region Draw


        public void Draw(SpriteBatch spriteBatch)
        {

            DrawTiles(spriteBatch);

        }

        private void DrawTiles(SpriteBatch spriteBatch)
        {
            Color color;

            switch (State.Instance.levelIndex)
            {
                case 1:
                    color = Color.Green;
                    break;
                case 2:
                    color = Color.Blue;
                    break;
                case 3:
                    color = Color.Red;
                    break;
                default:
                    color = Color.White;
                    break;
            }

            State.Instance.color = color;

            Color originalColor = color;

            int r = originalColor.R;
            int g = originalColor.G;
            int b = originalColor.B;

            int complementaryR = 255 - r;
            int complementaryG = 255 - g;
            int complementaryB = 255 - b;

            Color complementaryColor = new Color(complementaryR, complementaryG, complementaryB);

            Color targetColor;

            switch (State.Instance.tick)
            {
                case 0: targetColor = originalColor; break;
                case 1: targetColor = complementaryColor; break;
                case 2: targetColor = Color.White; break;
                default: targetColor = Color.White; break;
            }

            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {

                    Texture2D texture = tiles[x, y].Texture;

                    if (texture != null)
                    {
                        Vector2 position = new Vector2(x, y) * Tile.Size;

                        if (Regex.IsMatch(lines[y][x].ToString(), "[XWLKSG98]"))
                        {
                            spriteBatch.Draw(texture, position, Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(texture, position, targetColor);

                        }


                    }
                }
            }
        }

        #endregion
    }
}