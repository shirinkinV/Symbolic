using System;
using System.Collections.Generic;
using System.Text;

namespace ShirinkinV.Symbolic.Functions
{
    public class SVector3Matrix
    {
        private int n, m;
        private List<List<SVector3>> rows;

        public List<List<SVector3>> Rows { get => rows; set => rows = value; }

        public int N { get => n; }
        public int M { get => m; }
        #region Constructors
        public SVector3Matrix(int n, int m)
        {
            rows = new List<List<SVector3>>(n);
            for (int i = 0; i < n; i++)
            {
                rows[i] = new List<SVector3>(m);
                for (int j = 0; j < m; j++)
                {
                    rows[i][j] = new SVector3(0, 0, 0);
                }
            }
        }
        #endregion
        #region Matrix operations
        public static SVector3Matrix operator +(SVector3Matrix left, SVector3Matrix right)
        {
            if (left.n != right.n || left.m != right.m) throw new Exception();
            var result = new SVector3Matrix(left.n, left.m);
            for (int i = 0; i < left.N; i++)
            {
                for (int j = 0; j < left.M; j++)
                {
                    result[i, j] = left[i, j] + right[i, j];
                }
            }
            return result;
        }
        public static SVector3Matrix operator -(SVector3Matrix left, SVector3Matrix right)
        {
            if (left.n != right.n || left.m != right.m) throw new Exception();
            var result = new SVector3Matrix(left.n, left.m);
            for (int i = 0; i < left.N; i++)
            {
                for (int j = 0; j < left.M; j++)
                {
                    result[i, j] = left[i, j] - right[i, j];
                }
            }
            return result;
        }
        public static SVector3Matrix operator +(SVector3Matrix m)
        {
            var result = new SVector3Matrix(m.n, m.m);
            for (int i = 0; i < m.N; i++)
            {
                for (int j = 0; j < m.M; j++)
                {
                    result[i, j] = m[i, j].Clone();
                }
            }
            return result;
        }
        public static SVector3Matrix operator -(SVector3Matrix m)
        {
            var result = new SVector3Matrix(m.n, m.m);
            for (int i = 0; i < m.N; i++)
            {
                for (int j = 0; j < m.M; j++)
                {
                    result[i, j] = -m[i, j];
                }
            }
            return result;
        }
        public static SMatrix operator *(SVector3Matrix left, SVector3Matrix right)
        {
            if (left.m != right.n)
            {
                throw new ArgumentException("dimentions not equal");
            }
            var result = new SMatrix(left.n, right.m);
            for (int i = 0; i < left.n; i++)
            {
                for (int j = 0; j < right.m; j++)
                {
                    result[i, j] = ScalarProductOfVectors(right.Row(i), left.Column(j));
                }
            }
            return result;
        }

        private static CommonF ScalarProductOfVectors(List<SVector3> v1, List<SVector3> v2)
        {
            if (v1.Count != v2.Count) throw new Exception();
            CommonF result = 0;
            for (int i = 0; i < v1.Count; i++)
            {
                result += v1[i] * v2[i];
            }
            return result;
        }

        public static SVector3Matrix operator ^(SVector3Matrix left, SVector3Matrix right)
        {
            if (left.m != right.n)
            {
                throw new ArgumentException("dimentions not equal");
            }
            var result = new SVector3Matrix(left.n, right.m);
            for (int i = 0; i < left.n; i++)
            {
                for (int j = 0; j < right.m; j++)
                {
                    result[i, j] = VectorProductOfVectors(right.Row(i), left.Column(j));
                }
            }
            return result;
        }

        private static SVector3 VectorProductOfVectors(List<SVector3> v1, List<SVector3> v2)
        {
            if (v1.Count != v2.Count) throw new Exception();
            SVector3 result = new SVector3(0, 0, 0);
            for (int i = 0; i < v1.Count; i++)
            {
                result += v1[i] ^ v2[i];
            }
            return result;
        }

        #endregion
        #region Operations with other types
        public static SVector3Matrix operator *(SVector3Matrix left, SMatrix right)
        {
            if (left.m != right.N)
            {
                throw new ArgumentException("dimentions not equal");
            }
            var result = new SVector3Matrix(left.n, right.M);
            for (int i = 0; i < left.n; i++)
            {
                for (int j = 0; j < right.M; j++)
                {
                    result[i, j] = ScalarProductOfLV3andV(left.Row(i), right.Column(j));
                }
            }
            return result;
        }

        private static SVector3 ScalarProductOfLV3andV(List<SVector3> left, SVector right)
        {
            if (right.Count != left.Count) throw new Exception();
            var result = new SVector3(0, 0, 0);
            for (int i = 0; i < left.Count; i++)
            {
                result += left[i] * right[i];
            }
            return result;
        }

        public static SVector3Matrix operator *(SMatrix left, SVector3Matrix right)
        {
            if (left.M != right.N)
            {
                throw new ArgumentException("dimentions not equal");
            }
            var result = new SVector3Matrix(left.N, right.M);
            for (int i = 0; i < left.N; i++)
            {
                for (int j = 0; j < right.M; j++)
                {
                    result[i, j] = ScalarProductOfVandLV3(left.Row(i), right.Column(j));
                }
            }
            return result;
        }


        private static SVector3 ScalarProductOfVandLV3(SVector left, List<SVector3> right)
        {
            if (right.Count != left.Count) throw new Exception();
            var result = new SVector3(0, 0, 0);
            for (int i = 0; i < left.Count; i++)
            {
                result += left[i] * right[i];
            }
            return result;
        }
        #endregion

        #region Implicit
        public static implicit operator SVector3Matrix(SVector3[,] array)
        {
            var result = new SVector3Matrix(array.GetLength(0), array.GetLength(1));
            for (int i = 0; i < result.n; i++)
            {
                for (int j = 0; j < result.m; j++)
                {
                    result[i, j] = array[i, j];
                }
            }
            return result;
        }
        #endregion

        #region Access operators
        public List<SVector3> Row(int i)
        {
            List<SVector3> result = new List<SVector3>();
            for (int j = 0; j < m; j++)
            {
                result[j] = this[i, j];
            }
            return result;
        }
        public List<SVector3> Column(int j)
        {
            List<SVector3> result = new List<SVector3>();
            for (int i = 0; i < n; i++)
            {
                result[i] = this[i, j];
            }
            return result;
        }
        public SVector3 this[int i, int j]
        {
            get
            {
                return rows[i][j];
            }
            set
            {
                rows[i][j] = value;
            }
        }
        #endregion
    }
}
