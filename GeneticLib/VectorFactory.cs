using System;
using System.Collections.Generic;
using Vector = GeneticLib.EquatableReadOnlyList<int>; 

namespace GeneticLib
{
    public interface IFactory<TItem>
    {
        TItem Create();
    }


	public sealed class VectorFactory : IFactory<Vector>
	{
		#region Private Fields

		private readonly Vector _maxComponentValues;
		private readonly Random _random;

		#endregion

		#region Constructors

		public VectorFactory(Vector maxComponentValues)
		{
			_maxComponentValues = maxComponentValues;
			_random = new Random();
		}

		#endregion

		#region IFactory

		public Vector Create()
		{
			var componentsInVector = _maxComponentValues.Count;
			var vector = new List<int>();

			for (var j = 0; j != componentsInVector; ++j)
			{
				var maxComponentValue = _maxComponentValues[j];
				var componentValue = _random.Next(0, maxComponentValue + 1);
				vector.Add(componentValue);
			}

			return new Vector(vector);
		}

		#endregion

		
	}
}
