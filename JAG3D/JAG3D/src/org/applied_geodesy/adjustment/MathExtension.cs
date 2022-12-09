using System;

/// <summary>
///*********************************************************************
/// Copyright by Michael Loesler, https://software.applied-geodesy.org   *
///                                                                      *
/// This program is free software; you can redistribute it and/or modify *
/// it under the terms of the GNU General Public License as published by *
/// the Free Software Foundation; either version 3 of the License, or    *
/// at your option any later version.                                    *
///                                                                      *
/// This program is distributed in the hope that it will be useful,      *
/// but WITHOUT ANY WARRANTY; without even the implied warranty of       *
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
/// GNU General Public License for more details.                         *
///                                                                      *
/// You should have received a copy of the GNU General Public License    *
/// along with this program; if not, see <http://www.gnu.org/licenses/>  *
/// or write to the                                                      *
/// Free Software Foundation, Inc.,                                      *
/// 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.            *
///                                                                      *
/// **********************************************************************
/// </summary>

namespace org.applied_geodesy.adjustment
{
	using intW = org.netlib.util.intW;

	using LAPACK = com.github.fommil.netlib.LAPACK;

	using DenseMatrix = no.uib.cipr.matrix.DenseMatrix;
	using DenseVector = no.uib.cipr.matrix.DenseVector;
	using Matrix = no.uib.cipr.matrix.Matrix;
	using MatrixNotSPDException = no.uib.cipr.matrix.MatrixNotSPDException;
	using MatrixSingularException = no.uib.cipr.matrix.MatrixSingularException;
	using NotConvergedException = no.uib.cipr.matrix.NotConvergedException;
	using SVD = no.uib.cipr.matrix.SVD;
	using UnitUpperTriangBandMatrix = no.uib.cipr.matrix.UnitUpperTriangBandMatrix;
	using UpperSymmBandMatrix = no.uib.cipr.matrix.UpperSymmBandMatrix;
	using UpperSymmPackMatrix = no.uib.cipr.matrix.UpperSymmPackMatrix;
	using UpperTriangPackMatrix = no.uib.cipr.matrix.UpperTriangPackMatrix;
	using Vector = no.uib.cipr.matrix.Vector;
	using CompDiagMatrix = no.uib.cipr.matrix.sparse.CompDiagMatrix;

	public sealed class MathExtension
	{
		/// <summary>
		/// Liefert in Abhaengigkeit vom Vorzeichen von b den Wert a positiv oder negativ.
		/// Das Vorzeichen von a wird ignoriert. </summary>
		/// <param name="a"> </param>
		/// <param name="b"> </param>
		/// <returns> sign </returns>
		public static double SIGN(double a, double b)
		{
			return b >= 0.0 ? Math.Abs(a) : -Math.Abs(a);
		}

		/// <summary>
		/// Winkelreduktion auf ein pos. Intervall
		/// Vergleich zur modularen Operation:
		/// <pre>-50%400 == -50</pre>
		/// <pre>mod(-50,400) == 350</pre>
		/// </summary>
		/// <param name="x"> </param>
		/// <param name="y"> </param>
		/// <returns> mod </returns>
		public static double MOD(double x, double y)
		{
			return x - Math.Floor(x / y) * y;
		}

		/// <summary>
		/// Liefert die Kondition einer Matrix c = cond(M) 
		/// mithilfe von SVD
		/// 
		/// [u v w] = svd(M)
		/// c = max(v)/min(v)
		/// </summary>
		/// <param name="M"> </param>
		/// <returns> c = cond(M) </returns>
		/// <exception cref="NotConvergedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static double cond(no.uib.cipr.matrix.Matrix M) throws no.uib.cipr.matrix.NotConvergedException
		public static double cond(Matrix M)
		{
			SVD uwv = SVD.factorize(M);
			double[] s = uwv.getS();
			// vgl. http://www.mathworks.de/help/techdoc/ref/cond.html
			int m = Math.Min(M.numColumns(), M.numRows()) - 1;
			if (s[m] != 0)
			{
				return s[0] / s[m];
			}
			return 0;
		}

		/// <summary>
		/// Liefert die Pseudoinverse Q = M<sup>+1</sup> 
		/// der Matrix M mithilfe von SVD
		/// 
		/// [u v w] = svd(M)
		/// Q = v*w<sup>-1</sup>*u<sup>T</sup> 
		/// </summary>
		/// <param name="M"> </param>
		/// <returns> Q = M<sup>+1</sup> </returns>
		/// <exception cref="NotConvergedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static no.uib.cipr.matrix.Matrix pinv(no.uib.cipr.matrix.Matrix M) throws no.uib.cipr.matrix.NotConvergedException
		public static Matrix pinv(Matrix M)
		{
			return MathExtension.pinv(M, 0.0);
		}

