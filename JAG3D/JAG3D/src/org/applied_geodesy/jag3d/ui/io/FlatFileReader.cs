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

namespace org.applied_geodesy.jag3d.ui.io
{

	using I18N = org.applied_geodesy.jag3d.ui.i18n.I18N;
	using org.applied_geodesy.util.io;

	using ExtensionFilter = javafx.stage.FileChooser.ExtensionFilter;

	public abstract class FlatFileReader<T> : SourceFileReader<T>
	{

		protected internal FlatFileReader() : base()
		{
		}

		protected internal FlatFileReader(string fileName) : this((new File(fileName)).toPath())
		{
		}

		protected internal FlatFileReader(File sf) : this(sf.toPath())
		{
		}

		protected internal FlatFileReader(Path path) : base(path)
		{
		}

		public static ExtensionFilter[] ExtensionFilters
		{
			get
			{
				return new ExtensionFilter[] {new ExtensionFilter(I18N.Instance.getString("FlatFileReader.extension", "All files"), "*.*")};
			}
		}
	}

}