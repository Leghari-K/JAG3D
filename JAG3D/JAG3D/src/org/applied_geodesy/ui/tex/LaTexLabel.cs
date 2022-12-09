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

namespace org.applied_geodesy.ui.tex
{

	using TeXConstants = org.scilab.forge.jlatexmath.TeXConstants;
	using TeXFormula = org.scilab.forge.jlatexmath.TeXFormula;

	using SimpleStringProperty = javafx.beans.property.SimpleStringProperty;
	using StringProperty = javafx.beans.property.StringProperty;
	using ChangeListener = javafx.beans.value.ChangeListener;
	using ObservableValue = javafx.beans.value.ObservableValue;
	using SwingFXUtils = javafx.embed.swing.SwingFXUtils;
	using ContentDisplay = javafx.scene.control.ContentDisplay;
	using Label = javafx.scene.control.Label;
	using ImageView = javafx.scene.image.ImageView;
	using WritableImage = javafx.scene.image.WritableImage;

	public class LaTexLabel : Label
	{
		private class TexChangeListener : ChangeListener<string>
		{
			private readonly LaTexLabel outerInstance;

			public TexChangeListener(LaTexLabel outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void changed<T1>(ObservableValue<T1> observable, string oldValue, string newValue) where T1 : string
			{
				outerInstance.process();
			}
		}

		private int style = TeXConstants.STYLE_DISPLAY;
		private float size = 16f;
		private Color foregroundColor = Color.BLACK;
		private StringProperty tex = new SimpleStringProperty();

		public LaTexLabel() : this(null)
		{
		}

		public LaTexLabel(string tex)
		{
			this.setContentDisplay(ContentDisplay.GRAPHIC_ONLY);
			this.tex.addListener(new TexChangeListener(this));
			this.Tex = tex;
		}

		private void process()
		{
			if (this.tex.get().isBlank())
			{
				this.setGraphic(null);
			}
			else
			{
				TeXFormula formula = new TeXFormula(this.tex.get());
				Image imageAWT = formula.createBufferedImage(this.style, this.size, this.foregroundColor, null);
				WritableImage writableImage = SwingFXUtils.toFXImage((BufferedImage) imageAWT, null);
				ImageView view = new ImageView(writableImage);
				this.setGraphic(view);
			}
		}

		public virtual StringProperty texProperty()
		{
			return this.tex;
		}

		public virtual string Tex
		{
			get
			{
				return this.tex.get();
			}
			set
			{
				this.tex.set(value);
			}
		}


		public virtual int Style
		{
			set
			{
				this.style = value;
				this.process();
			}
		}

		public virtual float Size
		{
			set
			{
				this.size = value;
				this.process();
			}
		}

		public virtual Color ForegroundColor
		{
			set
			{
				this.foregroundColor = value;
				this.process();
			}
		}
	}

}