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

	using FeatureType = org.applied_geodesy.adjustment.geometry.FeatureType;
	using GeometricPrimitive = org.applied_geodesy.adjustment.geometry.GeometricPrimitive;
	using PrimitiveType = org.applied_geodesy.adjustment.geometry.PrimitiveType;
	using Circle = org.applied_geodesy.adjustment.geometry.curve.primitive.Circle;
	using Ellipse = org.applied_geodesy.adjustment.geometry.curve.primitive.Ellipse;
	using Line = org.applied_geodesy.adjustment.geometry.curve.primitive.Line;
	using QuadraticCurve = org.applied_geodesy.adjustment.geometry.curve.primitive.QuadraticCurve;
	using Cone = org.applied_geodesy.adjustment.geometry.surface.primitive.Cone;
	using Cylinder = org.applied_geodesy.adjustment.geometry.surface.primitive.Cylinder;
	using Ellipsoid = org.applied_geodesy.adjustment.geometry.surface.primitive.Ellipsoid;
	using Paraboloid = org.applied_geodesy.adjustment.geometry.surface.primitive.Paraboloid;
	using Plane = org.applied_geodesy.adjustment.geometry.surface.primitive.Plane;
	using QuadraticSurface = org.applied_geodesy.adjustment.geometry.surface.primitive.QuadraticSurface;
	using Sphere = org.applied_geodesy.adjustment.geometry.surface.primitive.Sphere;
	using I18N = org.applied_geodesy.juniform.ui.i18n.I18N;

	using Platform = javafx.application.Platform;
	using FXCollections = javafx.collections.FXCollections;
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

	public class GeometricPrimitiveDialog
	{
		private static I18N i18N = I18N.Instance;
		private static GeometricPrimitiveDialog geometricPrimitiveDialog = new GeometricPrimitiveDialog();
		private Dialog<GeometricPrimitive> dialog = null;
		private ComboBox<PrimitiveType> primitiveTypeComboBox;
		private TextField nameTextField;
		private Window window;

		private GeometricPrimitiveDialog()
		{
		}

		public static Window Owner
		{
			set
			{
				geometricPrimitiveDialog.window = value;
			}
		}

		public static Optional<GeometricPrimitive> showAndWait(FeatureType featureType)
		{
			geometricPrimitiveDialog.init();
			geometricPrimitiveDialog.FeatureType = featureType;
			// @see https://bugs.openjdk.java.net/browse/JDK-8087458
			Platform.runLater(() =>
			{
			try
			{
				geometricPrimitiveDialog.dialog.getDialogPane().requestLayout();
				Stage stage = (Stage) geometricPrimitiveDialog.dialog.getDialogPane().getScene().getWindow();
				stage.sizeToScene();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			});
			return geometricPrimitiveDialog.dialog.showAndWait();
		}

		private void init()
		{
			if (this.dialog != null)
			{
				return;
			}

			this.dialog = new Dialog<GeometricPrimitive>();
			this.dialog.setTitle(i18N.getString("GeometricPrimitiveDialog.title", "Geometry"));
			this.dialog.setHeaderText(i18N.getString("GeometricPrimitiveDialog.header", "Geometric primitives"));
			this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.OK, ButtonType.CANCEL);
			this.dialog.initModality(Modality.APPLICATION_MODAL);
			this.dialog.initOwner(window);
			this.dialog.getDialogPane().setContent(this.createPane());
			this.dialog.setResizable(true);
			this.dialog.setResultConverter(new CallbackAnonymousInnerClass(this));
		}

		private class CallbackAnonymousInnerClass : Callback<ButtonType, GeometricPrimitive>
		{
			private readonly GeometricPrimitiveDialog outerInstance;

			public CallbackAnonymousInnerClass(GeometricPrimitiveDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override GeometricPrimitive call(ButtonType buttonType)
			{
				if (buttonType == ButtonType.OK)
				{
					PrimitiveType primitiveType = outerInstance.primitiveTypeComboBox.getValue();
					if (primitiveType != null)
					{
						GeometricPrimitive geometricPrimitive = outerInstance.getGeometricPrimitive(primitiveType);
						DefaultName = geometricPrimitive;

						if (outerInstance.nameTextField.getText() != null && !outerInstance.nameTextField.getText().isBlank())
						{
							geometricPrimitive.Name = outerInstance.nameTextField.getText().Trim();
						}

						return geometricPrimitive;
					}
				}
				return null;
			}
		}

		private GeometricPrimitive getGeometricPrimitive(PrimitiveType primitiveType)
		{
			switch (primitiveType.innerEnumValue)
			{
			case PrimitiveType.InnerEnum.LINE:
				return new Line();
			case PrimitiveType.InnerEnum.CIRCLE:
				return new Circle();
			case PrimitiveType.InnerEnum.ELLIPSE:
				return new Ellipse();
			case PrimitiveType.InnerEnum.QUADRATIC_CURVE:
				return new QuadraticCurve();
			case PrimitiveType.InnerEnum.PLANE:
				return new Plane();
			case PrimitiveType.InnerEnum.SPHERE:
				return new Sphere();
			case PrimitiveType.InnerEnum.ELLIPSOID:
				return new Ellipsoid();
			case PrimitiveType.InnerEnum.CYLINDER:
				return new Cylinder();
			case PrimitiveType.InnerEnum.CONE:
				return new Cone();
			case PrimitiveType.InnerEnum.PARABOLOID:
				return new Paraboloid();
			case PrimitiveType.InnerEnum.QUADRATIC_SURFACE:
				return new QuadraticSurface();
			}
			throw new System.ArgumentException("Error, unknown type of geometric primitive " + primitiveType);
		}

		private FeatureType FeatureType
		{
			set
			{
				this.primitiveTypeComboBox.setItems(FXCollections.observableArrayList(PrimitiveType.values(value)));
				this.primitiveTypeComboBox.getSelectionModel().clearAndSelect(0);
			}
		}

		private Node createPane()
		{
			GridPane gridPane = DialogUtil.createGridPane();
			Label nameLabel = new Label(i18N.getString("GeometricPrimitiveDialog.primitive.name.label", "Name:"));
			Label typeLabel = new Label(i18N.getString("GeometricPrimitiveDialog.primitive.type.label", "Type:"));

			this.nameTextField = DialogUtil.createTextField(i18N.getString("GeometricPrimitiveDialog.primitive.name.tooltip", "Name of geometric primitive"), i18N.getString("GeometricPrimitiveDialog.primitive.name.prompt", "Name of geometry"));
			this.primitiveTypeComboBox = DialogUtil.createPrimitiveTypeComboBox(createPrimitiveTypeStringConverter(), i18N.getString("GeometricPrimitiveDialog.primitive.type.tooltip", "Select geometric primitive type"));

			nameLabel.setLabelFor(this.nameTextField);
			typeLabel.setLabelFor(this.primitiveTypeComboBox);

			nameLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			typeLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			nameLabel.setMaxWidth(double.MaxValue);
			typeLabel.setMaxWidth(double.MaxValue);

			GridPane.setHgrow(nameLabel, Priority.NEVER);
			GridPane.setHgrow(typeLabel, Priority.NEVER);

			GridPane.setHgrow(this.nameTextField, Priority.ALWAYS);
			GridPane.setHgrow(this.primitiveTypeComboBox, Priority.ALWAYS);

			// https://stackoverflow.com/questions/50479384/gridpane-with-gaps-inside-scrollpane-rendering-wrong
			Insets insetsLeft = new Insets(5, 7, 5, 5);
			Insets insetsRight = new Insets(5, 0, 5, 7);

			GridPane.setMargin(nameLabel, insetsLeft);
			GridPane.setMargin(typeLabel, insetsLeft);

			GridPane.setMargin(this.nameTextField, insetsRight);
			GridPane.setMargin(this.primitiveTypeComboBox, insetsRight);

			gridPane.add(typeLabel, 0, 0); // column, row, columnspan, rowspan,
			gridPane.add(this.primitiveTypeComboBox, 1, 0);

			gridPane.add(nameLabel, 0, 1);
			gridPane.add(this.nameTextField, 1, 1);

			return gridPane;
		}

		internal static StringConverter<PrimitiveType> createPrimitiveTypeStringConverter()
		{
			return new StringConverterAnonymousInnerClass();
		}

		private class StringConverterAnonymousInnerClass : StringConverter<PrimitiveType>
		{
			public override string toString(PrimitiveType primitiveType)
			{
				return getPrimitiveTypeLabel(primitiveType);
			}

			public override PrimitiveType fromString(string @string)
			{
				return PrimitiveType.valueOf(@string);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static String getPrimitiveTypeLabel(org.applied_geodesy.adjustment.geometry.PrimitiveType primitiveType) throws IllegalArgumentException
		public static string getPrimitiveTypeLabel(PrimitiveType primitiveType)
		{
			if (primitiveType == null)
			{
				return null;
			}

			switch (primitiveType.innerEnumValue)
			{
			/// <summary>
			/// Curves </summary>
			case PrimitiveType.InnerEnum.LINE:
				return i18N.getString("GeometricPrimitiveDialog.primitive.type.curve.line.label", "Line");

			case PrimitiveType.InnerEnum.CIRCLE:
				return i18N.getString("GeometricPrimitiveDialog.primitive.type.curve.circle.label", "Circle");

			case PrimitiveType.InnerEnum.ELLIPSE:
				return i18N.getString("GeometricPrimitiveDialog.primitive.type.curve.ellipse.label", "Ellipse");

			case PrimitiveType.InnerEnum.QUADRATIC_CURVE:
				return i18N.getString("GeometricPrimitiveDialog.primitive.type.curve.quadratic.label", "Quadratic curve");

			/// <summary>
			/// Surfaces </summary>
			case PrimitiveType.InnerEnum.PLANE:
				return i18N.getString("GeometricPrimitiveDialog.primitive.type.surface.plane.label", "Plane");

			case PrimitiveType.InnerEnum.SPHERE:
				return i18N.getString("GeometricPrimitiveDialog.primitive.type.surface.sphere.label", "Sphere");

			case PrimitiveType.InnerEnum.ELLIPSOID:
				return i18N.getString("GeometricPrimitiveDialog.primitive.type.surface.ellipsoid.label", "Ellipsoid");

			case PrimitiveType.InnerEnum.CYLINDER:
				return i18N.getString("GeometricPrimitiveDialog.primitive.type.surface.cylinder.label", "Cylinder");

			case PrimitiveType.InnerEnum.CONE:
				return i18N.getString("GeometricPrimitiveDialog.primitive.type.surface.cone.label", "Cone");

			case PrimitiveType.InnerEnum.PARABOLOID:
				return i18N.getString("GeometricPrimitiveDialog.primitive.type.surface.paraboloid.label", "Paraboloid");

			case PrimitiveType.InnerEnum.QUADRATIC_SURFACE:
				return i18N.getString("GeometricPrimitiveDialog.primitive.type.surface.quadratic.label", "Quadratic surface");
			}

			throw new System.ArgumentException("Error, unknown type of geometric primitive " + primitiveType);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void setDefaultName(org.applied_geodesy.adjustment.geometry.GeometricPrimitive geometricPrimitive) throws IllegalArgumentException
		public static GeometricPrimitive DefaultName
		{
			set
			{
				string name = getPrimitiveTypeLabel(value.PrimitiveType);
				if (string.ReferenceEquals(name, null))
				{
					throw new System.NullReferenceException("Error, name of geometric primitive cannot be null!");
				}
    
				name = String.format(Locale.ENGLISH, name + " (id: %d)", value.Id);
				value.Name = name;
			}
		}
	}

}