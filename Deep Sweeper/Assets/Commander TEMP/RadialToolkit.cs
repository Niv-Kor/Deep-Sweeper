using System.Collections.Generic;

namespace DeepSweeper.UI.Ingame.Spatials.Commander
{
    public static class RadialToolkit
    {
        public enum RadialDivision
        {
            None = 0,
            Single = 1,
            Double = 2,
            Triple = 3,
            Quadruple = 4,
        }

        public enum Segment
        {
            Sector0_180,
            Sector180_360,
            Sector0_120,
            Sector120_240,
            Sector240_360,
            Sector0_90,
            Sector90_180,
            Sector180_270,
            Sector270_360
        }

        /// <param name="amount">Amount of characters that the circle should contain.</param>
        /// <returns>
        /// the correct radial division type for a given amount of characters.
        /// If the amount of characters is not between 1-4 (inclusive),
        /// this method would return the type 'None'.
        /// </returns>
        public static RadialDivision Divide(int amount) {
            if (amount > 4 || amount < 0) return RadialDivision.None;
            else return (RadialDivision) amount;
        }

        public static int AsAmount(this RadialDivision division) {
            return (int) division;
        }

        public static RadialDivision Originate(Segment segment) {
            switch (segment) {
                case Segment.Sector0_180:
                case Segment.Sector180_360: return RadialDivision.Double;
                case Segment.Sector0_120:
                case Segment.Sector120_240:
                case Segment.Sector240_360: return RadialDivision.Triple;
                case Segment.Sector0_90:
                case Segment.Sector90_180:
                case Segment.Sector180_270:
                case Segment.Sector270_360: return RadialDivision.Quadruple;
                default: return RadialDivision.None;

            }
        }

        public static List<Segment> AsSegments(this RadialDivision division) {
            List<Segment> list = new List<Segment>();

            switch (division) {
                case RadialDivision.Double:
                    list.Add(Segment.Sector0_180);
                    list.Add(Segment.Sector180_360);
                    break;

                case RadialDivision.Triple:
                    list.Add(Segment.Sector0_120);
                    list.Add(Segment.Sector120_240);
                    list.Add(Segment.Sector240_360);
                    break;

                case RadialDivision.Quadruple:
                    list.Add(Segment.Sector0_90);
                    list.Add(Segment.Sector90_180);
                    list.Add(Segment.Sector180_270);
                    list.Add(Segment.Sector270_360);
                    break;
            }

            return list;
        }
    }
}