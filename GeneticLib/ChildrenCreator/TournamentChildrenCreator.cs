using System;
using System.Collections.Generic;
using System.Linq;
using GeneticLib.Base;

namespace SandBox.GeneticTry.ClassesForSettings
{
	public sealed class TournamentChildrenCreator<TVector, TFitnessValue> : IChildrenCreator<TVector>
		where TVector : IEquatable<TVector>
		where TFitnessValue : IComparable<TFitnessValue>
	{
		#region Private Fields

		private readonly ICrossover<TVector> _crossover;
		private readonly IFitnessFunction<TVector, TFitnessValue> _fitnessFunction;
		private readonly int _selectionSize;
		private readonly Random _random;

		#endregion

		#region Constructors

		public TournamentChildrenCreator(
			ICrossover<TVector> crossover,
			IFitnessFunction<TVector,TFitnessValue> fitnessFunction,
			int selectionSize)
		{
			_crossover = crossover;
			_fitnessFunction = fitnessFunction;
			_selectionSize = selectionSize;
			_random = new Random();
		}

		#endregion

		#region Private Methods

		private TVector GetParent(IReadOnlyList<TVector> population)
		{
			var knights = new Dictionary<TVector, TFitnessValue>();
			var populationSize = population.Count;

			for (var i = 0; i != _selectionSize; ++i)
			{
				while (true)
				{
					var knightId = _random.Next(0, populationSize);

					var vector = population[knightId];

					if (knights.ContainsKey(vector))
					{
						// продолжаем
					}
					else
					{
						var fitnessValue = _fitnessFunction.FitnessFunction(vector);
						knights.Add(vector, fitnessValue);
						break;
					}
				}

			}

			var champion = knights.Aggregate((l, r) => l.Value.CompareTo(r.Value) == 1 ? l : r).Key;

			return champion;
		}

		#endregion

		#region IChildrenCreator

		public IReadOnlyList<TVector> CreateChildren(IReadOnlyList<TVector> population, int desiredCount)
		{
			var children = new List<TVector>();

			var totalChildrenToAdd = desiredCount;

			while (totalChildrenToAdd > 0)
			{
				var firstParent = GetParent(population);
				var secondParent = GetParent(population);

				while (firstParent.Equals(secondParent))
				{
					firstParent = GetParent(population);
				}

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
