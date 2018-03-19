using System;
using System.Collections.Generic;
using Vector = GeneticLib.EquatableReadOnlyList<int>;

namespace GeneticLib.Base
{
    public sealed class GeneticAlgoHelper<TFitnessValue> : IGenerationAlgoHelper<Vector, TFitnessValue>
		where TFitnessValue:IComparable<TFitnessValue>
	{
		#region Private Fields

		private readonly IPopulationMutator<Vector> _populationMutator;
		private readonly INextGenerationCreator<Vector> _nextGenerationCreator;
		private readonly IFitnessFunction<Vector, TFitnessValue> _fitnessFunction;
		private readonly IChildrenCreator<Vector> _childrenCreator;
		private readonly IPopulationCreator<Vector> _populationCreator;

		#endregion

		#region Constructors

		public GeneticAlgoHelper(
			IPopulationMutator<Vector> populationMutator,
			INextGenerationCreator<Vector> nextGenerationCreator,
			IFitnessFunction<Vector, TFitnessValue> fitnessFunction,
			IChildrenCreator<Vector> childrenCreator,
			IPopulationCreator<Vector> populationCreator)
		{
			_populationMutator = populationMutator;
			_nextGenerationCreator = nextGenerationCreator;
			_fitnessFunction = fitnessFunction;
			_childrenCreator = childrenCreator;
			_populationCreator = populationCreator;
		}

		#endregion

		#region IGenerationAlgoHelper

		public IReadOnlyList<Vector> MutatePopulation(
			IReadOnlyList<Vector> population,
			int mutateCount)
		{
			return _populationMutator.MutatePopulation(population, mutateCount);
		}

		public IReadOnlyList<Vector> CreateFirstPopulation()
		{
			return _populationCreator.CreateFirstPopulation();
		}

		public TFitnessValue FitnessFunction(Vector vector)
		{
			return _fitnessFunction.FitnessFunction(vector);
		}

		public IReadOnlyList<Vector> CreateNextGeneration(IReadOnlyList<Vector> population, IReadOnlyList<Vector> children)
		{
			return _nextGenerationCreator.CreateNextGeneration(population, children);
		}

		public IReadOnlyList<Vector> CreateChildren(IReadOnlyList<Vector> population, int desiredCount)
		{
			return _childrenCreator.CreateChildren(population, desiredCount);
		}

		#endregion

	}
}
