using System.Text;
using TwelveEngine;

namespace Elves.Scenes.ModeSelectMenu {
    public struct TerminalLine {

        public readonly int LeaseID;
        public readonly StringBuilder StringBuilder;
        public FloatRectangle Area;

        public TerminalLine(int leaseID,StringBuilder stringBuilder) {
            Area = FloatRectangle.Empty;
            LeaseID = leaseID;
            StringBuilder = stringBuilder;
        }
    }
}
