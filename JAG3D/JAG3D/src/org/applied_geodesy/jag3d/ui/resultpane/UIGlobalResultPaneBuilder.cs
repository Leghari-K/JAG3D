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

namespace org.applied_geodesy.jag3d.ui.resultpane
{
	using ProjectDatabaseStateChangeListener = org.applied_geodesy.jag3d.sql.ProjectDatabaseStateChangeListener;
	using ProjectDatabaseStateEvent = org.applied_geodesy.jag3d.sql.ProjectDatabaseStateEvent;
	using ProjectDatabaseStateType = org.applied_geodesy.jag3d.sql.ProjectDatabaseStateType;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using UIPrincipalComponentTableBuilder = org.applied_geodesy.jag3d.ui.table.UIPrincipalComponentTableBuilder;
	using UITestStatisticTableBuilder = org.applied_geodesy.jag3d.ui.table.UITestStatisticTableBuilder;
	using UIVarianceComponentTableBuilder = org.applied_geodesy.jag3d.ui.table.UIVarianceComponentTableBuilder;
	using PrincipalComponentRow = org.applied_geodesy.jag3d.ui.table.row.PrincipalComponentRow;
	using Row = org.applied_geodesy.jag3d.ui.table.row.Row;
	using TestStatisticRow = org.applied_geodesy.jag3d.ui.table.row.TestStatisticRow;
	using VarianceComponentRow = org.applied_geodesy.jag3d.ui.table.row.VarianceComponentRow;
	using ColumnType = org.applied_geodesy.ui.table.ColumnType;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;

	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using Insets = javafx.geometry.Insets;
	using Node = javafx.scene.Node;
	using ComboBox = javafx.scene.control.ComboBox;
	using ListCell = javafx.scene.control.ListCell;
	using ListView = javafx.scene.control.ListView;
	using TableColumn = javafx.scene.control.TableColumn;
	using TableView = javafx.scene.control.TableView;
	using Tooltip = javafx.scene.control.Tooltip;
	using BorderPane = javafx.scene.layout.BorderPane;
	using HBox = javafx.scene.layout.HBox;
	using Priority = javafx.scene.layout.Priority;
	using Region = javafx.scene.layout.Region;
	using StackPane = javafx.scene.layout.StackPane;
	using Callback = javafx.util.Callback;

	public class UIGlobalResultPaneBuilder
	{
		private class DatabaseStateChangeListener : ProjectDatabaseStateChangeListener
		{
			private readonly UIGlobalResultPaneBuilder outerInstance;

			public DatabaseStateChangeListener(UIGlobalResultPaneBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public virtual void projectDatabaseStateChanged(ProjectDatabaseStateEvent evt)
			{
				if (evt.EventType == ProjectDatabaseStateType.CLOSED)
				{
					// clear all global result tables
					UITestStatisticTableBuilder.Instance.Table.getItems().setAll(UITestStatisticTableBuilder.Instance.EmptyRow);
					UIVarianceComponentTableBuilder.Instance.getTable(UIVarianceComponentTableBuilder.VarianceComponentDisplayType.OVERALL_COMPONENTS).getItems().setAll(UIVarianceComponentTableBuilder.Instance.EmptyRow);
					UIPrincipalComponentTableBuilder.Instance.Table.getItems().setAll(UIPrincipalComponentTableBuilder.Instance.EmptyRow);
				}
			}
		}

		private static UIGlobalResultPaneBuilder resultPaneBuilder = new UIGlobalResultPaneBuilder();

		private Node resultDataNode = null;

		private UIGlobalResultPaneBuilder()
		{
		}

		public static UIGlobalResultPaneBuilder Instance
		{
			get
			{
				resultPaneBuilder.init();
				return resultPaneBuilder;
			}
		}

		public virtual Node Node
		{
			get
			{
				return this.resultDataNode;
			}
		}

