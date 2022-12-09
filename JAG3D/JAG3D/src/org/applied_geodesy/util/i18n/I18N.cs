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

namespace org.applied_geodesy.util.i18n
{


	public class I18N
	{
		private ResourceBundle bundle;
		private Locale currentLocale = Locale.ENGLISH;
		private string baseName = null;

		public I18N(Locale locale, string baseName)
		{
			this.Locale = locale;
			this.BaseName = baseName;
		}

		public virtual Locale Locale
		{
			set
			{
				this.currentLocale = value;
				this.loadBundle();
			}
		}

		public virtual string BaseName
		{
			set
			{
				this.baseName = value;
				this.loadBundle();
			}
		}

		public virtual void loadBundle()
		{
			if (this.currentLocale != null && !string.ReferenceEquals(this.baseName, null))
			{
				try
				{
					ResourceBundle.clearCache();
					this.bundle = ResourceBundle.getBundle(this.baseName, this.currentLocale);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
		}

		public virtual string getString(string key, string defaultValue)
		{
			if (string.ReferenceEquals(key, null) || this.bundle == null || !this.bundle.containsKey(key))
			{
				Console.Error.WriteLine(this.GetType().Name + " WARNING: Missing entry in lang file.\r\n" + key + " = " + defaultValue);
			}
			return (string.ReferenceEquals(key, null) || this.bundle == null || !this.bundle.containsKey(key)) ? defaultValue : this.bundle.getString(key);
		}
	}

}