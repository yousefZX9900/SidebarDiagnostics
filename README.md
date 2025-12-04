# Sidebar Diagnostics

A modern and elegant hardware monitoring sidebar for Windows desktop that displays real-time system diagnostic information.

## Version 4.0.0

### System Requirements

| Operating System | Status |
|------------------|--------|
| Windows 11 |  Supported |
| Windows 10 |  Supported |

**Runtime Requirements:**
- .NET Framework 4.7.2 or later
- Administrator privileges (required for hardware monitoring)

---

## Download

Visit the [Releases](https://github.com/yousefZX9900/SidebarDiagnostics/releases) page to download the latest version.

**Available Downloads:**
- `SidebarDiagnostics-Setup-4.0.0.exe` - Installer (Recommended)
- `SidebarDiagnostics-v4.0.0.zip` - Portable Version

---

## What's New in Version 4.0.0

### New Features

**Floating Bar Mode**
- Compact, draggable floating bar displaying key system metrics
- Semi-transparent modern design with gradient styling
- Hover-to-reveal controls: Pin, Settings, Close
- Pin/Unpin functionality to lock position on screen
- Expandable details panel with navigation tabs for CPU, RAM, GPU, Network, and Drives
- Real-time temperature readings for CPU and GPU
- Smooth animations and visual effects

**Windows 11 Full Support**
- Complete compatibility with Windows 11 taskbar and snap layouts
- Optimized for Windows 11 design language
- Seamless integration with virtual desktops

**Arabic Language Support**
- Complete Arabic localization

**Updated Hardware Monitoring**
- Upgraded to Libre Hardware Monitor library v0.9.4
- Enhanced compatibility with newer hardware
- Improved GPU metrics accuracy

### Improvements
- Enhanced UI with gradient styling and animations
- Better performance and resource management
- Fixed version display issue
- Various bug fixes and stability improvements

---

## Core Features

- Real-time monitoring of CPU, RAM, GPU, Network, and Logical Drives
- Customizable graphs for all metrics
- Extensive customization options for appearance and behavior
- Configurable alerts with threshold notifications
- Custom hotkey bindings for quick access
- High-DPI display support
- Clock display option
- Multi-language support

### Supported Languages
English, Arabic, French, German, Japanese, Russian, Italian, Chinese, Spanish, Dutch, Danish, Finnish, Turkish

---

## Screenshots

### Classic Sidebar Mode
The traditional sidebar view provides comprehensive system information in a vertical layout.

### Floating Bar Mode
A compact, always-visible overview of system performance with modern styling.

---

## Installation

### Using the Installer (Recommended)
1. Download `SidebarDiagnostics-Setup-4.0.0.exe` from the Releases page
2. Run the installer as Administrator
3. Follow the installation wizard
4. Launch Sidebar Diagnostics from the Start Menu

### Portable Installation
1. Download `SidebarDiagnostics-v4.0.0.zip` from the Releases page
2. Extract to your preferred location
3. Run `SidebarDiagnostics.exe` as Administrator

---

## Building from Source

### Prerequisites
- Visual Studio 2019 or later
- .NET Framework 4.7.2 SDK
- NuGet Package Manager

### Build Steps
1. Clone the repository
2. Open `SidebarDiagnostics.sln` in Visual Studio
3. Restore NuGet packages
4. Build the solution in Release configuration

---

## Technical Information

- Written in C# using WPF (Windows Presentation Foundation)
- Target Framework: .NET Framework 4.7.2
- Hardware monitoring powered by [Libre Hardware Monitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor)

---

## Version History

### v4.0.0 (Current Release)
- New Floating Bar Mode with modern UI
- Full Windows 11 support
- Arabic language support
- Updated Libre Hardware Monitor to v0.9.4
- Fixed version display issue
- Enhanced UI with gradient styling and animations
- Various bug fixes and improvements

### v3.6.3
- Updated Libre Hardware Monitor
- Arabic language support

### v3.6.2
- Fixed GPU fan metric bug
- Fixed GPU VRAM load metric bug

### v3.6.1
- Updated Libre Hardware Monitor
- Fixed GPU fan metric bug

### v3.6.0
- Updated Libre Hardware Monitor

---

## License

GNU General Public License v3.0

See [LICENSE.md](LICENSE.md) for details.

---

## Credits

- Original project by [ArcadeRenegade](https://github.com/ArcadeRenegade/SidebarDiagnostics)
- Project Revived & Enhanced by [Yousef](https://x.com/yousef_dev921?s=21) "Note: AI tools were used to optimize parts of the code"
- Hardware monitoring by [Libre Hardware Monitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor)

---

## Support

For bug reports and feature requests, please use the [Issues](https://github.com/yousefZX9900/SidebarDiagnostics/issues) page.

## SECURITY WARNING — v4.0.0

Windows Defender may display a Severe alert for this tool because one of its current hardware-monitoring dependencies uses a known vulnerable kernel driver: WinRing0.sys (often detected as VulnerableDriver:WinNT/Winring0).

This warning relates to the third-party driver/library used to read low-level hardware sensors, not to any intentionally malicious behavior in this tool. However, due to the known vulnerability, using this driver may increase security risk in certain scenarios.

This issue affects version 4.0.0. A fix is in progress and will be released soon to remove/replace the vulnerable dependency.

Use v4.0.0 at your own risk.
