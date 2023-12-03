using John.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TwelveEngine.Game2D;
using TwelveEngine.Shell;
using TwelveEngine.Shell.UI;
using TwelveEngine.UI.Book;

namespace John {

    using static Constants;

    public sealed class JohnCollectionGame:GameState2D {

        private GrabbyClaw grabbyClaw;
        private readonly PoolOfJohns _johnPool;
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
            _johnPool = new PoolOfJohns(this);
        }

        private void CameraTracking() {
            Camera.Position = grabbyClaw.Position;
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

            grabbyClaw = new GrabbyClaw() {
                MinX = CLAW_OFFSET_MIN_X,
                MaxX = TileMap.Width + CLAW_OFFSET_MAX_X,
                MinY = CLAW_OFFSET_MIN_Y,
                MaxY = TileMap.Height + CLAW_OFFSET_MAX_Y,
                Position = GRABBY_CLAW_START_POS
            };

            Entities.Add(grabbyClaw);
            UI = new CollectionGameUI(TileMapTexture,this);

            StartGame(); //TODO: MAKE USER INITIATED THROUGH UI
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

        public TimeSpan GameStart { get; private set; }
        public TimeSpan RemainingTime => JOHN_GAME_DURATION - GameStart;

        public bool GameActive { get; private set; }

        public int RealJohnID { get; private set; }

        public void StartGame() {
            if(GameActive) {
                throw new InvalidOperationException("Game already active.");
            }
            Score.Reset();
            Decorator.ResetConfigs();
            for(int i = 0;i<JOHN_CONFIG_COUNT;i++) {
                Decorator.AddConfig(i,JohnConfig.CreateRandom(Random));
            }
            Decorator.GenerateTexture();

            GameStart = RealTime;
            GameActive = true;

            RealJohnID = 0;
        }

        private void EndGame() {
            if(!GameActive) {
                throw new InvalidOperationException("Game already inactive.");
            }

            //TODO: User interface prompts

            GameActive = false;
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
                configID = RealJohnID;
            } else {
                /* Warning: Assumption that RealJohnID is always zero so we don't include it in random chance or spike the odds of another value */
                configID = Random.Next(1,JOHN_CONFIG_COUNT);
            }

            WalkingJohn john = _johnPool.LeaseJohn(configID,startPosition);
            activeJohns.Add(john);

            _lastSummoning = Now + JOHN_SUMMON_VARIABILITY * Random.NextDouble();

            return true;
        }

        public ScoreCard Score { get; private init; } = new ScoreCard();

        public void ReturnJohn(WalkingJohn john,JohnReturnType johnReturnType) {
            bool johnIsJohn = john.ConfigID == RealJohnID;

            if(johnReturnType == JohnReturnType.JohnBin) {
                if(johnIsJohn) {
                    Score.JohnInJohnBin++;
                } else {
                    Score.NotJohnInJohnBin++;
                }
            } else if(johnReturnType == JohnReturnType.NotJohnBin) {
                if(johnIsJohn) {
                    Score.JohnInNotJohnBin++;
                } else {
                    Score.NotJohnInNotJohnBin++;
                }
            }

            activeJohns.Remove(john);
            _johnPool.ReturnJohn(john);
        }

        private void UpdateGame() {
            if(!GameActive) {
                return;
            }
            if(RemainingTime < TimeSpan.Zero) {
                EndGame();
                return;
            }
            if(activeJohns.Count < MAX_ARENA_JOHNS) {
                TrySummonJohn();
            }
        }
    }
}
