using System;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Serial;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Dynamics;

namespace TwelveEngine.Game2D.Objects {
    public class ObjectManager {

        private readonly PhysicsGrid2D _grid;

        public PhysicsGrid2D Grid => _grid;

        public ObjectManager(PhysicsGrid2D grid) {
            _grid = grid;
        }

        private readonly Dictionary<int,GameObject> objects = new Dictionary<int,GameObject>();

        private int id = 0;

        public void DeleteAll() {
            foreach(GameObject gameObject in GetEnumerable()) {
                if(gameObject.IsLoaded) {
                    Delete(gameObject);
                }
            }
            objects.Clear();
        }

        public void Delete(GameObject gameObject) {
            if(gameObject.Owner != this) {
                throw new InvalidOperationException("Cannot delete object, objects belongs to another ObjectManager!");
            }
            if(!objects.ContainsKey(gameObject.ID)) {
                throw new InvalidOperationException($"Cannot delete object with ID {gameObject.ID} because it is not in the objects dictionary!");
            }
            objects.Remove(gameObject.ID);
            gameObject.FireUnloadEvent();
        }

        internal void Load() {
            foreach(GameObject gameObject in objects.Values) {
                gameObject.FireLoadEvent();
            }
        }

        internal void Unload() {
            foreach(GameObject gameObject in objects.Values) {
                gameObject.FireUnloadEvent();
            }
        }

        private int GetNewObjectID() {
            return id++;
        }

        private void AddObject(GameObject gameObject) {
            objects.Add(gameObject.ID,gameObject);
            if(Grid.IsLoaded) {
                gameObject.FireLoadEvent();
            }
        }

        public GameObject CreateNoCollide() {
            GameObject gameObject = new GameObject(this,GetNewObjectID());
            AddObject(gameObject);
            return gameObject;
        }

        public PhysicsGameObject CreateStatic() {
            PhysicsGameObject gameObject = new PhysicsGameObject(this,GetNewObjectID(),BodyType.Static);
            AddObject(gameObject);
            return gameObject;
        }

        public PhysicsGameObject CreateDynamic() {
            PhysicsGameObject gameObject = new PhysicsGameObject(this,GetNewObjectID(),BodyType.Dynamic);
            AddObject(gameObject);
            return gameObject;
        }

        public PhysicsGameObject CreateKinematic() {
            PhysicsGameObject gameObject = new PhysicsGameObject(this,GetNewObjectID(),BodyType.Kinematic);
            AddObject(gameObject);
            return gameObject;
        }

        public IEnumerable<GameObject> GetEnumerable() => objects.Values;
    }
}
