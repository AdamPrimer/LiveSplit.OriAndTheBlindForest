using System.Globalization;

namespace LiveSplit.OriAndTheBlindForest.State
{
    public enum Origin
    {
        Center,
        BottomLeft
    }

    public class Vector2
    {
        public float X { get; set; }
        public float Y { get; set; }
        public Origin origin { get; set; }

        public Vector2(float x, float y) {
            this.X = x;
            this.Y = y;
            this.origin = Origin.BottomLeft;
        }
        public Vector2(float x, float y, Origin origin) {
            this.X = x;
            this.Y = y;
            this.origin = origin;
        }

        public Vector2(string cordinates) {
            string[] cords = cordinates.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            if (cords.Length == 2) {
                float temp = 0;
                float.TryParse(cords[0], NumberStyles.Number, CultureInfo.GetCultureInfo("en-US"), out temp);
                this.X = temp;
                float.TryParse(cords[1], NumberStyles.Number, CultureInfo.GetCultureInfo("en-US"), out temp);
                this.Y = temp;
            }
        }
        public bool Within(float x, float y, float width, float height) {
            return X >= x && Y >= y && X <= x + width && Y <= y + height;
        }
        public bool Within(Vector4 pos) {
            return X >= pos.X && Y >= pos.Y && X <= pos.X + pos.W && Y <= pos.Y + pos.H;
        }
        public override string ToString() {
            return string.Concat(X.ToString("0.000", CultureInfo.CreateSpecificCulture("en-US")), ", ", Y.ToString("0.000", CultureInfo.CreateSpecificCulture("en-US")));
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
        public Vector3(string cordinates) {
            string[] cords = cordinates.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            if (cords.Length == 3) {
                float temp = 0;
                float.TryParse(cords[0], NumberStyles.Number, CultureInfo.GetCultureInfo("en-US"), out temp);
                this.X = temp;
                float.TryParse(cords[1], NumberStyles.Number, CultureInfo.GetCultureInfo("en-US"), out temp);
                this.Y = temp;
                float.TryParse(cords[2], NumberStyles.Number, CultureInfo.GetCultureInfo("en-US"), out temp);
                this.Z = temp;
            }
        }
        public bool Within(float x, float y, float z, float width, float height, float depth) {
            return X >= x && Y >= y && Z >= z && X <= x + width && Y <= y + height && Z <= z + depth;
        }
        public override string ToString() {
            return string.Concat(X.ToString("0.000", CultureInfo.CreateSpecificCulture("en-US")), ", ", Y.ToString("0.000", CultureInfo.CreateSpecificCulture("en-US")), ", ", Z.ToString("0.000", CultureInfo.CreateSpecificCulture("en-US")));
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
        public Vector4(string cordinates) {
            string[] cords = cordinates.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            if (cords.Length == 4) {
                float temp = 0;
                float.TryParse(cords[0], NumberStyles.Number, CultureInfo.GetCultureInfo("en-US"), out temp);
                this.X = temp;
                float.TryParse(cords[1], NumberStyles.Number, CultureInfo.GetCultureInfo("en-US"), out temp);
                this.Y = temp;
                float.TryParse(cords[2], NumberStyles.Number, CultureInfo.GetCultureInfo("en-US"), out temp);
                this.W = temp;
                float.TryParse(cords[3], NumberStyles.Number, CultureInfo.GetCultureInfo("en-US"), out temp);
                this.H = temp;
            } else {
                this.X = 0;
                this.Y = 0;
                this.W = 0;
                this.H = 0;
            }
        }
        public Vector4(Vector2 pos, float w, float h) {
            if (pos.origin == Origin.Center) {
                this.X = pos.X - (w / 2);
                this.Y = pos.Y + (h / 2);
            } else {
                this.X = pos.X;
                this.Y = pos.Y;
            }

            this.W = w;
            this.H = h;
        }

        public Vector2 GetCenter() {
            return new Vector2(X + (W / 2), Y - (H / 2), Origin.Center);
        }

        public bool Intersects(Vector4 other) {
            return X + W >= other.X && other.X + other.W >= X && Y - H <= other.Y && other.Y - other.H <= Y;
        }

        public override string ToString() {
            return string.Concat(X.ToString("0.000", CultureInfo.CreateSpecificCulture("en-US")), ", ", Y.ToString("0.000", CultureInfo.CreateSpecificCulture("en-US")), ", ", W.ToString("0.000", CultureInfo.CreateSpecificCulture("en-US")), ", ", H.ToString("0.000", CultureInfo.CreateSpecificCulture("en-US")));
        }
    }
}