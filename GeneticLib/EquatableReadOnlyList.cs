using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GeneticLib
{
	public sealed class EquatableReadOnlyList<T> : IReadOnlyList<T>, IEquatable<EquatableReadOnlyList<T>>
	{
		#region Private Fields

		private readonly IReadOnlyList<T> _list;

		#endregion

		#region Constructors

		public EquatableReadOnlyList(IReadOnlyList<T> list)
		{
			_list = list;
		}

		#endregion

		#region IEquatable

		public bool Equals(EquatableReadOnlyList<T> other)
		{
			return _list.SequenceEqual(other._list);
		}

		#endregion

		#region IReadOnlyList

		public IEnumerator<T> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public int Count => _list.Count;

	    public T this[int index] => _list[index];

	    #endregion

	}
}
