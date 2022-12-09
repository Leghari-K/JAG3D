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
	using DefaultUncertainty = org.applied_geodesy.adjustment.network.DefaultUncertainty;
	using ObservationGroupUncertaintyType = org.applied_geodesy.adjustment.network.ObservationGroupUncertaintyType;
	using ParameterType = org.applied_geodesy.adjustment.network.ParameterType;
	using TabType = org.applied_geodesy.jag3d.ui.tabpane.TabType;

	using IntegerProperty = javafx.beans.property.IntegerProperty;
	using SimpleIntegerProperty = javafx.beans.property.SimpleIntegerProperty;

	public class ObservationTreeItemValue : TreeItemValue, Sortable, Groupable
	{
		private IntegerProperty groupId = new SimpleIntegerProperty(-1);
		private int orderId;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: ObservationTreeItemValue(int groupId, TreeItemType type, String name, int orderId) throws IllegalArgumentException
		internal ObservationTreeItemValue(int groupId, TreeItemType type, string name, int orderId) : this(groupId, type, name, true, orderId)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: ObservationTreeItemValue(int groupId, TreeItemType type, String name, boolean enable, int orderId) throws IllegalArgumentException
		internal ObservationTreeItemValue(int groupId, TreeItemType type, string name, bool enable, int orderId) : base(type, name)
		{
			this.GroupId = groupId;
			this.Enable = enable;
			this.OrderId = orderId;
		}

		public virtual IntegerProperty groupIdProperty()
		{
			return this.groupId;
		}

		public virtual int GroupId
		{
			get
			{
				return this.groupIdProperty().get();
			}
			set
			{
				this.groupIdProperty().set(value);
			}
		}


		public int Dimension
		{
			get
			{
				switch (this.ItemType.innerEnumValue)
				{
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_2D_LEAF:
					return 2;
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_3D_LEAF:
					return 3;
				default:
					return 1;
				}
			}
		}

		public override TabType[] TabTypes
		{
			get
			{
				return new TabType[] {TabType.RAW_DATA, TabType.PROPERTIES, TabType.RESULT_DATA, TabType.ADDITIONAL_PARAMETER, TabType.VARIANCE_COMPONENT};
			}
		}

		public static ParameterType[] getParameterTypes(TreeItemType itemType)
		{
			switch (itemType.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.LEVELING_LEAF:
				return new ParameterType[] {ParameterType.SCALE};
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.HORIZONTAL_DISTANCE_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.SLOPE_DISTANCE_LEAF:
				return new ParameterType[] {ParameterType.ZERO_POINT_OFFSET, ParameterType.SCALE};
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DIRECTION_LEAF:
				return new ParameterType[] {ParameterType.ORIENTATION};
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.ZENITH_ANGLE_LEAF:
				return new ParameterType[] {ParameterType.REFRACTION_INDEX};
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_1D_LEAF:
				return new ParameterType[] {ParameterType.ROTATION_Y, ParameterType.ROTATION_X, ParameterType.SCALE};
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_2D_LEAF:
				return new ParameterType[] {ParameterType.ROTATION_Z, ParameterType.SCALE};
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_3D_LEAF:
				return new ParameterType[] {ParameterType.ROTATION_Y, ParameterType.ROTATION_X, ParameterType.ROTATION_Z, ParameterType.SCALE};
			default:
				return new ParameterType[] {};
			}
		}

		public static double getDefaultUncertainty(TreeItemType itemType, ObservationGroupUncertaintyType uncertaintyType)
		{
			switch (itemType.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.LEVELING_LEAF:

				switch (uncertaintyType.innerEnumValue)
				{
				case ObservationGroupUncertaintyType.InnerEnum.ZERO_POINT_OFFSET:
					return DefaultUncertainty.UncertaintyLevelingZeroPointOffset;
				case ObservationGroupUncertaintyType.InnerEnum.SQUARE_ROOT_DISTANCE_DEPENDENT:
					return DefaultUncertainty.UncertaintyLevelingSquareRootDistanceDependent;
				case ObservationGroupUncertaintyType.InnerEnum.DISTANCE_DEPENDENT:
					return DefaultUncertainty.UncertaintyLevelingDistanceDependent;
				}

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.HORIZONTAL_DISTANCE_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.SLOPE_DISTANCE_LEAF:

				switch (uncertaintyType.innerEnumValue)
				{
				case ObservationGroupUncertaintyType.InnerEnum.ZERO_POINT_OFFSET:
					return DefaultUncertainty.UncertaintyDistanceZeroPointOffset;
				case ObservationGroupUncertaintyType.InnerEnum.SQUARE_ROOT_DISTANCE_DEPENDENT:
					return DefaultUncertainty.UncertaintyDistanceSquareRootDistanceDependent;
				case ObservationGroupUncertaintyType.InnerEnum.DISTANCE_DEPENDENT:
					return DefaultUncertainty.UncertaintyDistanceDistanceDependent;
				}

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DIRECTION_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.ZENITH_ANGLE_LEAF:

				switch (uncertaintyType.innerEnumValue)
				{
				case ObservationGroupUncertaintyType.InnerEnum.ZERO_POINT_OFFSET:
					return DefaultUncertainty.UncertaintyAngleZeroPointOffset;
				case ObservationGroupUncertaintyType.InnerEnum.SQUARE_ROOT_DISTANCE_DEPENDENT:
					return DefaultUncertainty.UncertaintyAngleSquareRootDistanceDependent;
				case ObservationGroupUncertaintyType.InnerEnum.DISTANCE_DEPENDENT:
					return DefaultUncertainty.UncertaintyAngleDistanceDependent;
				}

			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_1D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_2D_LEAF:
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.GNSS_3D_LEAF:

				switch (uncertaintyType.innerEnumValue)
				{
				case ObservationGroupUncertaintyType.InnerEnum.ZERO_POINT_OFFSET:
					return DefaultUncertainty.UncertaintyGNSSZeroPointOffset;
				case ObservationGroupUncertaintyType.InnerEnum.SQUARE_ROOT_DISTANCE_DEPENDENT:
					return DefaultUncertainty.UncertaintyGNSSSquareRootDistanceDependent;
				case ObservationGroupUncertaintyType.InnerEnum.DISTANCE_DEPENDENT:
					return DefaultUncertainty.UncertaintyGNSSDistanceDependent;
				}

			default:
				return 0;
			}
		}

		public virtual int OrderId
		{
			get
			{
				return this.orderId;
			}
			set
			{
				this.orderId = value;
			}
		}

	}
}