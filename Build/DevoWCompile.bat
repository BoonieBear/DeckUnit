cd "%GITDEVOW%"

del compile.log

rmdir AirMagnet.DevoWin.UI\bin\Release\ /s /q

cd "%GITDEVOW%"
call "%VS110COMMON%\IDE\devenv.exe" AirMagnet.DevoWin_All.sln /Rebuild Release /Out compile.log

ren "%GITDEVOW%\AirMagnet.DevoWin.UI\bin\Release\AirMagnet.AircheckWifiTester.Views.exe" "AirCheck Wi-Fi Tester.exe"
ren "%GITDEVOW%\AirMagnet.DevoWin.UI\bin\Release\AirMagnet.AircheckWifiTester.Views.exe.config" "AirCheck Wi-Fi Tester.exe.config"

