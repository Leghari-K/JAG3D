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

namespace org.applied_geodesy.jag3d.ui.table
{
	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;
	using AveragedObservationRow = org.applied_geodesy.jag3d.ui.table.row.AveragedObservationRow;
	using ColumnTooltipHeader = org.applied_geodesy.ui.table.ColumnTooltipHeader;
	using ColumnType = org.applied_geodesy.ui.table.ColumnType;
	using org.applied_geodesy.ui.table;
	using CellValueType = org.applied_geodesy.util.CellValueType;

	using Pos = javafx.geometry.Pos;
	using TableCell = javafx.scene.control.TableCell;
	using TableColumn = javafx.scene.control.TableColumn;
	using TableView = javafx.scene.control.TableView;
	using Callback = javafx.util.Callback;

	public class UIAverageObservationTableBuilder : UITableBuilder<AveragedObservationRow>
	{
		private enum DisplayFormatType
		{
			NORMAL,
			RESIDUAL
		}
		private static UIAverageObservationTableBuilder tableBuilder = new UIAverageObservationTableBuilder();
		private bool isInitialize = false;
		private UIAverageObservationTableBuilder() : base()
		{
		}

		public static UIAverageObservationTableBuilder Instance
		{
			get
			{
				tableBuilder.init();
				return tableBuilder;
			}
		}

