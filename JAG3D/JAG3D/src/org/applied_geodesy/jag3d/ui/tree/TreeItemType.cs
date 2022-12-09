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

namespace org.applied_geodesy.jag3d.ui.tree
{
	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;
	using PointType = org.applied_geodesy.adjustment.network.PointType;
	using VerticalDeflectionType = org.applied_geodesy.adjustment.network.VerticalDeflectionType;

	public sealed class TreeItemType
	{
		public static readonly TreeItemType ROOT = new TreeItemType("ROOT", InnerEnum.ROOT);

		public static readonly TreeItemType UNSPECIFIC = new TreeItemType("UNSPECIFIC", InnerEnum.UNSPECIFIC);

		/// <summary>
		/// Points * </summary>
		public static readonly TreeItemType REFERENCE_POINT_1D_DIRECTORY = new TreeItemType("REFERENCE_POINT_1D_DIRECTORY", InnerEnum.REFERENCE_POINT_1D_DIRECTORY);
		public static readonly TreeItemType STOCHASTIC_POINT_1D_DIRECTORY = new TreeItemType("STOCHASTIC_POINT_1D_DIRECTORY", InnerEnum.STOCHASTIC_POINT_1D_DIRECTORY);
		public static readonly TreeItemType DATUM_POINT_1D_DIRECTORY = new TreeItemType("DATUM_POINT_1D_DIRECTORY", InnerEnum.DATUM_POINT_1D_DIRECTORY);
		public static readonly TreeItemType NEW_POINT_1D_DIRECTORY = new TreeItemType("NEW_POINT_1D_DIRECTORY", InnerEnum.NEW_POINT_1D_DIRECTORY);

		public static readonly TreeItemType REFERENCE_POINT_2D_DIRECTORY = new TreeItemType("REFERENCE_POINT_2D_DIRECTORY", InnerEnum.REFERENCE_POINT_2D_DIRECTORY);
		public static readonly TreeItemType STOCHASTIC_POINT_2D_DIRECTORY = new TreeItemType("STOCHASTIC_POINT_2D_DIRECTORY", InnerEnum.STOCHASTIC_POINT_2D_DIRECTORY);
		public static readonly TreeItemType DATUM_POINT_2D_DIRECTORY = new TreeItemType("DATUM_POINT_2D_DIRECTORY", InnerEnum.DATUM_POINT_2D_DIRECTORY);
		public static readonly TreeItemType NEW_POINT_2D_DIRECTORY = new TreeItemType("NEW_POINT_2D_DIRECTORY", InnerEnum.NEW_POINT_2D_DIRECTORY);

		public static readonly TreeItemType REFERENCE_POINT_3D_DIRECTORY = new TreeItemType("REFERENCE_POINT_3D_DIRECTORY", InnerEnum.REFERENCE_POINT_3D_DIRECTORY);
		public static readonly TreeItemType STOCHASTIC_POINT_3D_DIRECTORY = new TreeItemType("STOCHASTIC_POINT_3D_DIRECTORY", InnerEnum.STOCHASTIC_POINT_3D_DIRECTORY);
		public static readonly TreeItemType DATUM_POINT_3D_DIRECTORY = new TreeItemType("DATUM_POINT_3D_DIRECTORY", InnerEnum.DATUM_POINT_3D_DIRECTORY);
		public static readonly TreeItemType NEW_POINT_3D_DIRECTORY = new TreeItemType("NEW_POINT_3D_DIRECTORY", InnerEnum.NEW_POINT_3D_DIRECTORY);

		public static readonly TreeItemType REFERENCE_POINT_1D_LEAF = new TreeItemType("REFERENCE_POINT_1D_LEAF", InnerEnum.REFERENCE_POINT_1D_LEAF);
		public static readonly TreeItemType STOCHASTIC_POINT_1D_LEAF = new TreeItemType("STOCHASTIC_POINT_1D_LEAF", InnerEnum.STOCHASTIC_POINT_1D_LEAF);
		public static readonly TreeItemType DATUM_POINT_1D_LEAF = new TreeItemType("DATUM_POINT_1D_LEAF", InnerEnum.DATUM_POINT_1D_LEAF);
		public static readonly TreeItemType NEW_POINT_1D_LEAF = new TreeItemType("NEW_POINT_1D_LEAF", InnerEnum.NEW_POINT_1D_LEAF);

