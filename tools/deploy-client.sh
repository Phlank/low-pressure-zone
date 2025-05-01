#!/bin/bash

cd src/client
yarn
sed -i 's/development/production/g' vite.config.ts
yarn build
sed -i 's/production/development/g' vite.config.ts
sudo rm -r /var/www/html/low-pressure-zone/*
sudo cp -r dist/* /var/www/html/low-pressure-zone
sudo chown -R root:www-data /var/www/html/low-pressure-zone
sudo chmod -R 770 /var/www/html/low-pressure-zone
sudo chmod 640 /var/www/html/low-pressure-zone/index.html
sudo chmod 640 /var/www/html/low-pressure-zone/assets/*
cd ../..
