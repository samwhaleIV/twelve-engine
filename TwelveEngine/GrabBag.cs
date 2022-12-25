using System;

namespace TwelveEngine {
    public sealed class GrabBag<T> {

        private readonly T[] grabBag;

        public GrabBag(params (T Value,int Weight)[] set) {
            int total = 0;
            int i;
            for(i = 0;i < set.Length;i++) {
                total += set[i].Weight;
            }
            grabBag = new T[total];
            for(i = 0;i < set.Length;i++) {
                (T Value, int Weight) = set[i];
                int count = Weight;
                while(count > 0) {
                    count--;
                    grabBag[i++] = Value;
                }
            }
        }

        public T GetRandom(Random random) {
            int index = random.Next(0,grabBag.Length);
            return grabBag[index];
        }
    }
}
