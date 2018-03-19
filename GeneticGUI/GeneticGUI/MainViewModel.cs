using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using GeneticLib;
using GeneticLib.Base;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;
using Vector = GeneticLib.EquatableReadOnlyList<int>;

//using GeneticGUI.Annotatoins;

namespace GeneticGUI
{
    public class MainViewModel : INotifyPropertyChanged
    {

        #region Private Fields

        private readonly ModelHelper _modelHelper;
        private ImageSource _canvasBackground;
        
        private readonly ObservableCollection<Shape> _currentPopulation;
        private int _valuePositionSlider;
        private int _valueSpeedSlider;
        private readonly RelayCommand _playCommand;
        private readonly RelayCommand _canvasSizeChangedCommand;
        private string _playButtonText;

        private int _canvasHeight;
        private int _canvasWidth;

        private const string _playString = "Play";
        private const string _pauseString = "Pause";

        private int _maximumPositionSlider;



        private readonly DispatcherTimer _dispatcherTimer;

        private readonly GeneticVector<EquatableReadOnlyList<int>, double> _maxVectors;
        private readonly Dictionary<Vector, PointF> _tempEllipsesPoints;

        #endregion

        #region Constructors

        public MainViewModel(ModelHelper modelHelper)
        {
            _modelHelper = modelHelper;
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += DispatcherTimerOnTick;
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            LoadedCommand = new RelayCommand(LoadedImplementation);
            _currentPopulation = new ObservableCollection<Shape>();
            _playCommand = new RelayCommand(PlayCommandAction);
            
            _playButtonText = _playString;

            _maxVectors = _modelHelper.CreateFinderAndStart();
            _maximumPositionSlider = _modelHelper.States.Count - 1;
            _canvasHeight = 300;
            _canvasWidth = 300;

            _tempEllipsesPoints = new Dictionary<Vector, PointF>();
        }

        

        #endregion

        #region Properties

        public int CanvasHeight
        {
            get => _canvasHeight;
            set => _canvasHeight = value;
        }

        public int CanvasWidth
        {
            get => _canvasWidth;
            set => _canvasWidth = value;
        }

        public string MaxVectors
        {
            get
            {
                var point = _modelHelper.ConvertToPoint(_maxVectors.Vector, _canvasWidth, _canvasHeight);
                return $"({point.X},{point.Y}) {_maxVectors.FitnessValue}";
            }
            set => OnPropertyChanged();
        }
        public int MaximumPositionSlider
        {
            get => _maximumPositionSlider;
            set
            {
                _maximumPositionSlider = value;
                OnPropertyChanged();
            }
        }

        public string PlayButtonText
        {
            get => _playButtonText;
            set
            {
                _playButtonText = value;
                OnPropertyChanged();
            }
        }

        public ICommand PlayCommand
        {
            get => _playCommand;
        }

        public ObservableCollection<Shape> CurrentPopulation
        {
            get => _currentPopulation;
        }

        public int ValuePositionSlider
        {
            get => _valuePositionSlider;
            set
            {
                _valuePositionSlider = value;
                
                ChangePosition();

            }
        }

        public int ValueSpeedSlider
        {
            get => _valueSpeedSlider;
            set
            {
                _valueSpeedSlider = value;
               
                ChangeSpeed();
            }
        }

        public ICommand LoadedCommand { get; set; }

