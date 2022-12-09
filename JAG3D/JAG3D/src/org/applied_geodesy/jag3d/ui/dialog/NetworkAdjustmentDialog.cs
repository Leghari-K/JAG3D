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

namespace org.applied_geodesy.jag3d.ui.dialog
{

	using EstimationStateType = org.applied_geodesy.adjustment.EstimationStateType;
	using EstimationType = org.applied_geodesy.adjustment.EstimationType;
	using NetworkAdjustment = org.applied_geodesy.adjustment.network.NetworkAdjustment;
	using IllegalProjectionPropertyException = org.applied_geodesy.adjustment.network.sql.IllegalProjectionPropertyException;
	using SQLAdjustmentManager = org.applied_geodesy.adjustment.network.sql.SQLAdjustmentManager;
	using PointTypeMismatchException = org.applied_geodesy.jag3d.sql.PointTypeMismatchException;
	using SQLManager = org.applied_geodesy.jag3d.sql.SQLManager;
	using UnderDeterminedPointException = org.applied_geodesy.jag3d.sql.UnderDeterminedPointException;
	using UITreeBuilder = org.applied_geodesy.jag3d.ui.tree.UITreeBuilder;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;
	using DatabaseVersionMismatchException = org.applied_geodesy.version.jag3d.DatabaseVersionMismatchException;

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

	public class NetworkAdjustmentDialog
	{

		private class AdjustmentTask : Task<EstimationStateType>, PropertyChangeListener
		{
			private readonly NetworkAdjustmentDialog outerInstance;

			internal readonly string iterationTextTemplate;
			internal readonly string convergenceTextTemplate;
			internal readonly string unscentedTransformationTextTemplate;
			internal NetworkAdjustment adjustment;
			internal double processState = 0.0;
			internal double finalStepProcesses = 0.0;
			internal SQLAdjustmentManager dataBaseManager;
			internal bool updateProgressOnIterate = true;
			internal AdjustmentTask(NetworkAdjustmentDialog outerInstance, SQLAdjustmentManager dataBaseManager)
			{
				this.outerInstance = outerInstance;
				this.dataBaseManager = dataBaseManager;

				this.unscentedTransformationTextTemplate = outerInstance.i18n.getString("NetworkAdjustmentDialog.unscentedtransformation.label", "%d. unscented transformation step of %d \u2026");
				this.iterationTextTemplate = outerInstance.i18n.getString("NetworkAdjustmentDialog.iteration.label", "%d. iteration step of maximal %d \u2026");
				this.convergenceTextTemplate = outerInstance.i18n.getString("NetworkAdjustmentDialog.convergence.label", "Convergence max|dx| = %.2e");
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override protected org.applied_geodesy.adjustment.EstimationStateType call() throws Exception
			protected internal override EstimationStateType call()
			{
				try
				{
					outerInstance.preventClosing = true;

					this.updateProgress(ProgressIndicator.INDETERMINATE_PROGRESS, ProgressIndicator.INDETERMINATE_PROGRESS);
					this.updateMessage(outerInstance.i18n.getString("NetworkAdjustmentDialog.initialize.label", "Initialize process\u2026"));
					this.updateIterationProgressMessage(outerInstance.i18n.getString("NetworkAdjustmentDialog.pleasewait.label", "Please wait\u2026"));
					this.updateConvergenceProgressMessage(null);

					SQLManager.Instance.checkNumberOfObersvationsPerUnknownParameter();

					this.adjustment = this.dataBaseManager.NetworkAdjustment;
					this.updateProgressOnIterate = !(this.adjustment.EstimationType == EstimationType.SPHERICAL_SIMPLEX_UNSCENTED_TRANSFORMATION || this.adjustment.EstimationType == EstimationType.MODIFIED_UNSCENTED_TRANSFORMATION);
					this.finalStepProcesses = 0.25 / (this.adjustment.hasCovarianceExportPathAndBaseName() ? 6.0 : 4.0);
					this.adjustment.addPropertyChangeListener(this);

					this.processState = 0.0;
					this.updateProgress(this.processState, 1.0);

					if (this.isCancelled())
					{
						return EstimationStateType.INTERRUPT;
					}

					EstimationStateType returnType = this.adjustment.estimateModel();

					if (this.isCancelled())
					{
						return EstimationStateType.INTERRUPT;
					}

					this.updateProgress(ProgressIndicator.INDETERMINATE_PROGRESS, ProgressIndicator.INDETERMINATE_PROGRESS);
					this.updateMessage(outerInstance.i18n.getString("NetworkAdjustmentDialog.save.label", "Save results\u2026"));
					this.updateIterationProgressMessage(outerInstance.i18n.getString("NetworkAdjustmentDialog.pleasewait.label", "Please wait\u2026"));
					this.updateConvergenceProgressMessage(null);
					this.dataBaseManager.saveResults();
					this.dataBaseManager.clear();
					return returnType;
				}
				finally
				{
					this.destroyNetworkAdjustment();
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
						text.setText(outerInstance.i18n.getString("NetworkAdjustmentDialog.done.label", "Done"));
						outerInstance.progressIndicator.setPrefWidth(text.getLayoutBounds().getWidth());
					}
				}
				});
			}

