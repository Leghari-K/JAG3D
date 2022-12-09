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

namespace org.applied_geodesy.adjustment.network
{
	public class RankDefect
	{
		private bool userDefinedRankDefect = false;

		private DefectType rx = DefectType.NOT_SET, ry = DefectType.NOT_SET, rz = DefectType.NOT_SET, tx = DefectType.NOT_SET, ty = DefectType.NOT_SET, tz = DefectType.NOT_SET, sx = DefectType.NOT_SET, sy = DefectType.NOT_SET, sz = DefectType.NOT_SET, mx = DefectType.NOT_SET, my = DefectType.NOT_SET, mz = DefectType.NOT_SET, mxy = DefectType.NOT_SET, mxyz = DefectType.NOT_SET;

		public RankDefect()
		{
		}

		internal virtual DefectType ScaleXYZ
		{
			set
			{
				this.mx = this.my = this.mz = this.mxy = DefectType.NOT_SET;
				this.mxyz = value == DefectType.FIXED?DefectType.FIXED:DefectType.FREE;
			}
			get
			{
				return this.mxyz;
			}
		}

		internal virtual DefectType ScaleXY
		{
			set
			{
				if (this.mxyz != DefectType.NOT_SET)
				{
					this.mx = this.my = this.mz = this.mxy = DefectType.NOT_SET;
				}
				else
				{
					this.mxy = value == DefectType.FIXED?DefectType.FIXED:DefectType.FREE;
				}
			}
			get
			{
				return this.mxy;
			}
		}

		internal virtual DefectType ScaleX
		{
			set
			{
				if (this.mxyz != DefectType.NOT_SET)
				{
					this.mx = this.my = this.mz = this.mxy = DefectType.NOT_SET;
				}
				else if (this.mxy != DefectType.NOT_SET)
				{
					this.mx = this.my = DefectType.NOT_SET;
				}
				else
				{
					this.mx = value == DefectType.FIXED?DefectType.FIXED:DefectType.FREE;
				}
			}
			get
			{
				return this.mx;
			}
		}

		internal virtual DefectType ScaleY
		{
			set
			{
				if (this.mxyz != DefectType.NOT_SET)
				{
					this.mx = this.my = this.mz = this.mxy = DefectType.NOT_SET;
				}
				else if (this.mxy != DefectType.NOT_SET)
				{
					this.mx = this.my = DefectType.NOT_SET;
				}
				else
				{
					this.my = value == DefectType.FIXED?DefectType.FIXED:DefectType.FREE;
				}
			}
			get
			{
				return this.my;
			}
		}

		internal virtual DefectType ScaleZ
		{
			set
			{
				if (this.mxyz != DefectType.NOT_SET)
				{
					this.mx = this.my = this.mz = this.mxy = DefectType.NOT_SET;
				}
				else
				{
					this.mz = value == DefectType.FIXED?DefectType.FIXED:DefectType.FREE;
				}
			}
			get
			{
				return this.mz;
			}
		}

		internal virtual DefectType ShearX
		{
			set
			{
				this.sx = value == DefectType.FIXED?DefectType.FIXED:DefectType.FREE;
			}
			get
			{
				return this.sx;
			}
		}

		internal virtual DefectType ShearY
		{
			set
			{
				this.sy = value == DefectType.FIXED?DefectType.FIXED:DefectType.FREE;
			}
			get
			{
				return this.sy;
			}
		}

		internal virtual DefectType ShearZ
		{
			set
			{
				this.sz = value == DefectType.FIXED?DefectType.FIXED:DefectType.FREE;
			}
			get
			{
				return this.sz;
			}
		}

		internal virtual DefectType RotationX
		{
			set
			{
				this.rx = value == DefectType.FIXED?DefectType.FIXED:DefectType.FREE;
			}
			get
			{
				return this.rx;
			}
		}

		internal virtual DefectType RotationY
		{
			set
			{
				this.ry = value == DefectType.FIXED?DefectType.FIXED:DefectType.FREE;
			}
			get
			{
				return this.ry;
			}
		}

		internal virtual DefectType RotationZ
		{
			set
			{
				this.rz = value == DefectType.FIXED?DefectType.FIXED:DefectType.FREE;
			}
			get
			{
				return this.rz;
			}
		}

		internal virtual DefectType TranslationX
		{
			set
			{
				this.tx = value == DefectType.FIXED?DefectType.FIXED:DefectType.FREE;
			}
			get
			{
				return this.tx;
			}
		}

		internal virtual DefectType TranslationY
		{
			set
			{
				this.ty = value == DefectType.FIXED?DefectType.FIXED:DefectType.FREE;
			}
			get
			{
				return this.ty;
			}
		}

		internal virtual DefectType TranslationZ
		{
			set
			{
				this.tz = value == DefectType.FIXED?DefectType.FIXED:DefectType.FREE;
			}
			get
			{
				return this.tz;
			}
		}

		public virtual DefectType ScaleXYZDefectType
		{
			set
			{
				this.userDefinedRankDefect = true;
				this.ScaleXYZ = value;
			}
		}

		public virtual DefectType ScaleXYDefectType
		{
			set
			{
				this.userDefinedRankDefect = true;
				this.ScaleXY = value;
			}
		}

