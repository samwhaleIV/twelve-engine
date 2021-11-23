using System;
using System.Collections.Generic;
using TwelveEngine.Game2D;
using TwelveEngine.Game2D.Entities;

namespace TwelveEngine {
    internal static class DefaultEntitiesList {

        private static List<(string, Func<Entity>)> GetList() {
            return new List<(string, Func<Entity> generator)>() {

                ("TheRedBox",()=>new TheRedBox()),
                ("Player",()=>new Player())

            };
        }

        internal static void Install() {
            var typeList = GetList();
            foreach(var (factoryID,generator) in typeList) {
                EntityFactory.SetType(factoryID,generator);
            }
        }
    }
}