			internal virtual void destroyNetworkAdjustment()
			{
				if (this.adjustment != null)
				{
					this.adjustment.removePropertyChangeListener(this);
					this.adjustment.clearMatrices();
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
					this.updateMessage(outerInstance.i18n.getString("NetworkAdjustmentDialog.busy.label", "Network adjustment in process\u2026"));
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
					this.updateMessage(outerInstance.i18n.getString("NetworkAdjustmentDialog.invert_normal_equation_matrix.label", "Invert normal equation matrix\u2026"));
					break;

				case EstimationStateType.InnerEnum.ESTIAMTE_STOCHASTIC_PARAMETERS:
					this.processState += this.finalStepProcesses;
					this.updateProgress(this.processState, 1.0);
					this.updateMessage(outerInstance.i18n.getString("NetworkAdjustmentDialog.estimate_stochastic_parameters.label", "Estimate stochastic parameters\u2026"));
					break;

				case EstimationStateType.InnerEnum.PRINCIPAL_COMPONENT_ANALYSIS:
					this.processState += this.finalStepProcesses;
					this.updateProgress(this.processState, 1.0);
					this.updateMessage(outerInstance.i18n.getString("NetworkAdjustmentDialog.principal_component_analysis.label", "Principal component analysis\u2026"));
					break;

				case EstimationStateType.InnerEnum.EXPORT_COVARIANCE_MATRIX:
				case EstimationStateType.InnerEnum.EXPORT_COVARIANCE_INFORMATION:
					this.processState += this.finalStepProcesses;
					this.updateProgress(this.processState, 1.0);
					this.updateMessage(outerInstance.i18n.getString("NetworkAdjustmentDialog.export_covariance_matrix.label", "Export covariance matrix\u2026"));
					if (newValue != null)
					{
						this.updateIterationProgressMessage(newValue.ToString());
						this.updateConvergenceProgressMessage(null);
					}
					break;

				case EstimationStateType.InnerEnum.ERROR_FREE_ESTIMATION:
					this.updateProgress(1.0, 1.0);
					this.updateMessage(outerInstance.i18n.getString("NetworkAdjustmentDialog.error_free_estimation.label", "Network adjustment finished\u2026"));
					break;

				case EstimationStateType.InnerEnum.INTERRUPT:
					this.updateMessage(outerInstance.i18n.getString("NetworkAdjustmentDialog.interrupt.label", "Terminate adjustment process\u2026"));
					this.updateIterationProgressMessage(outerInstance.i18n.getString("NetworkAdjustmentDialog.pleasewait.label", "Please wait\u2026"));
					this.updateConvergenceProgressMessage(null);
					break;

					// Adjustment faild (wo exception) 
				case EstimationStateType.InnerEnum.NOT_INITIALISED:
				case EstimationStateType.InnerEnum.NO_CONVERGENCE:
				case EstimationStateType.InnerEnum.OUT_OF_MEMORY:
				case EstimationStateType.InnerEnum.ROBUST_ESTIMATION_FAILED:
				case EstimationStateType.InnerEnum.SINGULAR_MATRIX:
					break;

				}
			}
		}

		private static NetworkAdjustmentDialog adjustmentDialog = new NetworkAdjustmentDialog();
		private Window window;
		private AdjustmentTask adjustmentTask;
		private bool preventClosing = false;
		private I18N i18n = I18N.Instance;
		private Dialog<Void> dialog = null;
		private ProgressIndicator progressIndicator = new ProgressIndicator(ProgressIndicator.INDETERMINATE_PROGRESS);
		private Label iterationLabel = new Label();
		private Label progressLabel = new Label();

		private NetworkAdjustmentDialog()
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

			this.dialog = new Dialog<Void>();
			this.dialog.initOwner(window);
			this.dialog.setTitle(i18n.getString("NetworkAdjustmentDialog.title", "Network adjustment"));
			this.dialog.setHeaderText(i18n.getString("NetworkAdjustmentDialog.header", "Network adjustment is processing\u2026"));
			this.dialog.getDialogPane().getButtonTypes().addAll(ButtonType.CANCEL);
			this.dialog.initModality(Modality.APPLICATION_MODAL);
			//		this.dialog.initStyle(StageStyle.UTILITY);
			VBox vbox = new VBox();
			vbox.getChildren().addAll(this.createProgressPane());

			this.dialog.getDialogPane().getScene().getWindow().setOnCloseRequest(new EventHandlerAnonymousInnerClass(this));

			this.dialog.setOnCloseRequest(new EventHandlerAnonymousInnerClass2(this));

			this.dialog.getDialogPane().setContent(vbox);

			//		this.dialog.setResultConverter(new Callback<ButtonType, EstimationStateType>() {
			//			@Override
			//			public EstimationStateType call(ButtonType buttonType) {
			//				EstimationStateType returnType = null;
			//				if (buttonType == ButtonType.CANCEL) {
			//					if (adjustmentTask != null) {
			//						adjustmentTask.cancelled();
			//						returnType = adjustmentTask.getValue();
			//					}
			//				}
			//				return returnType;
			//			}
			//		});
		}

		private class EventHandlerAnonymousInnerClass : EventHandler<WindowEvent>
		{
			private readonly NetworkAdjustmentDialog outerInstance;

			public EventHandlerAnonymousInnerClass(NetworkAdjustmentDialog outerInstance)
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
			private readonly NetworkAdjustmentDialog outerInstance;

			public EventHandlerAnonymousInnerClass2(NetworkAdjustmentDialog outerInstance)
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

		private void reset()
		{
			this.progressIndicator.setProgress(ProgressIndicator.INDETERMINATE_PROGRESS);
			this.iterationLabel.setText(null);
			this.progressLabel.setText(null);
			this.preventClosing = false;
		}

		private Node createProgressPane()
		{
			GridPane gridPane = new GridPane();
			gridPane.setMinWidth(400);
			gridPane.setHgap(20);
			gridPane.setVgap(15);
			gridPane.setPadding(new Insets(5, 10, 5, 10)); // oben, links, unten, rechts

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

			this.adjustmentTask = new AdjustmentTask(this, SQLManager.Instance.AdjustmentManager);
			this.adjustmentTask.setOnSucceeded(new EventHandlerAnonymousInnerClass3(this));

			this.adjustmentTask.setOnFailed(new EventHandlerAnonymousInnerClass4(this));

			Thread th = new Thread(this.adjustmentTask);
			th.setDaemon(true);
			th.Start();
		}

		private class EventHandlerAnonymousInnerClass3 : EventHandler<WorkerStateEvent>
		{
			private readonly NetworkAdjustmentDialog outerInstance;

			public EventHandlerAnonymousInnerClass3(NetworkAdjustmentDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(WorkerStateEvent @event)
			{
				EstimationStateType result = outerInstance.adjustmentTask.getValue();
				if (result != null)
				{
					switch (result.innerEnumValue)
					{
					case EstimationStateType.InnerEnum.NO_CONVERGENCE:
					case EstimationStateType.InnerEnum.ROBUST_ESTIMATION_FAILED:
						OptionDialog.showErrorDialog(outerInstance.i18n.getString("NetworkAdjustmentDialog.message.error.failed.noconvergence.title", "Network adjustment failed"), outerInstance.i18n.getString("NetworkAdjustmentDialog.message.error.failed.noconvergence.header", "Iteration process diverges"), outerInstance.i18n.getString("NetworkAdjustmentDialog.message.error.failed.noconvergence.message", "Error, iteration limit of adjustment process reached but without satisfactory convergence."));
						break;

					case EstimationStateType.InnerEnum.NOT_INITIALISED:
					case EstimationStateType.InnerEnum.SINGULAR_MATRIX:
						OptionDialog.showErrorDialog(outerInstance.i18n.getString("NetworkAdjustmentDialog.message.error.failed.singularmatrix.title", "Network adjustment failed"), outerInstance.i18n.getString("NetworkAdjustmentDialog.message.error.failed.singularmatrix.header", "Singular normal euqation matrix"), outerInstance.i18n.getString("NetworkAdjustmentDialog.message.error.failed.singularmatrix.message", "Error, could not invert normal equation matrix."));
						break;

					case EstimationStateType.InnerEnum.OUT_OF_MEMORY:
						OptionDialog.showErrorDialog(outerInstance.i18n.getString("NetworkAdjustmentDialog.message.error.failed.outofmemory.title", "Network adjustment failed"), outerInstance.i18n.getString("NetworkAdjustmentDialog.message.error.failed.outofmemory.header", "Out of memory"), outerInstance.i18n.getString("NetworkAdjustmentDialog.message.error.failed.outofmemory.message", "Error, not enough memory to adjust network. Please allocate more memory."));
						break;

					default:
		//						System.out.println(NetworkAdjustmentDialog.class.getSimpleName() + " Fishied " + result);
						break;
					}
				}
				UITreeBuilder.Instance.handleTreeSelections();
			}
		}

		private class EventHandlerAnonymousInnerClass4 : EventHandler<WorkerStateEvent>
		{
			private readonly NetworkAdjustmentDialog outerInstance;

			public EventHandlerAnonymousInnerClass4(NetworkAdjustmentDialog outerInstance)
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
					if (throwable is PointTypeMismatchException)
					{
						Platform.runLater(() =>
						{
						OptionDialog.showThrowableDialog(outerInstance.i18n.getString("NetworkAdjustmentDialog.message.error.pointtypemismatch.exception.title", "Initialization error"), outerInstance.i18n.getString("NetworkAdjustmentDialog.message.error.pointtypemismatch.exception.header", "Error, the project contains uncombinable point typs, i.e. datum points as well as reference or stochastic points."), outerInstance.i18n.getString("NetworkAdjustmentDialog.message.error.pointtypemismatch.exception.message", "An exception has occurred during network adjustment."), throwable);
						});
					}
					else if (throwable is UnderDeterminedPointException)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.applied_geodesy.jag3d.sql.UnderDeterminedPointException e = (org.applied_geodesy.jag3d.sql.UnderDeterminedPointException)throwable;
						UnderDeterminedPointException e = (UnderDeterminedPointException)throwable;
						Platform.runLater(() =>
						{
						OptionDialog.showThrowableDialog(outerInstance.i18n.getString("NetworkAdjustmentDialog.message.error.underdeterminded.exception.title", "Initialization error"), String.format(outerInstance.i18n.getString("NetworkAdjustmentDialog.message.error.underdeterminded.exception.header", "Error, the point %s of dimension %d has only %d observations and is indeterminable."), e.PointName, e.Dimension, e.NumberOfObservations), outerInstance.i18n.getString("NetworkAdjustmentDialog.message.error.underdeterminded.exception.message", "An exception has occurred during network adjustment."), throwable);
						});
					}
					else if (throwable is IllegalProjectionPropertyException)
					{
						Platform.runLater(() =>
						{
						OptionDialog.showThrowableDialog(outerInstance.i18n.getString("NetworkAdjustmentDialog.message.error.projection.exception.title", "Initialization error"), outerInstance.i18n.getString("NetworkAdjustmentDialog.message.error.projection.exception.header", "Error, the project contains unsupported projection properties."), outerInstance.i18n.getString("NetworkAdjustmentDialog.message.error.projection.exception.message", "An exception has occurred during network adjustment."), throwable);
						});
					}
					else if (throwable is DatabaseVersionMismatchException)
					{
						Platform.runLater(() =>
						{
						OptionDialog.showThrowableDialog(outerInstance.i18n.getString("NetworkAdjustmentDialog.message.error.databaseversion.exception.title", "Version error"), outerInstance.i18n.getString("NetworkAdjustmentDialog.message.error.databaseversion.exception.header", "Error, the database version is unsupported."), outerInstance.i18n.getString("NetworkAdjustmentDialog.message.error.databaseversion.exception.message", "An exception has occurred during network adjustment."), throwable);
						});
					}
					else
					{
						Platform.runLater(() =>
						{
						OptionDialog.showThrowableDialog(outerInstance.i18n.getString("NetworkAdjustmentDialog.message.error.failed.exception.title", "Network adjustment failed"), outerInstance.i18n.getString("NetworkAdjustmentDialog.message.error.failed.exception.header", "Error, could not adjust network."), outerInstance.i18n.getString("NetworkAdjustmentDialog.message.error.failed.exception.message", "An exception has occurred during network adjustment."), throwable);
						});
					}
					Console.WriteLine(throwable.ToString());
					Console.Write(throwable.StackTrace);
				}
				UITreeBuilder.Instance.Tree.getSelectionModel().select(0);
			}
		}
	}

}