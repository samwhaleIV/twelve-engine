using ElfGame.ElfSprite.Face.Controller;
using TwelveEngine.Shell.States;

namespace ElfGame.ElfSprite.Face {
    internal class ElfFace {

        public ElfFace() => State = new FaceState();

        public ElfFace(FaceState state) => State = state;

        private readonly Eyes eyes = new Eyes();
        private readonly Mouth mouth = new Mouth();

        private readonly FaceRenderer renderer = new FaceRenderer();

        public FaceRenderer Renderer => renderer;

        public Eyes Eyes => eyes;
        public Mouth Mouth => mouth;

        private FaceState _state;
        public FaceState State {
            get => _state;
            set {
                _state = value;
                eyes.State = value;
                mouth.State = value;
                Renderer.State = value;
            }
        }

        private InputGameState _gameState;
        public InputGameState GameState {
            get => _gameState;
            set {
                _gameState = value;
                eyes.GameState = value;
                mouth.GameState = value;
            }
        }
    }
}
