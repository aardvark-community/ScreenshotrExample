IF NOT exist .paket (
    dotnet tool restore
    dotnet paket restore
)

dotnet build src\screenhotr.example.sln

pause