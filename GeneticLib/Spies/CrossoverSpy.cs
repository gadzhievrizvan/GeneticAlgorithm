using System;
using System.Collections.Generic;
using GeneticLib.Base;

namespace GeneticLib.Spies
{
	public struct ChildrenStruct<TVector>
	{
		#region Private Fields

		private readonly TVector _firstParent;
		private readonly TVector _secondParent;
		private readonly IReadOnlyList<TVector> _children;

		#endregion

		#region Constructors

		public ChildrenStruct(
			TVector firstParent, 
			TVector secondParent, 
			IReadOnlyList<TVector> children)
		{
			_firstParent = firstParent;
			_secondParent = secondParent;
			_children = children;
		}

		#endregion

		#region Properties

		public TVector FirstParent => _firstParent;

	    public TVector SecondParent => _secondParent;

	    public IReadOnlyList<TVector> Children => _children;

	    #endregion

	}

	public sealed class CrossoverSpy<TVector> : ICrossover<TVector>
		where TVector : IEquatable<TVector>
	{
		#region Private Fields

		private readonly ICrossover<TVector> _crossover;

		#endregion

		#region Constructors

		public CrossoverSpy(ICrossover<TVector> crossover)
		{
			_crossover = crossover;
		}

		#endregion

		#region ICrossover

		public IReadOnlyList<TVector> Crossover(TVector first, TVector second)
		{
			var children = _crossover.Crossover(first, second);
			var childrenStruct = new ChildrenStruct<TVector>(first, second, children);
		    if (ChildrenCreated != null)
                ChildrenCreated.Invoke(this, childrenStruct);

		    return children;
		}

		#endregion

		#region Events

		public event EventHandler<ChildrenStruct<TVector>> ChildrenCreated;

		#endregion
	}
}
