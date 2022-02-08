using TwelveEngine;
using TwelveEngine.EntitySystem;
using TwelveEngine.Game2D;
using TwelveEngine.Serial.Map;

namespace JewelEditor {
    public static class MapCreator {

        private const int DefaultSize = 32;

        private static EntityFactory<Entity2D,Grid2D> GetEntityFactory() => new EntityFactory<Entity2D,Grid2D>(
            ((int)EntityTypes.MapEntity, () => new MapEntity()),
            ((int)EntityTypes.GridLines, () => new GridLines())
        );

        public static GameState Create() => Create(DefaultSize,DefaultSize);

        public static GameState Create(int width,int height) {
            var grid2D = new Grid2D(
                tileSize: 16,
                layerMode: LayerModes.SingleLayerBackground,
                collisionTypes: null,
                entityFactory: GetEntityFactory()
            ) {
                TileRenderer = new TilesetRenderer("JewelEditor/MapTiles")
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
                grid2D.Entities.Create((int)EntityTypes.GridLines);
            };

            return grid2D;
        }
    }
}
