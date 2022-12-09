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
	using VerticalDeflectionGroupUncertaintyType = org.applied_geodesy.adjustment.network.VerticalDeflectionGroupUncertaintyType;
	using TabType = org.applied_geodesy.jag3d.ui.tabpane.TabType;

	using IntegerProperty = javafx.beans.property.IntegerProperty;
	using SimpleIntegerProperty = javafx.beans.property.SimpleIntegerProperty;

	public class VerticalDeflectionTreeItemValue : TreeItemValue, Sortable, Groupable
	{
		private IntegerProperty groupId = new SimpleIntegerProperty(-1);
		private int orderId;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: VerticalDeflectionTreeItemValue(int groupId, TreeItemType type, String name, int orderId) throws IllegalArgumentException
		internal VerticalDeflectionTreeItemValue(int groupId, TreeItemType type, string name, int orderId) : this(groupId, type, name, true, orderId)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: VerticalDeflectionTreeItemValue(int groupId, TreeItemType type, String name, boolean enable, int orderId) throws IllegalArgumentException
		internal VerticalDeflectionTreeItemValue(int groupId, TreeItemType type, string name, bool enable, int orderId) : base(type, name)
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
				return 2;
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
				if (type == TreeItemType.STOCHASTIC_VERTICAL_DEFLECTION_LEAF)
				{
					tabTyps.Add(TabType.PROPERTIES);
				}
    
				// Results Point
				tabTyps.Add(TabType.RESULT_DATA);
    
				// Variance components estimation 
				if (type == TreeItemType.STOCHASTIC_VERTICAL_DEFLECTION_LEAF)
				{
					tabTyps.Add(TabType.VARIANCE_COMPONENT);
				}
    
				return ((List<TabType>)tabTyps).ToArray();
			}
		}

		public static double getDefaultUncertainty(VerticalDeflectionGroupUncertaintyType uncertaintyType)
		{
			switch (uncertaintyType.innerEnumValue)
			{
			case VerticalDeflectionGroupUncertaintyType.InnerEnum.DEFLECTION_X:
				return DefaultUncertainty.UncertaintyDeflectionX;
			case VerticalDeflectionGroupUncertaintyType.InnerEnum.DEFLECTION_Y:
				return DefaultUncertainty.UncertaintyDeflectionY;
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