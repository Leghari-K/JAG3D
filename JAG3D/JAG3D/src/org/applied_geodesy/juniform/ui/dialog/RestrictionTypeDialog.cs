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

namespace org.applied_geodesy.juniform.ui.dialog
{

	using RestrictionType = org.applied_geodesy.adjustment.geometry.restriction.RestrictionType;
	using I18N = org.applied_geodesy.juniform.ui.i18n.I18N;

	using Platform = javafx.application.Platform;
	using Insets = javafx.geometry.Insets;
	using Node = javafx.scene.Node;
	using ButtonType = javafx.scene.control.ButtonType;
	using ComboBox = javafx.scene.control.ComboBox;
	using Control = javafx.scene.control.Control;
	using Dialog = javafx.scene.control.Dialog;
	using Label = javafx.scene.control.Label;
	using GridPane = javafx.scene.layout.GridPane;
	using Priority = javafx.scene.layout.Priority;
	using Modality = javafx.stage.Modality;
	using Stage = javafx.stage.Stage;
	using Window = javafx.stage.Window;
	using Callback = javafx.util.Callback;
	using StringConverter = javafx.util.StringConverter;

	public class RestrictionTypeDialog
	{
		private static I18N i18N = I18N.Instance;
		private static RestrictionTypeDialog restrictionTypeDialog = new RestrictionTypeDialog();
		private Dialog<RestrictionType> dialog = null;
		private ComboBox<RestrictionType> restrictionTypeComboBox;
		private Window window;

		private RestrictionTypeDialog()
		{
		}

		public static Window Owner
		{
			set
			{
				restrictionTypeDialog.window = value;
			}
		}

		public static Optional<RestrictionType> showAndWait()
		{
			restrictionTypeDialog.init();
			// @see https://bugs.openjdk.java.net/browse/JDK-8087458
			Platform.runLater(() =>
			{
			try
			{
				restrictionTypeDialog.dialog.getDialogPane().requestLayout();
				Stage stage = (Stage) restrictionTypeDialog.dialog.getDialogPane().getScene().getWindow();
				stage.sizeToScene();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			});
			return restrictionTypeDialog.dialog.showAndWait();
		}

		private void init()
		{
			if (this.dialog != null)
			{
				return;
			}

			this.dialog = new Dialog<RestrictionType>();
			this.dialog.setTitle(i18N.getString("RestrictionTypeDialog.title", "Restrictions"));
			this.dialog.setHeaderText(i18N.getString("RestrictionTypeDialog.header", "Parameter restrictions"));
			this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.OK, ButtonType.CANCEL);
			this.dialog.initModality(Modality.APPLICATION_MODAL);
			this.dialog.initOwner(window);
			this.dialog.getDialogPane().setContent(this.createPane());
			this.dialog.setResizable(true);
			this.dialog.setResultConverter(new CallbackAnonymousInnerClass3(this));
		}

		private class CallbackAnonymousInnerClass3 : Callback<ButtonType, RestrictionType>
		{
			private readonly RestrictionTypeDialog outerInstance;

			public CallbackAnonymousInnerClass3(RestrictionTypeDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override RestrictionType call(ButtonType buttonType)
			{
				if (buttonType == ButtonType.OK)
				{
					return outerInstance.restrictionTypeComboBox.getValue();
				}
				return null;
			}
		}

		private Node createPane()
		{
			GridPane gridPane = DialogUtil.createGridPane();

			Label restrictionTypeLabel = new Label(i18N.getString("RestrictionTypeDialog.restriction.type.label", "Restriction type:"));
			this.restrictionTypeComboBox = DialogUtil.createRestrictionTypeComboBox(createRestrictionTypeStringConverter(), i18N.getString("RestrictionTypeDialog.restriction.type.tooltip", "Select restriction type"));

			restrictionTypeLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			restrictionTypeLabel.setMaxWidth(double.MaxValue);

			restrictionTypeLabel.setLabelFor(this.restrictionTypeComboBox);

			// https://stackoverflow.com/questions/50479384/gridpane-with-gaps-inside-scrollpane-rendering-wrong
			Insets insetsLeft = new Insets(5, 7, 5, 5);
			Insets insetsRight = new Insets(5, 0, 5, 7);

			GridPane.setMargin(restrictionTypeLabel, insetsLeft);

			GridPane.setMargin(this.restrictionTypeComboBox, insetsRight);

			GridPane.setHgrow(restrictionTypeLabel, Priority.NEVER);

			GridPane.setHgrow(this.restrictionTypeComboBox, Priority.ALWAYS);

			gridPane.add(restrictionTypeLabel, 0, 0); // column, row, columnspan, rowspan,
			gridPane.add(this.restrictionTypeComboBox, 1, 0);

			return gridPane;
		}

		internal static StringConverter<RestrictionType> createRestrictionTypeStringConverter()
		{
			return new StringConverterAnonymousInnerClass();
		}

		private class StringConverterAnonymousInnerClass : StringConverter<RestrictionType>
		{

			public override string toString(RestrictionType restrictionType)
			{
				return getRestrictionTypeLabel(restrictionType);
			}

			public override RestrictionType fromString(string @string)
			{
				return (RestrictionType)Enum.Parse(typeof(RestrictionType), @string);
			}
		}

		internal static string getRestrictionTypeLabel(RestrictionType restrictionType)
		{
			if (restrictionType == null)
			{
				return null;
			}

			switch (restrictionType)
			{
			case RestrictionType.AVERAGE:
				return i18N.getString("RestrictionTypeDialog.restriction.type.average", "Average value");

			case RestrictionType.PRODUCT_SUM:
				return i18N.getString("RestrictionTypeDialog.restriction.type.productsum", "k-th Power of product sum");

			case RestrictionType.TRIGONOMERTIC_FUNCTION:
				return i18N.getString("RestrictionTypeDialog.restriction.type.trigonometry", "Trigonometric function");

			case RestrictionType.FEATURE_POINT:
				return i18N.getString("RestrictionTypeDialog.restriction.type.featurepoint", "Feature point");

			case RestrictionType.VECTOR_ANGLE:
				return i18N.getString("RestrictionTypeDialog.restriction.type.vectorangle", "Vector angle");

			}
			throw new System.ArgumentException("Error, unknown restriction type " + restrictionType + "!");
		}
	}

}