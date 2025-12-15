using FlashCards.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FlashCards
{
    public partial class LearningSessionPanel : UserControl
    {
        // Dependency properties for binding from XAML
        public static readonly DependencyProperty WordProperty =
            DependencyProperty.Register(nameof(Word), typeof(string), typeof(LearningSessionPanel), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty SentenceProperty =
            DependencyProperty.Register(nameof(Sentence), typeof(string), typeof(LearningSessionPanel), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty PictureTextProperty =
            DependencyProperty.Register(nameof(PictureText), typeof(string), typeof(LearningSessionPanel), new PropertyMetadata(string.Empty));

        public string Word
        {
            get => (string)GetValue(WordProperty);
            set => SetValue(WordProperty, value);
        }

        public string Sentence
        {
            get => (string)GetValue(SentenceProperty);
            set => SetValue(SentenceProperty, value);
        }

        public string PictureText
        {
            get => (string)GetValue(PictureTextProperty);
            set => SetValue(PictureTextProperty, value);
        }

        private LearningSessionManager? _manager;
        private SessionCard? _current;

        public event Action? SessionFinished;

        public LearningSessionPanel()
        {
            InitializeComponent();
            this.Loaded += LearningSessionPanel_Loaded;
        }

        private void LearningSessionPanel_Loaded(object sender, RoutedEventArgs e)
        {
            // if panel is opened directly from MainWindow, create and attach manager
            if (_manager == null)
            {
                _manager = new LearningSessionManager();
                _manager.StartSession();
            }
            // show first
            ShowNextFromManager();
        }

        // Load card data into the panel (calls from manager)
        public void LoadCard(string word, string translation, string imagePath, string sentence)
        {
            Word = word ?? string.Empty;
            PictureText = string.IsNullOrWhiteSpace(imagePath) ? "No image" : System.IO.Path.GetFileName(imagePath);
            Sentence = sentence ?? translation ?? string.Empty;
        }

        // Helper overload using your Card model
        public void LoadCard(Card card)
        {
            if (card == null) return;
            LoadCard(card.Word, card.Translation, card.ImagePath, card.Sentence);
        }

        internal void AttachManager(LearningSessionManager manager)
        {
            _manager = manager;
        }

        // Called by container to request next card from manager and display it
        public void ShowNextFromManager()
        {
            if (_manager == null)
            {
                Word = "No session";
                Sentence = string.Empty;
                PictureText = string.Empty;
                return;
            }

            var next = _manager.Next();
            if (next == null || next.Card == null)
            {
                Word = "Session finished";
                Sentence = string.Empty;
                PictureText = string.Empty;
                SessionFinished?.Invoke();
                return;
            }

            _current = next;
            // ensure UI shows front side by default
            if (_current.IsFlipped)
            {
                // if session card persisted flipped state, show translation
                LoadCard(_current.Card?.Translation ?? string.Empty, string.Empty, _current.Card?.ImagePath ?? string.Empty, _current.Card?.Sentence ?? string.Empty);
            }
            else
            {
                LoadCard(_current.Card);
            }
        }

        // User interaction: flip (click on card)
        public void ToggleFlip()
        {
            if (_current == null) return;
            // let manager toggle the state
            _manager?.FlipCurrent();

            // read resulting state from _current and update UI accordingly
            if (_current.IsFlipped)
            {
                LoadCard(_current.Card?.Translation ?? string.Empty, string.Empty, _current.Card?.ImagePath ?? string.Empty, _current.Card?.Sentence ?? string.Empty);
            }
            else
            {
                LoadCard(_current.Card);
            }
        }

        // User swipe left
        public void SwipeLeft()
        {
            if (_current == null) return;
            _manager?.SwipeLeft();
            _current = null;
            ShowNextFromManager();
        }

        // User swipe right
        public void SwipeRight()
        {
            if (_current == null) return;
            _manager?.SwipeRight();
            _current = null;
            ShowNextFromManager();
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            SwipeLeft();
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            SwipeRight();
        }

        private void Card_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ToggleFlip();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("End learning session? Progress will be saved.", "End session", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var mw = Window.GetWindow(this) as MainWindow;
                if (mw != null)
                {
                    mw.LearnWordsContent.Content = null;
                    mw.Darkening.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
