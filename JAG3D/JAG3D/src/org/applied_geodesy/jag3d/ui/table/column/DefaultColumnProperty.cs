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

namespace org.applied_geodesy.jag3d.ui.table.column
{

	using DefaultUncertainty = org.applied_geodesy.adjustment.network.DefaultUncertainty;

	public class DefaultColumnProperty
	{
		private const double NARROW = 75;
		private const double NORMAL = 100;
		private const double LARGE = 125;
		private const double WIDE = 150;

		private static readonly Properties PROPERTIES = new Properties();

		static DefaultColumnProperty()
		{
			BufferedInputStream bis = null;
			const string path = "properties/tablecolumns.default";
			try
			{
				if (typeof(DefaultUncertainty).getClassLoader().getResourceAsStream(path) != null)
				{
					bis = new BufferedInputStream(typeof(DefaultUncertainty).getClassLoader().getResourceAsStream(path));
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

		private DefaultColumnProperty()
		{
		}

		public static double getPrefWidth(ColumnContentType columnContentType)
		{
			double value = -1;
			switch (columnContentType.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.ENABLE:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.CODE:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.INSTRUMENT_HEIGHT:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.REFLECTOR_HEIGHT:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.OMEGA:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.VARIANCE:

			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.REDUNDANCY:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.REDUNDANCY_X:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.REDUNDANCY_Y:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.REDUNDANCY_Z:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.NUMBER_OF_OBSERVATION:

			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.P_VALUE_APRIORI:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.P_VALUE_APOSTERIORI:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.TEST_STATISTIC_APRIORI:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.TEST_STATISTIC_APOSTERIORI:
				value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty(columnContentType.ToString()));
				}
				catch (Exception)
				{
				}
				return value > 0 ? Math.Max(ColumnProperty.MIN_WIDTH, value) : NARROW;

			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.INFLUENCE_ON_POINT_POSITION:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.INFLUENCE_ON_POINT_POSITION_X:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.INFLUENCE_ON_POINT_POSITION_Y:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.INFLUENCE_ON_POINT_POSITION_Z:

			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.FIRST_PRINCIPLE_COMPONENT_X:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.FIRST_PRINCIPLE_COMPONENT_Y:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.FIRST_PRINCIPLE_COMPONENT_Z:

			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.CONFIDENCE_ALPHA:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.CONFIDENCE_BETA:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.CONFIDENCE_GAMMA:

			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.CONFIDENCE_A:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.CONFIDENCE_B:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.CONFIDENCE_C:

			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.RESIDUAL:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.RESIDUAL_X:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.RESIDUAL_Y:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.RESIDUAL_Z:

			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.GROSS_ERROR:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.GROSS_ERROR_X:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.GROSS_ERROR_Y:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.GROSS_ERROR_Z:

			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.MINIMAL_DETECTABLE_BIAS:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.MINIMAL_DETECTABLE_BIAS_X:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.MINIMAL_DETECTABLE_BIAS_Y:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.MINIMAL_DETECTABLE_BIAS_Z:

			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.MAXIMUM_TOLERABLE_BIAS:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.MAXIMUM_TOLERABLE_BIAS_X:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.MAXIMUM_TOLERABLE_BIAS_Y:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.MAXIMUM_TOLERABLE_BIAS_Z:

			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.UNCERTAINTY_APRIORI:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.UNCERTAINTY_APOSTERIORI:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.UNCERTAINTY_X_APRIORI:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.UNCERTAINTY_X_APOSTERIORI:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.UNCERTAINTY_Y_APRIORI:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.UNCERTAINTY_Y_APOSTERIORI:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.UNCERTAINTY_Z_APRIORI:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.UNCERTAINTY_Z_APOSTERIORI:

			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.INFLUENCE_ON_NETWORK_DISTORTION:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.APPROXIMATED_DISTANCE_APRIORI:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.SIGNIFICANT:
				value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty(columnContentType.ToString()));
				}
				catch (Exception)
				{
				}
				return value > 0 ? Math.Max(ColumnProperty.MIN_WIDTH, value) : NORMAL;

			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.POINT_NAME:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.START_POINT_NAME:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.END_POINT_NAME:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.VARIANCE_COMPONENT_NAME:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.VARIANCE_COMPONENT_TYPE:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.PARAMETER_NAME:
				value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty(columnContentType.ToString()));
				}
				catch (Exception)
				{
				}
				return value > 0 ? Math.Max(ColumnProperty.MIN_WIDTH, value) : WIDE;


			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.VALUE_APRIORI:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.VALUE_APOSTERIORI:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.VALUE_X_APRIORI:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.VALUE_X_APOSTERIORI:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.VALUE_Y_APRIORI:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.VALUE_Y_APOSTERIORI:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.VALUE_Z_APRIORI:
			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.VALUE_Z_APOSTERIORI:
				value = -1;
				try
				{
					value = double.Parse(PROPERTIES.getProperty(columnContentType.ToString()));
				}
				catch (Exception)
				{
				}
				return value > 0 ? Math.Max(ColumnProperty.MIN_WIDTH, value) : LARGE;

			case org.applied_geodesy.jag3d.ui.table.column.ColumnContentType.InnerEnum.DEFAULT:
				return ColumnProperty.PREFERRED_WIDTH;
			}
			return ColumnProperty.PREFERRED_WIDTH;
		}
	}

}