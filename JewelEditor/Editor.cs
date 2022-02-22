using TwelveEngine.Game2D;
using TwelveEngine.Serial.Map;
using Microsoft.Xna.Framework;

namespace JewelEditor {
    public sealed class Editor:Grid2D {

        private const int DefaultSize = 16;
        public const string Tileset = "JewelEditor/MapTiles";
        public const string State = "State";
        public const string UI = "UI";
        public const int ButtonsPanelHeight = 96;
        public const int TileLayer = 0;
        public const int EraserIndex = 3; /* See below */

        internal static readonly ButtonData[] Buttons = new ButtonData[] {
            new ButtonData(new Rectangle(16,48,16,16), InputMode.Tile, TileType.Floor),
            new ButtonData(new Rectangle(32,48,16,16), InputMode.Tile, TileType.Wall),
            new ButtonData(new Rectangle(48,48,16,16), InputMode.Tile, TileType.Door),
            new ButtonData(new Rectangle(0,48,16,16), InputMode.Tile, TileType.None), /* <--  EraserIndex */
            new ButtonData(new Rectangle(16,32,16,16), InputMode.Entity)
        };

        public Editor() {
            TileSize = 16;
            EntityFactory = JewelEntities.GetFactory();
            LayerMode = LayerModes.SingleLayerBackground;
            TileRenderer = new TilesetRenderer(Tileset);
            RenderBackground = gameTime => Game.GraphicsDevice.Clear(Color.LightGray);

            int width = DefaultSize, height = DefaultSize;

            var layerSize = width * height;
            var layers = new int[1][];
            var baseLayer = new int[layerSize];
            for(int i = 0;i<layerSize;i++) {
                baseLayer[i] = (int)TileType.None;
            }
            layers[0] = baseLayer;

            var defaultMap = new Map(width,height,layers);
            ImportMap(defaultMap);

            Camera.Position = new Point(width,height).ToVector2() / 2f;
            Camera.HorizontalPadding = false;
            Camera.VerticalPadding = false;

            OnLoad += () => {
                Entities.Create(JewelEntities.StateEntity,State);
                Entities.Create(JewelEntities.InputEntity);
                Entities.Create(JewelEntities.GridLines);
                Entities.Create(JewelEntities.EntityMarker,"DebugMarker");
                Entities.Create(JewelEntities.UIEntity,UI);
            };
        }
    }
}
