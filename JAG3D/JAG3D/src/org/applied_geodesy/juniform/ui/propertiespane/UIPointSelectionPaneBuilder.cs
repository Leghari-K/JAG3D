using System;
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

namespace org.applied_geodesy.juniform.ui.propertiespane
{

	using GeometricPrimitive = org.applied_geodesy.adjustment.geometry.GeometricPrimitive;
	using FeaturePoint = org.applied_geodesy.adjustment.geometry.point.FeaturePoint;
	using org.applied_geodesy.util;
	using I18N = org.applied_geodesy.juniform.ui.i18n.I18N;

	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using FXCollections = javafx.collections.FXCollections;
	using ObservableList = javafx.collections.ObservableList;
	using FilteredList = javafx.collections.transformation.FilteredList;
	using ActionEvent = javafx.@event.ActionEvent;
	using EventHandler = javafx.@event.EventHandler;
	using Insets = javafx.geometry.Insets;
	using Pos = javafx.geometry.Pos;
	using Node = javafx.scene.Node;
	using Button = javafx.scene.control.Button;
	using CheckBox = javafx.scene.control.CheckBox;
	using Control = javafx.scene.control.Control;
	using Label = javafx.scene.control.Label;
	using ListCell = javafx.scene.control.ListCell;
	using ListView = javafx.scene.control.ListView;
	using SelectionMode = javafx.scene.control.SelectionMode;
	using TextField = javafx.scene.control.TextField;
	using TitledPane = javafx.scene.control.TitledPane;
	using Tooltip = javafx.scene.control.Tooltip;
	using GridPane = javafx.scene.layout.GridPane;
	using Priority = javafx.scene.layout.Priority;
	using VBox = javafx.scene.layout.VBox;
	using Callback = javafx.util.Callback;

	public class UIPointSelectionPaneBuilder
	{
		private enum SelectionEventType
		{
			ADD,
			REMOVE,
			ADD_ALL,
			REMOVE_ALL
		}

		private enum ListType
		{
			GLOBAL_POINTS,
			GEOMETRY_POINTS
		}

		private class FilterModeChangeListener : ChangeListener<bool>
		{
			private readonly UIPointSelectionPaneBuilder outerInstance;

			internal readonly ListType listType;
			internal FilterModeChangeListener(UIPointSelectionPaneBuilder outerInstance, ListType listType)
			{
				this.outerInstance = outerInstance;
				this.listType = listType;
			}

			public override void changed<T1>(ObservableValue<T1> observable, bool? oldValue, bool? newValue) where T1 : bool
			{
				outerInstance.filterList(this.listType, null);
			}
		}

		private class FilterTextChangeListener : ChangeListener<string>
		{
			private readonly UIPointSelectionPaneBuilder outerInstance;

			internal readonly ListType listType;
			internal FilterTextChangeListener(UIPointSelectionPaneBuilder outerInstance, ListType listType)
			{
				this.outerInstance = outerInstance;
				this.listType = listType;
			}

			public override void changed<T1>(ObservableValue<T1> observable, string oldValue, string newValue) where T1 : string
			{
				outerInstance.filterList(this.listType, newValue);
			}
		}

		private class PointSelectionEventHandler : EventHandler<ActionEvent>
		{
			private readonly UIPointSelectionPaneBuilder outerInstance;