		/// <summary>
		/// Liefert die Pseudoinverse Q = M<sup>+1</sup> 
		/// der Matrix M mithilfe von SVD
		/// 
		/// [u v w] = svd(M)
		/// Q = v*w<sup>-1</sup>*u<sup>T</sup> 
		/// </summary>
		/// <param name="M"> </param>
		/// <param name="tol"> </param>
		/// <returns> Q = M<sup>+1</sup> </returns>
		/// <exception cref="NotConvergedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static no.uib.cipr.matrix.Matrix pinv(no.uib.cipr.matrix.Matrix M, double tol) throws no.uib.cipr.matrix.NotConvergedException
		public static Matrix pinv(Matrix M, double tol)
		{
			SVD uwv = SVD.factorize(M);

			Matrix U = uwv.getU();
			Matrix VT = uwv.getVt();
			double[] s = uwv.getS();
			//Matrix W = new DenseMatrix(VT.numColumns(), U.numColumns());
			Matrix W = new CompDiagMatrix(VT.numColumns(), U.numColumns());
			// Bestimme Toleranz neu
			// vgl. http://www.mathworks.de/help/techdoc/ref/pinv.html
			if (tol < Constant.EPS)
			{
				double norm2 = 0.0;
				for (int i = 0; i < s.Length; i++)
				{
					norm2 = Math.Max(norm2, Math.Abs(s[i]));
				}

				tol = Math.Max(M.numColumns(), M.numRows()) * norm2 * (tol < 0 ? Math.Sqrt(Constant.EPS) : Constant.EPS);
			}

			for (int i = 0; i < s.Length; i++)
			{
				if (Math.Abs(s[i]) > tol)
				{
					W.set(i,i, 1.0 / s[i]);
				}
			}
			s = null;

			Matrix VW = new DenseMatrix(VT.numRows(), W.numColumns());
			VT.transAmult(W, VW);
			W = null;
			VT = null;

			Matrix Q = new DenseMatrix(M.numColumns(), M.numRows());
			VW.transBmult(U, Q);

			return Q;
		}

		/// <summary>
		/// Liefert die Pseudoinverse Q = M<sup>+1</sup> 
		/// der Matrix M mithilfe von SVD, wobei der Defekt
		/// der Matrix durch rank bereits vorgegeben ist.
		/// Es werden nur die ersten n Singulaerwerte beruecksichtigt.
		/// 
		/// [u v w] = svd(M)
		/// Q = v*w<sup>-1</sup>*u<sup>T</sup> 
		/// </summary>
		/// <param name="M"> </param>
		/// <param name="rank"> </param>
		/// <returns> Q = M<sup>+1</sup> </returns>
		/// <exception cref="NotConvergedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static no.uib.cipr.matrix.Matrix pinv(no.uib.cipr.matrix.Matrix M, int rank) throws no.uib.cipr.matrix.NotConvergedException
		public static Matrix pinv(Matrix M, int rank)
		{
			if (rank <= 0)
			{
				return MathExtension.pinv(M, (double)rank);
			}

			SVD uwv = SVD.factorize(M);

			Matrix U = uwv.getU();
			Matrix VT = uwv.getVt();
			double[] s = uwv.getS();
			Matrix W = new CompDiagMatrix(VT.numColumns(), U.numColumns());
			//Matrix W = new DenseMatrix(VT.numColumns(), U.numColumns());
			// Korrigiere das Intervall auf 0 und Anz. Singul.werte
			rank = Math.Max(0, Math.Min(rank, s.Length));

			for (int i = 0; i < rank; i++)
			{
				if (Math.Abs(s[i]) > 0)
				{
					W.set(i,i, 1.0 / s[i]);
				}
			}
			s = null;

			Matrix VW = new DenseMatrix(VT.numRows(), W.numColumns());
			VT.transAmult(W, VW);
			W = null;
			VT = null;

			Matrix Q = new DenseMatrix(M.numColumns(), M.numRows());
			VW.transBmult(U, Q);

			return Q;
		}

		/// <summary>
		/// Liefert eine quadratische Einheitsmatrix der Dimension <code>size</code> </summary>
		/// <param name="size"> </param>
		/// <returns> I </returns>
		public static Matrix identity(int size)
		{
			return new UnitUpperTriangBandMatrix(size,0);
		}

