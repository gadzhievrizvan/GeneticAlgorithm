using System;
using System.Collections.Generic;
using System.Linq;
using GeneticLib.Base;

namespace GeneticLib.PopulationSelection
{
	public sealed class EliteSelection<TVector, TFitnessValue> : INextGenerationCreator<TVector>
		where TVector:IEquatable<TVector>
		where TFitnessValue:IComparable<TFitnessValue>
	{
		#region Private Fields

		private readonly IFitnessFunction<TVector, TFitnessValue> _fitnessFunction;

		#endregion

		#region Constructors

		public EliteSelection(IFitnessFunction<TVector, TFitnessValue> fitnessFunction)
		{
			_fitnessFunction = fitnessFunction;
		}

		#endregion

		#region INextGenerationCreator

		public IReadOnlyList<TVector> CreateNextGeneration(IReadOnlyList<TVector> population, IReadOnlyList<TVector> children)
		{
			var targetPopulationSize = population.Count;

			var mergedList = population.Concat(children);
			var newPopulation = new List<TVector>();


			var shitInitVector = population[0];
			var shitInitValue = _fitnessFunction.FitnessFunction(population[0]);

			// TODO: struct
			var newPopulationMinValue = new KeyValuePair<TVector, TFitnessValue>(shitInitVector, shitInitValue);
			foreach (var vector in mergedList)
			{
				var fitnessValue = _fitnessFunction.FitnessFunction(vector);
				var currentValueGreaterThanMin = newPopulationMinValue.Value.CompareTo(fitnessValue) == 1;

				var shouldRemove = currentValueGreaterThanMin && newPopulation.Count == targetPopulationSize;
				var shouldAdd = currentValueGreaterThanMin || newPopulation.Count < targetPopulationSize;

				if (shouldRemove)
				{
					newPopulation.Remove(newPopulationMinValue.Key);
					newPopulationMinValue = new KeyValuePair<TVector, TFitnessValue>(vector, fitnessValue);
				}

				if (shouldAdd)
				{
					newPopulation.Add(vector);
				}
			}

			return newPopulation;
		}

		#endregion

	}
}
