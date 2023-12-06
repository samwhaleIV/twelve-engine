using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Dynamics;
using System;
using System.Collections.Generic;
using TwelveEngine.Game2D;
using TwelveEngine.Shell;
using TwelveEngine.Shell.UI;
using TwelveEngine.UI.Book;
using TwelveEngine;
using TwelveEngine.Input;
using TwelveEngine.Font;

namespace John {

    using static Constants;

    public sealed class CollectionGame:GameState2D {

        public GrabbyClaw GrabbyClaw { get; private set; }
        public PoolOfJohns JohnPool { get; private init; }
        public JohnDecorator Decorator { get; private set; }

        public Random Random { get; private init; } = Flags.Get(NO_RANDOM_SEED_FLAG) ? new Random(0) : new Random();

        public CollectionGame() {
            Name = "John Collection Game";
            OnLoad.Add(LoadGame);

            OnUpdate.Add(UpdateDPI,EventPriority.First);

            OnUpdate.Add(UpdateGame);

            OnUpdate.Add(CameraTracking);
            OnUnload.Add(UnloadGame);

            OnUpdate.Add(UpdateUI);
            OnRender.Add(RenderUI);

            Impulse.OnEvent += Impulse_OnEvent;

            PhysicsScale = PHYSICS_SIM_SCALE;
            JohnPool = new PoolOfJohns(this);
        }

        private void Impulse_OnEvent(ImpulseEvent impulseEvent) {
            if(impulseEvent.Impulse == TwelveEngine.Input.Impulse.Accept && impulseEvent.Pressed) {
                AdvanceStartScreen();
            }
        }

        private void CameraTracking() {
            Camera.Position = GrabbyClaw.Position;
        }

        private void UpdateDPI() {
            var scale = Viewport.Bounds.Height * DPI_SCALE_CONSTANT;
            scale = MathF.Max(MathF.Floor(scale),1);
            Camera.Scale = scale;
        }

        private NineGrid _nineGridBackground;

        private void LoadGame() {
            Camera.TileInputSize = INPUT_TILE_SIZE;
            Camera.Scale = CAMERA_SCALE;

            OnWriteDebug.Add(DrawDebugText);

            TileMapTexture = Content.Load<Texture2D>(TILEMAP_TEXTURE);
            Decorator = new JohnDecorator(GraphicsDevice,TileMapTexture,JOHN_ANIMATION_SOURCE);
            TileMap = TileMap.CreateFromJSON(MAP_FILE);

            GenerateWorldCollision(tileValue => !NON_COLLIDING_TILES.Contains(tileValue),SURFACE_FRICTION);
            GenerateGenericTileDictionary();

            PhysicsWorld.Gravity = new Vector2(0,GRAVITY);

            if(USE_CAM_BOUNDS) {
                SetCameraBounds();
                Camera.MinX += CAM_OFFSET_MIN_X;
                Camera.MaxX += CAM_OFFSET_MAX_X;
                Camera.MinY += CAM_OFFSET_MIN_Y;
                Camera.MaxY += CAM_OFFSET_MAX_Y;
            }

            GrabbyClaw = new GrabbyClaw(this) {
                MinX = CLAW_OFFSET_MIN_X,
                MaxX = TileMap.Width + CLAW_OFFSET_MAX_X,
                MinY = CLAW_OFFSET_MIN_Y,
                MaxY = TileMap.Height + CLAW_OFFSET_MAX_Y,
                Position = GRABBY_CLAW_START_POS,
                UpdatePriority = EntityUpdatePriority.Low
            };

            Entities.Add(GrabbyClaw);
            UI = new CollectionGameUI(this);

            _nineGridBackground = new NineGrid(TileMapTexture,16,new Point(160,0));

            StartGame(); //TODO: MAKE USER INITIATED THROUGH UI ?? (maybe)
        }

        private void UnloadGame() {
            Decorator?.Unload();
            Decorator = null;
        }

        private void DrawDebugText(DebugWriter writer) {
            writer.ToTopLeft();
            writer.Write(Camera.Position,"Camera Position");
            writer.Write(Camera.TileStart,"Tile Start");
            writer.Write(Camera.RenderOrigin,"Render Origin");
        }

        private SpriteBook _uiBook;
        public SpriteBook UI {
            get => _uiBook;
            set {
                if(value == _uiBook) {
                    return;
                }
                _uiBook?.UnbindInputEvents(this);
                _uiBook = value;
                value?.BindInputEvents(this);
            }
        }

