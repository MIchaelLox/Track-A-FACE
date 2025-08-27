!define APPNAME "FACE engine"
!define APPEXE "FACE.exe"

OutFile "${APPNAME}_Setup.exe"
InstallDir "$PROGRAMFILES\${APPNAME}"

Section "Install Files"
  SetOutPath "$INSTDIR"
  File "build\${APPEXE}"

  ; .NET Framework
  IfFileExists "$WINDIR\Microsoft.NET\Framework\v4.0.30319\mscoreei.dll" 0 DotNetInstallSkip
    File "dependencies\ndp472-x86-x64-allos-enu.exe"
    ExecWait '"$INSTDIR\ndp472-x86-x64-allos-enu.exe" /q /norestart'
  DotNetInstallSkip:

  ; VC++ Redistributable
  IfFileExists "$INSTDIR\vc_redist.x64.exe" 0 VCRedistInstallSkip
    File "dependencies\vc_redist.x64.exe"
    ExecWait '"$INSTDIR\vc_redist.x64.exe" /quiet /norestart'
  VCRedistInstallSkip:
SectionEnd
