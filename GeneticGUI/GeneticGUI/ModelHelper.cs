using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GeneticLib;
using GeneticLib.Some;
using GeneticLib.Base;
using Vector = GeneticLib.EquatableReadOnlyList<int>;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using GeneticLib.ChildrenCreator;
using GeneticLib.PopulationSelection;
using GeneticLib.Spies;
using Color = System.Drawing.Color;
using Point = System.Windows.Point;

namespace GeneticGUI
{
    public sealed class ModelHelper
    {
        #region Private Static Fields

        private static readonly IReadOnlyList<ChildrenStruct<Vector>> EmptyChildrenList;
        private static readonly IReadOnlyList<MutantStruct<Vector>> EmptyMutantList;

        #endregion

        #region Private Fields

        private readonly IFitnessFunctionHelper<Vector, DoublePoint> _fitnessFunctionHelper;
        private readonly SimpleFitnessFunction<Vector> _fitnessFunction;
        private readonly int _nodeCount;
        private readonly Color _backgroungColor;
        private readonly Color _foregroundColor;

        private readonly object _locker;


        private readonly List<StateStruct<Vector>> _states;

        #endregion
        
        #region Constructors

        static ModelHelper()
        {
            EmptyChildrenList = new List<ChildrenStruct<Vector>>();
            EmptyMutantList = new List<MutantStruct<Vector>>();
        }

        public ModelHelper(
            int nodeCount, 
            Color backgroungColor, 
            Color foregroundColor, 
            DoublePoint min,
            DoublePoint max)
        {
            _fitnessFunctionHelper = new FitnessFunctionHelper(min, max, nodeCount);
            _fitnessFunction = new SimpleFitnessFunction<Vector>(_fitnessFunctionHelper, true);
            _backgroungColor = backgroungColor;
            _foregroundColor = foregroundColor;
            _nodeCount = nodeCount;
            _locker = new object();

            _states = new List<StateStruct<Vector>>();
        }

        #endregion
        
        #region Properties

        public IReadOnlyList<StateStruct<Vector>> States => _states;

        #endregion
        
        #region Public Methods

        public async Task<ImageSource> CreateFunctionImage()
        {
            double[,] matrix = new double[_nodeCount, _nodeCount];
            var maxFunctionValue = double.MinValue;
            var minFunctionValue = double.MaxValue;
            var maxPoint = new Point(int.MinValue, int.MinValue);
            Bitmap bitmap = new Bitmap(_nodeCount, _nodeCount);

            await Task.Run(() => //Task.Run - выполняется асинхронно
            {
                for (var y = 0; y < _nodeCount; y++)
                {
                    for (var x = 0; x < _nodeCount; x++)
                    {
                        var vector = new Vector(new List<int> { x, y });
                        var value = _fitnessFunction.FitnessFunction(vector);
                        matrix[y, x] = value;
                        if (value > maxFunctionValue)
                        {
                            maxFunctionValue = value;
                            maxPoint = new Point(x, y);
                        }
                        if (value < minFunctionValue)
                            minFunctionValue = value;
                    }
                }

                var differenceMaxMin = maxFunctionValue - minFunctionValue;

                bitmap = CreateBitmap(matrix, minFunctionValue, differenceMaxMin);
            });

            var imageSource = ImageSourceForBitmap(bitmap);
            return imageSource;
        }

        public bool TryGetState(int desiredStateIndex, out StateStruct<Vector> targetState)
        {
            lock (_locker)
            {
                var statesCount = _states.Count;
                if (desiredStateIndex < statesCount)
                {
                    targetState = _states[desiredStateIndex];
                    return true;
                }
                else
                {
                    targetState = default(StateStruct<Vector>);
                    return false;
                }
            }
        }

