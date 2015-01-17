cd "%GITDEVOW%\Build"
del build.log

cd "%GITDEVOW%\Build"
call DevoWIncreaseVersion.bat %1 %2
IF %ERRORLEVEL% neq 0 (
goto end
)

set folder=%2\Eng

IF "%2" == "master" (
set folder=Windows
)

IF "%2" == "DevoW1.0" (
set folder=DevoW_Windows_1.0\Eng
)

IF "%2" == "DevoW1.1" (
set folder=DevoW_Windows_1.1\Eng
)

cd "%GITDEVOW%\Build"
call DevoWCompile.bat
IF %ERRORLEVEL% neq 0 (
cd "%GITDEVOW%\Build"
echo compile failed >> build.log
call DevoWPublishFailInfo.bat %folder%
goto end
)

cd "%GITDEVOW%\Build"
call DevoWPublishBinary.bat %folder%

cd "%GITDEVOW%\Build"
call DevoCompileInstallShield.bat
IF %ERRORLEVEL% neq 0 (
cd "%GITDEVOW%\Build"
echo install shiled compile failed >> build.log
call DevoWPublishPackageFailInfo.bat %folder%
goto end
)

cd "%GITDEVOW%\Build"
call DevoWPublishPackage.bat %folder%

:end
cd "%GITDEVOW%\Build"
call DevoWBackupToChinaServer.bat %folder%

echo "build complete"