		public static readonly TreeItemType REFERENCE_POINT_2D_LEAF = new TreeItemType("REFERENCE_POINT_2D_LEAF", InnerEnum.REFERENCE_POINT_2D_LEAF);
		public static readonly TreeItemType STOCHASTIC_POINT_2D_LEAF = new TreeItemType("STOCHASTIC_POINT_2D_LEAF", InnerEnum.STOCHASTIC_POINT_2D_LEAF);
		public static readonly TreeItemType DATUM_POINT_2D_LEAF = new TreeItemType("DATUM_POINT_2D_LEAF", InnerEnum.DATUM_POINT_2D_LEAF);
		public static readonly TreeItemType NEW_POINT_2D_LEAF = new TreeItemType("NEW_POINT_2D_LEAF", InnerEnum.NEW_POINT_2D_LEAF);

		public static readonly TreeItemType REFERENCE_POINT_3D_LEAF = new TreeItemType("REFERENCE_POINT_3D_LEAF", InnerEnum.REFERENCE_POINT_3D_LEAF);
		public static readonly TreeItemType STOCHASTIC_POINT_3D_LEAF = new TreeItemType("STOCHASTIC_POINT_3D_LEAF", InnerEnum.STOCHASTIC_POINT_3D_LEAF);
		public static readonly TreeItemType DATUM_POINT_3D_LEAF = new TreeItemType("DATUM_POINT_3D_LEAF", InnerEnum.DATUM_POINT_3D_LEAF);
		public static readonly TreeItemType NEW_POINT_3D_LEAF = new TreeItemType("NEW_POINT_3D_LEAF", InnerEnum.NEW_POINT_3D_LEAF);

		/// <summary>
		/// Terr. Observation * </summary>
		public static readonly TreeItemType LEVELING_DIRECTORY = new TreeItemType("LEVELING_DIRECTORY", InnerEnum.LEVELING_DIRECTORY);
		public static readonly TreeItemType DIRECTION_DIRECTORY = new TreeItemType("DIRECTION_DIRECTORY", InnerEnum.DIRECTION_DIRECTORY);
		public static readonly TreeItemType HORIZONTAL_DISTANCE_DIRECTORY = new TreeItemType("HORIZONTAL_DISTANCE_DIRECTORY", InnerEnum.HORIZONTAL_DISTANCE_DIRECTORY);
		public static readonly TreeItemType SLOPE_DISTANCE_DIRECTORY = new TreeItemType("SLOPE_DISTANCE_DIRECTORY", InnerEnum.SLOPE_DISTANCE_DIRECTORY);
		public static readonly TreeItemType ZENITH_ANGLE_DIRECTORY = new TreeItemType("ZENITH_ANGLE_DIRECTORY", InnerEnum.ZENITH_ANGLE_DIRECTORY);

		public static readonly TreeItemType LEVELING_LEAF = new TreeItemType("LEVELING_LEAF", InnerEnum.LEVELING_LEAF);
		public static readonly TreeItemType DIRECTION_LEAF = new TreeItemType("DIRECTION_LEAF", InnerEnum.DIRECTION_LEAF);
		public static readonly TreeItemType HORIZONTAL_DISTANCE_LEAF = new TreeItemType("HORIZONTAL_DISTANCE_LEAF", InnerEnum.HORIZONTAL_DISTANCE_LEAF);
		public static readonly TreeItemType SLOPE_DISTANCE_LEAF = new TreeItemType("SLOPE_DISTANCE_LEAF", InnerEnum.SLOPE_DISTANCE_LEAF);
		public static readonly TreeItemType ZENITH_ANGLE_LEAF = new TreeItemType("ZENITH_ANGLE_LEAF", InnerEnum.ZENITH_ANGLE_LEAF);

		/// <summary>
		/// GNSS * </summary>
		public static readonly TreeItemType GNSS_1D_DIRECTORY = new TreeItemType("GNSS_1D_DIRECTORY", InnerEnum.GNSS_1D_DIRECTORY);
		public static readonly TreeItemType GNSS_2D_DIRECTORY = new TreeItemType("GNSS_2D_DIRECTORY", InnerEnum.GNSS_2D_DIRECTORY);
		public static readonly TreeItemType GNSS_3D_DIRECTORY = new TreeItemType("GNSS_3D_DIRECTORY", InnerEnum.GNSS_3D_DIRECTORY);

		public static readonly TreeItemType GNSS_1D_LEAF = new TreeItemType("GNSS_1D_LEAF", InnerEnum.GNSS_1D_LEAF);
		public static readonly TreeItemType GNSS_2D_LEAF = new TreeItemType("GNSS_2D_LEAF", InnerEnum.GNSS_2D_LEAF);
		public static readonly TreeItemType GNSS_3D_LEAF = new TreeItemType("GNSS_3D_LEAF", InnerEnum.GNSS_3D_LEAF);

