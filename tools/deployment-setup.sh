sudo cp tools/service.template /etc/systemd/system/lpzapi.service
sudo mkdir /var/www/html/low-pressure-zone
sudo chown -R root:www-data /var/www/html/low-pressure-zone 
sudo mkdir /var/www/html/low-pressure-zone-api
sudo chown -R root:www-data /var/www/html/low-pressure-zone-api