
properties {
  $configuration = "Release"
  $version = ""
  $nugetApiKey = ""

  # Package Output
  $binRoot = "bin"

  # Source
  $srcRoot = "src\Harbour.RedisTempData"
  $srcProject = resolve-path "$srcRoot\*.csproj"
  
  # Tests
  $testsRoot = "tests\Harbour.RedisTempData.Test"
  $testProject = resolve-path "$testsRoot\*.csproj"

  $nuget = resolve-path ".nuget\nuget.exe"
  $xunit = resolve-path "packages\xunit.runners.*\tools\xunit.console.clr4.exe"
}

task default -depends build-package

task clean {
  if (test-path $binRoot) {
    rm $binRoot -force -recurse
  }
}

task build -depends clean {

  exec {
    msbuild "$srcProject" /t:"Clean;Build" /p:Configuration="$configuration"
  }

}

task test {

  exec {
    msbuild "$testProject" /t:"Clean;Build" /p:Configuration="$configuration"
  }

  exec {
    $testBin = resolve-path "$testsRoot\bin\$configuration\Harbour.RedisTempData.Test.dll"
    & $xunit "$testBin" /noshadow
  }

}

task build-package -depends build, test {
  
  mkdir $binRoot

  exec {
    & $nuget pack Harbour.RedisTempData.nuspec -outputDirectory $binRoot
  }

}

task deploy-package -depends build-package {

  exec {
    $nugetPackage = resolve-path "$binRoot\*.nupkg"
    & $nuget push "$nugetPackage" $nugetApiKey
  }

}

task bump-version {

  write-host "Updating the Nuspec..."

  $nuspec = resolve-path "*.nuspec"
  (gc $nuspec) -replace "\<version\>(.*)\<\/version\>","<version>$version</version>" | out-file $nuspec -encoding "UTF8"

  write-host "Updating AssemblyInfo..."

  $assemblyInfo = "$srcRoot\Properties\AssemblyInfo.cs"
  (gc $assemblyInfo) -replace "AssemblyVersion\(`"(.*)`"\)","AssemblyVersion(`"$version`")" | out-file $assemblyInfo -encoding "UTF8"
  (gc $assemblyInfo) -replace "AssemblyFileVersion\(`"(.*)`"\)","AssemblyFileVersion(`"$version`")" | out-file $assemblyInfo -encoding "UTF8"
}