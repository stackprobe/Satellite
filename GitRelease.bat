IF NOT EXIST .\GitRelease.bat GOTO END
CALL qq
C:\Factory\SubTools\GitFactory.exe /ow . C:\huge\GitHub\Satellite
rem CALL C:\var\go.bat
:END
