using CSharp_Steganography_WPF.ViewModels;
using System.Windows.Controls;

namespace CSharp_Steganography_WPF.Views
{
    /// <summary>
    /// Interaction logic for HideMessagePage.xaml
    /// </summary>
    public partial class HideMessagePage : Page
    {
        readonly MainWindowViewModel mainWindowViewModel;

        public HideMessagePage(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            DataContext = mainWindowViewModel;
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            mainWindowViewModel.CharCounter = textBox.Text.Length.ToString() + @"/" + mainWindowViewModel.MaxLength.ToString();
        }
    }
}
