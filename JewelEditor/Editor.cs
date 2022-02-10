using TwelveEngine.Game2D;
using TwelveEngine.Serial.Map;
using Microsoft.Xna.Framework;

namespace JewelEditor {
    public sealed class Editor:Grid2D {

        private const int DefaultSize = 16;

        public const string Tileset = "JewelEditor/MapTiles";

        public Editor() : base(
            tileSize: 16,
            layerMode: LayerModes.SingleLayerBackground,
            collisionTypes: null,
            entityFactory: JewelEntities.GetFactory()
        ) {
            TileRenderer = new TilesetRenderer(Tileset);
            BackgroundColor = Color.LightGray;

            int width = DefaultSize, height = DefaultSize;

            var layerSize = width * height;
            var layers = new int[1][];
            var baseLayer = new int[layerSize];
            for(int i = 0;i<layerSize;i++) {
                baseLayer[i] = (int)MapValue.None;
            }
            layers[0] = baseLayer;

            var defaultMap = new Map(width,height,layers);
            ImportMap(defaultMap);

            OnLoad += () => {
                Entities.Create(JewelEntities.InputEntity);
                Entities.Create(JewelEntities.GridLines);
                Entities.Create(JewelEntities.EntityMarker);
                Entities.Create(JewelEntities.UIEntity,"UIEntity");
            };
        }
    }
}
