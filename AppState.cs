using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace BatExtension
{
    public class AppState : INotifyPropertyChanged
    {
        private static readonly Lazy<AppState> _instance = new Lazy<AppState>(() => new AppState());

        public static AppState Instance => _instance.Value;

        private bool inCombat;
        private ObservableCollection<CombatStat> _combatStats = new ObservableCollection<CombatStat>();

        private AppState()
        {
        }

        public bool InCombat
        {
            get => inCombat;
            set
            {
                if (inCombat != value)
                {
                    inCombat = value;
                    OnPropertyChanged(nameof(InCombat));
                }
            }
        }

        public ObservableCollection<CombatStat> CombatStats
        {
            get => _combatStats;
            set
            {
                if (_combatStats != value)
                {
                    _combatStats = value;
                    OnPropertyChanged(nameof(CombatStats));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddCombatStat(CombatStat combatStat)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                CombatStats.Add(combatStat);
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() => CombatStats.Add(combatStat));
            }
        }
    }
}