using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using P02_TDV;
using P02_TDV.GameObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace P02_TDV
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private List<GameObject> _gameObjects;
        private int _numObject;

        SpriteFont _font;
        private int levelIndex = -1;

        private const int numberOfLevels = 5;

        public Texture2D _bg, _endgame, _overlay, _titlescreen, _cave_follow;
        private Texture2D[] _tutorials;
        int i = 0;
        public Texture2D gameSprite;

        public int screenWidth = State.SCREENWIDTH;
        public int screenHeight = State.SCREENHEIGHT;
        TimeSpan currentTime;
        bool isMap = false, debug = false;

        private Vector2 cameraPosition;
        private Vector2 targetCameraPosition;
        private float cameraLerpFactor = 0.1f;

        int currentMenuIndex = 0;

        bool keepEnterOnceTime = false;

        MouseState mouseState;
        Vector2 mousePosition;
        private TextButton _playButton, _tutorialButton, _quitButton;

        private string _text = "Pressiona qualquer tecla para iniciar...";
        private bool _isVisible = true;
        private double _elapsedTime = 0;
        private double _blinkInterval = 0.5;

        private DialogueManager _dialogueManager;
        private DialogueEntity[] _entities;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Window.Title = "P02-TDV";
            _graphics.PreferredBackBufferWidth = State.SCREENWIDTH;
            _graphics.PreferredBackBufferHeight = State.SCREENHEIGHT;
            _graphics.ApplyChanges();

            _gameObjects = new List<GameObject>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("GameFont");
            State.Instance.font = _font;

            _bg = Content.Load<Texture2D>("bg/factory");

            _endgame = Content.Load<Texture2D>("Overlay/GameEnding");
            _overlay = Content.Load<Texture2D>("Overlay/overlay");
            _titlescreen = Content.Load<Texture2D>("Overlay/TitleScreen");
            _cave_follow = Content.Load<Texture2D>("Overlay/cave-follow");

            _tutorials = new Texture2D[10];

            for (int i = 0; i < _tutorials.Length; i++)
            {
                _tutorials[i] = Content.Load<Texture2D>($"Tutorials/{i + 1}");

            }

            State.Instance.messageLog = new MessageLog(10, new Vector2(10, 10), _font);

            try
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = 0.1f;
                MediaPlayer.Play(Content.Load<Song>("Sounds/Music"));
            }
            catch { }

            gameSprite = this.Content.Load<Texture2D>("sprite");

            currentTime = TimeSpan.Zero;

            _playButton = new TextButton("Jogar", new Vector2(GraphicsDevice.Viewport.Width / 2 - _font.MeasureString("Jogar").X / 2, GraphicsDevice.Viewport.Height / 2 - _font.MeasureString("Jogar").Y * 1.5f), _font);
            _tutorialButton = new TextButton("Tutorial", new Vector2(GraphicsDevice.Viewport.Width / 2 - _font.MeasureString("Tutorial").X / 2, GraphicsDevice.Viewport.Height / 2), _font);
            _quitButton = new TextButton("Sair", new Vector2(GraphicsDevice.Viewport.Width / 2 - _font.MeasureString("Sair").X / 2, GraphicsDevice.Viewport.Height / 2 + _font.MeasureString("Sair").Y * 1.5f), _font);

            #region DIALOGUE
            _entities = new DialogueEntity[]
            {
                new DialogueEntity("Character A", new string[]
                {
                    "Teste1"
                }),
                new DialogueEntity("Character B", new string[]
                {
                    "Teste2"
                }),
                new DialogueEntity("Character C", new string[]
                {
                    "Teste3"
                })
            };
        
            Texture2D dialogueBoxTexture = Content.Load<Texture2D>("Overlay/DialogueBox");

            Rectangle dialogueBox = new Rectangle(10, State.SCREENHEIGHT - 100, State.SCREENWIDTH - 20, 90);
            _dialogueManager = new DialogueManager(_font, dialogueBox, dialogueBoxTexture);
            #endregion

            LoadNextLevel();
            Reset();
        }

        protected override void Update(GameTime gameTime)
        {
            State.Instance.CurrentKey = Keyboard.GetState();
            _numObject = _gameObjects.Count;

            #region DIALOGUE
            if (Keyboard.GetState().IsKeyDown(Keys.A) && State.Instance.CurrentKey != State.Instance.PreviousKey)
            {
                _dialogueManager.SetCurrentEntity(_entities[0]);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.B) && State.Instance.CurrentKey != State.Instance.PreviousKey)
            {
                _dialogueManager.SetCurrentEntity(_entities[1]);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.C) && State.Instance.CurrentKey != State.Instance.PreviousKey)
            {
                _dialogueManager.SetCurrentEntity(_entities[2]);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S) && State.Instance.CurrentKey != State.Instance.PreviousKey)
            {
                _dialogueManager.CloseDialogueBox();
            }

            _dialogueManager.Update(gameTime);
            #endregion

            switch (State.Instance.CurrentGameState)
            {
                case State.GameState.StartNewLife:

                    State.Instance.CurrentGameState = State.GameState.GamePlaying;
                    break;

                case State.GameState.TitleScreen:
                    if (!State.Instance.CurrentKey.Equals(State.Instance.PreviousKey) && State.Instance.CurrentKey.GetPressedKeys().Length > 0)
                    {
                        State.Instance.CurrentGameState = State.GameState.StartNewLife;
                    }

                    mouseState = Mouse.GetState();
                    mousePosition = new Vector2(mouseState.X, mouseState.Y);

                    _elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

                    if (_elapsedTime >= _blinkInterval)
                    {
                        _isVisible = !_isVisible;
                        _elapsedTime = 0;
                    }

                    _playButton.Update(mousePosition);
                    _tutorialButton.Update(mousePosition);
                    _quitButton.Update(mousePosition);

                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        if (_playButton.IsHovering)
                        {
                            State.Instance.CurrentGameState = State.GameState.StartNewLife;
                        }
                        else if (_tutorialButton.IsHovering)
                        {
                            State.Instance.CurrentGameState = State.GameState.Tutorial;
                        }
                        else if (_quitButton.IsHovering)
                        {
                            Exit();
                        }
                    }
                    break;

                case State.GameState.GamePlaying:

                    currentTime += gameTime.ElapsedGameTime;

                    for (int i = 0; i < _numObject; i++)
                    {
                        if (_gameObjects[i].IsActive) _gameObjects[i].Update(gameTime, _gameObjects);

                    }
                    for (int i = 0; i < _numObject; i++)
                    {
                        if (!_gameObjects[i].IsActive)
                        {
                            _gameObjects.RemoveAt(i);
                            i--;
                            _numObject--;
                        }
                    }

                    switch (levelIndex)
                    {
                        case 3:
                            if (!State.Instance.isBoss1Dead) cameraLerpFactor = 0.01f;
                            break;
                        case 4:
                            cameraLerpFactor = 0.1f;
                            MediaPlayer.Volume = 0f;
                            break;
                        default:
                            cameraLerpFactor = 0.1f;
                            MediaPlayer.Volume = 0.1f;
                            break;
                    }

                    if (State.Instance.level.ReachedExit)
                    {
                        if (_gameObjects.OfType<Enemy>().Count() == 0)
                        {
                            if (State.Instance.isBoss1Dead && State.Instance.levelIndex == 4)
                            {
                                LoadLevel(1);
                            }
                            else
                            {
                                LoadNextLevel();
                            }
                            Reset();
                        }
                    }

                    if ((Vector2.Distance(State.Instance.Player.Position, new Vector2(1487, 1059)) < 5 &&
                        State.Instance.isSkillDefected && State.Instance.levelIndex == 2))
                    {
                        LoadLevel(1);
                        Reset();
                    }

                    if (State.Instance.life <= 0)
                    {
                        State.Instance.playerDeadCount++;
                        State.Instance.CurrentGameState = State.GameState.GameOver;
                    }

                    State.Instance.level.Update();

                    break;

                case State.GameState.GameOver:

                    if (!State.Instance.CurrentKey.Equals(State.Instance.PreviousKey) && State.Instance.CurrentKey.GetPressedKeys().Length > 0)
                    {
                        ReloadCurrentLevel();
                        Reset();
                        State.Instance.CurrentGameState = State.GameState.StartNewLife;
                    }
                    break;

                case State.GameState.Tutorial:
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape) && State.Instance.CurrentKey != State.Instance.PreviousKey)
                    {
                        keepEnterOnceTime = true;
                        State.Instance.CurrentGameState = State.GameState.TitleScreen;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Left) && State.Instance.CurrentKey != State.Instance.PreviousKey)
                    {
                        currentMenuIndex = Math.Max(0, currentMenuIndex - 1);
                        i = currentMenuIndex;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Right) && State.Instance.CurrentKey != State.Instance.PreviousKey)
                    {
                        currentMenuIndex = Math.Min(9, currentMenuIndex + 1);
                        i = currentMenuIndex;
                    }
                    break;

                case State.GameState.GameWin:
                    if (!State.Instance.CurrentKey.Equals(State.Instance.PreviousKey) && State.Instance.CurrentKey.GetPressedKeys().Length > 0)
                    {
                        LoadLevel(0);

                        State.Instance.life = 3;
                        State.Instance.playerDeadCount = 0;
                        State.Instance.isBoss1Dead = false;

                        State.Instance.isCunning = false;
                        State.Instance.isColorSight = false;
                        State.Instance.isDig = false;
                        State.Instance.isSkillDefected = false;
                        State.Instance.tick = 0;

                        currentTime = TimeSpan.Zero;

                        Reset();
                        State.Instance.CurrentGameState = State.GameState.StartNewLife;
                    }
                    break;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.F1) && State.Instance.CurrentKey != State.Instance.PreviousKey)
                debug = !debug;

            if (debug)
            {

                if (Keyboard.GetState().IsKeyDown(Keys.F2) && State.Instance.CurrentKey != State.Instance.PreviousKey)
                {
                    ReloadCurrentLevel();
                    Reset();
                }

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.F4) && State.Instance.CurrentKey != State.Instance.PreviousKey)
                    State.Instance.CurrentGameState = State.GameState.GameWin;

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.F5) && State.Instance.CurrentKey != State.Instance.PreviousKey)
                {
                    LoadNextLevel();
                    Reset();
                }

                if (Keyboard.GetState().IsKeyDown(Keys.F6) && State.Instance.CurrentKey != State.Instance.PreviousKey)
                {
                    State.Instance.CurrentGameState = State.GameState.GameOver;
                }
            }

            if (State.Instance.isColorSight && Keyboard.GetState().IsKeyDown(Keys.Tab) && State.Instance.CurrentKey != State.Instance.PreviousKey)
            {
                State.Instance.tick++;
                if (State.Instance.tick > 2) State.Instance.tick = 0;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && State.Instance.CurrentKey != State.Instance.PreviousKey)
            {
                State.Instance.CurrentGameState = State.GameState.TitleScreen;
            }

            State.Instance.PreviousKey = State.Instance.CurrentKey;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            _spriteBatch.Draw(_bg, Vector2.Zero, State.Instance.color);

            _spriteBatch.End();

            targetCameraPosition = new Vector2(
                State.Instance.Player.Position.X - (State.SCREENWIDTH / 2),
                State.Instance.Player.Position.Y - (State.SCREENHEIGHT / 2)
            );

            cameraPosition = Vector2.Lerp(cameraPosition, targetCameraPosition, cameraLerpFactor);

            Matrix m = Matrix.CreateTranslation(-cameraPosition.X, -cameraPosition.Y, 0);

            _spriteBatch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: m);

            State.Instance.level.Draw(_spriteBatch);

            _numObject = _gameObjects.Count;

            for (int i = 0; i < _numObject; i++)
            {
                _gameObjects[i].Draw(_spriteBatch);
            }

            _spriteBatch.End();

            _spriteBatch.Begin();

            #region PLAYER CHALENGE VIEW
            switch (levelIndex)
            {
                case 1:
                    if (!State.Instance.isBoss1Dead)
                    {
                        _spriteBatch.Draw(_cave_follow, Vector2.Zero, Color.White);
                    }
                    break;
            }
            #endregion

            #region PLAYER DISPLAY

            if (debug)
            {
                _spriteBatch.DrawString(_font, String.Format("Coords: {0}", State.Instance.Player.Position), new Vector2(0, 0), Color.Red);
                _spriteBatch.DrawString(_font, String.Format("Jogador: {0}", State.Instance.Player), new Vector2(0, 32), Color.Red);
                _spriteBatch.DrawString(_font, String.Format("Vida: {0}", State.Instance.life), new Vector2(0, 64), Color.Red);
                _spriteBatch.DrawString(_font, String.Format("Nivel: {0}", State.Instance.levelIndex), new Vector2(0, 96), Color.Red);
            }

            switch (State.Instance.life)
            {
                case 3:
                    _spriteBatch.Draw(gameSprite, new Vector2(0, 448), new Rectangle(0, 512, 95, 32), Color.White); 
                    break;
                case 2:
                    _spriteBatch.Draw(gameSprite, new Vector2(0, 448), new Rectangle(0, 512, 63, 32), Color.White); 
                    break;
                case 1:
                    _spriteBatch.Draw(gameSprite, new Vector2(0, 448), new Rectangle(0, 512, 31, 32), Color.White); 
                    break;
                default:
                    _spriteBatch.Draw(gameSprite, new Vector2(0, 448), new Rectangle(0, 512, 95, 32), Color.White); 
                    break;

            }

            if (State.Instance.isCunning)
            {
                _spriteBatch.Draw(gameSprite, new Vector2(64, 416), new Rectangle(0, 912, 32, 32), Color.White);
            }

            if (State.Instance.isDig)
            {
                _spriteBatch.Draw(gameSprite, new Vector2(0, 416), new Rectangle(0, 560, 32, 32), Color.White);
            }

            if (State.Instance.isColorSight)
            {
                _spriteBatch.Draw(gameSprite, new Vector2(96, 416), new Rectangle(0, 960, 32, 32), Color.White);
            }

            if (State.Instance.isSkillDefected)
            {
                _spriteBatch.Draw(gameSprite, new Vector2(32, 416), new Rectangle(0, 864, 32, 32), Color.White);

            }

            _spriteBatch.Draw(gameSprite, new Vector2(0, 384), new Rectangle(0, 720, 32, 32), Color.White);
            _spriteBatch.DrawString(_font, String.Format("{0}", _gameObjects.OfType<Enemy>().Count()), new Vector2(32, 384), Color.Red);

            _spriteBatch.Draw(gameSprite, new Vector2(0, 352), new Rectangle(0, 768, 32, 32), Color.White);
            _spriteBatch.DrawString(_font, String.Format("{0}", State.Instance.playerDeadCount), new Vector2(32, 352), Color.Red);

            if (!keepEnterOnceTime)
            {
                _spriteBatch.Draw(gameSprite, new Vector2(0, 0), new Rectangle(64, 912, 64, 32), Color.White);
            }
            #endregion

            if (State.Instance.isBoss1Dead && State.Instance.levelIndex == 4)
            {
                Vector2 iamuPosition = new Vector2(screenWidth / 2, screenHeight / 2);
                _spriteBatch.DrawString(_font, "Derrotaste o Boss!", iamuPosition, Color.Red, 0, _font.MeasureString("Derrotaste o Boss!") / 2, 2.0f, SpriteEffects.None, 0);
            }

            #region OVERLAY
            switch (State.Instance.CurrentGameState)
            {
                case State.GameState.Tutorial:

                    _spriteBatch.Draw(_tutorials[i], Vector2.Zero, Color.White);

                    break;
            }

            if (State.Instance.CurrentGameState == State.GameState.GameWin)
            {
                _spriteBatch.Draw(_endgame, Vector2.Zero, Color.White); 
            }

            _spriteBatch.End();

            _spriteBatch.Begin();

            if (State.Instance.CurrentGameState == State.GameState.GameOver)
            {
                _spriteBatch.Draw(_overlay, Vector2.Zero, Color.Black * 0.7f);

                Vector2 gameOverPosition = new Vector2(screenWidth / 2, screenHeight / 4);
                _spriteBatch.DrawString(_font, "MORRESTE", gameOverPosition - new Vector2(64, 0), Color.Red);

                Vector2 deathCountPosition = new Vector2(screenWidth / 2, screenHeight / 2);
                string deathCountText = String.Format("Total de mortes: {0}{1}", State.Instance.playerDeadCount, State.Instance.playerDeadCount == 1 ? "" : "s");
                Vector2 deathCountOrigin = _font.MeasureString(deathCountText) / 2;
                _spriteBatch.DrawString(_font, deathCountText, deathCountPosition, Color.White, 0, deathCountOrigin, 1.0f, SpriteEffects.None, 0);

                Vector2 keepFightingPosition = new Vector2(screenWidth / 2, (screenHeight / 4) * 3);
                _spriteBatch.DrawString(_font, "Continua a lutar!", keepFightingPosition, Color.Yellow, 0, _font.MeasureString("Continua a lutar!") / 2, 1.0f, SpriteEffects.None, 0);

            }

            _spriteBatch.DrawString(_font, currentTime.ToString(@"hh\:mm\:ss"), new Vector2(704, 10), Color.White);

            if (State.Instance.CurrentGameState == State.GameState.TitleScreen)
            {
                _spriteBatch.Draw(_titlescreen, Vector2.Zero, Color.White);

                if (_isVisible)
                {
                    _spriteBatch.DrawString(_font, _text, new Vector2(State.SCREENWIDTH / 2 - (_font.MeasureString(_text).X) / 2, State.SCREENHEIGHT / 4), Color.Yellow);
                }

                _playButton.Draw(_spriteBatch, Color.White, Color.Yellow);
                _tutorialButton.Draw(_spriteBatch, Color.White, Color.Yellow);
                _quitButton.Draw(_spriteBatch, Color.White, Color.Yellow);
            }

            _dialogueManager.Draw(_spriteBatch);

            State.Instance.messageLog.Draw(_spriteBatch, gameTime);

            _spriteBatch.End();

            #endregion OVERLAY

            _graphics.BeginDraw();

            base.Draw(gameTime);
        }

        bool istemp = false;

        #region RESET & RESET_ENEMY
        protected void Reset()
        {
            if(!istemp)
            {
                State.Instance.CurrentGameState = State.GameState.TitleScreen;
                istemp = true;
            }
            else
            {
                State.Instance.CurrentGameState = State.GameState.StartNewLife;
            }

            SoundEffect shotSound = this.Content.Load<SoundEffect>("Sounds/PlayerShot");

            _gameObjects.Clear();

            _gameObjects.Add(new Player(gameSprite, Content.Load<SoundEffect>("Sounds/TakeDamage"))
            {
                Name = "Player",
                Viewport = new Rectangle(0, 348, 32, 36),
                Position = new Vector2(500, 300),
                Left = Keys.Left,
                Right = Keys.Right,
                Fire = Keys.Space,
                SoundEffect = shotSound,
                Up = Keys.Up,
                bullet = new Bullet(gameSprite)
                {
                    Name = "PlayerBullet",
                    Viewport = new Rectangle(224, 352, 32, 32),
                    Velocity = new Vector2(200f, 0),
                    axisDirection = Bullet.Axis.X
                }
            });

            ResetEnemies();

            foreach (GameObject s in _gameObjects)
            {
                s.Reset();
            }

        }

        protected void ResetEnemies()
        {
            switch (State.Instance.levelIndex)
            {

                case 1:
                    {
                        Skull skull = new Skull(gameSprite)
                        {
                            Name = "Skull",
                            Viewport = new Rectangle(0, 0, 32, 32),
                            Position = new Vector2(1060, 317),
                        };

                        _gameObjects.Add(skull);

                        ShieldMonster shieldMonster = new ShieldMonster(gameSprite)
                        {
                            Name = "ShieldMonster",
                            Viewport = new Rectangle(0, 128, 32, 32),
                            Position = new Vector2(2944, 799),
                            Speed = 50
                        };

                        _gameObjects.Add(shieldMonster);
                    }

                    break;
                case 2:
                    {
                        Skull skull = new Skull(gameSprite)
                        {
                            Name = "Skull",
                            Viewport = new Rectangle(0, 0, 32, 32),
                            Position = new Vector2(1208, 2365),
                        };

                        _gameObjects.Add(skull);


                        var cloneSkull = skull.Clone() as Skull;
                        cloneSkull.Position = new Vector2(1200, 2365);
                        _gameObjects.Add(cloneSkull);

                        ShieldMonster shieldMonster = new ShieldMonster(gameSprite)
                        {
                            Name = "ShieldMonster",
                            Viewport = new Rectangle(0, 128, 32, 32),
                            Position = new Vector2(2480, 2270),
                            Speed = 0
                        };

                        _gameObjects.Add(shieldMonster);

                        var cloneShieldMonster = shieldMonster.Clone() as ShieldMonster;
                        cloneShieldMonster.Position = new Vector2(2520, 2367);
                        cloneShieldMonster.Speed = 50;
                        _gameObjects.Add(cloneShieldMonster);




                        GunMonster gunMonster = new GunMonster(gameSprite)
                        {
                            Name = "GunMonster",
                            Viewport = new Rectangle(0, 65, 32, 32),
                            Position = new Vector2(1170, 1697),
                            bullet = new Bullet(gameSprite)
                            {
                                Name = "EnemyBullet",
                                Viewport = new Rectangle(224, 352, 32, 32),
                                Velocity = new Vector2(100f, 0),
                                axisDirection = Bullet.Axis.X
                            }
                        };
                        _gameObjects.Add(gunMonster);

                        var cloneGunMonster = gunMonster.Clone() as GunMonster;
                        cloneGunMonster.Position = new Vector2(1160, 1570);
                        _gameObjects.Add(cloneGunMonster);

                        PlaneMonster planeMonster = new PlaneMonster(gameSprite)
                        {
                            Name = "PlaneMonster",
                            Viewport = new Rectangle(0, 193, 63, 37),
                            Position = new Vector2(2300, 1570),
                            bullet = new Bullet(gameSprite)
                            {
                                Name = "EnemyBullet",
                                Viewport = new Rectangle(224, 352, 32, 32),
                                Velocity = new Vector2(0, 100f),
                                axisDirection = Bullet.Axis.Y
                            }
                        };
                        _gameObjects.Add(planeMonster);

                    }
                    break;

                case 3:
                    {

                        GunMonster gunMonster = new GunMonster(gameSprite)
                        {
                            Name = "GunMonster",
                            Viewport = new Rectangle(0, 65, 32, 32),
                            Position = new Vector2(816, 348),
                            bullet = new Bullet(gameSprite)
                            {
                                Name = "EnemyBullet",
                                Viewport = new Rectangle(224, 352, 32, 32),
                                Velocity = new Vector2(100f, 0),
                                axisDirection = Bullet.Axis.X
                            }
                        };

                        _gameObjects.Add(gunMonster);

                        var cloneGunMonster = gunMonster.Clone() as GunMonster;
                        cloneGunMonster.Position = new Vector2(1248, 226);
                        _gameObjects.Add(cloneGunMonster);

                        var cloneGunMonster1 = gunMonster.Clone() as GunMonster;
                        cloneGunMonster1.Position = new Vector2(1655, 867);
                        _gameObjects.Add(cloneGunMonster1);

                        var cloneGunMonster2 = gunMonster.Clone() as GunMonster;
                        cloneGunMonster2.Position = new Vector2(2450, 732);
                        _gameObjects.Add(cloneGunMonster2);

                        var cloneGunMonster3 = gunMonster.Clone() as GunMonster;
                        cloneGunMonster3.Position = new Vector2(2083, 156);
                        _gameObjects.Add(cloneGunMonster3);

                        var cloneGunMonster4 = gunMonster.Clone() as GunMonster;
                        cloneGunMonster.Position = new Vector2(2962, 30);
                        _gameObjects.Add(cloneGunMonster4);

                        PlaneMonster planeMonster = new PlaneMonster(gameSprite)
                        {
                            Name = "PlaneMonster",
                            Viewport = new Rectangle(0, 193, 63, 37),
                            Position = new Vector2(1647, 50),
                            bullet = new Bullet(gameSprite)
                            {
                                Name = "EnemyBullet",
                                Viewport = new Rectangle(224, 352, 32, 32),
                                Velocity = new Vector2(0, 100f),
                                axisDirection = Bullet.Axis.Y
                            }
                        };

                        planeMonster.Reset();
                        _gameObjects.Add(planeMonster);

                    }
                    break;

                case 4:
                    {

                        FirstBoss firstBoss = new FirstBoss(gameSprite)
                        {
                            Name = "FirstBoss",
                            Viewport = new Rectangle(0, 608, 47, 48),
                            Position = new Vector2(1850, 1104),
                            bullet = new Bullet(gameSprite)
                            {
                                Name = "EnemyBullet",
                                Viewport = new Rectangle(224, 352, 32, 32),
                                Velocity = new Vector2(100f, 0),
                                axisDirection = Bullet.Axis.X
                            }
                        };

                        firstBoss.Reset();
                        _gameObjects.Add(firstBoss);
                    }
                    break;
                case 0:
                    {
                        Skull skull = new Skull(gameSprite)
                        {
                            Name = "Skull",
                            Viewport = new Rectangle(0, 0, 32, 32),
                            Position = new Vector2(1120, 383),
                        };

                        _gameObjects.Add(skull);

                        var cloneSkull1 = skull.Clone() as Skull;
                        cloneSkull1.Position = new Vector2(2120, 387);
                        _gameObjects.Add(cloneSkull1);

                        PlaneMonster planeMonster = new PlaneMonster(gameSprite)
                        {
                            Name = "PlaneMonster",
                            Viewport = new Rectangle(0, 193, 63, 37),
                            Position = new Vector2(960, 189),
                            bullet = new Bullet(gameSprite)
                            {
                                Name = "EnemyBullet",
                                Viewport = new Rectangle(224, 352, 32, 32),
                                Velocity = new Vector2(0, 100f),
                                axisDirection = Bullet.Axis.Y
                            }
                        };

                        _gameObjects.Add(planeMonster);

                        var clonePlaneMonster = planeMonster.Clone() as PlaneMonster;
                        clonePlaneMonster.Position = new Vector2(2000, 300);
                        _gameObjects.Add(clonePlaneMonster);


                        GunMonster gunMonster = new GunMonster(gameSprite)
                        {
                            Name = "GunMonster",
                            Viewport = new Rectangle(0, 65, 32, 32),
                            Position = new Vector2(1808, 387),
                            bullet = new Bullet(gameSprite)
                            {
                                Name = "EnemyBullet",
                                Viewport = new Rectangle(224, 352, 32, 32),
                                Velocity = new Vector2(100f, 0),
                                axisDirection = Bullet.Axis.X
                            }
                        };
                        _gameObjects.Add(gunMonster);

                        ShieldMonster shieldMonster = new ShieldMonster(gameSprite)
                        {
                            Name = "ShieldMonster",
                            Viewport = new Rectangle(0, 128, 32, 32),
                            Position = new Vector2(1927, 387)
                        };

                        _gameObjects.Add(shieldMonster);
                    }
                    break;
                default: return;
            }
        }

        #endregion

        private void LoadNextLevel()
        {
            levelIndex = (levelIndex + 1) % numberOfLevels;

            State.Instance.levelIndex = levelIndex;

            if (State.Instance.level != null)
                State.Instance.level.Dispose();

            string levelPath = string.Format("Content/Levels/{0}.txt", levelIndex);
            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
                State.Instance.level = new TileBuilder(Services, fileStream);
        }

        private void ReloadCurrentLevel()
        {
            --levelIndex;
            LoadNextLevel();
        }

        private void LoadLevel(int levelDestination)
        {
            levelIndex = (levelDestination) % numberOfLevels;

            State.Instance.levelIndex = levelIndex;

            if (State.Instance.level != null)
                State.Instance.level.Dispose();

            string levelPath = string.Format("Content/Levels/{0}.txt", levelIndex);
            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
                State.Instance.level = new TileBuilder(Services, fileStream);
        }
    }
}
