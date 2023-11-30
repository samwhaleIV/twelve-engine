namespace TwelveEngine.Game2D {
    internal sealed class TileEdgeDetector {

        private const byte OPEN = 0, SOLID = 1;

        private readonly byte[,] _collisionMap;

        [Flags]
        private enum EdgeState {
            None = 0,
            Left = 1,
            Right = 2,
            Up = 4,
            Down = 8
        };

        private readonly int _width, _height;

        public TileEdgeDetector(int width,int height) {
            _width = width; _height = height;

            _collisionMap = new byte[width,height];

            for(int x = 0;x<width;x++) {
                for(int y = 0;y<height;y++) {
                    _collisionMap[x,y] = OPEN;
                }
            }
        }

        public void AddCollision(int x,int y) {
            _collisionMap[x,y] = SOLID;
        }

        private bool IsSolidTile(int x,int y) {
            if(x < 0 || y < 0 || x >= _width || y >= _height) {
                return true;
            }
            return _collisionMap[x,y] == SOLID;
        }

        private EdgeState GetSurroundingState(Point tile) {
            EdgeState surroundingState = EdgeState.None;
            if(IsSolidTile(tile.X-1,tile.Y)) {
                surroundingState |= EdgeState.Left;
            }
            if(IsSolidTile(tile.X+1,tile.Y)) {
                surroundingState |= EdgeState.Right;
            }
            if(IsSolidTile(tile.X,tile.Y-1)) {
                surroundingState |= EdgeState.Up;
            }
            if(IsSolidTile(tile.X,tile.Y+1)) {
                surroundingState |= EdgeState.Down;
            }
            return surroundingState;
        }

        private Dictionary<Point,EdgeState> GetOpenTileSet() {

            Dictionary<Point,EdgeState> openTiles = new();

            for(int x = 0;x<_width;x++) {
                for(int y = 0;y<_height;y++) {
                    if(_collisionMap[x,y] != OPEN) {
                        continue;
                    }
                    Point tile = new Point(x,y);
                    EdgeState surroundingState = GetSurroundingState(tile);
                    if(surroundingState == EdgeState.None) {
                        continue;
                    }
                    openTiles.Add(tile,surroundingState);
                }
            }

            return openTiles;
        }


        public IEnumerable<(Vector2,Vector2)> CreateEdges() {

            Dictionary<Point,EdgeState> openTileSet = GetOpenTileSet();

            (Point,Point) ReadEdge(Point start,Point scan,EdgeState edgeType) {
                openTileSet[start] = openTileSet[start] & edgeType;
                Point head = start + scan;

                while(openTileSet.TryGetValue(head,out EdgeState headType) && (headType & edgeType) != 0) {
                    openTileSet[head] = openTileSet[head] & edgeType;
                    head += scan;
                }

                return (start, head - scan);
            }

            foreach(var (tile,edgeState) in openTileSet) {
                if((edgeState & EdgeState.Left) != 0) {
                    var edge = ReadEdge(tile,new Point(0,1),EdgeState.Left);
                    yield return (edge.Item1.ToVector2(), edge.Item2.ToVector2() + new Vector2(0,1));
                }
                if((edgeState & EdgeState.Right) != 0) {
                    var edge = ReadEdge(tile,new Point(0,1),EdgeState.Right);
                    yield return (edge.Item1.ToVector2() + new Vector2(1,0), edge.Item2.ToVector2() + new Vector2(1,1));
                }
                if((edgeState & EdgeState.Up) != 0) {
                    var edge = ReadEdge(tile,new Point(1,0),EdgeState.Up);
                    yield return (edge.Item1.ToVector2(), edge.Item2.ToVector2() + new Vector2(1,0));
                }
                if((edgeState & EdgeState.Down) != 0) {
                    var edge = ReadEdge(tile,new Point(1,0),EdgeState.Down);
                    yield return (edge.Item1.ToVector2() + new Vector2(0,1), edge.Item2.ToVector2() + new Vector2(1,1));
                }
            }
        }
    }
}
