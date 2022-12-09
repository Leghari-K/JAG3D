using System;
using System.Collections.Generic;
using System.Text;

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

	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;
	using PointType = org.applied_geodesy.adjustment.network.PointType;
	using VerticalDeflectionType = org.applied_geodesy.adjustment.network.VerticalDeflectionType;
	using CSVObservationFileReader = org.applied_geodesy.jag3d.ui.io.CSVObservationFileReader;
	using CSVPointFileReader = org.applied_geodesy.jag3d.ui.io.CSVPointFileReader;
	using CSVVerticalDeflectionFileReader = org.applied_geodesy.jag3d.ui.io.CSVVerticalDeflectionFileReader;
	using ColumnDefinedObservationFileReader = org.applied_geodesy.jag3d.ui.io.ColumnDefinedObservationFileReader;
	using ColumnDefinedPointFileReader = org.applied_geodesy.jag3d.ui.io.ColumnDefinedPointFileReader;
	using ColumnDefinedVerticalDeflectionFileReader = org.applied_geodesy.jag3d.ui.io.ColumnDefinedVerticalDeflectionFileReader;
	using TreeItemValue = org.applied_geodesy.jag3d.ui.tree.TreeItemValue;
	using LimitedTextField = org.applied_geodesy.ui.textfield.LimitedTextField;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;
	using PreviewFileReader = org.applied_geodesy.util.io.PreviewFileReader;
	using org.applied_geodesy.util.io;
	using CSVColumnType = org.applied_geodesy.util.io.csv.CSVColumnType;
	using CSVOptionType = org.applied_geodesy.util.io.csv.CSVOptionType;
	using CSVParser = org.applied_geodesy.util.io.csv.CSVParser;
	using ColumnRange = org.applied_geodesy.util.io.csv.ColumnRange;

	using Platform = javafx.application.Platform;
	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
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
	using ScrollPane = javafx.scene.control.ScrollPane;
	using TextField = javafx.scene.control.TextField;
	using TextFormatter = javafx.scene.control.TextFormatter;
	using TitledPane = javafx.scene.control.TitledPane;
	using Change = javafx.scene.control.TextFormatter.Change;
	using ToggleGroup = javafx.scene.control.ToggleGroup;
	using Tooltip = javafx.scene.control.Tooltip;
	using TreeItem = javafx.scene.control.TreeItem;
	using MouseEvent = javafx.scene.input.MouseEvent;
	using Background = javafx.scene.layout.Background;
	using BackgroundFill = javafx.scene.layout.BackgroundFill;
	using Border = javafx.scene.layout.Border;
	using BorderStroke = javafx.scene.layout.BorderStroke;
	using BorderStrokeStyle = javafx.scene.layout.BorderStrokeStyle;
	using BorderWidths = javafx.scene.layout.BorderWidths;
	using CornerRadii = javafx.scene.layout.CornerRadii;
	using GridPane = javafx.scene.layout.GridPane;
	using HBox = javafx.scene.layout.HBox;
	using Priority = javafx.scene.layout.Priority;
	using VBox = javafx.scene.layout.VBox;
	using Color = javafx.scene.paint.Color;
	using Font = javafx.scene.text.Font;
	using FontWeight = javafx.scene.text.FontWeight;
	using Text = javafx.scene.text.Text;
	using TextAlignment = javafx.scene.text.TextAlignment;
	using Modality = javafx.stage.Modality;
	using Stage = javafx.stage.Stage;
	using Window = javafx.stage.Window;
	using Callback = javafx.util.Callback;
	using StringConverter = javafx.util.StringConverter;

	public class ColumnImportDialog
	{

		private class CSVUserDefinedOptionChangeListener : ChangeListener<string>
		{
			private readonly ColumnImportDialog outerInstance;

			internal readonly TextField textField;
			internal readonly CSVOptionType type;
			public CSVUserDefinedOptionChangeListener(ColumnImportDialog outerInstance, TextField textField)
			{
				this.outerInstance = outerInstance;
				this.textField = textField;
				if (this.textField.getUserData() == null || !(this.textField.getUserData() is CSVOptionType))
				{
					throw new System.ArgumentException(this.GetType().Name + " Error, no CSV option type defined!");
				}
				this.type = (CSVOptionType)this.textField.getUserData();
			}

			public override void changed<T1>(ObservableValue<T1> observable, string oldValue, string newValue) where T1 : string
			{
				if (textField.getText() != null && !textField.getText().isEmpty())
				{
					char c = textField.getText().charAt(0);
					switch (this.type)
					{
					case CSVOptionType.QUOTE:
						outerInstance.quotechar = c;
						break;
					case CSVOptionType.ESCAPE:
						outerInstance.escape = c;
						break;
					case CSVOptionType.SEPARATOR:
						outerInstance.separator = c;
						break;
					}
					outerInstance.changeMode();
				}
			}
		}

		private class CSVOptionChangeListener : ChangeListener<bool>
		{
			private readonly ColumnImportDialog outerInstance;

			internal readonly ToggleGroup group;
			internal readonly CSVOptionType type;
			public CSVOptionChangeListener(ColumnImportDialog outerInstance, ToggleGroup group)
			{
				this.outerInstance = outerInstance;
				this.group = group;
				if (this.group.getUserData() == null || !(this.group.getUserData() is CSVOptionType))
				{
					throw new System.ArgumentException(this.GetType().Name + " Error, no CSV option type defined!");
				}
				this.type = (CSVOptionType)this.group.getUserData();
			}

			public override void changed<T1>(ObservableValue<T1> observable, bool? oldValue, bool? newValue) where T1 : bool
			{
				if (newValue.Value)
				{
					if (this.group.getSelectedToggle().getUserData() != null)
					{
						char c = CSVParser.NULL_CHARACTER;

						if (this.group.getSelectedToggle().getUserData() is Character)
						{
							c = ((char?)this.group.getSelectedToggle().getUserData()).Value;
						}

						else if (this.group.getSelectedToggle().getUserData() is TextField)
						{
							TextField textField = (TextField)this.group.getSelectedToggle().getUserData();
							if (textField.getText() != null && !textField.getText().isEmpty())
							{
								c = textField.getText().charAt(0);
							}
						}

						switch (this.type)
						{
						case CSVOptionType.QUOTE:
							outerInstance.quotechar = c;
							break;
						case CSVOptionType.ESCAPE:
							outerInstance.escape = c;
							break;
						case CSVOptionType.SEPARATOR:
							outerInstance.separator = c;
							break;
						}
						outerInstance.changeMode();
					}
				}
			}
		}

		private class LocaleOptionChangeListener : ChangeListener<bool>
		{
			private readonly ColumnImportDialog outerInstance;

			internal readonly ToggleGroup group;

			public LocaleOptionChangeListener(ColumnImportDialog outerInstance, ToggleGroup group)
			{
				this.outerInstance = outerInstance;
				this.group = group;
			}

			public override void changed<T1>(ObservableValue<T1> arg0, bool? arg1, bool? arg2) where T1 : bool
			{
				if (this.group.getSelectedToggle().getUserData() != null && this.group.getSelectedToggle().getUserData() is Locale)
				{
					outerInstance.fileLocale = (Locale)this.group.getSelectedToggle().getUserData();
				}
			}
		}

		private class ColumnIndex
		{
			private readonly ColumnImportDialog outerInstance;

			internal readonly int column;
			public ColumnIndex(ColumnImportDialog outerInstance, int column)
			{
				this.outerInstance = outerInstance;
				this.column = column;
			}

			public virtual int Column
			{
				get
				{
					return this.column;
				}
			}
		}

		private class ColumnSelectionEventHandler : EventHandler<MouseEvent>
		{
			private readonly ColumnImportDialog outerInstance;

			public ColumnSelectionEventHandler(ColumnImportDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal ColumnIndex columnIndex = null;
			public override void handle(MouseEvent @event)
			{
				if (@event.getSource() == outerInstance.editor && @event.getPickResult() != null && @event.getPickResult().getIntersectedNode() != null && @event.getPickResult().getIntersectedNode() is Text && @event.getPickResult().getIntersectedNode().getParent() != null && @event.getPickResult().getIntersectedNode().getParent() is VBox && @event.getPickResult().getIntersectedNode().getParent().getUserData() != null && @event.getPickResult().getIntersectedNode().getParent().getUserData() is ColumnIndex)
				{
					this.columnIndex = (ColumnIndex)@event.getPickResult().getIntersectedNode().getParent().getUserData();
				}

				if (this.columnIndex != null)
				{
					bool highlight = false;
					if (@event.getEventType() == MouseEvent.MOUSE_PRESSED)
					{
						if (outerInstance.importCSVFileCheckBox.isSelected() && outerInstance.maxCharactersPerColumn != null)
						{
							int columnLength = 0;
							int column = columnIndex.Column;
							for (int i = 0; i < outerInstance.maxCharactersPerColumn.Length; i++)
							{
								int nextColumLength = columnLength + outerInstance.maxCharactersPerColumn[i];
								if (column >= columnLength && column < nextColumLength)
								{
									outerInstance.startColumn = columnLength;
								}
								if (column >= columnLength && column < nextColumLength)
								{
									outerInstance.endColumn = nextColumLength - 1;
								}
								columnLength = nextColumLength;
							}
						}
						else
						{
							outerInstance.startColumn = this.columnIndex.Column;
							outerInstance.endColumn = outerInstance.startColumn;
						}

						highlight = true;
					}

					else if (!outerInstance.importCSVFileCheckBox.isSelected() && outerInstance.startColumn >= 0 && outerInstance.endColumn >= 0 && @event.getEventType() == MouseEvent.MOUSE_DRAGGED)
					{
						outerInstance.endColumn = this.columnIndex.Column;
						highlight = true;
					}

					if (highlight)
					{
						outerInstance.selectColumns(Math.Min(outerInstance.startColumn, outerInstance.endColumn), Math.Max(outerInstance.startColumn, outerInstance.endColumn));
					}
				}
			}
		}


		private I18N i18n = I18N.Instance;
		private static ColumnImportDialog columnImportDialog = new ColumnImportDialog();
		private Dialog<SourceFileReader<TreeItem<TreeItemValue>>> dialog = null;
		private Window window;

		private int[] maxCharactersPerColumn = null;
		private IList<string> linesOfFile = new List<string>(20);
		private IList<TextField> textFieldList = new List<TextField>(20);
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: private javafx.scene.control.ComboBox<Enum<?>> importTypes;
		private ComboBox<Enum<object>> importTypes;
		private HBox editor;
		private IList<VBox> columns = new List<VBox>();

		private Node csvOptionPane;
		private Node pointColumnPickerPane;
		private Node deflectionColumnPickerPane;
		private Node terrestrialObservationColumnPickerPane;
		private Node gnssObservationColumnPickerPane;

		private CheckBox importCSVFileCheckBox;
		private int startColumn = -1;
		private int endColumn = -1;
		private const int MAX_LINES = 10;
		private readonly string TABULATOR = "     ";
		private char separator = CSVParser.DEFAULT_SEPARATOR;
		private char quotechar = CSVParser.DEFAULT_QUOTE_CHARACTER;
		private char escape = CSVParser.DEFAULT_ESCAPE_CHARACTER;
		private Locale fileLocale = Locale.ENGLISH;

		private ColumnImportDialog()
		{
		}

		public static Window Owner
		{
			set
			{
				columnImportDialog.window = value;
			}
		}

		public static Optional<SourceFileReader<TreeItem<TreeItemValue>>> showAndWait(File selectedFile)
		{
			if (selectedFile == null)
			{
				return null;
			}

			columnImportDialog.init();

			try
			{
				columnImportDialog.readPreview(selectedFile);
			}
			catch (Exception e) when (e is IOException || e is SQLException)
			{
				e.printStackTrace();
			}

			// @see https://bugs.openjdk.java.net/browse/JDK-8087458
			Platform.runLater(() =>
			{
			try
			{
				columnImportDialog.dialog.getDialogPane().requestLayout();
				Stage stage = (Stage) columnImportDialog.dialog.getDialogPane().getScene().getWindow();
				stage.sizeToScene();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			});
			return columnImportDialog.dialog.showAndWait();
		}

		private void init()
		{
			if (this.dialog != null)
			{
				return;
			}

			this.dialog = new Dialog<SourceFileReader<TreeItem<TreeItemValue>>>();
			this.dialog.setTitle(i18n.getString("ColumnImportDialog.title", "Column based file import"));
			this.dialog.setHeaderText(i18n.getString("ColumnImportDialog.header", "User-defined import of column-based files"));
			this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.OK, ButtonType.CLOSE);
			this.dialog.initModality(Modality.APPLICATION_MODAL);
			this.dialog.initOwner(window);
			this.dialog.setResizable(true);

			this.dialog.getDialogPane().setContent(this.createMainPane());

			this.dialog.setResultConverter(new CallbackAnonymousInnerClass(this));
		}

		private class CallbackAnonymousInnerClass : Callback<ButtonType, SourceFileReader<TreeItem<TreeItemValue>>>
		{
			private readonly ColumnImportDialog outerInstance;

			public CallbackAnonymousInnerClass(ColumnImportDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override SourceFileReader<TreeItem<TreeItemValue>> call(ButtonType buttonType)
			{
				if (buttonType == ButtonType.OK)
				{
					return outerInstance.SourceFileReader;
				}
				return null;
			}
		}

		private Node createMainPane()
		{
			VBox vBox = new VBox();
			vBox.setSpacing(10);
			vBox.setPadding(new Insets(3,3,3,3));

			Node editorPane = this.initEditor();
			this.importCSVFileCheckBox = this.createCSVCheckbox();
			this.pointColumnPickerPane = this.createPointColumnPickerPane();
			this.deflectionColumnPickerPane = this.createVerticalDeflectionColumnPickerPane();
			this.terrestrialObservationColumnPickerPane = this.createTerrestrialObservationColumnPickerPane();
			this.gnssObservationColumnPickerPane = this.createGNSSObservationColumnPickerPane();
			this.importTypes = this.createImportTypeComboBox();
			this.csvOptionPane = this.createCSVOptionPane();
			Node globalImportOptions = createImportOptionPane();

	//		((TitledPane)editorPane).prefWidthProperty().bind(
	//				Bindings.max(((TitledPane)this.deflectionColumnPickerPane).widthProperty(), 
	//						Bindings.max(
	//								((TitledPane)this.gnssObservationColumnPickerPane).widthProperty(), 
	//								Bindings.max(
	//										((TitledPane)this.pointColumnPickerPane).widthProperty(), 
	//										((TitledPane)this.terrestrialObservationColumnPickerPane).widthProperty()
	//										)
	//								)
	//						)
	//				);

			vBox.getChildren().addAll(globalImportOptions, this.csvOptionPane, this.pointColumnPickerPane, this.deflectionColumnPickerPane, this.terrestrialObservationColumnPickerPane, this.gnssObservationColumnPickerPane, editorPane);

			ScrollPane scrollPane = new ScrollPane(vBox);
			scrollPane.setFitToHeight(true);
			scrollPane.setFitToWidth(true);

			Platform.runLater(() =>
			{
			importTypes.requestFocus();
			});

			return scrollPane;
		}

		private Node createImportOptionPane()
		{
			GridPane gridPane = this.createGridPane();

			ToggleGroup localeGroup = new ToggleGroup();
			LocaleOptionChangeListener localeChangeListener = new LocaleOptionChangeListener(this, localeGroup);
			RadioButton englishLocaleRadioButton = this.createRadioButton(i18n.getString("ColumnImportDialog.decimal.separator.point.label", "Point"), i18n.getString("ColumnImportDialog.decimal.separator.point.tooltip", "If selected, decimal separator is set to point"), true, Locale.ENGLISH, localeGroup, localeChangeListener);
			RadioButton germanLocaleRadioButton = this.createRadioButton(i18n.getString("ColumnImportDialog.decimal.separator.comma.label", "Comma"), i18n.getString("ColumnImportDialog.decimal.separator.comma.tooltip", "If selected, decimal separator is set to comma"), false, Locale.GERMAN, localeGroup, localeChangeListener);



			int columnIndex = 0;
			int rowIndex = 0;

			gridPane.add(this.importTypes, columnIndex++, rowIndex);
			gridPane.add(this.importCSVFileCheckBox, columnIndex++, rowIndex);

			columnIndex = 0;
			rowIndex++;

			gridPane.add(new Label(i18n.getString("ColumnImportDialog.decimal.separator.label", "Decimal separator:")), columnIndex++, rowIndex);
			gridPane.add(englishLocaleRadioButton, columnIndex++, rowIndex);
			gridPane.add(germanLocaleRadioButton, columnIndex++, rowIndex);

			TitledPane titledPane = this.createTitledPane(i18n.getString("ColumnImportDialog.import.label", "Import options"), i18n.getString("ColumnImportDialog.import.tooltip", "Specify global import options"), gridPane);

			return titledPane;

		}

		private Node createCSVOptionPane()
		{
			GridPane gridPane = this.createGridPane();

			char separator = CSVParser.DEFAULT_SEPARATOR;
			char quotechar = CSVParser.DEFAULT_QUOTE_CHARACTER;
			char escape = CSVParser.DEFAULT_ESCAPE_CHARACTER;

			ToggleGroup separatorGroup = new ToggleGroup();
			separatorGroup.setUserData(CSVOptionType.SEPARATOR);
			LimitedTextField separatorTextField = new LimitedTextField(1,"|");
			separatorTextField.setPrefColumnCount(1);
			separatorTextField.setTooltip(new Tooltip(i18n.getString("ColumnImportDialog.csv.separator.user.field.tooltip", "User-defined column separator")));
			separatorTextField.setUserData(CSVOptionType.SEPARATOR);
			separatorTextField.setMaxWidth(Control.USE_PREF_SIZE);
			separatorTextField.setDisable(true);
			separatorTextField.textProperty().addListener(new CSVUserDefinedOptionChangeListener(this, separatorTextField));

			CSVOptionChangeListener separatorChangeListener = new CSVOptionChangeListener(this, separatorGroup);
			RadioButton commaSeparatorRadioButton = this.createRadioButton(i18n.getString("ColumnImportDialog.csv.separator.comma.label", "Comma"), i18n.getString("ColumnImportDialog.csv.separator.comma.tooltip", "If selected, column separator is set to comma"), separator == ',', ',', separatorGroup, separatorChangeListener);
			RadioButton semicolonSeparatorRadioButton = this.createRadioButton(i18n.getString("ColumnImportDialog.csv.separator.semicolon.label", "Semicolon"), i18n.getString("ColumnImportDialog.csv.separator.semicolon.tooltip", "If selected, column separator is set to semicolon"), separator == ';', ';', separatorGroup, separatorChangeListener);
			RadioButton blankSeparatorRadioButton = this.createRadioButton(i18n.getString("ColumnImportDialog.csv.separator.blank.label", "Blank"), i18n.getString("ColumnImportDialog.csv.separator.blank.tooltip", "If selected, column separator is set to blank"), separator == ' ', ' ', separatorGroup, separatorChangeListener);
			RadioButton tabulatorSeparatorRadioButton = this.createRadioButton(i18n.getString("ColumnImportDialog.csv.separator.tabulator.label", "Tabulator"), i18n.getString("ColumnImportDialog.csv.separator.tabulator.tooltip", "If selected, column separator is set to tabulator"), separator == '\t', '\t', separatorGroup, separatorChangeListener);
			RadioButton userdefinedSeparatorRadioButton = this.createRadioButton(i18n.getString("ColumnImportDialog.csv.separator.user.label", "User-defined"), i18n.getString("ColumnImportDialog.csv.separator.user.tooltip", "If selected, column separator is user-defined"), separatorTextField.getText().length() > 0 && separator == separatorTextField.getText().charAt(0), separatorTextField, separatorGroup, separatorChangeListener);
			HBox separatorBox = new HBox();
			separatorBox.setSpacing(3);
			separatorBox.setAlignment(Pos.CENTER_LEFT);
			separatorBox.getChildren().addAll(userdefinedSeparatorRadioButton, separatorTextField);
			userdefinedSeparatorRadioButton.selectedProperty().addListener(new ChangeListenerAnonymousInnerClass(this, separatorTextField));

			ToggleGroup quoteGroup = new ToggleGroup();
			quoteGroup.setUserData(CSVOptionType.QUOTE);

			CSVOptionChangeListener quoteChangeListener = new CSVOptionChangeListener(this, quoteGroup);
			RadioButton noneQuoteCharRadioButton = this.createRadioButton(i18n.getString("ColumnImportDialog.csv.quote.none.label", "None"), i18n.getString("ColumnImportDialog.csv.quote.none.tooltip", "If selected, quote character is undefined"), quotechar == CSVParser.NULL_CHARACTER, CSVParser.NULL_CHARACTER, quoteGroup, quoteChangeListener);
			RadioButton singleQuoteCharRadioButton = this.createRadioButton(i18n.getString("ColumnImportDialog.csv.quote.single.label", "Single quote"), i18n.getString("ColumnImportDialog.csv.quote.single.tooltip", "If selected, single quote character will used"), quotechar == '\'', '\'', quoteGroup, quoteChangeListener);
			RadioButton doubleQuoteCharRadioButton = this.createRadioButton(i18n.getString("ColumnImportDialog.csv.quote.double.label", "Double quote"), i18n.getString("ColumnImportDialog.csv.quote.double.tooltip", "If selected, double quote character will used"), quotechar == '"', '"', quoteGroup, quoteChangeListener);

			ToggleGroup escapeGroup = new ToggleGroup();
			escapeGroup.setUserData(CSVOptionType.ESCAPE);
			LimitedTextField escapeTextField = new LimitedTextField(1,escape.ToString());
			escapeTextField.setPrefColumnCount(1);
			escapeTextField.setTooltip(new Tooltip(i18n.getString("ColumnImportDialog.csv.escape.user.field.tooltip", "User-defined escape character")));
			escapeTextField.setMaxWidth(Control.USE_PREF_SIZE);
			escapeTextField.setUserData(CSVOptionType.ESCAPE);
			escapeTextField.textProperty().addListener(new CSVUserDefinedOptionChangeListener(this, escapeTextField));

			CSVOptionChangeListener escapeChangeListener = new CSVOptionChangeListener(this, escapeGroup);
			RadioButton noneEscapeCharRadioButton = this.createRadioButton(i18n.getString("ColumnImportDialog.csv.escape.none.label", "Note"), i18n.getString("ColumnImportDialog.csv.escape.none.tooltip", "If selected, escape character is undefined"), escape == CSVParser.NULL_CHARACTER, CSVParser.NULL_CHARACTER, escapeGroup, escapeChangeListener);
			RadioButton userdefinedEscapeRadioButton = this.createRadioButton(i18n.getString("ColumnImportDialog.csv.escape.user.label", "User-defined"), i18n.getString("ColumnImportDialog.csv.escape.user.tooltip", "If selected, column separator is user-defined"), escapeTextField.getText().length() > 0 && escape == escapeTextField.getText().charAt(0), escapeTextField, escapeGroup, escapeChangeListener);

			HBox escapeBox = new HBox();
			escapeBox.setSpacing(3);
			escapeBox.setAlignment(Pos.CENTER_LEFT);
			escapeBox.getChildren().addAll(userdefinedEscapeRadioButton, escapeTextField);
			userdefinedEscapeRadioButton.selectedProperty().addListener(new ChangeListenerAnonymousInnerClass2(this, escapeTextField));

			int columnIndex = 0;
			int rowIndex = 0;
			gridPane.add(new Label(i18n.getString("ColumnImportDialog.csv.separator.label", "Column separator:")), columnIndex++, rowIndex);
			gridPane.add(commaSeparatorRadioButton, columnIndex++, rowIndex);
			gridPane.add(semicolonSeparatorRadioButton, columnIndex++, rowIndex);
			gridPane.add(blankSeparatorRadioButton, columnIndex++, rowIndex);
			gridPane.add(tabulatorSeparatorRadioButton, columnIndex++, rowIndex);
			gridPane.add(separatorBox, columnIndex++, rowIndex);

			columnIndex = 0;
			rowIndex++;
			gridPane.add(new Label(i18n.getString("ColumnImportDialog.csv.quote.label", "Quote character:")), columnIndex++, rowIndex);
			gridPane.add(noneQuoteCharRadioButton, columnIndex++, rowIndex);
			gridPane.add(singleQuoteCharRadioButton, columnIndex++, rowIndex);
			gridPane.add(doubleQuoteCharRadioButton, columnIndex++, rowIndex);

			columnIndex = 0;
			rowIndex++;
			gridPane.add(new Label(i18n.getString("ColumnImportDialog.csv.escape.label", "Escape character:")), columnIndex++, rowIndex);
			gridPane.add(noneEscapeCharRadioButton, columnIndex++, rowIndex);
			gridPane.add(escapeBox, columnIndex++, rowIndex);

			TitledPane titledPane = this.createTitledPane(i18n.getString("ColumnImportDialog.csv.label", "CSV options"), i18n.getString("ColumnImportDialog.csv.tooltip", "Specify character-separated-values (CSV) import options"), gridPane);

			titledPane.setDisable(!this.importCSVFileCheckBox.isSelected());

			return titledPane;

		}

		private class ChangeListenerAnonymousInnerClass : ChangeListener<bool>
		{
			private readonly ColumnImportDialog outerInstance;

			private LimitedTextField separatorTextField;

			public ChangeListenerAnonymousInnerClass(ColumnImportDialog outerInstance, LimitedTextField separatorTextField)
			{
				this.outerInstance = outerInstance;
				this.separatorTextField = separatorTextField;
			}

			public override void changed<T1>(ObservableValue<T1> observable, bool? oldValue, bool? newValue) where T1 : bool
			{
				separatorTextField.setDisable(!newValue);
			}
		}

		private class ChangeListenerAnonymousInnerClass2 : ChangeListener<bool>
		{
			private readonly ColumnImportDialog outerInstance;

			private LimitedTextField escapeTextField;

			public ChangeListenerAnonymousInnerClass2(ColumnImportDialog outerInstance, LimitedTextField escapeTextField)
			{
				this.outerInstance = outerInstance;
				this.escapeTextField = escapeTextField;
			}

			public override void changed<T1>(ObservableValue<T1> observable, bool? oldValue, bool? newValue) where T1 : bool
			{
				escapeTextField.setDisable(!newValue);
			}
		}

		private Node createVerticalDeflectionColumnPickerPane()
		{
			GridPane gridPane = this.createGridPane();

			int columnIndex = 0;
			int rowIndex = 0;

			string buttonLabel = i18n.getString("ColumnImportDialog.column.deflection.name.label", "Point-id \u25B6");
			string buttonTooltip = i18n.getString("ColumnImportDialog.column.deflection.name.tooltip", "Add selected range for point-id");
			string textFieldTooltip = i18n.getString("ColumnImportDialog.column.deflection.name.text.tooltip", "Range for point-id column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.POINT_ID, buttonLabel, buttonTooltip, textFieldTooltip);

			// a-priori y-component
			buttonLabel = i18n.getString("ColumnImportDialog.column.deflection.y0.label", "\u03B6y0 \u25B6");
			buttonTooltip = i18n.getString("ColumnImportDialog.column.deflection.y0.tooltip", "Add selected range for a-priori deflection of y-component");
			textFieldTooltip = i18n.getString("ColumnImportDialog.column.deflection.y0.text.tooltip", "Range for a-priori deflection of y-component column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.Y, buttonLabel, buttonTooltip, textFieldTooltip);

			// a-priori x-component
			buttonLabel = i18n.getString("ColumnImportDialog.column.deflection.x0.label", "\u03B6x0 \u25B6");
			buttonTooltip = i18n.getString("ColumnImportDialog.column.deflection.x0.tooltip", "Add selected range for a-priori deflection of x-component");
			textFieldTooltip = i18n.getString("ColumnImportDialog.column.deflection.x0.text.tooltip", "Range for a-priori udeflection of x-component column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.X, buttonLabel, buttonTooltip, textFieldTooltip);

			rowIndex++;
			columnIndex = 2;

			// Uncertainty in y
			buttonLabel = i18n.getString("ColumnImportDialog.column.deflection.sigma.y0.label", "\u03C3y0 \u25B6");
			buttonTooltip = i18n.getString("ColumnImportDialog.column.deflection.sigma.y0.tooltip", "Add selected range for a-priori uncertainty of y-component");
			textFieldTooltip = i18n.getString("ColumnImportDialog.column.deflection.sigma.y0.text.tooltip", "Range for a-priori uncertainty of y-component column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.UNCERTAINTY_Y, buttonLabel, buttonTooltip, textFieldTooltip);

			// Uncertainty in x
			buttonLabel = i18n.getString("ColumnImportDialog.column.deflection.sigma.x0.label", "\u03C3x0 \u25B6");
			buttonTooltip = i18n.getString("ColumnImportDialog.column.deflection.sigma.x0.tooltip", "Add selected range for a-priori uncertainty of x-component");
			textFieldTooltip = i18n.getString("ColumnImportDialog.column.deflection.sigma.x0.text.tooltip", "Range for a-priori uncertainty of x-component column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.UNCERTAINTY_X, buttonLabel, buttonTooltip, textFieldTooltip);

			TitledPane titledPane = this.createTitledPane(i18n.getString("ColumnImportDialog.column.deflection.label", "Column defintions for vertical deflection"), i18n.getString("ColumnImportDialog.column.deflection.tooltip", "Specify column range of file for vertical deflection import"), gridPane);


			return titledPane;
		}

		private Node createPointColumnPickerPane()
		{
			GridPane gridPane = this.createGridPane();

			int columnIndex = 0;
			int rowIndex = 0;

			string buttonLabel = i18n.getString("ColumnImportDialog.column.point.name.label", "Point-id \u25B6");
			string buttonTooltip = i18n.getString("ColumnImportDialog.column.point.name.tooltip", "Add selected range for point-id");
			string textFieldTooltip = i18n.getString("ColumnImportDialog.column.point.name.text.tooltip", "Range for point-id column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.POINT_ID, buttonLabel, buttonTooltip, textFieldTooltip);

			// a-priori y-component
			buttonLabel = i18n.getString("ColumnImportDialog.column.point.y0.label", "y0 \u25B6");
			buttonTooltip = i18n.getString("ColumnImportDialog.column.point.y0.tooltip", "Add selected range for a-priori y-component");
			textFieldTooltip = i18n.getString("ColumnImportDialog.column.point.y0.text.tooltip", "Range for a-priori y-component column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.Y, buttonLabel, buttonTooltip, textFieldTooltip);

			// a-priori x-component
			buttonLabel = i18n.getString("ColumnImportDialog.column.point.x0.label", "x0 \u25B6");
			buttonTooltip = i18n.getString("ColumnImportDialog.column.point.x0.tooltip", "Add selected range for a-priori x-component");
			textFieldTooltip = i18n.getString("ColumnImportDialog.column.point.x0.text.tooltip", "Range for a-priori x-component column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.X, buttonLabel, buttonTooltip, textFieldTooltip);

			// a-priori z-component
			buttonLabel = i18n.getString("ColumnImportDialog.column.point.z0.label", "z0 \u25B6");
			buttonTooltip = i18n.getString("ColumnImportDialog.column.point.z0.tooltip", "Add selected range for a-priori z-component");
			textFieldTooltip = i18n.getString("ColumnImportDialog.column.point.z0.text.tooltip", "Range for a-priori z-component column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.Z, buttonLabel, buttonTooltip, textFieldTooltip);

			rowIndex++;
			columnIndex = 0;

			// Point code
			buttonLabel = i18n.getString("ColumnImportDialog.column.point.code.label", "Code \u25B6");
			buttonTooltip = i18n.getString("ColumnImportDialog.column.point.code.tooltip", "Add selected range for code");
			textFieldTooltip = i18n.getString("ColumnImportDialog.column.point.code.text.tooltip", "Range for code column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.POINT_CODE, buttonLabel, buttonTooltip, textFieldTooltip);

			// Uncertainty in y
			buttonLabel = i18n.getString("ColumnImportDialog.column.point.sigma.y0.label", "\u03C3y0 \u25B6");
			buttonTooltip = i18n.getString("ColumnImportDialog.column.point.sigma.y0.tooltip", "Add selected range for a-priori uncertainty of y-component");
			textFieldTooltip = i18n.getString("ColumnImportDialog.column.point.sigma.y0.text.tooltip", "Range for a-priori uncertainty of y-component column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.UNCERTAINTY_Y, buttonLabel, buttonTooltip, textFieldTooltip);

			// Uncertainty in x
			buttonLabel = i18n.getString("ColumnImportDialog.column.point.sigma.x0.label", "\u03C3x0 \u25B6");
			buttonTooltip = i18n.getString("ColumnImportDialog.column.point.sigma.x0.tooltip", "Add selected range for a-priori uncertainty of x-component");
			textFieldTooltip = i18n.getString("ColumnImportDialog.column.point.sigma.x0.text.tooltip", "Range for a-priori uncertainty of x-component column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.UNCERTAINTY_X, buttonLabel, buttonTooltip, textFieldTooltip);

			// Uncertainty in z
			buttonLabel = i18n.getString("ColumnImportDialog.column.point.sigma.z0.label", "\u03C3z0 \u25B6");
			buttonTooltip = i18n.getString("ColumnImportDialog.column.point.sigma.z0.tooltip", "Add selected range for a-priori uncertainty of z-component");
			textFieldTooltip = i18n.getString("ColumnImportDialog.column.point.sigma.z0.text.tooltip", "Range for a-priori uncertainty of z-component column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.UNCERTAINTY_Z, buttonLabel, buttonTooltip, textFieldTooltip);

			TitledPane titledPane = this.createTitledPane(i18n.getString("ColumnImportDialog.column.point.label", "Column defintions for points"), i18n.getString("ColumnImportDialog.column.point.tooltip", "Specify column range of file for points import"), gridPane);


			return titledPane;
		}

		private Node createGNSSObservationColumnPickerPane()
		{
			GridPane gridPane = this.createGridPane();

			int columnIndex = 0;
			int rowIndex = 0;

			string buttonLabel = i18n.getString("ColumnImportDialog.column.gnss.station.label", "Station \u25B6");
			string buttonTooltip = i18n.getString("ColumnImportDialog.column.gnss.station.tooltip", "Add selected range for station");
			string textFieldTooltip = i18n.getString("ColumnImportDialog.column.gnss.station.text.tooltip", "Range for station column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.STATION, buttonLabel, buttonTooltip, textFieldTooltip);

			// a-priori y-com,ponent
			buttonLabel = i18n.getString("ColumnImportDialog.column.gnss.y0.label", "y0 \u25B6");
			buttonTooltip = i18n.getString("ColumnImportDialog.column.gnss.y0.tooltip", "Add selected range for a-priori y-component");
			textFieldTooltip = i18n.getString("ColumnImportDialog.column.gnss.y0.text.tooltip", "Range for a-priori y-component column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.Y, buttonLabel, buttonTooltip, textFieldTooltip);

			// a-priori x-component
			buttonLabel = i18n.getString("ColumnImportDialog.column.gnss.x0.label", "x0 \u25B6");
			buttonTooltip = i18n.getString("ColumnImportDialog.column.gnss.x0.tooltip", "Add selected range for a-priori x-component");
			textFieldTooltip = i18n.getString("ColumnImportDialog.column.gnss.x0.text.tooltip", "Range for a-priori x-component column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.X, buttonLabel, buttonTooltip, textFieldTooltip);

			// a-priori z-component
			buttonLabel = i18n.getString("ColumnImportDialog.column.gnss.z0.label", "z0 \u25B6");
			buttonTooltip = i18n.getString("ColumnImportDialog.column.gnss.z0.tooltip", "Add selected range for a-priori z-component");
			textFieldTooltip = i18n.getString("ColumnImportDialog.column.gnss.z0.text.tooltip", "Range for a-priori z-component column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.Z, buttonLabel, buttonTooltip, textFieldTooltip);

			rowIndex++;
			columnIndex = 0;

			// Target point
			buttonLabel = i18n.getString("ColumnImportDialog.column.gnss.target.label", "Target \u25B6");
			buttonTooltip = i18n.getString("ColumnImportDialog.column.gnss.target.tooltip", "Add selected range for target point");
			textFieldTooltip = i18n.getString("ColumnImportDialog.column.gnss.target.text.tooltip", "Range for target point column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.TARGET, buttonLabel, buttonTooltip, textFieldTooltip);

			// Uncertainty in y
			buttonLabel = i18n.getString("ColumnImportDialog.column.gnss.sigma.y0.label", "\u03C3y0 \u25B6");
			buttonTooltip = i18n.getString("ColumnImportDialog.column.gnss.sigma.y0.tooltip", "Add selected range for a-priori uncertainty of y-component");
			textFieldTooltip = i18n.getString("ColumnImportDialog.column.gnss.sigma.y0.text.tooltip", "Range for a-priori uncertainty of y-component column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.UNCERTAINTY_Y, buttonLabel, buttonTooltip, textFieldTooltip);

			// Uncertainty in x
			buttonLabel = i18n.getString("ColumnImportDialog.column.gnss.sigma.x0.label", "\u03C3x0 \u25B6");
			buttonTooltip = i18n.getString("ColumnImportDialog.column.gnss.sigma.x0.tooltip", "Add selected range for a-priori uncertainty of x-component");
			textFieldTooltip = i18n.getString("ColumnImportDialog.column.gnss.sigma.x0.text.tooltip", "Range for a-priori uncertainty of x-component column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.UNCERTAINTY_X, buttonLabel, buttonTooltip, textFieldTooltip);

			// Uncertainty in z
			buttonLabel = i18n.getString("ColumnImportDialog.column.gnss.sigma.z0.label", "\u03C3z0 \u25B6");
			buttonTooltip = i18n.getString("ColumnImportDialog.column.gnss.sigma.z0.tooltip", "Add selected range for a-priori uncertainty of z-component");
			textFieldTooltip = i18n.getString("ColumnImportDialog.column.gnss.sigma.z0.text.tooltip", "Range for a-priori uncertainty of z-component column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.UNCERTAINTY_Z, buttonLabel, buttonTooltip, textFieldTooltip);


			TitledPane titledPane = this.createTitledPane(i18n.getString("ColumnImportDialog.column.gnss.label", "Column defintions for GNSS observations"), i18n.getString("ColumnImportDialog.column.gnss.tooltip", "Specify column range of file for GNSS observations import"), gridPane);


			return titledPane;
		}

		private Node createTerrestrialObservationColumnPickerPane()
		{
			GridPane gridPane = this.createGridPane();

			int columnIndex = 0;
			int rowIndex = 0;

			string buttonLabel = i18n.getString("ColumnImportDialog.column.terrestrial.station.label", "Station \u25B6");
			string buttonTooltip = i18n.getString("ColumnImportDialog.column.terrestrial.station.tooltip", "Add selected range for station");
			string textFieldTooltip = i18n.getString("ColumnImportDialog.column.terrestrial.station.text.tooltip", "Range for station column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.STATION, buttonLabel, buttonTooltip, textFieldTooltip);

			// Instrumenten height
			buttonLabel = i18n.getString("ColumnImportDialog.column.terrestrial.station.height.label", "ih \u25B6");
			buttonTooltip = i18n.getString("ColumnImportDialog.column.terrestrial.station.height.tooltip", "Add selected range for instrument height");
			textFieldTooltip = i18n.getString("ColumnImportDialog.column.terrestrial.station.height.text.tooltip", "Range for instrument height column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.INSTRUMENT_HEIGHT, buttonLabel, buttonTooltip, textFieldTooltip);

			// a-priori observation value
			buttonLabel = i18n.getString("ColumnImportDialog.column.terrestrial.value.label", "Value0 \u25B6");
			buttonTooltip = i18n.getString("ColumnImportDialog.column.terrestrial.value.tooltip", "Add selected range for a-priori observation value");
			textFieldTooltip = i18n.getString("ColumnImportDialog.column.terrestrial.value.text.tooltip", "Range for observation value column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.VALUE, buttonLabel, buttonTooltip, textFieldTooltip);

			// Next column
			columnIndex = 0;
			rowIndex++;

			// Target point
			buttonLabel = i18n.getString("ColumnImportDialog.column.terrestrial.target.label", "Target \u25B6");
			buttonTooltip = i18n.getString("ColumnImportDialog.column.terrestrial.target.tooltip", "Add selected range for target point");
			textFieldTooltip = i18n.getString("ColumnImportDialog.column.terrestrial.target.text.tooltip", "Range for target point column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.TARGET, buttonLabel, buttonTooltip, textFieldTooltip);

			// target height
			buttonLabel = i18n.getString("ColumnImportDialog.column.terrestrial.target.height.label", "th \u25B6");
			buttonTooltip = i18n.getString("ColumnImportDialog.column.terrestrial.target.height.tooltip", "Add selected range for target height");
			textFieldTooltip = i18n.getString("ColumnImportDialog.column.terrestrial.target.height.text.tooltip", "Range for target height column");

			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.TARGET_HEIGHT, buttonLabel, buttonTooltip, textFieldTooltip);

			// Uncertainty
			buttonLabel = i18n.getString("ColumnImportDialog.column.terrestrial.sigma0.label", "\u03C30 \u25B6");
			buttonTooltip = i18n.getString("ColumnImportDialog.column.terrestrial.sigma0.tooltip", "Add selected range for a-priori uncertainty");
			textFieldTooltip = i18n.getString("ColumnImportDialog.column.terrestrial.sigma0.text.tooltip", "Range for uncertainty column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.UNCERTAINTY, buttonLabel, buttonTooltip, textFieldTooltip);

			// Distance for uncertainty
			buttonLabel = i18n.getString("ColumnImportDialog.column.terrestrial.distance.label", "d0 \u25B6");
			buttonTooltip = i18n.getString("ColumnImportDialog.column.terrestrial.distance.tooltip", "Add selected range for length approximation for distance dependent uncertainty");
			textFieldTooltip = i18n.getString("ColumnImportDialog.column.terrestrial.distance.text.tooltip", "Range for length approximation for distance dependent uncertainty calculation column");
			columnIndex = this.addPickerElement(gridPane, rowIndex, columnIndex, CSVColumnType.DISTANCE_FOR_UNCERTAINTY, buttonLabel, buttonTooltip, textFieldTooltip);

			TitledPane titledPane = this.createTitledPane(i18n.getString("ColumnImportDialog.column.terrestrial.label", "Column defintions for terrestrial observations"), i18n.getString("ColumnImportDialog.column.terrestrial.tooltip", "Specify column range of file for terrestrial observations import"), gridPane);

			return titledPane;
		}

		private int addPickerElement(GridPane gridPane, int rowIndex, int columnIndex, CSVColumnType type, string buttonLabel, string buttonTooltip, string textFieldTooltip)
		{
			string promptText = i18n.getString("ColumnImportDialog.column.range.prompt", "from - to");
			TextField textField = new TextField();
			textField.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			textField.setMaxSize(double.MaxValue, Control.USE_PREF_SIZE); // maxWidth, double maxHeight
			textField.setPrefWidth(Control.USE_PREF_SIZE); // 100 besser 75
			textField.setTooltip(new Tooltip(textFieldTooltip));
			textField.setPromptText(promptText);
			textField.setUserData(type);

			System.Func<TextFormatter.Change, TextFormatter.Change> textFilter = (Change change) =>
			{
			string input = change.getControlNewText(); //change.getText();
			Pattern pattern;
			if (importCSVFileCheckBox.isSelected())
			{
				pattern = Pattern.compile("^(\\d+)$");
			}
			else
			{
				pattern = Pattern.compile("^(\\d+)(\\s*-\\s*(\\d*))?$");
			}

			Matcher matcher = pattern.matcher(input.Trim());
			if (!change.isContentChange() || input.Trim().Length == 0)
			{
				return change;
			}

			if (!matcher.matches())
			{
				return null;
			}

			return change;
			};

			textField.setTextFormatter(new TextFormatter<string>(textFilter));

			Button button = new Button(buttonLabel);
			button.setTooltip(new Tooltip(buttonTooltip));
			button.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			button.setMaxSize(double.MaxValue, Control.USE_PREF_SIZE); // maxWidth, double maxHeight
			button.setUserData(textField);

			button.setOnAction(new EventHandlerAnonymousInnerClass(this, textField));

			// child, columnIndex, rowIndex
			gridPane.add(button, columnIndex++, rowIndex);
			gridPane.add(textField, columnIndex++, rowIndex);

			GridPane.setHgrow(button, Priority.NEVER);
			GridPane.setHgrow(textField, Priority.ALWAYS);

			this.textFieldList.Add(textField);
			return columnIndex;
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<ActionEvent>
		{
			private readonly ColumnImportDialog outerInstance;

			private TextField textField;

			public EventHandlerAnonymousInnerClass(ColumnImportDialog outerInstance, TextField textField)
			{
				this.outerInstance = outerInstance;
				this.textField = textField;
			}

			public override void handle(ActionEvent e)
			{
				int[] columns = outerInstance.SelectedColumns;
				int min = columns[0];
				int max = columns[1];

				if (min >= 0 && max >= 0)
				{
					if (outerInstance.importCSVFileCheckBox.isSelected())
					{
						textField.setText(min.ToString());
					}
					else
					{
						textField.setText(min.ToString() + " - " + max.ToString());
					}
				}
			}
		}

		private Node initEditor()
		{
			ColumnSelectionEventHandler columnSelectionEventHandler = new ColumnSelectionEventHandler(this);
			this.editor = new HBox();
			this.editor.setOnMouseMoved(columnSelectionEventHandler);
			this.editor.setOnMouseReleased(columnSelectionEventHandler);
			this.editor.setOnMousePressed(columnSelectionEventHandler);
			this.editor.setOnMouseDragged(columnSelectionEventHandler);
			this.editor.setOnMouseClicked(columnSelectionEventHandler);

			this.editor.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			this.editor.setMaxSize(double.MaxValue, double.MaxValue);
			this.editor.setPadding(new Insets(5));

			this.editor.setBackground(new Background(new BackgroundFill(Color.WHITE, CornerRadii.EMPTY, Insets.EMPTY)));
			this.editor.setBorder(new Border(new BorderStroke(Color.DARKGRAY, BorderStrokeStyle.SOLID, CornerRadii.EMPTY, BorderWidths.DEFAULT)));

			VBox column = new VBox(0);
			column.setBackground(new Background(new BackgroundFill(Color.TRANSPARENT, CornerRadii.EMPTY, Insets.EMPTY)));
			column.setBorder(new Border(new BorderStroke(Color.TRANSPARENT, Color.TRANSPARENT, Color.TRANSPARENT, Color.TRANSPARENT, BorderStrokeStyle.NONE, BorderStrokeStyle.NONE, BorderStrokeStyle.NONE, BorderStrokeStyle.NONE, CornerRadii.EMPTY, new BorderWidths(1), Insets.EMPTY)));

			for (int j = 0; j < MAX_LINES; j++)
			{
				Text text = new Text(" ");
				text.setFont(Font.font("MonoSpace", FontWeight.NORMAL, 12));
				column.getChildren().add(text);
			}

			this.editor.getChildren().add(column);

			ScrollPane editorScrollPane = new ScrollPane(this.editor);
			editorScrollPane.setFitToHeight(true);
			editorScrollPane.setFitToWidth(true);

			TitledPane titledPane = this.createTitledPane(i18n.getString("ColumnImportDialog.file.preview.label", "File preview"), i18n.getString("ColumnImportDialog.file.preview.tooltip", "Preview of selected file"), editorScrollPane);

			return titledPane;
		}

		private void addColumnHeader(int columns, int[] columnSize)
		{
			StringBuilder headerLine = new StringBuilder();

			if (this.importCSVFileCheckBox != null && this.importCSVFileCheckBox.isSelected() && columnSize != null)
			{
				int j = 0, index = 1;
				int columnLength = 0;
				int halfInterval = 0;
				for (int i = 0; i < columns; i++)
				{
					if (i == columnLength)
					{
						headerLine.Append("["); //\uFE64
						columnLength += columnSize[j];
						halfInterval = columnSize[j] / 2 + 1; // new +1 to center marker
						j++;
					}
					else if (i == columnLength - 1)
					{
						headerLine.Append("]"); // \uFE65
					}
					else if (i == columnLength - halfInterval)
					{
						string str = (index++).ToString();
						int endPos = i + 1;
						int startPos = endPos - str.Length;
						headerLine.Remove(startPos, endPos - startPos).Insert(startPos, str);
					}
					else
					{
						headerLine.Append("\u2219");
					}
				}
			}
			else
			{
				for (int i = 0; i < columns; i++)
				{
					if (i % 5 == 0) // && i%10 != 0
					{
						headerLine.Append("+");
					}
					else
					{
						headerLine.Append("\u2219");
					}

					if (i % 10 == 0)
					{
						string str = i.ToString();
						int endPos = i + 1;
						int startPos = endPos - str.Length;
						headerLine.Remove(startPos, endPos - startPos).Insert(startPos, str);
					}
				}
			}

			for (int j = 0; j < columns; j++)
			{
				Text text = new Text(headerLine[j].ToString());
				text.setFont(Font.font("MonoSpace", FontWeight.BOLD, 12));
				text.setFill(Color.DARKBLUE);
				text.setTextAlignment(TextAlignment.CENTER);

				this.columns[j].getChildren().add(text);
			}
		}

		private void selectColumns(int startColumn, int endColumn)
		{
			for (int columnIndex = 0; columnIndex < this.columns.Count; columnIndex++)
			{
				VBox column = this.columns[columnIndex];
				if (columnIndex >= startColumn && columnIndex <= endColumn)
				{
					column.setBackground(new Background(new BackgroundFill(Color.LIGHTGRAY, CornerRadii.EMPTY, Insets.EMPTY)));
					column.setBorder(new Border(new BorderStroke(Color.BLACK, columnIndex == endColumn ? Color.BLACK : Color.TRANSPARENT, Color.BLACK, columnIndex == startColumn ? Color.BLACK : Color.TRANSPARENT, BorderStrokeStyle.DOTTED, columnIndex == endColumn ? BorderStrokeStyle.DOTTED : BorderStrokeStyle.NONE, BorderStrokeStyle.DOTTED, columnIndex == startColumn ? BorderStrokeStyle.DOTTED : BorderStrokeStyle.NONE, CornerRadii.EMPTY, new BorderWidths(1), Insets.EMPTY)));
				}
				else
				{
					column.setBackground(new Background(new BackgroundFill(Color.TRANSPARENT, CornerRadii.EMPTY, Insets.EMPTY)));
					column.setBorder(new Border(new BorderStroke(Color.TRANSPARENT, Color.TRANSPARENT, Color.TRANSPARENT, Color.TRANSPARENT, BorderStrokeStyle.NONE, BorderStrokeStyle.NONE, BorderStrokeStyle.NONE, BorderStrokeStyle.NONE, CornerRadii.EMPTY, new BorderWidths(1), Insets.EMPTY)));
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void readPreview(java.io.File f) throws IOException, java.sql.SQLException
		private void readPreview(File f)
		{
			PreviewFileReader reader = new PreviewFileReader(f, MAX_LINES);
			reader.ignoreLinesWhichStartWith("#");

			reader.read();
			this.linesOfFile.Clear();
			this.linesOfFile = reader.Lines;
			this.changeMode();
		}

		private int[] SelectedColumns
		{
			get
			{
				if (this.importCSVFileCheckBox.isSelected())
				{
					int columnLength = 0;
					for (int i = 0; i < this.maxCharactersPerColumn.Length; i++)
					{
						if (this.startColumn <= columnLength && columnLength < this.endColumn)
						{
							return new int[] {i + 1, i + 1};
						}
						columnLength += this.maxCharactersPerColumn[i];
					}
				}
    
				return new int[] {Math.Min(this.endColumn, this.startColumn), Math.Max(this.endColumn, this.startColumn)};
			}
		}

		private void showContentPreview()
		{
			// Size of content
			int contentColumns = 0;
			int contentRows = this.linesOfFile.Count;
			this.columns.Clear();
			this.editor.getChildren().clear();

			this.maxCharactersPerColumn = null;
			IList<string> formattedLines = new List<string>(contentRows);

			if (this.importCSVFileCheckBox != null && this.importCSVFileCheckBox.isSelected())
			{
				if (anyCharactersAreTheSame(this.separator, this.quotechar, this.escape))
				{
					return;
				}

				const int BUFFER_CHARS = 5;
				try
				{
					bool strictQuotes = CSVParser.DEFAULT_STRICT_QUOTES;
					bool ignoreLeadingWhiteSpace = CSVParser.DEFAULT_IGNORE_LEADING_WHITESPACE;
					bool ignoreQuotations = CSVParser.DEFAULT_IGNORE_QUOTATIONS;

					CSVParser csvParser = new CSVParser(this.separator, this.quotechar, this.escape, strictQuotes, ignoreLeadingWhiteSpace, ignoreQuotations);
					int columCounter = 0;

					// parse CSV 
					IList<string[]> fileData = new List<string[]>(20);
					IList<string> row = new List<string>(20);
					for (int i = 0; i < this.linesOfFile.Count; i++)
					{
						string line = this.linesOfFile[i];
						string[] parsedLine = csvParser.parseLineMulti(line);

						if (parsedLine != null && parsedLine.Length > 0)
						{
							((List<string>)row).AddRange(new List<string> {parsedLine});
						}

						if (!csvParser.Pending)
						{
							fileData.Add(((List<string>)row).ToArray());
							columCounter = Math.Max(columCounter, row.Count);
							row.Clear();
						}
					}

					// max. number of characters per column (for cell formatter)
					this.maxCharactersPerColumn = new int[columCounter];
					foreach (string[] line in fileData)
					{
						for (int i = 0; i < line.Length; i++)
						{
							line[i] = line[i].Replace('\n', ' ').replaceAll("\t", TABULATOR).Trim();
							this.maxCharactersPerColumn[i] = Math.Max(this.maxCharactersPerColumn[i], line[i].Length + BUFFER_CHARS);
						}
					}

					foreach (string[] line in fileData)
					{
						StringBuilder formattedLine = new StringBuilder();
						for (int i = 0; i < line.Length; i++)
						{
							formattedLine.Append(String.format(Locale.ENGLISH, "%" + maxCharactersPerColumn[i] + "s", line[i]));
						}
						formattedLines.Add(formattedLine.ToString().replaceAll("\\s+$", ""));
					}

				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
			else
			{
				foreach (string line in this.linesOfFile)
				{
					formattedLines.Add(line.replaceAll("\t", TABULATOR));
				}
			}

			// max row
			foreach (string str in formattedLines)
			{
				contentColumns = Math.Max(contentColumns, str.Length);
			}

			// Add column nodes to editor
			for (int i = 0; i < contentColumns; i++)
			{
				VBox column = new VBox(0);
				column.setUserData(new ColumnIndex(this, i));

				column.setBackground(new Background(new BackgroundFill(Color.TRANSPARENT, CornerRadii.EMPTY, Insets.EMPTY)));
				column.setBorder(new Border(new BorderStroke(Color.TRANSPARENT, Color.TRANSPARENT, Color.TRANSPARENT, Color.TRANSPARENT, BorderStrokeStyle.NONE, BorderStrokeStyle.NONE, BorderStrokeStyle.NONE, BorderStrokeStyle.NONE, CornerRadii.EMPTY, new BorderWidths(1), Insets.EMPTY)));

				this.editor.getChildren().add(column);
				this.columns.Add(column);
			}

			// set header line
			this.addColumnHeader(contentColumns, this.maxCharactersPerColumn);

			// add content
			for (int i = 0; i < Math.Min(formattedLines.Count, MAX_LINES); i++)
			{
				string line = String.format(Locale.ENGLISH, "%-" + contentColumns + "s", formattedLines[i]);
				for (int j = 0; j < contentColumns; j++)
				{
					Text character = new Text(line[j].ToString());
					character.setFont(Font.font("MonoSpace", FontWeight.NORMAL, 12));
					character.setFill(Color.BLACK);
					character.setTextAlignment(TextAlignment.CENTER);

					this.columns[j].getChildren().add(character);
				}
			}

			for (int i = formattedLines.Count; i < MAX_LINES; i++)
			{
				for (int j = 0; j < contentColumns; j++)
				{
					Text character = new Text(" ");
					character.setFont(Font.font("MonoSpace", FontWeight.NORMAL, 12));
					character.setFill(Color.TRANSPARENT);
					character.setTextAlignment(TextAlignment.CENTER);
					this.columns[j].getChildren().add(character);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: private javafx.scene.control.ComboBox<Enum<?>> createImportTypeComboBox()
		private ComboBox<Enum<object>> createImportTypeComboBox()
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: javafx.scene.control.ComboBox<Enum<?>> typeComboBox = new javafx.scene.control.ComboBox<Enum<?>>();
			ComboBox<Enum<object>> typeComboBox = new ComboBox<Enum<object>>();
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: java.util.List<Enum<?>> model = typeComboBox.getItems();
			IList<Enum<object>> model = typeComboBox.getItems();

			foreach (PointType type in PointType.values())
			{
				model.Add(type);
			}

			foreach (ObservationType type in ObservationType.values())
			{
				model.Add(type);
			}

			foreach (VerticalDeflectionType type in VerticalDeflectionType.values())
			{
				model.Add(type);
			}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: typeComboBox.setConverter(new javafx.util.StringConverter<Enum<?>>()
			typeComboBox.setConverter(new StringConverterAnonymousInnerClass(this));

			typeComboBox.setTooltip(new Tooltip(i18n.getString("ColumnImportDialog.import.type.tooltip", "Select import type")));

			typeComboBox.setMinWidth(Control.USE_PREF_SIZE);
			typeComboBox.setMaxWidth(double.MaxValue);
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: typeComboBox.getSelectionModel().selectedItemProperty().addListener(new javafx.beans.value.ChangeListener<Enum<?>>()
			typeComboBox.getSelectionModel().selectedItemProperty().addListener(new ChangeListenerAnonymousInnerClass3(this));
			typeComboBox.getSelectionModel().select(PointType.REFERENCE_POINT);
			return typeComboBox;
		}

		private class StringConverterAnonymousInnerClass : StringConverter<Enum<JavaToDotNetGenericWildcard>>
		{
			private readonly ColumnImportDialog outerInstance;

			public StringConverterAnonymousInnerClass(ColumnImportDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}


//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: @Override public Enum<?> fromString(String string)
			public override Enum<object> fromString(string @string)
			{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: Enum<?> type = org.applied_geodesy.adjustment.network.PointType.valueOf(string);
				Enum<object> type = PointType.valueOf(@string);
				return type != null ? type : ObservationType.valueOf(@string);
			}

			public override string toString<T1>(Enum<T1> item)
			{
				if (item is PointType)
				{
					PointType type = (PointType)item;
					switch (type.innerEnumValue)
					{
					case PointType.InnerEnum.DATUM_POINT:
						return outerInstance.i18n.getString("UITreeBuiler.directory.points.datum", "Datum points");

					case PointType.InnerEnum.NEW_POINT:
						return outerInstance.i18n.getString("UITreeBuiler.directory.points.new", "New points");

					case PointType.InnerEnum.REFERENCE_POINT:
						return outerInstance.i18n.getString("UITreeBuiler.directory.points.reference", "Reference points");

					case PointType.InnerEnum.STOCHASTIC_POINT:
						return outerInstance.i18n.getString("UITreeBuiler.directory.points.stochastic", "Stochastic points");
					}
				}

				else if (item is ObservationType)
				{
					ObservationType type = (ObservationType)item;
					switch (type.innerEnumValue)
					{
					case ObservationType.InnerEnum.LEVELING:
						return outerInstance.i18n.getString("UITreeBuiler.directory.observations.leveling", "Leveling data");

					case ObservationType.InnerEnum.DIRECTION:
						return outerInstance.i18n.getString("UITreeBuiler.directory.observations.direction", "Direction sets");

					case ObservationType.InnerEnum.HORIZONTAL_DISTANCE:
						return outerInstance.i18n.getString("UITreeBuiler.directory.observations.horizontal_distance", "Horizontal distances");

					case ObservationType.InnerEnum.SLOPE_DISTANCE:
						return outerInstance.i18n.getString("UITreeBuiler.directory.observations.slope_distance", "Slope distances");

					case ObservationType.InnerEnum.ZENITH_ANGLE:
						return outerInstance.i18n.getString("UITreeBuiler.directory.observations.zenith_angle", "Zenith angles");

					case ObservationType.InnerEnum.GNSS1D:
						return outerInstance.i18n.getString("UITreeBuiler.directory.gnss.1d", "GNSS baselines 1D");

					case ObservationType.InnerEnum.GNSS2D:
						return outerInstance.i18n.getString("UITreeBuiler.directory.gnss.2d", "GNSS baselines 2D");

					case ObservationType.InnerEnum.GNSS3D:
						return outerInstance.i18n.getString("UITreeBuiler.directory.gnss.3d", "GNSS baselines 3D");
					}
				}

				else if (item is VerticalDeflectionType)
				{
					VerticalDeflectionType type = (VerticalDeflectionType)item;
					switch (type.innerEnumValue)
					{
					case VerticalDeflectionType.InnerEnum.UNKNOWN_VERTICAL_DEFLECTION:
						return outerInstance.i18n.getString("UITreeBuiler.directory.vertical_deflection.unknown", "Unknown deflection");

					case VerticalDeflectionType.InnerEnum.STOCHASTIC_VERTICAL_DEFLECTION:
						return outerInstance.i18n.getString("UITreeBuiler.directory.vertical_deflection.stochastic", "Stochastic deflection");

					case VerticalDeflectionType.InnerEnum.REFERENCE_VERTICAL_DEFLECTION:
						return outerInstance.i18n.getString("UITreeBuiler.directory.vertical_deflection.reference", "Reference deflection");

					}
				}

				return null;
			}
		}

		private class ChangeListenerAnonymousInnerClass3 : ChangeListener<Enum<JavaToDotNetGenericWildcard>>
		{
			private readonly ColumnImportDialog outerInstance;

			public ChangeListenerAnonymousInnerClass3(ColumnImportDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1, T2, T3, T4>(ObservableValue<T1, T2> observable, Enum<T3> oldValue, Enum<T4> newValue) where T1 : Enum<T1>
			{
				if (newValue is PointType)
				{
					outerInstance.terrestrialObservationColumnPickerPane.setVisible(false);
					outerInstance.terrestrialObservationColumnPickerPane.setManaged(false);

					outerInstance.gnssObservationColumnPickerPane.setVisible(false);
					outerInstance.gnssObservationColumnPickerPane.setManaged(false);

					outerInstance.deflectionColumnPickerPane.setVisible(false);
					outerInstance.deflectionColumnPickerPane.setManaged(false);

					outerInstance.pointColumnPickerPane.setVisible(true);
					outerInstance.pointColumnPickerPane.setManaged(true);
				}


				else if (newValue is ObservationType)
				{
					if (outerInstance.isGNSS((ObservationType)newValue))
					{
						outerInstance.gnssObservationColumnPickerPane.setVisible(true);
						outerInstance.gnssObservationColumnPickerPane.setManaged(true);

						outerInstance.terrestrialObservationColumnPickerPane.setVisible(false);
						outerInstance.terrestrialObservationColumnPickerPane.setManaged(false);
					}
					else
					{
						outerInstance.terrestrialObservationColumnPickerPane.setVisible(true);
						outerInstance.terrestrialObservationColumnPickerPane.setManaged(true);

						outerInstance.gnssObservationColumnPickerPane.setVisible(false);
						outerInstance.gnssObservationColumnPickerPane.setManaged(false);
					}

					outerInstance.deflectionColumnPickerPane.setVisible(false);
					outerInstance.deflectionColumnPickerPane.setManaged(false);

					outerInstance.pointColumnPickerPane.setVisible(false);
					outerInstance.pointColumnPickerPane.setManaged(false);
				}

				else if (newValue is VerticalDeflectionType)
				{
					outerInstance.terrestrialObservationColumnPickerPane.setVisible(false);
					outerInstance.terrestrialObservationColumnPickerPane.setManaged(false);

					outerInstance.gnssObservationColumnPickerPane.setVisible(false);
					outerInstance.gnssObservationColumnPickerPane.setManaged(false);

					outerInstance.deflectionColumnPickerPane.setVisible(true);
					outerInstance.deflectionColumnPickerPane.setManaged(true);

					outerInstance.pointColumnPickerPane.setVisible(false);
					outerInstance.pointColumnPickerPane.setManaged(false);
				}

			}
		}

		private CheckBox createCSVCheckbox()
		{
			string title = i18n.getString("ColumnImportDialog.type.csv.label", "CSV file");
			string tooltip = i18n.getString("ColumnImportDialog.type.csv.tooltip", "If selected, file is parsed as character-separated-values (CSV) file");
			Label label = new Label(title);
			label.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			label.setMaxSize(double.MaxValue, double.MaxValue);
			label.setPadding(new Insets(0,0,0,3));
			CheckBox checkBox = new CheckBox();
			checkBox.setGraphic(label);
			checkBox.setTooltip(new Tooltip(tooltip));
			checkBox.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			checkBox.setMaxSize(double.MaxValue,double.MaxValue);
			checkBox.selectedProperty().addListener(new ChangeListenerAnonymousInnerClass4(this));
			this.changeMode();
			return checkBox;
		}

		private class ChangeListenerAnonymousInnerClass4 : ChangeListener<bool>
		{
			private readonly ColumnImportDialog outerInstance;

			public ChangeListenerAnonymousInnerClass4(ColumnImportDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public override void changed<T1>(ObservableValue<T1> observable, bool? oldValue, bool? newValue) where T1 : bool
			{
				outerInstance.changeMode();
			}

		}

		private RadioButton createRadioButton(string title, string tooltip, bool selected, object userData, ToggleGroup group, ChangeListener<bool> listener)
		{
			Label label = new Label(title);
			label.setPadding(new Insets(0,0,0,3));
			label.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			label.setMaxSize(double.MaxValue, double.MaxValue);
			RadioButton radioButton = new RadioButton();
			radioButton.setGraphic(label);
			radioButton.setTooltip(new Tooltip(tooltip));
			radioButton.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			radioButton.setMaxWidth(double.MaxValue);
			radioButton.setUserData(userData);
			radioButton.setSelected(selected);
			radioButton.setToggleGroup(group);
			radioButton.selectedProperty().addListener(listener);
			return radioButton;
		}

		private TitledPane createTitledPane(string title, string tooltip, Node content)
		{
			Label label = new Label(title);
			label.setTooltip(new Tooltip(tooltip));
			TitledPane titledPane = new TitledPane();
	//		titledPane.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			titledPane.setMaxSize(double.MaxValue,double.MaxValue);
			titledPane.setGraphic(label);
			titledPane.setCollapsible(false);
			titledPane.setAnimated(false);
			titledPane.setContent(content);
			titledPane.setPadding(new Insets(0, 10, 5, 10)); // oben, links, unten, rechts
			return titledPane;
		}

		private GridPane createGridPane()
		{
			GridPane gridPane = new GridPane();
			gridPane.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			gridPane.setMaxSize(double.MaxValue,double.MaxValue);
			gridPane.setAlignment(Pos.CENTER_LEFT);
			gridPane.setHgap(15);
			gridPane.setVgap(7);
			gridPane.setPadding(new Insets(5, 5, 5, 5)); // oben, recht, unten, links
			return gridPane;
		}

		private void changeMode()
		{
			bool isCSVMode = this.importCSVFileCheckBox != null && this.importCSVFileCheckBox.isSelected();
			if (this.csvOptionPane != null)
			{
				this.csvOptionPane.setDisable(!isCSVMode);
			}

			string promptText = isCSVMode ? i18n.getString("ColumnImportDialog.column.index.prompt", "Index") : i18n.getString("ColumnImportDialog.column.range.prompt", "from - to");
			foreach (TextField textfield in this.textFieldList)
			{
				textfield.setPromptText(promptText);
				textfield.setText("");
			}

			showContentPreview();
		}

		private bool anyCharactersAreTheSame(char separator, char quotechar, char escape)
		{
			return isSameCharacter(separator, quotechar) || isSameCharacter(separator, escape) || isSameCharacter(quotechar, escape);
		}

		private bool isSameCharacter(char c1, char c2)
		{
			return c1 != CSVParser.NULL_CHARACTER && c1 == c2;
		}

		private ColumnRange getColumnRange(TextField textField)
		{
			if (textField.getText() == null || textField.getText().Trim().isEmpty() || textField.getUserData() == null || !(textField.getUserData() is CSVColumnType))
			{
				return null;
			}

			CSVColumnType type = (CSVColumnType)textField.getUserData();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.regex.Pattern pattern = java.util.regex.Pattern.compile("^(\\d+)(\\s*-\\s*(\\d*))?$");
			Pattern pattern = Pattern.compile("^(\\d+)(\\s*-\\s*(\\d*))?$");
			Matcher matcher = pattern.matcher(textField.getText().Trim());

			if (matcher.matches())
			{
				if (matcher.group(3) == null && this.importCSVFileCheckBox.isSelected())
				{
					return new ColumnRange(type, int.Parse(matcher.group(1)), int.Parse(matcher.group(1)));
				}
				else
				{
					return new ColumnRange(type, int.Parse(matcher.group(1)), int.Parse(matcher.group(3)));
				}
			}
			return null;
		}

		private SourceFileReader<TreeItem<TreeItemValue>> SourceFileReader
		{
			get
			{
				IList<ColumnRange> columnRanges = new List<ColumnRange>(this.textFieldList.Count);
				foreach (TextField textField in this.textFieldList)
				{
					ColumnRange columnRange = this.getColumnRange(textField);
					if (columnRange != null)
					{
						columnRanges.Add(columnRange);
					}
				}
    
				if (columnRanges.Count == 0)
				{
					return null;
				}
    
				if (this.importCSVFileCheckBox.isSelected())
				{
					bool strictQuotes = CSVParser.DEFAULT_STRICT_QUOTES;
					bool ignoreLeadingWhiteSpace = CSVParser.DEFAULT_IGNORE_LEADING_WHITESPACE;
					bool ignoreQuotations = CSVParser.DEFAULT_IGNORE_QUOTATIONS;
					CSVParser csvParser = new CSVParser(this.separator, this.quotechar, this.escape, strictQuotes, ignoreLeadingWhiteSpace, ignoreQuotations);
    
					if (this.importTypes.getValue() is ObservationType)
					{
						ObservationType observationType = (ObservationType)this.importTypes.getValue();
						CSVObservationFileReader reader = new CSVObservationFileReader(observationType, csvParser);
						reader.ColumnRanges = columnRanges;
						reader.FileLocale = this.fileLocale;
						return reader;
					}
					else if (this.importTypes.getValue() is PointType)
					{
						PointType pointType = (PointType)this.importTypes.getValue();
						CSVPointFileReader reader = new CSVPointFileReader(pointType, csvParser);
						reader.ColumnRanges = columnRanges;
						reader.FileLocale = this.fileLocale;
						return reader;
					}
					else if (this.importTypes.getValue() is VerticalDeflectionType)
					{
						VerticalDeflectionType verticalDeflectionType = (VerticalDeflectionType)this.importTypes.getValue();
						CSVVerticalDeflectionFileReader reader = new CSVVerticalDeflectionFileReader(verticalDeflectionType, csvParser);
						reader.ColumnRanges = columnRanges;
						reader.FileLocale = this.fileLocale;
						return reader;
					}
				}
				else
				{
					if (this.importTypes.getValue() is ObservationType)
					{
						ObservationType observationType = (ObservationType)this.importTypes.getValue();
						ColumnDefinedObservationFileReader reader = new ColumnDefinedObservationFileReader(observationType, TABULATOR);
						reader.ColumnRanges = columnRanges;
						reader.FileLocale = this.fileLocale;
						return reader;
					}
					else if (this.importTypes.getValue() is PointType)
					{
						PointType pointType = (PointType)this.importTypes.getValue();
						ColumnDefinedPointFileReader reader = new ColumnDefinedPointFileReader(pointType, TABULATOR);
						reader.ColumnRanges = columnRanges;
						reader.FileLocale = this.fileLocale;
						return reader;
					}
					else if (this.importTypes.getValue() is VerticalDeflectionType)
					{
						VerticalDeflectionType verticalDeflectionType = (VerticalDeflectionType)this.importTypes.getValue();
						ColumnDefinedVerticalDeflectionFileReader reader = new ColumnDefinedVerticalDeflectionFileReader(verticalDeflectionType, TABULATOR);
						reader.ColumnRanges = columnRanges;
						reader.FileLocale = this.fileLocale;
						return reader;
					}
				}
				return null;
			}
		}

		private bool isGNSS(ObservationType type)
		{
			switch (type.innerEnumValue)
			{
			case ObservationType.InnerEnum.GNSS1D:
			case ObservationType.InnerEnum.GNSS2D:
			case ObservationType.InnerEnum.GNSS3D:
				return true;
			default:
				return false;
			}
		}
	}

}