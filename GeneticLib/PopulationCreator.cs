using GeneticLib.Base;
using GeneticLib.Some;
using System;
using System.Collections.Generic;

namespace GeneticLib
{
    public sealed class PopulationCreator<TVector> : IPopulationCreator<TVector> 
		where TVector : IEquatable<TVector>
	{
		#region Private Fields

		private readonly IFactory<TVector> _vectorFactory;
		private readonly int _populationSize;

		#endregion

		#region Constructors

		public PopulationCreator(IFactory<TVector> vectorFactory, int populationSize)
		{
			_vectorFactory = vectorFactory;
			_populationSize = populationSize;
		}

		#endregion

		#region IPopulationCreator

		public IReadOnlyList<TVector> CreateFirstPopulation()
		{
			var population = new List<TVector>();

			for (var i = 0; i != _populationSize; ++i)
			{
				var vector = _vectorFactory.Create();
				population.Add(vector);
			}

			return population;
		}

		#endregion

	}
}
