namespace TwelveEngine.TileGen {
    internal readonly struct MatrixMode {
        public MatrixMode(int size,Mirroring mirroring,Rotation rotation) {
            Size = size;
            Mirroring = mirroring;
            Rotation = rotation;
        }

        public readonly int Size;
        public readonly Mirroring Mirroring;
        public readonly Rotation Rotation;
    }
}