		private void init()
		{

			if (this.resultDataNode != null)
			{
				return;
			}

			I18N i18n = I18N.Instance;

			TableView<TestStatisticRow> testStatisticTable = UITestStatisticTableBuilder.Instance.Table;
			testStatisticTable.setUserData(GlobalResultType.TEST_STATISTIC);
			testStatisticTable.setMaxSize(double.MaxValue, double.MaxValue);

			TableView<VarianceComponentRow> varianceComponentTable = UIVarianceComponentTableBuilder.Instance.getTable(UIVarianceComponentTableBuilder.VarianceComponentDisplayType.OVERALL_COMPONENTS);
			this.TableColumnView = varianceComponentTable;
			varianceComponentTable.setUserData(GlobalResultType.VARIANCE_COMPONENT);
			varianceComponentTable.setMaxSize(double.MaxValue, double.MaxValue);

			TableView<PrincipalComponentRow> principalComponentTable = UIPrincipalComponentTableBuilder.Instance.Table;
			principalComponentTable.setUserData(GlobalResultType.PRINCIPAL_COMPONENT);
			principalComponentTable.setMaxSize(double.MaxValue, double.MaxValue);

			StackPane contenPane = new StackPane();
			contenPane.setPadding(new Insets(10, 30, 10, 30)); // oben, rechts, unten, links
			contenPane.getChildren().addAll(testStatisticTable, varianceComponentTable, principalComponentTable);
			varianceComponentTable.setVisible(false);
			principalComponentTable.setVisible(false);

			ComboBox<Node> paneSwitcherComboBox = new ComboBox<Node>();
			paneSwitcherComboBox.setCellFactory(new CallbackAnonymousInnerClass(this));
			paneSwitcherComboBox.setButtonCell(new GlobalResultTypeListCell());
			paneSwitcherComboBox.getItems().addAll(testStatisticTable, varianceComponentTable, principalComponentTable);
			paneSwitcherComboBox.getSelectionModel().selectedItemProperty().addListener(new ChangeListenerAnonymousInnerClass(this));
			paneSwitcherComboBox.getSelectionModel().select(varianceComponentTable);
			paneSwitcherComboBox.setTooltip(new Tooltip(i18n.getString("UIGlobalResultPaneBuilder.global_result_switcher.tooltip", "Global network adjustment results")));
			Region spacer = new Region();
			HBox hbox = new HBox(10);
			hbox.setPadding(new Insets(15, 30, 5, 0));
			HBox.setHgrow(spacer, Priority.ALWAYS);
			hbox.getChildren().addAll(spacer, paneSwitcherComboBox);

			BorderPane borderPane = new BorderPane();
			borderPane.setTop(hbox);
			borderPane.setCenter(contenPane);

			this.resultDataNode = borderPane;

			SQLManager.Instance.addProjectDatabaseStateChangeListener(new DatabaseStateChangeListener(this));
		}

		private class CallbackAnonymousInnerClass : Callback<ListView<Node>, ListCell<Node>>
		{
			private readonly UIGlobalResultPaneBuilder outerInstance;

			public CallbackAnonymousInnerClass(UIGlobalResultPaneBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override ListCell<Node> call(ListView<Node> param)
			{
				return new GlobalResultTypeListCell();
			}
		}

		private class ChangeListenerAnonymousInnerClass : ChangeListener<Node>
		{
			private readonly UIGlobalResultPaneBuilder outerInstance;

			public ChangeListenerAnonymousInnerClass(UIGlobalResultPaneBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public override void changed<T1>(ObservableValue<T1> observable, Node oldValue, Node newValue) where T1 : javafx.scene.Node
			{
				if (oldValue != null)
				{
					oldValue.setVisible(false);
				}
				if (newValue != null)
				{
					newValue.setVisible(true);
				}
			}

		}

		private TableView<T1> TableColumnView<T1> where T1 : org.applied_geodesy.jag3d.ui.table.row.Row
		{
			set
			{
				int columnCount = value.getColumns().size();
    
				for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
				{
	//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
	//ORIGINAL LINE: javafx.scene.control.TableColumn<? extends org.applied_geodesy.jag3d.ui.table.row.Row, ?> column = value.getColumns().get(columnIndex);
					TableColumn<Row, object> column = value.getColumns().get(columnIndex);
					if (column.getUserData() is ColumnType)
					{
						ColumnType columnType = (ColumnType)column.getUserData();
						switch (columnType)
						{
						case ColumnType.VISIBLE:
							column.setVisible(true);
    
							break;
						default:
							column.setVisible(false);
    
							break;
						}
					}
				}
			}
		}
	}

}