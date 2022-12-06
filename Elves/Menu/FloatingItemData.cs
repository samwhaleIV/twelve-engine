using Microsoft.Xna.Framework;

namespace Elves.Menu {
    public static class FloatingItemData {

        private const int BUBBLE_GRAB_WEIGHT = 10;
        private const int ELF_GRAB_WEIGHT = 1;

        private static (Rectangle TextureSource, int Weight) GetElfItem(int x,int y,int width,int height) {
            return (new Rectangle(x,y,width,height), ELF_GRAB_WEIGHT);
        }

        public static GrabBag<Rectangle> GetGrabBag() => new GrabBag<Rectangle>(new[]{
            (new Rectangle(38,78,4,4),BUBBLE_GRAB_WEIGHT),
            GetElfItem(0,31,7,23),
            GetElfItem(7,31,7,23),
            GetElfItem(14,31,7,23),
            GetElfItem(21,32,9,23),
            GetElfItem(30,31,9,23),
            GetElfItem(39,31,9,23),
            GetElfItem(48,33,9,21),
            GetElfItem(57,34,9,20),
            GetElfItem(66,39,9,15),
            GetElfItem(75,31,9,23),
            GetElfItem(84,31,9,23),
            GetElfItem(93,48,9,16),
            GetElfItem(0,54,11,24),
            GetElfItem(11,55,9,23),
            GetElfItem(20,55,9,23),
            GetElfItem(29,55,9,23),
            GetElfItem(38,55,9,23),
            GetElfItem(47,55,7,23),
            GetElfItem(54,55,9,23),
            GetElfItem(63,55,9,23),
            GetElfItem(72,55,9,23),
            GetElfItem(81,55,11,22),
            GetElfItem(92,55,9,23),
            GetElfItem(101,55,9,23),
            GetElfItem(110,55,9,23),
            GetElfItem(102,32,7,23),
        });


    }
}
