#!/bin/bash

echo "Stopping API service"
sudo service lpzapi stop

echo "Migrating databases"
dotnet ef database update --context DataContext --project ./src/server/LowPressureZone.Domain/LowPressureZone.Domain.csproj --startup-project ./src/server/LowPressureZone.Api/LowPressureZone.Api.csproj
dotnet ef database update --context IdentityContext --project ./src/server/LowPressureZone.Identity/LowPressureZone.Identity.csproj --startup-project ./src/server/LowPressureZone.Api/LowPressureZone.Api.csproj

echo "Publishing app"
cd src/server/LowPressureZone.Api || return
rm Properties/launchSettings.json
cp Properties/launchSettings-production.json Properties/launchSettings.json
dotnet publish LowPressureZone.Api.csproj -c Release -v diag

echo "Copying app to service directory"
cd bin/Release/net9.0 || return
sudo rm -r /var/www/html/low-pressure-zone-api/* || true
sudo cp -r * /var/www/html/low-pressure-zone-api
sudo chown -R root:www-data /var/www/html/low-pressure-zone-api/*
sudo chmod -R 650 /var/www/html/low-pressure-zone-api/*

echo "Starting service"
sudo service lpzapi start
cd ../../../../..
