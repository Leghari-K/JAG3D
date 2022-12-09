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

namespace org.applied_geodesy.jag3d.ui.io
{

	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;

	public class DefaultImportOption
	{
		private static readonly Properties PROPERTIES = new Properties();
		private static IDictionary<ObservationType, bool> DEFAULT_SEPARATE_STATION = new IDictionary<ObservationType, bool>
		{
			{ObservationType.LEVELING, false},
			{ObservationType.DIRECTION, true},
			{ObservationType.HORIZONTAL_DISTANCE, false},
			{ObservationType.SLOPE_DISTANCE, false},
			{ObservationType.ZENITH_ANGLE, false},
			{ObservationType.GNSS1D, false},
			{ObservationType.GNSS2D, false},
			{ObservationType.GNSS3D, false}
		};

		static DefaultImportOption()
		{
			BufferedInputStream bis = null;
			const string path = "properties/import.default";
			try
			{
				if (typeof(DefaultImportOption).getClassLoader().getResourceAsStream(path) != null)
				{
					bis = new BufferedInputStream(typeof(DefaultImportOption).getClassLoader().getResourceAsStream(path));
					PROPERTIES.load(bis);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			finally
			{
				try
				{
					if (bis != null)
					{
						bis.close();
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
		}

		public static bool isGroupSeparation(ObservationType observationType)
		{
			bool? value = null;
			switch (observationType.innerEnumValue)
			{
			case ObservationType.InnerEnum.LEVELING:
				try
				{
					value = PROPERTIES.getProperty("LEVELING") == null ? null : bool.Parse(PROPERTIES.getProperty("LEVELING"));
				}
				catch (Exception)
				{
				}
				return value == null ? DEFAULT_SEPARATE_STATION[observationType] : value.Value;
			case ObservationType.InnerEnum.DIRECTION:
				try
				{
					value = PROPERTIES.getProperty("DIRECTION") == null ? null : bool.Parse(PROPERTIES.getProperty("DIRECTION"));
				}
				catch (Exception)
				{
				}
				return value == null ? DEFAULT_SEPARATE_STATION[observationType] : value.Value;
			case ObservationType.InnerEnum.HORIZONTAL_DISTANCE:
				try
				{
					value = PROPERTIES.getProperty("HORIZONTAL_DISTANCE") == null ? null : bool.Parse(PROPERTIES.getProperty("HORIZONTAL_DISTANCE"));
				}
				catch (Exception)
				{
				}
				return value == null ? DEFAULT_SEPARATE_STATION[observationType] : value.Value;
			case ObservationType.InnerEnum.SLOPE_DISTANCE:
				try
				{
					value = PROPERTIES.getProperty("SLOPE_DISTANCE") == null ? null : bool.Parse(PROPERTIES.getProperty("SLOPE_DISTANCE"));
				}
				catch (Exception)
				{
				}
				return value == null ? DEFAULT_SEPARATE_STATION[observationType] : value.Value;
			case ObservationType.InnerEnum.ZENITH_ANGLE:
				try
				{
					value = PROPERTIES.getProperty("ZENITH_ANGLE") == null ? null : bool.Parse(PROPERTIES.getProperty("ZENITH_ANGLE"));
				}
				catch (Exception)
				{
				}
				return value == null ? DEFAULT_SEPARATE_STATION[observationType] : value.Value;
			case ObservationType.InnerEnum.GNSS1D:
				try
				{
					value = PROPERTIES.getProperty("GNSS2D") == null ? null : bool.Parse(PROPERTIES.getProperty("GNSS1D"));
				}
				catch (Exception)
				{
				}
				return value == null ? DEFAULT_SEPARATE_STATION[observationType] : value.Value;
			case ObservationType.InnerEnum.GNSS2D:
				try
				{
					value = PROPERTIES.getProperty("GNSS2D") == null ? null : bool.Parse(PROPERTIES.getProperty("GNSS2D"));
				}
				catch (Exception)
				{
				}
				return value == null ? DEFAULT_SEPARATE_STATION[observationType] : value.Value;
			case ObservationType.InnerEnum.GNSS3D:
				try
				{
					value = PROPERTIES.getProperty("GNSS3D") == null ? null : bool.Parse(PROPERTIES.getProperty("GNSS3D"));
				}
				catch (Exception)
				{
				}
				return value == null ? DEFAULT_SEPARATE_STATION[observationType] : value.Value;
			}
			return false;
		}
	}

}