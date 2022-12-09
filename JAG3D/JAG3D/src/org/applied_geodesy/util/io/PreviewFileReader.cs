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

namespace org.applied_geodesy.util.io
{

	public class PreviewFileReader : LockFileReader
	{
		private const int MAX_LINES = 15;
		private int maxLines = PreviewFileReader.MAX_LINES, lineCounter = 0;
		private IList<string> lines = new List<string>();

		public PreviewFileReader(File sf) : this(sf, MAX_LINES)
		{
		}

		public PreviewFileReader(File sf, int maxLines) : base(sf)
		{
			this.maxLines = maxLines;
		}

		public virtual void reset()
		{
			this.lineCounter = 0;
			this.lines.Clear();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void read() throws IOException, java.sql.SQLException
		public override void read()
		{
			this.reset();
			base.read();
		}

		public override void parse(string line)
		{
			if (this.lines.Add(line))
			{
				this.lineCounter++;
			}

			if (this.lineCounter >= this.maxLines)
			{
				base.interrupt();
			}
		}

		public virtual IList<string> Lines
		{
			get
			{
				return this.lines;
			}
		}
	}
}