        public GeneticVector<Vector, double> CreateFinderAndStart(int iterationsCount = 1000)
        {
            #region Константы

            const int startPopulationSize = 10;
            const double eliteCoef = 0.6;
            const double crossoverChance = 0.85;
            const double mutationChance = 0.15;
            const int childrenCount = 8;
            const int mutantsCount = 1;

            const int maxComponentsToMutate = 2;
            const int crossoverBreakNumber = 1;

            #endregion

            var constants = new GeneticAlgoConstants(
                startPopulationSize,
                crossoverChance,
                mutationChance,
                childrenCount,
                mutantsCount);

            var maxComponentValues = new Vector(
                new List<int>
                {
                    _nodeCount,
                    _nodeCount
                });

            var vectorFactory = new VectorFactory(maxComponentValues);

            #region Мутаторы

            var vectorMutator = new RandomVectorMutator(maxComponentsToMutate, maxComponentValues);
            var vectorMutatorSpy = new MutantSpy<Vector>(vectorMutator);
            var populationMutator = new WeakestMutator<Vector, double>(vectorMutatorSpy, _fitnessFunction);

            #endregion

            #region Создание детей
            
            var lineCrossover = new LineCrossover();
            var crossoverSpy = new CrossoverSpy<Vector>(lineCrossover);

            var rankChildrenCreator = new RankChildrenCreator<Vector>(crossoverSpy, _fitnessFunction);

            #endregion

            #region Создание поколений

            #region Начальное поколение

            var populationCreator = new PopulationCreator<Vector>(vectorFactory, startPopulationSize);
            var populationCreatorSpy = new PopulationCreatorSpy<Vector>(populationCreator);

            #endregion

            #region Последующие
            
            var variousSelection = new VariousSelection<Vector, double>(_fitnessFunction, vectorFactory, 0.7, 0.0, 0.3);
            var nextGenerationSpy = new NextGenerationCreatorSpy<Vector>(variousSelection);
            
            #endregion

            #endregion

            var geneticAlgoHelper = new GeneticAlgoHelper<double>(
                populationMutator,
                nextGenerationSpy,
                _fitnessFunction,
                rankChildrenCreator,
                populationCreatorSpy);

            #region Spy Handlers

            vectorMutatorSpy.VectorMutated += VectorMutatorSpyOnVectorMutated;
            crossoverSpy.ChildrenCreated += CrossoverSpyOnChildrenCreated;
            nextGenerationSpy.NewPopulationCreated += NextGenerationSpyOnNewPopulationCreated;
            populationCreatorSpy.PopulationCreated += PopulationCreatorSpyOnPopulationCreated;

            #endregion

            var maxValueFinder = new GeneticAlgorithm<Vector, double>(constants, geneticAlgoHelper);
            var maxFinded = maxValueFinder.FindVector(iterationsCount);

            //var maxFinded = maxValueFinder.FindVector(6.25);

            return maxFinded;
        }

        public void WriteToStringBuilder(StringBuilder sb, IEnumerable<Vector> population, int width, int height)
        {
            foreach (var vector in population)
            {
                sb.AppendLine();

                var realPoint = _fitnessFunctionHelper.Convert(vector);
                var pointOnImage = ConvertToPoint(vector, width, height);
                var fitnessValue = _fitnessFunction.FitnessFunction(vector);

                sb.Append("Real coordinates: ");
                sb.AppendLine(realPoint.ToString());

                sb.Append("FitnessValue: ");
                sb.AppendLine(fitnessValue.ToString());

                var locationOnImage = $"({pointOnImage.X:0.00};{pointOnImage.Y:0.00})";

                sb.Append("Location on image: ");
                sb.AppendLine(locationOnImage);
            }
        }

        public string GetMaxVectorString(GeneticVector<Vector, double> maxVector)
        {
            var realPointMax = _fitnessFunctionHelper.Convert(maxVector.Vector);
            var sb = new StringBuilder();
            sb.Append("Max found value: ");
            sb.AppendLine(maxVector.FitnessValue.ToString());
            sb.Append("Location: ");
            sb.AppendLine(realPointMax.ToString());
            sb.AppendLine();

            return sb.ToString();
        }

        public PointF ConvertToPoint(Vector vector, int width, int height)
        {
            var isWidthMax = width > height;
            var diff = Math.Abs(width - height) / 2;

            double realX = 0;
            double realY = 0;

            int coeff;
            if (isWidthMax)
            {
                coeff = height;
                realX += diff;
            }
            else
            {
                coeff = width;
                realY += diff;
            }

            realX += (double)vector[0] / _nodeCount * coeff;
            realY += (double)vector[1] / _nodeCount * coeff;

            var point = new PointF((float)realX, (float)realY);
            return point;
        }

