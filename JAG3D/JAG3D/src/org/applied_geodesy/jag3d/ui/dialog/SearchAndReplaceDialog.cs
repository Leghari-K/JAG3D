using System;

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

namespace org.applied_geodesy.jag3d.ui.dialog
{

	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using TreeItemValue = org.applied_geodesy.jag3d.ui.tree.TreeItemValue;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
	using org.applied_geodesy.util;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;

	using Platform = javafx.application.Platform;
	using ActionEvent = javafx.@event.ActionEvent;
	using EventHandler = javafx.@event.EventHandler;
	using Insets = javafx.geometry.Insets;
	using Pos = javafx.geometry.Pos;
	using Node = javafx.scene.Node;
	using Button = javafx.scene.control.Button;
	using ButtonType = javafx.scene.control.ButtonType;
	using CheckBox = javafx.scene.control.CheckBox;
	using ComboBox = javafx.scene.control.ComboBox;
	using Control = javafx.scene.control.Control;
	using Dialog = javafx.scene.control.Dialog;
	using Label = javafx.scene.control.Label;
	using RadioButton = javafx.scene.control.RadioButton;
	using ToggleGroup = javafx.scene.control.ToggleGroup;
	using Tooltip = javafx.scene.control.Tooltip;
	using GridPane = javafx.scene.layout.GridPane;
	using HBox = javafx.scene.layout.HBox;
	using Priority = javafx.scene.layout.Priority;
	using Modality = javafx.stage.Modality;
	using Stage = javafx.stage.Stage;
	using Window = javafx.stage.Window;
	using Callback = javafx.util.Callback;
	using StringConverter = javafx.util.StringConverter;

	public class SearchAndReplaceDialog
	{
		private I18N i18n = I18N.Instance;
		private static SearchAndReplaceDialog searchAndReplaceDialog = new SearchAndReplaceDialog();
		private Dialog<Void> dialog = null;
		private Window window;
		private ComboBox<string> searchComboBox = new ComboBox<string>();
		private ComboBox<string> replaceComboBox = new ComboBox<string>();
		private Label statusLabel = new Label();
		private RadioButton normalModeRadioButton;
		private RadioButton regularExpressionRadioButton;
		private CheckBox keepDialogOpenCheckBox;
		private ComboBox<ScopeType> scopeTypeComboBox;
		private TreeItemValue itemValue;
		private TreeItemValue[] selectedTreeItemValues;
		private SearchAndReplaceDialog()
		{
		}

		public static Window Owner
		{
			set
			{
				searchAndReplaceDialog.window = value;
			}
		}

