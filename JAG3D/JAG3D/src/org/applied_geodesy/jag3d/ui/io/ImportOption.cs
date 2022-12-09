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

namespace org.applied_geodesy.jag3d.ui.io
{

	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;

	public class ImportOption
	{

		private static ImportOption importOption = new ImportOption();

		private IDictionary<ObservationType, bool> separateStation = new Dictionary<ObservationType, bool>();

		private ImportOption()
		{
		}

		public static ImportOption Instance
		{
			get
			{
				return importOption;
			}
		}

		public virtual bool isGroupSeparation(ObservationType observationType)
		{
			if (this.separateStation.ContainsKey(observationType))
			{
				return this.separateStation[observationType];
			}
			return DefaultImportOption.isGroupSeparation(observationType);
		}

		public virtual void setGroupSeparation(ObservationType observationType, bool separate)
		{
			this.separateStation[observationType] = separate;
		}
	}

}