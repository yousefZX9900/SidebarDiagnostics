<h1><img src="sidebar.ico" width="64" height="64" /> Sidebar Diagnostics</h1>

A modern and elegant sidebar for Windows desktop that displays real-time hardware diagnostic information.

## Version 4.0.0

This is an updated and enhanced fork with full Windows 11 support and new features.

### Download

Go to the <a href="https://github.com/yousefZX9900/SidebarDiagnostics/releases">releases tab</a>.

---

## What's New in v4.0.0

### Floating Bar Mode
A compact, draggable floating bar that displays key metrics anywhere on your screen:
- Semi-transparent modern design with gradient styling
- Hover to reveal additional controls (Pin, Settings, Close)
- Pin/Unpin functionality to lock position
- Expandable details panel with navigation tabs for CPU, RAM, GPU, Network, and Drives
- Real-time temperature readings for CPU and GPU
- Smooth animations and visual effects

### Windows 11 Full Support
- Fully compatible with Windows 11 taskbar and snap layouts
- Optimized for the latest Windows 11 design language
- Works seamlessly with virtual desktops

### Arabic Language Support
- Full RTL (Right-to-Left) support for Arabic speakers
- Complete Arabic localization

### Updated Hardware Monitoring
- Latest Libre Hardware Monitor library (v0.9.4)
- Better compatibility with newer hardware
- Improved GPU metrics accuracy

---

## Core Features

* Monitors CPU, RAM, GPU, network, and logical drives
* Create graphs for all metrics
* Extensive customization options
* Configurable alerts for various values
* Custom hotkey bindings
* Supports monitors of all DPI types including high-DPI displays
* Clock display at the top
* Multi-language support (English, Arabic, French, German, Japanese, Russian, Italian, Chinese, and more)

## Important

If you are changing your screen's DPI settings, <a href="https://github.com/ArcadeRenegade/SidebarDiagnostics/wiki/DPI-Settings">view this page</a>.

## Supported Operating Systems

| OS | Status |
|---|---|
| Windows 11 | Fully Supported |
| Windows 10 | Supported |
| Windows 8.1 | Supported |
| Windows 8 | Supported |
| Windows 7 | Supported |

## Screenshots

### Classic Sidebar Mode
<img src="http://i.imgur.com/70LkdwO.png" />

### Floating Bar Mode
The floating bar provides a compact, always-visible overview of your system performance with modern styling and smooth animations.

<img src="http://i.imgur.com/mkrO6W6.png" />

---

## Technical Information

- Written in C# .NET WPF
- Compiled with <a href="https://dotnet.microsoft.com/download/dotnet-framework/thank-you/net472-web-installer">.NET Framework 4.7.2</a>
- Requires administrator privileges for hardware monitoring access
- Hardware data provided by <a href="https://github.com/LibreHardwareMonitor/LibreHardwareMonitor">Libre Hardware Monitor</a>

## License

GNU GENERAL PUBLIC LICENSE v3.0

Please provide a link to this GitHub repository if reuploading. Thank you.

## Credits

- Original project by <a href="https://github.com/ArcadeRenegade/SidebarDiagnostics">ArcadeRenegade</a>
- Hardware monitoring by <a href="https://github.com/LibreHardwareMonitor/LibreHardwareMonitor">Libre Hardware Monitor</a>

---

## Changelog

### v4.0.0 (Latest)
- New Floating Bar Mode with modern UI
- Full Windows 11 support
- Arabic language support (RTL)
- Updated Libre Hardware Monitor to v0.9.4
- Enhanced UI with gradient styling and animations
- Various bug fixes and improvements

### v3.6.3
- Updated Libre Hardware Monitor
- Arabic language support

### v3.6.2
- Fix GPU fan metric bug
- Fix GPU VRAM load metric bug

<details>
<summary>View older versions</summary>

### v3.6.1
- Updated Libre Hardware Monitor
- Fix GPU fan metric bug

### v3.6.0
- Updated Libre Hardware Monitor

### v3.5.x
- Various Libre Hardware Monitor updates
- Russian and Italian language support
- Bug fixes for Ryzen CPUs

</details>
