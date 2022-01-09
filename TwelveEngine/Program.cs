﻿using System.IO;
using Microsoft.Xna.Framework;
using TwelveEngine.UI;
using TwelveEngine.UI.Factory;
using TwelveEngine.Config;

namespace TwelveEngine {
    public static partial class Program {
        public static GameState GetUITestState() {
            return UIGameState.Create(UI => {
                var panel1 = DebugFactory.GetColoredScrollBox(UI,Color.Red,frame => {
                    frame.Width = 200;
                    frame.Height = 200;
                    frame.Positioning = Positioning.CenterParent;
                });

                var panel2 = DebugFactory.GetColoredFrame(UI,Color.Green,frame => {
                    frame.Width = 150;
                    frame.Height = 150;
                    frame.X = 25;
                    frame.Y = 25;
                });

                var panel3 = DebugFactory.GetColoredFrame(UI,Color.Blue,frame => {
                    frame.Width = 100;
                    frame.Height = 100;
                    frame.X = 25;
                    frame.Y = 25;
                });

                panel1.panel.IsInteractable = true;
                panel2.panel.IsInteractable = true;
                panel3.panel.IsInteractable = true;

                UI.AddChild(panel1.frame);

                panel1.panel.AddChild(panel2.frame);
                panel2.panel.AddChild(panel3.frame);
            });
        }
    }
}
