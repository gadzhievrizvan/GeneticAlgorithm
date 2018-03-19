using GeneticLib.Base;
using System;
using System.Collections.Generic;

namespace GeneticLib.Some
{
    public sealed class RandomPopulationMutator<TVector> : IPopulationMutator<TVector>
		where TVector:IEquatable<TVector>
	{
		#region Private Fields

		private readonly IVectorMutator<TVector> _vectorMutator;
		private readonly Random _rand;

		#endregion

		#region Constructors

		public RandomPopulationMutator(IVectorMutator<TVector> vectorMutator)
		{
			_vectorMutator = vectorMutator;
			_rand = new Random();
		}

		#endregion

		#region Private Methods

		private IEnumerable<TVector> GetVectorsToMutate(IReadOnlyList<TVector> population, int mutateCount)
		{
			var populationSize = population.Count;
			var vectorsToMutate = new List<TVector>();

			for (var i = 0; i != mutateCount; ++i)
			{
				while (true)
				{
					var vectorId = _rand.Next(0, populationSize);
					var vector = population[vectorId];
					var alreadyChoosen = vectorsToMutate.Contains(vector);
					if (alreadyChoosen)
					{
						// продолжаем
					}
					else
					{
						vectorsToMutate.Add(vector);
						break;
					}
				}
			}

			return vectorsToMutate;
		}

		#endregion

		#region IPopulationMutator

		public IReadOnlyList<TVector> MutatePopulation(IReadOnlyList<TVector> population, int mutateCount)
		{
			var populationSize = population.Count;
			if (populationSize < mutateCount)
				throw new ArgumentOutOfRangeException();


			var mutadedPopulation = new List<TVector>(population);
			var vectorsToMutate = GetVectorsToMutate(population, mutateCount);

			foreach (var vector in vectorsToMutate)
			{
				var mutatedVector = _vectorMutator.Mutate(vector);
				mutadedPopulation.Remove(vector);
				mutadedPopulation.Add(mutatedVector);
			}

			return mutadedPopulation;
		}

		#endregion
	}
}
