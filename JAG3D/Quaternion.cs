using System;

namespace org.applied_geodesy.adjustment.geometry
{
	public class Quaternion
	{
		private double[] q = new double[4];
		public Quaternion()
		{
		}

		public Quaternion(double[] n, double alpha)
		{
			double[] n0 = this.normalise(n);
			double c = Math.Cos(0.5 * alpha);
			double s = Math.Sin(0.5 * alpha);
			this.q[0] = c;
			this.q[1] = s * n0[0];
			this.q[2] = s * n0[1];
			this.q[3] = s * n0[2];
		}

		public Quaternion(double[] v) : this(v.Length == 4 ? v[0] : 0.0, v.Length == 4 ? v[1] : v[0], v.Length == 4 ? v[2] : v[1], v.Length == 4 ? v[3] : v[2])
		{
		}

		public Quaternion(double[][] R) : this(toQuaternion(R))
		{
		}

		public Quaternion(Quaternion q) : this(q.Q0, q.Q1, q.Q2, q.Q3)
		{
		}

		public Quaternion(double q0, double q1, double q2, double q3)
		{
			this.q[0] = q0;
			this.q[1] = q1;
			this.q[2] = q2;
			this.q[3] = q3;
		}

		public virtual double getArg(int i)
		{
			return this.q[i];
		}

		public virtual double Q0
		{
			get
			{
				return this.q[0];
			}
		}

		public virtual double Q1
		{
			get
			{
				return this.q[1];
			}
		}

		public virtual double Q2
		{
			get
			{
				return this.q[2];
			}
		}

		public virtual double Q3
		{
			get
			{
				return this.q[3];
			}
		}

		public virtual double[] toArray()
		{
			return this.q;
		}

		public virtual Quaternion rotate(Quaternion q)
		{
			Quaternion invQ = this.inv();
			Quaternion tmpQ = this.times(q);
			Quaternion qR = tmpQ.times(invQ);
			return qR;
		}


		public virtual Quaternion rotate(double[] p)
		{
			return this.rotate(new Quaternion(p));
		}

		public virtual Quaternion inv()
		{
			Quaternion cQ = this.conj();
			double abs2 = Math.Pow(this.abs(), 2);
			return new Quaternion(cQ.getArg(0) / abs2, cQ.getArg(1) / abs2, cQ.getArg(2) / abs2, cQ.getArg(3) / abs2);
		}


		public virtual Quaternion conj()
		{
			double[] qc = new double[this.q.Length];
			for (int i = 1; i < this.q.Length; i++)
			{
				qc[i] = -this.q[i];
			}
			qc[0] = this.q[0];
			return new Quaternion(qc[0], qc[1], qc[2], qc[3]);
		}


		public virtual double abs()
		{
			return this.norm(this.q);
		}


		public virtual Quaternion times(Quaternion q)
		{
			double[] mulQ = new double[4];

			mulQ[0] = this.q[0] * q.getArg(0) - this.q[1] * q.getArg(1) - this.q[2] * q.getArg(2) - this.q[3] * q.getArg(3);

			mulQ[1] = this.q[0] * q.getArg(1) + q.getArg(0) * this.q[1] + this.q[2] * q.getArg(3) - this.q[3] * q.getArg(2);
			mulQ[2] = this.q[0] * q.getArg(2) + q.getArg(0) * this.q[2] - this.q[1] * q.getArg(3) + this.q[3] * q.getArg(1);
			mulQ[3] = this.q[0] * q.getArg(3) + q.getArg(0) * this.q[3] + this.q[1] * q.getArg(2) - this.q[2] * q.getArg(1);
			return new Quaternion(mulQ[0], mulQ[1], mulQ[2], mulQ[3]);
		}

		public virtual double[][] toRotationMatrix()
		{
			return toRotationMatrix(this);
		}

		public static Quaternion toQuaternion(double[][] R)
		{
			double m11 = R[0][0];
			double m22 = R[1][1];
			double m33 = R[2][2];

			double m23 = R[1][2];
			double m32 = R[2][1];

			double m31 = R[2][0];
			double m13 = R[0][2];

			double m12 = R[0][1];
			double m21 = R[1][0];

			double q0 = 0.5 * Math.Sqrt(Math.Abs(m11 + m22 + m33 + 1.0));

			double q1 = (m32 - m23) / 4.0 / q0;
			double q2 = (m13 - m31) / 4.0 / q0;
			double q3 = (m21 - m12) / 4.0 / q0;

			return new Quaternion(q0, q1, q2, q3);
		}


		public static double[][] toRotationMatrix(Quaternion q)
		{
			double[][] R = { new double[3], new double[3], new double[3] };
			double q0 = q.Q0;
			double q1 = q.Q1;
			double q2 = q.Q2;
			double q3 = q.Q3;


			R[0] = new double[] { 2.0 * q0 * q0 - 1.0 + 2.0 * q1 * q1, 2.0 * (q1 * q2 - q0 * q3), 2.0 * (q1 * q3 + q0 * q2) };

			R[1] = new double[] { 2.0 * (q1 * q2 + q0 * q3), 2.0 * q0 * q0 - 1.0 + 2.0 * q2 * q2, 2.0 * (q2 * q3 - q0 * q1) };

			R[2] = new double[] { 2.0 * (q1 * q3 - q0 * q2), 2.0 * (q2 * q3 + q0 * q1), 2.0 * q0 * q0 - 1.0 + 2.0 * q3 * q3 };

			return R;
		}


		private double norm(double[] n)
		{
			double norm = 0;
			for (int i = 0; i < n.Length; i++)
			{
				norm = norm + n[i] * n[i];
			}
			return Math.Sqrt(norm);
		}

		private double[] normalise(double[] n)
		{
			double norm = this.norm(n);
			if (norm <= 0)
			{
				throw new System.ArgumentException("Error, norm of axis vector is equal or less then zero!");
			}

			for (int i = 0; i < n.Length; i++)
			{
				n[i] = n[i] / norm;
			}

			return n;
		}

		public override string ToString()
		{
			return "s = " + this.q[0] + "  q = [" + this.q[1] + ", " + this.q[2] + " ," + this.q[3] + "]";
		}

		public virtual double[] RotationAxis
		{
			get
			{
				double[] n = new double[3];
				double q0 = this.q[0];
				double s = Math.Sin(Math.Acos(q0));
				if (s == 0)
				{
					return n;
				}
				for (int i = 0; i < n.Length; i++)
				{
					n[i] = this.q[i + 1] / s;
				}
				return this.normalise(n);
			}
		}

		public virtual double[] EulerAngles
		{
			get
			{
				double q0 = this.Q0;
				double q1 = this.Q1;
				double q2 = this.Q2;
				double q3 = this.Q3;

				double r13 = 2.0 * (q1 * q3 + q0 * q2);
				double r23 = 2.0 * (q2 * q3 - q0 * q1);
				double r33 = 2.0 * q0 * q0 - 1.0 + 2.0 * q3 * q3;

				double r12 = 2.0 * (q1 * q2 - q0 * q3);
				double r11 = 2.0 * q0 * q0 - 1.0 + 2.0 * q1 * q1;

				double rx = Math.Atan2(r23, r33);
				double ry = Math.Atan2(-r13, hypo(r23, r33));
				double rz = Math.Atan2(r12, r11);

				return new double[] { rx, ry, rz };
			}
		}

		double hypo(double x, double y)
		{
			return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
		}
	}
}