        public ImageSource CanvasBackground
        {
            get { return _canvasBackground; }
            set
            {
                _canvasBackground = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Private Methods

        private void DispatcherTimerOnTick(object sender, EventArgs eventArgs)
        {
            if (_valueSpeedSlider < _maximumPositionSlider)
                ValuePositionSlider++;

        }
      

        private void ChangeSpeed()
        {
            const int slowestInterval = 2000;
            var stepCoef = 1;
            _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(slowestInterval - (stepCoef * _valueSpeedSlider));
        }

        private void ChangePosition()
        {
            ModelHelper.StateStruct<Vector> state;
            var getSuccess = _modelHelper.TryGetState(_valuePositionSlider, out state);
            if (getSuccess)
            {
                _currentPopulation.Clear();
                DrawState(state);
            }
        }
        private void PlayCommandAction()
        {
            if (_playButtonText == _playString)
            {
                PlayButtonText = _pauseString;
                _dispatcherTimer.Start();
            }
            else
            {
                PlayButtonText = _playString;
                _dispatcherTimer.Stop();
            }
        }

        private void DrawState(ModelHelper.StateStruct<Vector> targetState)
        {
            var population = targetState.Population;
            var childrenStructs = targetState.Children;
            var mutantsStructs = targetState.Mutants;
            var populationBrush = Brushes.Yellow;
            var childrenBrush = Brushes.Aqua;
            var mutantBrush = Brushes.Red;
            var ellipseSize = new Point(3, 3);

            #region Population

            foreach (var vector in population)
            {
                var point = _modelHelper.ConvertToPoint(vector, _canvasWidth, _canvasHeight);
                
                AddEllipse(populationBrush, point,ellipseSize);
                
                if (!_tempEllipsesPoints.ContainsKey(vector))
                    _tempEllipsesPoints.Add(vector, point);
                
            }

            #endregion

            #region Childrens

            for (var i = 0; i != childrenStructs.Count; ++i)
            {
                var childrenStruct = childrenStructs[i];

                var firstParent = childrenStruct.FirstParent;
                var secondParent = childrenStruct.SecondParent;
                var children = childrenStruct.Children;

                var parent1Point = _tempEllipsesPoints[firstParent];
                var parent2Point = _tempEllipsesPoints[secondParent];

                foreach (var child in children)
                {
                    var childPoint = _modelHelper.ConvertToPoint(child, _canvasWidth, _canvasHeight);

                    AddEllipse(childrenBrush,childPoint,ellipseSize);
                    
                    var shouldDrawLines = i == (childrenStructs.Count - 1)
                                          &&
                                          mutantsStructs.Count == 0;
                    
                    if (shouldDrawLines)
                    {
                        AddLine(childrenBrush, childPoint, parent1Point);
                        AddLine(childrenBrush, childPoint, parent2Point);
                    }
                }
            }

            #endregion

            #region Mutants

            for (var i = 0; i != mutantsStructs.Count; ++i)
            {
                var mutantStruct = mutantsStructs[i];

                var originalVector = mutantStruct.BaseVector;
                var mutatedVector = mutantStruct.MutatedVector;

                var originalVectorPoint = _tempEllipsesPoints[originalVector];
                var mutatedVectorPoint = _modelHelper.ConvertToPoint(mutatedVector, _canvasWidth, _canvasHeight);
                
                if (i == mutantsStructs.Count - 1)
                {
                    AddEllipse(mutantBrush, mutatedVectorPoint,ellipseSize);
                    AddLine(mutantBrush, originalVectorPoint, mutatedVectorPoint);
                }
            }

            #endregion

            _tempEllipsesPoints.Clear();
        }




        private void AddEllipse(Brush brush, PointF position , Point size)
        {
            var ellipse = new Ellipse()
            {
                Fill = brush,
                Width = size.X,
                Height = size.Y
            };

            Canvas.SetTop(ellipse, position.Y);
            Canvas.SetLeft(ellipse, position.X);

            _currentPopulation.Add(ellipse);
        }

        private void AddLine(Brush brush, PointF pointStart, PointF pointEnd)
        {
            var line = new Line()
            {
                StrokeThickness = 2,
                Fill = brush,
                X1 = pointStart.X,
                X2 = pointEnd.X,
                Y1 = pointStart.Y,
                Y2 = pointEnd.Y
            };
            
            _currentPopulation.Add(line);
        }
        private async void LoadedImplementation()
        {
            CanvasBackground = await _modelHelper.CreateFunctionImage();
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            //null - если нет подписчиков
        }

        

        #endregion
    }
}