        private void UpdateUI() {
            if(UI is null) {
                return;
            }
            UI.Update();
            CustomCursor.State = UI.CursorState;
        }

        public bool ShowingStartScreen { get; set; } = true;

        private string[] _textPages = new[] {
            "John has splintered into a endless stream of facsimiles, each but a sliver of his true form. Now, unsure of his individuality, John needs your help to regain his identity.",
            //todo
        };

        private int _textPageIndex = 0;

        private void AdvanceStartScreen() {
            if(++_textPageIndex < _textPages.Length) {
                return;
            }
            _textPageIndex--;
            ShowingStartScreen = false;
            Impulse.OnEvent -= Impulse_OnEvent;
            _startScreenEndStart = Now;
        }

        private TimeSpan _startScreenEndStart = TimeSpan.Zero;
        private static readonly TimeSpan _startScreenEndDuration = TimeSpan.FromSeconds(0.5f);

        private void DrawStartScreen() {
            float startScreenOffset = ShowingStartScreen ? 0 : MathF.Min((float)((Now - _startScreenEndStart) /  _startScreenEndDuration),1f);
            if(startScreenOffset >= 1) {
                return;
            }

            float margin = 0.25f;

            var screenSize = Viewport.Bounds.Size.ToVector2();

            var backgroundSize = screenSize - Camera.TileSize * margin;
            FloatRectangle backgroundArea = new(Camera.TileSize * margin * 0.5f,backgroundSize);

            float maxWidth = backgroundArea.Height * 0.8f;

            if(backgroundArea.Width > maxWidth) {
                var centerX = backgroundArea.CenterX;
                backgroundArea.Width = maxWidth;
                backgroundArea.Position = new Vector2(centerX-backgroundArea.Width*0.5f,backgroundArea.Position.Y);
            }

            Vector2 offset = new(0,(screenSize.Y - backgroundArea.Top) * startScreenOffset);

            backgroundArea.Position = Vector2.Floor(backgroundArea.Position + offset);

            SpriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            _nineGridBackground.Draw(SpriteBatch,Camera.Scale,backgroundArea);
            SpriteBatch.End();

            Fonts.Retro.Begin(SpriteBatch);

            Vector2 textMargin = new Vector2(Camera.Scale * 6);

            float textScale = Camera.Scale / 2;
            Color textColor = Color.White;

            Fonts.Retro.Draw(_textPages[_textPageIndex],backgroundArea.Position + textMargin,textScale,textColor,backgroundArea.Width - textMargin.X * 2);

            float y = MathF.Floor(backgroundArea.Bottom - textMargin.Y - textScale * Fonts.Retro.LineHeight * 0.5f);
            Fonts.Retro.DrawCentered("Press Any Key to Continue",new Vector2(MathF.Floor(backgroundArea.CenterX),y),textScale,textColor);

            Fonts.Retro.End();
        }

        private void RenderUI() {
            if(!ShowingStartScreen) {
                UI?.Render(SpriteBatch);
            }
            DrawStartScreen();
        }

        public JohnTypeMask RealJohnMask { get; private set; }
        public bool FindRealJohnMode { get; private set; } = true;

        private readonly Dictionary<int,JohnConfig> _configDictionary = new();

        private readonly List<List<int>> _maskTable = new();
        private readonly List<JohnTypeMask> _maskTypes = new();
        private int _maskTypeIndex = 0;

        private readonly JohnMatchType FirstMatchType = JohnMatchType.Pants;

        public void StartGame() {
            _configDictionary.Clear();
            Score = 0;
            Decorator.ResetConfigs();

            _maskTypes.Clear();
            _maskTable.Clear();

            for(int maskID = 0;maskID<MASK_TYPE_COUNT;maskID++) {
                _maskTypes.Add(new JohnTypeMask() {
                    Color = JohnConfig.GetRandomColor(Random),
                    ID = maskID,
                    Type = (JohnMatchType)(((int)FirstMatchType + maskID) % 3)
                });
                _maskTable.Add(new List<int>());
            }

            for(int configID = 0;configID<JOHN_CONFIG_COUNT;configID++) {
                var config = JohnConfig.CreateRandom(Random);
                var mask = _maskTypes[configID % _maskTypes.Count];
                config = config.ApplyMask(mask);
                Decorator.RegisterConfig(configID,config);
                _maskTable[mask.ID].Add(configID);
                _configDictionary[configID] = config;
            }

            _maskTypeIndex = 0;
            RealJohnMask = _maskTypes[_maskTypeIndex++];
            FindRealJohnMode = true;
            Decorator.GenerateTexture();
        }

