using System;
using Vector = GeneticLib.EquatableReadOnlyList<int>;  

namespace GeneticLib.Base
{
	public struct DoublePoint
	{
		#region Private Fields

		private readonly double _x;
		private readonly double _y;

		#endregion

		#region Constructors

		public DoublePoint(double x, double y)
		{
			_x = x;
			_y = y;
		}

		#endregion

		#region Properties

		public double X => _x;

	    public double Y => _y;

	    #endregion

		#region Override

		public override string ToString()
		{
			var stringPoint = $"({_x:0.000};{_y:0.000})";
			return stringPoint;
		}

		#endregion

	}

	public sealed class FitnessFunctionHelper : IFitnessFunctionHelper<Vector, DoublePoint>
	{
		#region Private Fields

		private readonly DoublePoint _min;
		private readonly DoublePoint _max;
		private readonly double _xStep;
		private readonly double _yStep;

		#endregion

		#region Constructors

		public FitnessFunctionHelper(
			DoublePoint min,
			DoublePoint max,
			int nodeCountПоКаждомуИзмерению)
		{
			_min = min;
			_max = max;

			_xStep = (_max.X - _min.X) / nodeCountПоКаждомуИзмерению;
			_yStep = (_max.Y - _min.Y) / nodeCountПоКаждомуИзмерению;
		}

		#endregion

		#region IFitnessFunctionHelper

		public DoublePoint Convert(Vector vector)
		{
			if (vector.Count != 2)
				throw new Exception();

			var shitX = vector[0];
			var shitY = vector[1];

			var realX = _min.X + shitX * _xStep;
			var realY = _min.Y + shitY * _yStep;

			var realPoint = new DoublePoint(realX, realY);
			return realPoint;
		}

		#endregion

	}
}
