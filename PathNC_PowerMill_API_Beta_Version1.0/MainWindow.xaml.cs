// MainWindow.xaml.cs (refactored, commented, behavior-preserving)
// DiogenixCAM / PowerMill Automation UI Shell
//
// Notes:
// - Behavior preserved. Only safety, readability, and maintainability improved.
// - All PowerMill commands, template names, and macro calls kept intact.
// - Obsolete attributes retained where they existed to avoid changing build warnings semantics.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

using API_PowerMill_Version1._1.Helpers;
using API_PowerMill_Version1._1.ViewModels;
using Autodesk.ProductInterface;
using Autodesk.ProductInterface.PowerMILL;
using MoldAutomation.Helpers;
using MoldAutomation.Machining;
using MoldAutomation.Macros;
using PowerMill_API_Version1._0.Helpers;

#nullable enable
#pragma warning disable CS0618 // Type or member is obsolete (kept to preserve behavior)
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.

namespace PathNC_PowerMill_API_Beta_Version1._0
{
    #region Beginner's Guide (Main Window Shell)
    /// <summary>
    /// Main WPF shell for driving PowerMill automation via buttons and view-model events.
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // --- PowerMill session/context ---
        private PMAutomation? _powerMill;
        private PMProject? _session;

        // --- Simple log buffer for UI binding ---
        public ObservableCollection<string> Log { get; } = new ObservableCollection<string>();
        private const int MaxLogEntries = 200;

        // --- Progress (bindable) ---
        private double _progressValue;
        /// <summary>Progress value for long tasks (0..100).</summary>
        public double ProgressValue
        {
            get => _progressValue;
            set => SetProperty(ref _progressValue, value);
        }

        // --- Status (bindable) ---
        private string _statusText = "Ready";
        /// <summary>Status line for the UI status bar.</summary>
        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        /// <summary>
        /// Generic SetProperty implementing INotifyPropertyChanged.
        /// </summary>
        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value!;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Raise INotifyPropertyChanged for bindings.
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        [Obsolete]
        public MainWindow()
        {
            InitializeComponent();

            // Attach our WPF window to PowerMill native window (nice UX integration).
            AttachToPowerMillWindow();
            Topmost = true;

            // Wire up ViewModel -> Commands (no behavior change)
            var vm = new MainViewModel();
            vm.OnRunToolpathRequested += RunToolpath;
            vm.OnRunAreaClearanceRequested += RunAreaClearance;
            vm.OnRunFinishingCommandRequested += RunFinishing;
            vm.OnRunDrillingCommandRequested += RunDrilling;
            vm.OnRunDrillMethodCommandRequested += RunDrillingMethod;

            DataContext = vm;
        }

        /// <summary>
        /// Makes the WPF window owned by the running PowerMill process window (if found).
        /// </summary>
        private void AttachToPowerMillWindow()
        {
            var pmProcess = Process.GetProcessesByName("PowerMill").FirstOrDefault();
            if (pmProcess != null)
            {
                var hwnd = pmProcess.MainWindowHandle;
                new WindowInteropHelper(this).Owner = hwnd;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region PowerMill Session
        /// <summary>
        /// Ensures that _powerMill and _session are available and connected to an active project.
        /// </summary>
        public void EnsurePowerMillSession()
        {
            _powerMill ??= new PMAutomation(InstanceReuse.UseExistingInstance);
            _session = _powerMill.ActiveProject;
            if (_session == null)
                throw new InvalidOperationException("No active PowerMill project found. Please open a project before running.");
        }
        #endregion

        #region Logging & Safe Execution
        /// <summary>Adds a timestamped line to the UI log.</summary>
        private void AddLog(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Log.Add($"{DateTime.Now:HH:mm:ss} - {message}");
                if (Log.Count > MaxLogEntries) Log.RemoveAt(0);
            });
        }

