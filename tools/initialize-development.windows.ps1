Write-Host "This process will delete appsettings.Development.json files if they exist. Ctrl+C if this is not desired."
Write-Host "The development environment requires some configuration. These variables will be set in the corresponding appsettings.Development.json files."

$admin_email = Read-Host "Admin user email"
$admin_username = Read-Host "Admin username"
$admin_displayname = Read-Host "Admin display name"
$admin_password = Read-Host "Admin password"

$aspireMigrationsPath = "src/server/LowPressureZone.Aspire.Migrations"
$apiPath = "src/server/LowPressureZone.Api"

$aspireDevSettings = Join-Path $aspireMigrationsPath "appsettings.Development.json"
$aspireTemplate = Join-Path $aspireMigrationsPath "appsettings-template.Development.json"
$apiDevSettings = Join-Path $apiPath "appsettings.Development.json"
$apiTemplate = Join-Path $apiPath "appsettings-template.Development.json"

if (Test-Path $aspireDevSettings) { Remove-Item -Force $aspireDevSettings }
if (Test-Path $apiDevSettings) { Remove-Item -Force $apiDevSettings }

Copy-Item -Force $aspireTemplate $aspireDevSettings

$aspireContent = Get-Content -Raw $aspireDevSettings
$aspireContent = $aspireContent -replace [regex]::Escape("{AdminUsername}"), $admin_username
$aspireContent = $aspireContent -replace [regex]::Escape("{AdminDisplayName}"), $admin_displayname
$aspireContent = $aspireContent -replace [regex]::Escape("{AdminEmail}"), $admin_email
$aspireContent = $aspireContent -replace [regex]::Escape("{AdminPassword}"), $admin_password
Set-Content $aspireDevSettings $aspireContent

Copy-Item -Force $apiTemplate $apiDevSettings

$apiContent = Get-Content -Raw $apiDevSettings
$apiContent = $apiContent -replace [regex]::Escape("{AdminEmail}"), $admin_email
Set-Content $apiDevSettings $apiContent

$azuracastPath = "tools/mounts/azuracast"
$icecastPath = "tools/mounts/icecast2"

if (Test-Path $azuracastPath) { Remove-Item -Recurse -Force $azuracastPath }
if (Test-Path $icecastPath) { Remove-Item -Recurse -Force $icecastPath }

Copy-Item -Recurse -Force "tools/mounts/init/*" "tools/mounts"

Write-Host "Configuration is complete. Run the application using the LowPressureZone.Aspire project located in src/server/LowPressureZone.Aspire"
