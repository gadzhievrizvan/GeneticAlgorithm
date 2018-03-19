using System;
using System.Collections.Generic;
using System.Linq;
using GeneticLib.Base;

namespace GeneticLib
{
    public sealed class TournamentParentSelector<TVector, TFitnessValue> : IParentSelector<TVector>
		where TVector:IEquatable<TVector>
		where TFitnessValue : IComparable<TFitnessValue>
	{
		#region Private Fields

		private readonly int _selectionSize;
		private readonly IFitnessFunction<TVector, TFitnessValue> _fitnessFunction;
		private readonly Random _random;

		#endregion

		#region Constructors

		public TournamentParentSelector(
			int selectionSize,
			IFitnessFunction<TVector, TFitnessValue> fitnessFunction)
		{
			_selectionSize = selectionSize;
			_fitnessFunction = fitnessFunction;
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

		#region IParentSelector

		public IReadOnlyList<ParentStruct<TVector>> GetParents(IReadOnlyList<TVector> population, int desiredCount)
		{
			var populationSize = population.Count;

			if(populationSize <= _selectionSize)
				throw new ArgumentOutOfRangeException();

			var parents = new List<ParentStruct<TVector>>();
			for (var i = 0; i != desiredCount; ++i)
			{
				var firstParent = GetParent(population);
				var secondParent = GetParent(population);

				while (firstParent.Equals(secondParent))
				{
					firstParent = GetParent(population);
				}

				var parentStruct = new ParentStruct<TVector>(firstParent, secondParent);
				parents.Add(parentStruct);
			}


			return parents;
		}

		#endregion

	}
}
