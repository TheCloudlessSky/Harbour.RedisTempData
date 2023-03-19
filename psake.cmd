.\.nuget\nuget.exe install psake -Version 4.9.0 -OutputDirectory packages && ^
.\packages\psake.4.9.0\tools\psake\psake.cmd -buildFile .\build\default.ps1 %*