        private void SelectNextJohnType() {
            RealJohnMask = _maskTypes[_maskTypeIndex];
            _maskTypeIndex = (_maskTypeIndex + 1) % _maskTypes.Count;
        }

        private readonly HashSet<WalkingJohn> _activeJohns = new();

        private TimeSpan _lastJohnSummonTime = -TimeSpan.FromHours(1);

        public bool GetRandomBool() {
            return Random.Next() > int.MaxValue / 2;
        }

        private float GetRealJohnProbability() {
            if(FindRealJohnMode) {
                return REAL_JOHN_PROBABILITY;
            } else {
                return KILL_IMPOSTER_JOHN_PROBABILITY;
            }
        }

        private void UpdateGame() {
            if(!(_activeJohns.Count < MAX_ARENA_JOHNS)) {
                return;
            }
            if(Now - _lastJohnSummonTime < JOHN_SUMMON_COOLDOWN) {
                return;
            }
            var startPosition = JOHN_SPAWN_LOCATIONS[Random.Next(0,JOHN_SPAWN_LOCATIONS.Length)];

            int configID;
            
            if(Random.NextSingle() < GetRealJohnProbability()) {
                var configList = _maskTable[RealJohnMask.ID];
                configID = configList[Random.Next(0,configList.Count)];
            } else {
                configID = Random.Next(0,JOHN_CONFIG_COUNT);
            }

            WalkingJohn john = JohnPool.LeaseJohn(configID,startPosition);
            _activeJohns.Add(john);
            _lastJohnSummonTime = Now + JOHN_SUMMON_VARIABILITY * Random.NextDouble();
        }

        public List<int> BinBuffer { get; private init; } = new();

        public bool JohnMatchesConfig(WalkingJohn john) {
            var config = _configDictionary[john.ConfigID];
            return RealJohnMask.Type switch {
                JohnMatchType.Hair => config.Color1,
                JohnMatchType.Shirt => config.Color2,
                JohnMatchType.Pants => config.Color3,
                _ => Color.Transparent
            } ==  RealJohnMask.Color;
        }

        private int _score = 0;
        public int Score {
            get => _score;
            set {
                if(_score == value) {
                    return;
                }
                if(value > _score) {
                    _score = value;
                    ScoreIncreased?.Invoke();
                } else {
                    _score = value;
                    ScoreDecreased?.Invoke();
                }
            }
        }

        public event Action JohnSaved, IncorrectJohnSaved, ImposterKilled, RealJohnKilled, WrongBin, ScoreIncreased, ScoreDecreased;

        public void ReturnJohn(WalkingJohn john,JohnReturnType johnReturnType) {
            if(johnReturnType == JohnReturnType.Default) {
                _activeJohns.Remove(john);
                JohnPool.ReturnJohn(john);
                return;
            }

            bool johnIsJohn = JohnMatchesConfig(john);

            if(johnReturnType == JohnReturnType.JohnBin) {
                if(FindRealJohnMode) {
                    if(johnIsJohn) {
                        Score++;
                        BinBuffer.Add(john.ConfigID);
                        JohnSaved?.Invoke();
                    } else {
                        Score--;
                        IncorrectJohnSaved?.Invoke();
                    }
                } else {
                    WrongBin?.Invoke();
                }
            } else {
                if(FindRealJohnMode) {
                    if(johnIsJohn) {
                        Score--;
                        RealJohnKilled?.Invoke();
                    } else {
                        WrongBin?.Invoke();
                    }
                } else {
                    Score++;
                    BinBuffer.Add(john.ConfigID);
                    ImposterKilled?.Invoke();
                }
            }

            if(BinBuffer.Count == ROUND_COMPLETION_COUNT) {
                BinBuffer.Clear();
                Score += 5;
                FindRealJohnMode = !FindRealJohnMode;
                if(FindRealJohnMode) {
                    SelectNextJohnType();
                }
            }

            _activeJohns.Remove(john);
            JohnPool.ReturnJohn(john);
        }
    }
}
