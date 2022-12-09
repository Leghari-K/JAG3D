using System;
using System.Collections.Generic;

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

namespace org.applied_geodesy.adjustment.network.approximation.bundle.transformation
{

	using PointBundle = org.applied_geodesy.adjustment.network.approximation.bundle.PointBundle;
	using Point = org.applied_geodesy.adjustment.network.approximation.bundle.point.Point;
	using Point1D = org.applied_geodesy.adjustment.network.approximation.bundle.point.Point1D;

	public class Transformation1D : Transformation
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			source = new PointBundle(this.Dimension);
			target = new PointBundle(this.Dimension);
			originalTarget = new PointBundle(this.Dimension);
			pointsToTransform = new PointBundle(this.Dimension);
		}

		public PointBundle source, target, originalTarget, pointsToTransform;
		public TransformationParameterSet transParameter = null;
		private double omega = 0.0;
		private IDictionary<TransformationParameterType, bool> fixedParameters = new IDictionary<TransformationParameterType, bool>
		{
			{TransformationParameterType.TRANSLATION_X, true},
			{TransformationParameterType.TRANSLATION_Y, true},
			{TransformationParameterType.TRANSLATION_Z, false},
			{TransformationParameterType.ROTATION_X, true},
			{TransformationParameterType.ROTATION_Y, true},
			{TransformationParameterType.ROTATION_Z, true},
			{TransformationParameterType.SCALE, true}
		};

		public Transformation1D(TransformationParameterSet transParameter)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.transParameter = transParameter;
		}

		public Transformation1D(PointBundle source, PointBundle target)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			if (source.Dimension != target.Dimension || source.Dimension != this.numberOfRequiredPoints())
			{
				throw new System.ArgumentException(this.GetType().Name + " Dimensionsfehler, benoetige d = " + this.Dimension);
			}
			this.init(source, target);
		}

		public virtual bool transformLMS()
		{
			TransformationParameterSet transParameter = null;
			double omega = double.MaxValue;
			for (int i = 0; i < this.numberOfIdenticalPoints(); i++)
			{
				Point pS1 = this.source.get(i);
				Point pT1 = this.target.get(pS1.Name);
				if (this.fixedParameters[TransformationParameterType.SCALE])
				{
					PointBundle subSource = new PointBundle(this.Dimension);
					PointBundle subTarget = new PointBundle(this.Dimension);

					subSource.addPoint(pS1);
					subTarget.addPoint(pT1);

					TransformationParameterSet subParameter = this.adjustTransformationsParameter(subSource, subTarget);

					double o = this.getOmega(subParameter, this.source, this.target, true);

					if (o < omega)
					{
						transParameter = subParameter;
						omega = o;
					}
				}
				else
				{
					for (int j = i + 1; j < this.numberOfIdenticalPoints(); j++)
					{
						Point pS2 = this.source.get(j);
						Point pT2 = this.target.get(pS2.Name);

						PointBundle subSource = new PointBundle(this.Dimension);
						PointBundle subTarget = new PointBundle(this.Dimension);

						subSource.addPoint(pS1);
						subSource.addPoint(pS2);

						subTarget.addPoint(pT1);
						subTarget.addPoint(pT2);

						TransformationParameterSet subParameter = this.adjustTransformationsParameter(subSource, subTarget);

						double o = this.getOmega(subParameter, this.source, this.target, true);

						if (o < omega)
						{
							transParameter = subParameter;
							omega = o;
						}
					}
				}

			}
			this.omega = omega;
			this.transParameter = transParameter;
			return this.transParameter != null;
		}

		private double getOmega(TransformationParameterSet transParameter, PointBundle source, PointBundle target, bool isLMS)
		{
			if (transParameter == null)
			{
				return double.MaxValue;
			}

			double m = transParameter.getParameterValue(TransformationParameterType.SCALE);
			double tZ = transParameter.getParameterValue(TransformationParameterType.TRANSLATION_Z);

			double[] omegaLMS = new double[source.size()];
			double omegaLS = 0.0;

			for (int i = 0; i < source.size(); i++)
			{
				Point pS = source.get(i);
				Point pT = target.get(pS.Name);

				double z = m * pS.Z + tZ;
				double vz = pT.Z - z;

				omegaLMS[i] = vz * vz;
				omegaLS += vz * vz;
			}
			Array.Sort(omegaLMS);

			if (isLMS)
			{
				Array.Sort(omegaLMS);
				if (omegaLMS.Length == this.numberOfRequiredPoints())
				{
					return omegaLMS[omegaLMS.Length - 1];
				}
				if (omegaLMS.Length < 2 * this.numberOfRequiredPoints())
				{
					return omegaLMS[this.numberOfRequiredPoints()];
				}
				if (omegaLMS.Length % 2 == 1)
				{
					return omegaLMS[(int)(omegaLMS.Length / 2)];
				}
				return 0.5 * (omegaLMS[(int)(omegaLMS.Length / 2) - 1] + omegaLMS[(int)(omegaLMS.Length / 2)]);
			}
			return omegaLS;
		}

		public virtual bool transformL2Norm()
		{
			if (this.numberOfIdenticalPoints() < this.numberOfRequiredPoints())
			{
				return false;
			}

			this.transParameter = null;
			//if (this.transParameter != null)
			//	return true;

			this.transParameter = this.adjustTransformationsParameter(this.source, this.target);
			this.omega = this.getOmega(this.transParameter, this.source, this.target, false);

			return this.transParameter != null;
		}

		public virtual TransformationParameterSet adjustTransformationsParameter(PointBundle source, PointBundle target)
		{

			Point centerPointS = source.CenterPoint;
			Point centerPointT = target.CenterPoint;

			double m = 0;
			int k = 0;
			for (int i = 0; i < source.size(); i++)
			{
				Point pS = source.get(i);
				Point pT = target.get(i);
				if ((pS.Z - centerPointS.Z) != 0)
				{
					m += Math.Abs((pT.Z - centerPointT.Z) / (pS.Z - centerPointS.Z));
					k++;
				}
			}
			if (!this.fixedParameters[TransformationParameterType.SCALE] && m > 0 && k > 0)
			{
				m /= k;
			}
			else
			{
				m = 1.0;
			}

			double tZ = 0.0;
			if (!this.fixedParameters[TransformationParameterType.TRANSLATION_Z])
			{
				tZ = centerPointT.Z - m * centerPointS.Z;
			}

			TransformationParameterSet transParameter = new TransformationParameterSet();
			if (!this.fixedParameters[TransformationParameterType.TRANSLATION_Z])
			{
				transParameter.setParameterValue(TransformationParameterType.TRANSLATION_Z, tZ);
			}
			if (!this.fixedParameters[TransformationParameterType.SCALE])
			{
				transParameter.setParameterValue(TransformationParameterType.SCALE, m);
			}
			return transParameter;
		}

		private void init(PointBundle source, PointBundle target)
		{
			this.originalTarget = target;
			for (int i = 0; i < source.size(); i++)
			{
				Point pS = source.get(i);
				Point tempPS = new Point1D(pS.Name, pS.Z);

				this.pointsToTransform.addPoint(tempPS);

				for (int j = 0; j < target.size(); j++)
				{
					Point pT = target.get(j);

					// Suche identische Punkte
					if (pS.Name.Equals(pT.Name))
					{
						Point tempPT = new Point1D(pT.Name, pT.Z);
						this.source.addPoint(tempPS);
						this.target.addPoint(tempPT);
						break;
					}
				}
			}
		}

		public virtual Point1D transformPoint2SourceSystem(Point point)
		{
			if (this.transParameter == null)
			{
				return null;
			}

			double m = this.transParameter.getParameterValue(TransformationParameterType.SCALE);
			double tZ = this.transParameter.getParameterValue(TransformationParameterType.TRANSLATION_Z);

			double z = (point.Z - tZ) / m;

			return new Point1D(point.Name, z);
		}

		public virtual Point1D transformPoint2TargetSystem(Point point)
		{
			if (this.transParameter == null || point == null)
			{
				return null;
			}

			double m = this.transParameter.getParameterValue(TransformationParameterType.SCALE);
			double tZ = this.transParameter.getParameterValue(TransformationParameterType.TRANSLATION_Z);

			double z = m * point.Z + tZ;

			return new Point1D(point.Name, z);
		}

		public int Dimension
		{
			get
			{
				return 1;
			}
		}

		public virtual int numberOfIdenticalPoints()
		{
			return this.source.size();
		}

		public virtual int numberOfRequiredPoints()
		{
			return 1;
		}

		public virtual PointBundle TransformdPoints
		{
			get
			{
				if (this.transParameter == null)
				{
					return null;
				}
    
				PointBundle transformedPoints = new PointBundle(this.Dimension);
    
				for (int i = 0; i < this.originalTarget.size(); i++)
				{
					Point pT = this.originalTarget.get(i);
					transformedPoints.addPoint(pT);
				}
    
				for (int i = 0; i < this.pointsToTransform.size(); i++)
				{
					Point pS = this.pointsToTransform.get(i);
					Point pT = transformedPoints.get(pS.Name);
					if (pT == null)
					{
						pT = this.transformPoint2TargetSystem(pS);
						transformedPoints.addPoint(pT);
					}
				}
				transformedPoints.TransformationParameterSet = this.transParameter;
				return transformedPoints;
			}
		}

		public virtual TransformationParameterSet TransformationParameterSet
		{
			get
			{
				return this.transParameter;
			}
		}

		public virtual void setFixedParameter(TransformationParameterType type, bool @fixed)
		{
			this.fixedParameters[type] = @fixed;
		}

		public virtual double Omega
		{
			get
			{
				return this.omega;
			}
		}
	}

}