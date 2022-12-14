using System;

namespace TwelveEngine {
    public sealed class GrabBag<T> {

        private readonly T[] grabBag;

        public GrabBag((T Value,int Weight)[] set) {
            int total = 0;
            foreach(var item in set) {
                total += item.Weight;
            }
            grabBag = new T[total];
            int i = 0;
            foreach(var item in set) {
                int count = item.Weight;
                while(count > 0) {
                    count--;
                    grabBag[i++] = item.Value;
                }
            }
        }

        public T GetRandom(Random random) {
            int index = random.Next(0,grabBag.Length);
            return grabBag[index];
        }
    }
}
