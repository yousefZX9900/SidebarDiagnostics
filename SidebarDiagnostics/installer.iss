; Inno Setup Script for SidebarDiagnostics
; Professional Installer with Floating Bar Feature

#define MyAppName "Sidebar Diagnostics"
#define MyAppVersion "4.0.0"
#define MyAppPublisher "ArcadeRenegade"
#define MyAppURL "https://github.com/ArcadeRenegade/SidebarDiagnostics"
#define MyAppExeName "SidebarDiagnostics.exe"

[Setup]
; Application Info
AppId={{8A5B2C4D-9E3F-4A1B-8C7D-6E5F4A3B2C1D}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}

; Installation Directories
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes

; Output Settings
OutputDir=..\Installer
OutputBaseFilename=SidebarDiagnostics-Setup-{#MyAppVersion}

; Compression (LZMA2 for best compression)
Compression=lzma2/ultra64
SolidCompression=yes
LZMAUseSeparateProcess=yes

; Visual Settings
WizardStyle=modern
WizardSizePercent=100

; Privileges
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=dialog

; Misc
UninstallDisplayIcon={app}\{#MyAppExeName}
UninstallDisplayName={#MyAppName}
VersionInfoVersion={#MyAppVersion}
VersionInfoCompany={#MyAppPublisher}
VersionInfoProductName={#MyAppName}
VersionInfoProductVersion={#MyAppVersion}

; Languages
[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "arabic"; MessagesFile: "compiler:Languages\Arabic.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"
Name: "turkish"; MessagesFile: "compiler:Languages\Turkish.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "startupicon"; Description: "Start with Windows"; GroupDescription: "Startup Options:"

[Files]
; Main Application
Source: "bin\Release\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\{#MyAppExeName}.config"; DestDir: "{app}"; Flags: ignoreversion

; DLL Files
Source: "bin\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\*.xml"; DestDir: "{app}"; Flags: ignoreversion

; Language Resources
Source: "bin\Release\ar\*"; DestDir: "{app}\ar"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "bin\Release\da\*"; DestDir: "{app}\da"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "bin\Release\de\*"; DestDir: "{app}\de"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "bin\Release\de-CH\*"; DestDir: "{app}\de-CH"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "bin\Release\es\*"; DestDir: "{app}\es"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "bin\Release\fi\*"; DestDir: "{app}\fi"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "bin\Release\fr\*"; DestDir: "{app}\fr"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "bin\Release\it\*"; DestDir: "{app}\it"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "bin\Release\ja\*"; DestDir: "{app}\ja"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "bin\Release\nl\*"; DestDir: "{app}\nl"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "bin\Release\ru\*"; DestDir: "{app}\ru"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "bin\Release\tr\*"; DestDir: "{app}\tr"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "bin\Release\zh\*"; DestDir: "{app}\zh"; Flags: ignoreversion recursesubdirs createallsubdirs

; Additional Language Folders
Source: "bin\Release\cs-CZ\*"; DestDir: "{app}\cs-CZ"; Flags: ignoreversion recursesubdirs createallsubdirs skipifsourcedoesntexist
Source: "bin\Release\hu\*"; DestDir: "{app}\hu"; Flags: ignoreversion recursesubdirs createallsubdirs skipifsourcedoesntexist
Source: "bin\Release\ja-JP\*"; DestDir: "{app}\ja-JP"; Flags: ignoreversion recursesubdirs createallsubdirs skipifsourcedoesntexist
Source: "bin\Release\nl-BE\*"; DestDir: "{app}\nl-BE"; Flags: ignoreversion recursesubdirs createallsubdirs skipifsourcedoesntexist
Source: "bin\Release\pl\*"; DestDir: "{app}\pl"; Flags: ignoreversion recursesubdirs createallsubdirs skipifsourcedoesntexist
Source: "bin\Release\pt-BR\*"; DestDir: "{app}\pt-BR"; Flags: ignoreversion recursesubdirs createallsubdirs skipifsourcedoesntexist
Source: "bin\Release\ro\*"; DestDir: "{app}\ro"; Flags: ignoreversion recursesubdirs createallsubdirs skipifsourcedoesntexist
Source: "bin\Release\sv\*"; DestDir: "{app}\sv"; Flags: ignoreversion recursesubdirs createallsubdirs skipifsourcedoesntexist
Source: "bin\Release\zh-CN\*"; DestDir: "{app}\zh-CN"; Flags: ignoreversion recursesubdirs createallsubdirs skipifsourcedoesntexist
Source: "bin\Release\zh-Hans\*"; DestDir: "{app}\zh-Hans"; Flags: ignoreversion recursesubdirs createallsubdirs skipifsourcedoesntexist
Source: "bin\Release\zh-Hant\*"; DestDir: "{app}\zh-Hant"; Flags: ignoreversion recursesubdirs createallsubdirs skipifsourcedoesntexist

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\Uninstall {#MyAppName}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Registry]
; Add to Windows startup if selected
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "SidebarDiagnostics"; ValueData: """{app}\{#MyAppExeName}"""; Flags: uninsdeletevalue; Tasks: startupicon

[Run]
; Use shellexec to properly handle UAC elevation request
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent shellexec

[UninstallRun]
Filename: "taskkill"; Parameters: "/F /IM {#MyAppExeName}"; Flags: runhidden

[Code]
// Check if .NET Framework 4.7.2 or later is installed
function IsDotNetInstalled(): Boolean;
var
  Release: Cardinal;
begin
  Result := False;
  if RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', Release) then
  begin
    // 461808 = .NET Framework 4.7.2
    Result := (Release >= 461808);
  end;
end;

function InitializeSetup(): Boolean;
begin
  Result := True;
  
  if not IsDotNetInstalled() then
  begin
    MsgBox('This application requires .NET Framework 4.7.2 or later.' + #13#10 + #13#10 +
           'Please install .NET Framework 4.7.2 or later and run this setup again.', 
           mbError, MB_OK);
    Result := False;
  end;
end;

// Kill running instance before install
procedure CurStepChanged(CurStep: TSetupStep);
var
  ResultCode: Integer;
begin
  if CurStep = ssInstall then
  begin
    Exec('taskkill', '/F /IM SidebarDiagnostics.exe', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  end;
end;

