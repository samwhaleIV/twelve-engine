using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Dynamics;
using System;
using System.Collections.Generic;
using TwelveEngine.Game2D;
using TwelveEngine.Shell;
using TwelveEngine.Shell.UI;
using TwelveEngine.UI.Book;

namespace John {

    using static Constants;

    public sealed class JohnCollectionGame:GameState2D {

        public GrabbyClaw GrabbyClaw { get; private set; }
        public PoolOfJohns JohnPool { get; private init; }
        public JohnDecorator Decorator { get; private set; }

        public Random Random { get; private init; } = new Random(0);

        public JohnCollectionGame() {
            Name = "John Collection Game";
            OnLoad.Add(LoadGame);

            OnUpdate.Add(UpdateGame);

            OnUpdate.Add(CameraTracking);
            OnUnload.Add(UnloadGame);

            OnUpdate.Add(UpdateUI);
            OnRender.Add(RenderUI);

            PhysicsScale = PHYSICS_SIM_SCALE;
            JohnPool = new PoolOfJohns(this);
        }

        private void CameraTracking() {
            Camera.Position = GrabbyClaw.Position;
        }

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
                Priority = EntityPriority.Low
            };

            Entities.Add(GrabbyClaw);
            UI = new CollectionGameUI(this);

            StartGame(); //TODO: MAKE USER INITIATED THROUGH UI ?? (maybe)
        }

        public float GetUIScale() {
            return 10; //TODO: RETURN REALTIME VALUE
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

        private void RenderUI() => UI?.Render(SpriteBatch);

        public JohnTypeMask RealJohnMask { get; private set; }
        public bool FindRealJohnMode { get; private set; } = true;

        private readonly Dictionary<int,JohnConfig> _configDictionary = new();

        private readonly List<List<int>> _maskTable = new();
        private readonly List<JohnTypeMask> _maskTypes = new();

        public void StartGame() {
            _configDictionary.Clear();
            Score = 0;
            Decorator.ResetConfigs();

            _maskTypes.Clear();
            _maskTable.Clear();

            for(int maskID = 0;maskID<MASK_TYPE_COUNT;maskID++) {
                _maskTypes.Add(JohnTypeMask.GetRandom(Random,maskID));
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

            SelectRandomJohnType();
            FindRealJohnMode = true;
            Decorator.GenerateTexture();
        }

        private void SelectRandomJohnType() {
            RealJohnMask = _maskTypes[Random.Next(0,_maskTypes.Count)];
        }

        private HashSet<WalkingJohn> activeJohns = new();

        private TimeSpan _lastSummoning = -TimeSpan.FromHours(1);

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

        private bool TrySummonJohn() {
            if(Now - _lastSummoning < JOHN_SUMMON_COOLDOWN) {
                return false;
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
            activeJohns.Add(john);

            _lastSummoning = Now + JOHN_SUMMON_VARIABILITY * Random.NextDouble();

            return true;
        }

        public List<JohnConfig> BinBuffer { get; private init; } = new();

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
                activeJohns.Remove(john);
                JohnPool.ReturnJohn(john);
                return;
            }

            bool johnIsJohn = JohnMatchesConfig(john);

            if(johnReturnType == JohnReturnType.JohnBin) {
                if(FindRealJohnMode) {
                    if(johnIsJohn) {
                        Score++;
                        BinBuffer.Add(_configDictionary[john.ConfigID]);
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
                    BinBuffer.Add(_configDictionary[john.ConfigID]);
                    ImposterKilled?.Invoke();
                }
            }

            if(BinBuffer.Count == ROUND_COMPLETION_COUNT) {
                BinBuffer.Clear();
                Score += 5;
                FindRealJohnMode = !FindRealJohnMode;
                if(FindRealJohnMode) {
                    SelectRandomJohnType();
                }
            }

            activeJohns.Remove(john);
            JohnPool.ReturnJohn(john);
        }

        private void UpdateGame() {
            if(!(activeJohns.Count < MAX_ARENA_JOHNS)) {
                return;
            }
            TrySummonJohn();
        }
    }
}
