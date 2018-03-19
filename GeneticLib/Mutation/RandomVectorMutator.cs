using System;
using System.Collections.Generic;
using GeneticLib.Base;
using Vector = GeneticLib.EquatableReadOnlyList<int>; 

namespace GeneticLib.Some
{
	public sealed class RandomVectorMutator : IVectorMutator<Vector>
	{
		#region Private Fields

		private readonly int _componentsToMutate;
		private readonly IReadOnlyList<int> _vectorMaxValues;
		private readonly Random _rand;

		#endregion

		#region Constructors

		public RandomVectorMutator(
			int componentsToMutate,
			IReadOnlyList<int> vectorMaxValues)
		{
			var vectorComponents = vectorMaxValues.Count;
			if(vectorComponents < componentsToMutate)
				throw new Exception();

			_componentsToMutate = componentsToMutate;
			_vectorMaxValues = vectorMaxValues;
			_rand = new Random();
		}

		#endregion

		#region IVectorMutator

		public Vector Mutate(Vector vector)
		{
			var componentsInVector = vector.Count;

			#region Выбрать компоненты для изменения

			var needMutateComponentsIds = new List<int>();
			while (true)
			{
				var idToMutate = _rand.Next(0, componentsInVector);
				var notUsedYet = !needMutateComponentsIds.Contains(idToMutate);
				if (notUsedYet)
				{
					needMutateComponentsIds.Add(idToMutate);
					var allChoosen = needMutateComponentsIds.Count == _componentsToMutate;
					if (allChoosen)
						break;
				}
				else
				{
					// продолжаем
				}
			}

			#endregion

			var mutatedVector = new List<int>(vector);

			#region Изменить компоненты

			foreach (var componentId in needMutateComponentsIds)
			{
				var maxValue = _vectorMaxValues[componentId];
				var currentValue = vector[componentId];

				var newValue = currentValue;

				while (newValue == currentValue)
				{
					newValue = _rand.Next(0, maxValue + 1);
				}

				mutatedVector[componentId] = newValue;
			}

			#endregion

			return new Vector(mutatedVector);
		}

		#endregion

	}
}
