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
	using ParameterType = org.applied_geodesy.adjustment.network.ParameterType;
	using RestrictionType = org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType;
	using TabType = org.applied_geodesy.jag3d.ui.tabpane.TabType;

	using IntegerProperty = javafx.beans.property.IntegerProperty;
	using SimpleIntegerProperty = javafx.beans.property.SimpleIntegerProperty;

	public class CongruenceAnalysisTreeItemValue : TreeItemValue, Sortable, Groupable
	{
		private IntegerProperty groupId = new SimpleIntegerProperty(-1);
		private int orderId;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: CongruenceAnalysisTreeItemValue(int groupId, TreeItemType type, String name, int orderId) throws IllegalArgumentException
		internal CongruenceAnalysisTreeItemValue(int groupId, TreeItemType type, string name, int orderId) : this(groupId, type, name, true, orderId)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: CongruenceAnalysisTreeItemValue(int groupId, TreeItemType type, String name, boolean enable, int orderId) throws IllegalArgumentException
		internal CongruenceAnalysisTreeItemValue(int groupId, TreeItemType type, string name, bool enable, int orderId) : base(type, name)
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
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_1D_LEAF:
					return 1;
    
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_2D_LEAF:
					return 2;
    
				case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_3D_LEAF:
					return 3;
				default:
					throw new System.ArgumentException(this.GetType().Name + " : Error, TreeItemType does not refer to an object point nexus (congruence analysis) " + this.ItemType);
				}
			}
		}

		public static RestrictionType[] getRestrictionTypes(TreeItemType itemType)
		{
			switch (itemType.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_1D_LEAF:
				return new RestrictionType[] {RestrictionType.FIXED_TRANSLATION_Z, RestrictionType.FIXED_SCALE_Z};
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_2D_LEAF:
				return new RestrictionType[] {RestrictionType.FIXED_TRANSLATION_Y, RestrictionType.FIXED_TRANSLATION_X, RestrictionType.FIXED_ROTATION_Z, RestrictionType.FIXED_SCALE_Y, RestrictionType.FIXED_SCALE_X, RestrictionType.FIXED_SHEAR_Z, RestrictionType.IDENT_SCALES_XY};
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_3D_LEAF:
				return new RestrictionType[] {RestrictionType.FIXED_TRANSLATION_Y, RestrictionType.FIXED_TRANSLATION_X, RestrictionType.FIXED_TRANSLATION_Z, RestrictionType.FIXED_ROTATION_Y, RestrictionType.FIXED_ROTATION_X, RestrictionType.FIXED_ROTATION_Z, RestrictionType.FIXED_SCALE_Y, RestrictionType.FIXED_SCALE_X, RestrictionType.FIXED_SCALE_Z, RestrictionType.FIXED_SHEAR_Y, RestrictionType.FIXED_SHEAR_X, RestrictionType.FIXED_SHEAR_Z, RestrictionType.IDENT_SCALES_XY, RestrictionType.IDENT_SCALES_XZ, RestrictionType.IDENT_SCALES_YZ};
			default:
				throw new System.ArgumentException(typeof(ObservationTreeItemValue).Name + " : Error, TreeItemType does not refer to a congruence analysis type " + itemType);
			}
		}

		public static ParameterType[] getParameterTypes(TreeItemType itemType)
		{
			switch (itemType.innerEnumValue)
			{
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_1D_LEAF:
				return new ParameterType[] {ParameterType.STRAIN_TRANSLATION_Z, ParameterType.STRAIN_SCALE_Z};
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_2D_LEAF:
				return new ParameterType[] {ParameterType.STRAIN_TRANSLATION_Y, ParameterType.STRAIN_TRANSLATION_X, ParameterType.STRAIN_ROTATION_Z, ParameterType.STRAIN_SCALE_Y, ParameterType.STRAIN_SCALE_X, ParameterType.STRAIN_SHEAR_Z};
			case org.applied_geodesy.jag3d.ui.tree.TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_3D_LEAF:
				return new ParameterType[] {ParameterType.STRAIN_TRANSLATION_Y, ParameterType.STRAIN_TRANSLATION_X, ParameterType.STRAIN_TRANSLATION_Z, ParameterType.STRAIN_ROTATION_Y, ParameterType.STRAIN_ROTATION_X, ParameterType.STRAIN_ROTATION_Z, ParameterType.STRAIN_SCALE_Y, ParameterType.STRAIN_SCALE_X, ParameterType.STRAIN_SHEAR_Y, ParameterType.STRAIN_SHEAR_X, ParameterType.STRAIN_SHEAR_Z};
			default:
				throw new System.ArgumentException(typeof(ObservationTreeItemValue).Name + " : Error, TreeItemType does not refer to a congruence analysis type " + itemType);
			}
		}

		public override TabType[] TabTypes
		{
			get
			{
				return new TabType[] {TabType.RAW_DATA, TabType.PROPERTIES, TabType.RESULT_CONGRUENCE_ANALYSIS, TabType.ADDITIONAL_PARAMETER};
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