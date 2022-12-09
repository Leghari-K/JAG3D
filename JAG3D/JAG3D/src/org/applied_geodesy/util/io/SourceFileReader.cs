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

namespace org.applied_geodesy.util.io
{

	public abstract class SourceFileReader<T> : LockFileReader
	{
		protected internal SourceFileReader() : base()
		{
		}

		protected internal SourceFileReader(string fileName) : this((new File(fileName)).toPath())
		{
		}

		protected internal SourceFileReader(File sf) : this(sf.toPath())
		{
		}

		protected internal SourceFileReader(Path path) : base(path)
		{
			this.Path = path;
		}

		public override Path Path
		{
			set
			{
				this.setPath(value, true);
			}
		}

		public virtual void setPath(Path path, bool reset)
		{
			if (reset)
			{
				this.reset();
			}
			base.Path = path;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public abstract T readAndImport() throws Exception;
		public abstract T readAndImport();

		public abstract void reset();

		public virtual string createItemName(string prefix, string suffix)
		{
			prefix = string.ReferenceEquals(prefix, null) ? "" : prefix;
			suffix = string.ReferenceEquals(suffix, null) ? "" : suffix;
			string fileName = this.Path.getFileName().ToString();
			if (fileName.Trim().Length > 0)
			{
				if (fileName.IndexOf('.') > 0)
				{
					fileName = fileName.Substring(0, fileName.LastIndexOf('.'));
				}
				return prefix + fileName + suffix;
			}
			return null;
		}
	}
}