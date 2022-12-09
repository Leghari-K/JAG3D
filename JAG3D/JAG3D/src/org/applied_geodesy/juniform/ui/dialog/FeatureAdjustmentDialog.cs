using System;
using System.Threading;

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

	using EstimationStateType = org.applied_geodesy.adjustment.EstimationStateType;
	using EstimationType = org.applied_geodesy.adjustment.EstimationType;
	using Feature = org.applied_geodesy.adjustment.geometry.Feature;
	using FeatureAdjustment = org.applied_geodesy.adjustment.geometry.FeatureAdjustment;
	using UIMenuBuilder = org.applied_geodesy.juniform.ui.menu.UIMenuBuilder;
	using UIParameterTableBuilder = org.applied_geodesy.juniform.ui.table.UIParameterTableBuilder;
	using UIPointTableBuilder = org.applied_geodesy.juniform.ui.table.UIPointTableBuilder;
	using UITreeBuilder = org.applied_geodesy.juniform.ui.tree.UITreeBuilder;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
	using I18N = org.applied_geodesy.juniform.ui.i18n.I18N;

	using Platform = javafx.application.Platform;
	using Task = javafx.concurrent.Task;
	using WorkerStateEvent = javafx.concurrent.WorkerStateEvent;
	using EventHandler = javafx.@event.EventHandler;
	using Insets = javafx.geometry.Insets;
	using Node = javafx.scene.Node;
	using ButtonType = javafx.scene.control.ButtonType;
	using Dialog = javafx.scene.control.Dialog;
	using DialogEvent = javafx.scene.control.DialogEvent;
	using Label = javafx.scene.control.Label;
	using ProgressIndicator = javafx.scene.control.ProgressIndicator;
	using GridPane = javafx.scene.layout.GridPane;
	using VBox = javafx.scene.layout.VBox;
	using Text = javafx.scene.text.Text;
	using Modality = javafx.stage.Modality;
	using Stage = javafx.stage.Stage;
	using Window = javafx.stage.Window;
	using WindowEvent = javafx.stage.WindowEvent;
	using Callback = javafx.util.Callback;
	using MatrixSingularException = no.uib.cipr.matrix.MatrixSingularException;
	using NotConvergedException = no.uib.cipr.matrix.NotConvergedException;

	public class FeatureAdjustmentDialog
	{

		private class AdjustmentTask : Task<EstimationStateType>, PropertyChangeListener
		{
			private readonly FeatureAdjustmentDialog outerInstance;

			internal readonly string iterationTextTemplate;
			internal readonly string convergenceTextTemplate;
			internal readonly string unscentedTransformationTextTemplate;
			internal FeatureAdjustment adjustment;
			internal double processState = 0.0;
			internal double finalStepProcesses = 0.0;
			internal bool updateProgressOnIterate = true;
			internal AdjustmentTask(FeatureAdjustmentDialog outerInstance, FeatureAdjustment adjustment)
			{
				this.outerInstance = outerInstance;
				this.adjustment = adjustment;

				this.unscentedTransformationTextTemplate = outerInstance.i18n.getString("FeatureAdjustmentDialog.unscentedtransformation.label", "%d. unscented transformation step of %d \u2026");
				this.iterationTextTemplate = outerInstance.i18n.getString("FeatureAdjustmentDialog.iteration.label", "%d. iteration step of maximal %d \u2026");
				this.convergenceTextTemplate = outerInstance.i18n.getString("FeatureAdjustmentDialog.convergence.label", "Convergence max|dx| = %.2e");
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override protected org.applied_geodesy.adjustment.EstimationStateType call() throws Exception
			protected internal override EstimationStateType call()
			{
				try
				{
					outerInstance.preventClosing = true;

					UIMenuBuilder.Instance.ReportMenuDisable = true;

					this.updateProgress(ProgressIndicator.INDETERMINATE_PROGRESS, ProgressIndicator.INDETERMINATE_PROGRESS);
					this.updateMessage(outerInstance.i18n.getString("FeatureAdjustmentDialog.initialize.label", "Initialize process\u2026"));
					this.updateIterationProgressMessage(outerInstance.i18n.getString("FeatureAdjustmentDialog.pleasewait.label", "Please wait\u2026"));
					this.updateConvergenceProgressMessage(null);

					this.updateProgressOnIterate = !(this.adjustment.EstimationType == EstimationType.SPHERICAL_SIMPLEX_UNSCENTED_TRANSFORMATION || this.adjustment.EstimationType == EstimationType.MODIFIED_UNSCENTED_TRANSFORMATION);
					this.finalStepProcesses = 0.25 / 3.0; // Nenner == Anzahl + 1 an zusaetzlichen Tasks (INVERT_NORMAL_EQUATION_MATRIX, ESTIAMTE_STOCHASTIC_PARAMETERS)
					this.adjustment.addPropertyChangeListener(this);

					this.processState = 0.0;
					this.updateProgress(this.processState, 1.0);

					if (this.isCancelled())
					{
						return EstimationStateType.INTERRUPT;
					}

					Feature feature = this.adjustment.Feature;

					// derive parameters for warm start of adjustment
					if (feature.EstimateInitialGuess)
					{
						feature.deriveInitialGuess();
					}

	//				// transfer inital guess to parameters to be estimated
	//				feature.applyInitialGuess();
	//				
	//				// reset center of mass
	//				if (feature.getCenterOfMass() != null) {
	//					feature.getCenterOfMass().setX0(0);
	//					feature.getCenterOfMass().setY0(0);
	//					feature.getCenterOfMass().setZ0(0);
	//				}
	//				
	//				// set center of mass
	//				if (feature.isEstimateCenterOfMass()) 
	//					feature.setCenterOfMass(Feature.deriveCenterOfMass(feature.getFeaturePoints()));

					this.adjustment.init();
					EstimationStateType returnType = this.adjustment.estimateModel();

					if (this.isCancelled())
					{
						return EstimationStateType.INTERRUPT;
					}

					this.updateProgress(ProgressIndicator.INDETERMINATE_PROGRESS, ProgressIndicator.INDETERMINATE_PROGRESS);
					this.updateMessage(outerInstance.i18n.getString("FeatureAdjustmentDialog.save.label", "Save results\u2026"));
					this.updateIterationProgressMessage(outerInstance.i18n.getString("FeatureAdjustmentDialog.pleasewait.label", "Please wait\u2026"));
					this.updateConvergenceProgressMessage(null);
					Platform.runLater(() =>
					{
					try
					{
						UIPointTableBuilder.Instance.Table.getSelectionModel().clearSelection();
						UIPointTableBuilder.Instance.Table.refresh();
						UIPointTableBuilder.Instance.Table.sort();
						UIParameterTableBuilder.Instance.Table.getSelectionModel().clearSelection();
						UIParameterTableBuilder.Instance.Table.refresh();
						UIParameterTableBuilder.Instance.Table.sort();
					}
					catch (Exception e)
					{
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
					}
					});
					return returnType;
				}
				finally
				{
					this.destroyFeatureAdjustment();
					this.updateIterationProgressMessage(null);
					this.updateConvergenceProgressMessage(null);

					outerInstance.preventClosing = false;
				}
			}

			protected internal override void succeeded()
			{
				outerInstance.preventClosing = false;
				base.succeeded();
				hideDialog();
			}

			protected internal override void failed()
			{
				outerInstance.preventClosing = false;
				base.failed();
				hideDialog();
			}

			protected internal override void cancelled()
			{
				base.cancelled();
				if (this.adjustment != null)
				{
					this.adjustment.interrupt();
				}
			}

			//		@Override
			//		protected void done() {	}

			internal virtual void hideDialog()
			{
				Platform.runLater(() =>
				{
				if (!outerInstance.preventClosing)
				{
					outerInstance.dialog.hide();
				}
				});
			}

			internal virtual void updateIterationProgressMessage(string message)
			{
				Platform.runLater(() =>
				{
				outerInstance.iterationLabel.setText(message);
				});
			}

			internal virtual void updateConvergenceProgressMessage(string message)
			{
				Platform.runLater(() =>
				{
				outerInstance.progressLabel.setText(message);
				});
			}

			protected internal override void updateMessage(string message)
			{
				base.updateMessage(message);
				Platform.runLater(() =>
				{
				outerInstance.dialog.setHeaderText(message);
				});
			}

			protected internal override void updateProgress(double workDone, double max)
			{
				base.updateProgress(workDone, max);
				Platform.runLater(() =>
				{
				outerInstance.progressIndicator.setProgress(workDone);
				if (workDone >= 1.0)
				{
					Node node = outerInstance.progressIndicator.lookup(".percentage");
					if (node != null && node is Text)
					{
						Text text = (Text)node;
						text.setText(outerInstance.i18n.getString("FeatureAdjustmentDialog.done.label", "Done"));
						outerInstance.progressIndicator.setPrefWidth(text.getLayoutBounds().getWidth());
					}
				}
				});
			}

			internal virtual void destroyFeatureAdjustment()
			{
				if (this.adjustment != null)
				{
					this.adjustment.removePropertyChangeListener(this);
					this.adjustment = null;
				}
			}

			public override void propertyChange(PropertyChangeEvent evt)
			{
				string name = evt.getPropertyName();

				EstimationStateType state = EstimationStateType.valueOf(name);
				if (state == null)
				{
					return;
				}

				object oldValue = evt.getOldValue();
				object newValue = evt.getNewValue();

				switch (state.innerEnumValue)
				{
				case EstimationStateType.InnerEnum.BUSY:
					this.updateMessage(outerInstance.i18n.getString("FeatureAdjustmentDialog.busy.label", "Feature adjustment in process\u2026"));
					this.updateIterationProgressMessage(null);
					this.updateConvergenceProgressMessage(null);
					break;

				case EstimationStateType.InnerEnum.CONVERGENCE:
					if (oldValue != null && newValue != null && oldValue is double? && newValue is Double)
					{
						double current = ((double?)newValue).Value;
						double minimal = ((double?)oldValue).Value;
						if (this.updateProgressOnIterate)
						{
							double frac = 0.75 * Math.Min(minimal / current, 1.0);
							this.processState = Math.Max(this.processState, frac);
							this.updateProgress(this.processState, 1.0);
						}
						this.updateConvergenceProgressMessage(String.format(Locale.ENGLISH, this.convergenceTextTemplate, current, minimal));
					}
					break;

				case EstimationStateType.InnerEnum.ITERATE:
					if (oldValue != null && newValue != null && oldValue is int? && newValue is Integer)
					{
						int current = ((int?)newValue).Value;
						int maximal = ((int?)oldValue).Value;
						if (this.updateProgressOnIterate)
						{
							double frac = 0.75 * Math.Min((double)current / (double)maximal, 1.0);
							this.processState = Math.Max(this.processState, frac);
							this.updateProgress(this.processState, 1.0);
						}
						this.updateIterationProgressMessage(String.format(Locale.ENGLISH, this.iterationTextTemplate, current, maximal));
					}
					break;

				case EstimationStateType.InnerEnum.UNSCENTED_TRANSFORMATION_STEP:
					if (oldValue != null && newValue != null && oldValue is int? && newValue is Integer)
					{
						int current = ((int?)newValue).Value;
						int maximal = ((int?)oldValue).Value;
						double frac = 0.75 * Math.Min((double)current / (double)maximal, 1.0);
						this.processState = Math.Max(this.processState, frac);
						this.updateMessage(String.format(Locale.ENGLISH, this.unscentedTransformationTextTemplate, current, maximal));
						this.updateProgress(this.processState, 1.0);
					}
					break;

				case EstimationStateType.InnerEnum.INVERT_NORMAL_EQUATION_MATRIX:
					this.processState += this.finalStepProcesses;
					this.updateProgress(this.processState, 1.0);
					this.updateMessage(outerInstance.i18n.getString("FeatureAdjustmentDialog.invert_normal_equation_matrix.label", "Invert normal equation matrix\u2026"));
					break;

				case EstimationStateType.InnerEnum.ESTIAMTE_STOCHASTIC_PARAMETERS:
					this.processState += this.finalStepProcesses;
					this.updateProgress(this.processState, 1.0);
					this.updateMessage(outerInstance.i18n.getString("FeatureAdjustmentDialog.estimate_stochastic_parameters.label", "Estimate stochastic parameters\u2026"));
					break;

				case EstimationStateType.InnerEnum.ERROR_FREE_ESTIMATION:
					this.updateProgress(ProgressIndicator.INDETERMINATE_PROGRESS, ProgressIndicator.INDETERMINATE_PROGRESS);
					this.updateMessage(outerInstance.i18n.getString("FeatureAdjustmentDialog.error_free_estimation.label", "Feature adjustment finished\u2026"));
					break;

				case EstimationStateType.InnerEnum.INTERRUPT:
					this.updateMessage(outerInstance.i18n.getString("FeatureAdjustmentDialog.interrupt.label", "Terminate adjustment process\u2026"));
					this.updateIterationProgressMessage(outerInstance.i18n.getString("FeatureAdjustmentDialog.pleasewait.label", "Please wait\u2026"));
					this.updateConvergenceProgressMessage(null);
					break;

				// unused cases
				case EstimationStateType.InnerEnum.PRINCIPAL_COMPONENT_ANALYSIS:
				case EstimationStateType.InnerEnum.EXPORT_COVARIANCE_MATRIX:
				case EstimationStateType.InnerEnum.EXPORT_COVARIANCE_INFORMATION:
					break;

				// adjustment failed (exception) 
				case EstimationStateType.InnerEnum.NOT_INITIALISED:
				case EstimationStateType.InnerEnum.NO_CONVERGENCE:
				case EstimationStateType.InnerEnum.OUT_OF_MEMORY:
				case EstimationStateType.InnerEnum.ROBUST_ESTIMATION_FAILED:
				case EstimationStateType.InnerEnum.SINGULAR_MATRIX:
					break;

				}
			}
		}

		private static FeatureAdjustmentDialog adjustmentDialog = new FeatureAdjustmentDialog();
		private Window window;
		private AdjustmentTask adjustmentTask;
		private bool preventClosing = false;
		private I18N i18n = I18N.Instance;
		private Dialog<EstimationStateType> dialog = null;
		private ProgressIndicator progressIndicator = new ProgressIndicator(ProgressIndicator.INDETERMINATE_PROGRESS);
		private Label iterationLabel = new Label();
		private Label progressLabel = new Label();
		private EstimationStateType result;

		private FeatureAdjustmentDialog()
		{
		}

		public static void show()
		{
			adjustmentDialog.init();
			adjustmentDialog.reset();

			adjustmentDialog.dialog.show();
			adjustmentDialog.process();

			// @see https://bugs.openjdk.java.net/browse/JDK-8087458
			Platform.runLater(() =>
			{
			try
			{
				adjustmentDialog.dialog.getDialogPane().requestLayout();
				Stage stage = (Stage) adjustmentDialog.dialog.getDialogPane().getScene().getWindow();
				stage.sizeToScene();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			});
		}

		public static Window Owner
		{
			set
			{
				adjustmentDialog.window = value;
			}
		}

		private void init()
		{
			if (this.dialog != null)
			{
				return;
			}

			this.dialog = new Dialog<EstimationStateType>();
			this.dialog.initOwner(window);
			this.dialog.setTitle(i18n.getString("FeatureAdjustmentDialog.title", "Feature adjustment"));
			this.dialog.setHeaderText(i18n.getString("FeatureAdjustmentDialog.header", "Feature adjustment is processing\u2026"));
			this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.CANCEL);
			this.dialog.initModality(Modality.APPLICATION_MODAL);
			VBox vbox = new VBox();
			vbox.getChildren().addAll(this.createProgressPane());

			this.dialog.getDialogPane().getScene().getWindow().setOnCloseRequest(new EventHandlerAnonymousInnerClass(this));

			this.dialog.setOnCloseRequest(new EventHandlerAnonymousInnerClass2(this));

			this.dialog.setResultConverter(new CallbackAnonymousInnerClass(this));

			this.dialog.getDialogPane().setContent(vbox);
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<WindowEvent>
		{
			private readonly FeatureAdjustmentDialog outerInstance;

			public EventHandlerAnonymousInnerClass(FeatureAdjustmentDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(WindowEvent @event)
			{
				if (outerInstance.preventClosing)
				{
					@event.consume();
					if (outerInstance.adjustmentTask != null)
					{
						outerInstance.adjustmentTask.cancelled();
					}
				}
			}
		}

		private class EventHandlerAnonymousInnerClass2 : EventHandler<DialogEvent>
		{
			private readonly FeatureAdjustmentDialog outerInstance;

			public EventHandlerAnonymousInnerClass2(FeatureAdjustmentDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(DialogEvent @event)
			{
				if (outerInstance.preventClosing)
				{
					@event.consume();
					if (outerInstance.adjustmentTask != null)
					{
						outerInstance.adjustmentTask.cancelled();
					}
				}
			}
		}

		private class CallbackAnonymousInnerClass : Callback<ButtonType, EstimationStateType>
		{
			private readonly FeatureAdjustmentDialog outerInstance;

			public CallbackAnonymousInnerClass(FeatureAdjustmentDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override EstimationStateType call(ButtonType buttonType)
			{
				return outerInstance.result;
			}
		}

		private void reset()
		{
			this.progressIndicator.setProgress(ProgressIndicator.INDETERMINATE_PROGRESS);
			this.iterationLabel.setText(null);
			this.progressLabel.setText(null);
			this.preventClosing = false;
			this.result = EstimationStateType.NOT_INITIALISED;
		}

		private Node createProgressPane()
		{
			GridPane gridPane = new GridPane();
			gridPane.setMinWidth(400);
			gridPane.setHgap(20);
			gridPane.setVgap(15);
			gridPane.setPadding(new Insets(5, 10, 5, 10)); // oben, recht, unten, links

			this.progressIndicator.setMinWidth(50);
			this.progressIndicator.setMinHeight(50);

			gridPane.add(this.progressIndicator, 0, 0, 1, 3); // col, row, colspan, rowspan
			gridPane.add(this.iterationLabel, 1, 0);
			gridPane.add(this.progressLabel, 1, 1);

			return gridPane;
		}

		private void process()
		{
			this.reset();
			FeatureAdjustment adjustment = UITreeBuilder.Instance.FeatureAdjustment;
			this.adjustmentTask = new AdjustmentTask(this, adjustment);
			this.adjustmentTask.setOnSucceeded(new EventHandlerAnonymousInnerClass3(this));

			this.adjustmentTask.setOnFailed(new EventHandlerAnonymousInnerClass4(this));

			Thread th = new Thread(this.adjustmentTask);
			th.setDaemon(true);
			th.Start();
		}

		private class EventHandlerAnonymousInnerClass3 : EventHandler<WorkerStateEvent>
		{
			private readonly FeatureAdjustmentDialog outerInstance;

			public EventHandlerAnonymousInnerClass3(FeatureAdjustmentDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(WorkerStateEvent @event)
			{
				outerInstance.result = outerInstance.adjustmentTask.getValue();
				UIMenuBuilder.Instance.ReportMenuDisable = false;
				UITreeBuilder.Instance.handleTreeSelections();
			}
		}

		private class EventHandlerAnonymousInnerClass4 : EventHandler<WorkerStateEvent>
		{
			private readonly FeatureAdjustmentDialog outerInstance;

			public EventHandlerAnonymousInnerClass4(FeatureAdjustmentDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(WorkerStateEvent @event)
			{
				outerInstance.preventClosing = false;
				outerInstance.dialog.hide();
				Exception throwable = outerInstance.adjustmentTask.getException();
				if (throwable != null)
				{
					if (throwable is NotConvergedException)
					{
						Platform.runLater(() =>
						{
						OptionDialog.showThrowableDialog(outerInstance.i18n.getString("FeatureAdjustmentDialog.message.error.failed.noconvergence.title", "Feature adjustment failed"), outerInstance.i18n.getString("FeatureAdjustmentDialog.message.error.failed.noconvergence.header", "Iteration process diverges"), outerInstance.i18n.getString("FeatureAdjustmentDialog.message.error.failed.noconvergence.message", "Error, iteration limit of adjustment process reached but without satisfactory convergence."), throwable);
						});
					}

					else if (throwable is MatrixSingularException)
					{
						Platform.runLater(() =>
						{
						OptionDialog.showThrowableDialog(outerInstance.i18n.getString("FeatureAdjustmentDialog.message.error.failed.singularmatrix.title", "Feature adjustment failed"), outerInstance.i18n.getString("FeatureAdjustmentDialog.message.error.failed.singularmatrix.header", "Singular normal euqation matrix"), outerInstance.i18n.getString("FeatureAdjustmentDialog.message.error.failed.singularmatrix.message", "Error, could not invert normal equation matrix."), throwable);
						});
					}

					else if (throwable is System.OutOfMemoryException)
					{
						Platform.runLater(() =>
						{
						OptionDialog.showThrowableDialog(outerInstance.i18n.getString("FeatureAdjustmentDialog.message.error.failed.outofmemory.title", "Feature adjustment failed"), outerInstance.i18n.getString("FeatureAdjustmentDialog.message.error.failed.outofmemory.header", "Out of memory"), outerInstance.i18n.getString("FeatureAdjustmentDialog.message.error.failed.outofmemory.message", "Error, not enough memory to adjust feature. Please allocate more memory."), throwable);
						});
					}

					else
					{
						Platform.runLater(() =>
						{
						OptionDialog.showThrowableDialog(outerInstance.i18n.getString("FeatureAdjustmentDialog.message.error.failed.exception.title", "Feature adjustment failed"), outerInstance.i18n.getString("FeatureAdjustmentDialog.message.error.failed.exception.header", "Error, could not adjust feature."), outerInstance.i18n.getString("FeatureAdjustmentDialog.message.error.failed.exception.message", "An exception has occurred during feature adjustment."), throwable);
						});
					}
					Console.WriteLine(throwable.ToString());
					Console.Write(throwable.StackTrace);
				}
				try
				{
					UIPointTableBuilder.Instance.Table.getSelectionModel().clearSelection();
					UIPointTableBuilder.Instance.Table.refresh();
					UIPointTableBuilder.Instance.Table.sort();
					UIParameterTableBuilder.Instance.Table.getSelectionModel().clearSelection();
					UIParameterTableBuilder.Instance.Table.refresh();
					UIParameterTableBuilder.Instance.Table.sort();
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
		}
	}

}