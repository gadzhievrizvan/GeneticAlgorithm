using System.Collections.Generic;
using GeneticLib.Base;
using Vector = GeneticLib.EquatableReadOnlyList<int>; 

namespace GeneticLib.Some
{
	public sealed class SinglePointCrossover : ICrossover<Vector>
	{
		#region Private Fields

		private readonly int _breakNumber;

		#endregion

		#region Constructors

		public SinglePointCrossover(int breakNumber)
		{
			_breakNumber = breakNumber;
		}

		#endregion

		#region ICrossover

		public IReadOnlyList<Vector> Crossover(Vector first, Vector second)
		{
			var componentsCount = first.Count;

			var child1 = new List<int>();
			var child2 = new List<int>();

			for (var i = 0; i < componentsCount; ++i)
			{
				if (i < _breakNumber)
				{
					child1.Add(first[i]);
					child2.Add(second[i]);
				}
				else
				{
					child1.Add(second[i]);
					child2.Add(first[i]);
				}
			}


			var children = new List<Vector> {new Vector(child1), new Vector(child2)};

			return children;
		}

		#endregion

	}
}
