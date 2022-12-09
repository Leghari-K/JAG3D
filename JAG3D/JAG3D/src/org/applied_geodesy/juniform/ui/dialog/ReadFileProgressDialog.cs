using System;
using System.Collections.Generic;
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

	using UIParameterTableBuilder = org.applied_geodesy.juniform.ui.table.UIParameterTableBuilder;
	using UIPointTableBuilder = org.applied_geodesy.juniform.ui.table.UIPointTableBuilder;
	using UITreeBuilder = org.applied_geodesy.juniform.ui.tree.UITreeBuilder;
	using OptionDialog = org.applied_geodesy.ui.dialog.OptionDialog;
	using I18N = org.applied_geodesy.juniform.ui.i18n.I18N;
	using FileProgressChangeListener = org.applied_geodesy.util.io.FileProgressChangeListener;
	using FileProgressEvent = org.applied_geodesy.util.io.FileProgressEvent;
	using org.applied_geodesy.util.io;
	using FileProgressEventType = org.applied_geodesy.util.io.FileProgressEvent.FileProgressEventType;

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

	public class ReadFileProgressDialog<T>
	{

		private class ReadFileTask : Task<T>, FileProgressChangeListener
		{
			private readonly ReadFileProgressDialog<T> outerInstance;


			internal SourceFileReader<T> reader;
			internal double processState = 0.0;
			internal IList<File> selectedFiles;
			internal const long DEFAULT_PARTICLE_SIZE = 1024 * 64;
			internal long totalBytes = 0L, readedBytes = 0L, particleSize = DEFAULT_PARTICLE_SIZE;
			internal int cnt = 1;
			public ReadFileTask(ReadFileProgressDialog<T> outerInstance, SourceFileReader<T> reader, IList<File> selectedFiles)
			{
				this.outerInstance = outerInstance;
				this.reader = reader;
				this.selectedFiles = selectedFiles;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override protected T call() throws Exception
			protected internal override T call()
			{
				try
				{
					outerInstance.preventClosing = true;

					this.updateProgress(ProgressIndicator.INDETERMINATE_PROGRESS, ProgressIndicator.INDETERMINATE_PROGRESS);
					this.updateProgressMessage(outerInstance.i18n.getString("ReadFileProgressDialog.pleasewait.label", "Please wait\u2026"));

					this.reader.addFileProgressChangeListener(this);

					if (this.isCancelled())
					{
						return default(T);
					}

					this.processState = 0.0;
					this.updateProgress(this.processState, 1.0);

					foreach (File file in this.selectedFiles)
					{
						this.totalBytes += Files.size(file.toPath());
					}

					// estimate particle size of file size
					this.particleSize = DEFAULT_PARTICLE_SIZE;
					this.cnt = 1;

					int ratio = (int)Math.Ceiling(this.totalBytes / this.particleSize);
					if (ratio < 15)
					{
						ratio = 15;
					}
					if (ratio > 30)
					{
						ratio = 30;
					}
					this.particleSize = (long)(this.totalBytes / ratio);

					this.reader.reset();
					T result = default(T);
					for (int i = 0; i < this.selectedFiles.Count; i++)
					{
						File file = this.selectedFiles[i];
						this.updateProgressMessage(String.format(Locale.ENGLISH, outerInstance.i18n.getString("ReadFileProgressDialog.sourcefile.label", "File %d of %d - Please wait\u2026\r\n%s"), (i + 1), this.selectedFiles.Count, file.getName()));
						this.reader.setPath(file.toPath(), false);
						result = this.reader.readAndImport();
						this.updateProgressMessage(outerInstance.i18n.getString("ReadFileProgressDialog.pleasewait.label", "Please wait\u2026"));
						this.readedBytes += Files.size(file.toPath());
					}

					if (this.isCancelled())
					{
						return default(T);
					}

					this.updateProgress(ProgressIndicator.INDETERMINATE_PROGRESS, ProgressIndicator.INDETERMINATE_PROGRESS);
					this.updateMessage(outerInstance.i18n.getString("ReadFileProgressDialog.save.label", "Save results\u2026"));
					this.updateProgressMessage(outerInstance.i18n.getString("ReadFileProgressDialog.pleasewait.label", "Please wait\u2026"));

					return result;
				}
				finally
				{
					this.destroy();
					this.updateProgressMessage(null);
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
				outerInstance.result = default(T);
				outerInstance.preventClosing = false;
				base.failed();
				hideDialog();
			}

			protected internal override void cancelled()
			{
				outerInstance.result = default(T);
				base.cancelled();
				if (this.reader != null)
				{
					this.reader.interrupt();
				}
			}

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

			internal virtual void updateProgressMessage(string message)
			{
				Platform.runLater(() =>
				{
				outerInstance.progressLabel.setText(message);
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
						text.setText(outerInstance.i18n.getString("ReadFileProgressDialog.done.label", "Done"));
						outerInstance.progressIndicator.setPrefWidth(text.getLayoutBounds().getWidth());
					}
				}
				});
			}

			internal virtual void destroy()
			{
				if (this.reader != null)
				{
					this.reader.removeFileProgressChangeListener(this);
				}
				this.reader = null;
				this.selectedFiles = null;
				this.totalBytes = 0;
				this.readedBytes = 0;
				this.particleSize = DEFAULT_PARTICLE_SIZE;
				this.cnt = 1;
				outerInstance.result = default(T);
			}

			public virtual void fileProgressChanged(FileProgressEvent evt)
			{

				if (evt.EventType == FileProgressEvent.FileProgressEventType.READ_LINE)
				{
					long readedBytes = evt.ReadedBytes;
	//				long totalBytes  = evt.getTotalBytes();

					readedBytes += this.readedBytes;

					if (readedBytes > this.cnt * this.particleSize)
					{
						this.cnt++;

						double frac = Math.Min((double)readedBytes / this.totalBytes, 1.0);
						this.processState = Math.Max(this.processState, frac);

						this.updateProgress(this.processState, 1.0);
					}
				}
			}
		}

		private static Window window;
		private ReadFileTask readFileTask;
		private bool preventClosing = false;
		private I18N i18n = I18N.Instance;
		private Dialog<T> dialog = null;
		private ProgressIndicator progressIndicator = new ProgressIndicator(ProgressIndicator.INDETERMINATE_PROGRESS);
		private Label progressLabel = new Label();
		private T result;
		public ReadFileProgressDialog()
		{
		}

		public virtual Optional<T> showAndWait(SourceFileReader<T> reader, IList<File> selectedFiles)
		{
			this.init();
			this.reset();

			this.process(reader, selectedFiles);

			// @see https://bugs.openjdk.java.net/browse/JDK-8087458
			Platform.runLater(() =>
			{
			try
			{
				dialog.getDialogPane().requestLayout();
				Stage stage = (Stage) dialog.getDialogPane().getScene().getWindow();
				stage.sizeToScene();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			});
			return this.dialog.showAndWait();
		}

		public static Window Owner
		{
			set
			{
				window = value;
			}
		}

		private void init()
		{
			if (this.dialog != null)
			{
				return;
			}

			this.dialog = new Dialog<T>();
			this.dialog.initOwner(window);
			this.dialog.setTitle(i18n.getString("ReadFileProgressDialog.title", "File reader"));
			this.dialog.setHeaderText(i18n.getString("ReadFileProgressDialog.header", "Read source file\u2026"));
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
			private readonly ReadFileProgressDialog<T> outerInstance;

			public EventHandlerAnonymousInnerClass(ReadFileProgressDialog<T> outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(WindowEvent @event)
			{
				if (outerInstance.preventClosing)
				{
					@event.consume();
					if (outerInstance.readFileTask != null)
					{
						outerInstance.readFileTask.cancelled();
					}
				}
			}
		}

		private class EventHandlerAnonymousInnerClass2 : EventHandler<DialogEvent>
		{
			private readonly ReadFileProgressDialog<T> outerInstance;

			public EventHandlerAnonymousInnerClass2(ReadFileProgressDialog<T> outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(DialogEvent @event)
			{
				if (outerInstance.preventClosing)
				{
					@event.consume();
					if (outerInstance.readFileTask != null)
					{
						outerInstance.readFileTask.cancelled();
					}
				}
			}
		}

		private class CallbackAnonymousInnerClass : Callback<ButtonType, T>
		{
			private readonly ReadFileProgressDialog<T> outerInstance;

			public CallbackAnonymousInnerClass(ReadFileProgressDialog<T> outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override T call(ButtonType buttonType)
			{
				return outerInstance.result;
			}
		}

		private void reset()
		{
			this.result = default(T);
			this.progressIndicator.setProgress(ProgressIndicator.INDETERMINATE_PROGRESS);
			this.progressLabel.setText(null);
			this.preventClosing = false;
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

			gridPane.add(this.progressIndicator, 0, 0, 1, 2); // col, row, colspan, rowspan
			gridPane.add(this.progressLabel, 1, 0);

			return gridPane;
		}

		private void process(SourceFileReader<T> reader, IList<File> selectedFiles)
		{
			this.reset();
			this.readFileTask = new ReadFileTask(this, reader, selectedFiles);
			this.readFileTask.setOnSucceeded(new EventHandlerAnonymousInnerClass3(this));

			this.readFileTask.setOnFailed(new EventHandlerAnonymousInnerClass4(this));

			Thread th = new Thread(this.readFileTask);
			th.setDaemon(true);
			th.Start();
		}

		private class EventHandlerAnonymousInnerClass3 : EventHandler<WorkerStateEvent>
		{
			private readonly ReadFileProgressDialog<T> outerInstance;

			public EventHandlerAnonymousInnerClass3(ReadFileProgressDialog<T> outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(WorkerStateEvent @event)
			{
				outerInstance.result = outerInstance.readFileTask.getValue();
			}
		}

		private class EventHandlerAnonymousInnerClass4 : EventHandler<WorkerStateEvent>
		{
			private readonly ReadFileProgressDialog<T> outerInstance;

			public EventHandlerAnonymousInnerClass4(ReadFileProgressDialog<T> outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void handle(WorkerStateEvent @event)
			{
				outerInstance.preventClosing = false;
				outerInstance.dialog.hide();
				Exception throwable = outerInstance.readFileTask.getException();
				if (throwable != null)
				{
					Platform.runLater(() =>
					{
					OptionDialog.showThrowableDialog(outerInstance.i18n.getString("ReadFileProgressDialog.message.error.import.exception.title", "I/O Error"), outerInstance.i18n.getString("ReadFileProgressDialog.message.error.import.exception.header", "Error, could not import selected file."), outerInstance.i18n.getString("ReadFileProgressDialog.message.error.import.exception.message", "An exception has occurred during file import."), throwable);
					});
					Console.WriteLine(throwable.ToString());
					Console.Write(throwable.StackTrace);
				}
				UITreeBuilder.Instance.Tree.getSelectionModel().select(0);
				UIPointTableBuilder.Instance.Table.refresh();
				UIParameterTableBuilder.Instance.Table.refresh();
			}
		}
	}

}