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

namespace org.applied_geodesy.adjustment.network.observation.group
{
	using Constant = org.applied_geodesy.adjustment.Constant;
	using MathExtension = org.applied_geodesy.adjustment.MathExtension;
	using DefaultUncertainty = org.applied_geodesy.adjustment.network.DefaultUncertainty;
	using Epoch = org.applied_geodesy.adjustment.network.Epoch;
	using ComponentType = org.applied_geodesy.adjustment.network.observation.ComponentType;
	using GNSSBaseline = org.applied_geodesy.adjustment.network.observation.GNSSBaseline;
	using GNSSBaseline3D = org.applied_geodesy.adjustment.network.observation.GNSSBaseline3D;
	using GNSSBaselineDeltaX3D = org.applied_geodesy.adjustment.network.observation.GNSSBaselineDeltaX3D;
	using GNSSBaselineDeltaY3D = org.applied_geodesy.adjustment.network.observation.GNSSBaselineDeltaY3D;
	using GNSSBaselineDeltaZ3D = org.applied_geodesy.adjustment.network.observation.GNSSBaselineDeltaZ3D;
	using Observation = org.applied_geodesy.adjustment.network.observation.Observation;
	using AdditionalUnknownParameter = org.applied_geodesy.adjustment.network.parameter.AdditionalUnknownParameter;
	using RotationX = org.applied_geodesy.adjustment.network.parameter.RotationX;
	using RotationY = org.applied_geodesy.adjustment.network.parameter.RotationY;
	using RotationZ = org.applied_geodesy.adjustment.network.parameter.RotationZ;
	using Scale = org.applied_geodesy.adjustment.network.parameter.Scale;
	using Point = org.applied_geodesy.adjustment.network.point.Point;
	using Point3D = org.applied_geodesy.adjustment.network.point.Point3D;
	using PointGroup = org.applied_geodesy.adjustment.network.point.group.PointGroup;

	using DenseMatrix = no.uib.cipr.matrix.DenseMatrix;
	using EVD = no.uib.cipr.matrix.EVD;
	using Matrix = no.uib.cipr.matrix.Matrix;
	using NotConvergedException = no.uib.cipr.matrix.NotConvergedException;

	public class GNSSBaseline3DGroup : ObservationGroup
	{

		private Scale scale = new Scale();
		private RotationX rx = new RotationX();
		private RotationY ry = new RotationY();
		private RotationZ rz = new RotationZ();

		private bool hasApproxValues = false;
		private double scaleParam = 1.0, rxParam = 0.0, ryParam = 0.0, rzParam = 0.0;

		public GNSSBaseline3DGroup(int id) : this(id, DefaultUncertainty.UncertaintyGNSSZeroPointOffset, DefaultUncertainty.UncertaintyGNSSSquareRootDistanceDependent, DefaultUncertainty.UncertaintyGNSSDistanceDependent, Epoch.REFERENCE)
		{
		}

		public GNSSBaseline3DGroup(int id, double sigmaA, double sigmaB, double sigmaC, Epoch epoch) : base(id, sigmaA, sigmaB, sigmaC, epoch)
		{
			this.scale.ObservationGroup = this;
			this.rx.ObservationGroup = this;
			this.ry.ObservationGroup = this;
			this.rz.ObservationGroup = this;
		}

		public override void add(Observation gnss3D)
		{
			throw new System.NotSupportedException(this.GetType().Name + " Fehler, GNSS-Basislinien bestehen nicht aus 3D-Beobachtungen!");
		}

		private void add(GNSSBaseline3D gnss3D)
		{
			gnss3D.Scale = this.scale;
			gnss3D.RotationX = this.rx;
			gnss3D.RotationY = this.ry;
			gnss3D.RotationZ = this.rz;
			base.add(gnss3D);
		}

		public virtual void add(GNSSBaselineDeltaX3D gnssX3D, GNSSBaselineDeltaY3D gnssY3D, GNSSBaselineDeltaZ3D gnssZ3D)
		{
			if (gnssX3D.Id == gnssY3D.Id && gnssX3D.Id == gnssZ3D.Id && gnssX3D.StartPoint.Name.Equals(gnssY3D.StartPoint.Name) && gnssX3D.EndPoint.Name.Equals(gnssY3D.EndPoint.Name) && gnssX3D.StartPoint.Name.Equals(gnssZ3D.StartPoint.Name) && gnssX3D.EndPoint.Name.Equals(gnssZ3D.EndPoint.Name))
			{

				this.add(gnssX3D);
				this.add(gnssY3D);
				this.add(gnssZ3D);

				gnssX3D.addAssociatedBaselineComponent(gnssY3D);
				gnssX3D.addAssociatedBaselineComponent(gnssZ3D);

				gnssY3D.addAssociatedBaselineComponent(gnssX3D);
				gnssY3D.addAssociatedBaselineComponent(gnssZ3D);

				gnssZ3D.addAssociatedBaselineComponent(gnssX3D);
				gnssZ3D.addAssociatedBaselineComponent(gnssY3D);
			}
		}

