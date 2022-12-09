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

namespace org.applied_geodesy.jag3d.ui.propertiespane
{

	using VerticalDeflectionGroupUncertaintyType = org.applied_geodesy.adjustment.network.VerticalDeflectionGroupUncertaintyType;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;
	using TreeItemType = org.applied_geodesy.jag3d.ui.tree.TreeItemType;
	using UITreeBuilder = org.applied_geodesy.jag3d.ui.tree.UITreeBuilder;
	using VerticalDeflectionTreeItemValue = org.applied_geodesy.jag3d.ui.tree.VerticalDeflectionTreeItemValue;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
	using DoubleTextField = org.applied_geodesy.ui.textfield.DoubleTextField;
	using UncertaintyTextField = org.applied_geodesy.ui.textfield.UncertaintyTextField;
	using CellValueType = org.applied_geodesy.util.CellValueType;

	using FadeTransition = javafx.animation.FadeTransition;
	using SequentialTransition = javafx.animation.SequentialTransition;
	using Platform = javafx.application.Platform;
	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using ActionEvent = javafx.@event.ActionEvent;
	using EventHandler = javafx.@event.EventHandler;
	using Insets = javafx.geometry.Insets;
	using Node = javafx.scene.Node;
	using Control = javafx.scene.control.Control;
	using Label = javafx.scene.control.Label;
	using ProgressIndicator = javafx.scene.control.ProgressIndicator;
	using ScrollPane = javafx.scene.control.ScrollPane;
	using TitledPane = javafx.scene.control.TitledPane;
	using Tooltip = javafx.scene.control.Tooltip;
	using GridPane = javafx.scene.layout.GridPane;
	using HBox = javafx.scene.layout.HBox;
	using Priority = javafx.scene.layout.Priority;
	using Region = javafx.scene.layout.Region;
	using VBox = javafx.scene.layout.VBox;
	using Color = javafx.scene.paint.Color;
	using Font = javafx.scene.text.Font;
	using Duration = javafx.util.Duration;

	public class UIVerticalDeflectionPropertiesPane
	{
		private class NumberChangeListener : ChangeListener<double>
		{
			private readonly UIVerticalDeflectionPropertiesPane outerInstance;

			internal readonly DoubleTextField field;

			internal NumberChangeListener(UIVerticalDeflectionPropertiesPane outerInstance, DoubleTextField field)
			{
				this.outerInstance = outerInstance;
				this.field = field;
			}

			public override void changed<T1>(ObservableValue<T1> observable, double? oldValue, double? newValue) where T1 : double
			{
				if (!outerInstance.ignoreValueUpdate && this.field.getUserData() != null && this.field.getUserData() is VerticalDeflectionGroupUncertaintyType)
				{
					VerticalDeflectionGroupUncertaintyType uncertaintyType = (VerticalDeflectionGroupUncertaintyType)this.field.getUserData();
					outerInstance.save(uncertaintyType);
				}
			}
		}

		private class SequentialTransitionFinishedListener : ChangeListener<EventHandler<ActionEvent>>
		{
			private readonly UIVerticalDeflectionPropertiesPane outerInstance;

			public SequentialTransitionFinishedListener(UIVerticalDeflectionPropertiesPane outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, EventHandler<ActionEvent> oldValue, EventHandler<ActionEvent> newValue) where T1 : javafx.@event.EventHandler<javafx.@event.ActionEvent>
			{
				outerInstance.ProgressIndicatorsVisible = false;
				if (outerInstance.sequentialTransition != null)
				{
					outerInstance.sequentialTransition.setNode(null);
				}
			}
		}

		private I18N i18n = I18N.Instance;
		private Node propertiesNode = null;
		private readonly TreeItemType type;

		private UncertaintyTextField uncertaintyDeflectionXField;
		private UncertaintyTextField uncertaintyDeflectionYField;

		private Label selectionInfoLabel = new Label();

		private IDictionary<object, ProgressIndicator> databaseTransactionProgressIndicators = new Dictionary<object, ProgressIndicator>(10);
		private IDictionary<object, Node> warningIconNodes = new Dictionary<object, Node>(10);
		private SequentialTransition sequentialTransition = new SequentialTransition();

