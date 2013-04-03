@echo off
set path=%path%;C:/Windows/Microsoft.NET/Framework/v4.0.30319;

echo Under construction. Use build-unmerged.cmd instead.
goto end

echo Building project...
msbuild src/Anodyne.sln /nologo /v:q /p:Configuration=Release /t:Clean
msbuild src/Anodyne.sln /nologo /v:q /p:Configuration=Release /clp:ErrorsOnly

echo Merging assemblies...
if exist output rmdir /s /q output
mkdir output
mkdir output\bin

:tools\ilmerge\ILMerge.exe /keyfile:src\Anodyne.snk /wildcards /target:library
tools\ilmerge\ILMerge.exe /target:library ^
 /targetplatform:"v4,C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319" ^
 /out:"output\bin\Anodyne-Core.dll" ^
 "src\main\Anodyne-Common\bin\Release\Anodyne-Common.dll" ^
 "src\main\Anodyne-Common\bin\Release\Anodyne-Abstractions.dll" ^
 "src\main\Anodyne-Node\bin\Release\Anodyne-Node.dll" ^
 "src\main\Anodyne-Domain\bin\Release\Anodyne-Domain.dll" ^
 "src\main\Anodyne-Wiring\bin\Release\Anodyne-Wiring.dll" ^
 "src\main\Anodyne-DataAccess\bin\Release\Anodyne-DataAccess.dll"

copy src\main\Anodyne-Windsor\bin\Release\Anodyne-Windsor.* output\bin
copy src\main\Anodyne-Autofac\bin\Release\Anodyne-Autofac.* output\bin
copy src\main\Anodyne-StructureMap\bin\Release\Anodyne-StructureMap.* output\bin
copy src\main\Anodyne-MongoDb\bin\Release\Anodyne-MongoDb.* output\bin
copy src\main\Anodyne-Web\bin\Release\Anodyne-Web.* output\bin
copy src\main\Anodyne-Web-Mvc\bin\Release\Anodyne-Web-Mvc.* output\bin
copy src\main\Anodyne-Web-Mvc4\bin\Release\Anodyne-Web-Mvc4.* output\bin

echo Done.

:end