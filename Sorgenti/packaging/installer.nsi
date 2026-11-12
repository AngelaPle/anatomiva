Unicode true
!include "MUI2.nsh"
!include "LogicLib.nsh"
!include "WinVer.nsh"
!include "x64.nsh"
Name "Anatomiva"
OutFile "../build/Installa_Anatomiva.exe"
InstallDir "$LOCALAPPDATA\Programs\Anatomiva"
InstallDirRegKey HKCU "Software\AngelaPlescia\Anatomiva" "InstallDir"
RequestExecutionLevel user
BrandingText "Angela Plescia · Anatomiva 2.0"
!define MUI_ICON "app.ico"
!define MUI_UNICON "app.ico"
!define MUI_ABORTWARNING
!define MUI_WELCOMEPAGE_TITLE "Benvenuta in Anatomiva"
!define MUI_WELCOMEPAGE_TEXT "Esplora anatomia e movimento con esperimenti, missioni e quiz bilingui.$\r$\n$\r$\nL'app funziona offline e salva i progressi sul tuo PC. Non occorre installare Python.$\r$\n$\r$\nAutrice del prototipo: Angela Plescia."
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!define MUI_FINISHPAGE_RUN "$INSTDIR\Anatomiva.exe"
!define MUI_FINISHPAGE_RUN_TEXT "Avvia Anatomiva"
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_LANGUAGE "Italian"
!insertmacro MUI_LANGUAGE "English"
Function .onInit
 ${IfNot} ${AtLeastWin10}
  MessageBox MB_OK|MB_ICONSTOP "Questa versione richiede Windows 10 o successivo."
  Abort
 ${EndIf}
 ${IfNot} ${RunningX64}
  MessageBox MB_OK|MB_ICONSTOP "Questa distribuzione è destinata a Windows a 64 bit."
  Abort
 ${EndIf}
 SetRegView 64
 ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" "Install"
 ${If} $0 != 1
  MessageBox MB_OK|MB_ICONSTOP "Il componente Windows .NET Framework 4 non risulta disponibile. Ripristinalo tramite le funzionalità di Windows o il supporto Microsoft. Nessuna installazione è stata eseguita."
  Abort
 ${EndIf}
FunctionEnd
Section "Anatomiva" Main
 SetShellVarContext current
 SetOutPath "$INSTDIR"
 File "../build/Anatomiva.exe"
 File "Anatomiva.exe.config"
 File "../docs/GUIDA_RAPIDA.txt"
 File "../docs/FONTI.md"
 WriteUninstaller "$INSTDIR\Disinstalla.exe"
 CreateDirectory "$SMPROGRAMS\Anatomiva"
 CreateShortcut "$SMPROGRAMS\Anatomiva\Anatomiva.lnk" "$INSTDIR\Anatomiva.exe"
 CreateShortcut "$DESKTOP\Anatomiva.lnk" "$INSTDIR\Anatomiva.exe"
 WriteRegStr HKCU "Software\AngelaPlescia\Anatomiva" "InstallDir" "$INSTDIR"
 WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\Anatomiva" "DisplayName" "Anatomiva"
 WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\Anatomiva" "DisplayVersion" "2.0.0"
 WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\Anatomiva" "Publisher" "Angela Plescia"
 WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\Anatomiva" "UninstallString" '$\"$INSTDIR\Disinstalla.exe$\"'
 WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\Anatomiva" "DisplayIcon" "$INSTDIR\Anatomiva.exe"
 WriteRegDWORD HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\Anatomiva" "NoModify" 1
 WriteRegDWORD HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\Anatomiva" "NoRepair" 1
SectionEnd
Section "Uninstall"
 SetShellVarContext current
 SetRegView 64
 Delete "$DESKTOP\Anatomiva.lnk"
 Delete "$SMPROGRAMS\Anatomiva\Anatomiva.lnk"
 RMDir "$SMPROGRAMS\Anatomiva"
 Delete "$INSTDIR\Anatomiva.exe"
 Delete "$INSTDIR\Anatomiva.exe.config"
 Delete "$INSTDIR\GUIDA_RAPIDA.txt"
 Delete "$INSTDIR\FONTI.md"
 Delete "$INSTDIR\Disinstalla.exe"
 RMDir "$INSTDIR"
 DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\Anatomiva"
 DeleteRegKey HKCU "Software\AngelaPlescia\Anatomiva"
 ; User progress is intentionally preserved. Reset is available in the application.
SectionEnd
