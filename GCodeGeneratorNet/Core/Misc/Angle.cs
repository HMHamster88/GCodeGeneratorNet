using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core.Misc
{
    public class Angle
    {
        double angle = 0;

        public Angle(double _angle)
        {
            angle = _angle;
        }

        public void Increment(double inc)
        {
            angle += inc;
            if (angle > Math.PI * 2)
                angle -= Math.PI * 2;
            else if (angle < 0)
                angle += Math.PI * 2;
        }
        public void Decrement(double dec)
        {
            Increment(-dec);
        }
        public override string ToString()
        {
            return angle.ToString();
        }
        public override int GetHashCode()
        {
            return angle.GetHashCode();
        }
        public static implicit operator double(Angle angleObj)
        {
            return angleObj.angle;
        }
        public static implicit operator Angle(double _angle)
        {
            return new Angle(_angle);
        }
        public static Angle operator +(Angle lhs, Angle rhs)
        {
            Angle angle = new Angle(lhs.angle);
            angle.Increment(rhs.angle);
            return angle;
        }
        public static Angle operator -(Angle lhs, Angle rhs)
        {
            Angle angle = new Angle(lhs.angle);
            angle.Decrement(rhs.angle);
            return angle;
        }
        public static bool operator <(Angle lhs, Angle rhs)
        {
            if (lhs.angle < rhs.angle)
                return true;
            return false;
        }
        public static bool operator <=(Angle lhs, Angle rhs)
        {
            if (lhs.angle <= rhs.angle)
                return true;
            return false;
        }
        public static bool operator >(Angle lhs, Angle rhs)
        {
            if (lhs.angle > rhs.angle)
                return true;
            return false;
        }
        public static bool operator >=(Angle lhs, Angle rhs)
        {
            if (lhs.angle >= rhs.angle)
                return true;
            return false;
        }
    }
}
