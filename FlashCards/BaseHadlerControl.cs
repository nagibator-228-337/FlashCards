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
    public  class BaseHandlerControl : UserControl
    {
        private KeyEventHandler? _windowPreviewHandler;

        public BaseHandlerControl()
        {
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
        }

        private void OnLoaded(object? sender, RoutedEventArgs e)
        {
            var w = Window.GetWindow(this);
            if (w != null)
            {
                _windowPreviewHandler = new KeyEventHandler(Window_PreviewKeyDown);
                // subscribe to window preview so we can catch keys even when focus is inside children
                w.AddHandler(Keyboard.PreviewKeyDownEvent, _windowPreviewHandler, handledEventsToo: true);
            }
        }

        private void OnUnloaded(object? sender, RoutedEventArgs e)
        {
            var w = Window.GetWindow(this);
            if (w != null && _windowPreviewHandler != null)
            {
                w.RemoveHandler(Keyboard.PreviewKeyDownEvent, _windowPreviewHandler);
                _windowPreviewHandler = null;
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                OnEscPressed();
                e.Handled = true;
            }
        }

        protected virtual void OnEscPressed()
        {
        }
    }
}