		public override double getStdA(Observation observation)
		{
			return this.StdA;
		}

		public override double getStdB(Observation observation)
		{
			double dist = observation.DistanceForUncertaintyModel;
			if (dist < Constant.EPS)
			{
				dist = Math.Abs(observation.ValueApriori); // should be equal to distance for uncertainty model
			}
			return this.StdB * Math.Sqrt(dist / 1000.0); // [km]
		}

		public override double getStdC(Observation observation)
		{
			double dist = observation.DistanceForUncertaintyModel;
			if (dist < Constant.EPS)
			{
				dist = Math.Abs(observation.ValueApriori); // should be equal to distance for uncertainty model
			}
			return this.StdC * dist;
		}

		public virtual Scale Scale
		{
			get
			{
				return this.scale;
			}
		}

		public virtual RotationX RotationX
		{
			get
			{
				return this.rx;
			}
		}

		public virtual RotationY RotationY
		{
			get
			{
				return this.ry;
			}
		}

		public virtual RotationZ RotationZ
		{
			get
			{
				return this.rz;
			}
		}

		public override int numberOfAdditionalUnknownParameter()
		{
			int num = 0;
			if (this.rx.Enable)
			{
				num++;
			}
			if (this.ry.Enable)
			{
				num++;
			}
			if (this.rz.Enable)
			{
				num++;
			}
			if (this.scale.Enable)
			{
				num++;
			}
			return num;
		}

