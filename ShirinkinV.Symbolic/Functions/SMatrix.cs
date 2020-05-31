using System;
using System.Collections.Generic;
using System.Text;

namespace ShirinkinV.Symbolic.Functions
{
    public class SMatrix
    {



        private int n, m;
        private List<SVector> rows;

        public List<SVector> Rows { get => rows; set => rows = value; }


        public int N { get => n; }
        public int M { get => m; }

        #region Constructors
        public SMatrix()
        {
            rows = new List<SVector>();
        }

        public SMatrix(int n)
        {
            this.n = n;
            this.m = n;
            rows = new List<SVector>();
            for (int i = 0; i < n; i++)
            {
                rows.Add(new SVector());
            }
        }

        public SMatrix(int n, int m)
        {
            this.n = n;
            this.m = m;
            rows = new List<SVector>();
            for (int i = 0; i < n; i++)
            {
                rows.Add(new SVector());
            }
        }

        public SMatrix(List<SVector> rows)
        {
            this.rows = rows;
        }
        #endregion

        #region Matrix operations
        public static SMatrix operator +(SMatrix m1, SMatrix m2)
        {
            if (m1.n != m2.n || m1.m != m2.m)
            {
                throw new ArgumentException("dimentions not equal");
            }
            var result = new SMatrix(m1.n, m1.m);
            for (int i = 0; i < m1.n; i++)
            {
                for (int j = 0; j < m1.m; j++)
                {
                    result[i, j] = m1[i, j] + m2[i, j];
                }
            }
            return result;
        }
        public static SMatrix operator -(SMatrix m1, SMatrix m2)
        {
            if (m1.n != m2.n || m1.m != m2.m)
            {
                throw new ArgumentException("dimentions not equal");
            }
            var result = new SMatrix(m1.n, m1.m);
            for (int i = 0; i < m1.n; i++)
            {
                for (int j = 0; j < m1.m; j++)
                {
                    result[i, j] = m1[i, j] - m2[i, j];
                }
            }
            return result;
        }
        public static SMatrix operator -(SMatrix m1)
        {
            var result = new SMatrix(m1.n, m1.m);
            for (int i = 0; i < m1.n; i++)
            {
                for (int j = 0; j < m1.m; j++)
                {
                    result[i, j] = -m1[i, j];
                }
            }
            return result;
        }
        public static SMatrix operator *(SMatrix m1, SMatrix m2)
        {
            if (m1.m != m2.n)
            {
                throw new ArgumentException("dimentions not equal");
            }
            var result = new SMatrix(m1.n, m2.m);
            for (int i = 0; i < m1.n; i++)
            {
                for (int j = 0; j < m2.m; j++)
                {
                    result[i, j] = m1.Row(i) * m2.Column(j);
                }
            }
            return result;
        }
        #endregion

        #region Operations with other types
        public static SMatrix operator *(CommonF s1, SMatrix m2)
        {
            var result = new SMatrix(m2.n, m2.m);
            for (int i = 0; i < m2.n; i++)
            {
                for (int j = 0; j < m2.m; j++)
                {
                    result[i, j] = m2[i, j] * s1;
                }
            }
            return result;
        }
        public static SMatrix operator /(SMatrix m1, CommonF s2)
        {
            var result = new SMatrix(m1.n, m1.m);
            for (int i = 0; i < m1.n; i++)
            {
                for (int j = 0; j < m1.m; j++)
                {
                    result[i, j] = m1[i, j] / s2;
                }
            }
            return result;
        }
        public static SMatrix operator *(SMatrix m1, CommonF s2)
        {
            var result = new SMatrix(m1.n, m1.m);
            for (int i = 0; i < m1.n; i++)
            {
                for (int j = 0; j < m1.m; j++)
                {
                    result[i, j] = m1[i, j] * s2;
                }
            }
            return result;
        }
        public static SVector operator *(SMatrix m1, SVector v2)
        {
            if (m1.m != v2.Count) throw new ArgumentException("dimentions not equal");
            SVector result = new SVector();
            for (int i = 0; i < m1.n; i++)
            {
                result[i] = m1.Row(i) * v2;
            }
            return result;
        }
        #endregion

        #region implicit
        public static implicit operator SMatrix(CommonF[,] src)
        {
            int n = src.GetLength(0);
            int m = src.GetLength(1);
            var result = new SMatrix(n, m);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    result[i, j] = src[i, j];
                }
            }

            return result;
        }
        #endregion

        #region Properties
        public CommonF Addition(int i, int j)
        {
            Constant multiplecator = ((i + j) % 2 == 0) ? 1 : -1;
            var minor = Minor(i, j);
            return multiplecator * minor.Determinant;
        }
        public SMatrix Inverse
        {
            get
            {
                if (n != m) throw new Exception("non quadratic matrix");
                var det = Determinant;
                var resultTmp = new SMatrix(n);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        resultTmp[i, j] = Addition(i, j);
                    }
                }
                return resultTmp.Transpose / det;
            }
        }
        public SMatrix Transpose
        {
            get
            {
                var result = new SMatrix(m, n);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        result[j, i] = this[i, j];
                    }
                }
                return result;
            }
        }
        public CommonF Determinant
        {
            get
            {
                CommonF result = new Constant(0);

                if (n != m) throw new Exception("non quadratic matrix");

                if (n == 1) result = this[0, 0];
                if (n == 2) result = this[0, 0] * this[1, 1] - this[1, 0] * this[0, 1];
                if (n == 3)
                {
                    result += this[0, 0] * this[1, 1] * this[2, 2];
                    result += this[1, 0] * this[2, 1] * this[0, 2];
                    result += this[0, 1] * this[1, 2] * this[2, 0];
                    result -= this[2, 0] * this[1, 1] * this[0, 2];
                    result -= this[0, 0] * this[2, 1] * this[1, 2];
                    result -= this[2, 2] * this[1, 0] * this[0, 1];
                }
                if (n >= 4)
                {
                    for (int i = 0; i < n; i++)
                    {
                        result += this[0, i] * Addition(0, i);
                    }
                }

                return result;
            }
        }
        #endregion

        #region Access operators

        public SVector Row(int i)
        {
            SVector result = new SVector();
            for (int j = 0; j < m; j++)
            {
                result[j] = this[i, j];
            }
            return result;
        }

        public SVector Column(int j)
        {
            SVector result = new SVector();
            for (int i = 0; i < n; i++)
            {
                result[i] = this[i, j];
            }
            return result;
        }
        public CommonF this[int i, int j]
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
        public SMatrix Minor(int i, int j)
        {
            SMatrix result = new SMatrix(n - 1, m - 1);
            //1 quarter
            for (int ii = 0; ii < i; ii++)
            {
                for (int jj = 0; jj < j; jj++)
                {
                    result[ii, jj] = this[ii, jj];
                }
            }
            //4 quarter
            for (int ii = i + 1; ii < n; ii++)
            {
                for (int jj = j + 1; jj < m; jj++)
                {
                    result[ii - 1, jj - 1] = this[ii, jj];
                }
            }
            //2 quarter
            for (int ii = 0; ii < i; ii++)
            {
                for (int jj = j + 1; jj < m; jj++)
                {
                    result[ii, jj - 1] = this[ii, jj];
                }
            }
            //3 quarter
            for (int ii = i + 1; ii < n; ii++)
            {
                for (int jj = 0; jj < j; jj++)
                {
                    result[ii - 1, jj] = this[ii, jj];
                }
            }
            return result;
        }
        #endregion
    }
}
