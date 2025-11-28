using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using SidebarDiagnostics.Utilities;
using SidebarDiagnostics.Monitoring;
using SidebarDiagnostics.Windows;
using SidebarDiagnostics.Framework;

namespace SidebarDiagnostics.Models
{
    public class SettingsModel : INotifyPropertyChanged
    {
        public SettingsModel(Sidebar sidebar)
        {
            try
            {
                DockEdgeItems = new DockItem[3]
                {
                    new DockItem() { Text = Resources.SettingsDockLeft, Value = DockEdge.Left },
                    new DockItem() { Text = Resources.SettingsDockRight, Value = DockEdge.Right },
                    new DockItem() { Text = Resources.SettingsDockFloating, Value = DockEdge.Floating }
                };

                DockEdge = Framework.Settings.Instance.DockEdge;

                // Floating Bar Settings
                FloatingBarAlwaysOnTop = Framework.Settings.Instance.FloatingBarAlwaysOnTop;
                FloatingBarOpacity = Framework.Settings.Instance.FloatingBarOpacity;

                Monitor[] _monitors = null;
                try
                {
                    _monitors = Monitor.GetMonitors();
                }
                catch { }
                
                if (_monitors == null || _monitors.Length == 0)
                {
                    // Fallback: create a single screen item
                    ScreenItems = new ScreenItem[] { new ScreenItem() { Index = 0, Text = "#1" } };
                    ScreenIndex = 0;
                }
                else
                {
                    ScreenItems = _monitors.Select((s, i) => new ScreenItem() { Index = i, Text = string.Format("#{0}", i + 1) }).ToArray();

                    if (Framework.Settings.Instance.ScreenIndex < _monitors.Length)
                    {
                        ScreenIndex = Framework.Settings.Instance.ScreenIndex;
                    }
                    else
                    {
                        var primaryIndex = _monitors.Select((s, i) => new { s, i }).FirstOrDefault(x => x.s != null && x.s.IsPrimary);
                        ScreenIndex = primaryIndex != null ? primaryIndex.i : 0;
                    }
                }

                CultureItems = Utilities.Culture.GetAll() ?? new CultureItem[0];
                Culture = Framework.Settings.Instance.Culture ?? "";

            UIScale = Framework.Settings.Instance.UIScale;
            XOffset = Framework.Settings.Instance.XOffset;
            YOffset = Framework.Settings.Instance.YOffset;
            PollingInterval = Framework.Settings.Instance.PollingInterval;
            UseAppBar = Framework.Settings.Instance.UseAppBar;
            AlwaysTop = Framework.Settings.Instance.AlwaysTop;
            ToolbarMode = Framework.Settings.Instance.ToolbarMode;
            ClickThrough = Framework.Settings.Instance.ClickThrough;
            ShowTrayIcon = Framework.Settings.Instance.ShowTrayIcon;
            AutoUpdate = Framework.Settings.Instance.AutoUpdate;
            RunAtStartup = Framework.Settings.Instance.RunAtStartup;
            SidebarWidth = Framework.Settings.Instance.SidebarWidth;
            AutoBGColor = Framework.Settings.Instance.AutoBGColor;
            BGColor = Framework.Settings.Instance.BGColor;
            BGOpacity = Framework.Settings.Instance.BGOpacity;

            TextAlignItems = new TextAlignItem[2]
            {
                new TextAlignItem() { Text = Resources.SettingsTextAlignLeft, Value = TextAlign.Left },
                new TextAlignItem() { Text = Resources.SettingsTextAlignRight, Value = TextAlign.Right }
            };

            TextAlign = Framework.Settings.Instance.TextAlign;

            FontSettingItems = new FontSetting[5]
            {
                FontSetting.x10,
                FontSetting.x12,
                FontSetting.x14,
                FontSetting.x16,
                FontSetting.x18
            };

            FontSetting = Framework.Settings.Instance.FontSetting;
            FontColor = Framework.Settings.Instance.FontColor;
            AlertFontColor = Framework.Settings.Instance.AlertFontColor;
            AlertBlink = Framework.Settings.Instance.AlertBlink;

            DateSettingItems = new DateSetting[4]
            {
                DateSetting.Disabled,
                DateSetting.Short,
                DateSetting.Normal,
                DateSetting.Long
            };

            DateSetting = Framework.Settings.Instance.DateSetting;
            CollapseMenuBar = Framework.Settings.Instance.CollapseMenuBar;
            InitiallyHidden = Framework.Settings.Instance.InitiallyHidden;
            ShowMachineName = Framework.Settings.Instance.ShowMachineName;
            ShowClock = Framework.Settings.Instance.ShowClock;
            Clock24HR = Framework.Settings.Instance.Clock24HR;

            // Safely create config collection with null check
            ObservableCollection<MonitorConfig> _config;
            if (Framework.Settings.Instance.MonitorConfig != null)
            {
                _config = new ObservableCollection<MonitorConfig>(Framework.Settings.Instance.MonitorConfig.Select(c => c.Clone()).OrderByDescending(c => c.Order));
            }
            else
            {
                _config = new ObservableCollection<MonitorConfig>();
            }

            // Only bind hardware if sidebar is available and ready
            if (sidebar != null && sidebar.Ready && sidebar.Model != null && sidebar.Model.MonitorManager != null)
            {
                foreach (MonitorConfig _record in _config)
                {
                    try
                    {
                        _record.HardwareOC = new ObservableCollection<HardwareConfig>(
                            from hw in sidebar.Model.MonitorManager.GetHardware(_record.Type)
                            join config in _record.Hardware on hw.ID equals config.ID into merged
                            from newhw in merged.DefaultIfEmpty(hw).Select(newhw => { newhw.ActualName = hw.ActualName; if (string.IsNullOrEmpty(newhw.Name)) { newhw.Name = hw.ActualName; } return newhw; })
                            orderby newhw.Order descending, newhw.Name ascending
                            select newhw
                            );
                    }
                    catch { }
                }
            }

            MonitorConfig = _config;

            if (Framework.Settings.Instance.Hotkeys != null)
            {
                ToggleKey = Framework.Settings.Instance.Hotkeys.FirstOrDefault(k => k.Action == Hotkey.KeyAction.Toggle);
                ShowKey = Framework.Settings.Instance.Hotkeys.FirstOrDefault(k => k.Action == Hotkey.KeyAction.Show);
                HideKey = Framework.Settings.Instance.Hotkeys.FirstOrDefault(k => k.Action == Hotkey.KeyAction.Hide);
                ReloadKey = Framework.Settings.Instance.Hotkeys.FirstOrDefault(k => k.Action == Hotkey.KeyAction.Reload);
                CloseKey = Framework.Settings.Instance.Hotkeys.FirstOrDefault(k => k.Action == Hotkey.KeyAction.Close);
                CycleEdgeKey = Framework.Settings.Instance.Hotkeys.FirstOrDefault(k => k.Action == Hotkey.KeyAction.CycleEdge);
                CycleScreenKey = Framework.Settings.Instance.Hotkeys.FirstOrDefault(k => k.Action == Hotkey.KeyAction.CycleScreen);
                ReserveSpaceKey = Framework.Settings.Instance.Hotkeys.FirstOrDefault(k => k.Action == Hotkey.KeyAction.ReserveSpace);
            }

            IsChanged = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SettingsModel Constructor Error: {ex.Message}\n{ex.StackTrace}");
                // Don't throw - just log and continue with defaults
                IsChanged = false;
            }
        }

