using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace CSharp_Steganography_WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        public MainWindowView()
        {
            InitializeComponent();
            Pages.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            Pages.Content = new HideMessagePage(MainWindowViewModel);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton.Equals(MouseButton.Left) && WindowState is not WindowState.Maximized)         
                DragMove();
        }

        private void Btn_Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void Btn_Maximize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState is WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;

        private void Btn_Close_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Btn_HideMessage_Click(object sender, RoutedEventArgs e)
        {
            Btn_HideMessage.Opacity = 1;
            Btn_ExtractMessage.Opacity = 0.25;
            Pages.Content = new HideMessagePage(MainWindowViewModel);
        }

        private void Btn_ExtractMessage_Click(object sender, RoutedEventArgs e)
        {
            Btn_ExtractMessage.Opacity = 1;
            Btn_HideMessage.Opacity = 0.25;
            Pages.Content = new ExtractMessagePage(MainWindowViewModel);
        }
    }
}
