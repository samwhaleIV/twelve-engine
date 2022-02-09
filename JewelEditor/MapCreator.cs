using TwelveEngine;
using TwelveEngine.Game2D;
using TwelveEngine.Serial.Map;
using Microsoft.Xna.Framework;

namespace JewelEditor {
    public static class MapCreator {

        public const string TilesetPath = "JewelEditor/MapTiles";

        private const int DefaultSize = 32;

        public static GameState Create() => Create(DefaultSize,DefaultSize);

        public static GameState Create(int width,int height) {
            var grid2D = new Grid2D(
                tileSize: 16,
                layerMode: LayerModes.SingleLayerBackground,
                collisionTypes: null,
                entityFactory: JewelEntities.GetFactory()
            ) {
                TileRenderer = new TilesetRenderer(TilesetPath),
                BackgroundColor = Color.LightGray
            };

            var layerSize = width * height;
            var layers = new int[1][];
            var baseLayer = new int[layerSize];
            for(int i = 0;i<layerSize;i++) {
                baseLayer[i] = (int)MapValue.None;
            }
            layers[0] = baseLayer;

            var defaultMap = new Map(width,height,layers);
            grid2D.ImportMap(defaultMap);

            grid2D.OnLoad += () => {
                var entities = grid2D.Entities;
                entities.Create(JewelEntities.MouseReceiver);
                entities.Create(JewelEntities.GridLines);
                entities.Create(JewelEntities.EntityMarker);
                entities.Create(JewelEntities.UIEntity,"UIEntity");
            };

            return grid2D;
        }
    }
}
