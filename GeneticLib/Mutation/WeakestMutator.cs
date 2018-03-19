using System;
using System.Collections.Generic;
using System.Linq;
using GeneticLib.Base;

namespace GeneticLib.Some
{
	public sealed class WeakestMutator<TVector, TFitnessValue> : IPopulationMutator<TVector>
		where TVector : IEquatable<TVector> where TFitnessValue : IComparable<TFitnessValue>
	{
		#region Private Fields

		private readonly IVectorMutator<TVector> _vectorMutator;
		private readonly IFitnessFunction<TVector, TFitnessValue> _fitnessFunction;

		#endregion

		#region Constructors

		public WeakestMutator(
			IVectorMutator<TVector> vectorMutator,
			IFitnessFunction<TVector, TFitnessValue> fitnessFunction)
		{
			_vectorMutator = vectorMutator;
			_fitnessFunction = fitnessFunction;
		}

		#endregion

		#region Private Methods

		private IEnumerable<TVector> GetWeakestVectors(IEnumerable<TVector> population, int mutateCount)
		{
			var vectorsToMutate = new List<GeneticVector<TVector, TFitnessValue>>();

			foreach (var vector in population)
			{
				var fitnessValue = _fitnessFunction.FitnessFunction(vector);

				var isLessThanAnySelected = vectorsToMutate.Any(x => fitnessValue.CompareTo(x.FitnessValue) == -1);
				var shouldAdd = vectorsToMutate.Count < mutateCount
								||
								isLessThanAnySelected;

				var shouldRemove = vectorsToMutate.Count == mutateCount
				                   &&
								   isLessThanAnySelected;

				if (shouldRemove)
				{
					var vectorToRemove = vectorsToMutate.Min();
					vectorsToMutate.Remove(vectorToRemove);
				}

				if (shouldAdd)
				{
					var geneticVector = new GeneticVector<TVector, TFitnessValue>(vector, fitnessValue);
					vectorsToMutate.Add(geneticVector);
				}
			}

			return vectorsToMutate.Select(x=>x.Vector);
		}

		#endregion

		#region IPopulationMutator

		public IReadOnlyList<TVector> MutatePopulation(IReadOnlyList<TVector> population, int mutateCount)
		{
			var populationSize = population.Count;
			if (populationSize < mutateCount)
				throw new ArgumentOutOfRangeException();

			var mutadedPopulation = new List<TVector>(population);
			var vectorsToMutate = GetWeakestVectors(population, mutateCount);

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
