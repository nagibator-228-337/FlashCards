using FlashCards.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для EditCardsPanel.xaml
    /// </summary>
    public partial class EditCardsPanel : UserControl
    {
        public ObservableCollection<string> Words { get; set; } = new ObservableCollection<string>();
        public EditCardsPanel()
        {
            InitializeComponent();

            var db = new DatabaseService();
            var panel = new AddCardUCPanel();

            wordList.ItemsSource = Words;

            var cardList = db.CardReader();
            foreach (var card in cardList)
            {
                string display = $"{card.Word} — {card.Translation}";
                Words.Add(display);
            }

            //panel.LoadCardForEdit(card);
        }
    }
}
