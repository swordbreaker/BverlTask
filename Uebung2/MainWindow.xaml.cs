using System.Windows;
using Uebung2.ViewModels;

namespace Uebung2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get; set; }

        public MainWindow()
        {
            ViewModel = new MainWindowViewModel();
            InitializeComponent();
        }
    }
}