			public PointSelectionEventHandler(UIPointSelectionPaneBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(ActionEvent @event)
			{
				if (@event.getSource() is Button && ((Button)@event.getSource()).getUserData() is SelectionEventType)
				{
					Button button = (Button)@event.getSource();
					SelectionEventType selectionEventType = (SelectionEventType)button.getUserData();

					switch (selectionEventType)
					{
					case org.applied_geodesy.juniform.ui.propertiespane.UIPointSelectionPaneBuilder.SelectionEventType.ADD:
						ICollection<FeaturePoint> selectedSourceItems = new LinkedHashSet<FeaturePoint>(outerInstance.sourceListView.getSelectionModel().getSelectedItems());
						outerInstance.targetPointList.addAll(selectedSourceItems);
						outerInstance.sourcePointList.removeAll(selectedSourceItems);
						outerInstance.targetListView.getSelectionModel().clearSelection();
						break;

					case org.applied_geodesy.juniform.ui.propertiespane.UIPointSelectionPaneBuilder.SelectionEventType.ADD_ALL:
						ICollection<FeaturePoint> filteredSourceItems = new LinkedHashSet<FeaturePoint>(outerInstance.filteredSourcePointList);
						outerInstance.targetPointList.addAll(filteredSourceItems);
						outerInstance.sourcePointList.removeAll(new HashSet<>(filteredSourceItems));
						break;

					case org.applied_geodesy.juniform.ui.propertiespane.UIPointSelectionPaneBuilder.SelectionEventType.REMOVE:
						ICollection<FeaturePoint> selectedTargetItems = new LinkedHashSet<FeaturePoint>(outerInstance.targetListView.getSelectionModel().getSelectedItems());
						outerInstance.targetPointList.removeAll(selectedTargetItems);
						outerInstance.sourcePointList.addAll(selectedTargetItems);
						outerInstance.sourceListView.getSelectionModel().clearSelection();
						break;

					case org.applied_geodesy.juniform.ui.propertiespane.UIPointSelectionPaneBuilder.SelectionEventType.REMOVE_ALL:
						ICollection<FeaturePoint> filteredTargetItems = new LinkedHashSet<FeaturePoint>(outerInstance.filteredTargetPointList);
						outerInstance.sourcePointList.addAll(filteredTargetItems);
						outerInstance.targetPointList.removeAll(filteredTargetItems);
						break;
					}
				}
			}
		}

		private static UIPointSelectionPaneBuilder pointSelectionPaneBuilder = new UIPointSelectionPaneBuilder();
		private Node pointSelectionNode = null;
		private ListView<FeaturePoint> sourceListView;
		private ListView<FeaturePoint> targetListView;

		private FilteredList<FeaturePoint> filteredTargetPointList;
		private FilteredList<FeaturePoint> filteredSourcePointList;

		private ObservableUniqueList<FeaturePoint> targetPointList;
		private ObservableList<FeaturePoint> sourcePointList;

		private CheckBox regExpSourceListFilterCheckBox;
		private CheckBox regExpTargetListFilterCheckBox;
		private TextField sourceListFilterTextField;
		private TextField targetListFilterTextField;

		private I18N i18n = I18N.Instance;

		private UIPointSelectionPaneBuilder() : base()
		{
		}

		public static UIPointSelectionPaneBuilder Instance
		{
			get
			{
				pointSelectionPaneBuilder.init();
				return pointSelectionPaneBuilder;
			}
		}

		public virtual Node getNode(ObservableList<FeaturePoint> sourcePoints, GeometricPrimitive geometricPrimitive)
		{
			this.targetPointList = geometricPrimitive.FeaturePoints;
			this.filteredTargetPointList = new FilteredList<FeaturePoint>(this.targetPointList);

			this.filteredSourcePointList = new FilteredList<FeaturePoint>(sourcePoints);
			this.filteredSourcePointList.setPredicate((FeaturePoint featurePoint) =>
			{
					return !targetPointList.contains(featurePoint);
			});

			this.sourcePointList = FXCollections.observableArrayList(this.filteredSourcePointList);
			this.filteredSourcePointList = new FilteredList<FeaturePoint>(this.sourcePointList);

			this.sourceListView.setItems(this.filteredSourcePointList);
			this.targetListView.setItems(this.filteredTargetPointList);

			// apply last filter to point list
			foreach (ListType listType in (ListType[])Enum.GetValues(typeof(ListType)))
			{
				filterList(listType, null);
			}

			return this.pointSelectionNode;
		}



