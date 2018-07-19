REM Start testlink Service
docker-compose up

REM Build solution
SET msbuild="C:/Program Files (x86)/Microsoft Visual Studio/2017/Enterprise/MSBuild/15.0/Bin/MSBuild.exe" 
%msbuild% /m src/TestLinkApi.sln /p:Configuration=Release /p:platform="Any CPU"

REM Run unit tests
tools\nunit3-console\nunit3-console.exe --noresult src/TestLinkApi.Tests/bin/Release/TestLinkApi.Tests.dll
