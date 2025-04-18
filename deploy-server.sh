#!/bin/bash

sudo service lpzapi stop
cd src/server/LowPressureZone.Api
dotnet publish LowPressureZone.Api.csproj -c Release -v diag
cd bin/Release/net9.0
sudo rm -r /var/www/html/low-pressure-zone-api/* || true
sudo cp -r * /var/www/html/low-pressure-zone-api
sudo chown -R root:www-data /var/www/html/low-pressure-zone-api/*
sudo chmod -R 650 /var/www/html/low-pressure-zone-api/*
sudo service lpzapi start
cd ../../../../..
