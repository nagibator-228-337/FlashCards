using FlashCards.Models;
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
    /// Логика взаимодействия для LerningSessionPanel.xaml
    /// </summary>
    public partial class LearningSessionPanel : UserControl
    {
        private MainWindow? _mainWindow;
        public LearningSessionPanel()
        {
            InitializeComponent();
        }

    }
}
