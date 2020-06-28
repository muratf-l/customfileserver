
[Setup]
AppName=Reflect Web File Server
AppVersion=1.0.33
VersionInfoVersion=1.0.33
AppCopyright=Copyright (C) 2020 Reflect Inc.
DefaultDirName={sd}\Reflect\Reflect Web Server
DefaultGroupName=Reflect
UninstallDisplayIcon={app}\Reflect.WebServer.exe
Compression=lzma2
SolidCompression=yes
OutputDir="..\build"
OutputBaseFilename=setup_webfileserver
AppPublisher=Reflect Inc.
AppPublisherURL=https://www.reflecth.ca/
AppContact=info@reflecth.ca
AlwaysShowComponentsList=no
DirExistsWarning=no
DisableDirPage=yes
DisableProgramGroupPage=yes
PrivilegesRequired=admin
SetupIconFile=..\global-connection.ico


[Files]
Source: "..\dev\Reflect.WebServer\bin\Release\Microsoft.WindowsAPICodePack.dll"; DestDir: "{app}"
Source: "..\dev\Reflect.WebServer\bin\Release\Microsoft.WindowsAPICodePack.ExtendedLinguisticServices.dll"; DestDir: "{app}"
Source: "..\dev\Reflect.WebServer\bin\Release\Microsoft.WindowsAPICodePack.Sensors.dll"; DestDir: "{app}"
Source: "..\dev\Reflect.WebServer\bin\Release\Microsoft.WindowsAPICodePack.Shell.dll"; DestDir: "{app}"
Source: "..\dev\Reflect.WebServer\bin\Release\Microsoft.WindowsAPICodePack.ShellExtensions.dll"; DestDir: "{app}"
Source: "..\dev\Reflect.WebServer\bin\Release\NamedPipeWrapper.dll"; DestDir: "{app}"
Source: "..\dev\Reflect.WebServer\bin\Release\Newtonsoft.Json.dll"; DestDir: "{app}"

Source: "..\dev\Reflect.WebServer\bin\Release\Reflect.WebServer.Data.dll"; DestDir: "{app}"
Source: "..\dev\Reflect.WebServer\bin\Release\Reflect.WebServer.exe"; DestDir: "{app}"
Source: "..\dev\Reflect.WebServer\bin\Release\Reflect.WebServer.exe.config"; DestDir: "{app}"
Source: "..\dev\Reflect.WebServer\bin\Release\Reflect.WebServer.Service.exe"; DestDir: "{app}"
Source: "..\dev\Reflect.WebServer\bin\Release\Reflect.WebServer.Service.exe.config"; DestDir: "{app}"

Source: "..\dev\Reflect.WebServer\bin\Release\Content\info.txt"; DestDir: "{app}\Content"
Source: "Scripts\service_start.bat"; DestDir: "{app}\Scripts"


[Icons]
Name: "{group}\Reflect Web Server"; Filename: "{app}\Reflect.WebServer.exe"
Name: "{userdesktop}\Reflect Web Server"; Filename: "{app}\Reflect.WebServer.exe"


[Run]
Filename: {sys}\sc.exe; Parameters: "create Reflect.WebServer.Service start= auto binPath= ""{app}\Reflect.WebServer.Service.exe""" ; Flags: runhidden
Filename: "{app}\Scripts\service_start.bat"; Parameters: "install"; Flags: runhidden


[UninstallRun]
Filename: {sys}\sc.exe; Parameters: "stop Reflect.WebServer.Service" ; Flags: runhidden
Filename: {sys}\sc.exe; Parameters: "delete Reflect.WebServer.Service" ; Flags: runhidden
