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

	using RestrictionType = org.applied_geodesy.adjustment.network.congruence.strain.RestrictionType;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using CongruenceAnalysisTreeItemValue = org.applied_geodesy.jag3d.ui.tree.CongruenceAnalysisTreeItemValue;
	using TreeItemType = org.applied_geodesy.jag3d.ui.tree.TreeItemType;
	using UITreeBuilder = org.applied_geodesy.jag3d.ui.tree.UITreeBuilder;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
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
	using CheckBox = javafx.scene.control.CheckBox;
	using Control = javafx.scene.control.Control;
	using Label = javafx.scene.control.Label;
	using ProgressIndicator = javafx.scene.control.ProgressIndicator;
	using ScrollPane = javafx.scene.control.ScrollPane;
	using TitledPane = javafx.scene.control.TitledPane;
	using Tooltip = javafx.scene.control.Tooltip;
	using GridPane = javafx.scene.layout.GridPane;
	using Priority = javafx.scene.layout.Priority;
	using Region = javafx.scene.layout.Region;
	using VBox = javafx.scene.layout.VBox;
	using Font = javafx.scene.text.Font;
	using Duration = javafx.util.Duration;

	public class UICongruenceAnalysisPropertiesPane
	{

		private class BooleanChangeListener : ChangeListener<bool>
		{
			private readonly UICongruenceAnalysisPropertiesPane outerInstance;

			internal readonly CheckBox button;

			internal BooleanChangeListener(UICongruenceAnalysisPropertiesPane outerInstance, CheckBox button)
			{
				this.outerInstance = outerInstance;
				this.button = button;
			}

			public override void changed<T1>(ObservableValue<T1> observable, bool? oldValue, bool? newValue) where T1 : bool
			{
				if (!outerInstance.ignoreValueUpdate && this.button.getUserData() != null)
				{
					if (this.button.getUserData() is RestrictionType)
					{
						RestrictionType paramType = (RestrictionType)this.button.getUserData();
						switch (paramType.innerEnumValue)
						{
						case RestrictionType.InnerEnum.IDENT_SCALES_XY:
						case RestrictionType.InnerEnum.IDENT_SCALES_XZ:
						case RestrictionType.InnerEnum.IDENT_SCALES_YZ:
							outerInstance.save(paramType, !this.button.isSelected());
							break;
						default:
							outerInstance.save(paramType, this.button.isSelected());
							break;

						}
					}
				}
			}
		}

		private class BoundedRestrictionChangeListener : ChangeListener<bool>
		{
			private readonly UICongruenceAnalysisPropertiesPane outerInstance;

			internal readonly CheckBox dependendButton1, dependendButton2;

			internal BoundedRestrictionChangeListener(UICongruenceAnalysisPropertiesPane outerInstance, CheckBox dependendButton1, CheckBox dependendButton2)
			{
				this.outerInstance = outerInstance;
				this.dependendButton1 = dependendButton1;
				this.dependendButton2 = dependendButton2;
			}

			public override void changed<T1>(ObservableValue<T1> observable, bool? oldValue, bool? newValue) where T1 : bool
			{
				if (newValue.Value)
				{
					this.dependendButton1.setSelected(true);
					this.dependendButton2.setSelected(true);
				}
			}
		}

		private class DependendRestrictionChangeListener : ChangeListener<bool>
		{
			private readonly UICongruenceAnalysisPropertiesPane outerInstance;

			internal readonly CheckBox boundedButton;

			internal DependendRestrictionChangeListener(UICongruenceAnalysisPropertiesPane outerInstance, CheckBox boundedButton)
			{
				this.outerInstance = outerInstance;
				this.boundedButton = boundedButton;
			}

			public override void changed<T1>(ObservableValue<T1> observable, bool? oldValue, bool? newValue) where T1 : bool
			{
				if ((!newValue).Value)
				{
					this.boundedButton.setSelected(false);
				}
			}
		}

		private class SequentialTransitionFinishedListener : ChangeListener<EventHandler<ActionEvent>>
		{
			private readonly UICongruenceAnalysisPropertiesPane outerInstance;

			public SequentialTransitionFinishedListener(UICongruenceAnalysisPropertiesPane outerInstance)
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

		private CheckBox translationXCheckBox;
		private CheckBox translationYCheckBox;
		private CheckBox translationZCheckBox;

		private CheckBox scaleXCheckBox;
		private CheckBox scaleYCheckBox;
		private CheckBox scaleZCheckBox;

		private CheckBox scaleXYCheckBox;
		private CheckBox scaleXZCheckBox;
		private CheckBox scaleYZCheckBox;

		private CheckBox rotationXCheckBox;
		private CheckBox rotationYCheckBox;
		private CheckBox rotationZCheckBox;

		private CheckBox shearXCheckBox;
		private CheckBox shearYCheckBox;
		private CheckBox shearZCheckBox;

		private Label selectionInfoLabel = new Label();

		private IDictionary<object, ProgressIndicator> databaseTransactionProgressIndicators = new Dictionary<object, ProgressIndicator>(10);
		private SequentialTransition sequentialTransition = new SequentialTransition();

		private bool ignoreValueUpdate = false;
		private CongruenceAnalysisTreeItemValue[] selectedCongruenceAnalysisItemValues = null;

		internal UICongruenceAnalysisPropertiesPane(TreeItemType type)
		{
			switch (type.innerEnumValue)
			{
			case TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_1D_LEAF:
			case TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_2D_LEAF:
			case TreeItemType.InnerEnum.CONGRUENCE_ANALYSIS_3D_LEAF:
				this.type = type;
				this.init();
				break;
			default:
				throw new System.ArgumentException(this.GetType().Name + " Error, unsupported item type " + type);
			}
		}

		public virtual void setTreeItemValue(string name, params CongruenceAnalysisTreeItemValue[] selectedCongruenceAnalysisItemValues)
		{
			if (this.selectedCongruenceAnalysisItemValues != selectedCongruenceAnalysisItemValues)
			{
				this.reset();
				this.selectedCongruenceAnalysisItemValues = selectedCongruenceAnalysisItemValues;
			}
			this.setGroupName(name, this.selectedCongruenceAnalysisItemValues != null ? this.selectedCongruenceAnalysisItemValues.Length : 0);
		}

		private void setGroupName(string name, int cnt)
		{
			if (this.selectionInfoLabel != null)
			{
				string groupNameTmpl = this.i18n.getString("UICongruenceAnalysisPropertiesPane.status.selection.name.label", "Status:");
				string selectionCntTmpl = cnt > 1 ? String.format(Locale.ENGLISH, this.i18n.getString("UICongruenceAnalysisPropertiesPane.status.selection.counter.label", "and %d more selected group(s)\u2026"), cnt) : "";
				string label = String.format(Locale.ENGLISH, "%s %s %s", groupNameTmpl, name, selectionCntTmpl);
				if (!this.selectionInfoLabel.getText().Equals(label))
				{
					this.selectionInfoLabel.setText(label);
				}
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

			// set focus to panel to commit text field values and to force db transaction
			UITreeBuilder.Instance.Tree.requestFocus();

			this.TranslationY = false;
			this.TranslationX = false;
			this.TranslationZ = false;

			this.RotationY = false;
			this.RotationX = false;
			this.RotationZ = false;

			this.ScaleY = false;
			this.ScaleX = false;
			this.ScaleZ = false;

			this.ShearY = false;
			this.ShearX = false;
			this.ShearZ = false;

			this.ScaleRestrictionXY = true;
			this.ScaleRestrictionXZ = true;
			this.ScaleRestrictionYZ = true;
		}

		public virtual bool setStrainParameter(RestrictionType restrictionType, bool? enable)
		{
			switch (restrictionType.innerEnumValue)
			{
			case RestrictionType.InnerEnum.FIXED_ROTATION_X:
				return this.setRotationX(enable);
			case RestrictionType.InnerEnum.FIXED_ROTATION_Y:
				return this.setRotationY(enable);
			case RestrictionType.InnerEnum.FIXED_ROTATION_Z:
				return this.setRotationZ(enable);
			case RestrictionType.InnerEnum.FIXED_SCALE_X:
				return this.setScaleX(enable);
			case RestrictionType.InnerEnum.FIXED_SCALE_Y:
				return this.setScaleY(enable);
			case RestrictionType.InnerEnum.FIXED_SCALE_Z:
				return this.setScaleZ(enable);
			case RestrictionType.InnerEnum.FIXED_SHEAR_X:
				return this.setShearX(enable);
			case RestrictionType.InnerEnum.FIXED_SHEAR_Y:
				return this.setShearY(enable);
			case RestrictionType.InnerEnum.FIXED_SHEAR_Z:
				return this.setShearZ(enable);
			case RestrictionType.InnerEnum.FIXED_TRANSLATION_X:
				return this.setTranslationX(enable);
			case RestrictionType.InnerEnum.FIXED_TRANSLATION_Y:
				return this.setTranslationY(enable);
			case RestrictionType.InnerEnum.FIXED_TRANSLATION_Z:
				return this.setTranslationZ(enable);
			case RestrictionType.InnerEnum.IDENT_SCALES_XY:
				return this.setScaleRestrictionXY(enable);
			case RestrictionType.InnerEnum.IDENT_SCALES_XZ:
				return this.setScaleRestrictionXZ(enable);
			case RestrictionType.InnerEnum.IDENT_SCALES_YZ:
				return this.setScaleRestrictionYZ(enable);
			default:
				return false;

			}
		}

		public virtual bool setTranslationX(bool? enable)
		{
			if (this.translationXCheckBox == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.translationXCheckBox.setSelected(enable != null && enable == true);
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setTranslationY(bool? enable)
		{
			if (this.translationYCheckBox == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.translationYCheckBox.setSelected(enable != null && enable == true);
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setTranslationZ(bool? enable)
		{
			if (this.translationZCheckBox == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.translationZCheckBox.setSelected(enable != null && enable == true);
			this.ignoreValueUpdate = false;
			return true;
		}


		public virtual bool setScaleRestrictionYZ(bool? enable)
		{
			if (this.scaleYZCheckBox == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.scaleYZCheckBox.setSelected(enable != null && enable == false);
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setScaleRestrictionXY(bool? enable)
		{
			if (this.scaleXYCheckBox == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.scaleXYCheckBox.setSelected(enable != null && enable == false);
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setScaleRestrictionXZ(bool? enable)
		{
			if (this.scaleXZCheckBox == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.scaleXZCheckBox.setSelected(enable != null && enable == false);
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setScaleX(bool? enable)
		{
			if (this.scaleXCheckBox == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.scaleXCheckBox.setSelected(enable != null && enable == true);
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setScaleY(bool? enable)
		{
			if (this.scaleYCheckBox == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.scaleYCheckBox.setSelected(enable != null && enable == true);
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setScaleZ(bool? enable)
		{
			if (this.scaleZCheckBox == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.scaleZCheckBox.setSelected(enable != null && enable == true);
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setRotationX(bool? enable)
		{
			if (this.rotationXCheckBox == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.rotationXCheckBox.setSelected(enable != null && enable == true);
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setRotationY(bool? enable)
		{
			if (this.rotationYCheckBox == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.rotationYCheckBox.setSelected(enable != null && enable == true);
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setRotationZ(bool? enable)
		{
			if (this.rotationZCheckBox == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.rotationZCheckBox.setSelected(enable != null && enable == true);
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setShearX(bool? enable)
		{
			if (this.shearXCheckBox == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.shearXCheckBox.setSelected(enable != null && enable == true);
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setShearY(bool? enable)
		{
			if (this.shearYCheckBox == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.shearYCheckBox.setSelected(enable != null && enable == true);
			this.ignoreValueUpdate = false;
			return true;
		}

		public virtual bool setShearZ(bool? enable)
		{
			if (this.shearZCheckBox == null)
			{
				return false;
			}
			this.ignoreValueUpdate = true;
			this.shearZCheckBox.setSelected(enable != null && enable == true);
			this.ignoreValueUpdate = false;
			return true;
		}

		private Node createTranslationPane(RestrictionType[] restrictionTypes)
		{
			GridPane gridPane = this.createGridPane();

			int row = 0;

			foreach (RestrictionType restrictionType in restrictionTypes)
			{
				CheckBox box = null;
				ProgressIndicator progressIndicator = null;

				switch (restrictionType.innerEnumValue)
				{
				case RestrictionType.InnerEnum.FIXED_TRANSLATION_Y:

					box = this.translationYCheckBox = this.createCheckBox(i18n.getString("UICongruenceAnalysisPropertiesPane.translation.y.label", "Translation y"), i18n.getString("UICongruenceAnalysisPropertiesPane.translation.y.label.tooltip", "Checked, if translation in y is a strain parameter to be estimated"), false, restrictionType);
					progressIndicator = this.createDatabaseTransactionProgressIndicator(restrictionType);

					break;
				case RestrictionType.InnerEnum.FIXED_TRANSLATION_X:

					box = this.translationXCheckBox = this.createCheckBox(i18n.getString("UICongruenceAnalysisPropertiesPane.translation.x.label", "Translation x"), i18n.getString("UICongruenceAnalysisPropertiesPane.translation.x.label.tooltip", "Checked, if translation in x is a strain parameter to be estimated"), false, restrictionType);
					progressIndicator = this.createDatabaseTransactionProgressIndicator(restrictionType);

					break;
				case RestrictionType.InnerEnum.FIXED_TRANSLATION_Z:

					box = this.translationZCheckBox = this.createCheckBox(i18n.getString("UICongruenceAnalysisPropertiesPane.translation.z.label", "Translation z"), i18n.getString("UICongruenceAnalysisPropertiesPane.translation.z.label.tooltip", "Checked, if translation in z is a strain parameter to be estimated"), false, restrictionType);
					progressIndicator = this.createDatabaseTransactionProgressIndicator(restrictionType);

					break;
				default:
					continue;
				}

				if (box != null && progressIndicator != null)
				{
					gridPane.add(box, 0, row);
					gridPane.add(progressIndicator, 1, row++);
				}
			}

			if (row == 0)
			{
				return null;
			}

			TitledPane parametersTitledPane = this.createTitledPane(i18n.getString("UICongruenceAnalysisPropertiesPane.translation.title", "Translation parameters"));
			parametersTitledPane.setContent(gridPane);
			return parametersTitledPane;
		}

		private Node createRotationPane(RestrictionType[] restrictionTypes)
		{
			GridPane gridPane = this.createGridPane();

			int row = 0;

			foreach (RestrictionType restrictionType in restrictionTypes)
			{
				CheckBox box = null;
				ProgressIndicator progressIndicator = null;

				switch (restrictionType.innerEnumValue)
				{
				case RestrictionType.InnerEnum.FIXED_ROTATION_Y:

					box = this.rotationYCheckBox = this.createCheckBox(i18n.getString("UICongruenceAnalysisPropertiesPane.rotation.y.label", "Rotation y"), i18n.getString("UICongruenceAnalysisPropertiesPane.rotation.y.label.tooltip", "Checked, if rotation angle around y-axis is a strain parameter to be estimated"), false, restrictionType);
					progressIndicator = this.createDatabaseTransactionProgressIndicator(restrictionType);

					break;
				case RestrictionType.InnerEnum.FIXED_ROTATION_X:

					box = this.rotationXCheckBox = this.createCheckBox(i18n.getString("UICongruenceAnalysisPropertiesPane.rotation.x.label", "Rotation x"), i18n.getString("UICongruenceAnalysisPropertiesPane.rotation.x.label.tooltip", "Checked, if rotation angle around x-axis is a strain parameter to be estimated"), false, restrictionType);
					progressIndicator = this.createDatabaseTransactionProgressIndicator(restrictionType);

					break;
				case RestrictionType.InnerEnum.FIXED_ROTATION_Z:

					box = this.rotationZCheckBox = this.createCheckBox(i18n.getString("UICongruenceAnalysisPropertiesPane.rotation.z.label", "Rotation z"), i18n.getString("UICongruenceAnalysisPropertiesPane.rotation.z.label.tooltip", "Checked, if rotation angle around z-axis is a strain parameter to be estimated"), false, restrictionType);
					progressIndicator = this.createDatabaseTransactionProgressIndicator(restrictionType);

					break;
				default:
					continue;
				}

				if (box != null && progressIndicator != null)
				{
					gridPane.add(box, 0, row);
					gridPane.add(progressIndicator, 1, row++);
				}
			}

			if (row == 0)
			{
				return null;
			}

			TitledPane parametersTitledPane = this.createTitledPane(i18n.getString("UICongruenceAnalysisPropertiesPane.rotation.title", "Rotation parameters"));
			parametersTitledPane.setContent(gridPane);
			return parametersTitledPane;
		}

		private Node createShearPane(RestrictionType[] restrictionTypes)
		{
			GridPane gridPane = this.createGridPane();

			int row = 0;

			foreach (RestrictionType restrictionType in restrictionTypes)
			{
				CheckBox box = null;
				ProgressIndicator progressIndicator = null;
				switch (restrictionType.innerEnumValue)
				{
				case RestrictionType.InnerEnum.FIXED_SHEAR_Y:

					box = this.shearYCheckBox = this.createCheckBox(i18n.getString("UICongruenceAnalysisPropertiesPane.shear.y.label", "Shear y"), i18n.getString("UICongruenceAnalysisPropertiesPane.shear.y.label.tooltip", "Checked, if shear in y is a strain parameter to be estimated"), false, restrictionType);
					progressIndicator = this.createDatabaseTransactionProgressIndicator(restrictionType);

					break;
				case RestrictionType.InnerEnum.FIXED_SHEAR_X:

					box = this.shearXCheckBox = this.createCheckBox(i18n.getString("UICongruenceAnalysisPropertiesPane.shear.x.label", "Shear x"), i18n.getString("UICongruenceAnalysisPropertiesPane.shear.x.label.tooltip", "Checked, if shear in x is a strain parameter to be estimated"), false, restrictionType);
					progressIndicator = this.createDatabaseTransactionProgressIndicator(restrictionType);

					break;
				case RestrictionType.InnerEnum.FIXED_SHEAR_Z:

					box = this.shearZCheckBox = this.createCheckBox(i18n.getString("UICongruenceAnalysisPropertiesPane.shear.z.label", "Shear z"), i18n.getString("UICongruenceAnalysisPropertiesPane.shear.z.label.tooltip", "Checked, if shear in z is a strain parameter to be estimated"), false, restrictionType);
					progressIndicator = this.createDatabaseTransactionProgressIndicator(restrictionType);

					break;
				default:
					continue;
				}

				if (box != null)
				{
					gridPane.add(box, 0, row);
					gridPane.add(progressIndicator, 1, row++);
				}
			}

			if (row == 0)
			{
				return null;
			}

			TitledPane parametersTitledPane = this.createTitledPane(i18n.getString("UICongruenceAnalysisPropertiesPane.shear.title", "Shear parameters"));
			parametersTitledPane.setContent(gridPane);
			return parametersTitledPane;
		}

		private Node createScalePane(RestrictionType[] restrictionTypes)
		{
			GridPane gridPane = this.createGridPane();

			int row = 0;

			foreach (RestrictionType restrictionType in restrictionTypes)
			{
				CheckBox box = null;
				ProgressIndicator progressIndicator = null;

				switch (restrictionType.innerEnumValue)
				{
				case RestrictionType.InnerEnum.FIXED_SCALE_Y:

					box = this.scaleYCheckBox = this.createCheckBox(i18n.getString("UICongruenceAnalysisPropertiesPane.scale.y.label", "Scale y"), i18n.getString("UICongruenceAnalysisPropertiesPane.scale.y.label.tooltip", "Checked, if scale in y is a strain parameter to be estimated"), false, restrictionType);
					progressIndicator = this.createDatabaseTransactionProgressIndicator(restrictionType);

					break;
				case RestrictionType.InnerEnum.FIXED_SCALE_X:

					box = this.scaleXCheckBox = this.createCheckBox(i18n.getString("UICongruenceAnalysisPropertiesPane.scale.x.label", "Scale x"), i18n.getString("UICongruenceAnalysisPropertiesPane.scale.x.label.tooltip", "Checked, if scale in x is a strain parameter to be estimated"), false, restrictionType);
					progressIndicator = this.createDatabaseTransactionProgressIndicator(restrictionType);

					break;
				case RestrictionType.InnerEnum.FIXED_SCALE_Z:

					box = this.scaleZCheckBox = this.createCheckBox(i18n.getString("UICongruenceAnalysisPropertiesPane.scale.z.label", "Scale z"), i18n.getString("UICongruenceAnalysisPropertiesPane.scale.z.label.tooltip", "Checked, if scale in z is a strain parameter to be estimated"), false, restrictionType);
					progressIndicator = this.createDatabaseTransactionProgressIndicator(restrictionType);

					break;
				default:
					continue;
				}

				if (box != null)
				{
					gridPane.add(box, 0, row);
					gridPane.add(progressIndicator, 1, row++);
				}
			}

			if (row == 0)
			{
				return null;
			}

			TitledPane parametersTitledPane = this.createTitledPane(i18n.getString("UICongruenceAnalysisPropertiesPane.scale.title", "Scale parameters"));
			parametersTitledPane.setContent(gridPane);
			return parametersTitledPane;
		}

		private Node createScaleRestrictionPane(RestrictionType[] restrictionTypes)
		{
			GridPane gridPane = this.createGridPane();

			int row = 0;

			foreach (RestrictionType restrictionType in restrictionTypes)
			{
				CheckBox box = null;
				ProgressIndicator progressIndicator = null;

				switch (restrictionType.innerEnumValue)
				{
				case RestrictionType.InnerEnum.IDENT_SCALES_XY:

					box = this.scaleXYCheckBox = this.createCheckBox(i18n.getString("UICongruenceAnalysisPropertiesPane.scale.restriction.xy.label", "Scale y = x"), i18n.getString("UICongruenceAnalysisPropertiesPane.scale.restriction.xy.label.tooltip", "Checked, if scale restriction has to applied"), false, restrictionType);
					progressIndicator = this.createDatabaseTransactionProgressIndicator(restrictionType);

					break;
				case RestrictionType.InnerEnum.IDENT_SCALES_YZ:

					box = this.scaleYZCheckBox = this.createCheckBox(i18n.getString("UICongruenceAnalysisPropertiesPane.scale.restriction.yz.label", "Scale y = z"), i18n.getString("UICongruenceAnalysisPropertiesPane.scale.restriction.yz.label.tooltip", "Checked, if scale restriction has to applied"), false, restrictionType);
					progressIndicator = this.createDatabaseTransactionProgressIndicator(restrictionType);

					break;
				case RestrictionType.InnerEnum.IDENT_SCALES_XZ:

					box = this.scaleXZCheckBox = this.createCheckBox(i18n.getString("UICongruenceAnalysisPropertiesPane.scale.restriction.xz.label", "Scale x = z"), i18n.getString("UICongruenceAnalysisPropertiesPane.scale.restriction.xz.label.tooltip", "Checked, if scale restriction has to applied"), false, restrictionType);
					progressIndicator = this.createDatabaseTransactionProgressIndicator(restrictionType);

					break;
				default:
					continue;
				}

				if (box != null)
				{
					gridPane.add(box, 0, row);
					gridPane.add(progressIndicator, 1, row++);
				}
			}

			if (row == 0)
			{
				return null;
			}

			TitledPane parametersTitledPane = this.createTitledPane(i18n.getString("UICongruenceAnalysisPropertiesPane.scale.restriction.title", "Scale restrictions"));
			parametersTitledPane.setContent(gridPane);
			return parametersTitledPane;
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

			RestrictionType[] parameterTypes = CongruenceAnalysisTreeItemValue.getRestrictionTypes(this.type);

			VBox content = new VBox();

			Node translationPane = this.createTranslationPane(parameterTypes);
			Node rotationPane = this.createRotationPane(parameterTypes);
			Node shearPane = this.createShearPane(parameterTypes);
			Node scalePane = this.createScalePane(parameterTypes);
			Node scaleRestrictionPane = this.createScaleRestrictionPane(parameterTypes);

			if (this.scaleXYCheckBox != null && this.scaleXCheckBox != null && this.scaleYCheckBox != null)
			{
				this.scaleXYCheckBox.selectedProperty().addListener(new BoundedRestrictionChangeListener(this, this.scaleXCheckBox, this.scaleYCheckBox));
				this.scaleXCheckBox.selectedProperty().addListener(new DependendRestrictionChangeListener(this, this.scaleXYCheckBox));
				this.scaleYCheckBox.selectedProperty().addListener(new DependendRestrictionChangeListener(this, this.scaleXYCheckBox));
			}

			if (this.scaleXZCheckBox != null && this.scaleXCheckBox != null && this.scaleZCheckBox != null)
			{
				this.scaleXZCheckBox.selectedProperty().addListener(new BoundedRestrictionChangeListener(this, this.scaleXCheckBox, this.scaleZCheckBox));
				this.scaleXCheckBox.selectedProperty().addListener(new DependendRestrictionChangeListener(this, this.scaleXZCheckBox));
				this.scaleZCheckBox.selectedProperty().addListener(new DependendRestrictionChangeListener(this, this.scaleXZCheckBox));
			}

			if (this.scaleYZCheckBox != null && this.scaleYCheckBox != null && this.scaleZCheckBox != null)
			{
				this.scaleYZCheckBox.selectedProperty().addListener(new BoundedRestrictionChangeListener(this, this.scaleYCheckBox, this.scaleZCheckBox));
				this.scaleYCheckBox.selectedProperty().addListener(new DependendRestrictionChangeListener(this, this.scaleYZCheckBox));
				this.scaleZCheckBox.selectedProperty().addListener(new DependendRestrictionChangeListener(this, this.scaleYZCheckBox));
			}

			this.reset();

			if (translationPane != null)
			{
				content.getChildren().add(translationPane);
			}

			if (rotationPane != null)
			{
				content.getChildren().add(rotationPane);
			}

			if (shearPane != null)
			{
				content.getChildren().add(shearPane);
			}

			if (scalePane != null)
			{
				content.getChildren().add(scalePane);
			}

			if (scaleRestrictionPane != null)
			{
				content.getChildren().add(scaleRestrictionPane);
			}

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

		private CheckBox createCheckBox(string title, string tooltipText, bool selected, RestrictionType userData)
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

		private void save(RestrictionType parameterType, bool selected)
		{
			try
			{
				if (this.selectedCongruenceAnalysisItemValues != null && this.selectedCongruenceAnalysisItemValues.Length > 0)
				{
					this.ProgressIndicatorsVisible = false;
					if (this.databaseTransactionProgressIndicators.ContainsKey(parameterType))
					{
						ProgressIndicator node = this.databaseTransactionProgressIndicators[parameterType];
						node.setVisible(true);
						this.sequentialTransition.stop();
						this.sequentialTransition.setNode(node);
						this.sequentialTransition.playFromStart();
					}
					SQLManager.Instance.saveStrainParameter(parameterType, selected, this.selectedCongruenceAnalysisItemValues);
				}

			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);

				this.sequentialTransition.stop();
				this.ProgressIndicatorsVisible = false;

				Platform.runLater(() =>
				{
				OptionDialog.showThrowableDialog(i18n.getString("UICongruenceAnalysisPropertiesPane.message.error.save.exception.title", "Unexpected SQL-Error"), i18n.getString("UICongruenceAnalysisPropertiesPane.message.error.save.exception.header", "Error, could not save strain properties to database."), i18n.getString("UICongruenceAnalysisPropertiesPane.message.error.save.exception.message", "An exception has occurred during database transaction."), e);
				});
			}
		}
	}

}