using GeneticLib.Base;
using SandBox.GeneticTry.ClassesForSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using GeneticLib.Some;

namespace GeneticLib.PopulationSelection
{
    public sealed class VariousSelection<TVector, TFitnessValue> : INextGenerationCreator<TVector>
		where TVector : IEquatable<TVector>
		where TFitnessValue : IComparable<TFitnessValue>
	{
		#region Private Fields

		private readonly IFitnessFunction<TVector, TFitnessValue> _fitnessFunction;
		private readonly IFactory<TVector> _vectorFactory;
		private readonly double _eliteCoef;
		private readonly double _weakCoef;
		private readonly double _recreateCoef;

		#endregion

		#region Constructors

		public VariousSelection(
			IFitnessFunction<TVector, TFitnessValue> fitnessFunction,
			IFactory<TVector> vectorFactory, 
			double eliteCoef, 
			double weakCoef, 
			double recreateCoef)
		{

			var summaryCoef = eliteCoef + weakCoef + recreateCoef;
			if(summaryCoef != 1)
				throw new ArgumentOutOfRangeException();

			_fitnessFunction = fitnessFunction;
			_vectorFactory = vectorFactory;
			_eliteCoef = eliteCoef;
			_weakCoef = weakCoef;
			_recreateCoef = recreateCoef;
		}

		#endregion

		private IReadOnlyList<GeneticVector<TVector, TFitnessValue>> GetFitnessValues(IEnumerable<TVector> population)
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

		private IEnumerable<TVector> CreateVectors(int recreateCount)
		{
			var vectors = new List<TVector>();
			for (var i = 0; i != recreateCount; ++i)
			{
				var vector = _vectorFactory.Create();
				vectors.Add(vector);
			}
			return vectors;
		}

		#region INextGenerationCreator

		public IReadOnlyList<TVector> CreateNextGeneration(IReadOnlyList<TVector> population, IReadOnlyList<TVector> children)
		{
			var targetPopulationSize = population.Count;
			var newPopulation = new List<TVector>();

			var eliteCount = (int) (targetPopulationSize*_eliteCoef);
			var weakCount = (int) (targetPopulationSize*_weakCoef);
			var recreateCount = (int) (targetPopulationSize*_recreateCoef);

			var populationWithValues = GetFitnessValues(population.Concat(children));
			var sortedPopulation = populationWithValues.OrderBy(x => x.FitnessValue).ToList();
            
			var weakest = sortedPopulation.GetRange(0, weakCount).Select(x=>x.Vector);

            var elite = sortedPopulation.Skip(Math.Max(0, sortedPopulation.Count - eliteCount)).Select(x => x.Vector);
			var newVectors = CreateVectors(recreateCount);

			newPopulation.AddRange(weakest);
			newPopulation.AddRange(elite);
			newPopulation.AddRange(newVectors);

			return newPopulation;
		}
        
		#endregion

	}
}
