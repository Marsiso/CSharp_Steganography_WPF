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
using System.Windows.Shapes;

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
    }
}
