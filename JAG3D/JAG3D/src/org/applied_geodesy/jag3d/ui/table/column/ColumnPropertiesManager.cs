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

namespace org.applied_geodesy.jag3d.ui.table.column
{

	using FXCollections = javafx.collections.FXCollections;
	using ObservableList = javafx.collections.ObservableList;
	using Pair = javafx.util.Pair;

	public class ColumnPropertiesManager : IEnumerable<KeyValuePair<Pair<TableContentType, ColumnContentType>, ColumnProperty>>
	{
		private static ColumnPropertiesManager columnPropertiesManager = new ColumnPropertiesManager();
		private IDictionary<Pair<TableContentType, ColumnContentType>, ColumnProperty> properties = new Dictionary<Pair<TableContentType, ColumnContentType>, ColumnProperty>();
		private IDictionary<TableContentType, ObservableList<ColumnContentType>> tableSortOrder = new Dictionary<TableContentType, ObservableList<ColumnContentType>>();
		private IDictionary<TableContentType, ObservableList<ColumnContentType>> tableColumnsOrder = new Dictionary<TableContentType, ObservableList<ColumnContentType>>();

		private ColumnPropertiesManager()
		{
		}

		public static ColumnPropertiesManager Instance
		{
			get
			{
				return columnPropertiesManager;
			}
		}

		public virtual ColumnProperty getProperty(TableContentType tableType, ColumnContentType columnType)
		{
			Pair<TableContentType, ColumnContentType> key = new Pair<TableContentType, ColumnContentType>(tableType, columnType);

			if (!this.properties.ContainsKey(key))
			{
				ColumnProperty property = new ColumnProperty(columnType);
				this.properties[key] = property;
			}

			return this.properties[key];
		}

		public virtual IEnumerator<KeyValuePair<Pair<TableContentType, ColumnContentType>, ColumnProperty>> GetEnumerator()
		{
			return this.properties.SetOfKeyValuePairs().GetEnumerator();
		}

		public virtual void clearOrder()
		{
			foreach (ObservableList<ColumnContentType> columnTypes in this.tableColumnsOrder.Values)
			{
				columnTypes.clear();
			}
			foreach (ObservableList<ColumnContentType> columnTypes in this.tableSortOrder.Values)
			{
				columnTypes.clear();
			}
		}

		public virtual ObservableList<ColumnContentType> getSortOrder(TableContentType tableType)
		{
			if (!this.tableSortOrder.ContainsKey(tableType))
			{
				ObservableList<ColumnContentType> columnTypes = FXCollections.observableArrayList();
				this.tableSortOrder[tableType] = columnTypes;
			}
			return this.tableSortOrder[tableType];
		}

		public virtual ObservableList<ColumnContentType> getColumnsOrder(TableContentType tableType)
		{
			if (!this.tableColumnsOrder.ContainsKey(tableType))
			{
				ObservableList<ColumnContentType> columnTypes = FXCollections.observableArrayList();
				this.tableColumnsOrder[tableType] = columnTypes;
			}
			return this.tableColumnsOrder[tableType];
		}
	}

}