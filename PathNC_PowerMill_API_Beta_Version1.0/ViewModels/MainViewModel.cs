using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace API_PowerMill_Version1._1.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _statusText = "Ready";
        private double _progressValue;

        private string _selectedSetupAction;
        private string _selectedToolpath;
        private string _selectedFeature;
        private string _selectedNcSim;
        private string _selectedPlate;
        private string _selectedCore;
        private string _selectedAutomationSetup;
        private string _selectedFinishAction;
        private string _selectedDrilling;
        private string _selectedDrillMethods;


        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); }
        }

        public double ProgressValue
        {
            get => _progressValue;
            set { _progressValue = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> DrillMethodsActions { get; } = new()

            {
              "Counterbored",
            };

        public ObservableCollection<string> LogMessages { get; } = new();

        // ComboBox collections
        public ObservableCollection<string> SetupActions { get; } = new() { "Connect", "Initialize", "Close" };
        public ObservableCollection<string> AreaClearanceActions { get; } = new()
        {
            "Model Area Clearance",
            "Model Rest Area Clearance",
            "Model Rest Profile",
            "Corner Clearance"

        };
        public ObservableCollection<string> ToolpathActions { get; } = new() {
        "Feature Pocket Area Clearance",//1
        "Feature Finishing Pocket ",// 3
        "Feature Finishing Floor",  // 
        "Feature Pocket Profile",// 2
        "Feature Finish Profile", //7
        "Feature Area Clearance",  //
        "Feature Finishing Wall-Floor",
        "Feature Rest Area Clearance"// 6
        };

        public ObservableCollection<string> FinishingActions { get; } = new()
        {
            "3D Offset Profile",
            "Constant Z Finishing",
            "Offset Flat Finishing",
            "Optimized Constant Z Finishing",
            "Step and  Shallow Finishing",
            "Corner Finishing",
            "Corner Pencil Finishing",
            "3D Offset Wall-Floor",
            "Corner Finishing Wall-Floor",
            "Surface Finishing",
            "Corner Multi-Pencil-Finishing",
            "Inclined Float Finishing",
            "Raster Finishing",
            "Spiral Finishing",
        };
        public ObservableCollection<string> DrillingActions { get; } = new()
        { "Center-Drill", 
            "Drill Deep",
            "Break Chip",
            "Helical",
            "Profile",
            "Tap",
            "Thread Mill",
            "Rigid Tap",
            "Ream",
            "Fine Boring",
        
        };
        public ObservableCollection<string> NcSimActions { get; } = new() { "Simulate Toolpath", "Postprocess NC" };
        public ObservableCollection<string> AutomationPlates { get; } = new() { "P1 Plate", "Core Plate" };
        public ObservableCollection<string> AutomationCores { get; } = new() { "Core A", "Core B" };
        public ObservableCollection<string> AutomationSetups { get; } = new() { "Setup1", "Setup2" };

        // Selected items

        public string SelectedDrillMethods
        {

            get => _selectedDrillMethods;
            set { _selectedDrillMethods = value; OnPropertyChanged(); }
        }
        public string SelectedSetupAction
        {
            get => _selectedSetupAction;
            set { _selectedSetupAction = value; OnPropertyChanged(); }
        }

        public string SelectedFinishing
        {

            get => _selectedFinishAction;
            set { _selectedFinishAction = value; OnPropertyChanged(); }
        }

        public string SelectedAreaClearance
        {

            get => _selectedSetupAction;
            set { _selectedSetupAction = value; OnPropertyChanged(); }

        }

        public string SelectedToolpath
        {
            get => _selectedToolpath;
            set { _selectedToolpath = value; OnPropertyChanged(); }
        }

        public string SelectedDrilling
        {
            get => _selectedDrilling;
            set { _selectedDrilling = value; OnPropertyChanged(); }
        }

        public string SelectedNcSim
        {
            get => _selectedNcSim;
            set { _selectedNcSim = value; OnPropertyChanged(); }
        }

        public string SelectedPlate
        {
            get => _selectedPlate;
            set { _selectedPlate = value; OnPropertyChanged(); }
        }

        public string SelectedCore
        {
            get => _selectedCore;
            set { _selectedCore = value; OnPropertyChanged(); }
        }

        public string SelectedAutomationSetup
        {
            get => _selectedAutomationSetup;
            set { _selectedAutomationSetup = value; OnPropertyChanged(); }
        }

        // Commands
        public ICommand AboutCommand { get; }
        public ICommand HelpCommand { get; }
        public ICommand RunSetupCommand { get; }
        public ICommand SaveSetupCommand { get; }
        public ICommand QuitSetupCommand { get; }
        public ICommand RunToolpathCommand { get; }
        public ICommand RunDrillingCommand { get; }
        public ICommand RunNcSimCommand { get; }
        public ICommand RunAutomationCommand { get; }
        public ICommand ClearLogCommand { get; }
        public ICommand RunAreaClearanceCommand { get; }
        public ICommand RunFinishingCommand { get; }
        public ICommand OpenCalculatorCommand { get; }
        public ICommand RunDrillMethodCommand {  get; }


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public MainViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        {
            AboutCommand = new RelayCommand(_ => AddLog("About clicked"));
            HelpCommand = new RelayCommand(_ => AddLog("Help clicked"));
            RunSetupCommand = new RelayCommand(_ => AddLog($"Running setup: {SelectedSetupAction}"));
            SaveSetupCommand = new RelayCommand(_ => AddLog("Setup saved"));
            QuitSetupCommand = new RelayCommand(_ => AddLog("Quit setup"));
            RunToolpathCommand = new RelayCommand(_ => OnRunToolpathRequested?.Invoke(SelectedToolpath));
            RunAreaClearanceCommand = new RelayCommand(_ => OnRunAreaClearanceRequested?.Invoke(SelectedAreaClearance));
            RunFinishingCommand = new RelayCommand(_ => OnRunFinishingCommandRequested?.Invoke(SelectedFinishing));
            RunDrillingCommand = new RelayCommand(_ => OnRunDrillingCommandRequested?.Invoke(SelectedDrilling));
            RunDrillMethodCommand = new RelayCommand(_ => OnRunDrillMethodCommandRequested?.Invoke(SelectedDrillMethods));
            RunNcSimCommand = new RelayCommand(_ => AddLog($"Running NC/Sim: {SelectedNcSim}"));
            RunAutomationCommand = new RelayCommand(_ => AddLog($"Automation started: {SelectedPlate}, {SelectedCore}, {SelectedAutomationSetup}"));
            ClearLogCommand = new RelayCommand(_ => LogMessages.Clear());
         
        }
        public event Action<string> OnRunToolpathRequested;
        public event Action<string> OnRunAreaClearanceRequested;
        public event Action<string> OnRunFinishingCommandRequested;
        public event Action<string> OnRunDrillingCommandRequested;
        public event Action<string> OnRunDrillMethodCommandRequested;




        private void AddLog(string message)
        {
            var line = $"[{DateTime.Now:HH:mm:ss}] {message}";
            LogMessages.Add(line);
            StatusText = message;
            ProgressValue = (ProgressValue + 20) % 100; // Simulated progress
        }

#pragma warning disable CS8612 // Nullability of reference types in type doesn't match implicitly implemented member.
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS8612 // Nullability of reference types in type doesn't match implicitly implemented member.
        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}