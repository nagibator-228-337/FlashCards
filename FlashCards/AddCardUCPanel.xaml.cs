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

namespace FlashCards
{
    /// <summary>
    /// Логика взаимодействия для AddCardUCPanel.xaml
    /// </summary>
    public partial class AddCardUCPanel : UserControl
    {

        private MainWindow? _mainWindow;
        public AddCardUCPanel()
        {
            InitializeComponent();

            this.Loaded += AddCardUCPanel_Loaded;

            InitWatermarkFor(WordTextBox);
            InitWatermarkFor(SentenceTextBox);
        }

        private static void InitWatermarkFor(TextBox box)
        {
            box.Loaded += (s, e) =>
            {
                box.ApplyTemplate();
                var wm = (TextBlock)box.Template.FindName("PART_Watermark", box);

                box.TextChanged += (_, __) => UpdateWatermarkVisibility(wm, box.Text);
                box.GotKeyboardFocus += (_, __) => 
                {
                    if (wm != null) wm.Visibility = Visibility.Collapsed;
                };
                box.LostKeyboardFocus += (_, __) => UpdateWatermarkVisibility(wm, box.Text);
                UpdateWatermarkVisibility(wm, box.Text);


            };
        }

        private static void UpdateWatermarkVisibility(TextBlock watermark, string text)
        {
            if (watermark == null) return;

            watermark.Visibility = string.IsNullOrWhiteSpace(text) ? Visibility.Visible : Visibility.Collapsed;
        }


        private void AddCardUCPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (_mainWindow == null)
                _mainWindow = Window.GetWindow(this) as MainWindow;

            if (_mainWindow != null)
            {
                this.MaxWidth = 0.7 * _mainWindow.ActualWidth;
                this.MaxHeight = 0.8 * _mainWindow.ActualHeight;
            }
        }

        private void CancelButtonClick(object? sender, RoutedEventArgs? e) 
        {
            if (_mainWindow != null)
            {
                _mainWindow.AddCardContent.Content = null;
                _mainWindow.Darkening.Visibility = Visibility.Collapsed; 
            }
        }

        private void DarkeningClick(object sender, MouseButtonEventArgs e)
        {
            CancelButtonClick(sender, null); 
        }
    }
}
