using System.Windows;
using MVVM_Vorlage;

namespace NAS.Updater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class MainWindow : Window
    {
        private MainWindowViewModel mainWindow;

        public MainWindow()
        {
            InitializeComponent();
            mainWindow = new MainWindowViewModel();
            DataContext = mainWindow;
        }
    }
}
