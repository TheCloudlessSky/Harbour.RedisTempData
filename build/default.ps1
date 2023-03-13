properties {
  $binRoot = "..\bin"
  $solution = "..\Harbour.RedisTempData.sln"
  $testsRoot = "..\tests\Harbour.RedisTempData.Test"
  $nuspec = "..\Harbour.RedisTempData.nuspec"
  $nugetApiKey = ""
  $nugetSource = ""
  $nuget = resolve-path "..\.nuget\nuget.exe"
}

task default -depends build-package

task clean {
  if (test-path $binRoot) {
    rm $binRoot -force -recurse
  }
}

task build -depends clean {

  exec {
    # NOTE: 1591 is XML docs.
    msbuild "$solution" /t:"Clean;Build" /p:Configuration="Release" /p:NoWarn=1591
  }

}

task test {

  $xunit = resolve-path "..\packages\xunit.runners.*\tools\xunit.console.clr4.exe"

  exec {
    $testBin = resolve-path "$testsRoot\bin\Release\Harbour.RedisTempData.Test.dll"
    & $xunit "$testBin" /noshadow
  }

}

task build-package -depends build, test {
  
  mkdir $binRoot

  exec {
    & $nuget pack $nuspec -outputDirectory $binRoot
  }

}

task publish-package -depends build-package {

  exec {
    $nugetPackage = resolve-path "$binRoot\*.nupkg"
    & $nuget push "$nugetPackage" -ApiKey $nugetApiKey -Source $nugetSource
  }

}