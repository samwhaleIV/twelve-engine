using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace TwelveEngine {
    /* Change the start target of the game by changing the subclass */
    public class RuntimeStartTarget:TestGame {
        public RuntimeStartTarget() {
            Runtime.Log("Initiating runtime start target...");
        }
    }
    public static class Runtime {
        public static readonly string TestString = "Hello, world!";
        public static void Log(string message) {
            Debug.WriteLine(message);
        }
    }
}
