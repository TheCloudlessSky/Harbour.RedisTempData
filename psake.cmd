.\.nuget\nuget.exe install psake -Version 4.9.0 -OutputDirectory packages && ^
.\.nuget\nuget.exe install xunit.runners -Version 1.9.2 -OutputDirectory packages && ^
.\packages\psake.4.9.0\tools\psake\psake.cmd -buildFile .\build\default.ps1 %*