		public override AdditionalUnknownParameter setApproximatedValue(AdditionalUnknownParameter param)
		{
			if (!this.hasApproxValues)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double SQRT_EPS = Math.sqrt(org.applied_geodesy.adjustment.Constant.EPS);
				double SQRT_EPS = Math.Sqrt(Constant.EPS);
				PointGroup sourceSystem = new PointGroup(1);
				PointGroup targetSystem = new PointGroup(1);

				for (int i = 0; i < this.size(); i++)
				{
					GNSSBaseline baseline = (GNSSBaseline)this.get(i);
					string id = baseline.Id.ToString();

					Point trgPoint = targetSystem.get(id);
					if (trgPoint == null)
					{
						trgPoint = new Point3D(id,0,0,0);
						targetSystem.add(trgPoint);
					}

					if (baseline.Component == ComponentType.X)
					{
						trgPoint.X = baseline.ValueApriori;
					}
					else if (baseline.Component == ComponentType.Y)
					{
						trgPoint.Y = baseline.ValueApriori;
					}
					else if (baseline.Component == ComponentType.Z)
					{
						trgPoint.Z = baseline.ValueApriori + baseline.EndPointHeight - baseline.StartPointHeight;
					}

					Point srcPoint = sourceSystem.get(id);
					if (srcPoint == null)
					{
						Point pS = baseline.StartPoint;
						Point pE = baseline.EndPoint;
						srcPoint = new Point3D(id,pE.X - pS.X,pE.Y - pS.Y,pE.Z - pS.Z);
						sourceSystem.add(srcPoint);
					}
				}
				int nop = sourceSystem.size();
				DenseMatrix S = new DenseMatrix(3,3);
				double[] q = new double[4];
				for (int k = 0; k < nop; k++)
				{
					Point pS = sourceSystem.get(k);
					Point pT = targetSystem.get(pS.Name);
					if (pT == null)
					{
						continue;
					}

					for (int i = 0; i < 3; i++)
					{
						for (int j = 0; j < 3; j++)
						{
							double a = 0, b = 0;

							if (i == 0)
							{
								a = pS.X;
							}
							else if (i == 1)
							{
								a = pS.Y;
							}
							else
							{
								a = pS.Z;
							}

							if (j == 0)
							{
								b = pT.X;
							}
							else if (j == 1)
							{
								b = pT.Y;
							}
							else
							{
								b = pT.Z;
							}

							S.set(i,j, S.get(i,j) + a * b);
						}
					}
				}
				DenseMatrix N = new DenseMatrix(4,4);
				N.set(0,0, S.get(0,0) + S.get(1,1) + S.get(2,2));
				N.set(0,1, S.get(1,2) - S.get(2,1));
				N.set(0,2, S.get(2,0) - S.get(0,2));
				N.set(0,3, S.get(0,1) - S.get(1,0));

				N.set(1,0, S.get(1,2) - S.get(2,1));
				N.set(1,1, S.get(0,0) - S.get(1,1) - S.get(2,2));
				N.set(1,2, S.get(0,1) + S.get(1,0));
				N.set(1,3, S.get(2,0) + S.get(0,2));

				N.set(2,0, S.get(2,0) - S.get(0,2));
				N.set(2,1, S.get(0,1) + S.get(1,0));
				N.set(2,2,-S.get(0,0) + S.get(1,1) - S.get(2,2));
				N.set(2,3, S.get(1,2) + S.get(2,1));

				N.set(3,0, S.get(0,1) - S.get(1,0));
				N.set(3,1, S.get(2,0) + S.get(0,2));
				N.set(3,2, S.get(1,2) + S.get(2,1));
				N.set(3,3,-S.get(0,0) - S.get(1,1) + S.get(2,2));

				EVD evd = new EVD(4);
				try
				{
					evd.factor(N);

					if (evd.hasRightEigenvectors())
					{
						Matrix eigVec = evd.getRightEigenvectors();
						double[] eigVal = evd.getRealEigenvalues();

						int indexMaxEigVal = 0;
						double maxEigVal = eigVal[indexMaxEigVal];
						for (int i = indexMaxEigVal + 1; i < eigVal.Length; i++)
						{
							if (maxEigVal < eigVal[i])
							{
								maxEigVal = eigVal[i];
								indexMaxEigVal = i;
							}
						}
						// Setze berechnetes Quaternion ein
						for (int i = 0; i < eigVal.Length; i++)
						{
							q[i] = eigVec.get(i, indexMaxEigVal);
						}
					}

				}
				catch (NotConvergedException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}

				// Bestimme Massstab
				double m1 = 0, m2 = 0;
				double q0 = q[0];
				double q1 = q[1];
				double q2 = q[2];
				double q3 = q[3];

				// Bestimme Drehung
				double r13 = 2.0 * (q1 * q3 + q0 * q2);
				double r11 = 2.0 * q0 * q0 - 1.0 + 2.0 * q1 * q1;
				double r12 = 2.0 * (q1 * q2 - q0 * q3);
				double r23 = 2.0 * (q2 * q3 - q0 * q1);
				double r21 = 2.0 * (q1 * q2 + q0 * q3);
				double r22 = 2.0 * q0 * q0 - 1.0 + 2.0 * q2 * q2;
				double r31 = 2.0 * (q1 * q3 - q0 * q2);
				double r32 = 2.0 * (q2 * q3 + q0 * q1);
				double r33 = 2.0 * q0 * q0 - 1.0 + 2.0 * q3 * q3;

				for (int k = 0; k < nop; k++)
				{
					Point pS = sourceSystem.get(k);
					Point pT = targetSystem.get(pS.Name);
					if (pT == null)
					{
						continue;
					}

					m1 += pT.X * (r11 * pS.X + r12 * pS.Y + r13 * pS.Z);
					m1 += pT.Y * (r21 * pS.X + r22 * pS.Y + r23 * pS.Z);
					m1 += pT.Z * (r31 * pS.X + r32 * pS.Y + r33 * pS.Z);

					m2 += pS.X * pS.X;
					m2 += pS.Y * pS.Y;
					m2 += pS.Z * pS.Z;
				}

				if (Math.Abs(m1) > SQRT_EPS && Math.Abs(m2) > SQRT_EPS)
				{
					this.scaleParam = Math.Abs(m1 / m2);
				}
				else
				{
					this.scaleParam = 1.0;
				}

				this.rxParam = MathExtension.MOD(Math.Atan2(-r32, r33), 2.0 * Math.PI);
				this.ryParam = MathExtension.MOD(Math.Asin(r31), 2.0 * Math.PI);
				this.rzParam = MathExtension.MOD(Math.Atan2(-r21, r11), 2.0 * Math.PI);
				this.hasApproxValues = true;
			}

			if (param == this.scale)
			{
				param.Value = this.scaleParam;
			}
			else if (param == this.rx)
			{
				param.Value = this.rxParam;
			}
			else if (param == this.ry)
			{
				param.Value = this.ryParam;
			}
			else if (param == this.rz)
			{
				param.Value = this.rzParam;
			}

			return param;
		}
	}

}