using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utiliities
{
    public struct Angle : IEquatable<Angle>, IComparable<Angle>
    {
        private double radians;

        private const double Pi = Math.PI;
        private const double PiOver2 = Math.PI / 2;
        private const double PiOver4 = Math.PI / 4;
        private const double TwoPi = Math.PI * 2;
        private const double RadToDeg = 180 / Math.PI;

        private static double ToDegrees(double radians)
        {
            return radians * RadToDeg;
        }
        private static double ToRadians(double degrees)
        {
            return degrees / RadToDeg;
        }

        public double Radians
        {
            get
            {
                return radians;
            }
            set
            {
                radians = value;
            }
        }
        public double Degrees
        {
            get
            {
                return ToDegrees(radians);
            }
            set
            {
                radians = ToRadians(value);
            }
        }

        private Angle(double radians)
        {
            this.radians = radians;
        }

        public static Angle FromRadians(double radians)
        {
            return new Angle(radians);
        }
        public static Angle FromDegrees(double degrees)
        {
            return new Angle(ToRadians(degrees));
        }

        public static Angle Zero
        {
            get { return new Angle(0); }
        }
        public static Angle QuarterCircle
        {
            get { return new Angle(PiOver2); }
        }
        public static Angle HalfCircle
        {
            get { return new Angle(Pi); }
        }
        public static Angle FullCircle
        {
            get { return new Angle(TwoPi); }
        }

        public Angle DistanceTo(Angle angle)
        {
            return Abs(DirectionalDistanceTo(angle));
        }

        public Angle DirectionalDistanceTo(Angle angle)
        {
            double simpleDistance = angle.radians - this.radians;
            return new Angle((simpleDistance + Pi).PMod(TwoPi) - Pi);
        }

        /// <summary>
        /// Normalizes this angle between -π and +π (-180° and 180°).
        /// </summary>
        public void Normalize()
        {
            this = -DirectionalDistanceTo(Angle.Zero);
        }

        public int CompareTo(Angle angle)
        {
            return this.radians.CompareTo(angle.radians);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is Angle)
            {
                return ((Angle)obj) == this;
            }
            else
            {
                return false;
            }
        }
        public bool Equals(Angle angle)
        {
            return this == angle;
        }

        public override int GetHashCode()
        {
            return radians.GetHashCode();
        }

        public override string ToString()
        {
            return Degrees + "°";
        }

        #region Operator Overloads
        public static Angle operator -(Angle angle)
        {
            return new Angle(-angle.radians);
        }
        public static Angle operator +(Angle angle)
        {
            return new Angle(+angle.radians);
        }
        public static Angle operator +(Angle angle1, Angle angle2)
        {
            return new Angle(angle1.radians + angle2.radians);
        }
        public static Angle operator -(Angle angle1, Angle angle2)
        {
            return new Angle(angle1.radians - angle2.radians);
        }
        public static Angle operator *(Angle angle, double scale)
        {
            return new Angle(angle.radians * scale);
        }
        public static Angle operator /(Angle angle, double scale)
        {
            return new Angle(angle.radians / scale);
        }
        public static Angle operator %(Angle angle1, Angle angle2)
        {
            return new Angle(angle1.radians % angle2.radians);
        }
        public static bool operator ==(Angle angle1, Angle angle2)
        {
            return angle1.radians == angle2.radians;
        }
        public static bool operator !=(Angle angle1, Angle angle2)
        {
            return angle1.radians != angle2.radians;
        }
        public static bool operator <(Angle angle1, Angle angle2)
        {
            return angle1.radians < angle2.radians;
        }
        public static bool operator >(Angle angle1, Angle angle2)
        {
            return angle1.radians > angle2.radians;
        }
        public static bool operator <=(Angle angle1, Angle angle2)
        {
            return angle1.radians <= angle2.radians;
        }
        public static bool operator >=(Angle angle1, Angle angle2)
        {
            return angle1.radians >= angle2.radians;
        }
        #endregion

        #region System.Math replacements
        public static Angle Abs(Angle angle)
        {
            return new Angle(Math.Abs(angle.radians));
        }
        public static float Cos(Angle angle)
        {
            return (float)Math.Cos(angle.radians);
        }
        public static Angle Max(Angle angle1, Angle angle2)
        {
            return angle1 > angle2 ? angle1 : angle2;
        }
        public static Angle Min(Angle angle1, Angle angle2)
        {
            return angle1 < angle2 ? angle1 : angle2;
        }
        public static int Sign(Angle angle)
        {
            return Math.Sign(angle.radians);
        }
        public static float Sin(Angle angle)
        {
            return (float)Math.Sin(angle.radians);
        }
        public static float Tan(Angle angle)
        {
            return (float)Math.Tan(angle.radians);
        }
        #endregion
    }
}
