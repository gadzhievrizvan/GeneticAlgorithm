using System;
using System.Collections.Generic;
using System.Linq;
using GeneticLib.Base;
using GeneticLib.Some;
using SandBox.GeneticTry.ClassesForSettings;

namespace GeneticLib.PopulationSelection
{
    public sealed class EliteAndRecreate<TVector, TFitnessValue> : INextGenerationCreator<TVector>
		where TVector : IEquatable<TVector>
		where TFitnessValue : IComparable<TFitnessValue>
	{
		#region Private Fields

		private readonly IFitnessFunction<TVector, TFitnessValue> _fitnessFunction;
		private readonly IFactory<TVector> _vectorFactory;

		/// <summary>
		///  0 < eliteCoef < 1
		/// </summary>
		private readonly double _eliteCoef;

		#endregion

		#region Constructors

		public EliteAndRecreate(
			IFitnessFunction<TVector, TFitnessValue> fitnessFunction,
			IFactory<TVector> vectorFactory,
			double eliteCoef)
		{
			_fitnessFunction = fitnessFunction;
			_vectorFactory = vectorFactory;
			_eliteCoef = eliteCoef;
		}

		#endregion

		#region Private Methods

		private IEnumerable<TVector> GetElite(IEnumerable<TVector> population, int eliteCount)
		{
			var populationWithFitnessValues = GetFitnessValues(population);
			var sortedPopulation = populationWithFitnessValues.OrderByDescending(x => x.FitnessValue).ToList();

		    var elite = sortedPopulation
		        .GetRange(0, eliteCount)
		        .Select(x => x.Vector);

		    return elite;
		}

		private IEnumerable<GeneticVector<TVector, TFitnessValue>> GetFitnessValues(IEnumerable<TVector> population)
		{
			var geneticVectors = new List<GeneticVector<TVector, TFitnessValue>>();
			foreach (var vector in population)
			{
				var fitnessValue = _fitnessFunction.FitnessFunction(vector);
				var geneticVector = new GeneticVector<TVector, TFitnessValue>(vector, fitnessValue);
				geneticVectors.Add(geneticVector);
			}

			return geneticVectors;
		}

		#endregion

		#region INextGenerationCreator

		public IReadOnlyList<TVector> CreateNextGeneration(IReadOnlyList<TVector> population, IReadOnlyList<TVector> children)
		{
			var targetPopulationSize = population.Count;

			var eliteCount = (int) (targetPopulationSize * _eliteCoef);
			var mergedList = population.Concat(children);
			var eliteVectors = GetElite(mergedList, eliteCount);

			var newPopulation = new List<TVector>();
			newPopulation.AddRange(eliteVectors);

			var vectorsToCreate = targetPopulationSize - eliteCount;
			for (var i = 0; i != vectorsToCreate; ++i)
			{
				var vector = _vectorFactory.Create();
				newPopulation.Add(vector);
			}

			return newPopulation;
		}

		#endregion
	}
}
