using GeneticLib.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Color = System.Drawing.Color;

namespace GeneticGUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var modelHelper = new ModelHelper(1200, Color.Black, Color.White,
                new DoublePoint(-3, -3), new DoublePoint(3, 3));
            var viewModel = new MainViewModel(modelHelper);
            this.DataContext = viewModel;
        }

        
    }
}