		private void init()
		{
			if (this.pointSelectionNode != null)
			{
				return;
			}

			// List of points
			this.sourceListView = this.createListView();
			this.targetListView = this.createListView();

			// usnig regexp
			this.regExpSourceListFilterCheckBox = this.createCheckBox(i18n.getString("UIPointSelectionPaneBuilder.mode.regex.global.label", "Regular expression"), i18n.getString("UIPointSelectionPaneBuilder.mode.regex.global.tooltip", "If selected, regular expression mode will be applied to global point list"), ListType.GLOBAL_POINTS);

			this.regExpTargetListFilterCheckBox = this.createCheckBox(i18n.getString("UIPointSelectionPaneBuilder.mode.regex.geometry.label", "Regular expression"), i18n.getString("UIPointSelectionPaneBuilder.mode.regex.geometry.tooltip", "If selected, regular expression mode will be applied to geometry point list"), ListType.GEOMETRY_POINTS);

			// Filtering textfields
			this.sourceListFilterTextField = this.createTextField(i18n.getString("UIPointSelectionPaneBuilder.filter.list.global.prompt", "Enter filter sequence"), i18n.getString("UIPointSelectionPaneBuilder.filter.list.global.tooltip", "Filtering global point list by character sequence"), ListType.GLOBAL_POINTS);

			this.targetListFilterTextField = this.createTextField(i18n.getString("UIPointSelectionPaneBuilder.filter.list.geometry.prompt", "Enter filter sequence"), i18n.getString("UIPointSelectionPaneBuilder.filter.list.geometry.tooltip", "Filtering geometry point list by character sequence"), ListType.GEOMETRY_POINTS);

			// Selection buttons		
			PointSelectionEventHandler pointSelectionEventHandler = new PointSelectionEventHandler(this);
			Button addSelectedPointsButton = this.createButton(i18n.getString("UIPointSelectionPaneBuilder.add.selection.label", "Add selection \u25B7"), i18n.getString("UIPointSelectionPaneBuilder.add.selection.tooltip", "Add selected points to geometric primitive."), pointSelectionEventHandler, SelectionEventType.ADD);

			Button removeSelectedPointsButton = this.createButton(i18n.getString("UIPointSelectionPaneBuilder.remove.selection.label", "\u25C1 Remove selection"), i18n.getString("UIPointSelectionPaneBuilder.remove.selection.tooltip", "Remove selected points from geometric primitive"), pointSelectionEventHandler, SelectionEventType.REMOVE);

			Button addAllPointsButton = this.createButton(i18n.getString("UIPointSelectionPaneBuilder.add.all.label", "Add all \u25B6"), i18n.getString("UIPointSelectionPaneBuilder.add.all.tooltip", "Add all points to geometric primitive"), pointSelectionEventHandler, SelectionEventType.ADD_ALL);

			Button removeAllPointsButton = this.createButton(i18n.getString("UIPointSelectionPaneBuilder.remove.all.label", "\u25C0 Remove all"), i18n.getString("UIPointSelectionPaneBuilder.remove.all.tooltip", "Remove all points from geometric primitive"), pointSelectionEventHandler, SelectionEventType.REMOVE_ALL);

			TitledPane sourceTitledPane = this.createTitledPane(i18n.getString("UIPointSelectionPaneBuilder.pointlist.global.title", "Points of project"));
			TitledPane targetTitledPane = this.createTitledPane(i18n.getString("UIPointSelectionPaneBuilder.pointlist.geometry.title", "Points of geometry"));

			sourceTitledPane.setContent(this.sourceListView);
			targetTitledPane.setContent(this.targetListView);

			VBox topVbox = new VBox();
			topVbox.setMaxSize(double.MaxValue,double.MaxValue);
			topVbox.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			VBox bottomVbox = new VBox();
			bottomVbox.setMaxSize(double.MaxValue,double.MaxValue);
			bottomVbox.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			VBox buttonVbox = new VBox();
			buttonVbox.setMaxSize(double.MaxValue,double.MaxValue);
			buttonVbox.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

														// top, right, bottom, left 
			VBox.setMargin(addSelectedPointsButton, new Insets(5, 10, 5, 10));
			VBox.setMargin(removeSelectedPointsButton, new Insets(5, 10, 10, 10));
			VBox.setMargin(addAllPointsButton, new Insets(10, 10, 5, 10));
			VBox.setMargin(removeAllPointsButton, new Insets(5, 10, 5, 10));

			VBox.setVgrow(addSelectedPointsButton, Priority.ALWAYS);
			VBox.setVgrow(removeSelectedPointsButton, Priority.ALWAYS);
			VBox.setVgrow(addAllPointsButton, Priority.ALWAYS);
			VBox.setVgrow(removeAllPointsButton, Priority.ALWAYS);

			buttonVbox.getChildren().addAll(addSelectedPointsButton, removeSelectedPointsButton, addAllPointsButton, removeAllPointsButton);

			GridPane gridPane = this.createGridPane();

			GridPane.setMargin(sourceTitledPane, new Insets(2, 5, 2, 2)); // top, right, bottom, left
			GridPane.setMargin(targetTitledPane, new Insets(2, 2, 2, 5)); // top, right, bottom, left

			GridPane.setMargin(this.regExpSourceListFilterCheckBox, new Insets(0, 5, 2, 2)); // top, right, bottom, left
			GridPane.setMargin(this.regExpTargetListFilterCheckBox, new Insets(0, 2, 2, 5)); // top, right, bottom, left

			GridPane.setMargin(this.sourceListFilterTextField, new Insets(2, 5, 0, 2)); // top, right, bottom, left
			GridPane.setMargin(this.targetListFilterTextField, new Insets(2, 2, 0, 5)); // top, right, bottom, left

			gridPane.setAlignment(Pos.TOP_CENTER);
	//		gridPane.setGridLinesVisible(true);

			GridPane.setHgrow(this.sourceListFilterTextField, Priority.ALWAYS);
			GridPane.setVgrow(this.sourceListFilterTextField, Priority.NEVER);

			GridPane.setHgrow(this.targetListFilterTextField, Priority.ALWAYS);
			GridPane.setVgrow(this.targetListFilterTextField, Priority.NEVER);

			GridPane.setHgrow(this.regExpSourceListFilterCheckBox, Priority.NEVER);
			GridPane.setVgrow(this.regExpSourceListFilterCheckBox, Priority.NEVER);

			GridPane.setHgrow(this.regExpTargetListFilterCheckBox, Priority.NEVER);
			GridPane.setVgrow(this.regExpTargetListFilterCheckBox, Priority.NEVER);


			GridPane.setHgrow(sourceTitledPane, Priority.ALWAYS);
			GridPane.setVgrow(sourceTitledPane, Priority.ALWAYS);

			GridPane.setHgrow(targetTitledPane, Priority.ALWAYS);
			GridPane.setVgrow(targetTitledPane, Priority.ALWAYS);

			GridPane.setHgrow(topVbox, Priority.NEVER);
			GridPane.setVgrow(topVbox, Priority.ALWAYS);

			GridPane.setHgrow(buttonVbox, Priority.NEVER);
			GridPane.setVgrow(buttonVbox, Priority.NEVER);

			GridPane.setHgrow(bottomVbox, Priority.NEVER);
			GridPane.setVgrow(bottomVbox, Priority.ALWAYS);

			int row = 0;

			gridPane.add(this.sourceListFilterTextField, 0, row); // col, row, colspan, rowspan
			gridPane.add(this.targetListFilterTextField, 2, row++);

			gridPane.add(this.regExpSourceListFilterCheckBox, 0, row);
			gridPane.add(this.regExpTargetListFilterCheckBox, 2, row++);

			gridPane.add(sourceTitledPane, 0, row, 1, 3); // col, row, colspan, rowspan
			gridPane.add(targetTitledPane, 2, row, 1, 3);

			gridPane.add(topVbox, 1, row++);
			gridPane.add(buttonVbox, 1, row++);
			gridPane.add(bottomVbox, 1, row++);

			gridPane.setPadding(new Insets(10, 10, 10, 10)); // top, right, bottom, left

	//		ScrollPane scroller = new ScrollPane(gridPane);
	//		scroller.setPadding(new Insets(10, 15, 10, 15)); // top, right, bottom, left 
	//		scroller.setFitToHeight(true);
	//		scroller.setFitToWidth(true);
			this.pointSelectionNode = gridPane;
		}


