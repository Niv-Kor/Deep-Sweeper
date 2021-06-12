using System;
using System.Collections.Generic;
using UnityEngine;

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
            Sector0_0,
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

        public static RadialDivision Originate(this Segment segment) {
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

        public static Vector2 AsCoordinates(this Segment segment) {
            switch (segment) {
                case Segment.Sector0_180: return Vector2.right;
                case Segment.Sector180_360: return Vector2.left;
                case Segment.Sector0_120: return Vector2.right * .9f + Vector2.up * .5f;
                case Segment.Sector120_240: return Vector2.down;
                case Segment.Sector240_360: return Vector2.left * .9f + Vector2.up * .5f;
                case Segment.Sector0_90: return Vector2.right + Vector2.up;
                case Segment.Sector90_180: return Vector2.right + Vector2.down;
                case Segment.Sector180_270: return Vector2.left + Vector2.down;
                case Segment.Sector270_360: return Vector2.left + Vector2.up;
                default: return Vector2.zero;
            }
        }

        public static Segment ToSegment(this RadialDivision division, float angle) {
            angle = 360 - (angle % 360);

            switch (division) {
                case RadialDivision.Double:
                    if (angle >= 0 && angle <= 180) return Segment.Sector0_180;
                    else return Segment.Sector180_360;

                case RadialDivision.Triple:
                    if (angle >= 0 && angle <= 120) return Segment.Sector0_120;
                    else if (angle > 120 && angle <= 240) return Segment.Sector120_240;
                    else return Segment.Sector240_360;

                case RadialDivision.Quadruple:
                    if (angle >= 0 && angle <= 90) return Segment.Sector0_90;
                    else if (angle > 90 && angle <= 180) return Segment.Sector90_180;
                    else if (angle > 180 && angle <= 270) return Segment.Sector180_270;
                    else return Segment.Sector270_360;

                default: return Segment.Sector0_0;
            }
        }
    }
}