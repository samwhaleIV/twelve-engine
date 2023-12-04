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

        public bool GameActive { get; private set; }

        public JohnMatchType MatchType { get; private set; }
        public Color MatchColor { get; private set; }

        private readonly Dictionary<int,JohnConfig> _configDictionary = new();

        public void StartGame() {
            if(GameActive) {
                throw new InvalidOperationException("Game already active.");
            }
            _configDictionary.Clear();
            Score.Reset();
            Decorator.ResetConfigs();

            /* There is a very small possibility for a randomly generated john to surpass `REAL_JOHN_COUNT` */

            MatchColor = JohnConfig.GetRandomColor(Random);
            MatchType = (JohnMatchType)Random.Next(0,3);

            for(int i = 0;i<JOHN_CONFIG_COUNT;i++) {
                var config = JohnConfig.CreateRandom(Random);
                if(i < REAL_JOHN_COUNT) {
                    config = config.Mask(MatchType,MatchColor);
                }
                Decorator.AddConfig(i,config);
                _configDictionary[i] = config;
            }

            Decorator.GenerateTexture();
            GameActive = true;
        }

        private HashSet<WalkingJohn> activeJohns = new();

        private TimeSpan _lastSummoning = -TimeSpan.FromHours(1);

        public bool GetRandomBool() {
            return Random.Next() > int.MaxValue / 2;
        }

        private bool TrySummonJohn() {
            if(Now - _lastSummoning < JOHN_SUMMON_COOLDOWN) {
                return false;
            }

            var startPosition = JOHN_SPAWN_LOCATIONS[Random.Next(0,JOHN_SPAWN_LOCATIONS.Length)];

            int configID;
            
            if(Random.NextSingle() < REAL_JOHN_PROBABILITY) {
                configID = Random.Next(0,REAL_JOHN_COUNT);
            } else {
                configID = Random.Next(REAL_JOHN_COUNT,JOHN_CONFIG_COUNT);
            }

            WalkingJohn john = JohnPool.LeaseJohn(configID,startPosition);
            activeJohns.Add(john);

            _lastSummoning = Now + JOHN_SUMMON_VARIABILITY * Random.NextDouble();

            return true;
        }

        public ScoreCard Score { get; private init; } = new ScoreCard();

        public List<JohnConfig> CorrectJohnBinBuffer { get; private init; } = new();

        public bool JohnMatchesConfig(WalkingJohn john) {
            var config = _configDictionary[john.ConfigID];
            return MatchType switch {
                JohnMatchType.Hair => config.Color1, JohnMatchType.Shirt => config.Color2, JohnMatchType.Pants => config.Color3,
                _ => Color.Transparent
            } == MatchColor;
        }

        public void ReturnJohn(WalkingJohn john,JohnReturnType johnReturnType) {
            bool johnIsJohn = JohnMatchesConfig(john);

            if(johnReturnType == JohnReturnType.JohnBin) {
                if(johnIsJohn) {
                    Score.JohnInJohnBin++;
                } else {
                    Score.NotJohnInJohnBin++;
                }
                CorrectJohnBinBuffer.Add(_configDictionary[john.ConfigID]);
            } else if(johnReturnType == JohnReturnType.NotJohnBin) {
                if(johnIsJohn) {
                    Score.JohnInNotJohnBin++;
                } else {
                    Score.NotJohnInNotJohnBin++;
                }
            }

            activeJohns.Remove(john);
            JohnPool.ReturnJohn(john);
        }

        public Fixture TestPoint(Vector2 point) {
            return PhysicsWorld.TestPoint(point * PhysicsScale);
        }

        private void UpdateGame() {
            if(!GameActive) {
                return;
            }
            if(activeJohns.Count < MAX_ARENA_JOHNS) {
                TrySummonJohn();
            }
        }
    }
}
