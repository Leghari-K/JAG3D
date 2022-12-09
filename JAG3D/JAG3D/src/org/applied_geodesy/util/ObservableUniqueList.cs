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

namespace org.applied_geodesy.util
{

	using ModifiableObservableListBase = javafx.collections.ModifiableObservableListBase;
	using ObservableList = javafx.collections.ObservableList;

	// https://github.com/shitapatilptc/java/blob/master/src/javafx.base/com/sun/javafx/collections/ObservableSequentialListWrapper.java
	public class ObservableUniqueList<T> : ModifiableObservableListBase<T>, ObservableList<T>
	{
		private readonly IList<T> list;
		private readonly ISet<T> set;

		public ObservableUniqueList()
		{
			this.list = new List<T>();
			this.set = new HashSet<T>();
		}

		public ObservableUniqueList(int size)
		{
			this.list = new List<T>(size);
			this.set = new HashSet<T>(size);
		}

		//	@Override
		//	protected void doAdd(int index, T element) {
		//		if (element == null)
		//			throw new NullPointerException("Error, list does not support null values!");
		//		if (this.set.contains(element))
		//			throw new IllegalArgumentException("Error, list elements must be unique but element already exists " + element + "!");
		//		
		//		this.set.add(element);
		//		this.list.add(index, element);
		//	}
		//
		//	@Override
		//	protected T doRemove(int index) {
		//		T element = this.list.remove(index);
		//		this.set.remove(element);
		//		return element;
		//	}
		//
		//	@Override
		//	protected T doSet(int index, T element) {
		//		if (element == null)
		//			throw new NullPointerException("Error, list does not support null values!");
		//		if (this.set.contains(element))
		//			throw new IllegalArgumentException("Error, list elements must be unique but element already exists " + element + "!");
		//		
		//		this.set.add(element);
		//		return this.list.set(index, element);
		//	}
		//
		//	@Override
		//	public T get(int index) {
		//		return this.list.get(index);
		//	}

		public override bool containsAll<T1>(ICollection<T1> c)
		{
			return this.set.ContainsAll(c);
		}

		public override int indexOf(object o)
		{
			return this.list.IndexOf(o);
		}

		public override int lastIndexOf(object o)
		{
			return this.list.LastIndexOf(o);
		}

		public override void clear()
		{
			this.set.Clear();
			this.list.Clear();
		}

		public override bool contains(object @object)
		{
			return this.set.Contains(@object);
		}

		public override IEnumerator<T> listIterator(in int index)
		{
			return new ListIteratorAnonymousInnerClass(this, index);
		}

		private class ListIteratorAnonymousInnerClass : IEnumerator<T>
		{
			private readonly ObservableUniqueList<T> outerInstance;

			private int index;

			public ListIteratorAnonymousInnerClass(ObservableUniqueList<T> outerInstance, int index)
			{
				this.outerInstance = outerInstance;
				this.index = index;
				backingIt = outerInstance.list.listIterator(index);
			}


			private readonly IEnumerator<T> backingIt;
			private T lastReturned;

			public bool hasNext()
			{
				return this.backingIt.hasNext();
			}

			public T next()
			{
				return this.lastReturned = this.backingIt.next();
			}

			public bool hasPrevious()
			{
				return this.backingIt.hasPrevious();
			}

			public T previous()
			{
				return this.lastReturned = this.backingIt.previous();
			}

			public int nextIndex()
			{
				return this.backingIt.nextIndex();
			}

			public int previousIndex()
			{
				return this.backingIt.previousIndex();
			}

			public void remove()
			{
				beginChange();
				int idx = previousIndex();
				this.backingIt.remove();
				nextRemove(idx, this.lastReturned);
				outerInstance.set.remove(this.lastReturned);
				endChange();
			}

			public void set(T e)
			{
				if (e == null)
				{
					throw new System.NullReferenceException("Error, list does not support null values!");
				}
				if (outerInstance.set.Contains(e))
				{
					throw new System.ArgumentException("Error, list elements must be unique but element already exists " + e + "!");
				}

				beginChange();
				int idx = previousIndex();
				this.backingIt.set(e);
				outerInstance.set.Add(e);
				nextSet(idx, this.lastReturned);
				endChange();
			}

