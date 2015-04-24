namespace LiveSplit.OriAndTheBlindForest
{
    public class Vector2
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2(float x, float y) {
            this.X = x;
            this.Y = y;
        }
        public bool Within(float x, float y, float width, float height) {
            return X >= x && Y >= y && X <= x + width && Y <= y + height;
        }
        public override string ToString() {
            return string.Concat("(", X.ToString("0.000"), ", ", Y.ToString("0.000"), ")");
        }
    }
    public class Vector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3(float x, float y, float z) {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public bool Within(float x, float y, float z, float width, float height, float depth) {
            return X >= x && Y >= y && Z >= z && X <= x + width && Y <= y + height && Z <= z + depth;
        }
        public override string ToString() {
            return string.Concat("(", X.ToString("0.000"), ", ", Y.ToString("0.000"), ", ", Z.ToString("0.000"), ")");
        }
    }
    public class Vector4
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float W { get; set; }
        public float H { get; set; }

        public Vector4(float x, float y, float w, float h) {
            this.X = x;
            this.Y = y;
            this.W = w;
            this.H = h;
        }

        public override string ToString() {
            return string.Concat("(", X.ToString("0.000"), ", ", Y.ToString("0.000"), ", ", W.ToString("0.000"), ", ", H.ToString("0.000"), ")");
        }
    }
}