		/// <summary>
		/// Loest das Gleichungssystem <code>N * x = n</code>. Der Vektor n wird hierbei mit dem Loesungsvektor <code>x</code> ueberschrieben. 
		/// Wenn <code>invert = true</code>, dann wird <code>N</code> mit dessen Inverse ueberschrieben.
		/// </summary>
		/// <param name="N"> </param>
		/// <param name="n"> </param>
		/// <param name="numRows"> </param>
		/// <param name="invert"> </param>
		/// <exception cref="MatrixSingularException"> </exception>
		/// <exception cref="IllegalArgumentException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void solve(no.uib.cipr.matrix.UpperSymmPackMatrix N, no.uib.cipr.matrix.DenseVector n, int numRows, boolean invert) throws MatrixSingularException, IllegalArgumentException
		public static void solve(UpperSymmPackMatrix N, DenseVector n, int numRows, bool invert)
		{
			const string UPLO = "U";

			double[] nd = n.getData();
			double[] Nd = N.getData();
			int[] ipiv = new int[numRows];

			intW info = new intW(0);

			// http://www.netlib.org/lapack/double/dspsv.f
			LAPACK.getInstance().dspsv(UPLO, numRows, 1, Nd, ipiv, nd, Math.Max(1, numRows), info);

			if (info.val > 0)
			{
				throw new MatrixSingularException();
			}
			else if (info.val < 0)
			{
				throw new System.ArgumentException();
			}

			if (invert)
			{
				double[] work = new double[numRows];

				// http://www.netlib.org/lapack/double/dsptri.f
				LAPACK.getInstance().dsptri(UPLO, numRows, Nd, ipiv, work, info);

				if (info.val > 0)
				{
					throw new MatrixSingularException();
				}
				else if (info.val < 0)
				{
					throw new System.ArgumentException();
				}
			}
		}

		/// <summary>
		/// Loest das Gleichungssystem <code>N * x = n</code>. Der Vektor n wird hierbei mit dem Loesungsvektor <code>x</code> ueberschrieben. 
		/// Wenn <code>invert = true</code>, dann wird <code>N</code> mit dessen Inverse ueberschrieben.
		/// </summary>
		/// <param name="N"> </param>
		/// <param name="n"> </param>
		/// <param name="invert"> </param>
		/// <exception cref="MatrixSingularException"> </exception>
		/// <exception cref="IllegalArgumentException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void solve(no.uib.cipr.matrix.UpperSymmPackMatrix N, no.uib.cipr.matrix.DenseVector n, boolean invert) throws MatrixSingularException, IllegalArgumentException
		public static void solve(UpperSymmPackMatrix N, DenseVector n, bool invert)
		{
			solve(N, n, N.numRows(), invert);
		}

		/// <summary>
		/// Liefert die Inverse einer symmetrischen oberen Dreiecksmatrix mittels <code>N = LDL'</code> Zerlegung. <code>N</code> wird hierbei ueberschrieben.
		/// </summary>
		/// <param name="N"> Matrix </param>
		/// <exception cref="MatrixSingularException"> </exception>
		/// <exception cref="IllegalArgumentException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void inv(no.uib.cipr.matrix.UpperSymmPackMatrix N) throws MatrixSingularException, IllegalArgumentException
		public static void inv(UpperSymmPackMatrix N)
		{
			inv(N, N.numRows());
		}

		/// <summary>
		/// Liefert die Inverse einer symmetrischen oberen Dreiecksmatrix mittels <code>N = LDL'</code> Zerlegung. <code>N</code> wird hierbei ueberschrieben.
		/// </summary>
		/// <param name="N"> Matrix </param>
		/// <param name="numRows"> Anzahl der Spalten in N, die beim invertieren zu beruecksichtigen sind </param>
		/// <exception cref="MatrixSingularException"> </exception>
		/// <exception cref="IllegalArgumentException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void inv(no.uib.cipr.matrix.UpperSymmPackMatrix N, int numRows) throws MatrixSingularException, IllegalArgumentException
		public static void inv(UpperSymmPackMatrix N, int numRows)
		{
			const string UPLO = "U";
			int[] ipiv = new int[numRows];
			intW info = new intW(0);
			double[] qd = N.getData();

			// http://www.netlib.org/lapack/double/dsptrf.f
			LAPACK.getInstance().dsptrf(UPLO, numRows, qd, ipiv, info);

			if (info.val > 0)
			{
				throw new MatrixSingularException();
			}
			else if (info.val < 0)
			{
				throw new System.ArgumentException();
			}

			double[] work = new double[numRows];

			// http://www.netlib.org/lapack/double/dsptri.f
			LAPACK.getInstance().dsptri(UPLO, numRows, qd, ipiv, work, info);

			if (info.val > 0)
			{
				throw new MatrixSingularException();
			}
			else if (info.val < 0)
			{
				throw new System.ArgumentException();
			}
		}

