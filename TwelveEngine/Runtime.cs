using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace TwelveEngine {
    public class RuntimeStartTarget:TestGame {
        public RuntimeStartTarget() {
            Debug.WriteLine("Initiating runtime start target...");
        }
    }
    public static class Runtime {
        public static readonly string TestString = "Hello, world!";
    }
}
