@echo off
setlocal
cd /d "%~dp0\.."
if not exist build mkdir build
set "BODY_CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if not exist "%BODY_CSC%" set "BODY_CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe"
if not exist "%BODY_CSC%" (
 echo Compiler not found. Install the .NET Framework developer tools.
 exit /b 1
)
"%BODY_CSC%" /nologo /langversion:5 /target:winexe /platform:anycpu /optimize+ /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:System.Xml.Linq.dll /resource:content\catalog.xml,catalog.xml /resource:packaging\app.ico,app.ico /win32icon:packaging\app.ico /win32manifest:packaging\app.manifest /out:build\Anatomiva.exe src\*.cs
if errorlevel 1 exit /b 1
copy /y packaging\Anatomiva.exe.config build\ >nul
"%BODY_CSC%" /nologo /langversion:5 /reference:System.Xml.Linq.dll /resource:content\catalog.xml,catalog.xml /out:build\CoreTests.exe src\Core.cs src\Learning.cs src\ProgressExport.cs tests\CoreTests.cs
if errorlevel 1 exit /b 1
build\CoreTests.exe
if errorlevel 1 exit /b 1
where makensis >nul 2>nul
if not errorlevel 1 (
 pushd packaging
 makensis installer.nsi
 if errorlevel 1 (popd & exit /b 1)
 popd
) else (
 echo App built. To also build the installer, install NSIS and add it to PATH.
)
echo Build completed.
