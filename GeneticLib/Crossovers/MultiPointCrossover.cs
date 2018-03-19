using System;
using System.Collections.Generic;
using System.Linq;
using GeneticLib.Base;
using Vector = GeneticLib.EquatableReadOnlyList<int>;  

namespace GeneticLib.Some
{
	public sealed class MultiPointCrossover : ICrossover<Vector>
	{
		#region Private Fields

		private readonly IReadOnlyList<int> _crossPoints;

		#endregion

		#region Constructors

		public MultiPointCrossover(IEnumerable<int> crossPoints)
		{
			_crossPoints = crossPoints.OrderBy(x=>x).ToList();
		}

		#endregion

		#region ICrossover

		public IReadOnlyList<Vector> Crossover(Vector first, Vector second)
		{
			var componentsCount = first.Count;

			if(componentsCount < _crossPoints.Last())
				throw new Exception();

			var child1 = new List<int>();
			var child2 = new List<int>();

			var startIndex = 0;
			var isWriteDirect = true;
			foreach (var crossPoint in _crossPoints)
			{
				var crossIndex = crossPoint;
				
				for (var i = startIndex; i != crossIndex; ++i)
				{
					if (isWriteDirect)
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

				isWriteDirect = !isWriteDirect;
				startIndex = crossPoint;
			}

			var children = new List<Vector> { new Vector(child1), new Vector(child2) };

			return children;
		}

		#endregion

	}
}
