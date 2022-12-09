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

namespace org.applied_geodesy.ui.dialog
{
	using Node = javafx.scene.Node;
	using Alert = javafx.scene.control.Alert;
	using AlertType = javafx.scene.control.Alert.AlertType;
	using ButtonType = javafx.scene.control.ButtonType;
	using Control = javafx.scene.control.Control;
	using Label = javafx.scene.control.Label;
	using TextArea = javafx.scene.control.TextArea;
	using GridPane = javafx.scene.layout.GridPane;
	using Priority = javafx.scene.layout.Priority;
	using Modality = javafx.stage.Modality;
	using Window = javafx.stage.Window;


	public class OptionDialog
	{
		private static Window window;

		private OptionDialog()
		{
		}

		public static Window Owner
		{
			set
			{
				window = value;
			}
		}

		private static Optional<ButtonType> showDialog(Alert.AlertType type, string title, string header, string message)
		{
			Alert alert = new Alert(type);
			alert.setTitle(title);
			alert.setHeaderText(header);
			alert.setContentText(message);
			alert.initModality(Modality.APPLICATION_MODAL);
			alert.initOwner(window);
			alert.setResizable(true);
			alert.getDialogPane().setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);
			return alert.showAndWait();
		}

		public static Optional<ButtonType> showErrorDialog(string title, string header, string message)
		{
			return showDialog(Alert.AlertType.ERROR, title, header, message);
		}

		public static Optional<ButtonType> showWarningDialog(string title, string header, string message)
		{
			return showDialog(Alert.AlertType.WARNING, title, header, message);
		}

		public static Optional<ButtonType> showInformationDialog(string title, string header, string message)
		{
			return showDialog(Alert.AlertType.INFORMATION, title, header, message);
		}

		public static Optional<ButtonType> showConfirmationDialog(string title, string header, string message)
		{
			return showDialog(Alert.AlertType.CONFIRMATION, title, header, message);
		}

		public static Optional<ButtonType> showContentDialog(Alert.AlertType type, string title, string header, string message, Node node)
		{
			Alert alert = new Alert(type);
			alert.setTitle(title);
			alert.setHeaderText(header);
			alert.setContentText(message);
			alert.initModality(Modality.APPLICATION_MODAL);
			alert.initOwner(window);

			GridPane.setVgrow(node, Priority.ALWAYS);
			GridPane.setHgrow(node, Priority.ALWAYS);

			GridPane content = new GridPane();
			content.setMaxWidth(double.MaxValue);

			int row = 0;
			if (!string.ReferenceEquals(message, null))
			{
				Label label = new Label(message);
				content.add(label, 0, ++row);
			}
			content.add(node, 0, ++row);
			alert.getDialogPane().setContent(content);
			alert.getDialogPane().setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			return alert.showAndWait();
		}

		public static Optional<ButtonType> showThrowableDialog(string title, string header, string message, Exception throwable)
		{
			Alert alert = new Alert(Alert.AlertType.ERROR);
			alert.setTitle(title);
			alert.setHeaderText(header);
			alert.setContentText(message);
			alert.initModality(Modality.APPLICATION_MODAL);
			alert.initOwner(window);

			StringWriter stringWriter = new StringWriter();
			PrintWriter printWriter = new PrintWriter(stringWriter);
			throwable.printStackTrace(printWriter);
			string throwableText = stringWriter.ToString();

			Label label = new Label("Stacktrace:");
			TextArea textArea = new TextArea(throwableText);
			textArea.setEditable(false);
			textArea.setWrapText(true);

			textArea.setMaxWidth(double.MaxValue);
			textArea.setMaxHeight(double.MaxValue);
			GridPane.setVgrow(textArea, Priority.ALWAYS);
			GridPane.setHgrow(textArea, Priority.ALWAYS);

			GridPane expContent = new GridPane();
			expContent.setMaxWidth(double.MaxValue);
			expContent.add(label, 0, 0);
			expContent.add(textArea, 0, 1);
			alert.getDialogPane().setExpandableContent(expContent);
			alert.getDialogPane().setMinSize(Control.USE_PREF_SIZE, Control.USE_PREF_SIZE);

			return alert.showAndWait();
		}
	}
}