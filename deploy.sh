sudo service lpzapi stop
sudo rm -rf /var/www/html/low-pressure-zone-api/*
sudo mv publish/* /var/www/html/low-pressure-zone-api/
sudo service lpzapi start
