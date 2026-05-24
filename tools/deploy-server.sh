#!/bin/bash

echo "Stopping API service"
sudo service lpzapi stop

echo "Publishing app"
cd src/server/LowPressureZone.Api || return
rm Properties/launchSettings.json
cp Properties/launchSettings-production.json Properties/launchSettings.json
dotnet publish LowPressureZone.Api.csproj -c Release -v diag -o app

echo "Copying app to service directory"
cd app || return
sudo rm -r /var/www/html/low-pressure-zone-api/* || true 
sudo cp -r * /var/www/html/low-pressure-zone-api
sudo chown -R root:www-data /var/www/html/low-pressure-zone-api/*
sudo chmod -R 650 /var/www/html/low-pressure-zone-api/*

echo "Starting service"
sudo service lpzapi start
cd ../../../../..
