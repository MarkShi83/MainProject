dotnet test /p:collectcoverage=true /p:CoverletOutputFormat=opencover --logger "trx;LogFileName=Results.xml"
ReportGenerator.exe -reports:"coverage.opencover.xml" -targetdir:"report" -sourcedirs:"../"