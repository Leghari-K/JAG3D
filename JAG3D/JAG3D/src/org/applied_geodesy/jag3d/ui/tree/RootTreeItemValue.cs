﻿/// <summary>
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
	using TabType = org.applied_geodesy.jag3d.ui.tabpane.TabType;

	public class RootTreeItemValue : TreeItemValue
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: RootTreeItemValue(String name) throws IllegalArgumentException
		internal RootTreeItemValue(string name) : base(TreeItemType.ROOT, name)
		{
		}

		public override TabType[] TabTypes
		{
			get
			{
				TabType[] tabTypes = new TabType[] {TabType.META_DATA, TabType.RESULT_DATA, TabType.GRAPHIC};
				return tabTypes;
			}
		}
	}

}