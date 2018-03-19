using System;
using System.Collections.Generic;
using GeneticLib.Base;

namespace GeneticLib.ChildrenCreator
{
    public sealed class RankChildrenCreator<TVector> : IChildrenCreator<TVector>
		where TVector : IEquatable<TVector>
	{
		#region Private Fields

		private readonly ICrossover<TVector> _crossover;
		private readonly IFitnessFunction<TVector, double> _fitnessFunction;
		private readonly Random _random;

		#endregion

		#region Constructors

		public RankChildrenCreator(
			ICrossover<TVector> crossover,
			IFitnessFunction<TVector, double> fitnessFunction)
		{
			_crossover = crossover;
			_fitnessFunction = fitnessFunction;
			_random = new Random();
		}

		#endregion

		#region Private Methods

		private TVector GetParent(
			IReadOnlyList<GeneticVector<TVector, int>> rankedPopulation,
			int summaryRanksValue)
		{
			var treshold = _random.Next(0, summaryRanksValue + 1);

			foreach (var item in rankedPopulation)
			{
				var vector = item.Vector;
				var rank = item.FitnessValue;

				summaryRanksValue -= rank;
				if (summaryRanksValue <= treshold)
				{
					return vector;
				}
				else
				{
					// продолжаем
				}
			}

			throw new Exception();
		}

		private List<GeneticVector<TVector, double>> GetPopulationWithFitnessValues(IReadOnlyList<TVector> population)
		{
			var populationWithFitnessValues = new List<GeneticVector<TVector, double>>();
			foreach (var vector in population)
			{
				var fitnessValue = _fitnessFunction.FitnessFunction(vector);
				var geneticVector = new GeneticVector<TVector, double>(vector, fitnessValue);

				populationWithFitnessValues.Add(geneticVector);
			}

			return populationWithFitnessValues;
		}

		#endregion

		#region IChildrenCreator

		public IReadOnlyList<TVector> CreateChildren(IReadOnlyList<TVector> population, int desiredCount)
		{
			var children = new List<TVector>();

			var summaryRank = (population.Count*(1 + population.Count)) / 2;
			var rankedPopulation = GetRankedPopulation(population);

			var totalChildrenToAdd = desiredCount;

			while (totalChildrenToAdd > 0)
			{
				#region Выбрать родителей

				var firstParent = GetParent(rankedPopulation, summaryRank);
				var secondParent = GetParent(rankedPopulation, summaryRank);

				while (firstParent.Equals(secondParent))
				{
					firstParent = GetParent(rankedPopulation, summaryRank);
				}
				
				#endregion

				var newChildren = _crossover.Crossover(firstParent, secondParent);

				var childrenToAdd = Math.Min(newChildren.Count, totalChildrenToAdd);

				for (var i = 0; i != childrenToAdd; ++i)
				{
					var currentChildren = newChildren[i];
					children.Add(currentChildren);
				}

				totalChildrenToAdd -= childrenToAdd;
			}

			return children;
		}

		private IReadOnlyList<GeneticVector<TVector, int>> GetRankedPopulation(IReadOnlyList<TVector> population)
		{
			var populationWithFitnessValues = GetPopulationWithFitnessValues(population);
			populationWithFitnessValues.Sort((x, y) => x.FitnessValue.CompareTo(y.FitnessValue));

			var rankedPopulation = new List<GeneticVector<TVector, int>>();
			var maxRank = population.Count;
			var rank = maxRank;

			foreach (var item in populationWithFitnessValues)
			{
				var vector = item.Vector;
				rankedPopulation.Add(new GeneticVector<TVector, int>(vector, rank));
				rank--;
			}

			return rankedPopulation;
		}

		#endregion

	}
}
