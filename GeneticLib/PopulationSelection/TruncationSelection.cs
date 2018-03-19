using GeneticLib.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticLib.PopulationSelection
{
    public sealed class TruncationSelection<TVector, TFitnessValue> : INextGenerationCreator<TVector>
		where TVector : IEquatable<TVector>
		where TFitnessValue : IComparable<TFitnessValue>
	{
		#region Private Fields

		private readonly IFitnessFunction<TVector, TFitnessValue> _fitnessFunction;
		private readonly double _eliminateCoef;
		private readonly Random _random;

		/// <summary>
		/// чтобы не было new`шек
		/// </summary>
		private readonly List<GeneticVector<TVector, TFitnessValue>> _selectionParicipants;

		#endregion

		#region Constructors

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fitnessFunction"></param>
		/// <param name="eliminateCoef"> от 0 до 1, какая часть худших особей не участвует в отборе</param>
		public TruncationSelection(IFitnessFunction<TVector, TFitnessValue> fitnessFunction, double eliminateCoef)
		{
			if(eliminateCoef < 0 || eliminateCoef >=1)
				throw new Exception();

			_fitnessFunction = fitnessFunction;
			_eliminateCoef = eliminateCoef;
			_random = new Random();
			_selectionParicipants = new List<GeneticVector<TVector, TFitnessValue>>();
		}

		#endregion

		#region Private Methods

		private IEnumerable<GeneticVector<TVector, TFitnessValue>> GetGeneticVectors(IEnumerable<TVector> allVectors)
		{
			var geneticVectors = new List<GeneticVector<TVector, TFitnessValue>>();

			foreach (var vector in allVectors)
			{
				var fitnessValue = _fitnessFunction.FitnessFunction(vector);

				var geneticVector = new GeneticVector<TVector, TFitnessValue>(vector, fitnessValue);
				geneticVectors.Add(geneticVector);
			}

			return geneticVectors;
		}

		#endregion

		#region INextGenerationCreator

		public IReadOnlyList<TVector> CreateNextGeneration(IReadOnlyList<TVector> population, IReadOnlyList<TVector> children)
		{
			var allVectors = population.Concat(children);
			var vectorsCount = population.Count + children.Count;

			var geneticVectors = GetGeneticVectors(allVectors);
			var nextSelectionParticipantsCount = vectorsCount - (int)(vectorsCount * _eliminateCoef);
			if (nextSelectionParticipantsCount == 0)
				throw  new Exception();


			#region Выбрать nextSelectionParticipantsCount лучших
            
			foreach (var geneticVector in geneticVectors)
			{
				var fitnessValue = geneticVector.FitnessValue;

				var greaterThanAnySelected = _selectionParicipants.Any(x => fitnessValue.CompareTo(x.FitnessValue) == 1);

				var shouldAdd = _selectionParicipants.Count < nextSelectionParticipantsCount
				                ||
				                greaterThanAnySelected;

				var shouldRemove = _selectionParicipants.Count == nextSelectionParticipantsCount
				                   &&
				                   greaterThanAnySelected;

				if (shouldRemove)
				{
					var vectorToRemove = _selectionParicipants.First(x => x.FitnessValue.CompareTo(fitnessValue) == -1);
					_selectionParicipants.Remove(vectorToRemove);
				}

				if (shouldAdd)
					_selectionParicipants.Add(geneticVector);
			}

			#endregion

			var newPopulation = new List<TVector>();
			for (var i = 0; i != nextSelectionParticipantsCount; ++i)
			{
				var vectorIndex = _random.Next(0, nextSelectionParticipantsCount);
				var geneticVector = _selectionParicipants[vectorIndex];
				newPopulation.Add(geneticVector.Vector);
			}

			_selectionParicipants.Clear();

			return newPopulation;
		}

		#endregion
	}
}
