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

namespace org.applied_geodesy.ui.textfield
{
	using TextField = javafx.scene.control.TextField;

	public class LimitedTextField : TextField
	{

		private readonly int limit;

		public LimitedTextField(int limit) : this(limit, null)
		{
		}

		public LimitedTextField(int limit, string text) : base(text)
		{
			this.limit = limit;
		}

		public override void replaceText(int start, int end, string text)
		{
			base.replaceText(start, end, text);
			this.verify();
		}

		public override void replaceSelection(string text)
		{
			base.replaceSelection(text);
			this.verify();
		}

		private void verify()
		{
			if (getText().length() > this.limit)
			{
				setText(getText().Substring(0, this.limit));
				positionCaret(this.limit);
			}
		}
	}
}