		/// <summary>
		/// Deformation analysis * </summary>
		public static readonly TreeItemType CONGRUENCE_ANALYSIS_1D_DIRECTORY = new TreeItemType("CONGRUENCE_ANALYSIS_1D_DIRECTORY", InnerEnum.CONGRUENCE_ANALYSIS_1D_DIRECTORY);
		public static readonly TreeItemType CONGRUENCE_ANALYSIS_2D_DIRECTORY = new TreeItemType("CONGRUENCE_ANALYSIS_2D_DIRECTORY", InnerEnum.CONGRUENCE_ANALYSIS_2D_DIRECTORY);
		public static readonly TreeItemType CONGRUENCE_ANALYSIS_3D_DIRECTORY = new TreeItemType("CONGRUENCE_ANALYSIS_3D_DIRECTORY", InnerEnum.CONGRUENCE_ANALYSIS_3D_DIRECTORY);

		public static readonly TreeItemType CONGRUENCE_ANALYSIS_1D_LEAF = new TreeItemType("CONGRUENCE_ANALYSIS_1D_LEAF", InnerEnum.CONGRUENCE_ANALYSIS_1D_LEAF);
		public static readonly TreeItemType CONGRUENCE_ANALYSIS_2D_LEAF = new TreeItemType("CONGRUENCE_ANALYSIS_2D_LEAF", InnerEnum.CONGRUENCE_ANALYSIS_2D_LEAF);
		public static readonly TreeItemType CONGRUENCE_ANALYSIS_3D_LEAF = new TreeItemType("CONGRUENCE_ANALYSIS_3D_LEAF", InnerEnum.CONGRUENCE_ANALYSIS_3D_LEAF);

		/// <summary>
		/// Deflection of vertical * </summary>
		public static readonly TreeItemType REFERENCE_VERTICAL_DEFLECTION_DIRECTORY = new TreeItemType("REFERENCE_VERTICAL_DEFLECTION_DIRECTORY", InnerEnum.REFERENCE_VERTICAL_DEFLECTION_DIRECTORY);
		public static readonly TreeItemType STOCHASTIC_VERTICAL_DEFLECTION_DIRECTORY = new TreeItemType("STOCHASTIC_VERTICAL_DEFLECTION_DIRECTORY", InnerEnum.STOCHASTIC_VERTICAL_DEFLECTION_DIRECTORY);
		public static readonly TreeItemType UNKNOWN_VERTICAL_DEFLECTION_DIRECTORY = new TreeItemType("UNKNOWN_VERTICAL_DEFLECTION_DIRECTORY", InnerEnum.UNKNOWN_VERTICAL_DEFLECTION_DIRECTORY);

		public static readonly TreeItemType REFERENCE_VERTICAL_DEFLECTION_LEAF = new TreeItemType("REFERENCE_VERTICAL_DEFLECTION_LEAF", InnerEnum.REFERENCE_VERTICAL_DEFLECTION_LEAF);
		public static readonly TreeItemType STOCHASTIC_VERTICAL_DEFLECTION_LEAF = new TreeItemType("STOCHASTIC_VERTICAL_DEFLECTION_LEAF", InnerEnum.STOCHASTIC_VERTICAL_DEFLECTION_LEAF);
		public static readonly TreeItemType UNKNOWN_VERTICAL_DEFLECTION_LEAF = new TreeItemType("UNKNOWN_VERTICAL_DEFLECTION_LEAF", InnerEnum.UNKNOWN_VERTICAL_DEFLECTION_LEAF);

		private static readonly List<TreeItemType> valueList = new List<TreeItemType>();

