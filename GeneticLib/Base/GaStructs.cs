
namespace GeneticLib.Base
{
	public struct GeneticAlgoConstants
	{
		#region Private Fields

		private readonly int _populationSize;
		private readonly double _crossoverChance;
		private readonly double _mutationChance;
		private readonly int _childrenCount;
		private readonly int _mutantsCount;

		#endregion

		#region Constructors

		public GeneticAlgoConstants(
			int populationSize,
			double crossoverChance,
			double mutationChance,
			int childrenCount,
			int mutantsCount)
		{
			_populationSize = populationSize;
			_crossoverChance = crossoverChance;
			_mutationChance = mutationChance;
			_childrenCount = childrenCount;
			_mutantsCount = mutantsCount;
		}

		#endregion

		#region Properties

		public int PopulationSize => _populationSize;

	    public double CrossoverChance => _crossoverChance;

	    public double MutationChance => _mutationChance;

	    public int ChildrenCount => _childrenCount;

	    public int MutantsCount => _mutantsCount;

	    #endregion

	}

	public struct GeneticVector<TVector, TFitnessValue>
	{
		#region Private Fields

		private readonly TVector _vector;
		private readonly TFitnessValue _fitnessValue;

		#endregion

		#region Constructors

		public GeneticVector(TVector vector, TFitnessValue fitnessValue)
		{
			_vector = vector;
			_fitnessValue = fitnessValue;
		}

		#endregion

		#region Properties

		public TVector Vector => _vector;

	    public TFitnessValue FitnessValue => _fitnessValue;

	    #endregion

	}

	public struct ParentStruct<TVector>
	{
		#region Private Fields

		private readonly TVector _first;
		private readonly TVector _second;

		#endregion

		#region Constructors

		public ParentStruct(TVector first, TVector second)
		{
			_first = first;
			_second = second;
		}

		#endregion

		#region Properties

		public TVector First => _first;

	    public TVector Second => _second;

	    #endregion

	}

}
