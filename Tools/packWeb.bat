@echo off

cd ..\..\Waveface.Stream.WindowsClient
if exist ..\Setup\WebFiles.zip del /q /f ..\Setup\WebFiles.zip
..\..\Tools\7z.exe a ..\Setup\WebFiles.zip Web