using System.Collections.Generic;
using System.Linq;

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

namespace org.applied_geodesy.util
{

	using ModifiableObservableListBase = javafx.collections.ModifiableObservableListBase;

	public class ObservableLimitedList<T> : ModifiableObservableListBase<T>
	{
		private LinkedList<T> list;
		private int maxSize;

		public ObservableLimitedList(int maxSize)
		{
			this.maxSize = maxSize;
			this.list = new LinkedList<T>();
		}

		public override bool add(T element)
		{
			bool result = base.add(element);
			if (size() > this.maxSize)
			{
				remove(0);
			}

			return result;
		}

		public override T get(int index)
		{
			return this.list.ToList()[index];
		}

		public override int size()
		{
			return this.list.Count;
		}

		protected internal override void doAdd(int index, T element)
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET LinkedList equivalent to the 2-parameter Java 'add' method:
			this.list.add(index, element);
		}

		protected internal override T doSet(int index, T element)
		{
			return this.list.set(index, element);
		}

		protected internal override T doRemove(int index)
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET LinkedList equivalent to the Java 'remove' method:
			return this.list.remove(index);
		}
	}
}