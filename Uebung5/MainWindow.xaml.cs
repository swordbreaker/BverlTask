using System.Windows;

namespace Uebung5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get; }  
       
        public MainWindow()
        {
            ViewModel = new MainWindowViewModel();
            InitializeComponent();
        }


    }
}
