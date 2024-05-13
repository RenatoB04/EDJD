using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using P02_TDV;
using P02_TDV.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P02_TDV
{
    class State
    {
        public const int SCREENWIDTH = 800;
        public const int SCREENHEIGHT = 480;

        public enum GameState
        {
            StartNewLife,
            GamePlaying,
            GameOver,
            Tutorial,
            GameWin,
            TitleScreen,
        }

        public GameState CurrentGameState;

        public KeyboardState PreviousKey, CurrentKey;
        private static State instance;

        public Random random = new Random();

        private State() { }

        public TileBuilder level;

        public Player Player = new(null);

        public List<GameObject> gameObjects = null;

        public Texture2D Tuxture = null;

        public int levelIndex;

        public int life = 3;

        public MessageLog messageLog;

        public Color color;

        public int tick;

        public bool isCunning = false;
        public bool isColorSight = false;
        public bool isDig = false;
        public bool isSkillDefected = false;

        public SpriteFont font;

        public bool isBoss1Dead = false;

        public int playerDeadCount = 0;


        public static State Instance

        {
            get
            {
                if (instance == null)
                {
                    instance = new State();
                }
                return instance;
            }
        }
    }
}