		private bool ignoreValueUpdate = false;
		private VerticalDeflectionTreeItemValue[] selectedVerticalDeflectionItemValues = null;

		internal UIVerticalDeflectionPropertiesPane(TreeItemType type)
		{
			switch (type.innerEnumValue)
			{
			case TreeItemType.InnerEnum.REFERENCE_VERTICAL_DEFLECTION_LEAF:
			case TreeItemType.InnerEnum.STOCHASTIC_VERTICAL_DEFLECTION_LEAF:
			case TreeItemType.InnerEnum.UNKNOWN_VERTICAL_DEFLECTION_LEAF:
				this.type = type;
				this.init();
				break;
			default:
				throw new System.ArgumentException(this.GetType().Name + " Error, unsupported item type " + type);
			}
		}

		public virtual Node Node
		{
			get
			{
				return this.propertiesNode;
			}
		}
		private void reset()
		{
			this.sequentialTransition.stop();
			this.ProgressIndicatorsVisible = false;
			this.WarningIconsVisible = false;

			// set focus to panel to commit text field values and to force db transaction
			UITreeBuilder.Instance.Tree.requestFocus();

			this.UncertaintyDeflectionY = VerticalDeflectionTreeItemValue.getDefaultUncertainty(VerticalDeflectionGroupUncertaintyType.DEFLECTION_Y);
			this.UncertaintyDeflectionX = VerticalDeflectionTreeItemValue.getDefaultUncertainty(VerticalDeflectionGroupUncertaintyType.DEFLECTION_Y);
		}

		public virtual void setTreeItemValue(string name, params VerticalDeflectionTreeItemValue[] selectedVerticalDeflectionItemValues)
		{
			if (this.selectedVerticalDeflectionItemValues != selectedVerticalDeflectionItemValues)
			{
				this.reset();
				this.selectedVerticalDeflectionItemValues = selectedVerticalDeflectionItemValues;
			}
			this.setGroupName(name, this.selectedVerticalDeflectionItemValues != null ? this.selectedVerticalDeflectionItemValues.Length : 0);
		}

		private void setGroupName(string name, int cnt)
		{
			if (this.selectionInfoLabel != null)
			{
				string groupNameTmpl = this.i18n.getString("UIVerticalDeflectionPropertiesPane.status.selection.name.label", "Status:");
				string selectionCntTmpl = cnt > 1 ? String.format(Locale.ENGLISH, this.i18n.getString("UIVerticalDeflectionPropertiesPane.status.selection.counter.label", "and %d more selected group(s)\u2026"), cnt) : "";
				string label = String.format(Locale.ENGLISH, "%s %s %s", groupNameTmpl, name, selectionCntTmpl);
				if (!this.selectionInfoLabel.getText().Equals(label))
				{
					this.selectionInfoLabel.setText(label);
				}
			}
		}