		public static Optional<Void> showAndWait(TreeItemValue itemValue, params TreeItemValue[] selectedTreeItemValues)
		{
			searchAndReplaceDialog.itemValue = itemValue;
			searchAndReplaceDialog.selectedTreeItemValues = selectedTreeItemValues;
			searchAndReplaceDialog.init();
			searchAndReplaceDialog.statusLabel.setText(null);

			// @see https://bugs.openjdk.java.net/browse/JDK-8087458
			Platform.runLater(() =>
			{
			try
			{
				searchAndReplaceDialog.dialog.getDialogPane().requestLayout();
				Stage stage = (Stage) searchAndReplaceDialog.dialog.getDialogPane().getScene().getWindow();
				stage.sizeToScene();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			});
			return searchAndReplaceDialog.dialog.showAndWait();
		}

		private void init()
		{
			if (this.dialog != null)
			{
				return;
			}

			this.dialog = new Dialog<Void>();
			this.dialog.setTitle(i18n.getString("SearchAndReplaceDialog.title", "Search and replace"));
			this.dialog.setHeaderText(i18n.getString("SearchAndReplaceDialog.header", "Search and replace point names"));
			this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.OK, ButtonType.CLOSE);
			this.dialog.initModality(Modality.APPLICATION_MODAL);
			//		this.dialog.initStyle(StageStyle.UTILITY);
			this.dialog.initOwner(window);
			this.dialog.getDialogPane().setContent(this.createPane());
			this.dialog.setResizable(true);
			Button applyButton = (Button) this.dialog.getDialogPane().lookupButton(ButtonType.OK);
			applyButton.addEventFilter(ActionEvent.ACTION, new EventHandlerAnonymousInnerClass(this));
			this.dialog.setResultConverter(new CallbackAnonymousInnerClass(this));
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<ActionEvent>
		{
			private readonly SearchAndReplaceDialog outerInstance;

			public EventHandlerAnonymousInnerClass(SearchAndReplaceDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(ActionEvent @event)
			{
				outerInstance.save();
				if (outerInstance.keepDialogOpenCheckBox.isSelected())
				{
					@event.consume();
				}
			}
		}

		private class CallbackAnonymousInnerClass : Callback<ButtonType, Void>
		{
			private readonly SearchAndReplaceDialog outerInstance;

			public CallbackAnonymousInnerClass(SearchAndReplaceDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override Void call(ButtonType buttonType)
			{
		//				if (buttonType == ButtonType.OK) {
		//					save();
		//				}
				return null;
			}
		}

		private Node createPane()
		{

			this.searchComboBox = this.createComboBox(i18n.getString("SearchAndReplaceDialog.search.prompt", "Old point name"), i18n.getString("SearchAndReplaceDialog.search.tooltip", "Enter old point name"));

			this.replaceComboBox = this.createComboBox(i18n.getString("SearchAndReplaceDialog.replace.prompt", "New point name"), i18n.getString("SearchAndReplaceDialog.replace.tooltip", "Enter new point name"));

			this.normalModeRadioButton = this.createRadioButton(i18n.getString("SearchAndReplaceDialog.mode.normal.label", "Normal"), i18n.getString("SearchAndReplaceDialog.mode.normal.tooltip", "If selected, normal search and replace mode will be applied"));

			this.regularExpressionRadioButton = this.createRadioButton(i18n.getString("SearchAndReplaceDialog.mode.regex.label", "Regular expression"), i18n.getString("SearchAndReplaceDialog.mode.regex.tooltip", "If selected, regular expression mode will be applied"));

			this.keepDialogOpenCheckBox = this.createCheckBox(i18n.getString("SearchAndReplaceDialog.keep_open.label", "Keep dialog open after modification"), i18n.getString("SearchAndReplaceDialog.keep_open.tooltip", "If selected, dialog will be kept open after data modification"));
			this.keepDialogOpenCheckBox.setSelected(false);

			this.scopeTypeComboBox = this.createScopeTypeComboBox(ScopeType.SELECTION, i18n.getString("SearchAndReplaceDialog.scope.tooltip", "Select scope of application"));

			Button switchInputButton = new Button(i18n.getString("SearchAndReplaceDialog.switch.label", "\u21C5"));
			switchInputButton.setTooltip(new Tooltip(i18n.getString("SearchAndReplaceDialog.switch.tooltip", "Switch values")));
			switchInputButton.setStyle("-fx-font-size:2em");
			switchInputButton.setPadding(new Insets(2, 2, 2, 2));
			switchInputButton.setOnAction(new EventHandlerAnonymousInnerClass2(this));

			Label scopeLabel = new Label(i18n.getString("SearchAndReplaceDialog.scope.label", "Scope:"));
			Label searchLabel = new Label(i18n.getString("SearchAndReplaceDialog.search.label", "Find what:"));
			Label replaceLabel = new Label(i18n.getString("SearchAndReplaceDialog.replace.label", "Replace with:"));
			Label modeLabel = new Label(i18n.getString("SearchAndReplaceDialog.mode.label", "Mode:"));

			scopeLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			searchLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			replaceLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			switchInputButton.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			scopeLabel.setLabelFor(this.scopeTypeComboBox);
			searchLabel.setLabelFor(this.searchComboBox);
			replaceLabel.setLabelFor(this.replaceComboBox);

			ToggleGroup group = new ToggleGroup();
			group.getToggles().addAll(this.normalModeRadioButton, this.regularExpressionRadioButton);
			this.normalModeRadioButton.setSelected(true);

			HBox hbox = new HBox(10, this.normalModeRadioButton, this.regularExpressionRadioButton);
			hbox.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			GridPane gridPane = new GridPane();
			gridPane.setMaxWidth(double.MaxValue);
			gridPane.setHgap(10);
			gridPane.setVgap(10);
			gridPane.setAlignment(Pos.CENTER);
			gridPane.setPadding(new Insets(5,15,5,15));
			//gridPane.setGridLinesVisible(true);

			GridPane.setHgrow(scopeLabel, Priority.NEVER);
			GridPane.setHgrow(searchLabel, Priority.NEVER);
			GridPane.setHgrow(replaceLabel, Priority.NEVER);
			GridPane.setHgrow(modeLabel, Priority.NEVER);
			GridPane.setHgrow(switchInputButton, Priority.NEVER);

			GridPane.setHgrow(this.searchComboBox, Priority.ALWAYS);
			GridPane.setHgrow(this.replaceComboBox, Priority.ALWAYS);
			GridPane.setHgrow(hbox, Priority.ALWAYS);
			GridPane.setHgrow(this.scopeTypeComboBox, Priority.ALWAYS);
			GridPane.setHgrow(this.keepDialogOpenCheckBox, Priority.ALWAYS);
			GridPane.setHgrow(this.statusLabel, Priority.ALWAYS);

			int row = 1;
			gridPane.add(scopeLabel, 0, row);
			gridPane.add(this.scopeTypeComboBox, 1, row++, 2, 1);

			gridPane.add(switchInputButton, 3, row, 1, 2);
			gridPane.add(searchLabel, 0, row);
			gridPane.add(this.searchComboBox, 1, row++, 2, 1);

			gridPane.add(replaceLabel, 0, row);
			gridPane.add(this.replaceComboBox, 1, row++, 2, 1);

			gridPane.add(modeLabel, 0, row);
			gridPane.add(hbox, 1, row++, 2, 1);

			gridPane.add(this.keepDialogOpenCheckBox, 1, row++, 2, 1);
			gridPane.add(this.statusLabel, 1, row++, 2, 1);

			Platform.runLater(() =>
			{
			searchComboBox.requestFocus();
			});

			return gridPane;
		}

		private class EventHandlerAnonymousInnerClass2 : EventHandler<ActionEvent>
		{
			private readonly SearchAndReplaceDialog outerInstance;

			public EventHandlerAnonymousInnerClass2(SearchAndReplaceDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(ActionEvent e)
			{
				string search = outerInstance.searchComboBox.getValue();
				string replace = outerInstance.replaceComboBox.getValue();

				outerInstance.searchComboBox.setValue(replace);
				outerInstance.replaceComboBox.setValue(search);
			}
		}

		private ComboBox<string> createComboBox(string promtText, string tooltip)
		{
			ComboBox<string> comboBox = new ComboBox<string>(new ObservableLimitedList<string>(150));
			comboBox.setEditable(true);
			comboBox.setPromptText(promtText);
			comboBox.setTooltip(new Tooltip(tooltip));
			comboBox.setMinSize(250, Control.USE_PREF_SIZE);
			comboBox.setMaxWidth(double.MaxValue);
			return comboBox;
		}

		private RadioButton createRadioButton(string text, string tooltip)
		{
			Label label = new Label(text);
			label.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			label.setPadding(new Insets(0,0,0,3));
			RadioButton radioButton = new RadioButton();
			radioButton.setGraphic(label);
			radioButton.setTooltip(new Tooltip(tooltip));
			radioButton.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			radioButton.setMaxWidth(double.MaxValue);
			return radioButton;
		}

		private CheckBox createCheckBox(string text, string tooltip)
		{
			Label label = new Label(text);
			label.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			label.setPadding(new Insets(0,0,0,3));
			CheckBox checkBox = new CheckBox();
			checkBox.setGraphic(label);
			checkBox.setTooltip(new Tooltip(tooltip));
			checkBox.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			checkBox.setMaxWidth(double.MaxValue);
			return checkBox;
		}

		private ComboBox<ScopeType> createScopeTypeComboBox(ScopeType item, string tooltip)
		{
			ComboBox<ScopeType> typeComboBox = new ComboBox<ScopeType>();
			typeComboBox.getItems().setAll((ScopeType[])Enum.GetValues(typeof(ScopeType)));
			typeComboBox.getSelectionModel().select(item);
			typeComboBox.setConverter(new StringConverterAnonymousInnerClass(this));
			typeComboBox.setTooltip(new Tooltip(tooltip));
			typeComboBox.setMinWidth(150);
			typeComboBox.setMaxWidth(double.MaxValue);
			return typeComboBox;
		}

		private class StringConverterAnonymousInnerClass : StringConverter<ScopeType>
		{
			private readonly SearchAndReplaceDialog outerInstance;

			public StringConverterAnonymousInnerClass(SearchAndReplaceDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public override string toString(ScopeType type)
			{
				if (type == null)
				{
					return null;
				}
				switch (type)
				{
				case org.applied_geodesy.jag3d.ui.dialog.ScopeType.SELECTION:
					return outerInstance.i18n.getString("SearchAndReplaceDialog.scope.selection.label", "Selected items");
				case org.applied_geodesy.jag3d.ui.dialog.ScopeType.PROJECT:
					return outerInstance.i18n.getString("SearchAndReplaceDialog.scope.project.label", "Whole project");
				case org.applied_geodesy.jag3d.ui.dialog.ScopeType.REFERENCE_EPOCH:
					return outerInstance.i18n.getString("SearchAndReplaceDialog.scope.reference_epoch.label", "Observations of reference epoch");
				case org.applied_geodesy.jag3d.ui.dialog.ScopeType.CONTROL_EPOCH:
					return outerInstance.i18n.getString("SearchAndReplaceDialog.scope.control_epoch.label", "Observations of control epoch");
				}
				return null;
			}

			public override ScopeType fromString(string @string)
			{
				return (ScopeType)Enum.Parse(typeof(ScopeType), @string);
			}
		}

		private void save()
		{
			try
			{
				this.statusLabel.setText(null);
				this.searchComboBox.commitValue();
				this.replaceComboBox.commitValue();

				string search = this.searchComboBox.getValue();
				string replace = this.replaceComboBox.getValue();

				search = string.ReferenceEquals(search, null) ? "" : search;
				replace = string.ReferenceEquals(replace, null) ? "" : replace;

				// add new items to combo boxes
				if (search.Length > 0 && !this.searchComboBox.getItems().contains(search))
				{
					this.searchComboBox.getItems().add(search);
				}
				this.searchComboBox.setValue(search);

				if (replace.Length > 0 && !this.replaceComboBox.getItems().contains(replace))
				{
					this.replaceComboBox.getItems().add(replace);
				}
				this.replaceComboBox.setValue(replace);

				ScopeType scopeType = this.scopeTypeComboBox.getValue();
				bool regExp = this.regularExpressionRadioButton.isSelected();

				// masking values
				if (!regExp)
				{
					search = "^\\Q" + search + "\\E";
				}

				int rows = SQLManager.Instance.searchAndReplacePointNames(search, replace, scopeType, this.itemValue, this.selectedTreeItemValues);
				if (this.keepDialogOpenCheckBox.isSelected())
				{
					this.statusLabel.setText(String.format(Locale.ENGLISH, i18n.getString("SearchAndReplaceDialog.result.label", "%d row(s) edited\u2026"), rows));
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				Platform.runLater(() =>
				{
				OptionDialog.showThrowableDialog(i18n.getString("SearchAndReplaceDialog.message.error.save.exception.title", "Unexpected SQL-Error"), i18n.getString("SearchAndReplaceDialog.message.error.save.exception.header", "Error, could not save renamed points to database."), i18n.getString("SearchAndReplaceDialog.message.error.save.exception.message", "An exception has occurred during database transaction."), e);
				});
			}
		}
	}

}