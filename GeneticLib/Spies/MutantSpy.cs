using System;
using GeneticLib.Base;

namespace GeneticLib.Spies
{
	public struct MutantStruct<TVector>
	{
		#region Private Fields

		private readonly TVector _baseVector;
		private readonly TVector _mutatedVector;

		#endregion

		#region Constructors

		public MutantStruct(TVector baseVector, TVector mutatedVector)
		{
			_baseVector = baseVector;
			_mutatedVector = mutatedVector;
		}

		#endregion

		#region Properties

		public TVector BaseVector => _baseVector;

	    public TVector MutatedVector => _mutatedVector;

	    #endregion
	}

	public sealed class MutantSpy<TVector> : IVectorMutator<TVector>
		where TVector : IEquatable<TVector>
	{
		#region Private Fields

		private readonly IVectorMutator<TVector> _mutator;

		#endregion

		#region Constructors

		public MutantSpy(IVectorMutator<TVector> mutator)
		{
			_mutator = mutator;
		}

		#endregion

		#region IVectorMutator

		public TVector Mutate(TVector vector)
		{
			var mutant = _mutator.Mutate(vector);

			var mutantStruct = new MutantStruct<TVector>(vector, mutant);
		    VectorMutated?.Invoke(this, mutantStruct);
		    return mutant;
		}

		#endregion

		#region Events

		public event EventHandler<MutantStruct<TVector>> VectorMutated;

		#endregion
	}
}