		/// <summary>
		/// Bestimmt ausgewaehlte Eigenwerte einer symmetrischen oberen Dreiecksmatrix <code>N</code>. Die Indizes der zu bestimmeden
		/// Eigenwerte ergeben sich aus dem Intervall <code>il <= i <= iu</code>, mit <code>il >= 1</code> und <code>ul <= n</code>.
		/// Sie werden in aufsteigender Reihenfolge ermittelt. Ist die Flag <code>vectors = true</code> gesetzt, werden die zugehoerigen
		/// Eigenvektoren mitbestimmt. Durch die Flag <code>n</code> kann die Eigenwert/-vektorbestimmung auf die ersten <code>n</code>-Elemente
		/// begrenzt werden.
		/// 
		/// Die Eigenwerte <code>eval</code> werden als UpperSymmBandMatrix gespeichert, die Eigenvektoren <code>evec</code> in einer DenseMatrix.
		/// 
		/// HINWEIS: Die Matrix <code>N</code> wird bei dieser Zerlegung ueberschrieben!!!
		/// </summary>
		/// <param name="N"> </param>
		/// <param name="n"> </param>
		/// <param name="il"> </param>
		/// <param name="iu"> </param>
		/// <param name="vectors"> </param>
		/// <returns> {eval, evec} </returns>
		/// <exception cref="NotConvergedException"> </exception>
		/// <exception cref="IllegalArgumentException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static no.uib.cipr.matrix.Matrix[] eig(no.uib.cipr.matrix.UpperSymmPackMatrix N, int n, int il, int iu, boolean vectors) throws NotConvergedException, IllegalArgumentException
		public static Matrix[] eig(UpperSymmPackMatrix N, int n, int il, int iu, bool vectors)
		{
			n = n < 0 ? N.numRows() : n;
			if (il < 1)
			{
				throw new System.ArgumentException("Error, lower index of eigenvalue must be il >= 1: il = " + il);
			}
			if (iu > n)
			{
				throw new System.ArgumentException("Error, upper index of eigenvalue must be iu > n: iu = " + iu + ", n = " + n);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String jobz = vectors ? "V" : "N";
			string jobz = vectors ? "V" : "N";
			const string range = "I";
			const string uplo = "U";

			double[] ap = N.getData();
			double vl = 0;
			double vu = 0;
			double abstol = 2.0 * LAPACK.getInstance().dlamch("S");
			intW m = new intW(0);
			double[] evalArray = new double[n]; // n because of multiple roots
			//DenseMatrix evec = vectors ? new DenseMatrix(iu-il + 1, n) : new DenseMatrix(0, 0);
			DenseMatrix evec = vectors ? new DenseMatrix(n, iu - il + 1) : new DenseMatrix(0, 0);
			int ldz = Math.Max(1,n);
			double[] work = new double[8 * n];
			int[] iwork = new int[5 * n];
			int[] ifail = vectors ? new int[n] : new int[0];
			intW info = new intW(0);

			if (il <= 0 || il > iu && n > 0 || iu > n)
			{
				throw new System.ArgumentException("Error, invalid or wrong arguments, i.e., il <= 0 || il > iu && n > 0 || iu > n! il = " + il + ", iu = " + iu + ", n = " + n);
			}

			// http://www.netlib.org/lapack/double/dspevx.f
			//LAPACK.getInstance().dspevx(jobz, range, uplo, n, ap, vl, vu, il, iu, abstol, m, eval.getData(), evec.getData(), ldz, work, iwork, ifail, info);
			LAPACK.getInstance().dspevx(jobz, range, uplo, n, ap, vl, vu, il, iu, abstol, m, evalArray, evec.getData(), ldz, work, iwork, ifail, info);

			if (info.val > 0)
			{
				throw new NotConvergedException(NotConvergedException.Reason.Breakdown);
			}
			else if (info.val < 0)
			{
				throw new System.ArgumentException("Error, invalid or wrong argument for function call dspevx() " + info.val + "!");
			}

			work = null;
			iwork = null;
			ifail = null;

			UpperSymmBandMatrix eval = new UpperSymmBandMatrix(iu - il + 1, 0);
			Array.Copy(evalArray, 0, eval.getData(), 0, iu - il + 1);

			return new Matrix[] {eval, evec};
		}

		/// <summary>
		/// Druckt eine Matrix auf der Konsole aus </summary>
		/// <param name="M"> </param>
		public static void print(Matrix M)
		{
			for (int i = 0; i < M.numRows(); i++)
			{
				for (int j = 0; j < M.numColumns(); j++)
				{
					Console.Write(M.get(i,j) + "  ");
				}
				Console.WriteLine();
			}
		}

		/// <summary>
		/// Druckt einen Vektor auf der Konsole aus </summary>
		/// <param name="v"> </param>
		public static void print(Vector v)
		{
			for (int i = 0; i < v.size(); i++)
			{
				Console.WriteLine(v.get(i));
			}
		}

		/// <summary>
		/// Liefert das Kreuzprodukt zweier 3x1-Vektoren </summary>
		/// <param name="a"> </param>
		/// <param name="b"> </param>
		/// <returns> c </returns>
		/// <exception cref="IllegalArgumentException"> Wenn die Anzahl der Elemente in a oder/und b ungleich 3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static no.uib.cipr.matrix.DenseVector cross(no.uib.cipr.matrix.Vector a, no.uib.cipr.matrix.Vector b) throws IllegalArgumentException
		public static DenseVector cross(Vector a, Vector b)
		{
			if (a.size() != 3 || b.size() != 3)
			{
				throw new System.ArgumentException("Error, cross-product can only applied to 3 x 1 - vectors, " + a.size() + " and " + b.size());
			}
			DenseVector c = new DenseVector(3);
			c.set(0, a.get(1) * b.get(2) - a.get(2) * b.get(1));
			c.set(1, a.get(2) * b.get(0) - a.get(0) * b.get(2));
			c.set(2, a.get(0) * b.get(1) - a.get(1) * b.get(0));
			return c;
		}

		/// <summary>
		/// In-Place Cholesky-Zerlegung einer (oberen) symmetrischen Matrix.
		/// </summary>
		/// <param name="M"> </param>
		/// <exception cref="IllegalArgumentException"> </exception>
		/// <exception cref="MatrixNotSPDException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void chol(no.uib.cipr.matrix.UpperTriangPackMatrix M) throws IllegalArgumentException, no.uib.cipr.matrix.MatrixNotSPDException
		public static void chol(UpperTriangPackMatrix M)
		{
			packChol(M.numRows(), M.getData());
		}

		/// <summary>
		/// In-Place Cholesky-Zerlegung einer (oberen) symmetrischen Matrix.
		/// </summary>
		/// <param name="M"> </param>
		/// <exception cref="IllegalArgumentException"> </exception>
		/// <exception cref="MatrixNotSPDException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void chol(no.uib.cipr.matrix.UpperSymmPackMatrix M) throws IllegalArgumentException, no.uib.cipr.matrix.MatrixNotSPDException
		public static void chol(UpperSymmPackMatrix M)
		{
			packChol(M.numRows(), M.getData());
		}

		/// <summary>
		/// In-Place Cholesky-Zerlegung einer (oberen) symmetrischen Matrix. 
		/// Die Symmetrie wird nicht geprueft waerend der Zerlegung. Das Array
		/// liegt im PACK-Format vor.
		/// </summary>
		/// <param name="size"> </param>
		/// <param name="data"> </param>
		/// <exception cref="IllegalArgumentException"> </exception>
		/// <exception cref="MatrixNotSPDException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private static void packChol(int size, double data[]) throws IllegalArgumentException, no.uib.cipr.matrix.MatrixNotSPDException
		private static void packChol(int size, double[] data)
		{
			const string uplo = "U";
			intW info = new intW(0);

			LAPACK.getInstance().dpptrf(uplo, size, data, info);

			if (info.val > 0)
			{
				throw new MatrixNotSPDException("Error, matrix must be positive definite!");
			}
			else if (info.val < 0)
			{
				throw new System.ArgumentException("Error, invalid or wrong argument for function call dpptrf() " + info.val + "!");
			}
		}

		/// <summary>
		/// Bestimmt den Kotangens </summary>
		/// <param name="x"> </param>
		/// <returns> cot(x) </returns>
		public static double cot(double x)
		{
			return 1.0 / Math.Tan(x);
		}

		/// <summary>
		/// Bestimmt den Arkuskotangens </summary>
		/// <param name="x"> </param>
		/// <returns> acot(x) </returns>
		public static double acot(double x)
		{
			return Math.Atan(1.0 / x);
		}

		/// <summary>
		/// Bestimmt den Arkuskotangens mit Quadrantenabfrage, d.h., atan2(y, x) == acot2(x, y) </summary>
		/// <param name="x"> </param>
		/// <param name="y"> </param>
		/// <returns> acot2(x, y) </returns>
		public static double acot2(double x, double y)
		{
			return Math.Atan2(y, x);
		}
	}
}