		private Button createButton(string title, string tooltip, PointSelectionEventHandler pointSelectionEventHandler, SelectionEventType selectionEventType)
		{
			Label label = new Label(title);
			label.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			label.setMaxSize(double.MaxValue, Control.USE_PREF_SIZE);
			label.setAlignment(Pos.CENTER);
			label.setPadding(new Insets(0,0,0,3));
			Button button = new Button();
			button.setGraphic(label);
			button.setTooltip(new Tooltip(tooltip));
			button.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			button.setMaxSize(double.MaxValue, double.MaxValue);
			button.setUserData(selectionEventType);
			button.setOnAction(pointSelectionEventHandler);
			return button;
		}

		private ListView<FeaturePoint> createListView()
		{
			ListView<FeaturePoint> listView = new ListView<FeaturePoint>();
			listView.setCellFactory(createFeaturePointCellFactory());
			listView.setMinSize(50, 100);
			listView.setPrefSize(50, 100);
			listView.setMaxSize(double.MaxValue, double.MaxValue);
			listView.getSelectionModel().setSelectionMode(SelectionMode.MULTIPLE);
			ListView<string> placeholderListView = new ListView<string>();
			placeholderListView.setDisable(true);
			placeholderListView.getItems().add("");
			listView.setPlaceholder(placeholderListView);

			return listView;
		}

		public static Callback<ListView<FeaturePoint>, ListCell<FeaturePoint>> createFeaturePointCellFactory()
		{
			return new CallbackAnonymousInnerClass();
		}

