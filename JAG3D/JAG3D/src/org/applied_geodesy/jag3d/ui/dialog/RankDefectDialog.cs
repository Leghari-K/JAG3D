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

	using DefectType = org.applied_geodesy.adjustment.network.DefectType;
	using RankDefect = org.applied_geodesy.adjustment.network.RankDefect;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;

	using Platform = javafx.application.Platform;
	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using EventHandler = javafx.@event.EventHandler;
	using Insets = javafx.geometry.Insets;
	using Node = javafx.scene.Node;
	using Accordion = javafx.scene.control.Accordion;
	using ButtonType = javafx.scene.control.ButtonType;
	using CheckBox = javafx.scene.control.CheckBox;
	using Control = javafx.scene.control.Control;
	using Dialog = javafx.scene.control.Dialog;
	using DialogEvent = javafx.scene.control.DialogEvent;
	using Label = javafx.scene.control.Label;
	using TitledPane = javafx.scene.control.TitledPane;
	using Tooltip = javafx.scene.control.Tooltip;
	using HBox = javafx.scene.layout.HBox;
	using VBox = javafx.scene.layout.VBox;
	using Modality = javafx.stage.Modality;
	using Stage = javafx.stage.Stage;
	using Window = javafx.stage.Window;
	using Callback = javafx.util.Callback;

	public class RankDefectDialog
	{

		private class ScaleSelectionChangeListener : ChangeListener<bool>
		{
			private readonly RankDefectDialog outerInstance;

			internal CheckBox checkBox;
			internal ScaleSelectionChangeListener(RankDefectDialog outerInstance, CheckBox checkBox)
			{
				this.outerInstance = outerInstance;
				this.checkBox = checkBox;
			}

			public override void changed<T1>(ObservableValue<T1> observable, bool? oldValue, bool? newValue) where T1 : bool
			{
				if (this.checkBox == outerInstance.userDefinedCheckBox)
				{
					outerInstance.disableCheckBoxes(!outerInstance.userDefinedCheckBox.isSelected());
				}
				else
				{
					if ((this.checkBox == outerInstance.scaleXYCheckBox || this.checkBox == outerInstance.scaleXYZCheckBox) && this.checkBox.isSelected())
					{
						outerInstance.scaleXCheckBox.setSelected(false);
						outerInstance.scaleYCheckBox.setSelected(false);
						if (outerInstance.scaleXYZCheckBox == this.checkBox)
						{
							outerInstance.scaleZCheckBox.setSelected(false);
							outerInstance.scaleXYCheckBox.setSelected(false);
						}
						else
						{
							outerInstance.scaleXYZCheckBox.setSelected(false);
						}
					}

					if ((this.checkBox == outerInstance.scaleXCheckBox || this.checkBox == outerInstance.scaleYCheckBox || this.checkBox == outerInstance.scaleZCheckBox) && this.checkBox.isSelected())
					{
						outerInstance.scaleXYZCheckBox.setSelected(false);
						if (outerInstance.scaleZCheckBox != this.checkBox)
						{
							outerInstance.scaleXYCheckBox.setSelected(false);
						}
					}
				}
			}
		}

		private I18N i18n = I18N.Instance;
		private static RankDefectDialog rankDefectDialog = new RankDefectDialog();
		private Dialog<RankDefect> dialog = null;
		private Window window;
		private CheckBox userDefinedCheckBox;
		private CheckBox translationXCheckBox, translationYCheckBox, translationZCheckBox;
		private CheckBox rotationXCheckBox, rotationYCheckBox, rotationZCheckBox;
		private CheckBox shearXCheckBox, shearYCheckBox, shearZCheckBox;
		private CheckBox scaleXCheckBox, scaleYCheckBox, scaleZCheckBox, scaleXYCheckBox, scaleXYZCheckBox;
		private Accordion accordion;
		private RankDefectDialog()
		{
		}

		public static Window Owner
		{
			set
			{
				rankDefectDialog.window = value;
			}
		}

		public static Optional<RankDefect> showAndWait()
		{
			rankDefectDialog.init();
			rankDefectDialog.load();
			rankDefectDialog.accordion.setExpandedPane(rankDefectDialog.accordion.getPanes().get(0));
			// @see https://bugs.openjdk.java.net/browse/JDK-8087458
			Platform.runLater(() =>
			{
			try
			{
				rankDefectDialog.dialog.getDialogPane().requestLayout();
				Stage stage = (Stage) rankDefectDialog.dialog.getDialogPane().getScene().getWindow();
				stage.sizeToScene();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			});
			return rankDefectDialog.dialog.showAndWait();
		}

		private void init()
		{
			if (this.dialog != null)
			{
				return;
			}

			this.dialog = new Dialog<RankDefect>();
			this.dialog.setTitle(i18n.getString("RankDefectDialog.title", "Rank defect"));
			this.dialog.setHeaderText(i18n.getString("RankDefectDialog.header", "User defined rank defect properties"));
			this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.OK, ButtonType.CANCEL);
			this.dialog.initModality(Modality.APPLICATION_MODAL);
			//		this.dialog.initStyle(StageStyle.UTILITY);
			this.dialog.initOwner(window);

			this.dialog.getDialogPane().setContent(this.createPane());
			this.dialog.setResizable(true);

			this.dialog.setResultConverter(new CallbackAnonymousInnerClass(this));

			this.dialog.setOnCloseRequest(new EventHandlerAnonymousInnerClass(this));
		}

		private class CallbackAnonymousInnerClass : Callback<ButtonType, RankDefect>
		{
			private readonly RankDefectDialog outerInstance;

			public CallbackAnonymousInnerClass(RankDefectDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override RankDefect call(ButtonType buttonType)
			{
				if (buttonType == ButtonType.OK)
				{
					RankDefect rankDefect = new RankDefect();
					if (outerInstance.userDefinedCheckBox.isSelected())
					{
						if (outerInstance.translationYCheckBox.isSelected())
						{
							rankDefect.TranslationYDefectType = DefectType.FREE;
						}

						if (outerInstance.translationXCheckBox.isSelected())
						{
							rankDefect.TranslationXDefectType = DefectType.FREE;
						}

						if (outerInstance.translationZCheckBox.isSelected())
						{
							rankDefect.TranslationZDefectType = DefectType.FREE;
						}

						if (outerInstance.rotationYCheckBox.isSelected())
						{
							rankDefect.RotationYDefectType = DefectType.FREE;
						}

						if (outerInstance.rotationXCheckBox.isSelected())
						{
							rankDefect.RotationXDefectType = DefectType.FREE;
						}

						if (outerInstance.rotationZCheckBox.isSelected())
						{
							rankDefect.RotationZDefectType = DefectType.FREE;
						}

						if (outerInstance.shearYCheckBox.isSelected())
						{
							rankDefect.ShearYDefectType = DefectType.FREE;
						}

						if (outerInstance.shearXCheckBox.isSelected())
						{
							rankDefect.ShearXDefectType = DefectType.FREE;
						}

						if (outerInstance.shearZCheckBox.isSelected())
						{
							rankDefect.ShearZDefectType = DefectType.FREE;
						}

						if (outerInstance.scaleYCheckBox.isSelected())
						{
							rankDefect.ScaleYDefectType = DefectType.FREE;
						}

						if (outerInstance.scaleXCheckBox.isSelected())
						{
							rankDefect.ScaleXDefectType = DefectType.FREE;
						}

						if (outerInstance.scaleZCheckBox.isSelected())
						{
							rankDefect.ScaleZDefectType = DefectType.FREE;
						}

						if (outerInstance.scaleXYCheckBox.isSelected())
						{
							rankDefect.ScaleXYDefectType = DefectType.FREE;
						}

						if (outerInstance.scaleXYZCheckBox.isSelected())
						{
							rankDefect.ScaleXYZDefectType = DefectType.FREE;
						}
					}
					outerInstance.save(rankDefect);
				}
				return null;
			}
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<DialogEvent>
		{
			private readonly RankDefectDialog outerInstance;

			public EventHandlerAnonymousInnerClass(RankDefectDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(DialogEvent @event)
			{
				outerInstance.accordion.setExpandedPane(outerInstance.accordion.getPanes().get(0));
			}
		}

		private Node createPane()
		{
			VBox box = this.createVbox();

			string labelUserDefined = i18n.getString("RankDefectDialog.userdefined.title", "User defined defect analysis");
			string tooltipUserDefined = i18n.getString("RankDefectDialog.userdefined.tooltip", "If checked, user defined condition equations will be applied (without examination) to free network adjustment");

			this.userDefinedCheckBox = this.createCheckBox(labelUserDefined, tooltipUserDefined);
			this.userDefinedCheckBox.selectedProperty().addListener(new ScaleSelectionChangeListener(this, this.userDefinedCheckBox));

			this.accordion = new Accordion();
			this.accordion.getPanes().addAll(this.createTranslationPane(), this.createRotationPane(), this.createScalePane(), this.createShearPane());
			this.accordion.setMaxSize(double.MaxValue, double.MaxValue);

			Platform.runLater(() =>
			{
			userDefinedCheckBox.requestFocus();
			});

			box.getChildren().addAll(this.userDefinedCheckBox, this.accordion);
			return box;
		}


		private TitledPane createTranslationPane()
		{
			string title = i18n.getString("RankDefectDialog.translation.title", "Translations");
			string tooltip = i18n.getString("RankDefectDialog.translation.tooltip", "Condition equation for network translations");

			string labelY = i18n.getString("RankDefectDialog.translation.y.label", "Translation y");
			string tooltipY = i18n.getString("RankDefectDialog.translation.y.tooltip", "If checked, condition equation for y-translation will be applied to free network adjustment");

			string labelX = i18n.getString("RankDefectDialog.translation.x.label", "Translation x");
			string tooltipX = i18n.getString("RankDefectDialog.translation.x.tooltip", "If checked, condition equation for x-translation will be applied to free network adjustment");

			string labelZ = i18n.getString("RankDefectDialog.translation.z.label", "Translation z");
			string tooltipZ = i18n.getString("RankDefectDialog.translation.z.tooltip", "If checked, condition equation for z-translation will be applied to free network adjustment");

			this.translationYCheckBox = this.createCheckBox(labelY, tooltipY);
			this.translationXCheckBox = this.createCheckBox(labelX, tooltipX);
			this.translationZCheckBox = this.createCheckBox(labelZ, tooltipZ);

			VBox box = createVbox();
			box.getChildren().addAll(this.translationYCheckBox, this.translationXCheckBox, this.translationZCheckBox);

			return this.createTitledPane(title, tooltip, box);
		}

		private TitledPane createRotationPane()
		{
			string title = i18n.getString("RankDefectDialog.rotation.title", "Rotations");
			string tooltip = i18n.getString("RankDefectDialog.rotation.tooltip", "Condition equation for network rotations");

			string labelY = i18n.getString("RankDefectDialog.rotation.y.label", "Rotation y");
			string tooltipY = i18n.getString("RankDefectDialog.rotation.y.tooltip", "If checked, condition equation for y-rotation will be applied to free network adjustment");

			string labelX = i18n.getString("RankDefectDialog.rotation.x.label", "Rotation x");
			string tooltipX = i18n.getString("RankDefectDialog.rotation.x.tooltip", "If checked, condition equation for x-rotation will be applied to free network adjustment");

			string labelZ = i18n.getString("RankDefectDialog.rotation.z.label", "Rotation z");
			string tooltipZ = i18n.getString("RankDefectDialog.rotation.z.tooltip", "If checked, condition equation for z-rotation will be applied to free network adjustment");

			this.rotationYCheckBox = this.createCheckBox(labelY, tooltipY);
			this.rotationXCheckBox = this.createCheckBox(labelX, tooltipX);
			this.rotationZCheckBox = this.createCheckBox(labelZ, tooltipZ);

			VBox box = createVbox();
			box.getChildren().addAll(this.rotationYCheckBox, this.rotationXCheckBox, this.rotationZCheckBox);

			return this.createTitledPane(title, tooltip, box);
		}

		private TitledPane createShearPane()
		{
			string title = i18n.getString("RankDefectDialog.shear.title", "Shears");
			string tooltip = i18n.getString("RankDefectDialog.shear.tooltip", "Condition equation for network shears (unusual conditions)");

			string labelY = i18n.getString("RankDefectDialog.shear.y.label", "Shear y");
			string tooltipY = i18n.getString("RankDefectDialog.shear.y.tooltip", "If checked, condition equation for y-shear will be applied to free network adjustment");

			string labelX = i18n.getString("RankDefectDialog.shear.x.label", "Shear x");
			string tooltipX = i18n.getString("RankDefectDialog.shear.x.tooltip", "If checked, condition equation for x-shear will be applied to free network adjustment");

			string labelZ = i18n.getString("RankDefectDialog.shear.z.label", "Shear z");
			string tooltipZ = i18n.getString("RankDefectDialog.shear.z.tooltip", "If checked, condition equation for z-shear will be applied to free network adjustment");

			this.shearYCheckBox = this.createCheckBox(labelY, tooltipY);
			this.shearXCheckBox = this.createCheckBox(labelX, tooltipX);
			this.shearZCheckBox = this.createCheckBox(labelZ, tooltipZ);

			VBox box = createVbox();
			box.getChildren().addAll(this.shearYCheckBox, this.shearXCheckBox, this.shearZCheckBox);

			return this.createTitledPane(title, tooltip, box);
		}

		private TitledPane createScalePane()
		{
			string title = i18n.getString("RankDefectDialog.scale.title", "Scales");
			string tooltip = i18n.getString("RankDefectDialog.scale.tooltip", "Condition equation for network scales");

			string labelY = i18n.getString("RankDefectDialog.scale.y.label", "Scale y");
			string tooltipY = i18n.getString("RankDefectDialog.scale.y.tooltip", "If checked, condition equation for y-scale will be applied to free network adjustment");

			string labelX = i18n.getString("RankDefectDialog.scale.x.label", "Scale x");
			string tooltipX = i18n.getString("RankDefectDialog.scale.x.tooltip", "If checked, condition equation for x-scale will be applied to free network adjustment");

			string labelZ = i18n.getString("RankDefectDialog.scale.z.label", "Scale z");
			string tooltipZ = i18n.getString("RankDefectDialog.scale.z.tooltip", "If checked, condition equation for z-scale will be applied to free network adjustment");

			string labelXY = i18n.getString("RankDefectDialog.scale.xy.label", "Scale y, x");
			string tooltipXY = i18n.getString("RankDefectDialog.scale.xy.tooltip", "If checked, condition equation for horizontal scale will be applied to free network adjustment");

			string labelXYZ = i18n.getString("RankDefectDialog.scale.xyz.label", "Scale y, x, z");
			string tooltipXYZ = i18n.getString("RankDefectDialog.scale.xyz.tooltip", "If checked, condition equation for spatial scale will be applied to free network adjustment");


			this.scaleYCheckBox = this.createCheckBox(labelY, tooltipY);
			this.scaleYCheckBox.selectedProperty().addListener(new ScaleSelectionChangeListener(this, this.scaleYCheckBox));

			this.scaleXCheckBox = this.createCheckBox(labelX, tooltipX);
			this.scaleXCheckBox.selectedProperty().addListener(new ScaleSelectionChangeListener(this, this.scaleXCheckBox));

			this.scaleZCheckBox = this.createCheckBox(labelZ, tooltipZ);
			this.scaleZCheckBox.selectedProperty().addListener(new ScaleSelectionChangeListener(this, this.scaleZCheckBox));

			this.scaleXYCheckBox = this.createCheckBox(labelXY, tooltipXY);
			this.scaleXYCheckBox.selectedProperty().addListener(new ScaleSelectionChangeListener(this, this.scaleXYCheckBox));

			this.scaleXYZCheckBox = this.createCheckBox(labelXYZ, tooltipXYZ);
			this.scaleXYZCheckBox.selectedProperty().addListener(new ScaleSelectionChangeListener(this, this.scaleXYZCheckBox));

			VBox leftBox = createVbox();
			leftBox.getChildren().addAll(this.scaleYCheckBox, this.scaleXCheckBox, this.scaleZCheckBox);

			VBox rightBox = createVbox();
			rightBox.getChildren().addAll(this.scaleXYCheckBox, this.scaleXYZCheckBox);

			HBox hbox = new HBox(0);
			hbox.setMaxSize(double.MaxValue, double.MaxValue);
			hbox.setPadding(new Insets(0, 0, 0, 0));
			hbox.getChildren().addAll(leftBox, rightBox);

			return this.createTitledPane(title, tooltip, hbox);
		}

		private TitledPane createTitledPane(string title, string tooltip, Node content)
		{
			TitledPane titledPane = new TitledPane();
			titledPane.setCollapsible(true);
			titledPane.setAnimated(true);
			titledPane.setContent(content);
			titledPane.setMaxSize(double.MaxValue, double.MaxValue);
			titledPane.setPadding(new Insets(0, 10, 5, 10)); // oben, links, unten, rechts
			//titledPane.setText(title);
			Label label = new Label(title);
			label.setTooltip(new Tooltip(tooltip));
			titledPane.setGraphic(label);
			return titledPane;
		}

		private CheckBox createCheckBox(string title, string tooltip)
		{
			Label label = new Label(title);
			label.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			label.setMaxSize(double.MaxValue, double.MaxValue);
			label.setPadding(new Insets(0,0,0,3));
			CheckBox checkBox = new CheckBox();
			checkBox.setGraphic(label);
			checkBox.setTooltip(new Tooltip(tooltip));
			checkBox.setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			checkBox.setMaxWidth(double.MaxValue);
			return checkBox;
		}

		private VBox createVbox()
		{
			VBox vBox = new VBox();
			vBox.setMaxSize(double.MaxValue, double.MaxValue);
			vBox.setPadding(new Insets(5, 10, 5, 10)); // oben, recht, unten, links
			vBox.setSpacing(10);
			return vBox;
		}

		private void disableCheckBoxes(bool disable)
		{
			this.translationYCheckBox.setDisable(disable);
			this.translationXCheckBox.setDisable(disable);
			this.translationZCheckBox.setDisable(disable);

			this.rotationYCheckBox.setDisable(disable);
			this.rotationXCheckBox.setDisable(disable);
			this.rotationZCheckBox.setDisable(disable);

			this.scaleYCheckBox.setDisable(disable);
			this.scaleXCheckBox.setDisable(disable);
			this.scaleZCheckBox.setDisable(disable);

			this.shearYCheckBox.setDisable(disable);
			this.shearXCheckBox.setDisable(disable);
			this.shearZCheckBox.setDisable(disable);

			this.scaleXYCheckBox.setDisable(disable);
			this.scaleXYZCheckBox.setDisable(disable);
		}

		private void save(RankDefect rankDefect)
		{
			try
			{
				bool userDefined = this.userDefinedCheckBox.isSelected();
				if (userDefined && rankDefect.Defect == 0) // if no defect is selected, use the automated detection
				{
					userDefined = false;
				}
				SQLManager.Instance.save(userDefined, rankDefect);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				Platform.runLater(() =>
				{
				OptionDialog.showThrowableDialog(i18n.getString("RankDefectDialog.message.error.save.exception.title", "Unexpected SQL-Error"), i18n.getString("RankDefectDialog.message.error.save.exception.header", "Error, could not save user-defined rank defect properties to database."), i18n.getString("RankDefectDialog.message.error.save.exception.message", "An exception has occurred during database transaction."), e);
				});
			}
		}

		private void load()
		{
			try
			{
				RankDefect rankDefect = SQLManager.Instance.RankDefectDefinition;
				this.userDefinedCheckBox.setSelected(rankDefect.UserDefinedRankDefect);
				this.disableCheckBoxes(!rankDefect.UserDefinedRankDefect);
				if (rankDefect.UserDefinedRankDefect)
				{
					this.translationYCheckBox.setSelected(rankDefect.estimateTranslationY());
					this.translationXCheckBox.setSelected(rankDefect.estimateTranslationX());
					this.translationZCheckBox.setSelected(rankDefect.estimateTranslationZ());

					this.rotationYCheckBox.setSelected(rankDefect.estimateRotationY());
					this.rotationXCheckBox.setSelected(rankDefect.estimateRotationX());
					this.rotationZCheckBox.setSelected(rankDefect.estimateRotationZ());

					this.shearYCheckBox.setSelected(rankDefect.estimateShearY());
					this.shearXCheckBox.setSelected(rankDefect.estimateShearX());
					this.shearZCheckBox.setSelected(rankDefect.estimateShearZ());

					this.scaleYCheckBox.setSelected(rankDefect.estimateScaleY());
					this.scaleXCheckBox.setSelected(rankDefect.estimateScaleX());
					this.scaleZCheckBox.setSelected(rankDefect.estimateScaleZ());

					this.scaleXYCheckBox.setSelected(rankDefect.estimateScaleXY());
					this.scaleXYZCheckBox.setSelected(rankDefect.estimateScaleXYZ());
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				Platform.runLater(() =>
				{
				OptionDialog.showThrowableDialog(i18n.getString("RankDefectDialog.message.error.load.exception.title", "Unexpected SQL-Error"), i18n.getString("RankDefectDialog.message.error.load.exception.header", "Error, could not load user-defined rank defect properties from database."), i18n.getString("RankDefectDialog.message.error.load.exception.message", "An exception has occurred during database transaction."), e);
				});
			}
		}
	}

}