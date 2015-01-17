for /f "tokens=*" %%a in (buildnumber.txt) do (
  set version=%%a
)

set jabbadir=\\129.196.199.27\share\AirMagnet Build\Devo\%1

md "%jabbadir%\%version%"
copy "%GITDEVOW%\Build\build.log" "%jabbadir%\%version%"

md "%jabbadir%\%version%\compilelog"
copy "%GITDEVOW%\compile.log" "%jabbadir%\%version%\compilelog"

md "%jabbadir%\%version%\binary"
copy "%GITDEVOW%\AirMagnet.DevoWin.InstallWin\bin\Release\*.*" "%jabbadir%\%version%\binary"
xcopy /E "%GITDEVOW%\AirMagnet.DevoWin.UI\bin\Release" "%jabbadir%\%version%\binary" /Y

md "%jabbadir%\%version%\symbol"
move "%jabbadir%\%version%\binary\*.pdb" "%jabbadir%\%version%\symbol"
