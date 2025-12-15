using FlashCards.Data;
using FlashCards.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace FlashCards
{
    public partial class EditCardsPanel : UserControl
    {
        private MainWindow? _mainWindow;
        private AddCardUCPanel? _activePanel; 
        public ObservableCollection<Card> Cards { get; } = new();

        public EditCardsPanel()
        {
            InitializeComponent();

            wordList.ItemsSource = Cards;

            var db = new DatabaseService();
            foreach (var card in db.CardReader())
            {
                Cards.Add(card);
            }

            Loaded += (s, e) =>
            {
                _mainWindow = Window.GetWindow(this) as MainWindow;
            };
        }

        private void worldlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_mainWindow == null || wordList.SelectedItem == null)
                return;

            var selectedCard = (Card)wordList.SelectedItem;

            // close last penel, if it open
            if (_activePanel != null)
            {
                _activePanel.RequestClose();
            }

            _activePanel = new AddCardUCPanel();
            _activePanel.LoadCardForEdit(selectedCard);

            _mainWindow.EditCardContent.Content = _activePanel;
            _mainWindow.Darkening.Visibility = Visibility.Visible;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mainWindow == null)
                _mainWindow = Window.GetWindow(this) as MainWindow;

            if (_mainWindow != null)
            {
                _mainWindow.EditCardContent.Content = null;
                _mainWindow.Darkening.Visibility = Visibility.Collapsed;
            }
        }
    }
}
