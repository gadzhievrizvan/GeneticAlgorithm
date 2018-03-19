using System;
using System.Collections.Generic;
using GeneticLib.Base;
using Vector = GeneticLib.EquatableReadOnlyList<int>; 

namespace GeneticLib.Some
{
	public sealed class LineCrossover : ICrossover<Vector>
	{
		private readonly Random _random;

		public LineCrossover()
		{
			_random = new Random();
		}

		public IReadOnlyList<Vector> Crossover(Vector first, Vector second)
		{
			if (first.Count != 2)
				throw new Exception();

			var x1 = first[0];
			var x2 = second[0];

			var y1 = first[1];
			var y2 = second[1];

			const double treshold = 0.1;
			var alpha = treshold + _random.NextDouble()*(1 - 2*treshold);


			var coeff = 1 - alpha;
			var newX = alpha*x1 + coeff*x2;
			var newY = alpha*y1 + coeff*y2;
		    var child = new Vector(
		        new List<int>
		        {
		            (int) newX,
		            (int) newY
		        });

		    var children = new List<Vector> {child};
		    return children;
		}


	}
}
