using System;

namespace AStarPathFinding
{
    public struct Vec3 : IEquatable<Vec3>
    {
        public const float kEpsilon = 0.00001F;

        public static readonly Vec3 right = new Vec3(1, 0, 0);
        public static readonly Vec3 left = new Vec3(-1, 0, 0);
        public static readonly Vec3 up = new Vec3(0, 1, 0);
        public static readonly Vec3 down = new Vec3(0, -1, 0);
        public static readonly Vec3 forward = new Vec3(0, 0, 1);
        public static readonly Vec3 back = new Vec3(0, 0, -1);

        public float x;
        public float y;
        public float z;

        public float magnitude => (float)Math.Sqrt(x * x + y * y + z * z);
        public float sqrMagnitude => x * x + y * y + z * z;

        public Vec3 normalized
        {
            get
            {
                var mag = magnitude;
                if(mag > kEpsilon)
                {
                    return new Vec3(this.x / mag, this.y / mag, this.z / mag);
                }
                else
                {
                    return new Vec3(0, 0, 0);
                }
            }
        }

        public Vec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static float Distance(Vec3 a, Vec3 b)
        {
            var diffVec = a - b;
            return (float)Math.Sqrt(diffVec.x * diffVec.x + diffVec.y * diffVec.y + diffVec.z * diffVec.z);
        }

        #region ---------- Operators ----------
        
        public static Vec3 operator -(Vec3 a, Vec3 b)
        {
            return new Vec3(a.x - b.x, a.y - b.y, a.z - b.z);
        }
        // Negates a vector.
        public static Vec3 operator -(Vec3 a) { return new Vec3(-a.x, -a.y, -a.z); }

        public static Vec3 operator +(Vec3 a, Vec3 b)
        {
            return new Vec3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        // Multiplies a vector by a number.
        public static Vec3 operator *(Vec3 a, float d) { return new Vec3(a.x * d, a.y * d, a.z * d); }
        // Multiplies a vector by a number.
        public static Vec3 operator *(float d, Vec3 a) { return new Vec3(a.x * d, a.y * d, a.z * d); }
        // Divides a vector by a number.
        public static Vec3 operator /(Vec3 a, float d) { return new Vec3(a.x / d, a.y / d, a.z / d); }

        // Returns true if the vectors are equal.
        public static bool operator ==(Vec3 lhs, Vec3 rhs)
        {
            // Returns false in the presence of NaN values.
            float diff_x = lhs.x - rhs.x;
            float diff_y = lhs.y - rhs.y;
            float diff_z = lhs.z - rhs.z;
            float sqrmag = diff_x * diff_x + diff_y * diff_y + diff_z * diff_z;
            return sqrmag < kEpsilon * kEpsilon;
        }

        // Returns true if vectors are different.
        public static bool operator !=(Vec3 lhs, Vec3 rhs)
        {
            // Returns true in the presence of NaN values.
            return !(lhs == rhs);
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (!(obj is Vec3)) return false;

            return Equals((Vec3)obj);
        }

        public bool Equals(Vec3 other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
        }
    }

}