        public void Save()
        {
            if (!string.Equals(Culture, Framework.Settings.Instance.Culture, StringComparison.Ordinal))
            {
                MessageBox.Show(Resources.LanguageChangedText, Resources.LanguageChangedTitle, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }

            Framework.Settings.Instance.DockEdge = DockEdge;
            Framework.Settings.Instance.FloatingBarAlwaysOnTop = FloatingBarAlwaysOnTop;
            Framework.Settings.Instance.FloatingBarOpacity = FloatingBarOpacity;
            Framework.Settings.Instance.ScreenIndex = ScreenIndex;
            Framework.Settings.Instance.Culture = Culture;
            Framework.Settings.Instance.UIScale = UIScale;
            Framework.Settings.Instance.XOffset = XOffset;
            Framework.Settings.Instance.YOffset = YOffset;
            Framework.Settings.Instance.PollingInterval = PollingInterval;
            Framework.Settings.Instance.UseAppBar = UseAppBar;
            Framework.Settings.Instance.AlwaysTop = AlwaysTop;
            Framework.Settings.Instance.ToolbarMode = ToolbarMode;
            Framework.Settings.Instance.ClickThrough = ClickThrough;
            Framework.Settings.Instance.ShowTrayIcon = ShowTrayIcon;
            Framework.Settings.Instance.AutoUpdate = AutoUpdate;
            Framework.Settings.Instance.RunAtStartup = RunAtStartup;
            Framework.Settings.Instance.SidebarWidth = SidebarWidth;
            Framework.Settings.Instance.AutoBGColor = AutoBGColor;
            Framework.Settings.Instance.BGColor = BGColor;
            Framework.Settings.Instance.BGOpacity = BGOpacity;
            Framework.Settings.Instance.TextAlign = TextAlign;
            Framework.Settings.Instance.FontSetting = FontSetting;
            Framework.Settings.Instance.FontColor = FontColor;
            Framework.Settings.Instance.AlertFontColor = AlertFontColor;
            Framework.Settings.Instance.AlertBlink = AlertBlink;
            Framework.Settings.Instance.DateSetting = DateSetting;
            Framework.Settings.Instance.CollapseMenuBar = CollapseMenuBar;
            Framework.Settings.Instance.InitiallyHidden = InitiallyHidden;
            Framework.Settings.Instance.ShowMachineName = ShowMachineName;
            Framework.Settings.Instance.ShowClock = ShowClock;
            Framework.Settings.Instance.Clock24HR = Clock24HR;

            // Handle MonitorConfig safely
            if (MonitorConfig != null && MonitorConfig.Count > 0)
            {
                MonitorConfig[] _config = MonitorConfig.Select(c => c.Clone()).ToArray();

                for (int i = 0; i < _config.Length; i++)
                {
                    if (_config[i].HardwareOC != null)
                    {
                        HardwareConfig[] _hardware = _config[i].HardwareOC.ToArray();

                        for (int v = 0; v < _hardware.Length; v++)
                        {
                            _hardware[v].Order = Convert.ToByte(_hardware.Length - v);

                            if (string.IsNullOrEmpty(_hardware[v].Name) || string.Equals(_hardware[v].Name, _hardware[v].ActualName, StringComparison.Ordinal))
                            {
                                _hardware[v].Name = null;
                            }
                        }

                        _config[i].Hardware = _hardware;
                        _config[i].HardwareOC = null;
                    }

                    _config[i].Order = Convert.ToByte(_config.Length - i);
                }

                Framework.Settings.Instance.MonitorConfig = _config;
            }

            List<Hotkey> _hotkeys = new List<Hotkey>();

            if (ToggleKey != null)
            {
                _hotkeys.Add(ToggleKey);
            }
            
            if (ShowKey != null)
            {
                _hotkeys.Add(ShowKey);
            }

            if (HideKey != null)
            {
                _hotkeys.Add(HideKey);
            }

            if (ReloadKey != null)
            {
                _hotkeys.Add(ReloadKey);
            }

            if (CloseKey != null)
            {
                _hotkeys.Add(CloseKey);
            }

            if (CycleEdgeKey != null)
            {
                _hotkeys.Add(CycleEdgeKey);
            }

            if (CycleScreenKey != null)
            {
                _hotkeys.Add(CycleScreenKey);
            }

            if (ReserveSpaceKey != null)
            {
                _hotkeys.Add(ReserveSpaceKey);
            }

            Framework.Settings.Instance.Hotkeys = _hotkeys.ToArray();

            Framework.Settings.Instance.Save();

            App.RefreshIcon();

            if (RunAtStartup)
            {
                Startup.EnableStartupTask();
            }
            else
            {
                Startup.DisableStartupTask();
            }

            IsChanged = false;
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            if (propertyName != "IsChanged")
            {
                IsChanged = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Child_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsChanged = true;
        }

        private void Child_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsChanged = true;
        }

        private bool _isChanged { get; set; } = false;

        public bool IsChanged
        {
            get
            {
                return _isChanged;
            }
            set
            {
                _isChanged = value;

                NotifyPropertyChanged("IsChanged");
            }
        }

        private DockEdge _dockEdge { get; set; }

        public DockEdge DockEdge
        {
            get
            {
                return _dockEdge;
            }
            set
            {
                _dockEdge = value;

                NotifyPropertyChanged("DockEdge");
                NotifyPropertyChanged("IsFloatingMode");
                NotifyPropertyChanged("IsSidebarMode");
            }
        }

        // Returns true if current dock mode is Floating
        public bool IsFloatingMode
        {
            get { return _dockEdge == DockEdge.Floating; }
        }

        // Returns true if current dock mode is Left or Right (Sidebar)
        public bool IsSidebarMode
        {
            get { return _dockEdge != DockEdge.Floating; }
        }

        private DockItem[] _dockEdgeItems { get; set; }

        public DockItem[] DockEdgeItems
        {
            get
            {
                return _dockEdgeItems;
            }
            set
            {
                _dockEdgeItems = value;

                NotifyPropertyChanged("DockEdgeItems");
            }
        }

        private int _screenIndex { get; set; }

        public int ScreenIndex
        {
            get
            {
                return _screenIndex;
            }
            set
            {
                _screenIndex = value;

                NotifyPropertyChanged("ScreenIndex");
            }
        }

        private ScreenItem[] _screenItems { get; set; }

        public ScreenItem[] ScreenItems
        {
            get
            {
                return _screenItems;
            }
            set
            {
                _screenItems = value;

                NotifyPropertyChanged("ScreenItems");
            }
        }

        private string _culture { get; set; }

        public string Culture
        {
            get
            {
                return _culture;
            }
            set
            {
                _culture = value;

                NotifyPropertyChanged("Culture");
            }
        }

        private CultureItem[] _cultureItems { get; set; }

        public CultureItem[] CultureItems
        {
            get
            {
                return _cultureItems;
            }
            set
            {
                _cultureItems = value;

                NotifyPropertyChanged("CultureItems");
            }
        }

        private double _uiScale { get; set; }

        public double UIScale
        {
            get
            {
                return _uiScale;
            }
            set
            {
                _uiScale = value;

                NotifyPropertyChanged("UIScale");
            }
        }

        private int _xOffset { get; set; }

        public int XOffset
        {
            get
            {
                return _xOffset;
            }
            set
            {
                _xOffset = value;

                NotifyPropertyChanged("XOffset");
            }
        }

        private int _yOffset { get; set; }

        public int YOffset
        {
            get
            {
                return _yOffset;
            }
            set
            {
                _yOffset = value;

                NotifyPropertyChanged("YOffset");
            }
        }

        private int _pollingInterval { get; set; }

        public int PollingInterval
        {
            get
            {
                return _pollingInterval;
            }
            set
            {
                _pollingInterval = value;

                NotifyPropertyChanged("PollingInterval");
            }
        }

        private bool _useAppBar { get; set; }

        public bool UseAppBar
        {
            get
            {
                return _useAppBar;
            }
            set
            {
                _useAppBar = value;

                NotifyPropertyChanged("UseAppBar");
            }
        }

        private bool _alwaysTop { get; set; }

        public bool AlwaysTop
        {
            get
            {
                return _alwaysTop;
            }
            set
            {
                _alwaysTop = value;

                NotifyPropertyChanged("AlwaysTop");
            }
        }

        private bool _toolbarMode { get; set; }
        
        public bool ToolbarMode
        {
            get
            {
                return _toolbarMode;
            }
            set
            {
                _toolbarMode = value;

                NotifyPropertyChanged("ToolbarMode");
            }
        }

        private bool _clickThrough { get; set; }

        public bool ClickThrough
        {
            get
            {
                return _clickThrough;
            }
            set
            {
                _clickThrough = value;

                NotifyPropertyChanged("ClickThrough");
            }
        }

        private bool _showTrayIcon { get; set; }

        public bool ShowTrayIcon
        {
            get
            {
                return _showTrayIcon;
            }
            set
            {
                _showTrayIcon = value;

                NotifyPropertyChanged("ShowTrayIcon");
            }
        }

        private bool _autoUpdate { get; set; }

        public bool AutoUpdate
        {
            get
            {
                return _autoUpdate;
            }
            set
            {
                _autoUpdate = value;

                NotifyPropertyChanged("AutoUpdate");
            }
        }

        private bool _runAtStartup { get; set; }

        public bool RunAtStartup
        {
            get
            {
                return _runAtStartup;
            }
            set
            {
                _runAtStartup = value;

                NotifyPropertyChanged("RunAtStartup");
            }
        }

        private int _sidebarWidth { get; set; }

        public int SidebarWidth
        {
            get
            {
                return _sidebarWidth;
            }
            set
            {
                _sidebarWidth = value;

                NotifyPropertyChanged("SidebarWidth");
            }
        }

        private bool _autoBGColor { get; set; }

        public bool AutoBGColor
        {
            get
            {
                return _autoBGColor;
            }
            set
            {
                _autoBGColor = value;

                NotifyPropertyChanged("AutoBGColor");
            }
        }

        private string _bgColor { get; set; }

        public string BGColor
        {
            get
            {
                return _bgColor;
            }
            set
            {
                _bgColor = value;

                NotifyPropertyChanged("BGColor");
            }
        }

        private double _bgOpacity { get; set; }

        public double BGOpacity
        {
            get
            {
                return _bgOpacity;
            }
            set
            {
                _bgOpacity = value;

                NotifyPropertyChanged("BGOpacity");
            }
        }

        private TextAlign _textAlign { get; set; }

        public TextAlign TextAlign
        {
            get
            {
                return _textAlign;
            }
            set
            {
                _textAlign = value;

                NotifyPropertyChanged("TextAlign");
            }
        }

        private TextAlignItem[] _textAlignItems { get; set; }

        public TextAlignItem[] TextAlignItems
        {
            get
            {
                return _textAlignItems;
            }
            set
            {
                _textAlignItems = value;

                NotifyPropertyChanged("TextAlignItems");
            }
        }

        private FontSetting _fontSetting { get; set; }

        public FontSetting FontSetting
        {
            get
            {
                return _fontSetting;
            }
            set
            {
                _fontSetting = value;

                NotifyPropertyChanged("FontSize");
            }
        }

        private FontSetting[] _fontSettingItems { get;  set;}

        public FontSetting[] FontSettingItems
        {
            get
            {
                return _fontSettingItems;
            }
            set
            {
                _fontSettingItems = value;

                NotifyPropertyChanged("FontSizeItems");
            }
        }

        private string _fontColor { get; set; }

        public string FontColor
        {
            get
            {
                return _fontColor;
            }
            set
            {
                _fontColor = value;

                NotifyPropertyChanged("FontColor");
            }
        }

        private string _alertFontColor { get; set; }

        public string AlertFontColor
        {
            get
            {
                return _alertFontColor;
            }
            set
            {
                _alertFontColor = value;

                NotifyPropertyChanged("AlertFontColor");
            }
        }

        private bool _alertBlink { get; set; } = true;
        
        public bool AlertBlink
        {
            get
            {
                return _alertBlink;
            }
            set
            {
                _alertBlink = value;

                NotifyPropertyChanged("AlertBlink");
            }
        }

        private DateSetting _dateSetting { get; set; }

        public DateSetting DateSetting
        {
            get
            {
                return _dateSetting;
            }
            set
            {
                _dateSetting = value;

                NotifyPropertyChanged("DateSetting");
            }
        }

        private DateSetting[] _dateSettingItems { get; set; }

        public DateSetting[] DateSettingItems
        {
            get
            {
                return _dateSettingItems;
            }
            set
            {
                _dateSettingItems = value;

                NotifyPropertyChanged("DateSettingItems");
            }
        }

        private bool _collapseMenuBar { get; set; }

        public bool CollapseMenuBar
        {
            get
            {
                return _collapseMenuBar;
            }
            set
            {
                _collapseMenuBar = value;

                NotifyPropertyChanged("CollapseMenuBar");
            }
        }

        private bool _initiallyHidden { get; set; }
        
        public bool InitiallyHidden
        {
            get
            {
                return _initiallyHidden;
            }
            set
            {
                _initiallyHidden = value;

                NotifyPropertyChanged("InitiallyHidden");
            }
        }

        private bool _showMachineName { get; set; } = true;

        public bool ShowMachineName
        {
            get
            {
                return _showMachineName;
            }
            set
            {
                _showMachineName = value;

                NotifyPropertyChanged("ShowMachineName");
            }
        }

        private bool _showClock { get; set; }

        public bool ShowClock
        {
            get
            {
                return _showClock;
            }
            set
            {
                _showClock = value;

                NotifyPropertyChanged("ShowClock");
            }
        }

        private bool _clock24HR { get; set; }

        public bool Clock24HR
        {
            get
            {
                return _clock24HR;
            }
            set
            {
                _clock24HR = value;

                NotifyPropertyChanged("Clock24HR");
            }
        }

        private ObservableCollection<MonitorConfig> _monitorConfig { get; set; }

        public ObservableCollection<MonitorConfig> MonitorConfig
        {
            get
            {
                return _monitorConfig;
            }
            set
            {
                _monitorConfig = value;

                _monitorConfig.CollectionChanged += Child_CollectionChanged;

                foreach (MonitorConfig _config in _monitorConfig)
                {
                    _config.PropertyChanged += Child_PropertyChanged;

                    _config.HardwareOC.CollectionChanged += Child_CollectionChanged;

                    foreach (HardwareConfig _hardware in _config.HardwareOC)
                    {
                        _hardware.PropertyChanged += Child_PropertyChanged;
                    }

                    foreach (MetricConfig _metric in _config.Metrics)
                    {
                        _metric.PropertyChanged += Child_PropertyChanged;
                    }

                    foreach (ConfigParam _param in _config.Params)
                    {
                        _param.PropertyChanged += Child_PropertyChanged;
                    }
                }

                NotifyPropertyChanged("MonitorConfig");
            }
        }

        private Hotkey _toggleKey { get; set; }

        public Hotkey ToggleKey
        {
            get
            {
                return _toggleKey;
            }
            set
            {
                _toggleKey = value;

                NotifyPropertyChanged("ToggleKey");
            }
        }

        private Hotkey _showKey { get; set; }

        public Hotkey ShowKey
        {
            get
            {
                return _showKey;
            }
            set
            {
                _showKey = value;

                NotifyPropertyChanged("ShowKey");
            }
        }

        private Hotkey _hideKey { get; set; }

        public Hotkey HideKey
        {
            get
            {
                return _hideKey;
            }
            set
            {
                _hideKey = value;

                NotifyPropertyChanged("HideKey");
            }
        }

        private Hotkey _reloadKey { get; set; }

        public Hotkey ReloadKey
        {
            get
            {
                return _reloadKey;
            }
            set
            {
                _reloadKey = value;

                NotifyPropertyChanged("ReloadKey");
            }
        }

        private Hotkey _closeKey { get; set; }

        public Hotkey CloseKey
        {
            get
            {
                return _closeKey;
            }
            set
            {
                _closeKey = value;

                NotifyPropertyChanged("CloseKey");
            }
        }

        private Hotkey _cycleEdgeKey { get; set; }

        public Hotkey CycleEdgeKey
        {
            get
            {
                return _cycleEdgeKey;
            }
            set
            {
                _cycleEdgeKey = value;

                NotifyPropertyChanged("CycleEdgeKey");
            }
        }

        private Hotkey _cycleScreenKey { get; set; }

        public Hotkey CycleScreenKey
        {
            get
            {
                return _cycleScreenKey;
            }
            set
            {
                _cycleScreenKey = value;

                NotifyPropertyChanged("CycleScreenKey");
            }
        }

        private Hotkey _reserveSpaceKey { get; set; }

        public Hotkey ReserveSpaceKey
        {
            get
            {
                return _reserveSpaceKey;
            }
            set
            {
                _reserveSpaceKey = value;

                NotifyPropertyChanged("ReserveSpaceKey");
            }
        }

        private bool _floatingBarAlwaysOnTop { get; set; }

        public bool FloatingBarAlwaysOnTop
        {
            get
            {
                return _floatingBarAlwaysOnTop;
            }
            set
            {
                _floatingBarAlwaysOnTop = value;

                NotifyPropertyChanged("FloatingBarAlwaysOnTop");
            }
        }

        private double _floatingBarOpacity { get; set; }

        public double FloatingBarOpacity
        {
            get
            {
                return _floatingBarOpacity;
            }
            set
            {
                _floatingBarOpacity = value;

                NotifyPropertyChanged("FloatingBarOpacity");
            }
        }
    }

    public class DockItem
    {
        public DockEdge Value { get; set; }

        public string Text { get; set; }
    }

    public class ScreenItem
    {
        public int Index { get; set; }

        public string Text { get; set; }
    }

    public class TextAlignItem
    {
        public TextAlign Value { get; set; }

        public string Text { get; set; }
    }
}