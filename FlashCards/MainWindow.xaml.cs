using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;
using FlashCards.Data;
using FlashCards.Models;
using System.Collections.ObjectModel;

namespace FlashCards
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddButton_click(object sender, RoutedEventArgs e)
        {
            var panel = new AddCardUCPanel();
            AddCardContent.Content = panel;
            AddCardContent.Visibility = Visibility.Visible;
            Darkening.Visibility = Visibility.Visible;
        }

        private void EditButton_click(object sender, RoutedEventArgs e)
        {
            var panel = new EditCardsPanel();
            EditCardContent.Content = panel;
            EditCardContent.Visibility = Visibility.Visible;
            Darkening.Visibility = Visibility.Visible;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = this.ActualWidth;
            double radius = width * 0.025;

            if (radius < 15) radius = 15;
            if (radius > 60) radius = 60;

            MainCardBorder.CornerRadius = new CornerRadius(radius);
        }

        private void DarkeningButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }
}