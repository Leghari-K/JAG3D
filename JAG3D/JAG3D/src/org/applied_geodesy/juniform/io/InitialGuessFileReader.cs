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

namespace org.applied_geodesy.juniform.io
{

	using GeometricPrimitive = org.applied_geodesy.adjustment.geometry.GeometricPrimitive;
	using ParameterType = org.applied_geodesy.adjustment.geometry.parameter.ParameterType;
	using UnknownParameter = org.applied_geodesy.adjustment.geometry.parameter.UnknownParameter;
	using I18N = org.applied_geodesy.juniform.ui.i18n.I18N;
	using org.applied_geodesy.util.io;

	using ExtensionFilter = javafx.stage.FileChooser.ExtensionFilter;

	public class InitialGuessFileReader : SourceFileReader<GeometricPrimitive>
	{
		private readonly GeometricPrimitive geometricPrimitive;
		private bool containsValidContent = false;
		public InitialGuessFileReader(GeometricPrimitive geometricPrimitive)
		{
			this.geometricPrimitive = geometricPrimitive;
			this.reset();
		}

		public InitialGuessFileReader(string fileName, GeometricPrimitive geometricPrimitive) : this((new File(fileName)).toPath(), geometricPrimitive)
		{
		}

		public InitialGuessFileReader(File sf, GeometricPrimitive geometricPrimitive) : this(sf.toPath(), geometricPrimitive)
		{
		}

		public InitialGuessFileReader(Path path, GeometricPrimitive geometricPrimitive) : base(path)
		{
			this.geometricPrimitive = geometricPrimitive;
			this.reset();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public org.applied_geodesy.adjustment.geometry.GeometricPrimitive readAndImport() throws Exception
		public override GeometricPrimitive readAndImport()
		{
			this.ignoreLinesWhichStartWith("#");
			base.read();

			if (!this.containsValidContent)
			{
				throw new System.ArgumentException("Error, selected file does not contain valid initial values!");
			}

			return this.geometricPrimitive;
		}

		public override void reset()
		{
			this.containsValidContent = false;
		}

		public override void parse(string line)
		{
			try
			{
				if (string.ReferenceEquals(line, null) || line.Trim().Length == 0)
				{
					return;
				}

				string[] data = line.Trim().Split("[;=\\s]+", true);

				if (data.Length < 2)
				{
					return;
				}
				ParameterType type = (ParameterType)Enum.Parse(typeof(ParameterType), data[0]);
				double value0 = double.Parse(data[1]);
				UnknownParameter parameter = this.geometricPrimitive.getUnknownParameter(type);

				if (parameter != null)
				{
					parameter.Value0 = value0;
					this.containsValidContent = true;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
		}

		public static ExtensionFilter[] ExtensionFilters
		{
			get
			{
				return new ExtensionFilter[] {new ExtensionFilter(I18N.Instance.getString("InitialGuessFileReader.extension.description", "All files"), "*.*")};
			}
		}
	}

}