        #endregion

        #region Private Methods

        #region SpyHandlers

        private void PopulationCreatorSpyOnPopulationCreated(object sender, IReadOnlyList<Vector> population)
        {
            lock (_locker)
            {
                var state = new StateStruct<Vector>(population.ToList(), EmptyChildrenList, EmptyMutantList);
                _states.Add(state);
            }
        }

        private void NextGenerationSpyOnNewPopulationCreated(object sender, IReadOnlyList<Vector> population)
        {

            lock (_locker)
            {
                var state = new StateStruct<Vector>(population.ToList(), EmptyChildrenList, EmptyMutantList);
                _states.Add(state);
            }
        }

        private void CrossoverSpyOnChildrenCreated(object sender, ChildrenStruct<Vector> childrenStruct)
        {
            lock (_locker)
            {
                var previousStep = _states[_states.Count - 1];

                var population = previousStep.Population;
                var children = previousStep.Children.ToList();
                var mutants = previousStep.Mutants;

                children.Add(childrenStruct);

                var state = new StateStruct<Vector>(population, children, mutants);
                _states.Add(state);

            }
        }

        private void VectorMutatorSpyOnVectorMutated(object sender, MutantStruct<Vector> mutantStruct)
        {
            lock (_locker)
            {
                var previousStep = _states[_states.Count - 1];

                var population = previousStep.Population;
                var children = previousStep.Children;
                var mutants = previousStep.Mutants.ToList();

                mutants.Add(mutantStruct);

                var state = new StateStruct<Vector>(population, children, mutants);
                _states.Add(state);
            }
        }

        #endregion

        #region ImageSource

        private Bitmap CreateBitmap(
            double[,] matrix,
            double minFunctionValue,
            double differenceMaxMin)
        {
            var bitmap = new Bitmap(_nodeCount, _nodeCount);
            for (var y = 0; y < _nodeCount; y++)
            {
                for (var x = 0; x < _nodeCount; x++)
                {
                    var value = matrix[y, x];
                    var differenceCurrentMin = value - minFunctionValue;
                    var relative = differenceCurrentMin / differenceMaxMin;
                    var pixelColor = Mix(_backgroungColor, _foregroundColor, Math.Pow(relative, 3));
                    bitmap.SetPixel(x, y, pixelColor);
                }
            }

            return bitmap;
        }

        private static int MixInt(int background, int foreground, double alpha) //alpha - прозрачность
        {
            var value = (int)(background * (1 - alpha) + foreground * alpha);
            if (value < 0)
                return 0;
            if (value > 255)
                return 255;
            return value;
        }

        private static Color Mix(Color background, Color foreground, double alpha)
        {
            var newRed = MixInt(background.R, foreground.R, alpha);
            var newGreen = MixInt(background.G, foreground.G, alpha);
            var newBlue = MixInt(background.B, foreground.B, alpha);
            var color = Color.FromArgb(255, newRed, newGreen, newBlue);
            return color;
        }


        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject([In] IntPtr hObject);

        private ImageSource ImageSourceForBitmap(Bitmap bitmap)
        {
            var handle = bitmap.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(handle);
            }
        }

        #endregion

        #endregion

        #region Nested Types

        public struct StateStruct<TVector>
        {
            private readonly IReadOnlyList<TVector> _population;
            private readonly IReadOnlyList<ChildrenStruct<TVector>> _children;
            private readonly IReadOnlyList<MutantStruct<TVector>> _mutants;

            public StateStruct(
                IReadOnlyList<TVector> population,
                IReadOnlyList<ChildrenStruct<TVector>> children,
                IReadOnlyList<MutantStruct<TVector>> mutants)
            {
                _population = population;
                _children = children;
                _mutants = mutants;
            }

            public IReadOnlyList<TVector> Population => _population;

            public IReadOnlyList<ChildrenStruct<TVector>> Children => _children;

            public IReadOnlyList<MutantStruct<TVector>> Mutants => _mutants;

        }

        #endregion
    }
}
