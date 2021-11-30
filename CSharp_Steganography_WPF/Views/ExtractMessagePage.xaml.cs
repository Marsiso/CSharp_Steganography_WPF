using CSharp_Steganography_WPF.ViewModels;
using System.Windows.Controls;
using System.Windows.Media;

namespace CSharp_Steganography_WPF.Views
{
    /// <summary>
    /// Interaction logic for ExtractMessagePage.xaml
    /// </summary>
    public partial class ExtractMessagePage : Page
    {
        readonly MainWindowViewModel mainWindowViewModel;

        private double _zoomValue = 1.0;

        public ExtractMessagePage(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            DataContext = mainWindowViewModel;
            InitializeComponent();
        }

        private void Image_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                _zoomValue += 0.05;
            }
            else
            {
                _zoomValue -= 0.05;
                if (_zoomValue < 0.05)
                    _zoomValue = 0.05;
            }
            Image? image = sender as Image;
            if (image is null)
            {
                e.Handled = true;
                return;
            }
            ScaleTransform scale = new ScaleTransform(_zoomValue, _zoomValue);
            image.LayoutTransform = scale;
            e.Handled = true;
        }
    }
}
