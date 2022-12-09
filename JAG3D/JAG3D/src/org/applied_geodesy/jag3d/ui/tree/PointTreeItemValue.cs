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

	using DefaultUncertainty = org.applied_geodesy.adjustment.network.DefaultUncertainty;
	using PointGroupUncertaintyType = org.applied_geodesy.adjustment.network.PointGroupUncertaintyType;
	using TabType = org.applied_geodesy.jag3d.ui.tabpane.TabType;

	using IntegerProperty = javafx.beans.property.IntegerProperty;
	using SimpleIntegerProperty = javafx.beans.property.SimpleIntegerProperty;

	public class PointTreeItemValue : TreeItemValue, Sortable, Groupable
	{
		private IntegerProperty groupId = new SimpleIntegerProperty(-1);
		private int orderId;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: PointTreeItemValue(int groupId, TreeItemType type, String name, int orderId) throws IllegalArgumentException
		internal PointTreeItemValue(int groupId, TreeItemType type, string name, int orderId) : this(groupId, type, name, true, orderId)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: PointTreeItemValue(int groupId, TreeItemType type, String name, boolean enable, int orderId) throws IllegalArgumentException
		internal PointTreeItemValue(int groupId, TreeItemType type, string name, bool enable, int orderId) : base(type, name)
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
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_1D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_1D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_1D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_1D_LEAF:
					return 1;
    
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_2D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_2D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_2D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_2D_LEAF:
					return 2;
    
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.REFERENCE_POINT_3D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_3D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_3D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.NEW_POINT_3D_LEAF:
					return 3;
				default:
					throw new System.ArgumentException(this.GetType().Name + " : Error, TreeItemType does not refer to a PointType " + this.ItemType);
				}
			}
		}


		public override TabType[] TabTypes
		{
			get
			{
				TreeItemType type = this.ItemType;
				IList<TabType> tabTyps = new List<TabType>(5);
    
				// A-priori Values
				tabTyps.Add(TabType.RAW_DATA);
    
				// Properties 
				switch (type.innerEnumValue)
				{
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_1D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_2D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_3D_LEAF:
					tabTyps.Add(TabType.PROPERTIES);
					break;
				default:
					break;
				}
    
				// Results Point
				tabTyps.Add(TabType.RESULT_DATA);
    
				// Congruence points
				switch (type.innerEnumValue)
				{
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_1D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_2D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.DATUM_POINT_3D_LEAF:
					tabTyps.Add(TabType.RESULT_CONGRUENCE_ANALYSIS);
					break;
				default:
					break;
				}
    
				// Variance components estimation 
				switch (type.innerEnumValue)
				{
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_1D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_2D_LEAF:
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.STOCHASTIC_POINT_3D_LEAF:
					tabTyps.Add(TabType.VARIANCE_COMPONENT);
					break;
				default:
					break;
				}
    
				return ((List<TabType>)tabTyps).ToArray();
			}
		}

		public static double getDefaultUncertainty(PointGroupUncertaintyType uncertaintyType)
		{
			switch (uncertaintyType.innerEnumValue)
			{
			case PointGroupUncertaintyType.InnerEnum.COMPONENT_X:
				return DefaultUncertainty.UncertaintyX;
			case PointGroupUncertaintyType.InnerEnum.COMPONENT_Y:
				return DefaultUncertainty.UncertaintyY;
			case PointGroupUncertaintyType.InnerEnum.COMPONENT_Z:
				return DefaultUncertainty.UncertaintyZ;
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