			public void add(T e)
			{
				if (e == null)
				{
					throw new System.NullReferenceException("Error, list does not support null values!");
				}
				if (outerInstance.set.Contains(e))
				{
					throw new System.ArgumentException("Error, list elements must be unique but element already exists " + e + "!");
				}

				beginChange();
				int idx = nextIndex();
				this.backingIt.add(e);
				outerInstance.set.Add(e);
				nextAdd(idx, idx + 1);
				endChange();
			}
		}

		public override IEnumerator<T> iterator()
		{
			return listIterator();
		}

		public override T get(int index)
		{
			try
			{
				return this.list.listIterator(index).next();
			}
			catch (NoSuchElementException)
			{
				throw new System.IndexOutOfRangeException("Index: " + index);
			}
		}

		public override bool addAll<T1>(int index, ICollection<T1> c) where T1 : T
		{
			try
			{
				beginChange();
				bool modified = false;
				IEnumerator<T> e1 = this.listIterator(index);
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: java.util.Iterator<? extends T> e2 = c.iterator();
				IEnumerator<T> e2 = c.GetEnumerator();
				while (e2.MoveNext())
				{
					e1.add(e2.Current);
					modified = true;
				}
				endChange();
				return modified;
			}
			catch (NoSuchElementException)
			{
				throw new System.IndexOutOfRangeException("Index: " + index);
			}
		}

		public override bool removeAll<T1>(ICollection<T1> c)
		{
			Objects.requireNonNull(c);
			beginChange();
			bool modified = false;
			if (size() > c.Count)
			{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: for (java.util.Iterator<?> i = c.iterator(); i.hasNext();)
				for (IEnumerator<object> i = c.GetEnumerator(); i.MoveNext();)
				{
					modified |= remove(i.Current);
				}
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: java.util.Collection<?> removals = null;
				ICollection<object> removals = null;
				if (c is ObservableUniqueList)
				{
					removals = c;
				}
				else
				{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: removals = new java.util.HashSet<>(c);
					removals = new HashSet<object>(c);
				}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: for (java.util.Iterator<?> i = iterator(); i.hasNext();)
				for (IEnumerator<object> i = iterator(); i.MoveNext();)
				{
					if (removals.Contains(i.Current))
					{
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
						i.remove();
						modified = true;
					}
				}
			}
			endChange();
			return modified;
		}

		public override int size()
		{
			return this.list.Count;
		}

		protected internal override void doAdd(int index, T element)
		{
			if (element == null)
			{
				throw new System.NullReferenceException("Error, list does not support null values!");
			}
			if (this.set.Contains(element))
			{
				throw new System.ArgumentException("Error, list elements must be unique but element already exists " + element + "!");
			}

			try
			{
				this.list.listIterator(index).add(element);
				this.set.Add(element);
			}
			catch (NoSuchElementException)
			{
				throw new System.IndexOutOfRangeException("Index: " + index);
			}
		}

		protected internal override T doSet(int index, T element)
		{
			if (element == null)
			{
				throw new System.NullReferenceException("Error, list does not support null values!");
			}
			if (this.set.Contains(element))
			{
				throw new System.ArgumentException("Error, list elements must be unique but element already exists " + element + "!");
			}

			try
			{
				IEnumerator<T> e = this.list.listIterator(index);
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				T oldVal = e.next();
				this.set.remove(oldVal);
				e.set(element);
				this.set.Add(element);
				return oldVal;
			}
			catch (NoSuchElementException)
			{
				throw new System.IndexOutOfRangeException("Index: " + index);
			}
		}

		protected internal override T doRemove(int index)
		{
			try
			{
				IEnumerator<T> e = this.list.listIterator(index);
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				T element = e.next();
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
				e.remove();
				this.set.remove(element);
				return element;
			}
			catch (NoSuchElementException)
			{
				throw new System.IndexOutOfRangeException("Index: " + index);
			}
		}
	}

}