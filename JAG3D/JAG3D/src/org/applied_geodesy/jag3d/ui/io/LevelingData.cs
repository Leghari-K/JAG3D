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

namespace org.applied_geodesy.jag3d.ui.io
{
	internal class LevelingData
	{
		private double? r1 = null, r2 = null, dr1 = null, dr2 = null;
		private double? v1 = null, v2 = null, dv1 = null, dv2 = null;
		private string startPointName = null, endPointName = null;

		public virtual void addBackSightReading(string name, double r, double d)
		{
			this.addBackSightReading(name, r, d, this.r1 == null);
		}

		public virtual void addForeSightReading(string name, double v, double d)
		{
			this.addForeSightReading(name, v, d, this.v1 == null);
		}

		public virtual void addForeSightReading(string name, double v, double d, bool isFirstForeSightReading)
		{
			if (isFirstForeSightReading)
			{
				this.endPointName = name;
				this.v1 = v;
				this.dv1 = d;
			}
			else if (!isFirstForeSightReading && (string.ReferenceEquals(name, null) || this.endPointName.Equals(name)))
			{
				this.v2 = v;
				this.dv2 = d;
			}
		}

		public virtual void addBackSightReading(string name, double r, double d, bool isFirstBackSightReading)
		{
			if (isFirstBackSightReading)
			{
				this.startPointName = name;
				this.r1 = r;
				this.dr1 = d;
			}
			else if (!isFirstBackSightReading && (string.ReferenceEquals(name, null) || this.startPointName.Equals(name)))
			{
				this.r2 = r;
				this.dr2 = d;
			}
		}

		public virtual double Distance
		{
			get
			{
				double dr = 0;
				double dv = 0;
    
				if (this.dr1 != null && this.dr2 != null && this.dr1 > 0 && this.dr2 > 0)
				{
					dr = 0.5 * (this.dr1.Value + this.dr2.Value);
				}
				else if (this.dr1 != null && this.dr1 > 0)
				{
					dr = this.dr1.Value;
				}
    
				if (this.dv1 != null && this.dv2 != null && this.dv1 > 0 && this.dv2 > 0)
				{
					dv = 0.5 * (this.dv1.Value + this.dv2.Value);
				}
				else if (this.dv1 != null && this.dv1 > 0)
				{
					dv = this.dv1.Value;
				}
    
				return dr + dv;
			}
		}

		public virtual double DeltaH
		{
			get
			{
				double r = 0;
				double v = 0;
    
				if (this.r1 != null && this.r2 != null)
				{
					r = 0.5 * (this.r1.Value + this.r2.Value);
				}
				else if (this.r1 != null)
				{
					r = this.r1.Value;
				}
    
				if (this.v1 != null && this.v2 != null)
				{
					v = 0.5 * (this.v1.Value + this.v2.Value);
				}
				else if (this.v1 != null)
				{
					v = this.v1.Value;
				}
    
				return r - v;
			}
		}

		public virtual string StartPointName
		{
			get
			{
				return this.startPointName;
			}
		}

		public virtual string EndPointName
		{
			get
			{
				return this.endPointName;
			}
		}

		public override string ToString()
		{
			return "LevelingData [startPointName=" + startPointName + " - endPointName=" + endPointName + ", r1=" + r1 + ", r2=" + r2 + ", dr1=" + dr1 + ", dr2=" + dr2 + ", v1=" + v1 + ", v2=" + v2 + ", dv1=" + dv1 + ", dv2=" + dv2 + "]";
		}

		public virtual bool hasFirstBackSightReading()
		{
			return this.r1 != null;
		}

		public virtual bool hasSecondBackSightReading()
		{
			return this.r2 != null;
		}

		public virtual bool hasFirstForeSightReading()
		{
			return this.v1 != null;
		}

		public virtual bool hasSecondForeSightReading()
		{
			return this.v2 != null;
		}
	}

}