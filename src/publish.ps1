$apiKey=$env:NUGET_APIKEY

# Packages (nupkg)
Get-ChildItem .\FluentResults\bin\Release\ | Where{$_.FullName -Match "Drizin.FluentResults.*\.nupkg$" } | ForEach { dotnet nuget push $_.FullName --api-key $apiKey --source https://api.nuget.org/v3/index.json --skip-duplicate }

# Symbols (snupkg) will be automatically uploaded after the main package (nupkg). But this can be used to explicitly upload a missing symbol (specially useful for first release since it won't upload automatically)
Get-ChildItem .\FluentResults\bin\Release\ | Where{$_.FullName -Match "Drizin.FluentResults.*\.snupkg$" } | ForEach { dotnet nuget push $_.FullName --api-key $apiKey --source https://api.nuget.org/v3/index.json --skip-duplicate }