		private void init()
		{
			if (this.isInitialize)
			{
				return;
			}

			TableColumn<AveragedObservationRow, string> stringColumn = null;
			TableColumn<AveragedObservationRow, ObservationType> observationTypeColumn = null;
			TableColumn<AveragedObservationRow, double> doubleColumn = null;

			TableView<AveragedObservationRow> table = this.createTable();

			// Observation type
			int columnIndex = table.getColumns().size();
			string labelText = i18n.getString("UIAverageObservationTableBuilder.tableheader.type.label", "Observation");
			string tooltipText = i18n.getString("UIAverageObservationTableBuilder.tableheader.type.tooltip", "Type of the observation");
			ColumnTooltipHeader header = new ColumnTooltipHeader(CellValueType.STRING, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			observationTypeColumn = this.getColumn<ObservationType>(header, AveragedObservationRow::observationTypeProperty, ObservationTypeCallback, ColumnType.VISIBLE, columnIndex, false);
			table.getColumns().add(observationTypeColumn);

			// Station-ID
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIAverageObservationTableBuilder.tableheader.station.name.label", "Station-Id");
			tooltipText = i18n.getString("UIAverageObservationTableBuilder.tableheader.station.name.tooltip", "Id of station");
			header = new ColumnTooltipHeader(CellValueType.STRING, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			stringColumn = this.getColumn<string>(header, AveragedObservationRow::startPointNameProperty, StringCallback, ColumnType.VISIBLE, columnIndex, false);
			stringColumn.setComparator(new NaturalOrderComparator<string>());
			table.getColumns().add(stringColumn);

			// Target-ID
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIAverageObservationTableBuilder.tableheader.target.name.label", "Target-Id");
			tooltipText = i18n.getString("UIAverageObservationTableBuilder.tableheader.target.name.tooltip", "Id of target point");
			header = new ColumnTooltipHeader(CellValueType.STRING, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			stringColumn = this.getColumn<string>(header, AveragedObservationRow::endPointNameProperty, StringCallback, ColumnType.VISIBLE, columnIndex, false);
			stringColumn.setComparator(new NaturalOrderComparator<string>());
			table.getColumns().add(stringColumn);

			// Parameter value
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIAverageObservationTableBuilder.tableheader.value.label", "Value");
			tooltipText = i18n.getString("UIAverageObservationTableBuilder.tableheader.value.tooltip", "A-priori observation");
			header = new ColumnTooltipHeader(CellValueType.STRING, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, AveragedObservationRow::valueProperty, getDoubleValueWithUnitCallback(DisplayFormatType.NORMAL), ColumnType.VISIBLE, columnIndex, false);
			table.getColumns().add(doubleColumn);

			// Nabla
			columnIndex = table.getColumns().size();
			labelText = i18n.getString("UIAverageObservationTableBuilder.tableheader.grosserror.label", "\u2207");
			tooltipText = i18n.getString("UIAverageObservationTableBuilder.tableheader.grosserror.tooltip", "Deviation w.r.t. median");
			header = new ColumnTooltipHeader(CellValueType.STRING, labelText, tooltipText);
//JAVA TO C# CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			doubleColumn = this.getColumn<double>(header, AveragedObservationRow::grossErrorProperty, getDoubleValueWithUnitCallback(DisplayFormatType.RESIDUAL), ColumnType.VISIBLE, columnIndex, false);
			table.getColumns().add(doubleColumn);

			table.setColumnResizePolicy(TableView.CONSTRAINED_RESIZE_POLICY);
			this.table = table;
			this.isInitialize = true;
		}

		// https://stackoverflow.com/questions/27281370/javafx-tableview-format-one-cell-based-on-the-value-of-another-in-the-row
		private static Callback<TableColumn<AveragedObservationRow, ObservationType>, TableCell<AveragedObservationRow, ObservationType>> ObservationTypeCallback
		{
			get
			{
				return new CallbackAnonymousInnerClass();
			}
		}

		private class CallbackAnonymousInnerClass : Callback<TableColumn<AveragedObservationRow, ObservationType>, TableCell<AveragedObservationRow, ObservationType>>
		{
			public override TableCell<AveragedObservationRow, ObservationType> call(TableColumn<AveragedObservationRow, ObservationType> cell)
			{
				TableCell<AveragedObservationRow, ObservationType> tableCell = new TableCellAnonymousInnerClass(this, cell);
				tableCell.setAlignment(Pos.CENTER_LEFT);
				return tableCell;
			}

			private class TableCellAnonymousInnerClass : TableCell<AveragedObservationRow, ObservationType>
			{
				private readonly CallbackAnonymousInnerClass outerInstance;

				private TableColumn<AveragedObservationRow, ObservationType> cell;

				public TableCellAnonymousInnerClass(CallbackAnonymousInnerClass outerInstance, TableColumn<AveragedObservationRow, ObservationType> cell)
				{
					this.outerInstance = outerInstance;
					this.cell = cell;
				}

				protected internal override void updateItem(ObservationType value, bool empty)
				{
					int currentIndex = indexProperty().getValue();
					if (!empty && value != null && currentIndex >= 0 && currentIndex < cell.getTableView().getItems().size())
					{
						AveragedObservationRow row = cell.getTableView().getItems().get(currentIndex);
						if (row != null && row.ObservationType != null)
						{
							string component = null;
							if (row.ComponentType != null)
							{
								switch (row.ComponentType)
								{
								case org.applied_geodesy.adjustment.network.observation.ComponentType.X:
									component = i18n.getString("UIAverageObservationTableBuilder.gnss.x.label", "x");
									break;
								case org.applied_geodesy.adjustment.network.observation.ComponentType.Y:
									component = i18n.getString("UIAverageObservationTableBuilder.gnss.y.label", "y");
									break;
								case org.applied_geodesy.adjustment.network.observation.ComponentType.Z:
									component = i18n.getString("UIAverageObservationTableBuilder.gnss.z.label", "z");
									break;
								}
							}

							switch (row.ObservationType.innerEnumValue)
							{
							case ObservationType.InnerEnum.LEVELING:
								this.setText(i18n.getString("UIAverageObservationTableBuilder.leveling.label", "Leveling"));
								break;
							case ObservationType.InnerEnum.DIRECTION:
								this.setText(i18n.getString("UIAverageObservationTableBuilder.direction.label", "Direction"));
								break;
							case ObservationType.InnerEnum.HORIZONTAL_DISTANCE:
								this.setText(i18n.getString("UIAverageObservationTableBuilder.horizontal_distance.label", "Horizontal distance"));
								break;
							case ObservationType.InnerEnum.SLOPE_DISTANCE:
								this.setText(i18n.getString("UIAverageObservationTableBuilder.slope_distance.label", "Slope distance"));
								break;
							case ObservationType.InnerEnum.ZENITH_ANGLE:
								this.setText(i18n.getString("UIAverageObservationTableBuilder.zenith_angle.label", "Zenith angle"));
								break;
							case ObservationType.InnerEnum.GNSS1D:
								this.setText(i18n.getString("UIAverageObservationTableBuilder.gnss.1d.label", "GNSS baseline 1D") + (!string.ReferenceEquals(component, null) ? " " + component : ""));
								break;
							case ObservationType.InnerEnum.GNSS2D:
								this.setText(i18n.getString("UIAverageObservationTableBuilder.gnss.2d.label", "GNSS baseline 2D") + (!string.ReferenceEquals(component, null) ? " " + component : ""));
								break;
							case ObservationType.InnerEnum.GNSS3D:
								this.setText(i18n.getString("UIAverageObservationTableBuilder.gnss.3d.label", "GNSS baseline 3D") + (!string.ReferenceEquals(component, null) ? " " + component : ""));
								break;
							}
						}
						else
						{
							setText(null);
						}
					}
					else
					{
						setText(value == null ? null : value.ToString());
					}
				}
			}
		}

		// https://stackoverflow.com/questions/27281370/javafx-tableview-format-one-cell-based-on-the-value-of-another-in-the-row
		private static Callback<TableColumn<AveragedObservationRow, double>, TableCell<AveragedObservationRow, double>> getDoubleValueWithUnitCallback(DisplayFormatType displayFormatType)
		{
			return new CallbackAnonymousInnerClass2(displayFormatType);
		}

		private class CallbackAnonymousInnerClass2 : Callback<TableColumn<AveragedObservationRow, double>, TableCell<AveragedObservationRow, double>>
		{
			private org.applied_geodesy.jag3d.ui.table.UIAverageObservationTableBuilder.DisplayFormatType displayFormatType;

			public CallbackAnonymousInnerClass2(org.applied_geodesy.jag3d.ui.table.UIAverageObservationTableBuilder.DisplayFormatType displayFormatType)
			{
				this.displayFormatType = displayFormatType;
			}

			public override TableCell<AveragedObservationRow, double> call(TableColumn<AveragedObservationRow, double> cell)
			{
				TableCell<AveragedObservationRow, double> tableCell = new TableCellAnonymousInnerClass2(this, cell);
				tableCell.setAlignment(Pos.CENTER_RIGHT);
				return tableCell;
			}

			private class TableCellAnonymousInnerClass2 : TableCell<AveragedObservationRow, double>
			{
				private readonly CallbackAnonymousInnerClass2 outerInstance;

				private TableColumn<AveragedObservationRow, double> cell;

				public TableCellAnonymousInnerClass2(CallbackAnonymousInnerClass2 outerInstance, TableColumn<AveragedObservationRow, double> cell)
				{
					this.outerInstance = outerInstance;
					this.cell = cell;
				}

				protected internal override void updateItem(double? value, bool empty)
				{
					int currentIndex = indexProperty().getValue();
					if (!empty && value != null && currentIndex >= 0 && currentIndex < cell.getTableView().getItems().size())
					{
						AveragedObservationRow row = cell.getTableView().getItems().get(currentIndex);
						if (row != null && row.ObservationType != null)
						{
							switch (row.ObservationType.innerEnumValue)
							{
							case ObservationType.InnerEnum.DIRECTION:
							case ObservationType.InnerEnum.ZENITH_ANGLE:
								if (outerInstance.displayFormatType == DisplayFormatType.NORMAL)
								{
									this.setText(options.toAngleFormat(value.Value, true));
								}
								else if (outerInstance.displayFormatType == DisplayFormatType.RESIDUAL)
								{
									this.setText(options.toAngleResidualFormat(value.Value, true));
								}
								break;
							default:
								if (outerInstance.displayFormatType == DisplayFormatType.NORMAL)
								{
									this.setText(options.toLengthFormat(value.Value, true));
								}
								else if (outerInstance.displayFormatType == DisplayFormatType.RESIDUAL)
								{
									this.setText(options.toLengthResidualFormat(value.Value, true));
								}
								break;
							}
						}
						else
						{
							setText(null);
						}
					}
					else
					{
						setText(value == null ? null : value.ToString());
					}
				}
			}
		}

		public override AveragedObservationRow EmptyRow
		{
			get
			{
				return new AveragedObservationRow();
			}
		}

		internal override void setValue(AveragedObservationRow row, int columnIndex, object oldValue, object newValue)
		{
		}
	}

}