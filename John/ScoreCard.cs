namespace John {
    public class ScoreCard {
        public int JohnInJohnBin { get; set; } = 0;
        public int JohnInNotJohnBin { get; set; } = 0;

        public int NotJohnInJohnBin { get; set; } = 0;
        public int NotJohnInNotJohnBin { get; set; } = 0;

        public void Reset() {
            JohnInJohnBin = 0;
            JohnInNotJohnBin = 0;
            NotJohnInJohnBin = 0;
            JohnInNotJohnBin = 0;
        }
    }
}
