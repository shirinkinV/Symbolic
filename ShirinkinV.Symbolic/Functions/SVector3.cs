using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShirinkinV.Symbolic.Functions
{
    public class SVector3 : SVector
    {
        public SVector3(List<CommonF> components) : base(components)
        {
            if (components.Count != 3) throw new Exception("3 dimension vector");
        }

        public SVector3(List<DefinedCommonF> components) : base(components)
        {
            if (components.Count != 3) throw new Exception("3 dimension vector");
        }

        public SVector3(CommonF x, CommonF y, CommonF z) : base(new List<CommonF>())
        {
            this[0] = x;
            this[1] = y;
            this[2] = z;
        }

        public new SVector3 Clone()
        {
            return new SVector3(X.Clone(), Y.Clone(), Z.Clone());
        }

        public CommonF X
        {
            get { return this[0]; }
            set { this[0] = value; }
        }

        public CommonF Y
        {
            get { return this[1]; }
            set { this[1] = value; }
        }

        public CommonF Z
        {
            get { return this[2]; }
            set { this[2] = value; }
        }

        public static SVector3 operator +(SVector3 left, SVector3 right)
        {
            return new SVector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }
        public static SVector3 operator -(SVector3 left, SVector3 right)
        {
            return new SVector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }
        public static CommonF operator *(SVector3 left, SVector3 right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        }
        public static SVector3 operator ^(SVector3 left, SVector3 right)
        {
            return new SVector3(
                left.Y * right.Z - left.Z * right.Y,
                left.Z * right.X - left.X * right.Z,
                left.X * right.Y - left.Y * right.X
                );
        }
        public static SVector3 operator -(SVector3 operand)
        {
            return new SVector3(-operand.X, -operand.Y, -operand.Z);
        }

        public static SVector3 operator *(SVector3 left, CommonF right)
        {
            return new SVector3(
                left.X * right,
                left.Y * right,
                left.Z * right);
        }
        public static SVector3 operator *(CommonF left, SVector3 right)
        {
            return new SVector3(
                right.X * left,
                right.Y * left,
                right.Z * left);
        }


        public static SVector3 operator /(SVector3 left, CommonF right)
        {
            return new SVector3(
                left.X / right,
                left.Y / right,
                left.Z / right);
        }


    }
}
