using FlashCards.Data;
using FlashCards.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace FlashCards
{
    public partial class AddCardUCPanel : BaseHandlerControl
    {
        private MainWindow? _mainWindow;
        private DispatcherTimer _highlightTimer;

        private int? _edittingCardId = null;

        public AddCardUCPanel()
        {
            InitializeComponent();

            this.Loaded += AddCardUCPanel_Loaded;

            InitWatermarkFor(WordTextBox);
            InitWatermarkFor(SentenceTextBox);
            InitWatermarkFor(TranslateTextBox);

            _highlightTimer = new DispatcherTimer();
            _highlightTimer.Interval = TimeSpan.FromSeconds(2);
            _highlightTimer.Tick += (s, e) =>
            {
                WordBorder.ClearValue(Border.BorderBrushProperty);
                TranslateBorder.ClearValue(Border.BorderBrushProperty);
                FlipButton.ClearValue(Border.BorderBrushProperty);
                _highlightTimer.Stop();
            };
        }

        private static void InitWatermarkFor(TextBox box)
        {
            box.Loaded += (s, e) =>
            {
                box.ApplyTemplate();
                var wm = (TextBlock)box.Template.FindName("PART_Watermark", box);
                box.TextChanged += (_, __) => UpdateWatermarkVisibility(wm, box.Text);
                box.GotKeyboardFocus += (_, __) => { if (wm != null) wm.Visibility = Visibility.Collapsed; };
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


        private void FlipButtonClick(object sender, RoutedEventArgs e)
        {
            if (FrontSide.Visibility == Visibility.Visible)
            {
                FrontSide.Visibility = Visibility.Collapsed;
                BackSide.Visibility = Visibility.Visible;
            }
            else
            {
                BackSide.Visibility = Visibility.Collapsed;
                FrontSide.Visibility = Visibility.Visible;
            }
        }

        private void CancelButtonClick(object? sender, RoutedEventArgs? e)
        {
            if (string.IsNullOrWhiteSpace(WordTextBox.Text) && string.IsNullOrWhiteSpace(TranslateTextBox.Text) && string.IsNullOrWhiteSpace(SentenceTextBox.Text))
            {
                RequestClose();
                return;
            }

            // are adnn panel is loaded?
            if (this.IsLoaded)
            {
                var result = MessageBox.Show("Discard changes?", "Cancel", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    RequestClose();
                }
            }
            else
            {
                RequestClose();
            }
        }

        protected override void OnEscPressed()
        {
            CancelButtonClick(null, null);
        }

        //closing
        public void RequestClose()
        {
            if (_mainWindow == null)
            {
                _mainWindow = Window.GetWindow(this) as MainWindow;
            }

            if (_edittingCardId == null)
            {
                if (_mainWindow != null)
                {
                    _mainWindow.AddCardContent.Content = null;
                    _mainWindow.Darkening.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                if (_mainWindow != null)
                {
                    _mainWindow.EditCardContent.Content = null;
                    _mainWindow.Darkening.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            bool hasError = false;

            WordBorder.ClearValue(Border.BorderBrushProperty);
            TranslateBorder.ClearValue(Border.BorderBrushProperty);

            ErrorLabelFront.Visibility = Visibility.Collapsed;
            ErrorLabelBack.Visibility = Visibility.Collapsed;
            StatusLabelFront.Visibility = Visibility.Collapsed;
            StatusLabelBack.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(WordTextBox.Text))
            {
                hasError = true;
                WordBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 150, 150));
            }

            if (string.IsNullOrWhiteSpace(TranslateTextBox.Text))
            {
                hasError = true;
                TranslateBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 150, 150));
            }

            if (hasError)
            {
                if (FrontSide.Visibility == Visibility.Visible)
                {
                    ErrorLabelFront.Content = "Word and Translation are required!";
                    ErrorLabelFront.Visibility = Visibility.Visible;
                }
                else
                {
                    ErrorLabelBack.Content = "Word and Translation are required!";
                    ErrorLabelBack.Visibility = Visibility.Visible;
                }

                FlipButton.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 150, 150));
                FlipButton.BorderThickness = new Thickness(2);

                _highlightTimer.Stop();
                _highlightTimer.Start();
                return;
            }

            var dbService = new DatabaseService();
            dbService.InitDataBase();

            var newCard = new Card()
            {
                Id = _edittingCardId ?? 0,
                Word = WordTextBox.Text,
                Sentence = SentenceTextBox.Text,
                Translation = TranslateTextBox.Text,
                ImagePath = "",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            if (_edittingCardId == null)
            {
                dbService.AddCard(newCard);
                StatusLabelFront.Content = "Card saved!";
                StatusLabelFront.Visibility = Visibility.Visible;

                //clearing
                WordTextBox.Text = "";
                SentenceTextBox.Text = "";
                TranslateTextBox.Text = "";
                CardImage.Source = null;

                RequestClose();
            }
            else
            {
                dbService.UpdateCard(newCard);
                StatusLabelFront.Content = "Card updated!";
                StatusLabelFront.Visibility = Visibility.Visible;
                StatusLabelBack.Content = "Card updated!";
                StatusLabelBack.Visibility = Visibility.Visible;
            }
        }


        public void LoadCardForEdit(Card card)
        {
            _edittingCardId = card.Id;

            WordTextBox.Text = card.Word;
            TranslateTextBox.Text = card.Translation;
            SentenceTextBox.Text = card.Sentence;

            SaveButton.Content = "Update";
        }
    }
}
