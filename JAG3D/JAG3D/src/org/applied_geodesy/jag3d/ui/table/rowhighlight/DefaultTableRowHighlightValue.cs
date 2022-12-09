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

namespace org.applied_geodesy.jag3d.ui.table.rowhighlight
{

	using Color = javafx.scene.paint.Color;

	public class DefaultTableRowHighlightValue
	{
		private static readonly double[] REDUNDANCY = new double[] {0.3, 0.7}; // 30 % - 70 %
		private static readonly double[] P_PRIO_VALUE = new double[] {0.01, 0.05}; //  1 % - 5 %
		private static readonly double[] INFLUENCE_ON_POSITION = new double[] {0.001, 0.005}; // 1 mm - 5 mm

		private static readonly Color EXCELLENT_COLOR = Color.web("#bcee68");
		private static readonly Color SATISFACTORY_COLOR = Color.web("#ffec8b");
		private static readonly Color INADEQUATE_COLOR = Color.web("#ff3030");

		private static readonly Properties PROPERTIES = new Properties();

		static DefaultTableRowHighlightValue()
		{
			BufferedInputStream bis = null;
			const string path = "properties/tablerowhighlight.default";
			try
			{
				if (typeof(DefaultTableRowHighlightValue).getClassLoader().getResourceAsStream(path) != null)
				{
					bis = new BufferedInputStream(typeof(DefaultTableRowHighlightValue).getClassLoader().getResourceAsStream(path));
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

		private DefaultTableRowHighlightValue()
		{
		}

		public static Color ExcellentColor
		{
			get
			{
				try
				{
					return Color.web(PROPERTIES.getProperty("EXCELLENT_COLOR", "#bcee68"));
				}
				catch (Exception)
				{
					return EXCELLENT_COLOR;
				}
			}
		}

		public static Color SatisfactoryColor
		{
			get
			{
				try
				{
					return Color.web(PROPERTIES.getProperty("SATISFACTORY_COLOR", "#ffec8b"));
				}
				catch (Exception)
				{
					return SATISFACTORY_COLOR;
				}
			}
		}

		public static Color InadequateColor
		{
			get
			{
				try
				{
					return Color.web(PROPERTIES.getProperty("INADEQUATE_COLOR", "#ff3030"));
				}
				catch (Exception)
				{
					return INADEQUATE_COLOR;
				}
			}
		}

		public static double[] RangeOfRedundancy
		{
			get
			{
				double leftBoundary = -1;
				double rightBoundary = -1;
				try
				{
					leftBoundary = double.Parse(PROPERTIES.getProperty("REDUNDANCY_LEFT_BOUNDARY"));
					rightBoundary = double.Parse(PROPERTIES.getProperty("REDUNDANCY_RIGHT_BOUNDARY"));
				}
				catch (Exception)
				{
				}
    
				return leftBoundary >= 0 && rightBoundary >= 0 && leftBoundary <= 1 && rightBoundary <= 1 && leftBoundary < rightBoundary ? new double[] {leftBoundary, rightBoundary} : new double[] {REDUNDANCY[0], REDUNDANCY[1]};
			}
		}

		public static double[] RangeOfPprioValue
		{
			get
			{
				double leftBoundary = -1;
				double rightBoundary = -1;
				try
				{
					leftBoundary = double.Parse(PROPERTIES.getProperty("P_PRIO_VALUE_LEFT_BOUNDARY"));
					rightBoundary = double.Parse(PROPERTIES.getProperty("P_PRIO_VALUE_RIGHT_BOUNDARY"));
				}
				catch (Exception)
				{
				}
    
				return leftBoundary > 0 && rightBoundary > 0 && leftBoundary < 1 && rightBoundary < 1 && leftBoundary < rightBoundary ? new double[] {leftBoundary, rightBoundary} : new double[] {P_PRIO_VALUE[0], P_PRIO_VALUE[1]};
			}
		}

		public static double[] RangeOfInfluenceOnPosition
		{
			get
			{
				double leftBoundary = -1;
				double rightBoundary = -1;
				try
				{
					leftBoundary = double.Parse(PROPERTIES.getProperty("INFLUENCE_ON_POSITION_LEFT_BOUNDARY"));
					rightBoundary = double.Parse(PROPERTIES.getProperty("INFLUENCE_ON_POSITION_RIGHT_BOUNDARY"));
				}
				catch (Exception)
				{
				}
    
				return leftBoundary >= 0 && rightBoundary >= 0 && leftBoundary <= rightBoundary ? new double[] {leftBoundary, rightBoundary} : new double[] {INFLUENCE_ON_POSITION[0], INFLUENCE_ON_POSITION[1]};
			}
		}

		public static double[] getRange(TableRowHighlightType type)
		{
			switch (type.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType.InnerEnum.INFLUENCE_ON_POSITION:
				return RangeOfInfluenceOnPosition;

			case org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType.InnerEnum.P_PRIO_VALUE:
				return RangeOfPprioValue;

			case org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType.InnerEnum.REDUNDANCY:
				return RangeOfRedundancy;

			case org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType.InnerEnum.NONE:
			case org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType.InnerEnum.TEST_STATISTIC:
			case org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType.InnerEnum.GROSS_ERROR:
				return null;
			}

			return null;
		}

		public static Color getColor(TableRowHighlightRangeType type)
		{
			switch (type.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightRangeType.InnerEnum.EXCELLENT:
				return ExcellentColor;

			case org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightRangeType.InnerEnum.SATISFACTORY:
				return SatisfactoryColor;

			case org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightRangeType.InnerEnum.INADEQUATE:
				return InadequateColor;

			case org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightRangeType.InnerEnum.NONE:
				return Color.TRANSPARENT;
			}

			return Color.TRANSPARENT;
		}
	}

}