		static TreeItemType()
		{
			valueList.Add(ROOT);
			valueList.Add(UNSPECIFIC);
			valueList.Add(REFERENCE_POINT_1D_DIRECTORY);
			valueList.Add(STOCHASTIC_POINT_1D_DIRECTORY);
			valueList.Add(DATUM_POINT_1D_DIRECTORY);
			valueList.Add(NEW_POINT_1D_DIRECTORY);
			valueList.Add(REFERENCE_POINT_2D_DIRECTORY);
			valueList.Add(STOCHASTIC_POINT_2D_DIRECTORY);
			valueList.Add(DATUM_POINT_2D_DIRECTORY);
			valueList.Add(NEW_POINT_2D_DIRECTORY);
			valueList.Add(REFERENCE_POINT_3D_DIRECTORY);
			valueList.Add(STOCHASTIC_POINT_3D_DIRECTORY);
			valueList.Add(DATUM_POINT_3D_DIRECTORY);
			valueList.Add(NEW_POINT_3D_DIRECTORY);
			valueList.Add(REFERENCE_POINT_1D_LEAF);
			valueList.Add(STOCHASTIC_POINT_1D_LEAF);
			valueList.Add(DATUM_POINT_1D_LEAF);
			valueList.Add(NEW_POINT_1D_LEAF);
			valueList.Add(REFERENCE_POINT_2D_LEAF);
			valueList.Add(STOCHASTIC_POINT_2D_LEAF);
			valueList.Add(DATUM_POINT_2D_LEAF);
			valueList.Add(NEW_POINT_2D_LEAF);
			valueList.Add(REFERENCE_POINT_3D_LEAF);
			valueList.Add(STOCHASTIC_POINT_3D_LEAF);
			valueList.Add(DATUM_POINT_3D_LEAF);
			valueList.Add(NEW_POINT_3D_LEAF);
			valueList.Add(LEVELING_DIRECTORY);
			valueList.Add(DIRECTION_DIRECTORY);
			valueList.Add(HORIZONTAL_DISTANCE_DIRECTORY);
			valueList.Add(SLOPE_DISTANCE_DIRECTORY);
			valueList.Add(ZENITH_ANGLE_DIRECTORY);
			valueList.Add(LEVELING_LEAF);
			valueList.Add(DIRECTION_LEAF);
			valueList.Add(HORIZONTAL_DISTANCE_LEAF);
			valueList.Add(SLOPE_DISTANCE_LEAF);
			valueList.Add(ZENITH_ANGLE_LEAF);
			valueList.Add(GNSS_1D_DIRECTORY);
			valueList.Add(GNSS_2D_DIRECTORY);
			valueList.Add(GNSS_3D_DIRECTORY);
			valueList.Add(GNSS_1D_LEAF);
			valueList.Add(GNSS_2D_LEAF);
			valueList.Add(GNSS_3D_LEAF);
			valueList.Add(CONGRUENCE_ANALYSIS_1D_DIRECTORY);
			valueList.Add(CONGRUENCE_ANALYSIS_2D_DIRECTORY);
			valueList.Add(CONGRUENCE_ANALYSIS_3D_DIRECTORY);
			valueList.Add(CONGRUENCE_ANALYSIS_1D_LEAF);
			valueList.Add(CONGRUENCE_ANALYSIS_2D_LEAF);
			valueList.Add(CONGRUENCE_ANALYSIS_3D_LEAF);
			valueList.Add(REFERENCE_VERTICAL_DEFLECTION_DIRECTORY);
			valueList.Add(STOCHASTIC_VERTICAL_DEFLECTION_DIRECTORY);
			valueList.Add(UNKNOWN_VERTICAL_DEFLECTION_DIRECTORY);
			valueList.Add(REFERENCE_VERTICAL_DEFLECTION_LEAF);
			valueList.Add(STOCHASTIC_VERTICAL_DEFLECTION_LEAF);
			valueList.Add(UNKNOWN_VERTICAL_DEFLECTION_LEAF);
		}

