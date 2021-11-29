using CSharp_Steganography_WPF.ViewModels;
using System.Windows.Controls;

namespace CSharp_Steganography_WPF.Views
{
    /// <summary>
    /// Interaction logic for ExtractMessagePage.xaml
    /// </summary>
    public partial class ExtractMessagePage : Page
    {
        readonly MainWindowViewModel mainWindowViewModel;

        public ExtractMessagePage(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            DataContext = mainWindowViewModel;
            InitializeComponent();
        }
    }
}
