$ErrorActionPreference = 'Stop'

$mysqlPassword = Read-Host 'MySQL root password' -AsSecureString
$jwtKey = Read-Host 'JWT signing key (at least 32 characters; input is hidden)' -AsSecureString

$mysqlPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($mysqlPassword)
$jwtPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($jwtKey)
try {
    $env:ConnectionStrings__Default = "Server=127.0.0.1;Port=3306;Database=game_asm1;User=root;Password=$([Runtime.InteropServices.Marshal]::PtrToStringBSTR($mysqlPointer));"
    $env:Jwt__Key = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($jwtPointer)
    if ([Text.Encoding]::UTF8.GetByteCount($env:Jwt__Key) -lt 32) {
        throw 'JWT signing key must be at least 32 bytes.'
    }

    dotnet run --project $PSScriptRoot\GameApi.csproj
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
finally {
    Remove-Item Env:ConnectionStrings__Default, Env:Jwt__Key -ErrorAction SilentlyContinue
    [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($mysqlPointer)
    [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($jwtPointer)
    $mysqlPassword.Dispose()
    $jwtKey.Dispose()
}
