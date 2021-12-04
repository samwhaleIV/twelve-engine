using System;
using System.Collections.Generic;
using TwelveEngine.Game2D;
using TwelveEngine.Game2D.Entities;

namespace TwelveEngine {
    internal static class DefaultEntities {

        private static List<(EntityType,Func<Entity>)> GetList() {
            return new List<(EntityType,Func<Entity> generator)>() {
                (EntityType.RedBox,()=>new RedBox()),
                (EntityType.Player,()=>new Player())
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
