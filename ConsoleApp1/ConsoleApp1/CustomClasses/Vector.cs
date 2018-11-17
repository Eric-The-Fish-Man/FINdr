using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FINdr
{
    //two-directional vector class
    public class Vector
    {
        public float x;
        public float y;

        public Vector(float X, float Y)
        {
            x = X;
            y = Y;
        }

        public float GetAngle()
        {
            double Val = Math.Round(Math.Atan2(x, y) * 180 / Math.PI, 3);
            if (Val < 0)
            {
                Val += 360;
            }

            return (float)Val;
        }

        public float GetMagnitude()
        {
            return (float)Math.Abs(Math.Round(Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)), 3));
        }


        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(b.x - a.x, b.y - a.y);
        }
    }
}
