@echo off
REM Step 1: Build the project
dotnet build

REM Step 2: Navigate to the build output directory
cd src\bin\Debug\net6.0

REM Step 3: Move the DLL to the Among Us plugins directory
move /Y "MalumMenu.dll" "D:\SteamLibrary\steamapps\common\Among Us\BepInEx\plugins"

REM Optional: Print a message if the operation was successful
if %errorlevel%==0 (
    echo DLL successfully moved to Among Us plugins directory.
) else (
    echo Failed to move the DLL.
)
Pause Nul