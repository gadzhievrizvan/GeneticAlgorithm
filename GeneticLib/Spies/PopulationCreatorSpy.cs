using System;
using System.Collections.Generic;
using GeneticLib.Base;

namespace GeneticLib.Spies
{
	public sealed class PopulationCreatorSpy<TVector> : IPopulationCreator<TVector>
		where TVector : IEquatable<TVector>
	{
		#region Private Fields

		private readonly IPopulationCreator<TVector> _populationCreator;

		#endregion

		#region Constructors

		public PopulationCreatorSpy(IPopulationCreator<TVector> populationCreator)
		{
			_populationCreator = populationCreator;
		}

		#endregion

		#region IVectorMutator

		public IReadOnlyList<TVector> CreateFirstPopulation()
		{
			var population = _populationCreator.CreateFirstPopulation();
		    PopulationCreated?.Invoke(this, population);
		    return population;
		}

		#endregion

		#region Events

		public event EventHandler<IReadOnlyList<TVector>> PopulationCreated;

		#endregion


	}
}
