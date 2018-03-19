using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticLib;
using GeneticLib.Base;

namespace SandBox.GeneticTry.ClassesForSettings
{
	public sealed class RouletteChildrenCreator<TVector> : IChildrenCreator<TVector> 
		where TVector : IEquatable<TVector>
	{
		#region Private Fields

		private readonly ICrossover<TVector> _crossover;
		private readonly IFitnessFunction<TVector, double> _fitnessFunction;
		private readonly Random _random;

		#endregion

		#region Constructors

		public RouletteChildrenCreator(
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
			IReadOnlyList<GeneticVector<TVector, double>> populationWithFitnessValues,
			double totalFitnessValue)
		{
			var treshold = _random.Next(0, (int) totalFitnessValue + 1);

			foreach (var item in populationWithFitnessValues)
			{
				var vector = item.Vector;
				var fitnessValue = item.FitnessValue;

				totalFitnessValue -= fitnessValue;
				if (totalFitnessValue <= treshold)
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

			var populationWithFitnessValues = GetPopulationWithFitnessValues(population);
			var totalFitnessValues = populationWithFitnessValues.Sum(x => x.FitnessValue);

			populationWithFitnessValues.Sort((x, y) => x.FitnessValue.CompareTo(y.FitnessValue));

			var totalChildrenToAdd = desiredCount;

			while (totalChildrenToAdd > 0)
			{
				#region Выбрать родителей

				var firstParent = GetParent(populationWithFitnessValues, totalFitnessValues);
				var secondParent = GetParent(populationWithFitnessValues, totalFitnessValues);

				while (firstParent.Equals(secondParent))
				{
					firstParent = GetParent(populationWithFitnessValues, totalFitnessValues);
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

		#endregion

	}
}