		public virtual bool setUncertaintyDeflectionY(double? value)
		{
			if (this.uncertaintyDeflectionYField == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.uncertaintyDeflectionYField.Value = value != null && value > 0 ? value : null;
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setUncertaintyDeflectionX(double? value)
		{
			if (this.uncertaintyDeflectionXField == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.uncertaintyDeflectionXField.Value = value != null && value > 0 ? value : null;
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setUncertainty(VerticalDeflectionGroupUncertaintyType type, double? value, bool displayWarningIcon)
		{
			if (this.warningIconNodes.ContainsKey(type))
			{
				this.warningIconNodes[type].setVisible(displayWarningIcon);
				this.warningIconNodes[type].setManaged(displayWarningIcon);
			}

			switch (type.innerEnumValue)
			{
			case VerticalDeflectionGroupUncertaintyType.InnerEnum.DEFLECTION_X:
				return this.setUncertaintyDeflectionX(value);
			case VerticalDeflectionGroupUncertaintyType.InnerEnum.DEFLECTION_Y:
				return this.setUncertaintyDeflectionY(value);
			default:
				return false;
			}
		}

		private Node createUncertaintiesPane()
		{
			if (this.type == TreeItemType.STOCHASTIC_VERTICAL_DEFLECTION_LEAF)
			{
				double fieldMinWidth = 200;
				double fieldMaxWidth = 350;

				double sigmaY = VerticalDeflectionTreeItemValue.getDefaultUncertainty(VerticalDeflectionGroupUncertaintyType.DEFLECTION_Y);
				double sigmaX = VerticalDeflectionTreeItemValue.getDefaultUncertainty(VerticalDeflectionGroupUncertaintyType.DEFLECTION_X);

				Node warningIconUncertaintyTypeYNode = this.createWarningIcon(VerticalDeflectionGroupUncertaintyType.DEFLECTION_Y, i18n.getString("UIVerticalDeflectionPropertiesPane.uncertainty.y.warning.label", "\u26A0"), String.format(Locale.ENGLISH, i18n.getString("UIVerticalDeflectionPropertiesPane.uncertainty.y.warning.tooltip", "Note: The selected groups have different values and \u03C3y differs by more than %.1f \u2030."), SQLManager.EQUAL_VALUE_TRESHOLD * 1000.0));
				Node warningIconUncertaintyTypeXNode = this.createWarningIcon(VerticalDeflectionGroupUncertaintyType.DEFLECTION_X, i18n.getString("UIVerticalDeflectionPropertiesPane.uncertainty.x.warning.label", "\u26A0"), String.format(Locale.ENGLISH, i18n.getString("UIVerticalDeflectionPropertiesPane.uncertainty.x.warning.tooltip", "Note: The selected groups have different values and \u03C3x differs by more than %.1f \u2030."), SQLManager.EQUAL_VALUE_TRESHOLD * 1000.0));

				ProgressIndicator uncertaintyDeflectionYProgressIndicator = this.createDatabaseTransactionProgressIndicator(VerticalDeflectionGroupUncertaintyType.DEFLECTION_Y);
				ProgressIndicator uncertaintyDeflectionXProgressIndicator = this.createDatabaseTransactionProgressIndicator(VerticalDeflectionGroupUncertaintyType.DEFLECTION_X);

				Label uncertaintyDeflectionYLabel = new Label(i18n.getString("UIVerticalDeflectionPropertiesPane.uncertainty.y.label", "\u03C3y"));
				Label uncertaintyDeflectionXLabel = new Label(i18n.getString("UIVerticalDeflectionPropertiesPane.uncertainty.x.label", "\u03C3x"));
				uncertaintyDeflectionYLabel.setMinWidth(Control.USE_PREF_SIZE);
				uncertaintyDeflectionXLabel.setMinWidth(Control.USE_PREF_SIZE);

				this.uncertaintyDeflectionYField = new UncertaintyTextField(sigmaY, CellValueType.ANGLE_UNCERTAINTY, true, DoubleTextField.ValueSupport.EXCLUDING_INCLUDING_INTERVAL);
				this.uncertaintyDeflectionYField.setTooltip(new Tooltip(i18n.getString("UIVerticalDeflectionPropertiesPane.uncertainty.y.tooltip", "Uncertainty of y-component of deflection of the vertical")));
				this.uncertaintyDeflectionYField.setUserData(VerticalDeflectionGroupUncertaintyType.DEFLECTION_Y);
				this.uncertaintyDeflectionYField.numberProperty().addListener(new NumberChangeListener(this, this.uncertaintyDeflectionYField));
				this.uncertaintyDeflectionYField.setMinWidth(fieldMinWidth);
				this.uncertaintyDeflectionYField.setMaxWidth(fieldMaxWidth);

				this.uncertaintyDeflectionXField = new UncertaintyTextField(sigmaX, CellValueType.ANGLE_UNCERTAINTY, true, DoubleTextField.ValueSupport.EXCLUDING_INCLUDING_INTERVAL);
				this.uncertaintyDeflectionXField.setTooltip(new Tooltip(i18n.getString("UIVerticalDeflectionPropertiesPane.uncertainty.x.tooltip", "Uncertainty of x-component of deflection of the vertical")));
				this.uncertaintyDeflectionXField.setUserData(VerticalDeflectionGroupUncertaintyType.DEFLECTION_X);
				this.uncertaintyDeflectionXField.numberProperty().addListener(new NumberChangeListener(this, this.uncertaintyDeflectionXField));
				this.uncertaintyDeflectionXField.setMinWidth(fieldMinWidth);
				this.uncertaintyDeflectionXField.setMaxWidth(fieldMaxWidth);

				uncertaintyDeflectionYLabel.setLabelFor(this.uncertaintyDeflectionYField);
				uncertaintyDeflectionXLabel.setLabelFor(this.uncertaintyDeflectionXField);

				GridPane gridPane = this.createGridPane();
	//			GridPane.setHgrow(uncertaintyDeflectionYLabel, Priority.SOMETIMES);
	//			GridPane.setHgrow(uncertaintyDeflectionXLabel, Priority.SOMETIMES);
	//			GridPane.setHgrow(this.uncertaintyDeflectionYField, Priority.ALWAYS);
	//			GridPane.setHgrow(this.uncertaintyDeflectionXField, Priority.ALWAYS);

				gridPane.add(uncertaintyDeflectionYLabel, 0, 0);
				gridPane.add(this.uncertaintyDeflectionYField, 1, 0);
				gridPane.add(new HBox(warningIconUncertaintyTypeYNode, uncertaintyDeflectionYProgressIndicator), 2, 0);

				gridPane.add(uncertaintyDeflectionXLabel, 0, 1);
				gridPane.add(this.uncertaintyDeflectionXField, 1, 1);
				gridPane.add(new HBox(warningIconUncertaintyTypeXNode, uncertaintyDeflectionXProgressIndicator), 2, 1);

				TitledPane uncertaintiesTitledPane = this.createTitledPane(i18n.getString("UIVerticalDeflectionPropertiesPane.uncertainty.title", "Uncertainties of deflection of the vertical"));
				uncertaintiesTitledPane.setContent(gridPane);
				return uncertaintiesTitledPane;
			}
			return null;
		}

		private ProgressIndicator createDatabaseTransactionProgressIndicator(object userData)
		{
			ProgressIndicator progressIndicator = new ProgressIndicator(ProgressIndicator.INDETERMINATE_PROGRESS);

			progressIndicator.setVisible(false);
			progressIndicator.setMinSize(17, 17);
			progressIndicator.setMaxSize(17, 17);
			progressIndicator.setUserData(userData);

			this.databaseTransactionProgressIndicators[userData] = progressIndicator;
			return progressIndicator;
		}

		private Node createWarningIcon(object userData, string text, string tooltip)
		{
			Label label = new Label();

			// Workaround, da setFont auf den Text und den Tooltip angewandt wird
			// https://bugs.openjdk.java.net/browse/JDK-8094344
			Label txtNode = new Label(text);
			txtNode.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			txtNode.setMaxSize(double.MaxValue, double.MaxValue);
			txtNode.setTextFill(Color.DARKORANGE);
			txtNode.setPadding(new Insets(0,0,0,0));

			label.setGraphic(txtNode);
			label.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			label.setMaxSize(double.MaxValue, double.MaxValue);
			label.setTooltip(new Tooltip(tooltip));
			label.setUserData(userData);
			label.setVisible(false);
			label.setManaged(false);
			label.setPadding(new Insets(0,0,0,0));
			this.warningIconNodes[userData] = label;
			return label;
		}

		private GridPane createGridPane()
		{
			GridPane gridPane = new GridPane();
			gridPane.setMaxWidth(double.MaxValue);
			gridPane.setHgap(10);
			gridPane.setVgap(10);
			gridPane.setPadding(new Insets(20, 10, 20, 10)); // oben, links, unten, rechts
			return gridPane;
		}

		private TitledPane createTitledPane(string title)
		{
			TitledPane parametersTitledPane = new TitledPane();
			parametersTitledPane.setMaxWidth(double.MaxValue);
			parametersTitledPane.setCollapsible(false);
			parametersTitledPane.setAnimated(false);
			parametersTitledPane.setText(title);
			parametersTitledPane.setPadding(new Insets(5, 0, 5, 0)); // oben, links, unten, rechts
			return parametersTitledPane;
		}

		private void init()
		{
			VBox content = new VBox();

			Node uncertainties = this.createUncertaintiesPane();
			if (uncertainties != null)
			{
				content.getChildren().add(uncertainties);
			}

			this.reset();

			ScrollPane scroller = new ScrollPane(content);
			scroller.setPadding(new Insets(20, 50, 20, 50)); // oben, links, unten, rechts
			scroller.setFitToHeight(true);
			scroller.setFitToWidth(true);

			Region spacer = new Region();
			spacer.setPrefHeight(0);
			VBox.setVgrow(spacer, Priority.ALWAYS);
			this.selectionInfoLabel.setPadding(new Insets(1,5,2,10));
			this.selectionInfoLabel.setFont(new Font(10.5));
			this.propertiesNode = new VBox(scroller, spacer, this.selectionInfoLabel);
	//		this.propertiesNode = scroller;

			FadeTransition fadeIn = new FadeTransition(Duration.millis(150));
			FadeTransition fadeOut = new FadeTransition(Duration.millis(150));

			fadeIn.setFromValue(0.0);
			fadeIn.setToValue(1.0);
			fadeIn.setCycleCount(1);
			fadeIn.setAutoReverse(false);

			fadeOut.setFromValue(1.0);
			fadeOut.setToValue(0.0);
			fadeOut.setCycleCount(1);
			fadeOut.setAutoReverse(false);

			this.sequentialTransition.getChildren().addAll(fadeIn, fadeOut);
			this.sequentialTransition.setAutoReverse(false);
			this.sequentialTransition.onFinishedProperty().addListener(new SequentialTransitionFinishedListener(this));
		}

		private bool ProgressIndicatorsVisible
		{
			set
			{
				if (this.databaseTransactionProgressIndicators != null)
				{
					foreach (ProgressIndicator progressIndicator in this.databaseTransactionProgressIndicators.Values)
					{
						progressIndicator.setVisible(value);
					}
				}
			}
		}

		private bool WarningIconsVisible
		{
			set
			{
				if (this.warningIconNodes != null)
				{
					foreach (Node warningIconNode in this.warningIconNodes.Values)
					{
						warningIconNode.setVisible(value);
						warningIconNode.setManaged(value);
					}
				}
			}
		}

		private void save(VerticalDeflectionGroupUncertaintyType uncertaintyType)
		{
			try
			{
				double? value = null;
				switch (uncertaintyType.innerEnumValue)
				{
				case VerticalDeflectionGroupUncertaintyType.InnerEnum.DEFLECTION_X:
					value = this.uncertaintyDeflectionXField.Number;
					break;
				case VerticalDeflectionGroupUncertaintyType.InnerEnum.DEFLECTION_Y:
					value = this.uncertaintyDeflectionYField.Number;
					break;
				default:
					Console.Error.WriteLine(this.GetType().Name + " : Error, unsupported uncertainty type " + uncertaintyType);
					break;
				}

				if (value != null && value.Value > 0 && this.selectedVerticalDeflectionItemValues != null && this.selectedVerticalDeflectionItemValues.Length > 0)
				{
					this.ProgressIndicatorsVisible = false;
					if (this.warningIconNodes.ContainsKey(uncertaintyType))
					{
						Node warningIconNodes = this.warningIconNodes[uncertaintyType];
						warningIconNodes.setVisible(false);
						warningIconNodes.setManaged(false);
					}
					if (this.databaseTransactionProgressIndicators.ContainsKey(uncertaintyType))
					{
						ProgressIndicator node = this.databaseTransactionProgressIndicators[uncertaintyType];
						node.setVisible(true);
						this.sequentialTransition.stop();
						this.sequentialTransition.setNode(node);
						this.sequentialTransition.playFromStart();
					}
					SQLManager.Instance.saveUncertainty(uncertaintyType, value.Value, this.selectedVerticalDeflectionItemValues);
				}

			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);

				this.ProgressIndicatorsVisible = false;
				this.sequentialTransition.stop();

				Platform.runLater(() =>
				{
				OptionDialog.showThrowableDialog(i18n.getString("UIVerticalDeflectionPropertiesPane.message.error.save.uncertainty.exception.title", "Unexpected SQL-Error"), i18n.getString("UIVerticalDeflectionPropertiesPane.message.error.save.uncertainty.exception.header", "Error, could not save group uncertainties to database."), i18n.getString("UIVerticalDeflectionPropertiesPane.message.error.save.uncertainty.exception.message", "An exception has occurred during database transaction."), e);
				});
			}
		}
	}

}