using System;
using System.Collections.Generic;
using GeneticLib.Base;

namespace GeneticLib.Spies
{
	public sealed class NextGenerationCreatorSpy<TVector> : INextGenerationCreator<TVector>
		where TVector : IEquatable<TVector> 
	{
		#region Private Fields

		private readonly INextGenerationCreator<TVector> _nextGenerationCreator;

		#endregion

		#region Constructors

		public NextGenerationCreatorSpy(INextGenerationCreator<TVector> nextGenerationCreator)
		{
			_nextGenerationCreator = nextGenerationCreator;
		}

		#endregion

		#region INextGenerationCreator

		public IReadOnlyList<TVector> CreateNextGeneration(
			IReadOnlyList<TVector> population,
			IReadOnlyList<TVector> children)
		{
			var newPopulation = _nextGenerationCreator.CreateNextGeneration(population, children);
		    NewPopulationCreated?.Invoke(this, newPopulation);

		    return newPopulation;
		}

		#endregion

		#region Events

		public event EventHandler<IReadOnlyList<TVector>> NewPopulationCreated;

		#endregion

	}
}
