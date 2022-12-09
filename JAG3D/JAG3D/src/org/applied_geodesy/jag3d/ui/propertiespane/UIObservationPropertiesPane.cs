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

	using Epoch = org.applied_geodesy.adjustment.network.Epoch;
	using ObservationGroupUncertaintyType = org.applied_geodesy.adjustment.network.ObservationGroupUncertaintyType;
	using ParameterType = org.applied_geodesy.adjustment.network.ParameterType;
	using DeltaZ = org.applied_geodesy.adjustment.network.observation.DeltaZ;
	using Direction = org.applied_geodesy.adjustment.network.observation.Direction;
	using GNSSBaselineDeltaX2D = org.applied_geodesy.adjustment.network.observation.GNSSBaselineDeltaX2D;
	using GNSSBaselineDeltaX3D = org.applied_geodesy.adjustment.network.observation.GNSSBaselineDeltaX3D;
	using GNSSBaselineDeltaZ1D = org.applied_geodesy.adjustment.network.observation.GNSSBaselineDeltaZ1D;
	using HorizontalDistance = org.applied_geodesy.adjustment.network.observation.HorizontalDistance;
	using Observation = org.applied_geodesy.adjustment.network.observation.Observation;
	using SlopeDistance = org.applied_geodesy.adjustment.network.observation.SlopeDistance;
	using ZenithAngle = org.applied_geodesy.adjustment.network.observation.ZenithAngle;
	using DeltaZGroup = org.applied_geodesy.adjustment.network.observation.group.DeltaZGroup;
	using DirectionGroup = org.applied_geodesy.adjustment.network.observation.group.DirectionGroup;
	using GNSSBaseline1DGroup = org.applied_geodesy.adjustment.network.observation.group.GNSSBaseline1DGroup;
	using GNSSBaseline2DGroup = org.applied_geodesy.adjustment.network.observation.group.GNSSBaseline2DGroup;
	using GNSSBaseline3DGroup = org.applied_geodesy.adjustment.network.observation.group.GNSSBaseline3DGroup;
	using HorizontalDistanceGroup = org.applied_geodesy.adjustment.network.observation.group.HorizontalDistanceGroup;
	using ObservationGroup = org.applied_geodesy.adjustment.network.observation.group.ObservationGroup;
	using SlopeDistanceGroup = org.applied_geodesy.adjustment.network.observation.group.SlopeDistanceGroup;
	using ZenithAngleGroup = org.applied_geodesy.adjustment.network.observation.group.ZenithAngleGroup;
	using Point = org.applied_geodesy.adjustment.network.point.Point;
	using Point3D = org.applied_geodesy.adjustment.network.point.Point3D;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using ObservationTreeItemValue = org.applied_geodesy.jag3d.ui.tree.ObservationTreeItemValue;
	using TreeItemType = org.applied_geodesy.jag3d.ui.tree.TreeItemType;
	using UITreeBuilder = org.applied_geodesy.jag3d.ui.tree.UITreeBuilder;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
	using DoubleTextField = org.applied_geodesy.ui.textfield.DoubleTextField;
	using UncertaintyTextField = org.applied_geodesy.ui.textfield.UncertaintyTextField;
	using ValueSupport = org.applied_geodesy.ui.textfield.DoubleTextField.ValueSupport;
	using CellValueType = org.applied_geodesy.util.CellValueType;
	using FormatterChangedListener = org.applied_geodesy.util.FormatterChangedListener;
	using FormatterEvent = org.applied_geodesy.util.FormatterEvent;
	using FormatterOptions = org.applied_geodesy.util.FormatterOptions;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;

	using FadeTransition = javafx.animation.FadeTransition;
	using SequentialTransition = javafx.animation.SequentialTransition;
	using Platform = javafx.application.Platform;
	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using ActionEvent = javafx.@event.ActionEvent;
	using EventHandler = javafx.@event.EventHandler;
	using Insets = javafx.geometry.Insets;
	using Node = javafx.scene.Node;
	using LineChart = javafx.scene.chart.LineChart;
	using NumberAxis = javafx.scene.chart.NumberAxis;
	using XYChart = javafx.scene.chart.XYChart;
	using ButtonBase = javafx.scene.control.ButtonBase;
	using CheckBox = javafx.scene.control.CheckBox;
	using Control = javafx.scene.control.Control;
	using Label = javafx.scene.control.Label;
	using ProgressIndicator = javafx.scene.control.ProgressIndicator;
	using RadioButton = javafx.scene.control.RadioButton;
	using ScrollPane = javafx.scene.control.ScrollPane;
	using TitledPane = javafx.scene.control.TitledPane;
	using ToggleGroup = javafx.scene.control.ToggleGroup;
	using Tooltip = javafx.scene.control.Tooltip;
	using GridPane = javafx.scene.layout.GridPane;
	using HBox = javafx.scene.layout.HBox;
	using Priority = javafx.scene.layout.Priority;
	using Region = javafx.scene.layout.Region;
	using VBox = javafx.scene.layout.VBox;
	using Color = javafx.scene.paint.Color;
	using Font = javafx.scene.text.Font;
	using Duration = javafx.util.Duration;
	using StringConverter = javafx.util.StringConverter;

	public class UIObservationPropertiesPane
	{
		private class TickFormatChangedListener : FormatterChangedListener
		{
			private readonly UIObservationPropertiesPane outerInstance;

			public TickFormatChangedListener(UIObservationPropertiesPane outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public virtual void formatterChanged(FormatterEvent evt)
			{
				outerInstance.updateUncertaintyChart(outerInstance.lineChart);
				outerInstance.updateTickLabels(outerInstance.lineChart);
			}
		}

		private class NumberChangeListener : ChangeListener<double>
		{
			private readonly UIObservationPropertiesPane outerInstance;

			internal readonly DoubleTextField field;

			internal NumberChangeListener(UIObservationPropertiesPane outerInstance, DoubleTextField field)
			{
				this.outerInstance = outerInstance;
				this.field = field;
			}

			public override void changed<T1>(ObservableValue<T1> observable, double? oldValue, double? newValue) where T1 : double
			{
				if (!outerInstance.ignoreValueUpdate && this.field.getUserData() != null)
				{
					if (this.field.getUserData() is ParameterType)
					{
						ParameterType paramType = (ParameterType)this.field.getUserData();
						outerInstance.save(paramType);
					}
					else if (this.field.getUserData() is ObservationGroupUncertaintyType)
					{
						ObservationGroupUncertaintyType uncertaintyType = (ObservationGroupUncertaintyType)this.field.getUserData();
						outerInstance.save(uncertaintyType);
					}
				}
			}
		}

		private class BooleanChangeListener : ChangeListener<bool>
		{
			private readonly UIObservationPropertiesPane outerInstance;

			internal readonly ButtonBase button;

			internal BooleanChangeListener(UIObservationPropertiesPane outerInstance, ButtonBase button)
			{
				this.outerInstance = outerInstance;
				this.button = button;
			}

			public override void changed<T1>(ObservableValue<T1> observable, bool? oldValue, bool? newValue) where T1 : bool
			{
				if (!outerInstance.ignoreValueUpdate && this.button.getUserData() != null)
				{
					if (this.button == outerInstance.referenceEpochRadioButton)
					{
						outerInstance.save();
					}
					else if (this.button.getUserData() is ParameterType)
					{
						ParameterType paramType = (ParameterType)this.button.getUserData();
						outerInstance.save(paramType);
					}
				}
			}
		}

		private class SequentialTransitionFinishedListener : ChangeListener<EventHandler<ActionEvent>>
		{
			private readonly UIObservationPropertiesPane outerInstance;

			public SequentialTransitionFinishedListener(UIObservationPropertiesPane outerInstance)
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

		private DoubleTextField zeroPointOffsetField;
		private CheckBox zeroPointOffsetCheckBox;

		private DoubleTextField scaleField;
		private CheckBox scaleCheckBox;

		private DoubleTextField rotationXField;
		private CheckBox rotationXCheckBox;

		private DoubleTextField rotationYField;
		private CheckBox rotationYCheckBox;

		private DoubleTextField rotationZField;
		private CheckBox rotationZCheckBox;

		private DoubleTextField orientationOffsetField;
		private CheckBox orientationOffsetCheckBox;

		private DoubleTextField refractionIndexField;
		private CheckBox refractionIndexCheckBox;

		private UncertaintyTextField zeroPointOffsetUncertaintyField;
		private UncertaintyTextField squareRootDistanceDependentUncertaintyField;
		private UncertaintyTextField distanceDependentUncertaintyField;

		private Label selectionInfoLabel = new Label();

		private RadioButton referenceEpochRadioButton;
		private RadioButton controlEpochRadioButton;

		private IDictionary<object, ProgressIndicator> databaseTransactionProgressIndicators = new Dictionary<object, ProgressIndicator>(10);
		private IDictionary<object, Node> warningIconNodes = new Dictionary<object, Node>(10);
		private SequentialTransition sequentialTransition = new SequentialTransition();

		private bool ignoreValueUpdate = false;
		private ObservationTreeItemValue[] selectedObservationItemValues = null;

		private LineChart<Number, Number> lineChart;
		private readonly double minDistanceForUncertaintyChart = 5.0;
		private readonly double maxDistanceForUncertaintyChart = 150.0;

		private FormatterOptions options = FormatterOptions.Instance;

		internal UIObservationPropertiesPane(TreeItemType type)
		{
			switch (type.innerEnumValue)
			{
			case TreeItemType.InnerEnum.LEVELING_LEAF:
			case TreeItemType.InnerEnum.DIRECTION_LEAF:
			case TreeItemType.InnerEnum.HORIZONTAL_DISTANCE_LEAF:
			case TreeItemType.InnerEnum.SLOPE_DISTANCE_LEAF:
			case TreeItemType.InnerEnum.ZENITH_ANGLE_LEAF:
			case TreeItemType.InnerEnum.GNSS_1D_LEAF:
			case TreeItemType.InnerEnum.GNSS_2D_LEAF:
			case TreeItemType.InnerEnum.GNSS_3D_LEAF:
				this.type = type;
				this.init();
				break;
			default:
				throw new System.ArgumentException(this.GetType().Name + " Error, unsupported item type " + type);
			}
		}

		public virtual void setTreeItemValue(string name, params ObservationTreeItemValue[] selectedObservationItemValues)
		{
			if (this.selectedObservationItemValues != selectedObservationItemValues)
			{
				this.reset();
				this.selectedObservationItemValues = selectedObservationItemValues;
			}
			this.setGroupName(name, this.selectedObservationItemValues != null ? this.selectedObservationItemValues.Length : 0);
		}

		public virtual Node Node
		{
			get
			{
				return this.propertiesNode;
			}
		}

		private void setGroupName(string name, int cnt)
		{
			if (this.selectionInfoLabel != null)
			{
				string groupNameTmpl = this.i18n.getString("UIObservationPropertiesPane.status.selection.name.label", "Status:");
				string selectionCntTmpl = cnt > 1 ? String.format(Locale.ENGLISH, this.i18n.getString("UIObservationPropertiesPane.status.selection.counter.label", "and %d more selected group(s)\u2026"), cnt) : "";
				string label = String.format(Locale.ENGLISH, "%s %s %s", groupNameTmpl, name, selectionCntTmpl);
				if (!this.selectionInfoLabel.getText().Equals(label))
				{
					this.selectionInfoLabel.setText(label);
				}
			}
		}

		private void reset()
		{
			this.sequentialTransition.stop();
			this.ProgressIndicatorsVisible = false;
			this.WarningIconsVisible = false;

			// set focus to panel to commit text field values and to force db transaction
			UITreeBuilder.Instance.Tree.requestFocus();

			double offset = 0.0;
			double scale = 1.0;
			double orientation = 0.0;
			double refraction = 0.0;
			double rotationY = 0.0;
			double rotationX = 0.0;
			double rotationZ = 0.0;

			this.ZeroPointOffsetUncertainty = ObservationTreeItemValue.getDefaultUncertainty(this.type, ObservationGroupUncertaintyType.ZERO_POINT_OFFSET);
			this.SquareRootDistanceDependentUncertainty = ObservationTreeItemValue.getDefaultUncertainty(this.type, ObservationGroupUncertaintyType.SQUARE_ROOT_DISTANCE_DEPENDENT);
			this.DistanceDependentUncertainty = ObservationTreeItemValue.getDefaultUncertainty(this.type, ObservationGroupUncertaintyType.DISTANCE_DEPENDENT);

			this.ReferenceEpoch = true;

			this.setZeroPointOffset(offset, false);
			this.setScale(scale, false);
			this.setOrientation(orientation, true);
			this.setRotationX(rotationX, false);
			this.setRotationY(rotationY, false);
			this.setRotationZ(rotationZ, false);
			this.setRefractionIndex(refraction, false);
		}

		public virtual bool setZeroPointOffsetUncertainty(double? value)
		{
			if (this.zeroPointOffsetUncertaintyField == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.zeroPointOffsetUncertaintyField.Value = value != null && value > 0 ? value : null;
			//this.zeroPointOffsetUncertaintyField.setStyle("-fx-border-color: red; -fx-border-width: 2px;");
			this.updateUncertaintyChart(this.lineChart);
			this.updateTickLabels(this.lineChart);
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setSquareRootDistanceDependentUncertainty(double? value)
		{
			if (this.squareRootDistanceDependentUncertaintyField == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.squareRootDistanceDependentUncertaintyField.Value = value != null && value >= 0 ? value : null;
			this.updateUncertaintyChart(this.lineChart);
			this.updateTickLabels(this.lineChart);
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setDistanceDependentUncertainty(double? value)
		{
			if (this.distanceDependentUncertaintyField == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.distanceDependentUncertaintyField.Value = value != null && value >= 0 ? value : null;
			this.updateUncertaintyChart(this.lineChart);
			this.updateTickLabels(this.lineChart);
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setReferenceEpoch(bool? referenceEpoch)
		{
			if (this.referenceEpochRadioButton == null || this.controlEpochRadioButton == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.referenceEpochRadioButton.setSelected(referenceEpoch != null && referenceEpoch == true);
			this.controlEpochRadioButton.setSelected(referenceEpoch == null || referenceEpoch == false);
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setUncertainty(ObservationGroupUncertaintyType type, double? value, bool displayWarningIcon)
		{
			if (this.warningIconNodes.ContainsKey(type))
			{
				this.warningIconNodes[type].setVisible(displayWarningIcon);
				this.warningIconNodes[type].setManaged(displayWarningIcon);
			}

			switch (type.innerEnumValue)
			{
			case ObservationGroupUncertaintyType.InnerEnum.ZERO_POINT_OFFSET:
				return this.setZeroPointOffsetUncertainty(value);
			case ObservationGroupUncertaintyType.InnerEnum.DISTANCE_DEPENDENT:
				return this.setDistanceDependentUncertainty(value);
			case ObservationGroupUncertaintyType.InnerEnum.SQUARE_ROOT_DISTANCE_DEPENDENT:
				return this.setSquareRootDistanceDependentUncertainty(value);
			default:
				return false;
			}
		}

		public virtual bool setAdditionalParameter(ParameterType paramType, double? value, bool? enable, bool displayWarningIcon)
		{
			if (this.warningIconNodes.ContainsKey(paramType))
			{
				this.warningIconNodes[paramType].setVisible(displayWarningIcon);
				this.warningIconNodes[paramType].setManaged(displayWarningIcon);
			}

			switch (paramType.innerEnumValue)
			{
			case ParameterType.InnerEnum.ZERO_POINT_OFFSET:
				return this.setZeroPointOffset(value, enable);
			case ParameterType.InnerEnum.SCALE:
				return this.setScale(value, enable);
			case ParameterType.InnerEnum.ORIENTATION:
				return this.setOrientation(value, enable);
			case ParameterType.InnerEnum.ROTATION_X:
				return this.setRotationX(value, enable);
			case ParameterType.InnerEnum.ROTATION_Y:
				return this.setRotationY(value, enable);
			case ParameterType.InnerEnum.ROTATION_Z:
				return this.setRotationZ(value, enable);
			case ParameterType.InnerEnum.REFRACTION_INDEX:
				return this.setRefractionIndex(value, enable);
			default:
				return false;
			}
		}

		public virtual bool setZeroPointOffset(double? value, bool? enable)
		{
			if (this.zeroPointOffsetCheckBox == null || this.zeroPointOffsetField == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.zeroPointOffsetCheckBox.setSelected(enable != null && enable == true);
			this.zeroPointOffsetField.Value = value;
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setScale(double? value, bool? enable)
		{
			if (this.scaleCheckBox == null || this.scaleField == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.scaleCheckBox.setSelected(enable != null && enable == true);
			this.scaleField.Value = value;
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setRotationX(double? value, bool? enable)
		{
			if (this.rotationXCheckBox == null || this.rotationXField == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.rotationXCheckBox.setSelected(enable != null && enable == true);
			this.rotationXField.Value = value;
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setRotationY(double? value, bool? enable)
		{
			if (this.rotationYCheckBox == null || this.rotationYField == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.rotationYCheckBox.setSelected(enable != null && enable == true);
			this.rotationYField.Value = value;
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setRotationZ(double? value, bool? enable)
		{
			if (this.rotationZCheckBox == null || this.rotationZField == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.rotationZCheckBox.setSelected(enable != null && enable == true);
			this.rotationZField.Value = value;
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setOrientation(double? value, bool? enable)
		{
			if (this.orientationOffsetCheckBox == null || this.orientationOffsetField == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.orientationOffsetCheckBox.setSelected(enable != null && enable == true);
			this.orientationOffsetField.Value = value;
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setRefractionIndex(double? value, bool? enable)
		{
			if (this.refractionIndexCheckBox == null || this.refractionIndexField == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.refractionIndexCheckBox.setSelected(enable != null && enable == true);
			this.refractionIndexField.Value = value;
			this.ignoreValueUpdate = false;
			return true;
		}

		private Node createAdditionalParametersPane()
		{
			GridPane gridPane = this.createGridPane();

			double offset = 0.0;
			double scale = 1.0;
			double orientation = 0.0;
			double refraction = 0.0;
			double rotationY = 0.0;
			double rotationX = 0.0;
			double rotationZ = 0.0;

			int row = 0;
			ParameterType[] paramTypes = ObservationTreeItemValue.getParameterTypes(this.type);
			foreach (ParameterType paramType in paramTypes)
			{
				CheckBox box = null;
				DoubleTextField field = null;
				ProgressIndicator progressIndicator = null;
				Node warningIcon = null;

				switch (paramType.innerEnumValue)
				{

				case ParameterType.InnerEnum.ORIENTATION:

					box = this.orientationOffsetCheckBox = this.createCheckBox(i18n.getString("UIObservationPropertiesPane.additionalparameter.orientation.label", "Orientation o"), i18n.getString("UIObservationPropertiesPane.additionalparameter.orientation.label.tooltip", "Checked, if orientation is an unknown parameter to be estimated"), false, paramType);
					field = this.orientationOffsetField = this.createDoubleTextField(orientation, CellValueType.ANGLE_RESIDUAL, true, DoubleTextField.ValueSupport.NON_NULL_VALUE_SUPPORT, i18n.getString("UIObservationPropertiesPane.additionalparameter.orientation.tooltip", "Set orientation offset"), paramType);
					progressIndicator = this.createDatabaseTransactionProgressIndicator(paramType);
					warningIcon = this.createWarningIcon(paramType, i18n.getString("UIObservationPropertiesPane.additionalparameter.orientation.warning.label", "\u26A0"), String.format(Locale.ENGLISH, i18n.getString("UIObservationPropertiesPane.additionalparameter.orientation.warning.tooltip", "Note: The selected groups have different values and the orientation differs by more than %.1f \u2030."), SQLManager.EQUAL_VALUE_TRESHOLD * 1000.0));

					break;

				case ParameterType.InnerEnum.REFRACTION_INDEX:

					box = this.refractionIndexCheckBox = this.createCheckBox(i18n.getString("UIObservationPropertiesPane.additionalparameter.refraction.label", "Refraction index k"), i18n.getString("UIObservationPropertiesPane.additionalparameter.refraction.label.tooltip", "Checked, if refraction index is an unknown parameter to be estimated"), true, paramType);
					field = this.refractionIndexField = this.createDoubleTextField(refraction, CellValueType.STATISTIC, false, DoubleTextField.ValueSupport.NON_NULL_VALUE_SUPPORT, i18n.getString("UIObservationPropertiesPane.additionalparameter.refraction.tooltip", "Set refraction index offset"), paramType);
					progressIndicator = this.createDatabaseTransactionProgressIndicator(paramType);
					warningIcon = this.createWarningIcon(paramType, i18n.getString("UIObservationPropertiesPane.additionalparameter.refraction.warning.label", "\u26A0"), String.format(Locale.ENGLISH, i18n.getString("UIObservationPropertiesPane.additionalparameter.refraction.warning.tooltip", "Note: The selected groups have different values and the refraction index differs by more than %.1f \u2030."), SQLManager.EQUAL_VALUE_TRESHOLD * 1000.0));

					break;

				case ParameterType.InnerEnum.ROTATION_Y:

					box = this.rotationYCheckBox = this.createCheckBox(i18n.getString("UIObservationPropertiesPane.additionalparameter.rotation.y.label", "Rotation angle ry"), i18n.getString("UIObservationPropertiesPane.additionalparameter.rotation.y.label.tooltip", "Checked, if rotation angle around y-axis is an unknown parameter to be estimated"), true, paramType);
					field = this.rotationYField = this.createDoubleTextField(rotationY, CellValueType.ANGLE_RESIDUAL, true, DoubleTextField.ValueSupport.NON_NULL_VALUE_SUPPORT, i18n.getString("UIObservationPropertiesPane.additionalparameter.rotation.y.tooltip", "Set rotation angle around y-axis"), paramType);
					progressIndicator = this.createDatabaseTransactionProgressIndicator(paramType);
					warningIcon = this.createWarningIcon(paramType, i18n.getString("UIObservationPropertiesPane.additionalparameter.rotation.y.warning.label", "\u26A0"), String.format(Locale.ENGLISH, i18n.getString("UIObservationPropertiesPane.additionalparameter.rotation.y.warning.tooltip", "Note: The selected groups have different values and the rotation angle around y-axis differs by more than %.1f \u2030."), SQLManager.EQUAL_VALUE_TRESHOLD * 1000.0));

					break;

				case ParameterType.InnerEnum.ROTATION_X:

					box = this.rotationXCheckBox = this.createCheckBox(i18n.getString("UIObservationPropertiesPane.additionalparameter.rotation.x.label", "Rotation angle rx"), i18n.getString("UIObservationPropertiesPane.additionalparameter.rotation.x.label.tooltip", "Checked, if rotation angle around x-axis is an unknown parameter to be estimated"), true, paramType);
					field = this.rotationXField = this.createDoubleTextField(rotationX, CellValueType.ANGLE_RESIDUAL, true, DoubleTextField.ValueSupport.NON_NULL_VALUE_SUPPORT, i18n.getString("UIObservationPropertiesPane.additionalparameter.rotation.x.tooltip", "Set rotation angle around x-axis"), paramType);
					progressIndicator = this.createDatabaseTransactionProgressIndicator(paramType);
					warningIcon = this.createWarningIcon(paramType, i18n.getString("UIObservationPropertiesPane.additionalparameter.rotation.x.warning.label", "\u26A0"), String.format(Locale.ENGLISH, i18n.getString("UIObservationPropertiesPane.additionalparameter.rotation.x.warning.tooltip", "Note: The selected groups have different values and the rotation angle around x-axis differs by more than %.1f \u2030."), SQLManager.EQUAL_VALUE_TRESHOLD * 1000.0));

					break;

				case ParameterType.InnerEnum.ROTATION_Z:

					box = this.rotationZCheckBox = this.createCheckBox(i18n.getString("UIObservationPropertiesPane.additionalparameter.rotation.z.label", "Rotation angle rz"), i18n.getString("UIObservationPropertiesPane.additionalparameter.rotation.z.label.tooltip", "Checked, if rotation angle around z-axis is an unknown parameter to be estimated"), true, paramType);
					field = this.rotationZField = this.createDoubleTextField(rotationZ, CellValueType.ANGLE_RESIDUAL, true, DoubleTextField.ValueSupport.NON_NULL_VALUE_SUPPORT, i18n.getString("UIObservationPropertiesPane.additionalparameter.rotation.z.tooltip", "Set rotation angle around z-axis"), paramType);
					progressIndicator = this.createDatabaseTransactionProgressIndicator(paramType);
					warningIcon = this.createWarningIcon(paramType, i18n.getString("UIObservationPropertiesPane.additionalparameter.rotation.z.warning.label", "\u26A0"), String.format(Locale.ENGLISH, i18n.getString("UIObservationPropertiesPane.additionalparameter.rotation.z.warning.tooltip", "Note: The selected groups have different values and the rotation angle around z-axis differs by more than %.1f \u2030."), SQLManager.EQUAL_VALUE_TRESHOLD * 1000.0));

					break;
				case ParameterType.InnerEnum.SCALE:

					box = this.scaleCheckBox = this.createCheckBox(i18n.getString("UIObservationPropertiesPane.additionalparameter.scale.label", "Scale s"), i18n.getString("UIObservationPropertiesPane.additionalparameter.scale.label.tooltip", "Checked, if scale is an unknown parameter to be estimated"), true, paramType);
					field = this.scaleField = this.createDoubleTextField(scale, CellValueType.SCALE, true, DoubleTextField.ValueSupport.NON_NULL_VALUE_SUPPORT, i18n.getString("UIObservationPropertiesPane.additionalparameter.scale.tooltip", "Set scale"), paramType);
					progressIndicator = this.createDatabaseTransactionProgressIndicator(paramType);
					warningIcon = this.createWarningIcon(paramType, i18n.getString("UIObservationPropertiesPane.additionalparameter.scale.warning.label", "\u26A0"), String.format(Locale.ENGLISH, i18n.getString("UIObservationPropertiesPane.additionalparameter.scale.warning.tooltip", "Note: The selected groups have different values and the scale differs by more than %.1f \u2030."), SQLManager.EQUAL_VALUE_TRESHOLD * 1000.0));

					break;

				case ParameterType.InnerEnum.ZERO_POINT_OFFSET:

					box = this.zeroPointOffsetCheckBox = this.createCheckBox(i18n.getString("UIObservationPropertiesPane.additionalparameter.zero_point_offset.label", "Offset a"), i18n.getString("UIObservationPropertiesPane.additionalparameter.zero_point_offset.label.tooltip", "Checked, if zero point offset is an unknown parameter to be estimated"), true, paramType);
					field = this.zeroPointOffsetField = this.createDoubleTextField(offset, CellValueType.LENGTH_RESIDUAL, true, DoubleTextField.ValueSupport.NON_NULL_VALUE_SUPPORT, i18n.getString("UIObservationPropertiesPane.additionalparameter.zero_point_offset.tooltip", "Set zero point offset"), paramType);
					progressIndicator = this.createDatabaseTransactionProgressIndicator(paramType);
					warningIcon = this.createWarningIcon(paramType, i18n.getString("UIObservationPropertiesPane.additionalparameter.zero_point_offset.warning.label", "\u26A0"), String.format(Locale.ENGLISH, i18n.getString("UIObservationPropertiesPane.additionalparameter.zero_point_offset.warning.tooltip", "Note: The selected groups have different values and the zero point offset differs by more than %.1f \u2030."), SQLManager.EQUAL_VALUE_TRESHOLD * 1000.0));

					break;

				default:
					break;

				}

				if (field != null && box != null && progressIndicator != null && warningIcon != null)
				{
					gridPane.add(box, 0, row);
					gridPane.add(field, 1, row);
					gridPane.add(new HBox(warningIcon, progressIndicator), 2, row++);
				}
			}

			TitledPane additionalParametersTitledPane = this.createTitledPane(i18n.getString("UIObservationPropertiesPane.additionalparameter.title", "Additional parameters"));
			additionalParametersTitledPane.setContent(gridPane);
			return additionalParametersTitledPane;
		}

		private Node createUncertaintiesPane()
		{
			CellValueType constantUncertaintyCellValueType = null;
			CellValueType squareRootDistanceDependentUncertaintyCellValueType = CellValueType.LENGTH_UNCERTAINTY;
			CellValueType distanceDependentUncertaintyCellValueType = null;

			double sigmaZeroPointOffset = ObservationTreeItemValue.getDefaultUncertainty(this.type, ObservationGroupUncertaintyType.ZERO_POINT_OFFSET);
			double sigmaSquareRootDistance = ObservationTreeItemValue.getDefaultUncertainty(this.type, ObservationGroupUncertaintyType.SQUARE_ROOT_DISTANCE_DEPENDENT);
			double sigmaDistanceDependent = ObservationTreeItemValue.getDefaultUncertainty(this.type, ObservationGroupUncertaintyType.DISTANCE_DEPENDENT);

			switch (this.type.innerEnumValue)
			{
			case TreeItemType.InnerEnum.LEVELING_LEAF:
				constantUncertaintyCellValueType = CellValueType.LENGTH_UNCERTAINTY;
				distanceDependentUncertaintyCellValueType = CellValueType.SCALE_UNCERTAINTY;

				break;

			case TreeItemType.InnerEnum.HORIZONTAL_DISTANCE_LEAF:
			case TreeItemType.InnerEnum.SLOPE_DISTANCE_LEAF:
				constantUncertaintyCellValueType = CellValueType.LENGTH_UNCERTAINTY;
				distanceDependentUncertaintyCellValueType = CellValueType.SCALE_UNCERTAINTY;

				break;

			case TreeItemType.InnerEnum.DIRECTION_LEAF:
			case TreeItemType.InnerEnum.ZENITH_ANGLE_LEAF:
				constantUncertaintyCellValueType = CellValueType.ANGLE_UNCERTAINTY;
				distanceDependentUncertaintyCellValueType = CellValueType.LENGTH_UNCERTAINTY;

				break;

			case TreeItemType.InnerEnum.GNSS_1D_LEAF:
			case TreeItemType.InnerEnum.GNSS_2D_LEAF:
			case TreeItemType.InnerEnum.GNSS_3D_LEAF:
				constantUncertaintyCellValueType = CellValueType.LENGTH_UNCERTAINTY;
				distanceDependentUncertaintyCellValueType = CellValueType.SCALE_UNCERTAINTY;

				break;

			default:
				Console.Error.WriteLine(this.GetType().Name + " : Error, unsupported node type " + type);
				return null;
			}

			GridPane gridPane = this.createGridPane();

			Node warningIconUncertaintyTypeANode = this.createWarningIcon(ObservationGroupUncertaintyType.ZERO_POINT_OFFSET, i18n.getString("UIObservationPropertiesPane.uncertainty.ua.warning.label", "\u26A0"), String.format(Locale.ENGLISH, i18n.getString("UIObservationPropertiesPane.uncertainty.ua.warning.tooltip", "Note: The selected groups have different values and \u03C3a differs by more than %.1f \u2030."), SQLManager.EQUAL_VALUE_TRESHOLD * 1000.0));
			ProgressIndicator databaseTransactionUncertaintyTypeAProgressIndicator = this.createDatabaseTransactionProgressIndicator(ObservationGroupUncertaintyType.ZERO_POINT_OFFSET);
			Label uncertaintyTypeALabel = new Label(i18n.getString("UIObservationPropertiesPane.uncertainty.ua.label", "\u03C3a"));
			uncertaintyTypeALabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			this.zeroPointOffsetUncertaintyField = this.createUncertaintyTextField(sigmaZeroPointOffset, constantUncertaintyCellValueType, true, DoubleTextField.ValueSupport.EXCLUDING_INCLUDING_INTERVAL, i18n.getString("UIObservationPropertiesPane.uncertainty.ua.tooltip", "Set constant part of combined uncertainty"), ObservationGroupUncertaintyType.ZERO_POINT_OFFSET);

			Node warningIconUncertaintyTypeBNode = this.createWarningIcon(ObservationGroupUncertaintyType.SQUARE_ROOT_DISTANCE_DEPENDENT, i18n.getString("UIObservationPropertiesPane.uncertainty.ub.warning.label", "\u26A0"), String.format(Locale.ENGLISH, i18n.getString("UIObservationPropertiesPane.uncertainty.ub.warning.tooltip", "Note: The selected groups have different values and \u03C3b differs by more than %.1f \u2030."), SQLManager.EQUAL_VALUE_TRESHOLD * 1000.0));
			ProgressIndicator databaseTransactionUncertaintyTypeBProgressIndicator = this.createDatabaseTransactionProgressIndicator(ObservationGroupUncertaintyType.SQUARE_ROOT_DISTANCE_DEPENDENT);
			Label uncertaintyTypeBLabel = new Label(i18n.getString("UIObservationPropertiesPane.uncertainty.ub.label", "\u03C3b(\u221Ad)"));
			uncertaintyTypeBLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			this.squareRootDistanceDependentUncertaintyField = this.createUncertaintyTextField(sigmaSquareRootDistance, squareRootDistanceDependentUncertaintyCellValueType, true, DoubleTextField.ValueSupport.INCLUDING_INCLUDING_INTERVAL, i18n.getString("UIObservationPropertiesPane.uncertainty.ub.tooltip", "Set square-root distance dependent part of combined uncertainty"), ObservationGroupUncertaintyType.SQUARE_ROOT_DISTANCE_DEPENDENT);

			Node warningIconUncertaintyTypeCNode = this.createWarningIcon(ObservationGroupUncertaintyType.DISTANCE_DEPENDENT, i18n.getString("UIObservationPropertiesPane.uncertainty.uc.warning.label", "\u26A0"), String.format(Locale.ENGLISH, i18n.getString("UIObservationPropertiesPane.uncertainty.uc.warning.tooltip", "Note: The selected groups have different values and \u03C3c differs by more than %.1f \u2030."), SQLManager.EQUAL_VALUE_TRESHOLD * 1000.0));
			ProgressIndicator databaseTransactionUncertaintyTypeCProgressIndicator = this.createDatabaseTransactionProgressIndicator(ObservationGroupUncertaintyType.DISTANCE_DEPENDENT);
			Label uncertaintyTypeCLabel = new Label(i18n.getString("UIObservationPropertiesPane.uncertainty.uc.label", "\u03C3c(d)"));
			uncertaintyTypeCLabel.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			this.distanceDependentUncertaintyField = this.createUncertaintyTextField(sigmaDistanceDependent, distanceDependentUncertaintyCellValueType, true, DoubleTextField.ValueSupport.INCLUDING_INCLUDING_INTERVAL, i18n.getString("UIObservationPropertiesPane.uncertainty.uc.tooltip", "Set distance dependent part of combined uncertainty"), ObservationGroupUncertaintyType.DISTANCE_DEPENDENT);

			uncertaintyTypeALabel.setLabelFor(this.zeroPointOffsetUncertaintyField);
			uncertaintyTypeBLabel.setLabelFor(this.squareRootDistanceDependentUncertaintyField);
			uncertaintyTypeCLabel.setLabelFor(this.distanceDependentUncertaintyField);

	//		GridPane.setHgrow(uncertaintyTypeALabel, Priority.SOMETIMES);
	//		GridPane.setHgrow(this.zeroPointOffsetUncertaintyField, Priority.ALWAYS);
	//		GridPane.setHgrow(databaseTransactionUncertaintyTypeAProgressIndicator, Priority.NEVER);
	//		
	//		GridPane.setHgrow(uncertaintyTypeBLabel, Priority.SOMETIMES);
	//		GridPane.setHgrow(this.squareRootDistanceDependentUncertaintyField, Priority.ALWAYS);
	//		GridPane.setHgrow(databaseTransactionUncertaintyTypeBProgressIndicator, Priority.NEVER);
	//		
	//		GridPane.setHgrow(uncertaintyTypeCLabel, Priority.SOMETIMES);
	//		GridPane.setHgrow(this.distanceDependentUncertaintyField, Priority.ALWAYS);
	//		GridPane.setHgrow(databaseTransactionUncertaintyTypeCProgressIndicator, Priority.NEVER);

			gridPane.add(uncertaintyTypeALabel, 0, 0);
			gridPane.add(this.zeroPointOffsetUncertaintyField, 1, 0);
			gridPane.add(new HBox(warningIconUncertaintyTypeANode, databaseTransactionUncertaintyTypeAProgressIndicator), 2, 0);

			gridPane.add(uncertaintyTypeBLabel, 0, 1);
			gridPane.add(this.squareRootDistanceDependentUncertaintyField, 1, 1);
			gridPane.add(new HBox(warningIconUncertaintyTypeBNode, databaseTransactionUncertaintyTypeBProgressIndicator), 2, 1);

			gridPane.add(uncertaintyTypeCLabel, 0, 2);
			gridPane.add(this.distanceDependentUncertaintyField, 1, 2);
			gridPane.add(new HBox(warningIconUncertaintyTypeCNode, databaseTransactionUncertaintyTypeCProgressIndicator), 2, 2);

			TitledPane uncertaintiesTitledPane = this.createTitledPane(i18n.getString("UIObservationPropertiesPane.uncertainty.title", "Uncertainties"));
			uncertaintiesTitledPane.setContent(gridPane);
			return uncertaintiesTitledPane;
		}

		private Node createCongruenceAnalysisPane()
		{
			GridPane gridPane = this.createGridPane();

			ProgressIndicator databaseTransactionReferenceEpochProgressIndicator = this.createDatabaseTransactionProgressIndicator(Epoch.REFERENCE);
			ProgressIndicator databaseTransactionControlEpochProgressIndicator = this.createDatabaseTransactionProgressIndicator(Epoch.CONTROL);

			ToggleGroup group = new ToggleGroup();

			this.referenceEpochRadioButton = this.createRadioButton(i18n.getString("UIObservationPropertiesPane.congruenceanalysis.referenceepoch.label", "Reference epoch"), i18n.getString("UIObservationPropertiesPane.congruenceanalysis.referenceepoch.tooltip", "Selected, if group is referred to reference epoch"), group, true, Epoch.REFERENCE);

			this.controlEpochRadioButton = this.createRadioButton(i18n.getString("UIObservationPropertiesPane.congruenceanalysis.controlepoch.label", "Control epoch"), i18n.getString("UIObservationPropertiesPane.congruenceanalysis.controlepoch.tooltip", "Selected, if group is referred to control epoch"), group, false, Epoch.CONTROL);

			gridPane.add(this.referenceEpochRadioButton, 0, 0);
			gridPane.add(databaseTransactionReferenceEpochProgressIndicator, 1, 0);

			gridPane.add(this.controlEpochRadioButton, 0, 1);
			gridPane.add(databaseTransactionControlEpochProgressIndicator, 1, 1);

			TitledPane congruenceAnalysisTitledPane = this.createTitledPane(i18n.getString("UIObservationPropertiesPane.congruenceanalysis.title", "Congruence analysis"));
			congruenceAnalysisTitledPane.setContent(gridPane);
			return congruenceAnalysisTitledPane;
		}

		private Node createUncertaintyChartPane()
		{
			NumberAxis xAxis = new NumberAxis(0, 155, 10);
			NumberAxis yAxis = new NumberAxis();

			xAxis.setForceZeroInRange(true);
			xAxis.setMinorTickVisible(false);
			xAxis.setAnimated(false);

			yAxis.setMinorTickVisible(false);
			yAxis.setForceZeroInRange(false);
			yAxis.setAnimated(false);

			this.lineChart = new LineChart<Number, Number>(xAxis,yAxis);
			this.lineChart.setLegendVisible(false);
			this.lineChart.setAnimated(false);
			this.lineChart.setPadding(new Insets(0, 0, 0, 0));
			this.lineChart.setCreateSymbols(false);
			this.lineChart.setMinHeight(175);

			this.updateUncertaintyChart(this.lineChart);
			this.updateTickLabels(this.lineChart);

			TitledPane uncertaintyChartTitledPane = this.createTitledPane(i18n.getString("UIObservationPropertiesPane.chart.title", "Combined Uncertainties"));
			uncertaintyChartTitledPane.setContent(this.lineChart);
			return uncertaintyChartTitledPane;
		}

		private void init()
		{
			VBox content = new VBox();
			content.getChildren().addAll(this.createUncertaintiesPane(), this.createAdditionalParametersPane(), this.createCongruenceAnalysisPane(), this.createUncertaintyChartPane());

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
			this.options.addFormatterChangedListener(new TickFormatChangedListener(this));
		}

		private DoubleTextField createDoubleTextField(double value, CellValueType type, bool displayUnit, DoubleTextField.ValueSupport valueSupport, string tooltipText, ParameterType userData)
		{
			DoubleTextField field = new DoubleTextField(value, type, displayUnit, valueSupport);
			field.setTooltip(new Tooltip(tooltipText));
			field.setMinWidth(200);
			field.setMaxWidth(350);
			field.setUserData(userData);
			field.numberProperty().addListener(new NumberChangeListener(this, field));
			return field;
		}

		private UncertaintyTextField createUncertaintyTextField(double value, CellValueType type, bool displayUnit, DoubleTextField.ValueSupport valueSupport, string tooltipText, ObservationGroupUncertaintyType userData)
		{
			UncertaintyTextField field = new UncertaintyTextField(value, type, displayUnit, valueSupport);
			field.setTooltip(new Tooltip(tooltipText));
			field.setMinWidth(200);
			field.setMaxWidth(350);
			field.setUserData(userData);
			field.numberProperty().addListener(new NumberChangeListener(this, field));
			return field;
		}

		private CheckBox createCheckBox(string title, string tooltipText, bool selected, ParameterType userData)
		{
			Label label = new Label(title);
			label.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			label.setMaxSize(double.MaxValue, double.MaxValue);
			label.setPadding(new Insets(0,0,0,3));
			CheckBox checkBox = new CheckBox();
			checkBox.setGraphic(label);
			checkBox.setTooltip(new Tooltip(tooltipText));
			checkBox.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			checkBox.setSelected(selected);
			checkBox.setUserData(userData);
			checkBox.selectedProperty().addListener(new BooleanChangeListener(this, checkBox));
			return checkBox;
		}

		private RadioButton createRadioButton(string title, string tooltipText, ToggleGroup group, bool selected, Epoch userData)
		{
			Label label = new Label(title);
			label.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			label.setMaxSize(double.MaxValue, double.MaxValue);
			label.setPadding(new Insets(0,0,0,3));
			RadioButton radioButton = new RadioButton();
			radioButton.setGraphic(label);
			radioButton.setTooltip(new Tooltip(tooltipText));
			radioButton.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			radioButton.setToggleGroup(group);
			radioButton.setSelected(selected);
			radioButton.setUserData(userData);
			radioButton.selectedProperty().addListener(new BooleanChangeListener(this, radioButton));
			return radioButton;
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

		private void save(ObservationGroupUncertaintyType uncertaintyType)
		{
			try
			{
				double? value = null;
				switch (uncertaintyType.innerEnumValue)
				{
				case ObservationGroupUncertaintyType.InnerEnum.ZERO_POINT_OFFSET:
					value = this.zeroPointOffsetUncertaintyField.Number;
					break;
				case ObservationGroupUncertaintyType.InnerEnum.DISTANCE_DEPENDENT:
					value = this.distanceDependentUncertaintyField.Number;
					break;
				case ObservationGroupUncertaintyType.InnerEnum.SQUARE_ROOT_DISTANCE_DEPENDENT:
					value = this.squareRootDistanceDependentUncertaintyField.Number;
					break;
				default:
					Console.Error.WriteLine(this.GetType().Name + " : Error, unsupported uncertainty type " + uncertaintyType);
					break;
				}

				if (value != null && value.Value >= 0 && this.selectedObservationItemValues != null && this.selectedObservationItemValues.Length > 0)
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
						ProgressIndicator progressIndicator = this.databaseTransactionProgressIndicators[uncertaintyType];
						progressIndicator.setVisible(true);
						this.sequentialTransition.stop();
						this.sequentialTransition.setNode(progressIndicator);
						this.sequentialTransition.playFromStart();
					}
					SQLManager.Instance.saveUncertainty(uncertaintyType, value.Value, this.selectedObservationItemValues);
					this.updateUncertaintyChart(this.lineChart);
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
				OptionDialog.showThrowableDialog(i18n.getString("UIObservationPropertiesPane.message.error.save.uncertainty.exception.title", "Unexpected SQL-Error"), i18n.getString("UIObservationPropertiesPane.message.error.save.uncertainty.exception.header", "Error, could not save group uncertainties to database."), i18n.getString("UIObservationPropertiesPane.message.error.save.uncertainty.exception.message", "An exception has occurred during database transaction."), e);
				});
			}
		}

		private void save(ParameterType parameterType)
		{
			try
			{
				bool enable = false;
				double? value = null;
				switch (parameterType.innerEnumValue)
				{
				case ParameterType.InnerEnum.ORIENTATION:
					enable = this.orientationOffsetCheckBox.isSelected();
					value = this.orientationOffsetField.Number;
					break;
				case ParameterType.InnerEnum.REFRACTION_INDEX:
					enable = this.refractionIndexCheckBox.isSelected();
					value = this.refractionIndexField.Number;
					break;
				case ParameterType.InnerEnum.ROTATION_X:
					enable = this.rotationXCheckBox.isSelected();
					value = this.rotationXField.Number;
					break;
				case ParameterType.InnerEnum.ROTATION_Y:
					enable = this.rotationYCheckBox.isSelected();
					value = this.rotationYField.Number;
					break;
				case ParameterType.InnerEnum.ROTATION_Z:
					enable = this.rotationZCheckBox.isSelected();
					value = this.rotationZField.Number;
					break;
				case ParameterType.InnerEnum.SCALE:
					enable = this.scaleCheckBox.isSelected();
					value = this.scaleField.Number;
					break;
				case ParameterType.InnerEnum.ZERO_POINT_OFFSET:
					enable = this.zeroPointOffsetCheckBox.isSelected();
					value = this.zeroPointOffsetField.Number;
					break;
				default:
					Console.Error.WriteLine(this.GetType().Name + " : Error, unsupported parameter type " + parameterType);
					break;
				}

				if (value != null && this.selectedObservationItemValues != null && this.selectedObservationItemValues.Length > 0)
				{
					this.ProgressIndicatorsVisible = false;
					if (this.warningIconNodes.ContainsKey(parameterType))
					{
						Node warningIconNodes = this.warningIconNodes[parameterType];
						warningIconNodes.setVisible(false);
						warningIconNodes.setManaged(false);
					}
					if (this.databaseTransactionProgressIndicators.ContainsKey(parameterType))
					{
						ProgressIndicator node = this.databaseTransactionProgressIndicators[parameterType];
						node.setVisible(true);
						this.sequentialTransition.stop();
						this.sequentialTransition.setNode(node);
						this.sequentialTransition.playFromStart();
					}
					SQLManager.Instance.saveAdditionalParameter(parameterType, enable, value.Value, this.selectedObservationItemValues);
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
				OptionDialog.showThrowableDialog(i18n.getString("UIObservationPropertiesPane.message.error.save.parameter.exception.title", "Unexpected SQL-Error"), i18n.getString("UIObservationPropertiesPane.message.error.save.parameter.exception.header", "Error, could not save properties of additional group parameters to database."), i18n.getString("UIObservationPropertiesPane.message.error.save.parameter.exception.message", "An exception has occurred during database transaction."), e);
				});
			}
		}

		private void save()
		{
			try
			{
				if (this.selectedObservationItemValues != null && this.selectedObservationItemValues.Length > 0)
				{
					this.ProgressIndicatorsVisible = false;
					if (this.databaseTransactionProgressIndicators.ContainsKey(this.referenceEpochRadioButton.isSelected() ? this.referenceEpochRadioButton.getUserData() : this.controlEpochRadioButton.getUserData()))
					{
						ProgressIndicator node = this.databaseTransactionProgressIndicators[this.referenceEpochRadioButton.isSelected() ? this.referenceEpochRadioButton.getUserData() : this.controlEpochRadioButton.getUserData()];
						node.setVisible(true);
						this.sequentialTransition.stop();
						this.sequentialTransition.setNode(node);
						this.sequentialTransition.playFromStart();
					}
					SQLManager.Instance.saveEpoch(this.referenceEpochRadioButton.isSelected(), this.selectedObservationItemValues);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);

				this.ProgressIndicatorsVisible = false;
				this.ReferenceEpoch = !this.referenceEpochRadioButton.isSelected();
				this.sequentialTransition.stop();

				Platform.runLater(() =>
				{
				OptionDialog.showThrowableDialog(i18n.getString("UIObservationPropertiesPane.message.error.save.epoch.exception.title", "Unexpected SQL-Error"), i18n.getString("UIObservationPropertiesPane.message.error.save.epoch.exception.header", "Error, could not save observation epoch properties to database."), i18n.getString("UIObservationPropertiesPane.message.error.save.epoch.exception.message", "An exception has occurred during database transaction."), e);
				});
			}
		}

		private void updateTickLabels(LineChart<Number, Number> lineChart)
		{
			NumberAxis xAxis = (NumberAxis)lineChart.getXAxis();
			NumberAxis yAxis = (NumberAxis)lineChart.getYAxis();

			CellValueType cellValueType;
			switch (this.type.innerEnumValue)
			{
			case TreeItemType.InnerEnum.DIRECTION_LEAF:
			case TreeItemType.InnerEnum.ZENITH_ANGLE_LEAF:
				cellValueType = CellValueType.ANGLE_UNCERTAINTY;
				yAxis.setLabel(String.format(Locale.ENGLISH, "%s %s", i18n.getString("UIObservationPropertiesPane.chart.axis.y.label", "\u03C30"), options.FormatterOptions[CellValueType.ANGLE_UNCERTAINTY].getUnit().toFormattedAbbreviation()).Trim());
				break;

			default:
				cellValueType = CellValueType.LENGTH_UNCERTAINTY;
				yAxis.setLabel(String.format(Locale.ENGLISH, "%s %s", i18n.getString("UIObservationPropertiesPane.chart.axis.y.label", "\u03C30"), options.FormatterOptions[CellValueType.LENGTH_UNCERTAINTY].getUnit().toFormattedAbbreviation()).Trim());
				break;
			}

			yAxis.setTickLabelFormatter(new StringConverterAnonymousInnerClass(this, cellValueType));

			xAxis.setTickLabelFormatter(new StringConverterAnonymousInnerClass2(this));
			xAxis.setLabel(String.format(Locale.ENGLISH, "%s %s", i18n.getString("UIObservationPropertiesPane.chart.axis.x.label", "d0"), options.FormatterOptions[CellValueType.LENGTH].getUnit().toFormattedAbbreviation()).Trim());
		}

		private class StringConverterAnonymousInnerClass : StringConverter<Number>
		{
			private readonly UIObservationPropertiesPane outerInstance;

			private CellValueType cellValueType;

			public StringConverterAnonymousInnerClass(UIObservationPropertiesPane outerInstance, CellValueType cellValueType)
			{
				this.outerInstance = outerInstance;
				this.cellValueType = cellValueType;
			}

			public override string toString(Number number)
			{
				return outerInstance.options.toViewFormat(cellValueType, number.doubleValue(), false);
			}

			public override Number fromString(string @string)
			{
				return null;
			}
		}

		private class StringConverterAnonymousInnerClass2 : StringConverter<Number>
		{
			private readonly UIObservationPropertiesPane outerInstance;

			public StringConverterAnonymousInnerClass2(UIObservationPropertiesPane outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override string toString(Number number)
			{
				return (number.doubleValue() > outerInstance.maxDistanceForUncertaintyChart) ? "" : String.format(Locale.ENGLISH, "%.0f", number.doubleValue());
			}

			public override Number fromString(string @string)
			{
				return null;
			}
		}

		private void updateUncertaintyChart(LineChart<Number, Number> lineChart)
		{
			XYChart.Series<Number, Number> uncertaintyChartSeries = new XYChart.Series<Number, Number>();

			double uncertaintyA = this.zeroPointOffsetUncertaintyField.Number.Value;
			double uncertaintyB = this.squareRootDistanceDependentUncertaintyField.Number.Value;
			double uncertaintyC = this.distanceDependentUncertaintyField.Number.Value;

			ObservationGroup group = null;
			switch (this.type.innerEnumValue)
			{
			case TreeItemType.InnerEnum.LEVELING_LEAF:
				group = new DeltaZGroup(-1, uncertaintyA, uncertaintyB, uncertaintyC, Epoch.REFERENCE);
				break;
			case TreeItemType.InnerEnum.DIRECTION_LEAF:
				group = new DirectionGroup(-1, uncertaintyA, uncertaintyB, uncertaintyC, Epoch.REFERENCE);
				break;
			case TreeItemType.InnerEnum.HORIZONTAL_DISTANCE_LEAF:
				group = new HorizontalDistanceGroup(-1, uncertaintyA, uncertaintyB, uncertaintyC, Epoch.REFERENCE);
				break;
			case TreeItemType.InnerEnum.SLOPE_DISTANCE_LEAF:
				group = new SlopeDistanceGroup(-1, uncertaintyA, uncertaintyB, uncertaintyC, Epoch.REFERENCE);
				break;
			case TreeItemType.InnerEnum.ZENITH_ANGLE_LEAF:
				group = new ZenithAngleGroup(-1, uncertaintyA, uncertaintyB, uncertaintyC, Epoch.REFERENCE);
				break;
			case TreeItemType.InnerEnum.GNSS_1D_LEAF:
				group = new GNSSBaseline1DGroup(-1, uncertaintyA, uncertaintyB, uncertaintyC, Epoch.REFERENCE);
				break;
			case TreeItemType.InnerEnum.GNSS_2D_LEAF:
				group = new GNSSBaseline2DGroup(-1, uncertaintyA, uncertaintyB, uncertaintyC, Epoch.REFERENCE);
				break;
			case TreeItemType.InnerEnum.GNSS_3D_LEAF:
				group = new GNSSBaseline3DGroup(-1, uncertaintyA, uncertaintyB, uncertaintyC, Epoch.REFERENCE);
				break;
			default:
				Console.Error.WriteLine(this.GetType().Name + " Error, unsupported obsewrvation group type " + type);
				break;
			}

			if (group == null)
			{
				return;
			}

			for (double distanceInViewUnit = this.minDistanceForUncertaintyChart; distanceInViewUnit <= this.maxDistanceForUncertaintyChart; distanceInViewUnit += 0.25)
			{
				double distanceInModelUnit = options.convertLengthToModel(distanceInViewUnit); // distance in meter
				Observation observation = this.createObservation(this.type, distanceInModelUnit);
				if (observation == null)
				{
					break;
				}

				double uncertaintyInModelUnit = group.getStd(observation);
				double uncertaintyInViewUnit = 0;

				switch (this.type.innerEnumValue)
				{
				case TreeItemType.InnerEnum.DIRECTION_LEAF:
				case TreeItemType.InnerEnum.ZENITH_ANGLE_LEAF:
					uncertaintyInViewUnit = options.convertAngleUncertaintyToView(uncertaintyInModelUnit);
					break;
				default:
					uncertaintyInViewUnit = options.convertLengthUncertaintyToView(uncertaintyInModelUnit);
					break;
				}
				uncertaintyChartSeries.getData().add(new XYChart.Data<Number, Number>(distanceInViewUnit, uncertaintyInViewUnit));
			}

			this.lineChart.getData().clear();
			this.lineChart.getData().add(uncertaintyChartSeries);

			Node line = uncertaintyChartSeries.getNode().lookup(".chart-series-line");
			line.setStyle("-fx-stroke: rgba(200, 0, 0, 1); -fx-stroke-width: 2.5px;");
		}

		private Observation createObservation(TreeItemType type, double distance)
		{
			Point startPoint = new Point3D("0", 0, 0, 0);
			Point endPoint = new Point3D("1", distance, 0, 0);

			Observation observation = null;
			switch (this.type.innerEnumValue)
			{
			case TreeItemType.InnerEnum.LEVELING_LEAF:
				observation = new DeltaZ(-1, startPoint, endPoint, 0.0, 0.0, 0.0, -1, distance);
				break;
			case TreeItemType.InnerEnum.DIRECTION_LEAF:
				observation = new Direction(-1, startPoint, endPoint, 0.0, 0.0, 0.0, -1, distance);
				break;
			case TreeItemType.InnerEnum.HORIZONTAL_DISTANCE_LEAF:
				observation = new HorizontalDistance(-1, startPoint, endPoint, 0.0, 0.0, 0.0, -1, distance);
				break;
			case TreeItemType.InnerEnum.SLOPE_DISTANCE_LEAF:
				observation = new SlopeDistance(-1, startPoint, endPoint, 0.0, 0.0, 0.0, -1, distance);
				break;
			case TreeItemType.InnerEnum.ZENITH_ANGLE_LEAF:
				observation = new ZenithAngle(-1, startPoint, endPoint, 0.0, 0.0, 0.0, -1, distance);
				break;
			case TreeItemType.InnerEnum.GNSS_1D_LEAF:
				observation = new GNSSBaselineDeltaZ1D(-1, startPoint, endPoint, distance, -1);
				break;
			case TreeItemType.InnerEnum.GNSS_2D_LEAF:
				observation = new GNSSBaselineDeltaX2D(-1, startPoint, endPoint, distance, -1);
				break;
			case TreeItemType.InnerEnum.GNSS_3D_LEAF:
				observation = new GNSSBaselineDeltaX3D(-1, startPoint, endPoint, distance, -1);
				break;
			default:
				Console.Error.WriteLine(this.GetType().Name + " Error, unsupported obsewrvation group type " + type);
				break;
			}
			return observation;
		}

	}
}