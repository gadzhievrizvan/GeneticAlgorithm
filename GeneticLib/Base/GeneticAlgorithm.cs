using System;
using System.Collections.Generic;

namespace GeneticLib.Base
{
	public sealed class GeneticAlgorithm<TVector, TFitnessValue>
		where TVector : IEquatable<TVector>
		where TFitnessValue : IComparable<TFitnessValue>
	{
		#region Static Fields

		private static readonly IReadOnlyList<TVector> EmptyChildrenList;

		#endregion

		#region Private Fields

		private readonly Random _random;
		private readonly GeneticAlgoConstants _constants;
		private readonly IGenerationAlgoHelper<TVector, TFitnessValue> _helper;


		private GeneticVector<TVector, TFitnessValue> _maxVector;

		#endregion

		#region Constructors

		static GeneticAlgorithm()
		{
			EmptyChildrenList = new List<TVector>();
		}

		public GeneticAlgorithm(
			GeneticAlgoConstants constants,
			IGenerationAlgoHelper<TVector, TFitnessValue> helper)
		{
			_constants = constants;
			_helper = helper;
			_random = new Random();
		}

		#endregion

		#region Public Methods

        /// <summary>
        /// Можно сделать 1 метод вместо 2х принимая Func<TFitnessValue, int, bool>
        /// </summary>
        /// <param name="iterationsCount"></param>
        /// <returns></returns>
		public GeneticVector<TVector, TFitnessValue> FindVector(int iterationsCount)
		{
			if (iterationsCount <= 0)
				throw new ArgumentException();

			var iteration = 0;
			var population = _helper.CreateFirstPopulation();
			_maxVector = InitMaxVector(population);


			while (true)
			{
				var shouldCreateChildren = CheckCanCreateChildren();
				var shouldMutate = CheckCanMutate();

				IReadOnlyList<TVector> children = null;

				if (shouldCreateChildren)
				{
					children = _helper.CreateChildren(population, _constants.ChildrenCount);
					UpdateMaxVector(children);
				}

				if (shouldMutate)
				{
					population = _helper.MutatePopulation(population, _constants.MutantsCount);
					UpdateMaxVector(population);
				}
                
				iteration++;
				if (iteration == iterationsCount)
					break;

				if (shouldCreateChildren)
				{
					population = _helper.CreateNextGeneration(population, children);
				}
				else
				{
					population = _helper.CreateNextGeneration(population, EmptyChildrenList);
				}
				UpdateMaxVector(population);
			}

			return _maxVector;
		}

		public GeneticVector<TVector, TFitnessValue> FindVector(TFitnessValue treshold)
		{
			var population = _helper.CreateFirstPopulation();
			_maxVector = InitMaxVector(population);

			while (true)
			{
				var shouldStop = CheckTreshold(treshold);
				if (shouldStop)
					return _maxVector;

				var shouldCreateChildren = CheckCanCreateChildren();
				var shouldMutate = CheckCanMutate();

				IReadOnlyList<TVector> children = null;

				if (shouldCreateChildren)
				{
					children = _helper.CreateChildren(population, _constants.ChildrenCount);
					UpdateMaxVector(children);
				}

				if (shouldMutate)
				{
					population = _helper.MutatePopulation(population, _constants.MutantsCount);
					UpdateMaxVector(population);
				}

				if (shouldCreateChildren)
				{
					population = _helper.CreateNextGeneration(population, children);
				}
				else
				{
					population = _helper.CreateNextGeneration(population, EmptyChildrenList);
				}
				UpdateMaxVector(population);
			}
		}

		private bool CheckTreshold(TFitnessValue treshold)
		{
			return treshold.CompareTo(_maxVector.FitnessValue) < 1;
		}

		#endregion

		#region Private Methods

		private GeneticVector<TVector, TFitnessValue> InitMaxVector(IEnumerable<TVector> population)
		{
			GeneticVector<TVector, TFitnessValue>? maxVector = null;

			foreach (var vector in population)
			{
				var fitnessValue = _helper.FitnessFunction(vector);

				var greaterThanMax = false;
				if (maxVector.HasValue)
					greaterThanMax = fitnessValue.CompareTo(maxVector.Value.FitnessValue) == 1;

				if (!maxVector.HasValue || greaterThanMax)
					maxVector = new GeneticVector<TVector, TFitnessValue>(vector, fitnessValue);
			}

			return maxVector.Value;
		}

		private void UpdateMaxVector(IEnumerable<TVector> population)
		{
			foreach (var vector in population)
			{
				var fitnessValue = _helper.FitnessFunction(vector);
				var currentLess = _maxVector.FitnessValue.CompareTo(fitnessValue) == -1;
				if (currentLess)
				{
					_maxVector = new GeneticVector<TVector, TFitnessValue>(vector, fitnessValue);
				}
				else
				{
					// изменения не требуются
				}
			}
		}

		private bool CheckCanCreateChildren()
		{
			var iterationRandomValue = _random.NextDouble();
			var shouldCrossover = _constants.CrossoverChance >= iterationRandomValue;
			return shouldCrossover;
		}

		private bool CheckCanMutate()
		{
			var iterationRandomValue = _random.NextDouble();
			var shouldMutate = _constants.MutationChance >= iterationRandomValue;
			return shouldMutate;
		}

		#endregion

	}
}