		public virtual DefectType ScaleXDefectType
		{
			set
			{
				this.userDefinedRankDefect = true;
				this.ScaleX = value;
			}
		}

		public virtual DefectType ScaleYDefectType
		{
			set
			{
				this.userDefinedRankDefect = true;
				this.ScaleY = value;
			}
		}

		public virtual DefectType ScaleZDefectType
		{
			set
			{
				this.userDefinedRankDefect = true;
				this.ScaleZ = value;
			}
		}

		public virtual DefectType ShearXDefectType
		{
			set
			{
				this.userDefinedRankDefect = true;
				this.ShearX = value;
			}
		}

		public virtual DefectType ShearYDefectType
		{
			set
			{
				this.userDefinedRankDefect = true;
				this.ShearY = value;
			}
		}

		public virtual DefectType ShearZDefectType
		{
			set
			{
				this.userDefinedRankDefect = true;
				this.ShearZ = value;
			}
		}

		public virtual DefectType RotationXDefectType
		{
			set
			{
				this.userDefinedRankDefect = true;
				this.RotationX = value;
			}
		}

		public virtual DefectType RotationYDefectType
		{
			set
			{
				this.userDefinedRankDefect = true;
				this.RotationY = value;
			}
		}

		public virtual DefectType RotationZDefectType
		{
			set
			{
				this.userDefinedRankDefect = true;
				this.RotationZ = value;
			}
		}

		public virtual DefectType TranslationXDefectType
		{
			set
			{
				this.userDefinedRankDefect = true;
				this.TranslationX = value;
			}
		}

		public virtual DefectType TranslationYDefectType
		{
			set
			{
				this.userDefinedRankDefect = true;
				this.TranslationY = value;
			}
		}

		public virtual DefectType TranslationZDefectType
		{
			set
			{
				this.userDefinedRankDefect = true;
				this.TranslationZ = value;
			}
		}

		public virtual bool estimateScaleX()
		{
			return this.mx == DefectType.FREE;
		}

		public virtual bool estimateScaleY()
		{
			return this.my == DefectType.FREE;
		}

		public virtual bool estimateScaleZ()
		{
			return this.mz == DefectType.FREE;
		}

		public virtual bool estimateScaleXY()
		{
			return this.mxy == DefectType.FREE;
		}

		public virtual bool estimateScaleXYZ()
		{
			return this.mxyz == DefectType.FREE;
		}

		public virtual bool estimateShearX()
		{
			return this.sx == DefectType.FREE;
		}

		public virtual bool estimateShearY()
		{
			return this.sy == DefectType.FREE;
		}

		public virtual bool estimateShearZ()
		{
			return this.sz == DefectType.FREE;
		}

		public virtual bool estimateRotationX()
		{
			return this.rx == DefectType.FREE;
		}

		public virtual bool estimateRotationY()
		{
			return this.ry == DefectType.FREE;
		}

		public virtual bool estimateRotationZ()
		{
			return this.rz == DefectType.FREE;
		}

		public virtual bool estimateTranslationX()
		{
			return this.tx == DefectType.FREE;
		}

		public virtual bool estimateTranslationY()
		{
			return this.ty == DefectType.FREE;
		}

		public virtual bool estimateTranslationZ()
		{
			return this.tz == DefectType.FREE;
		}















		public virtual int Defect
		{
			get
			{
				int d = 0;
				d += this.estimateScaleX()?1:0;
				d += this.estimateScaleY()?1:0;
				d += this.estimateScaleZ()?1:0;
				d += this.estimateScaleXY()?1:0;
				d += this.estimateScaleXYZ()?1:0;
				d += this.estimateRotationX()?1:0;
				d += this.estimateRotationY()?1:0;
				d += this.estimateRotationZ()?1:0;
				d += this.estimateShearX()?1:0;
				d += this.estimateShearY()?1:0;
				d += this.estimateShearZ()?1:0;
				d += this.estimateTranslationX()?1:0;
				d += this.estimateTranslationY()?1:0;
				d += this.estimateTranslationZ()?1:0;
    
				return d;
			}
		}

		public virtual bool UserDefinedRankDefect
		{
			get
			{
				return this.userDefinedRankDefect;
			}
		}

		public virtual void reset()
		{
			this.userDefinedRankDefect = false;
			this.tx = this.ty = this.tz = DefectType.NOT_SET;
			this.rx = this.ry = this.rz = DefectType.NOT_SET;
			this.sx = this.sy = this.sz = DefectType.NOT_SET;
			this.mx = this.my = this.mz = DefectType.NOT_SET;
			this.mxy = this.mxyz = DefectType.NOT_SET;
		}

		public override string ToString()
		{
			return "RankDefect [rx=" + rx + ", ry=" + ry + ", rz=" + rz + ", tx=" + tx + ", ty=" + ty + ", tz=" + tz + ", sx=" + sx + ", sy=" + sy + ", sz=" + sz + ", mx=" + mx + ", my=" + my + ", mz=" + mz + ", mxy=" + mxy + ", mxyz=" + mxyz + ", defect=" + Defect + ", userDefinedRankDefect=" + userDefinedRankDefect + "]";
		}


	}

}