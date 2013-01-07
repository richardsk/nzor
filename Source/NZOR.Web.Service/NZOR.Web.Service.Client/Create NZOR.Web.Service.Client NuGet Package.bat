REM Creates a NuGet package for the NZOR .NET Client and stores it in the Local Repository

ECHO This will publish the solution from the RELEASE configuration to the local repository

PAUSE

NuGet.exe pack .\NZOR.Web.Service.Client.csproj -Prop Configuration=Release -OutputDirectory "\\lofty\Landcare NuGet Package Source"

PAUSE