		public enum InnerEnum
		{
			ROOT,
			UNSPECIFIC,
			REFERENCE_POINT_1D_DIRECTORY,
			STOCHASTIC_POINT_1D_DIRECTORY,
			DATUM_POINT_1D_DIRECTORY,
			NEW_POINT_1D_DIRECTORY,
			REFERENCE_POINT_2D_DIRECTORY,
			STOCHASTIC_POINT_2D_DIRECTORY,
			DATUM_POINT_2D_DIRECTORY,
			NEW_POINT_2D_DIRECTORY,
			REFERENCE_POINT_3D_DIRECTORY,
			STOCHASTIC_POINT_3D_DIRECTORY,
			DATUM_POINT_3D_DIRECTORY,
			NEW_POINT_3D_DIRECTORY,
			REFERENCE_POINT_1D_LEAF,
			STOCHASTIC_POINT_1D_LEAF,
			DATUM_POINT_1D_LEAF,
			NEW_POINT_1D_LEAF,
			REFERENCE_POINT_2D_LEAF,
			STOCHASTIC_POINT_2D_LEAF,
			DATUM_POINT_2D_LEAF,
			NEW_POINT_2D_LEAF,
			REFERENCE_POINT_3D_LEAF,
			STOCHASTIC_POINT_3D_LEAF,
			DATUM_POINT_3D_LEAF,
			NEW_POINT_3D_LEAF,
			LEVELING_DIRECTORY,
			DIRECTION_DIRECTORY,
			HORIZONTAL_DISTANCE_DIRECTORY,
			SLOPE_DISTANCE_DIRECTORY,
			ZENITH_ANGLE_DIRECTORY,
			LEVELING_LEAF,
			DIRECTION_LEAF,
			HORIZONTAL_DISTANCE_LEAF,
			SLOPE_DISTANCE_LEAF,
			ZENITH_ANGLE_LEAF,
			GNSS_1D_DIRECTORY,
			GNSS_2D_DIRECTORY,
			GNSS_3D_DIRECTORY,
			GNSS_1D_LEAF,
			GNSS_2D_LEAF,
			GNSS_3D_LEAF,
			CONGRUENCE_ANALYSIS_1D_DIRECTORY,
			CONGRUENCE_ANALYSIS_2D_DIRECTORY,
			CONGRUENCE_ANALYSIS_3D_DIRECTORY,
			CONGRUENCE_ANALYSIS_1D_LEAF,
			CONGRUENCE_ANALYSIS_2D_LEAF,
			CONGRUENCE_ANALYSIS_3D_LEAF,
			REFERENCE_VERTICAL_DEFLECTION_DIRECTORY,
			STOCHASTIC_VERTICAL_DEFLECTION_DIRECTORY,
			UNKNOWN_VERTICAL_DEFLECTION_DIRECTORY,
			REFERENCE_VERTICAL_DEFLECTION_LEAF,
			STOCHASTIC_VERTICAL_DEFLECTION_LEAF,
			UNKNOWN_VERTICAL_DEFLECTION_LEAF
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private TreeItemType(string name, InnerEnum innerEnum)
		{
			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		public static TreeItemType getLeafByDirectoryType(TreeItemType type)
		{
			switch (type.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_1D_DIRECTORY:
				return TreeItemType.REFERENCE_POINT_1D_LEAF;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_2D_DIRECTORY:
				return TreeItemType.REFERENCE_POINT_2D_LEAF;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_3D_DIRECTORY:
				return TreeItemType.REFERENCE_POINT_3D_LEAF;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_1D_DIRECTORY:
				return TreeItemType.STOCHASTIC_POINT_1D_LEAF;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_2D_DIRECTORY:
				return TreeItemType.STOCHASTIC_POINT_2D_LEAF;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_3D_DIRECTORY:
				return TreeItemType.STOCHASTIC_POINT_3D_LEAF;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_1D_DIRECTORY:
				return TreeItemType.DATUM_POINT_1D_LEAF;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_2D_DIRECTORY:
				return TreeItemType.DATUM_POINT_2D_LEAF;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_3D_DIRECTORY:
				return TreeItemType.DATUM_POINT_3D_LEAF;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_1D_DIRECTORY:
				return TreeItemType.NEW_POINT_1D_LEAF;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_2D_DIRECTORY:
				return TreeItemType.NEW_POINT_2D_LEAF;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_3D_DIRECTORY:
				return TreeItemType.NEW_POINT_3D_LEAF;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.LEVELING_DIRECTORY:
				return TreeItemType.LEVELING_LEAF;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DIRECTION_DIRECTORY:
				return TreeItemType.DIRECTION_LEAF;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.HORIZONTAL_DISTANCE_DIRECTORY:
				return TreeItemType.HORIZONTAL_DISTANCE_LEAF;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.SLOPE_DISTANCE_DIRECTORY:
				return TreeItemType.SLOPE_DISTANCE_LEAF;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.ZENITH_ANGLE_DIRECTORY:
				return TreeItemType.ZENITH_ANGLE_LEAF;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_1D_DIRECTORY:
				return TreeItemType.GNSS_1D_LEAF;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_2D_DIRECTORY:
				return TreeItemType.GNSS_2D_LEAF;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_3D_DIRECTORY:
				return TreeItemType.GNSS_3D_LEAF;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_1D_DIRECTORY:
				return TreeItemType.CONGRUENCE_ANALYSIS_1D_LEAF;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_2D_DIRECTORY:
				return TreeItemType.CONGRUENCE_ANALYSIS_2D_LEAF;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_3D_DIRECTORY:
				return TreeItemType.CONGRUENCE_ANALYSIS_3D_LEAF;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_VERTICAL_DEFLECTION_DIRECTORY:
				return TreeItemType.REFERENCE_VERTICAL_DEFLECTION_LEAF;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_VERTICAL_DEFLECTION_DIRECTORY:
				return TreeItemType.STOCHASTIC_VERTICAL_DEFLECTION_LEAF;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.UNKNOWN_VERTICAL_DEFLECTION_DIRECTORY:
				return TreeItemType.UNKNOWN_VERTICAL_DEFLECTION_LEAF;

			default:
				return null;
			}
		}

		public static TreeItemType getDirectoryByLeafType(TreeItemType type)
		{
			switch (type.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_1D_LEAF:
				return TreeItemType.REFERENCE_POINT_1D_DIRECTORY;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_2D_LEAF:
				return TreeItemType.REFERENCE_POINT_2D_DIRECTORY;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_3D_LEAF:
				return TreeItemType.REFERENCE_POINT_3D_DIRECTORY;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_1D_LEAF:
				return TreeItemType.STOCHASTIC_POINT_1D_DIRECTORY;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_2D_LEAF:
				return TreeItemType.STOCHASTIC_POINT_2D_DIRECTORY;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_3D_LEAF:
				return TreeItemType.STOCHASTIC_POINT_3D_DIRECTORY;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_1D_LEAF:
				return TreeItemType.DATUM_POINT_1D_DIRECTORY;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_2D_LEAF:
				return TreeItemType.DATUM_POINT_2D_DIRECTORY;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_3D_LEAF:
				return TreeItemType.DATUM_POINT_3D_DIRECTORY;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_1D_LEAF:
				return TreeItemType.NEW_POINT_1D_DIRECTORY;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_2D_LEAF:
				return TreeItemType.NEW_POINT_2D_DIRECTORY;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_3D_LEAF:
				return TreeItemType.NEW_POINT_3D_DIRECTORY;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.LEVELING_LEAF:
				return TreeItemType.LEVELING_DIRECTORY;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DIRECTION_LEAF:
				return TreeItemType.DIRECTION_DIRECTORY;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.HORIZONTAL_DISTANCE_LEAF:
				return TreeItemType.HORIZONTAL_DISTANCE_DIRECTORY;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.SLOPE_DISTANCE_LEAF:
				return TreeItemType.SLOPE_DISTANCE_DIRECTORY;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.ZENITH_ANGLE_LEAF:
				return TreeItemType.ZENITH_ANGLE_DIRECTORY;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_1D_LEAF:
				return TreeItemType.GNSS_1D_DIRECTORY;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_2D_LEAF:
				return TreeItemType.GNSS_2D_DIRECTORY;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_3D_LEAF:
				return TreeItemType.GNSS_3D_DIRECTORY;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_1D_LEAF:
				return TreeItemType.CONGRUENCE_ANALYSIS_1D_DIRECTORY;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_2D_LEAF:
				return TreeItemType.CONGRUENCE_ANALYSIS_2D_DIRECTORY;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_3D_LEAF:
				return TreeItemType.CONGRUENCE_ANALYSIS_3D_DIRECTORY;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_VERTICAL_DEFLECTION_LEAF:
				return TreeItemType.REFERENCE_VERTICAL_DEFLECTION_DIRECTORY;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_VERTICAL_DEFLECTION_LEAF:
				return TreeItemType.STOCHASTIC_VERTICAL_DEFLECTION_DIRECTORY;
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.UNKNOWN_VERTICAL_DEFLECTION_LEAF:
				return TreeItemType.UNKNOWN_VERTICAL_DEFLECTION_DIRECTORY;

			default:
				return null;
			}
		}

		public static org.applied_geodesy.adjustment.network.PointType getPointTypeByTreeItemType(TreeItemType type)
		{
			switch (type.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_1D_DIRECTORY:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_1D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_2D_DIRECTORY:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_2D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_3D_DIRECTORY:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_3D_LEAF:
				return PointType.DATUM_POINT;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_1D_DIRECTORY:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_1D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_2D_DIRECTORY:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_2D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_3D_DIRECTORY:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_3D_LEAF:
				return PointType.NEW_POINT;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_1D_DIRECTORY:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_1D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_2D_DIRECTORY:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_2D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_3D_DIRECTORY:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_3D_LEAF:
				return PointType.REFERENCE_POINT;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_1D_DIRECTORY:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_1D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_2D_DIRECTORY:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_2D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_3D_DIRECTORY:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_3D_LEAF:
				return PointType.STOCHASTIC_POINT;

			default:
				return null;
			}
		}

		public static org.applied_geodesy.adjustment.network.VerticalDeflectionType getVerticalDeflectionTypeByTreeItemType(TreeItemType type)
		{
			switch (type.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_VERTICAL_DEFLECTION_DIRECTORY:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_VERTICAL_DEFLECTION_LEAF:
				return VerticalDeflectionType.REFERENCE_VERTICAL_DEFLECTION;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_VERTICAL_DEFLECTION_DIRECTORY:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_VERTICAL_DEFLECTION_LEAF:
				return VerticalDeflectionType.STOCHASTIC_VERTICAL_DEFLECTION;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.UNKNOWN_VERTICAL_DEFLECTION_DIRECTORY:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.UNKNOWN_VERTICAL_DEFLECTION_LEAF:
				return VerticalDeflectionType.UNKNOWN_VERTICAL_DEFLECTION;

			default:
				return null;
			}
		}

		public static org.applied_geodesy.adjustment.network.ObservationType getObservationTypeByTreeItemType(TreeItemType type)
		{
			switch (type.innerEnumValue)
			{

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.LEVELING_DIRECTORY:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.LEVELING_LEAF:
				return ObservationType.LEVELING;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DIRECTION_DIRECTORY:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DIRECTION_LEAF:
				return ObservationType.DIRECTION;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.HORIZONTAL_DISTANCE_DIRECTORY:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.HORIZONTAL_DISTANCE_LEAF:
				return ObservationType.HORIZONTAL_DISTANCE;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.SLOPE_DISTANCE_DIRECTORY:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.SLOPE_DISTANCE_LEAF:
				return ObservationType.SLOPE_DISTANCE;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.ZENITH_ANGLE_DIRECTORY:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.ZENITH_ANGLE_LEAF:
				return ObservationType.ZENITH_ANGLE;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_1D_DIRECTORY:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_1D_LEAF:
				return ObservationType.GNSS1D;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_2D_DIRECTORY:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_2D_LEAF:
				return ObservationType.GNSS2D;

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_3D_DIRECTORY:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_3D_LEAF:
				return ObservationType.GNSS3D;

			default:
				return null;
			}
		}

		public static TreeItemType getTreeItemTypeByObservationType(org.applied_geodesy.adjustment.network.ObservationType type)
		{
			switch (type.innerEnumValue)
			{
			case ObservationType.InnerEnum.LEVELING:
				return TreeItemType.LEVELING_LEAF;
			case ObservationType.InnerEnum.DIRECTION:
				return TreeItemType.DIRECTION_LEAF;
			case ObservationType.InnerEnum.HORIZONTAL_DISTANCE:
				return TreeItemType.HORIZONTAL_DISTANCE_LEAF;
			case ObservationType.InnerEnum.SLOPE_DISTANCE:
				return TreeItemType.SLOPE_DISTANCE_LEAF;
			case ObservationType.InnerEnum.ZENITH_ANGLE:
				return TreeItemType.ZENITH_ANGLE_LEAF;
			case ObservationType.InnerEnum.GNSS1D:
				return TreeItemType.GNSS_1D_LEAF;
			case ObservationType.InnerEnum.GNSS2D:
				return TreeItemType.GNSS_2D_LEAF;
			case ObservationType.InnerEnum.GNSS3D:
				return TreeItemType.GNSS_3D_LEAF;
			default:
				return null;
			}
		}

		public static TreeItemType getTreeItemTypeByVerticalDeflectionType(org.applied_geodesy.adjustment.network.VerticalDeflectionType type)
		{
			switch (type.innerEnumValue)
			{
			case VerticalDeflectionType.InnerEnum.REFERENCE_VERTICAL_DEFLECTION:
				return TreeItemType.REFERENCE_VERTICAL_DEFLECTION_LEAF;
			case VerticalDeflectionType.InnerEnum.STOCHASTIC_VERTICAL_DEFLECTION:
				return TreeItemType.STOCHASTIC_VERTICAL_DEFLECTION_LEAF;
			case VerticalDeflectionType.InnerEnum.UNKNOWN_VERTICAL_DEFLECTION:
				return TreeItemType.UNKNOWN_VERTICAL_DEFLECTION_LEAF;
			default:
				return null;
			}
		}

		public static TreeItemType getTreeItemTypeByPointType(org.applied_geodesy.adjustment.network.PointType type, int dimension)
		{
			switch (type.innerEnumValue)
			{
			case PointType.InnerEnum.REFERENCE_POINT:
				if (dimension == 1)
				{
					return TreeItemType.REFERENCE_POINT_1D_LEAF;
				}
				else if (dimension == 2)
				{
					return TreeItemType.REFERENCE_POINT_2D_LEAF;
				}
				else if (dimension == 3)
				{
					return TreeItemType.REFERENCE_POINT_3D_LEAF;
				}
			case PointType.InnerEnum.STOCHASTIC_POINT:
				if (dimension == 1)
				{
					return TreeItemType.STOCHASTIC_POINT_1D_LEAF;
				}
				else if (dimension == 2)
				{
					return TreeItemType.STOCHASTIC_POINT_2D_LEAF;
				}
				else if (dimension == 3)
				{
					return TreeItemType.STOCHASTIC_POINT_3D_LEAF;
				}
			case PointType.InnerEnum.DATUM_POINT:
				if (dimension == 1)
				{
					return TreeItemType.DATUM_POINT_1D_LEAF;
				}
				else if (dimension == 2)
				{
					return TreeItemType.DATUM_POINT_2D_LEAF;
				}
				else if (dimension == 3)
				{
					return TreeItemType.DATUM_POINT_3D_LEAF;
				}
			case PointType.InnerEnum.NEW_POINT:
				if (dimension == 1)
				{
					return TreeItemType.NEW_POINT_1D_LEAF;
				}
				else if (dimension == 2)
				{
					return TreeItemType.NEW_POINT_2D_LEAF;
				}
				else if (dimension == 3)
				{
					return TreeItemType.NEW_POINT_3D_LEAF;
				}
			default:
				return null;
			}
		}

		public static TreeItemType getTreeItemTypeByCongruenceAnalysisDimension(int dimension)
		{
			switch (dimension)
			{
			case 1:
				return TreeItemType.CONGRUENCE_ANALYSIS_1D_LEAF;
			case 2:
				return TreeItemType.CONGRUENCE_ANALYSIS_2D_LEAF;
			case 3:
				return TreeItemType.CONGRUENCE_ANALYSIS_3D_LEAF;
			default:
				return null;
			}
		}

		public static bool isPointTypeLeaf(TreeItemType type)
		{
			switch (type.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_1D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_2D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_3D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_1D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_2D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_3D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_1D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_2D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_3D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_1D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_2D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_3D_LEAF:
				return true;
			default:
				return false;
			}
		}

		public static bool isPointTypeDirectory(TreeItemType type)
		{
			TreeItemType child = getLeafByDirectoryType(type);
			return child != null && isPointTypeLeaf(child);
		}

		public static bool isObservationTypeLeaf(TreeItemType type)
		{
			switch (type.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.LEVELING_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DIRECTION_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.HORIZONTAL_DISTANCE_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.SLOPE_DISTANCE_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.ZENITH_ANGLE_LEAF:
				return true;

			default:
				return false;
			}
		}

		public static bool isObservationTypeDirectory(TreeItemType type)
		{
			TreeItemType child = getLeafByDirectoryType(type);
			return child != null && isObservationTypeLeaf(child);
		}

		public static bool isGNSSObservationTypeLeaf(TreeItemType type)
		{
			switch (type.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_1D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_2D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_3D_LEAF:
				return true;

			default:
				return false;
			}
		}

		public static bool isGNSSObservationTypeDirectory(TreeItemType type)
		{
			TreeItemType child = getLeafByDirectoryType(type);
			return child != null && isGNSSObservationTypeLeaf(child);
		}


		public static bool isCongruenceAnalysisTypeLeaf(TreeItemType type)
		{
			switch (type.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_1D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_2D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_3D_LEAF:
				return true;
			default:
				return false;
			}
		}

		public static bool isCongruenceAnalysisTypeDirectory(TreeItemType type)
		{
			TreeItemType child = getLeafByDirectoryType(type);
			return child != null && isCongruenceAnalysisTypeLeaf(child);
		}

		public static bool isVerticalDeflectionTypeLeaf(TreeItemType type)
		{
			switch (type.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_VERTICAL_DEFLECTION_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_VERTICAL_DEFLECTION_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.UNKNOWN_VERTICAL_DEFLECTION_LEAF:
				return true;
			default:
				return false;
			}
		}

		public static bool isVerticalDeflectionTypeDirectory(TreeItemType type)
		{
			TreeItemType child = getLeafByDirectoryType(type);
			return child != null && isVerticalDeflectionTypeLeaf(child);
		}

		public static TreeItemType[] values()
		{
			return valueList.ToArray();
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public override string ToString()
		{
			return nameValue;
		}

		public static TreeItemType valueOf(string name)
		{
			foreach (TreeItemType enumInstance in TreeItemType.valueList)
			{
				if (enumInstance.nameValue == name)
				{
					return enumInstance;
				}
			}
			throw new System.ArgumentException(name);
		}
	}

}