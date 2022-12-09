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

	using ParameterType = org.applied_geodesy.adjustment.geometry.parameter.ParameterType;
	using ProcessingType = org.applied_geodesy.adjustment.geometry.parameter.ProcessingType;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using I18N = org.applied_geodesy.juniform.ui.i18n.I18N;

	using Platform = javafx.application.Platform;
	using Insets = javafx.geometry.Insets;
	using Node = javafx.scene.Node;
	using ButtonType = javafx.scene.control.ButtonType;
	using ComboBox = javafx.scene.control.ComboBox;
	using Control = javafx.scene.control.Control;
	using Dialog = javafx.scene.control.Dialog;
	using Label = javafx.scene.control.Label;
	using TextField = javafx.scene.control.TextField;
	using GridPane = javafx.scene.layout.GridPane;
	using Priority = javafx.scene.layout.Priority;
	using Modality = javafx.stage.Modality;
	using Stage = javafx.stage.Stage;
	using Window = javafx.stage.Window;
	using Callback = javafx.util.Callback;
	using StringConverter = javafx.util.StringConverter;

	public class UnknownParameterTypeDialog
	{
		private static I18N i18N = I18N.Instance;
		private static UnknownParameterTypeDialog parameterTypeDialog = new UnknownParameterTypeDialog();
		private Dialog<UnknownParameter> dialog = null;
		private ComboBox<ParameterType> parameterTypeComboBox;
		private TextField nameTextField;
		private Window window;

		private UnknownParameterTypeDialog()
		{
		}

		public static Window Owner
		{
			set
			{
				parameterTypeDialog.window = value;
			}
		}

		public static Optional<UnknownParameter> showAndWait()
		{
			parameterTypeDialog.init();
			// @see https://bugs.openjdk.java.net/browse/JDK-8087458
			Platform.runLater(() =>
			{
			try
			{
				parameterTypeDialog.dialog.getDialogPane().requestLayout();
				Stage stage = (Stage) parameterTypeDialog.dialog.getDialogPane().getScene().getWindow();
				stage.sizeToScene();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			});
			return parameterTypeDialog.dialog.showAndWait();
		}

		private void init()
		{
			if (this.dialog != null)
			{
				return;
			}

			this.dialog = new Dialog<UnknownParameter>();
			this.dialog.setTitle(i18N.getString("UnknownParameterTypeDialog.title", "Unknown parameter type"));
			this.dialog.setHeaderText(i18N.getString("UnknownParameterTypeDialog.header", "Type of unknown model parameter"));
			this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.OK, ButtonType.CANCEL);
			this.dialog.initModality(Modality.APPLICATION_MODAL);
			this.dialog.initOwner(window);
			this.dialog.getDialogPane().setContent(this.createPane());
			this.dialog.setResizable(true);
			this.dialog.setResultConverter(new CallbackAnonymousInnerClass3(this));
		}

		private class CallbackAnonymousInnerClass3 : Callback<ButtonType, UnknownParameter>
		{
			private readonly UnknownParameterTypeDialog outerInstance;

			public CallbackAnonymousInnerClass3(UnknownParameterTypeDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override UnknownParameter call(ButtonType buttonType)
			{
				if (buttonType == ButtonType.OK)
				{
					ParameterType parameterType = outerInstance.parameterTypeComboBox.getValue();
					if (parameterType != null)
					{
						UnknownParameter unknownParameter = getUnknownParameter(parameterType);
						unknownParameter.Name = outerInstance.nameTextField.getText().Trim();
						unknownParameter.Description = i18N.getString("UnknownParameterTypeDialog.parameter.description.default", "User defined parameter");
						return unknownParameter;
					}
				}
				return null;
			}
		}

		internal static UnknownParameter getUnknownParameter(ParameterType parameterType)
		{
			return new UnknownParameter(parameterType, false, 0.0, true, ProcessingType.POSTPROCESSING);
		}

		private Node createPane()
		{
			GridPane gridPane = DialogUtil.createGridPane();

			Label nameLabel = new Label(i18N.getString("UnknownParameterTypeDialog.parameter.name.label", "Name:"));
			Label typeLabel = new Label(i18N.getString("UnknownParameterTypeDialog.parameter.type.label", "Type:"));

			this.nameTextField = DialogUtil.createTextField(i18N.getString("UnknownParameterTypeDialog.parameter.name.tooltip", "Name of new unknwon parameter"), i18N.getString("UnknownParameterTypeDialog.parameter.name.prompt", "Parameter name"));
			this.parameterTypeComboBox = DialogUtil.createParameterTypeComboBox(createParameterTypeStrincConverter(), i18N.getString("UnknownParameterTypeDialog.parameter.type.tooltip", "Select parameter type"));

			nameLabel.setLabelFor(this.nameTextField);
			typeLabel.setLabelFor(this.parameterTypeComboBox);

			nameLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			typeLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			nameLabel.setMaxWidth(double.MaxValue);
			typeLabel.setMaxWidth(double.MaxValue);

			GridPane.setHgrow(nameLabel, Priority.NEVER);
			GridPane.setHgrow(typeLabel, Priority.NEVER);

			GridPane.setHgrow(this.nameTextField, Priority.ALWAYS);
			GridPane.setHgrow(this.parameterTypeComboBox, Priority.ALWAYS);

			// https://stackoverflow.com/questions/50479384/gridpane-with-gaps-inside-scrollpane-rendering-wrong
			Insets insetsLeft = new Insets(5, 7, 5, 5);
			Insets insetsRight = new Insets(5, 0, 5, 7);

			GridPane.setMargin(nameLabel, insetsLeft);
			GridPane.setMargin(typeLabel, insetsLeft);

			GridPane.setMargin(this.nameTextField, insetsRight);
			GridPane.setMargin(this.parameterTypeComboBox, insetsRight);

			gridPane.add(typeLabel, 0, 0); // column, row, columnspan, rowspan,
			gridPane.add(this.parameterTypeComboBox, 1, 0);

			gridPane.add(nameLabel, 0, 1);
			gridPane.add(this.nameTextField, 1, 1);

			return gridPane;
		}

		internal static StringConverter<ParameterType> createParameterTypeStrincConverter()
		{
			return new StringConverterAnonymousInnerClass2();
		}

		private class StringConverterAnonymousInnerClass2 : StringConverter<ParameterType>
		{

			public override string toString(ParameterType parameterType)
			{
				return getParameterTypeLabel(parameterType);
			}

			public override ParameterType fromString(string @string)
			{
				return (ParameterType)Enum.Parse(typeof(ParameterType), @string);
			}
		}

		public static string getParameterTypeLabel(ParameterType parameterType)
		{
			if (parameterType == null)
			{
				return null;
			}

			switch (parameterType)
			{
			case ParameterType.COORDINATE_X:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.coordinate.x", "Coordinate x");
			case ParameterType.COORDINATE_Y:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.coordinate.y", "Coordinate y");
			case ParameterType.COORDINATE_Z:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.coordinate.z", "Coordinate z");
			case ParameterType.LENGTH:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.length", "Length d");
			case ParameterType.VECTOR_X:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.vector.x", "Vector nx");
			case ParameterType.VECTOR_Y:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.vector.y", "Vector ny");
			case ParameterType.VECTOR_Z:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.vector.z", "Vector nz");
			case ParameterType.VECTOR_LENGTH:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.vector.norm", "Norm of vector \u2016n\u2016");
			case ParameterType.ORIGIN_COORDINATE_X:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.origin.x", "Origin x0");
			case ParameterType.ORIGIN_COORDINATE_Y:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.origin.y", "Origin y0");
			case ParameterType.ORIGIN_COORDINATE_Z:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.origin.z", "Origin z0");
			case ParameterType.RADIUS:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.radius", "Radius r");
			case ParameterType.MAJOR_AXIS_COEFFICIENT:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.axis.major", "Major axis coefficient a");
			case ParameterType.MIDDLE_AXIS_COEFFICIENT:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.axis.middle", "Middle axis coefficient b");
			case ParameterType.MINOR_AXIS_COEFFICIENT:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.axis.minor", "Minor axis coefficient c");
			case ParameterType.PRIMARY_FOCAL_COORDINATE_X:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.focal.primary.x", "Primary focal x1");
			case ParameterType.PRIMARY_FOCAL_COORDINATE_Y:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.focal.primary.y", "Primary focal y1");
			case ParameterType.PRIMARY_FOCAL_COORDINATE_Z:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.focal.primary.z", "Primary focal z1");
			case ParameterType.SECONDARY_FOCAL_COORDINATE_X:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.focal.secondary.x", "Secundary focal x2");
			case ParameterType.SECONDARY_FOCAL_COORDINATE_Y:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.focal.secondary.y", "Secundary focal y2");
			case ParameterType.SECONDARY_FOCAL_COORDINATE_Z:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.focal.secondary.z", "Secundary focal z2");
			case ParameterType.CONSTANT:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.constant", "Constant");
			case ParameterType.ROTATION_COMPONENT_R11:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.rotation.r11", "Rotation element r11");
			case ParameterType.ROTATION_COMPONENT_R12:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.rotation.r12", "Rotation element r12");
			case ParameterType.ROTATION_COMPONENT_R13:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.rotation.r13", "Rotation element r13");
			case ParameterType.ROTATION_COMPONENT_R21:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.rotation.r21", "Rotation element r21");
			case ParameterType.ROTATION_COMPONENT_R22:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.rotation.r22", "Rotation element r22");
			case ParameterType.ROTATION_COMPONENT_R23:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.rotation.r23", "Rotation element r23");
			case ParameterType.ROTATION_COMPONENT_R31:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.rotation.r31", "Rotation element r31");
			case ParameterType.ROTATION_COMPONENT_R32:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.rotation.r32", "Rotation element r32");
			case ParameterType.ROTATION_COMPONENT_R33:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.rotation.r33", "Rotation element r33");
			case ParameterType.POLYNOMIAL_COEFFICIENT_A:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.polynomial.a", "Polynomial coefficient a1");
			case ParameterType.POLYNOMIAL_COEFFICIENT_B:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.polynomial.b", "Polynomial coefficient a2");
			case ParameterType.POLYNOMIAL_COEFFICIENT_C:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.polynomial.c", "Polynomial coefficient a3");
			case ParameterType.POLYNOMIAL_COEFFICIENT_D:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.polynomial.d", "Polynomial coefficient a4");
			case ParameterType.POLYNOMIAL_COEFFICIENT_E:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.polynomial.e", "Polynomial coefficient a5");
			case ParameterType.POLYNOMIAL_COEFFICIENT_F:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.polynomial.f", "Polynomial coefficient a6");
			case ParameterType.POLYNOMIAL_COEFFICIENT_G:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.polynomial.g", "Polynomial coefficient a7");
			case ParameterType.POLYNOMIAL_COEFFICIENT_H:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.polynomial.h", "Polynomial coefficient a8");
			case ParameterType.POLYNOMIAL_COEFFICIENT_I:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.polynomial.i", "Polynomial coefficient a9");
			case ParameterType.ANGLE:
				return i18N.getString("UnknownParameterTypeDialog.parameter.type.angle", "Angle \u03C6");
			}
			return null;
		}
	}

}