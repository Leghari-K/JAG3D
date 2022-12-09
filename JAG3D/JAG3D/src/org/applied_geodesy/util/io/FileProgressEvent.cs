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

	public class FileProgressEvent : EventObject
	{

		public enum FileProgressEventType
		{
			READ_LINE
		}

		private const long serialVersionUID = 1580425438267707405L;
		private long readedBytes = 0, totalBytes = 0;

		private readonly FileProgressEventType type;
		internal FileProgressEvent(File file, FileProgressEventType type, long readedBytes, long totalBytes) : base(file)
		{
			this.type = type;
			this.readedBytes = readedBytes;
			this.totalBytes = totalBytes;
		}

		public override File Source
		{
			get
			{
				return (File)base.getSource();
			}
		}

		public virtual FileProgressEventType EventType
		{
			get
			{
				return this.type;
			}
		}

		public virtual long ReadedBytes
		{
			get
			{
				return this.readedBytes;
			}
		}

		public virtual long TotalBytes
		{
			get
			{
				return this.totalBytes;
			}
		}
	}

}