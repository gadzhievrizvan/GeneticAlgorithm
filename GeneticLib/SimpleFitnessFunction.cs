using System;
using System.Collections.Generic;
using GeneticLib.Base;

namespace GeneticLib.Some
{
	public sealed class SimpleFitnessFunction<TVector> : IFitnessFunction<TVector, double>
		where TVector : IEquatable<TVector>
	{
		#region Private Fields

		private readonly Dictionary<TVector, double> _mappingVectorToFitnessValue;
		private readonly IFitnessFunctionHelper<TVector, DoublePoint> _fitnessFunctionHelper;
		private readonly bool _shouldCache;

		private readonly List<Parameters> _parameterses; 


		#endregion

		#region Constructors

		public SimpleFitnessFunction(
			IFitnessFunctionHelper<TVector,DoublePoint> fitnessFunctionHelper,
			bool shouldCache)
		{
			_fitnessFunctionHelper = fitnessFunctionHelper;
			_shouldCache = shouldCache;
			_mappingVectorToFitnessValue = new Dictionary<TVector, double>();

			var random = new Random();

			_parameterses = new List<Parameters>();

			for (var i = 0; i != 40; ++i)
			{
				double a = random.NextDouble() * 6 - 3;
				double b = random.NextDouble() * 6 - 3;
				double c = random.NextDouble() * 6 - 3;

				_parameterses.Add(new Parameters(a,b,c));
			}

		}

		#endregion

		#region Private Static Methods

		private double SixDomeFunction(DoublePoint doublePoint)
		{
			double sum = 0;
			foreach (var parameters in _parameterses)
			{
				sum += DomeFunction(doublePoint, parameters.A, parameters.B, parameters.C, 10);
			}
			return sum;


			//var f1 = DomeFunction(x, y, 0, 0, -3);
			//var f2 = DomeFunction(x, y, 2, 2, 2);
			//var f3 = DomeFunction(x, y, -2, -2, 2);
			//var f4 = DomeFunction(x, y, -1, 2, 3);
			//var f5 = DomeFunction(x, y, 1, -2, 2);
			//var f6 = DomeFunction(x, y, 2, 0, 2);

			//return f1 + f2 + f3 + f4 + f5 + f6;
		}

		private static double DomeFunction(DoublePoint doublePoint, double a, double b, double c, double factor)
		{
			var xa2 = Math.Pow(doublePoint.X - a, 2);
			var yb2 = Math.Pow(doublePoint.Y - b, 2);

			var expression = -factor * (xa2 + yb2);
			var value = c*Math.Exp(expression);
			return value;
		}

		#endregion

		#region IFitnessFunction

		public double FitnessFunction(TVector vector)
		{
			double fitnessValue;
			var isSuccess = _mappingVectorToFitnessValue.TryGetValue(vector, out fitnessValue);

			if (isSuccess)
			{
				// взяли из коллекции => не надо считать
			}
			else
			{
				var point = _fitnessFunctionHelper.Convert(vector);
				fitnessValue = SixDomeFunction(point);

				if (_shouldCache)
					_mappingVectorToFitnessValue.Add(vector, fitnessValue);
			}

			return fitnessValue;
		}

		#endregion

		private struct Parameters
		{
			private readonly double _a;
			private readonly double _b;
			private readonly double _c;

			public Parameters(double a, double b, double c)
			{
				_a = a;
				_b = b;
				_c = c;
			}

			public double A => _a;

		    public double B => _b;

		    public double C => _c;
		}
	}


}