		private class CallbackAnonymousInnerClass : Callback<ListView<FeaturePoint>, ListCell<FeaturePoint>>
		{
			public override ListCell<FeaturePoint> call(ListView<FeaturePoint> listView)
			{
				return new ListCellAnonymousInnerClass(this);};
			}

		private class ListCellAnonymousInnerClass : ListCell<FeaturePoint>
		{
			private readonly CallbackAnonymousInnerClass outerInstance;

			public ListCellAnonymousInnerClass(CallbackAnonymousInnerClass outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			protected internal override void updateItem(FeaturePoint featurePoint, bool empty)
			{
				base.updateItem(featurePoint, empty);

				if (empty || featurePoint == null || string.ReferenceEquals(featurePoint.Name, null))
				{
					this.setText(null);
					this.setGraphic(null);
				}
				else
				{
					this.setText(featurePoint.Name);
				}
			}
		}

		private void filterList(ListType listType, string value)
		{
			FilteredList<FeaturePoint> filteredList = null;
			CheckBox regExpFilterCheckBox = null;
			TextField listFilterTextField = null;
			switch (listType)
			{
			case org.applied_geodesy.juniform.ui.propertiespane.UIPointSelectionPaneBuilder.ListType.GLOBAL_POINTS:
				regExpFilterCheckBox = outerInstance.regExpSourceListFilterCheckBox;
				listFilterTextField = outerInstance.sourceListFilterTextField;
				filteredList = outerInstance.filteredSourcePointList;
				break;
			case org.applied_geodesy.juniform.ui.propertiespane.UIPointSelectionPaneBuilder.ListType.GEOMETRY_POINTS:
				regExpFilterCheckBox = outerInstance.regExpTargetListFilterCheckBox;
				listFilterTextField = outerInstance.targetListFilterTextField;
				filteredList = outerInstance.filteredTargetPointList;
				break;
			}

			if (filteredList != null && listFilterTextField != null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String filterText = value == null || value.isBlank() ? listFilterTextField.getText() : value;
				string filterText = string.ReferenceEquals(value, null) || value.Trim().Length == 0 ? listFilterTextField.getText() : value;
				if (string.ReferenceEquals(filterText, null) || filterText.Trim().Length == 0)
				{
					filteredList.setPredicate(null);
				}
				else
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean regExp = regExpFilterCheckBox != null && regExpFilterCheckBox.isSelected();
					bool regExp = regExpFilterCheckBox != null && regExpFilterCheckBox.isSelected();
					filteredList.setPredicate((FeaturePoint featurePoint) =>
					{
						if (!regExp)
						{
							return featurePoint.getName().contains(filterText);
						}
						else
						{
							try
							{
								return featurePoint.getName().matches(filterText);
							}
							catch (Exception)
							{
							}
							return true;
						}
					});
				}
			}
		}

		private TitledPane createTitledPane(string title)
		{
			TitledPane titledPane = new TitledPane();
			titledPane.setMinSize(Control.USE_PREF_SIZE, 200);
			titledPane.setMaxSize(double.MaxValue, double.MaxValue);
			titledPane.setCollapsible(false);
			titledPane.setAnimated(false);
			titledPane.setText(title);
			return titledPane;
		}

		private TextField createTextField(string promptText, string tooltipText, ListType listType)
		{
			TextField field = new TextField();
			field.setTooltip(new Tooltip(tooltipText));
			field.setPromptText(promptText);
			field.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			field.setMaxSize(double.MaxValue, Control.USE_PREF_SIZE);
			field.textProperty().addListener(new FilterTextChangeListener(listType));
			return field;
		}

		private CheckBox createCheckBox(string text, string tooltip, ListType listType)
		{
			Label label = new Label(text);
			label.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			label.setPadding(new Insets(0,0,0,3));
			CheckBox checkBox = new CheckBox();
			checkBox.setGraphic(label);
			checkBox.setTooltip(new Tooltip(tooltip));
			checkBox.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			checkBox.setMaxSize(double.MaxValue, Control.USE_PREF_SIZE);
			checkBox.selectedProperty().addListener(new FilterModeChangeListener(listType));
			return checkBox;
		}

		private GridPane createGridPane()
		{
			GridPane gridPane = new GridPane();
			gridPane.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			gridPane.setMaxSize(double.MaxValue, double.MaxValue);
			gridPane.setHgap(10);
			gridPane.setVgap(10);
			return gridPane;
		}
		}

	}