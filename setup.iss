; ============================================================
;  MediaTekDocuments — Inno Script
;  Placer à la racine du dépôt.
;  Pour build, ouvrir le terminal et faire: iscc setup.iss
;  Ne pas oublier de régénérer la solution en mode release avant de build
; ============================================================

#define MyAppName      "MediaTekDocuments"
#define MyAppVersion   GetEnv("APP_VERSION")
#define MyAppPublisher "Mediatek86"
#define MyAppURL       "https://github.com/patrickbrouhard/mediatekdocuments"
#define MyAppExeName   "MediaTekDocuments.exe"
#define MyAppId        "{75DE903D-6147-4E14-BBE0-FA20CD1F9840}"
; Relative to the .iss file (repo root)
#define MyBuildDir     "MediaTekDocuments\bin\Release"

[Setup]
AppId={{#MyAppId}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
SetupIconFile=md86.ico

; Default install dans Program Files (respects 32/64-bit)
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}

OutputDir=installer
OutputBaseFilename=MediaTekDocuments-Setup-{#MyAppVersion}
Compression=lzma2/ultra64
SolidCompression=yes

; Require admin rights (needed to write to Program Files and Event Log)
PrivilegesRequired=admin

; Windows only, AnyCPU build
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "french"; MessagesFile: "compiler:Languages\French.isl"

[Tasks]
; Optional desktop shortcut
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; \
    GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; --- All Release output (exe + all dependency DLLs + .config + .pdb) ---
Source: "{#MyBuildDir}\*.*";    DestDir: "{app}"; \
    Flags: ignoreversion recursesubdirs createallsubdirs
Source: "md86.ico"; DestDir: "{app}"

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\md86.ico"
Name: "{group}\Désinstaller {#MyAppName}"; Filename: "{uninstallexe}"

Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\md86.ico"; Tasks: desktopicon

[Run]
; Launch app after install (user can uncheck)
Filename: "{app}\{#MyAppExeName}"; \
    Description: "Lancer {#MyAppName}"; \
    Flags: nowait postinstall skipifsilent

[UninstallDelete]
; Remove any log files written by Serilog.Sinks.File at runtime
Type: filesandordirs; Name: "{app}\logs"