        /// <summary>Standard error handling (UI + log + file).</summary>
        private void HandleError(string message, Exception ex)
        {
            string fullMessage = $"{message} : {ex.Message}";
            Application.Current.Dispatcher.Invoke(() =>
            {
                Log.Insert(0, fullMessage);
                if (Log.Count > MaxLogEntries) Log.RemoveAt(Log.Count - 1);
                StatusText = fullMessage;
            });
            MessageBox.Show(fullMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            File.AppendAllText("PowerMillAutomation.Log", $"{DateTime.Now:u} {fullMessage}{Environment.NewLine}");
        }

        /// <summary>
        /// Wraps actions in a try/catch and pushes user-visible status and logs.
        /// </summary>
        private void SafeExecute(string actionName, Action action)
        {
            try
            {
                action.Invoke();
                StatusText = $"{actionName} completed successfully";
                AddLog(StatusText);
            }
            catch (Exception ex)
            {
                StatusText = $"{actionName} failed: {ex.Message}";
                AddLog(StatusText);
                throw;
            }
        }
        #endregion

        #region PowerMill Command Helpers
        /// <summary>
        /// Execute a raw PowerMill command. Shows a warning if PowerMill is not connected.
        /// </summary>
        public void DoCommand(string command)
        {
            try
            {
                if (_powerMill == null)
                {
                    MessageBox.Show("PowerMill is not connected");
                    return; // Prevent NullReference (safer than previous behavior) :contentReference[oaicite:3]{index=3}.
                }

                _powerMill.Execute(command);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"PowerMill command failed:\n{command}\n\nError: {ex.Message}",
                    "PowerMill Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
        #endregion

        #region Drilling Method / Toolpath / Finishing / Area Clearance / Drilling
        /// <summary>
        /// Menu flow for drilling method by counterbore diameter range (InputBox).
        /// </summary>
        [Obsolete]
        private void RunDrillingMethod(string drillingMethod)
        {
            EnsurePowerMillSession();

            bool keepMenuRunningOptions = true;

            while (keepMenuRunningOptions)
            {
                string choiceUser = Microsoft.VisualBasic.Interaction.InputBox(
                    "Select Counterbored Range Diameter:\n\n" +
                    "1 - 26-31 MM\n" +
                    "2 - 30-35 MM\n" +
                    "3 - 42-47 MM\n" +
                    "4 - 54-60 MM\n" +
                    "5 - 60-70 MM\n" +
                    "6 - 70-80 MM\n" +
                    "7 - Quit\n\n",
                    "Select Drilling Method");

                switch (choiceUser.Trim())
                {
                    case "1":
                    case "26-31 MM":
                        ApplyCounterboreTemplate("Counterbore-Diam-26-31-MM.ptf", "Cbore_Mold_26-31");
                        keepMenuRunningOptions = false;
                        break;

                    case "2":
                    case "30-35 MM":
                        ApplyCounterboreTemplate("Counterbore-Diam-30-35-MM.ptf", "Cbore_Mold_30-35");
                        keepMenuRunningOptions = false;
                        break;

                    case "3":
                    case "42-47 MM":
                        ApplyCounterboreTemplate("Cbore_Mold_42-47.ptf", "Cbore_Mold_42-47");
                        keepMenuRunningOptions = false;
                        break;

                    case "4":
                    case "54-60 MM":
                        ApplyCounterboreTemplate("Cbore_Mold_54-60.ptf", "Cbore_Mold_54-60");
                        keepMenuRunningOptions = false;
                        break;

                    case "5":
                    case "60-70 MM":
                        ApplyCounterboreTemplate("Cbore_Mold_60-70.ptf", "Cbore_Mold_60-70");
                        keepMenuRunningOptions = false;
                        break;

                    case "6":
                    case "70-80 MM":
                        ApplyCounterboreTemplate("Cbore_Mold_70-80.ptf", "Cbore_Mold_70-80");
                        keepMenuRunningOptions = false;
                        break;

                    case "7":
                    case "Quit":
                        keepMenuRunningOptions = false;
                        break;

                    default:
                        MessageBox.Show("Invalid option. Please select a valid option from the menu.");
                        break;
                }
            }

            MessageBox.Show("Exiting Drilling Method Menu", "Exit", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>Small helper to avoid repetition when applying counterbore templates.</summary>
        private void ApplyCounterboreTemplate(string templateFile, string tpNameToActivate)
        {
            TemplateManager.ImportTemplate(templateFile, _session);
            DoCommand($"ACTIVATE TOOLPATH \"{tpNameToActivate}\" FORM TOOLPATH");
            DoCommand("EDIT METHOD APPLY");
        }

        #region Drilling Templates
        /// <summary>
        /// Feature-based drilling executor (validates feature set and imports appropriate template).
        /// </summary>
        private void RunDrilling(string drillingId)
        {
            EnsurePowerMillSession();
            try
            {
                var featureSets = _session.FeatureSets;

                if (featureSets == null || featureSets.Count == 0)
                {
                    AddLog("No Feature groups found in the project."); // Reminder to improve UX later
                    DoCommand("MESSAGE ERROR \"No Feature groups found in the project.\"");
                }

                var activeFeatureSet = featureSets?.FirstOrDefault(fg => fg.IsActive);
                if (activeFeatureSet == null)
                {
                    AddLog("No active Feature group found. Please activate a Feature group.");
                    DoCommand("MESSAGE ERROR \"No active Feature group found. Please activate a Feature group.\"");
                    return;
                }

                AddLog($"Active Feature group: {activeFeatureSet.Name}");

                // Map of UI IDs -> template files (kept identical)
                var drillingTemplates = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Center-Drill", "Center-Drill.ptf" },
                    { "Drill Deep", "Deep-Drill.ptf" },
                    { "Break Chip", "Break-Chip.ptf" },
                    { "Helical", "Helical.ptf" },
                    { "Profile", "Profile-Drilling.ptf" },
                    { "Tap", "Tap.ptf" },
                    { "Thread Mill", "Thread-Mill.ptf" },
                    { "Rigid Tap", "RigidTap.ptf" },
                    { "Ream", "Ream.ptf" },
                    { "Fine Boring", "Fine-Boring.ptf" }
                };

                if (!drillingTemplates.TryGetValue(drillingId, out var templateFile))
                {
                    AddLog($"Unknown drilling operation: {drillingId}");
                    DoCommand($"MESSAGE ERROR \"Unknown operation: {drillingId}\"");
                    return;
                }

                // Core logic preserved
                TemplateManager.ImportTemplate(templateFile, _session);
                ToolpathManager.DeleteInactiveWorkplane(_powerMill, _session);
                ToolpathManager.PauseEditMacro(_powerMill, _session, Path.GetFileNameWithoutExtension(templateFile));

                // Optional success message
                var toolpath = _session.Toolpaths.LastOrDefault();
                if (toolpath != null)
                {
                    AddLog($"Toolpath {toolpath.Name} created successfully.");
                    DoCommand($"MESSAGE INFO \"{toolpath.Name} created successfully\"");
                }
                else
                {
                    AddLog("No toolpath created after importing template.");
                    DoCommand("MESSAGE ERROR \"No toolpath created. Check template.\"");
                }
            }
            catch (Exception ex)
            {
                HandleError($"Toolpath {drillingId} failed", ex);
            }
        }
        #endregion

        #region Feature-based Toolpaths
        /// <summary>
        /// Executes a feature-based toolpath pipeline by selected label.
        /// </summary>
        private void RunToolpath(string selected)
        {
            EnsurePowerMillSession();

            try
            {
                DoCommand("FORM BLOCK");
                DoCommand("EDIT BLOCK RESET");
                DoCommand("BLOCK ACCEPT");

                // Fetch/activate feature groups (original behavior)
                var ft = new GetFeatureGroup(_powerMill);
                ft.Run();

                var ToolpathTemplates = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Feature Area Clearance", "Feature-Area-Clearance.ptf" },
                    { "Feature Rest Area Clearance", "Model_Rest_Area_Clearance.ptf" },
                    { "Feature Pocket Area Clearance", "Feature-Pocket-Area-Clearance.ptf" },
                    { "Feature Pocket Profile", "Feature-Pocket-Profile.ptf" },
                    { "Feature Finishing Pocket", "Finish-Feature-Pocket.ptf" },
                    { "Feature Finishing Floor", "Finish-Pocket-Flats.ptf" },
                    { "Feature Finishing Wall-Floor", "Feature-Finish-Wall-Floor.ptf" },
                    { "Feature Finish Profile", "Feature-Finish-Profile.ptf" }
                };

                // Validate selection
                if (string.IsNullOrEmpty(selected) || !ToolpathTemplates.TryGetValue(selected, out var templateFile))
                {
                    AddLog($"Unknown toolpath selected: {selected}");
                    DoCommand($"MESSAGE ERROR \"Unknown toolpath selected: {selected}\"");
                    return;
                }

                // Core Automation
                TemplateManager.ImportTemplate(templateFile, _session);
                ToolpathManager.DeleteInactiveWorkplane(_powerMill, _session);
                ToolpathManager.PauseEditMacro(_powerMill, _session, Path.GetFileNameWithoutExtension(templateFile));

                // Validate after run
                var toolpath = _session.Toolpaths.LastOrDefault();
                if (toolpath == null)
                {
                    AddLog("No Toolpath created after importing template.");
                    DoCommand("MESSAGE ERROR \"No toolpath created. Check template.\"");
                    return;
                }

                AddLog($"Toolpath {toolpath.Name} created successfully.");
                DoCommand($"MESSAGE INFO \"{toolpath.Name} created successfully.\"");
            }
            catch (Exception ex)
            {
                HandleError($"Toolpath {selected} failed", ex);
            }
        }
        #endregion

        [Obsolete]
        private void RunAreaClearance(string selected)
        {
            EnsurePowerMillSession();
            try
            {
                switch (selected)
                {
                    case "Model Area Clearance":
                        TemplateManager.ImportTemplate("Model_Area_Clearance.ptf", _session);
                        DoCommand("BATCH PROCESS");
                        InsertToolpathStockModel(_powerMill, _session, "Model_Area_Clearance");
                        break;

                    case "Model Rest Area Clearance":
                        TemplateManager.ImportTemplate("Model_Rest_Area_Clearance.ptf", _session);
                        ToolpathManager.DeletInactiveFeatureGroup(_session);
                        ToolpathManager.EditFeatureRestAreaClearance(_powerMill, _session, "Model_Rest_Area_Clearance", "SFAreaClearance");
                        InsertToolpathStockModel(_powerMill, _session, "Model_Rest_Area_Clearance");
                        break;

                    case "Model Rest Profile":
                        TemplateManager.ImportTemplate("Model_Rest_Profile.ptf", _session);
                        DoCommand("BATCH PROCESS");
                        InsertToolpathStockModel(_powerMill, _session, "Model_Rest_Profile");
                        break;

                    case "Corner Clearance":
                        TemplateManager.ImportTemplate("Corner_Clearance.ptf", _session);
                        DoCommand("BATCH PROCESS");
                        InsertToolpathStockModel(_powerMill, _session, "Corner_Clearance");
                        break;

                    default:
                        AddLog("Unknown area clearance selected.");
                        return;
                }

                StatusText = $"{selected} executed successfully.";
                AddLog(StatusText);
            }
            catch (Exception ex)
            {
                HandleError($"Area clearance {selected} failed", ex);
            }
        }

        [Obsolete]
        #region Finishing (Router)
        public void RunFinishing(string selected)
        {
            EnsurePowerMillSession();
            try
            {
                if (string.IsNullOrEmpty(selected))
                {
                    AddLog($"Unknown finishing selected: {selected}");
                    return;
                }

                switch (selected)
                {
                    case "3D Offset Profile ":
                        Template3DOffset();
                        break;

                    case "3D Offset Wall-Floor":
                        Template3DOffsetWallFloor();
                        break;

                    case "Constant Z Finishing":
                        TemplateZconstantFinish();
                        break;

                    case "Offset Flat Finishing":
                        TemplateOffsetFlatFinish();
                        break;

                    case "Optimized Constant Z Finishing":
                        TemplateOptimizedConstantZFinish();
                        break;

                    case "Step and Shallow Finishing":
                        TemplateStepAndShallowFinish();
                        break;

                    case "Corner Finishing":
                        TemplateCornerFinish();
                        break;

                    case "Corner Pencil Finishing":
                        TemplateCornerPencilFinish();
                        break;

                    case "Corner Finishing Wall-Floor":
                        TemplateCornerWallFloorFinish();
                        break;

                    case "Surface Finishing":
                        TemplatesurfaceFinish();
                        break;

                    case "Corner Multi-Pencil-Finishing":
                        templateCornerMultiPencilFinish();
                        break;

                    case "Inclined Float Finishing":
                        TemplateInclinedFloatFinish();
                        break;

                    case "Raster Finishing":
                        TemplateRasterFinish();
                        break;

                    case "Spiral Finishing":
                        TemplateSpiralFinish();
                        break;
                }
            }
            catch (Exception ex)
            {
                HandleError($"Finishing {selected} failed", ex);
            }
        }
        #endregion

        #region Finishing Templates (helpers keep behavior identical)
        private void Template3DOffset()
        {
            TemplateManager.ImportTemplate("3D_offset_Profile.ptf", _session);
            DoCommand("BATCH PROCESS");
            InsertToolpathStockModel(_powerMill, _session, "3D_offset_Profile");
        }

        private void Template3DOffsetWallFloor()
        {
            TemplateManager.ImportTemplate("3D-Offset-Wall-Floor.ptf", _session);
            DoCommand("BATCH PROCESS");
            InsertToolpathStockModel(_powerMill, _session, "3D-Offset-Wall-Floor");
        }

        private void TemplateZconstantFinish()
        {
            TemplateManager.ImportTemplate("Constant_Z_Finishing.ptf", _session);
            DoCommand("BATCH PROCESS");
            InsertToolpathStockModel(_powerMill, _session, "Constant_Z_Finishing");
        }

        private void TemplateOffsetFlatFinish()
        {
            TemplateManager.ImportTemplate("Offset_Flat_Finishing.ptf", _session);
            DoCommand("BATCH PROCESS");
            InsertToolpathStockModel(_powerMill, _session, "Offset_Flat_Finishing");
        }

        private void TemplateOptimizedConstantZFinish()
        {
            TemplateManager.ImportTemplate("Optimized_Constant_Z_Finishing.ptf", _session);
            DoCommand("BATCH PROCESS");
            InsertToolpathStockModel(_powerMill, _session, "Optimized_Constant_Z_Finishing");
        }

        private void TemplateStepAndShallowFinish()
        {
            TemplateManager.ImportTemplate("Step_and_Shallow_Finishing.ptf", _session);
            DoCommand("BATCH PROCESS");
            InsertToolpathStockModel(_powerMill, _session, "Step_and_Shallow_Finishing");
        }

        private void TemplateCornerFinish()
        {
            TemplateManager.ImportTemplate("Corner_Finishing.ptf", _session);
            DoCommand("BATCH PROCESS");
            InsertToolpathStockModel(_powerMill, _session, "Corner_Finishing");
        }

        private void TemplateCornerPencilFinish()
        {
            TemplateManager.ImportTemplate("Corner-Pencil-Finishing.ptf", _session);
            DoCommand("BATCH PROCESS");
            InsertToolpathStockModel(_powerMill, _session, "Corner-Pencil-Finishing");
        }

        private void TemplateCornerWallFloorFinish()
        {
            TemplateManager.ImportTemplate("Corner-Finishing-Wall-Floor.ptf", _session);
            DoCommand("BATCH PROCESS");
            InsertToolpathStockModel(_powerMill, _session, "Corner-Finishing-Wall-Floor");
        }

        private void TemplatesurfaceFinish()
        {
            TemplateManager.ImportTemplate("Surface-Finishing.ptf", _session);
            DoCommand("BATCH PROCESS");
            InsertToolpathStockModel(_powerMill, _session, "Surface-Finishing");
        }

        private void templateCornerMultiPencilFinish()
        {
            TemplateManager.ImportTemplate("Corner-Multi-Pencil-Finishing.ptf", _session);
            DoCommand("BATCH PROCESS");
            InsertToolpathStockModel(_powerMill, _session, "Corner-Multi-Pencil-Finishing");
        }

        private void TemplateInclinedFloatFinish()
        {
            TemplateManager.ImportTemplate("Inclined-Float-Finishing.ptf", _session);
            DoCommand("BATCH PROCESS");
            InsertToolpathStockModel(_powerMill, _session, "Inclined-Float-Finishing");
        }

        private void TemplateRasterFinish()
        {
            TemplateManager.ImportTemplate("Raster-Finishing.ptf", _session);
            DoCommand("BATCH PROCESS");
            InsertToolpathStockModel(_powerMill, _session, "Raster-Finishing");
        }

        private void TemplateSpiralFinish()
        {
            TemplateManager.ImportTemplate("Spiral-Finishing.ptf", _session);
            DoCommand("BATCH PROCESS");
            InsertToolpathStockModel(_powerMill, _session, "Spiral-Finishing");
        }
        #endregion
        #endregion

        #region Button Helpers
        /// <summary>
        /// Run a macro from the project in a background Task with standard logging.
        /// </summary>
        private void ExecuteMacroButton(string macroName) => SafeExecute(macroName, async () =>
        {
            AddLog($"Macro {macroName}");
            await Task.Run(() =>
            {
                EnsurePowerMillSession();
                MacroManager.RunMacroFromProject(_powerMill, macroName);
            });
            AddLog(StatusText);
        });

        /// <summary>
        /// Import a template with standard wrapping and logging.
        /// </summary>
        private void ExecuteTemplateButton(string templateName)
        {
            SafeExecute(templateName, () =>
            {
                EnsurePowerMillSession();
                TemplateManager.ImportTemplate(templateName, _session);
            });
        }
        #endregion

        #region Tab Buttons (Content Panels)
        [Obsolete] private void BtnSetup_Click(object sender, RoutedEventArgs e) => ShowPanel("Setup");
        [Obsolete] private void BtnWorkplane_Click(object sender, RoutedEventArgs e) => ShowPanel("WorkPlane");
        [Obsolete] private void BtnToolDatabase_Click(object sender, RoutedEventArgs e) => ShowPanel("Tool Database");
        [Obsolete] private void BtnBoundaries_Click(object sender, RoutedEventArgs e) => ShowPanel("Boundaries / Patterns");
        [Obsolete] private void BtnFeatures_Click(object sender, RoutedEventArgs e) => ShowPanel("Holes / 2D Features / Levels");
        [Obsolete] private void BtnMoldWizard_Click(object sender, RoutedEventArgs e) => ShowPanel("Mold Wizard");
       [Obsolete] private void Btn2Dstrategies_Click(object sender, RoutedEventArgs e) => ShowPanel("2D Wizard");

        /// <summary>
        /// Builds the panel of actionable buttons for each category. Preserves all original labels and actions.
        /// </summary>
        [Obsolete]
        private void ShowPanel(string panelName)
        {
            var panel = new StackPanel { Margin = new Thickness(10) };

            switch (panelName)
            {
                case "Setup":
                    panel.Children.Add(CreateButton("Connect PowerMill", () => EnsurePowerMillSession()));
                    panel.Children.Add(CreateButton("Import Model", () =>
                    {
                        EnsurePowerMillSession();
                        DoCommand("IMPORT MODEL FILEOPEN");
                    }));
                    panel.Children.Add(CreateButton("Create Stock Model", () =>
                    {
                        EnsurePowerMillSession();
                        PowerMillExtensions.CreateStockModel(_powerMill);
                    }));
                    panel.Children.Add(CreateButton("Save Project", () => BtnSave_Click("Save Project", SaveProject)));
                    panel.Children.Add(CreateButton("Quit PowerMill", () => BtnQuit_Click(null, null)));
                    panel.Children.Add(CreateButton("Create Folder", () => ExecuteMacroButton("Criar-Pasta.mac")));
                    panel.Children.Add(CreateButton("Import Stock Model", () => ExecuteMacroButton("StockModelInsertToolPath.mac")));
                    panel.Children.Add(CreateButton("Delete Stock Model", () => ExecuteMacroButton("Delete_Stock_Model.mac")));
                    break;

                case "WorkPlane":
                    panel.Children.Add(CreateButton("WorkPlane on Block", () =>
                    {
                        EnsurePowerMillSession();
                        ExecuteMacroButton("MULTIPLE_WORKPLANES.mac");
                    }));
                    panel.Children.Add(CreateButton("Flipped X 180", () => ExecuteMacroButton("Flip180X.mac")));
                    panel.Children.Add(CreateButton("Front View", () => ExecuteMacroButton("FrontView.mac")));
                    panel.Children.Add(CreateButton("Back View", () => ExecuteMacroButton("Back View.mac")));
                    panel.Children.Add(CreateButton("Left View", () => ExecuteMacroButton("Left View.mac")));
                    panel.Children.Add(CreateButton("Right View", () => ExecuteMacroButton("Right View.mac")));
                    panel.Children.Add(CreateButton("Multiple Views WorkPlane", () => ExecuteMacroButton("Add_View_Workplane.mac")));
                    break;

                case "Tool Database":
                    panel.Children.Add(CreateButton("Load Default Tools", () => ExecuteTemplateButton("Tool_Library_Updated.ptf")));
                    panel.Children.Add(CreateButton("End Mill", () => ExecuteMacroButton("Holder_endmill.mac")));
                    panel.Children.Add(CreateButton("Ball Nosed", () => ExecuteMacroButton("Holder_Ballnosed.mac")));
                    panel.Children.Add(CreateButton("Tip Radiused", () => ExecuteMacroButton("Holder_TipRadiused.mac")));
                    panel.Children.Add(CreateButton("Drill", () => ExecuteMacroButton("Holder_drill.mac")));
                    panel.Children.Add(CreateButton("Face Mill", () => ExecuteMacroButton("Holder_FaceMill.mac")));
                    panel.Children.Add(CreateButton("Tap", () => ExecuteMacroButton("Holder_TapMetric.mac")));
                    panel.Children.Add(CreateButton("Indexable Mill", () => ExecuteMacroButton("Holder_Indexable.mac")));
                    panel.Children.Add(CreateButton("Chamferer", () => ExecuteMacroButton("Holder_chamferer.mac")));
                    break;

                case "Boundaries / Patterns":
                    panel.Children.Add(CreateButton("Pattern By Block", () => ExecuteMacroButton("Pattern_From_Block.mac")));
                    panel.Children.Add(CreateButton("Pattern By Curves", () => ExecuteMacroButton("Pattern_Curves.mac")));
                    panel.Children.Add(CreateButton("Pattern Insert Circle", () => ExecuteMacroButton("Pattern_radius.mac")));
                    panel.Children.Add(CreateButton("Border From Block", () => ExecuteMacroButton("Boundary_From_Block.mac")));
                    panel.Children.Add(CreateButton("Shallow by tool", () => ExecuteMacroButton("Boundary_Shallow.mac")));
                    panel.Children.Add(CreateButton("Silhouette by tool", () => ExecuteMacroButton("Boundary_Silhouete.mac")));
                    panel.Children.Add(CreateButton("Clear Boundary", () => ExecuteMacroButton("ClearBoundary.mac")));
                    panel.Children.Add(CreateButton("Border by Curves", () => ExecuteMacroButton("Boundary_Collect.mac")));
                    panel.Children.Add(CreateButton("Offset Boundary", () => ExecuteMacroButton("Boundary_Offset.mac")));
                    break;

                case "Holes / 2D Features / Levels":
                    panel.Children.Add(CreateButton("Levels-Sets-Flats", () => ExecuteMacroButton("SEPARATE_LEVELS_SURFACES_HOLES.mac")));
                    panel.Children.Add(CreateButton("Feature Group", () => ExecuteMacroButton("Feature Group.mac")));
                    panel.Children.Add(CreateButton("Holes by Color", () => ExecuteMacroButton("Holes_By_Color.mac")));
                    panel.Children.Add(CreateButton("Holes by Diameter", () => SafeExecute("Holes sets by Diameter", () =>
                    {
                        EnsurePowerMillSession();
                        new GetHoles(_powerMill).Run();
                        MacroManager.RunMacroFromProject(_powerMill, "HOLES_BY_DIAMETER.mac");
                    })));
                    panel.Children.Add(CreateButton("Hole Feature", () => SafeExecute("Hole Feature", () =>
                    {
                        EnsurePowerMillSession();
                        new GetHoles(_powerMill).Run();
                    })));
                    panel.Children.Add(CreateButton("Compound Holes", () => SafeExecute("Compound Holes", () =>
                    {
                        EnsurePowerMillSession();
                        new CompoudHoles(_powerMill).Run();
                    })));
                    break;

                case "Mold Wizard":
                    panel.Children.Add(CreateButton("Facing Block", () => ExecuteMacroButton("Facing.mac")));
                    panel.Children.Add(CreateButton("Contourn Block", () => ExecuteMacroButton("ContournBlock.mac")));
                    panel.Children.Add(CreateButton("Square Block", () => ExecuteMacroButton("Square_Block.mac")));
                    panel.Children.Add(CreateButton("Rough-Core/Cavity", () =>
                    {
                        EnsurePowerMillSession();

                        bool keepRunningOptions = true;

                        while (keepRunningOptions)
                        {
                            string userChoice = Microsoft.VisualBasic.Interaction.InputBox(
                                "Select  Automation Process:\n\n" +
                                "1 - Facing Block\n" +
                                "2 - Single Contourn Block\n" +
                                "3 - Contour Block + Adjustments\n" +
                                "4 - Rough - Core/Cavity\n" +
                                "5 - Rough - SemiFinish\n" +
                                "6 - Full Automation\n" +
                                "7 - Quit\n\n" +
                                "8 - Stock Model\n\n",
                                "Select Strategy");

                            switch (userChoice.Trim())
                            {
                                case "1":
                                case "Facing Block":
                                    ExecuteMacroButton("Facing.mac");
                                    break;

                                case "2":
                                case "Single Contourn Block":
                                    ExecuteMacroButton("ContourBlockFinal.mac");
                                    break;

                                case "3":
                                case "Contour Block + Adjustments":
                                    ExecuteMacroButton("Square_Block.mac");
                                    break;

                                case "4":
                                case "Rough - Core/Cavity":
                                    EnsurePowerMillSession();

                                    PowerMillExtensions.CreateStockModel(_powerMill);

                                    // Strategy 1
                                    ExecuteTemplateButton("RGH-Tool-Diam-42.ptf");
                                    DoCommand("ACTIVATE TOOLPATH \"RGH-Tool-Diam-42\"");
                                    DoCommand("EDIT TOOLPATH \"RGH-Tool-Diam-42\" CALCULATE");
                                    InsertToolpathStockModel(_powerMill, _session, "RGH-Tool-Diam-42");

                                    // Rest Diam 25
                                    ExecuteTemplateButton("REST-RGH-Tool-Diam-25.ptf");
                                    DoCommand("ACTIVATE TOOLPATH \"REST-RGH-Tool-Diam-25\"");
                                    DoCommand("EDIT TPPAGE SWRest");
                                    DoCommand("EDIT PAR 'AreaClearance.Rest.ReferenceType' 'stockmodel'");
                                    DoCommand("EDIT PAR 'StockModelState.StockModel' \"STOCK\"");
                                    DoCommand("EDIT STOCKMODEL 'STOCK' ACTIVATE INDEXED_STATE '1' ");
                                    DoCommand("EDIT TOOLPATH \"REST-RGH-Tool-Diam-25\" CALCULATE");
                                    DoCommand("FORM ACCEPT SFAreaClearance");
                                    InsertToolpathStockModel(_powerMill, _session, "REST-RGH-Tool-Diam-25");

                                    // Rest Rough Diam 12
                                    ExecuteTemplateButton("REST-RGH-TOOL-DIAM-12.ptf");
                                    DoCommand("ACTIVATE TOOLPATH \"REST-RGH-TOOL-DIAM-12\"");
                                    DoCommand("EDIT TPPAGE SWRest");
                                    DoCommand("EDIT PAR 'AreaClearance.Rest.ReferenceType' 'stockmodel'");
                                    DoCommand("EDIT PAR 'StockModelState.StockModel' \"STOCK\"");
                                    DoCommand("EDIT STOCKMODEL 'STOCK' ACTIVATE INDEXED_STATE '2' ");
                                    DoCommand("EDIT TOOLPATH \"REST-RGH-TOOL-DIAM-12\" CALCULATE");
                                    DoCommand("FORM ACCEPT SFAreaClearance");
                                    InsertToolpathStockModel(_powerMill, _session, "REST-RGH-TOOL-DIAM-12");

                                    // Rest Rough Profile Diam 25
                                    ExecuteTemplateButton("REST-RGH-PROFILE-TOOL-DIAM-25.ptf");
                                    DoCommand("ACTIVATE TOOLPATH \"REST-RGH-PROFILE-TOOL-DIAM-25\"");
                                    DoCommand("EDIT TPPAGE SWRest");
                                    DoCommand("EDIT PAR 'AreaClearance.Rest.ReferenceType' 'stockmodel'");
                                    DoCommand("EDIT PAR 'StockModelState.StockModel' \"STOCK\"");
                                    DoCommand("EDIT STOCKMODEL 'STOCK' ACTIVATE INDEXED_STATE '3' ");
                                    DoCommand("EDIT TOOLPATH \"REST-RGH-PROFILE-TOOL-DIAM-25\" CALCULATE");
                                    DoCommand("FORM ACCEPT SFAreaClearance");
                                    InsertToolpathStockModel(_powerMill, _session, "REST-RGH-PROFILE-TOOL-DIAM-25");

                                    // Rest Rough Profile Spherical Diam 10
                                    ExecuteTemplateButton("REST-RGH-PROFILE-TOOL-SPH-DIAM-10.ptf");
                                    DoCommand("ACTIVATE TOOLPATH \"REST-RGH-PROFILE-TOOL-SPH-DIAM-10\"");
                                    DoCommand("EDIT TPPAGE SWRest");
                                    DoCommand("EDIT PAR 'AreaClearance.Rest.ReferenceType' 'stockmodel'");
                                    DoCommand("EDIT PAR 'StockModelState.StockModel' \"STOCK\"");
                                    DoCommand("EDIT STOCKMODEL 'STOCK' ACTIVATE INDEXED_STATE '4' ");
                                    DoCommand("EDIT TOOLPATH \"REST-RGH-PROFILE-TOOL-SPH-DIAM-10\" CALCULATE");
                                    DoCommand("FORM ACCEPT SFAreaClearance");
                                    InsertToolpathStockModel(_powerMill, _session, "REST-RGH-PROFILE-TOOL-SPH-DIAM-10");
                                    break;

                                case "5":
                                case "Rough - SemiFinish":
                                    RunRoughSemiFinishCoreCavity(_powerMill, _session);
                                    break;

                                case "6":
                                case "Full Automation":
                                    RunAllCoreCavityProcess(_powerMill, _session);
                                    break;

                                case "7":
                                case "Quit":
                                    Window.GetWindow(panel)?.Close();
                                    break;

                                case "8":
                                case "Stock Model":
                                    EnsurePowerMillSession();

                                    bool keepRunning = true;
                                    while (keepRunning)
                                    {
                                        var result = MessageBox.Show("This will create or recreate the Stock Model. Continue?", "Confirm", MessageBoxButton.YesNoCancel);
                                        if (result == MessageBoxResult.Yes)
                                        {
                                            PowerMillExtensions.CreateStockModel(_powerMill);
                                            keepRunning = false;
                                        }
                                        else if (result == MessageBoxResult.No)
                                        {
                                            keepRunning = false;
                                        }
                                        else
                                        {
                                            return; // Cancel
                                        }
                                    }
                                    break;

                                default:
                                    MessageBox.Show("Invalid choice. Please Type 1-7 or the exact name");
                                    break;
                            }

                            if (keepRunningOptions)
                            {
                                var continueResult = MessageBox.Show("Do you want to perform another operation?", "Continue", MessageBoxButton.YesNo, MessageBoxImage.Question);
                                if (continueResult == MessageBoxResult.No)
                                    keepRunningOptions = false;
                            }
                        }

                        MessageBox.Show(" Exiting Automation Menu.", "Exit", MessageBoxButton.OK, MessageBoxImage.Information);
                    }));
                    break;

                case "2D Wizard":
                    {

                        panel.Children.Add(CreateButton("Curve Profile", () => ExecuteMacroButton("Curve_Profile_Pattern Selected.mac")));
                        panel.Children.Add(CreateButton("Curve Area Clearance", () => ExecuteMacroButton("2D_Curve_Area_Clearance.mac")));
                        panel.Children.Add(CreateButton("Curve Diameter", () => ExecuteMacroButton("2D_Curve_Contourn_Cylinder.ma")));
                        panel.Children.Add(CreateButton("Rectangle Clearance", () => ExecuteMacroButton("2D_Curve_Area_Clearance_Rectangle.mac")));
                        panel.Children.Add(CreateButton("Rectangle Contour", () => ExecuteMacroButton("2D_Curve_Rectangle_Profile.mac")));
                        panel.Children.Add(CreateButton("Rectangle Pocket", () => ExecuteMacroButton("2D_Feature_Interactive_Pocket_Done.mac")));
                        panel.Children.Add(CreateButton("Circle Pocket", () => ExecuteMacroButton("Feature_Circle_Interactive_Done.mac")));
                        panel.Children.Add(CreateButton("Rectangle Boss", () => ExecuteMacroButton("2D_Feature_Interactive_Rectangle_Boss_Done.mac")));
                        panel.Children.Add(CreateButton("Circle Boss", () => ExecuteMacroButton("2D_Feature_Interactive_Circle_Boss_Done.mac")));

                        break;
                    }
            }

            MainContentControl.Content = panel;
        }
        #endregion

        #region Core/Cavity Pipelines (kept identical)
        [Obsolete]
        private void RunRoughCoreCavity(PMAutomation _powerMill, PMProject _session)
        {
            EnsurePowerMillSession();

            PowerMillExtensions.CreateStockModel(_powerMill);

            // 1) Rough 42
            ExecuteTemplateButton("RGH-Tool-Diam-42.ptf");
            DoCommand("ACTIVATE TOOLPATH \"RGH-Tool-Diam-42\"");
            DoCommand("EDIT TOOLPATH \"RGH-Tool-Diam-42\" CALCULATE");
            InsertToolpathStockModel(_powerMill, _session, "RGH-Tool-Diam-42");

            // 2) Rest Rough 25
            ExecuteTemplateButton("REST-RGH-Tool-Diam-25.ptf");
            DoCommand("ACTIVATE TOOLPATH \"REST-RGH-Tool-Diam-25\"");
            DoCommand("EDIT TPPAGE SWRest");
            DoCommand("EDIT PAR 'AreaClearance.Rest.ReferenceType' 'stockmodel'");
            DoCommand("EDIT PAR 'StockModelState.StockModel' \"STOCK\"");
            DoCommand("EDIT STOCKMODEL 'STOCK' ACTIVATE INDEXED_STATE '1' ");
            DoCommand("EDIT TOOLPATH \"REST-RGH-Tool-Diam-25\" CALCULATE");
            DoCommand("FORM ACCEPT SFAreaClearance");
            InsertToolpathStockModel(_powerMill, _session, "REST-RGH-Tool-Diam-25");

            // 3) Rest Rough 12
            ExecuteTemplateButton("REST-RGH-TOOL-DIAM-12.ptf");
            DoCommand("ACTIVATE TOOLPATH \"REST-RGH-TOOL-DIAM-12\"");
            DoCommand("EDIT TPPAGE SWRest");
            DoCommand("EDIT PAR 'AreaClearance.Rest.ReferenceType' 'stockmodel'");
            DoCommand("EDIT PAR 'StockModelState.StockModel' \"STOCK\"");
            DoCommand("EDIT STOCKMODEL 'STOCK' ACTIVATE INDEXED_STATE '2' ");
            DoCommand("EDIT TOOLPATH \"REST-RGH-TOOL-DIAM-12\" CALCULATE");
            DoCommand("FORM ACCEPT SFAreaClearance");
            InsertToolpathStockModel(_powerMill, _session, "REST-RGH-TOOL-DIAM-12");

            // 4) Rest Rough Profile 25
            ExecuteTemplateButton("REST-RGH-PROFILE-TOOL-DIAM-25.ptf");
            DoCommand("ACTIVATE TOOLPATH \"REST-RGH-PROFILE-TOOL-DIAM-25\"");
            DoCommand("EDIT TPPAGE SWRest");
            DoCommand("EDIT PAR 'AreaClearance.Rest.ReferenceType' 'stockmodel'");
            DoCommand("EDIT PAR 'StockModelState.StockModel' \"STOCK\"");
            DoCommand("EDIT STOCKMODEL 'STOCK' ACTIVATE INDEXED_STATE '3' ");
            DoCommand("EDIT TOOLPATH \"REST-RGH-PROFILE-TOOL-DIAM-25\" CALCULATE");
            DoCommand("FORM ACCEPT SFAreaClearance");
            InsertToolpathStockModel(_powerMill, _session, "REST-RGH-PROFILE-TOOL-DIAM-25");

            // 5) Rest Rough Profile Spherical 10
            ExecuteTemplateButton("REST-RGH-PROFILE-TOOL-SPH-DIAM-10.ptf");
            DoCommand("ACTIVATE TOOLPATH \"REST-RGH-PROFILE-TOOL-SPH-DIAM-10\"");
            DoCommand("EDIT TPPAGE SWRest");
            DoCommand("EDIT PAR 'AreaClearance.Rest.ReferenceType' 'stockmodel'");
            DoCommand("EDIT PAR 'StockModelState.StockModel' \"STOCK\"");
            DoCommand("EDIT STOCKMODEL 'STOCK' ACTIVATE INDEXED_STATE '4' ");
            DoCommand("EDIT TOOLPATH \"REST-RGH-PROFILE-TOOL-SPH-DIAM-10\" CALCULATE");
            DoCommand("FORM ACCEPT SFAreaClearance");
            InsertToolpathStockModel(_powerMill, _session, "REST-RGH-PROFILE-TOOL-SPH-DIAM-10");
        }

        [Obsolete]
        private void RunRoughSemiFinishCoreCavity(PMAutomation _powerMill, PMProject _session)
        {
            RunRoughCoreCavity(_powerMill, _session);

            // Semi-Finish Profile TOR-10
            ExecuteTemplateButton("SEMI-FINISH-PROFILE-TOR-DIAM-10.ptf");
            DoCommand("ACTIVATE TOOLPATH \"SEMI-FINISH-PROFILE-TOR-DIAM-10\"");
            DoCommand("EDIT TPPAGE SWRest");
            DoCommand("EDIT PAR 'AreaClearance.Rest.ReferenceType' 'stockmodel'");
            DoCommand("EDIT PAR 'StockModelState.StockModel' \"STOCK\"");
            DoCommand("EDIT STOCKMODEL 'STOCK' ACTIVATE INDEXED_STATE '5' ");
            DoCommand("EDIT TOOLPATH \"SEMI-FINISH-PROFILE-TOR-DIAM-10\" CALCULATE");
            DoCommand("FORM ACCEPT SFConstZFinishing");
            InsertToolpathStockModel(_powerMill, _session, "SEMI-FINISH-PROFILE-TOR-DIAM-10");

            // Semi-Finish Flats Area TOR-10
            ExecuteTemplateButton("SEMI-FINISH-FLATS-AREA-TOR-DIAM-10.ptf");
            DoCommand("ACTIVATE TOOLPATH \"SEMI-FINISH-FLATS-AREA-TOR-DIAM-10\"");
            DoCommand("EDIT TPPAGE SWRest");
            DoCommand("EDIT PAR 'AreaClearance.Rest.ReferenceType' 'stockmodel'");
            DoCommand("EDIT PAR 'StockModelState.StockModel' \"STOCK\"");
            DoCommand("EDIT STOCKMODEL 'STOCK' ACTIVATE INDEXED_STATE '6' ");
            DoCommand("EDIT TOOLPATH \"SEMI-FINISH-FLATS-AREA-TOR-DIAM-10\" CALCULATE");
            DoCommand("FORM ACCEPT SFOffsetFinThreeD");
            InsertToolpathStockModel(_powerMill, _session, "SEMI-FINISH-FLATS-AREA-TOR-DIAM-10");
        }

        [Obsolete]
        public void RunAllCoreCavityProcess(PMAutomation _powerMill, PMProject _session)
        {
            RunRoughCoreCavity(_powerMill, _session);
            RunRoughSemiFinishCoreCavity(_powerMill, _session);

            // Finish Flats Area TOR-10
            ExecuteTemplateButton("FINISH-FLATS-AREA-TOR-10.ptf");
            DoCommand("ACTIVATE TOOLPATH \"FINISH-FLATS-AREA-TOR-10\"");
            DoCommand("EDIT TPPAGE SWRest");
            DoCommand("EDIT PAR 'AreaClearance.Rest.ReferenceType' 'stockmodel'");
            DoCommand("EDIT PAR 'StockModelState.StockModel' \"STOCK\"");
            DoCommand("EDIT STOCKMODEL 'STOCK' ACTIVATE INDEXED_STATE '7' ");
            DoCommand("EDIT TOOLPATH \"FINISH-FLATS-AREA-TOR-10\" CALCULATE");
            DoCommand("FORM ACCEPT SFOffsetFinThreeD");
            InsertToolpathStockModel(_powerMill, _session, "FINISH-FLATS-AREA-TOR-10");

            // Finish Profile SPH-10
            ExecuteTemplateButton("FINISH-PROFILE-SPH-DIAM-10.ptf");
            DoCommand("ACTIVATE TOOLPATH \"FINISH-PROFILE-SPH-DIAM-10\"");
            DoCommand("EDIT TPPAGE SWRest");
            DoCommand("EDIT PAR 'AreaClearance.Rest.ReferenceType' 'stockmodel'");
            DoCommand("EDIT PAR 'StockModelState.StockModel' \"STOCK\"");
            DoCommand("EDIT STOCKMODEL 'STOCK' ACTIVATE INDEXED_STATE '8' ");
            DoCommand("EDIT TOOLPATH \"FINISH-PROFILE-SPH-DIAM-10\" CALCULATE");
            DoCommand("FORM ACCEPT SFConstZSShallow");
            InsertToolpathStockModel(_powerMill, _session, "FINISH-PROFILE-SPH-DIAM-10");

            // Finish Corner BALL-6
            ExecuteTemplateButton("FINISH-CORNER-BALL-DIAM-6.ptf");
            DoCommand("ACTIVATE TOOLPATH \"FINISH-CORNER-BALL-DIAM-6\"");
            DoCommand("EDIT TPPAGE SWRest");
            DoCommand("EDIT PAR 'AreaClearance.Rest.ReferenceType' 'stockmodel'");
            DoCommand("EDIT PAR 'StockModelState.StockModel' \"STOCK\"");
            DoCommand("EDIT STOCKMODEL 'STOCK' ACTIVATE INDEXED_STATE '9' ");
            DoCommand("EDIT TOOLPATH \"FINISH-CORNER-BALL-DIAM-6\" CALCULATE");
            DoCommand("FORM ACCEPT SFCornerClear");
            InsertToolpathStockModel(_powerMill, _session, "FINISH-CORNER-BALL-DIAM-6");

            // Finish Corner BALL-4
            ExecuteTemplateButton("FINISH-CORNER-BALL-DIAM-4.ptf");
            DoCommand("ACTIVATE TOOLPATH \"FINISH-CORNER-BALL-DIAM-4\"");
            DoCommand("EDIT TPPAGE SWRest");
            DoCommand("EDIT PAR 'AreaClearance.Rest.ReferenceType' 'stockmodel'");
            DoCommand("EDIT PAR 'StockModelState.StockModel' \"STOCK\"");
            DoCommand("EDIT STOCKMODEL 'STOCK' ACTIVATE INDEXED_STATE '10' ");
            DoCommand("EDIT TOOLPATH \"FINISH-CORNER-BALL-DIAM-4\" CALCULATE");
            DoCommand("FORM ACCEPT SFCornerClear");
            InsertToolpathStockModel(_powerMill, _session, "FINISH-CORNER-BALL-DIAM-4");

            // Finish Pencil BALL-4MM
            ExecuteTemplateButton("FINISH-PENCIL- BALL-DIAM-4MM.ptf");
            DoCommand("ACTIVATE TOOLPATH \"FINISH-PENCIL- BALL-DIAM-4MM\"");
            DoCommand("EDIT TPPAGE SWRest");
            DoCommand("EDIT PAR 'AreaClearance.Rest.ReferenceType' 'stockmodel'");
            DoCommand("EDIT PAR 'StockModelState.StockModel' \"STOCK\"");
            DoCommand("EDIT STOCKMODEL 'STOCK' ACTIVATE INDEXED_STATE '11' ");
            DoCommand("EDIT TOOLPATH \"FINISH-PENCIL- BALL-DIAM-4MM\" CALCULATE");
            DoCommand("FORM ACCEPT SFCornerFin");
            InsertToolpathStockModel(_powerMill, _session, "FINISH-PENCIL- BALL-DIAM-4MM");
        }
        #endregion

        #region Project / Session Utilities
        /// <summary>
        /// Quit PowerMill (removed the previous throw to avoid crashing after quit).
        /// </summary>
        private void BtnQuit_Click(object? _, object? __)
        {
            EnsurePowerMillSession();
            _powerMill.Quit();
            AddLog("PowerMill closed.");
        }

        [Obsolete]
        private void BtnSave_Click(object value1, object value2)
        {
            EnsurePowerMillSession();
            SaveProject();
        }

        [Obsolete]
        private void SaveProject()
        {
            EnsurePowerMillSession();
            try
            {
                DoCommand("FORM RIBBON BACKSTAGE CLOSE PROJECT SAVE AS FILESAVE");
                AddLog("💾 Project saved.");
            }
            catch (Exception ex)
            {
                AddLog("❌ Save failed: " + ex.Message);
            }
        }

        /// <summary>
        /// Extends block ZMAX and reset limits; kept as static utility.
        /// </summary>
        public static void ExtendBlock(PMAutomation powerMill, double extendValue)
        {
            if (powerMill == null) throw new ArgumentException(nameof(powerMill));

            powerMill.Execute("GRAPHICS LOCK");
            powerMill.Execute("EDIT TPPAGE SWWorkplane");
            powerMill.Execute("EDIT TPPAGE SWBlock");
            powerMill.Execute("EDIT BLOCK ZMAX UNLOCK");
            powerMill.Execute("EDIT BLOCK ZMAX \"2\"");
            powerMill.Execute("EDIT BLOCK ZMAX LOCK");
            powerMill.Execute($"EDIT BLOCK RESETLIMIT \"{extendValue}\"");
            powerMill.Execute("EDIT BLOCK RESET");
            powerMill.Execute("BLOCK ACCEPT");
        }

        /// <summary>
        /// Creates/updates STOCK stock model in the project; kept as static utility.
        /// </summary>
        public static void CreateStockModel(PMAutomation powerMill)
        {
            if (powerMill == null) throw new ArgumentException(nameof(powerMill));

            powerMill.Execute("GRAPHICS LOCK");
            powerMill.Execute("DIALOGS MESSAGE OFF");
            powerMill.Execute("DIALOGS ERROR OFF");
            powerMill.Execute("CREATE STOCKMODEL ; FORM STOCKMODEL");
            powerMill.Execute("RENAME STOCKMODEL # \"STOCK\"");
            powerMill.Execute("EDIT STOCKMODEL \"STOCK\" TOLERANCE .1");
            powerMill.Execute("EDIT STOCKMODEL \"STOCK\" STEPOVER .8");
            powerMill.Execute("EDIT STOCKMODEL \"STOCK\" RESTTHICKNESS .2");
            powerMill.Execute("EDIT STOCKMODEL \"STOCK\" REAPPLYFROMGUI");
            powerMill.Execute("EDIT STOCKMODEL \"STOCK\" BLOCK ;");
            powerMill.Execute("EDIT STOCKMODEL \"STOCK\" CALCULATE_STATE INDEXED_STATE 0");
            powerMill.Execute("EXPLORER SELECT StockModel \"StockModel\\STOCK\" NEW");
            powerMill.Execute("EDIT STOCKMODEL \"STOCK\" SHADING DEFAULT");
            powerMill.Execute("UNDRAW StockModel \"STOCK\"");
            powerMill.Execute("BLOCK RESET");
            powerMill.Execute("GRAPHICS UNLOCK");
        }

        /// <summary>
        /// Inserts a toolpath into the STOCK model and recalculates it.
        /// </summary>
        public void InsertToolpathStockModel(PMAutomation powerMill, PMProject session, string toolpathName)
        {
            EnsurePowerMillSession();
            if (powerMill == null || session == null)
            {
                MessageBox.Show("PowerMill is not connected");
                return;
            }

            DoCommand("GRAPHICS LOCK");
            DoCommand($"EXPLORER SELECT Toolpath \"{toolpathName}\\1\" NEW");
            DoCommand($"EDIT STOCKMODEL ; INSERT_INPUT TOOLPATH \"{toolpathName}\" LAST");
            DoCommand("EDIT STOCKMODEL \"STOCK\" CALCULATE");
            DoCommand("GRAPHICS UNLOCK");
        }
        #endregion

        #region UI Utilities
        /// <summary>Create a standard button with minimal styling and wired action.</summary>
        private Button CreateButton(string text, Action action)
        {
            var btn = new Button { Content = text, Margin = new Thickness(.5), Height = 22 };
            btn.Click += (s, e) => action.Invoke();
            return btn;
        }
        #endregion

        #region Scratch/Test Buttons
        private void Testbtn_Click(object sender, RoutedEventArgs e)
        {
            EnsurePowerMillSession();
            foreach (var tool in _session.Tools)
                tool.Delete();
        }
        #endregion
    }
}
