using System;
using System.Collections.Generic;

namespace GeneticLib.Base
{

    #region IVectorMutator

    public interface IVectorMutator<TVector>
		where TVector : IEquatable<TVector>
	{
		TVector Mutate(TVector vector);
	}

	#endregion

	#region IPopulationMutator

	public interface IPopulationMutator<TVector>
		where TVector : IEquatable<TVector>
	{
		IReadOnlyList<TVector> MutatePopulation(
			IReadOnlyList<TVector> population,
			int mutateCount);
	}

	#endregion

	#region ICrossover

	public interface ICrossover<TVector>
		where TVector : IEquatable<TVector>
	{
		IReadOnlyList<TVector> Crossover(
			TVector first,
			TVector second);
	}

	#endregion

	#region IParentSelector

	public interface IParentSelector<TVector>
		where TVector : IEquatable<TVector>
	{
		IReadOnlyList<ParentStruct<TVector>> GetParents(IReadOnlyList<TVector> population, int desiredCount);
	}

	#endregion

	#region IChildrenCreator

	/// <summary>
	/// Идейно это IParentSelector + ICrossover
	/// </summary>
	/// <typeparam name="TVector"></typeparam>
	public interface IChildrenCreator<TVector>
		where TVector : IEquatable<TVector>
	{
		IReadOnlyList<TVector> CreateChildren(IReadOnlyList<TVector> population, int desiredCount);
	}

	#endregion

	#region INextGenerationCreator

	public interface INextGenerationCreator<TVector, TFitnessValue>
		where TVector : IEquatable<TVector>
		where TFitnessValue : IComparable<TFitnessValue>
	{
		IReadOnlyList<TVector> CreateNextGeneration(
			IReadOnlyDictionary<TVector, TFitnessValue> population,
			IReadOnlyDictionary<TVector, TFitnessValue> children);
	}

	public interface INextGenerationCreator<TVector>
		where TVector : IEquatable<TVector>
	{
		IReadOnlyList<TVector> CreateNextGeneration(IReadOnlyList<TVector> population, IReadOnlyList<TVector> children);
	}

	#endregion

	#region IFitnessFunction

	public interface IFitnessFunction<TVector, TFitnessValue>
		where TVector : IEquatable<TVector>
		where TFitnessValue : IComparable<TFitnessValue>
	{
		TFitnessValue FitnessFunction(TVector vector);
	}

	public interface IFitnessFunctionHelper<TVector, TRealValue>
		where TVector : IEquatable<TVector>
	{
		TRealValue Convert(TVector vector);
	}

	#endregion

	#region IPopulationCreator

	public interface IPopulationCreator<TVector>
		where TVector : IEquatable<TVector>
	{
		IReadOnlyList<TVector> CreateFirstPopulation();
	}

	#endregion

	#region IGenerationAlgoHelper
    
	public interface IGenerationAlgoHelper<TVector, TFitnessValue> :
		IPopulationMutator<TVector>,
		INextGenerationCreator<TVector>,
		IChildrenCreator<TVector>,
		IFitnessFunction<TVector, TFitnessValue>,
		IPopulationCreator<TVector>
		
		where TVector : IEquatable<TVector>
		where TFitnessValue : IComparable<TFitnessValue>
	{

	}

	#endregion

}
