using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using SidebarDiagnostics.Monitoring;

namespace SidebarDiagnostics
{
    /// <summary>
    /// Interaction logic for FloatingBar.xaml
    /// </summary>
    public partial class FloatingBar : Window, INotifyPropertyChanged
    {
        private DispatcherTimer _updateTimer;
        private MonitorManager _monitorManager;
        private FloatingBarDetails _detailsPopup;
        private bool _isInitialized = false;
        private bool _isPinned = false;
        private DateTime _lastSaveTime = DateTime.MinValue;
        
        // Cached brushes for temperature colors (performance optimization)
        private static readonly SolidColorBrush TempBrushCool = new SolidColorBrush(Color.FromRgb(46, 204, 113));
        private static readonly SolidColorBrush TempBrushWarm = new SolidColorBrush(Color.FromRgb(241, 196, 15));
        private static readonly SolidColorBrush TempBrushHot = new SolidColorBrush(Color.FromRgb(230, 126, 34));
        private static readonly SolidColorBrush TempBrushVeryHot = new SolidColorBrush(Color.FromRgb(231, 76, 60));
        private static readonly SolidColorBrush TempBrushCritical = new SolidColorBrush(Color.FromRgb(192, 57, 43));
        private static readonly SolidColorBrush TempBrushDefault = new SolidColorBrush(Color.FromRgb(255, 149, 0));

        public FloatingBar()
        {
            InitializeComponent();
            DataContext = this;
            Ready = false;
            
            // Load pinned state from settings
            _isPinned = Framework.Settings.Instance.FloatingBarAlwaysOnTop;
            UpdatePinState();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsPinned
        {
            get => _isPinned;
            set
            {
                if (_isPinned != value)
                {
                    _isPinned = value;
                    Framework.Settings.Instance.FloatingBarAlwaysOnTop = value;
                    SavePositionAsync();
                    UpdatePinState();
                    NotifyPropertyChanged(nameof(IsPinned));
                    NotifyPropertyChanged(nameof(PinButtonColor));
                }
            }
        }

        public Brush PinButtonColor => _isPinned ? 
            new SolidColorBrush(Color.FromRgb(52, 152, 219)) : 
            new SolidColorBrush(Color.FromRgb(142, 142, 147));

        private void UpdatePinState()
        {
            Topmost = _isPinned;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Set initial position from settings
                Left = Framework.Settings.Instance.FloatingBarX;
                Top = Framework.Settings.Instance.FloatingBarY;

                // Ensure window is on screen
                EnsureOnScreen();

                // Hide from alt-tab first
                HideFromAltTab();

                // Disable aero peek
                DisableAeroPeek();

                // Initialize monitor manager
                InitializeMonitors();

                // Start update timer
                StartUpdateTimer();

                _isInitialized = true;
                Ready = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FloatingBar Load Error: {ex.Message}");
            }
        }

        private void EnsureOnScreen()
        {
            var screen = System.Windows.Forms.Screen.FromPoint(
                new System.Drawing.Point((int)Left, (int)Top));
            
            if (Left < screen.WorkingArea.Left)
                Left = screen.WorkingArea.Left + 10;
            if (Top < screen.WorkingArea.Top)
                Top = screen.WorkingArea.Top + 10;
            if (Left > screen.WorkingArea.Right - 100)
                Left = screen.WorkingArea.Right - 300;
            if (Top > screen.WorkingArea.Bottom - 50)
                Top = screen.WorkingArea.Bottom - 100;
        }

        private void InitializeMonitors()
        {
            try
            {
                if (_monitorManager != null)
                {
                    _monitorManager.Dispose();
                    _monitorManager = null;
                }

                var config = Framework.Settings.Instance.MonitorConfig;
                if (config != null && config.Length > 0)
                {
                    _monitorManager = new MonitorManager(config);
                    _monitorManager.Update();
                    UpdateMetrics();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"InitializeMonitors Error: {ex.Message}");
            }
        }

