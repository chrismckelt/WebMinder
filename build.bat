@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)
 
set version=1.0.0
if not "%PackageVersion%" == "" (
   set version=%PackageVersion%
)

set nuget=%2
if "%nuget%" == "" (
	set nuget=nuget
)

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild WebMinder.sln /p:Configuration="Release" /p:OutputPath=build /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=diag /nr:false

mkdir Build
%nuget% pack "WebMinder.nuspec" -NoPackageAnalysis -verbosity detailed -o Build -Version %version% -p Configuration="Release"