using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using SidebarDiagnostics.Monitoring;

namespace SidebarDiagnostics
{
    /// <summary>
    /// Interaction logic for FloatingBarDetails.xaml
    /// </summary>
    public partial class FloatingBarDetails : Window
    {
        public FloatingBarDetails()
        {
            InitializeComponent();
            Loaded += FloatingBarDetails_Loaded;
            Closing += FloatingBarDetails_Closing;
        }

        private void FloatingBarDetails_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                HideFromAltTab();
            }
            catch { }
        }

        private void FloatingBarDetails_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Cleanup event handlers
            Loaded -= FloatingBarDetails_Loaded;
            Closing -= FloatingBarDetails_Closing;
        }

        private void NavButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.Tag != null)
            {
                string tag = rb.Tag.ToString();
                
                // Hide all panels
                CpuPanel.Visibility = Visibility.Collapsed;
                RamPanel.Visibility = Visibility.Collapsed;
                GpuPanel.Visibility = Visibility.Collapsed;
                NetPanel.Visibility = Visibility.Collapsed;
                DrivesPanel.Visibility = Visibility.Collapsed;
                
                // Show selected panel
                switch (tag)
                {
                    case "CPU":
                        CpuPanel.Visibility = Visibility.Visible;
                        break;
                    case "RAM":
                        RamPanel.Visibility = Visibility.Visible;
                        break;
                    case "GPU":
                        GpuPanel.Visibility = Visibility.Visible;
                        break;
                    case "NET":
                        NetPanel.Visibility = Visibility.Visible;
                        break;
                    case "DRIVES":
                        DrivesPanel.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        public void UpdateMetrics(MonitorManager monitorManager)
        {
            if (monitorManager == null || monitorManager.MonitorPanels == null)
                return;

            try
            {
                UpdateCpuMetrics(monitorManager);
                UpdateRamMetrics(monitorManager);
                UpdateGpuMetrics(monitorManager);
                UpdateNetworkMetrics(monitorManager);
                UpdateDriveMetrics(monitorManager);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateMetrics Error: {ex.Message}");
            }
        }

        private void UpdateCpuMetrics(MonitorManager monitorManager)
        {
            var cpuPanel = monitorManager.MonitorPanels?.FirstOrDefault(p => p.Title == Framework.Resources.CPU);
            if (cpuPanel?.Monitors == null) return;

            foreach (var monitor in cpuPanel.Monitors)
            {
                if (monitor?.Metrics == null) continue;

                // Set CPU name
                if (!string.IsNullOrEmpty(monitor.Name))
                {
                    CpuName.Text = monitor.Name;
                }

                // Load
                var loadMetric = monitor.Metrics.FirstOrDefault(m => m.Key == MetricKey.CPULoad);
                if (loadMetric != null)
                {
                    CpuLoad.Text = loadMetric.Text ?? "--%";
                    CpuLoadBar.Value = ExtractValue(loadMetric.Text);
                }

                // Temperature
                var tempMetric = monitor.Metrics.FirstOrDefault(m => m.Key == MetricKey.CPUTemp);
                if (tempMetric != null)
                {
                    CpuTemp.Text = tempMetric.Text ?? "--°C";
                }

                // Clock
                var clockMetric = monitor.Metrics.FirstOrDefault(m => m.Key == MetricKey.CPUClock);
                if (clockMetric != null)
                {
                    CpuClock.Text = clockMetric.Text ?? "-- MHz";
                }

                // Fan
                var fanMetric = monitor.Metrics.FirstOrDefault(m => m.Key == MetricKey.CPUFan);
                if (fanMetric != null)
                {
                    CpuFan.Text = fanMetric.Text ?? "-- RPM";
                }

                break; // Only use first CPU
            }
        }

        private void UpdateRamMetrics(MonitorManager monitorManager)
        {
            var ramPanel = monitorManager.MonitorPanels?.FirstOrDefault(p => p.Title == Framework.Resources.RAM);
            if (ramPanel?.Monitors == null) return;

            foreach (var monitor in ramPanel.Monitors)
            {
                if (monitor?.Metrics == null) continue;

                // Load
                var loadMetric = monitor.Metrics.FirstOrDefault(m => m.Key == MetricKey.RAMLoad);
                if (loadMetric != null)
                {
                    RamLoad.Text = loadMetric.Text ?? "--%";
                    RamLoadBar.Value = ExtractValue(loadMetric.Text);
                }

                // Used
                var usedMetric = monitor.Metrics.FirstOrDefault(m => m.Key == MetricKey.RAMUsed);
                if (usedMetric != null)
                {
                    RamUsed.Text = usedMetric.Text ?? "-- GB";
                }

                // Free
                var freeMetric = monitor.Metrics.FirstOrDefault(m => m.Key == MetricKey.RAMFree);
                if (freeMetric != null)
                {
                    RamFree.Text = freeMetric.Text ?? "-- GB";
                }

                // Clock
                var clockMetric = monitor.Metrics.FirstOrDefault(m => m.Key == MetricKey.RAMClock);
                if (clockMetric != null)
                {
                    RamClock.Text = clockMetric.Text ?? "-- MHz";
                }

                break; // Only use first RAM module
            }
        }

        private void UpdateGpuMetrics(MonitorManager monitorManager)
        {
            var gpuPanel = monitorManager.MonitorPanels?.FirstOrDefault(p => p.Title == Framework.Resources.GPU);
            if (gpuPanel?.Monitors == null) return;

            foreach (var monitor in gpuPanel.Monitors)
            {
                if (monitor?.Metrics == null) continue;

                // Set GPU name
                if (!string.IsNullOrEmpty(monitor.Name))
                {
                    GpuName.Text = monitor.Name;
                }

                // Core Load
                var loadMetric = monitor.Metrics.FirstOrDefault(m => m.Key == MetricKey.GPUCoreLoad);
                if (loadMetric != null)
                {
                    GpuLoad.Text = loadMetric.Text ?? "--%";
                    GpuLoadBar.Value = ExtractValue(loadMetric.Text);
                }

                // Temperature
                var tempMetric = monitor.Metrics.FirstOrDefault(m => m.Key == MetricKey.GPUTemp);
                if (tempMetric != null)
                {
                    GpuTemp.Text = tempMetric.Text ?? "--°C";
                }

                // Core Clock
                var coreClockMetric = monitor.Metrics.FirstOrDefault(m => m.Key == MetricKey.GPUCoreClock);
                if (coreClockMetric != null)
                {
                    GpuCoreClock.Text = coreClockMetric.Text ?? "-- MHz";
                }

                // VRAM Load
                var vramLoadMetric = monitor.Metrics.FirstOrDefault(m => m.Key == MetricKey.GPUVRAMLoad);
                if (vramLoadMetric != null)
                {
                    GpuVram.Text = vramLoadMetric.Text ?? "--%";
                }

                break; // Only use first GPU
            }
        }

        private void UpdateNetworkMetrics(MonitorManager monitorManager)
        {
            var netPanel = monitorManager.MonitorPanels?.FirstOrDefault(p => p.Title == Framework.Resources.Network);
            if (netPanel?.Monitors == null) return;

            foreach (var monitor in netPanel.Monitors)
            {
                if (monitor?.Metrics == null) continue;

                // Set Network adapter name
                if (!string.IsNullOrEmpty(monitor.Name))
                {
                    NetName.Text = monitor.Name;
                }

                // In
                var inMetric = monitor.Metrics.FirstOrDefault(m => m.Key == MetricKey.NetworkIn);
                if (inMetric != null)
                {
                    NetIn.Text = inMetric.Text ?? "-- KB/s";
                }

                // Out
                var outMetric = monitor.Metrics.FirstOrDefault(m => m.Key == MetricKey.NetworkOut);
                if (outMetric != null)
                {
                    NetOut.Text = outMetric.Text ?? "-- KB/s";
                }

                // IP
                var ipMetric = monitor.Metrics.FirstOrDefault(m => m.Key == MetricKey.NetworkIP);
                if (ipMetric != null)
                {
                    NetIP.Text = ipMetric.Text ?? "--";
                }

                break; // Only use first network adapter
            }
        }

        private void UpdateDriveMetrics(MonitorManager monitorManager)
        {
            var drivePanel = monitorManager.MonitorPanels?.FirstOrDefault(p => p.Title == Framework.Resources.Drives);
            if (drivePanel?.Monitors == null)
            {
                DriveInfo.Text = "No drives found";
                return;
            }

            StringBuilder sb = new StringBuilder();
            int driveCount = 0;

            foreach (var monitor in drivePanel.Monitors)
            {
                if (monitor == null) continue;
                if (driveCount >= 5) break; // Limit to 5 drives

                string driveName = monitor.Name ?? "Drive";
                string driveLoad = "--%";
                string driveFree = "";

                if (monitor.Metrics != null)
                {
                    var loadMetric = monitor.Metrics.FirstOrDefault(m => m.Key == MetricKey.DriveLoad);
                    if (loadMetric != null)
                    {
                        driveLoad = loadMetric.Text ?? "--%";
                    }

                    var freeMetric = monitor.Metrics.FirstOrDefault(m => m.Key == MetricKey.DriveFree);
                    if (freeMetric != null)
                    {
                        driveFree = $" ({freeMetric.Text} free)";
                    }
                }

                if (sb.Length > 0) sb.AppendLine();
                sb.Append($"{driveName}: {driveLoad}{driveFree}");
                driveCount++;
            }

            DriveInfo.Text = sb.Length > 0 ? sb.ToString() : "No drives found";
        }

        private double ExtractValue(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0;

            try
            {
                string numStr = "";
                foreach (char c in text)
                {
                    if (char.IsDigit(c) || c == '.' || c == ',')
                    {
                        numStr += c;
                    }
                }
                
                if (string.IsNullOrEmpty(numStr)) return 0;
                
                numStr = numStr.Replace(',', '.');
                if (double.TryParse(numStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                {
                    return Math.Min(100, Math.Max(0, value));
                }
            }
            catch { }

            return 0;
        }

        private void HideFromAltTab()
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            if (hwnd != IntPtr.Zero)
            {
                int exStyle = (int)GetWindowLong(hwnd, GWL_EXSTYLE);
                exStyle |= WS_EX_TOOLWINDOW;
                SetWindowLong(hwnd, GWL_EXSTYLE, exStyle);
            }
        }

        #region Native Methods
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);
        #endregion
    }
}
