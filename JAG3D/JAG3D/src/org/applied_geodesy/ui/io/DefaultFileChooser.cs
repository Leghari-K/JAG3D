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

namespace org.applied_geodesy.ui.io
{

	using DefaultApplicationProperty = org.applied_geodesy.jag3d.DefaultApplicationProperty;

	using FileChooser = javafx.stage.FileChooser;
	using ExtensionFilter = javafx.stage.FileChooser.ExtensionFilter;
	using Window = javafx.stage.Window;

	public class DefaultFileChooser
	{
		private static File lastSelectedDirectory = DefaultApplicationProperty.DefaultWorkspacePath;
		private static FileChooser.ExtensionFilter lastSelectedExtensionFilter = null;
		private static string lastSelectedFileName = "";
		private static readonly FileChooser fileChooser = new FileChooser();

		private DefaultFileChooser()
		{
		}

		private static void prepareFileChooser(string title, string initialFileName, params FileChooser.ExtensionFilter[] extensionFilters)
		{
			if (lastSelectedDirectory == null && System.getProperty("user.home", null) != null)
			{
				lastSelectedDirectory = new File(System.getProperty("user.home"));
			}
			fileChooser.setInitialDirectory(lastSelectedDirectory != null && Files.exists(lastSelectedDirectory.toPath()) ? lastSelectedDirectory : null);
			fileChooser.setInitialFileName(string.ReferenceEquals(initialFileName, null) ? lastSelectedFileName : initialFileName);
			fileChooser.setTitle(title);

			if (extensionFilters != null && extensionFilters.Length > 0)
			{

				bool containsFilter = false;
				foreach (FileChooser.ExtensionFilter filter in extensionFilters)
				{
					if (equalExtensionFilters(lastSelectedExtensionFilter, filter))
					{
						LastSelectedExtensionFilter = filter;
						containsFilter = true;
						break;
					}
				}

				fileChooser.getExtensionFilters().setAll(extensionFilters);

				if (!containsFilter || lastSelectedExtensionFilter == null)
				{
					lastSelectedExtensionFilter = extensionFilters[0];
				}
				fileChooser.setSelectedExtensionFilter(lastSelectedExtensionFilter);
			}
			else
			{
				fileChooser.getExtensionFilters().clear();
			}
		}

		private static bool equalExtensionFilters(FileChooser.ExtensionFilter filter1, FileChooser.ExtensionFilter filter2)
		{
			if (filter1 == null || filter2 == null || filter1.getExtensions().size() != filter2.getExtensions().size() || !filter1.getDescription().Equals(filter2.getDescription()))
			{
				return false;
			}

			IList<string> extensions = filter1.getExtensions();
			foreach (string extension in extensions)
			{
				if (!filter2.getExtensions().contains(extension))
				{
					return false;
				}
			}

			return true;
		}

		public static File showOpenDialog(Window window, string title, string initialFileName, params FileChooser.ExtensionFilter[] extensionFilters)
		{
			prepareFileChooser(title, initialFileName, extensionFilters);
			File file = fileChooser.showOpenDialog(window);
			LastSelectedDirectory = file;
			LastSelectedExtensionFilter = fileChooser.getSelectedExtensionFilter();
			return file;
		}

		public static IList<File> showOpenMultipleDialog(Window window, string title, string initialFileName, params FileChooser.ExtensionFilter[] extensionFilters)
		{
			prepareFileChooser(title, initialFileName, extensionFilters);
			IList<File> files = fileChooser.showOpenMultipleDialog(window);
			if (files != null && files.Count > 0 && files[0] != null)
			{
				LastSelectedDirectory = files[0];
				LastSelectedExtensionFilter = fileChooser.getSelectedExtensionFilter();
			}

			return files;
		}

		public static File showSaveDialog(Window window, string title, string initialFileName, params FileChooser.ExtensionFilter[] extensionFilters)
		{
			prepareFileChooser(title, initialFileName, extensionFilters);

			File file = fileChooser.showSaveDialog(window);
			FileChooser.ExtensionFilter extensionFilter = fileChooser.getSelectedExtensionFilter();

			if (file != null && file.getParentFile() != null)
			{
				LastSelectedDirectory = file.getParentFile();
				LastSelectedExtensionFilter = extensionFilter;
			}

			if (file != null && extensionFilter != null)
			{
				bool hasExtension = false;
				IList<string> extensions = extensionFilter.getExtensions();
				if (extensions != null && extensions.Count > 0)
				{
					foreach (string extension in extensions)
					{
						// if *.* filter is enabled or file ends with current extension --> true
						if (extension.EndsWith("*.*", StringComparison.Ordinal) || file.getName().ToLower().EndsWith(extension.ToLower().Substring(1), StringComparison.Ordinal))
						{
							hasExtension = true;
							break;
						}
					}
					// add first extension
					if (!hasExtension)
					{
						file = new File(file.getAbsolutePath() + extensions[0].Substring(1));
					}
				}
			}

			return file;
		}

		public static FileChooser.ExtensionFilter SelectedExtensionFilter
		{
			get
			{
				return fileChooser.getSelectedExtensionFilter();
			}
		}

		public static FileChooser.ExtensionFilter LastSelectedExtensionFilter
		{
			set
			{
				if (value == null)
				{
					return;
				}
				lastSelectedExtensionFilter = value;
			}
		}

		public static File LastSelectedDirectory
		{
			set
			{
				if (value == null)
				{
					return;
				}
    
				if (value.isDirectory())
				{
					lastSelectedDirectory = value;
				}
				else if (value.isFile() && value.getParentFile() != null)
				{
					lastSelectedDirectory = value.getParentFile();
					LastSelectedFileName = value;
				}
			}
		}

		public static File LastSelectedFileName
		{
			set
			{
				if (value == null)
				{
					return;
				}
    
				lastSelectedFileName = value.getName().replaceFirst(".[^.]+$", "");
			}
		}
	}

}