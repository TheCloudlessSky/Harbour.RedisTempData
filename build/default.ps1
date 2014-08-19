properties {
  $nugetApiKey = ""

  $binRoot = "..\bin"
  $solution = "..\Harbour.RedisTempData.sln"
  $testsRoot = "..\tests\Harbour.RedisTempData.Test"
  $nuspec = "..\Harbour.RedisTempData.nuspec"

  $nuget = resolve-path "..\.nuget\nuget.exe"
  $xunit = resolve-path "..\packages\xunit.runners.*\tools\xunit.console.clr4.exe"
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

task deploy-package -depends build-package {

  exec {
    $nugetPackage = resolve-path "$binRoot\*.nupkg"
    & $nuget push "$nugetPackage" $nugetApiKey
  }

}