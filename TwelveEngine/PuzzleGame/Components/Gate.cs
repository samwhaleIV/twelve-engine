namespace TwelveEngine.PuzzleGame.Components {
    public abstract class Gate:Component {
        public Component SecondInput { get; set; } = null;
        public abstract override void UpdateSignal();
        public Gate SetInputs(Component input1,Component input2) {
            input1.Outputs.Add(this);
            input2.Outputs.Add(this);
            Input = input1;
            SecondInput = input2;
            return this;
        }
    }
}
