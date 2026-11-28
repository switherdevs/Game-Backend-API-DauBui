$ErrorActionPreference = 'Stop'

$mysqlPassword = Read-Host 'MySQL root password' -AsSecureString
$jwtKey = Read-Host 'JWT signing key (at least 32 characters; input is hidden)' -AsSecureString

$mysqlPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($mysqlPassword)
$jwtPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($jwtKey)
try {
    $connection = [System.Data.Common.DbConnectionStringBuilder]::new()
    $connection['Server'] = '127.0.0.1'
    $connection['Port'] = 3306
    $connection['Database'] = 'game_asm1'
    $connection['User ID'] = 'root'
    $connection['Password'] = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($mysqlPointer)
    $env:ConnectionStrings__Default = $connection.ConnectionString
    $env:Jwt__Key = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($jwtPointer)
    if ([Text.Encoding]::UTF8.GetByteCount($env:Jwt__Key) -lt 32) {
        throw 'JWT signing key must be at least 32 bytes.'
    }

    # Free port 5254 if an old GameApi instance is still lingering
    $oldConnections = Get-NetTCPConnection -LocalPort 5254 -ErrorAction SilentlyContinue
    foreach ($conn in $oldConnections) {
        $p = Get-Process -Id $conn.OwningProcess -ErrorAction SilentlyContinue
        if ($p -and $p.ProcessName -match "GameApi|dotnet") {
            Write-Host "Closing previous backend process (PID: $($p.Id))..."
            Stop-Process -Id $p.Id -Force -ErrorAction SilentlyContinue
            Start-Sleep -Milliseconds 500
        }
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
