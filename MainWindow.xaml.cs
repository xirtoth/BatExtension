using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;
using System.Text.RegularExpressions;

namespace BatExtension
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private BatListener? listener;
        private bool inCombat;

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool InCombat
        {
            get => inCombat;
            private set
            {
                if (inCombat != value)
                {
                    inCombat = value;
                    OnPropertyChanged(nameof(InCombat));
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this; // Set DataContext to MainWindow
            listener = new BatListener();
            listener.Running = true;
            listener.LineReceived += OnLineReceived; // Subscribe to the event
            LineProcessor.Initialize(SetCombatState);
            Task.Run(() => listener.Listen());
            OpenCombatWindow();
        }

        private void OpenCombatWindow()
        {
            var combatWindow = new CombatWindow(AppState.Instance);
            combatWindow.Show();
        }

        // Method to handle the LineReceived event
        private void OnLineReceived(string line)
        {
            // Use Dispatcher to update the UI from the background thread
            Dispatcher.Invoke(() =>
            {
                var (paragraph, inCombat) = LineProcessor.ProcessLine(line);
                InCombat = inCombat; // Update the InCombat property
                OutputRichTextBox.Document.Blocks.Add(paragraph);
                OutputRichTextBox.ScrollToEnd();
            });
        }

        private void SetCombatState(bool inCombat)
        {
            Dispatcher.Invoke(() => InCombat = inCombat);
            AppState.Instance.InCombat = InCombat;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}