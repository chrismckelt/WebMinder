@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)
 
set version=1.1
if not "%PackageVersion%" == "" (
   set version=%PackageVersion%
)

set nuget=%2
if "%nuget%" == "" (
	set nuget=nuget
)

mkdir lib


%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild WebMinder.sln /p:Configuration="Release" /p:OutputPath=$(SolutionDir)\lib /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=diag /nr:false
%nuget% pack "WebMinder.nuspec" -NoPackageAnalysis -verbosity detailed -o lib -Version %version% -p Configuration="Release"