        private void StartUpdateTimer()
        {
            if (_updateTimer != null)
            {
                _updateTimer.Stop();
                _updateTimer = null;
            }

            _updateTimer = new DispatcherTimer(DispatcherPriority.Background);
            _updateTimer.Interval = TimeSpan.FromMilliseconds(Framework.Settings.Instance.PollingInterval);
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (!_isInitialized || _monitorManager == null)
                return;

            try
            {
                UpdateMetrics();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateTimer Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns a cached color brush based on temperature value
        /// Green (cool) -> Yellow (warm) -> Orange (hot) -> Red (critical)
        /// </summary>
        private SolidColorBrush GetTemperatureColor(string tempText)
        {
            try
            {
                // Extract numeric value from text like "45°C" or "113°F"
                string numericPart = new string(tempText.Where(c => char.IsDigit(c) || c == '.').ToArray());
                if (double.TryParse(numericPart, out double temp))
                {
                    // Check if it's Fahrenheit (typically > 100 for normal temps)
                    bool isFahrenheit = tempText.Contains("F") || temp > 100;
                    
                    // Convert to Celsius for consistent thresholds
                    double tempC = isFahrenheit ? (temp - 32) * 5 / 9 : temp;
                    
                    // Return cached brushes based on temperature thresholds
                    if (tempC < 45) return TempBrushCool;       // Cool - Green
                    if (tempC < 60) return TempBrushWarm;       // Warm - Yellow
                    if (tempC < 75) return TempBrushHot;        // Hot - Orange
                    if (tempC < 85) return TempBrushVeryHot;    // Very Hot - Red
                    return TempBrushCritical;                    // Critical - Dark Red
                }
            }
            catch { }
            
            return TempBrushDefault;
        }

        private void UpdateMetrics()
        {
            if (_monitorManager == null)
                return;

            try
            {
                _monitorManager.Update();

                // Update CPU - Load and Temperature
                var cpuPanel = _monitorManager.MonitorPanels?.FirstOrDefault(p => 
                    p.Title == Framework.Resources.CPU);
                if (cpuPanel?.Monitors != null)
                {
                    bool foundLoad = false;
                    bool foundTemp = false;
                    
                    foreach (var monitor in cpuPanel.Monitors)
                    {
                        if (monitor?.Metrics == null) continue;
                        
                        // Find CPU Load
                        if (!foundLoad)
                        {
                            var loadMetric = monitor.Metrics.FirstOrDefault(m => m.Key == MetricKey.CPULoad);
                            if (loadMetric != null && loadMetric.Text != null && loadMetric.Text.Contains("%"))
                            {
                                CpuValue.Text = loadMetric.Text;
                                foundLoad = true;
                            }
                        }
                        
                        // Find CPU Temperature
                        if (!foundTemp)
                        {
                            var tempMetric = monitor.Metrics.FirstOrDefault(m => m.Key == MetricKey.CPUTemp);
                            if (tempMetric != null && tempMetric.Text != null)
                            {
                                CpuTemp.Text = tempMetric.Text;
                                CpuTemp.Foreground = GetTemperatureColor(tempMetric.Text);
                                foundTemp = true;
                            }
                        }
                        
                        if (foundLoad && foundTemp) break;
                    }
                }

                // Update RAM - Load only
                var ramPanel = _monitorManager.MonitorPanels?.FirstOrDefault(p => 
                    p.Title == Framework.Resources.RAM);
                if (ramPanel?.Monitors != null)
                {
                    foreach (var monitor in ramPanel.Monitors)
                    {
                        if (monitor?.Metrics == null) continue;
                        
                        var loadMetric = monitor.Metrics.FirstOrDefault(m => m.Key == MetricKey.RAMLoad);
                        
                        if (loadMetric != null && loadMetric.Text != null && loadMetric.Text.Contains("%"))
                        {
                            RamValue.Text = loadMetric.Text;
                            break;
                        }
                    }
                }

                // Update GPU - Load and Temperature
                var gpuPanel = _monitorManager.MonitorPanels?.FirstOrDefault(p => 
                    p.Title == Framework.Resources.GPU);
                if (gpuPanel?.Monitors != null)
                {
                    bool foundLoad = false;
                    bool foundTemp = false;
                    
                    foreach (var monitor in gpuPanel.Monitors)
                    {
                        if (monitor?.Metrics == null) continue;
                        
                        // Find GPU Core Load
                        if (!foundLoad)
                        {
                            var loadMetric = monitor.Metrics.FirstOrDefault(m => m.Key == MetricKey.GPUCoreLoad);
                            if (loadMetric != null && loadMetric.Text != null && loadMetric.Text.Contains("%"))
                            {
                                GpuValue.Text = loadMetric.Text;
                                foundLoad = true;
                            }
                        }
                        
                        // Find GPU Temperature
                        if (!foundTemp)
                        {
                            var tempMetric = monitor.Metrics.FirstOrDefault(m => m.Key == MetricKey.GPUTemp);
                            if (tempMetric != null && tempMetric.Text != null)
                            {
                                GpuTemp.Text = tempMetric.Text;
                                GpuTemp.Foreground = GetTemperatureColor(tempMetric.Text);
                                foundTemp = true;
                            }
                        }
                        
                        if (foundLoad && foundTemp) break;
                    }
                }

                // Update details popup if open
                if (_detailsPopup != null && _detailsPopup.IsVisible)
                {
                    _detailsPopup.UpdateMetrics(_monitorManager);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateMetrics Error: {ex.Message}");
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Don't allow dragging if pinned
            if (_isPinned)
                return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try
                {
                    DragMove();
                }
                catch (InvalidOperationException)
                {
                    // Can happen if button is released during DragMove
                }
                finally
                {
                    // Save position asynchronously after drag completes
                    SavePositionAsync();
                }
            }
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            
            // Update popup position if visible (no saving here - causes lag)
            if (_detailsPopup != null && _detailsPopup.IsVisible)
            {
                PositionDetailsPopup();
            }
        }

        private async void SavePositionAsync()
        {
            if (!_isInitialized) return;
            
            // Debounce: Don't save more than once per 500ms
            var now = DateTime.Now;
            if ((now - _lastSaveTime).TotalMilliseconds < 500)
                return;
            
            _lastSaveTime = now;
            
            // Small delay to ensure drag is fully complete
            await Task.Delay(100);
            
            // Capture values on UI thread
            double left = Left;
            double top = Top;
            
            await Task.Run(() =>
            {
                try
                {
                    Framework.Settings.Instance.FloatingBarX = left;
                    Framework.Settings.Instance.FloatingBarY = top;
                    Framework.Settings.Instance.Save();
                }
                catch { }
            });
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            ControlsPanel.Visibility = Visibility.Visible;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsMouseOver)
            {
                ControlsPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            IsPinned = !IsPinned;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if settings window is already open
            var existingSettings = Application.Current.Windows.OfType<Settings>().FirstOrDefault();
            if (existingSettings != null)
            {
                existingSettings.Activate();
                return;
            }

            // Pause updates
            _updateTimer?.Stop();

            // Open settings directly
            var settings = new Settings(this);
            
            // Resume updates when settings closes
            settings.Closed += (s, args) => _updateTimer?.Start();
        }

        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_detailsPopup == null || !_detailsPopup.IsVisible)
                {
                    ShowDetailsPopup();
                }
                else
                {
                    HideDetailsPopup();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Expand Error: {ex.Message}");
            }
        }

        private void ShowDetailsPopup()
        {
            try
            {
                if (_detailsPopup != null)
                {
                    _detailsPopup.Close();
                }

                _detailsPopup = new FloatingBarDetails();
                _detailsPopup.Closed += (s, args) => _detailsPopup = null;

                PositionDetailsPopup();
                _detailsPopup.Show();
                
                if (_monitorManager != null)
                {
                    _detailsPopup.UpdateMetrics(_monitorManager);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ShowDetails Error: {ex.Message}");
            }
        }

        private void PositionDetailsPopup()
        {
            if (_detailsPopup == null) return;

            try
            {
                _detailsPopup.Left = Left;
                _detailsPopup.Top = Top + ActualHeight + 5;

                // Make sure popup stays on screen
                var screen = System.Windows.Forms.Screen.FromPoint(
                    new System.Drawing.Point((int)Left, (int)Top));
                
                if (_detailsPopup.Left + 280 > screen.WorkingArea.Right)
                {
                    _detailsPopup.Left = screen.WorkingArea.Right - 290;
                }
                if (_detailsPopup.Top + 220 > screen.WorkingArea.Bottom)
                {
                    _detailsPopup.Top = Top - 230;
                }
            }
            catch { }
        }

        private void HideDetailsPopup()
        {
            try
            {
                if (_detailsPopup != null)
                {
                    _detailsPopup.Close();
                    _detailsPopup = null;
                }
            }
            catch { }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _isInitialized = false;
            Ready = false;

            try
            {
                // Stop timer
                if (_updateTimer != null)
                {
                    _updateTimer.Stop();
                    _updateTimer.Tick -= UpdateTimer_Tick;
                    _updateTimer = null;
                }

                // Close details popup
                HideDetailsPopup();

                // Dispose monitor manager
                if (_monitorManager != null)
                {
                    _monitorManager.Dispose();
                    _monitorManager = null;
                }
            }
            catch { }
        }

        private void HideFromAltTab()
        {
            try
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                if (hwnd != IntPtr.Zero)
                {
                    int exStyle = (int)GetWindowLong(hwnd, GWL_EXSTYLE);
                    exStyle |= WS_EX_TOOLWINDOW;
                    SetWindowLong(hwnd, GWL_EXSTYLE, exStyle);
                }
            }
            catch { }
        }

        private void DisableAeroPeek()
        {
            try
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                if (hwnd != IntPtr.Zero)
                {
                    int status = 1;
                    DwmSetWindowAttribute(hwnd, DWMWA_EXCLUDED_FROM_PEEK, ref status, sizeof(int));
                }
            }
            catch { }
        }

        #region Native Methods
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int DWMWA_EXCLUDED_FROM_PEEK = 12;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);
        #endregion

        public bool Ready { get; private set; }

        public MonitorManager MonitorManager => _monitorManager;

        public void Reload()
        {
            try
            {
                InitializeMonitors();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Reload Error: {ex.Message}");
            }
        }
    }
}
