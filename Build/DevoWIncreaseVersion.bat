cd "%GITDEVOW%\Build"

:start

call AssemblyInfoUtil.exe -inc:3 "../AirMagnet.DevoWin.UI/Properties/SharedAssemblyInfo.cs"

for /f "tokens=*" %%a in (buildnumber.txt) do (
  set version=%%a
)

call git commit -m "build for %version%" -a
call git push -f %1 %2

IF %ERRORLEVEL% neq 0 (
call git clead -fd
call git reset --hard HEAD~1
call git pull --rebase %1
call git checkout %2

goto start
)
