echo "This process will delete appsettings.Development.json files if they exist. Ctrl+C if this is not desired."
echo "The development environment requires some configuration. These variables will be set in the corresponding appsettings.Development.json files."
echo "Admin user email:"
read admin_email
echo "Admin username:"
read admin_username
echo "Admin display name:"
read admin_displayname
echo "Admin password:"
read admin_password

rm src/server/LowPressureZone.Aspire.Migrations/appsettings.Development.json
rm src/server/LowPressureZone.Api/appsettings.Development.json

cp src/server/LowPressureZone.Aspire.Migrations/appsettings-template.Development.json src/server/LowPressureZone.Aspire.Migrations/appsettings.Development.json
sed -i "s/{AdminUsername}/$admin_username/g" src/server/LowPressureZone.Aspire.Migrations/appsettings.Development.json
sed -i "s/{AdminDisplayName}/$admin_displayname/g" src/server/LowPressureZone.Aspire.Migrations/appsettings.Development.json
sed -i "s/{AdminEmail}/$admin_email/g" src/server/LowPressureZone.Aspire.Migrations/appsettings.Development.json
sed -i "s/{AdminPassword}/$admin_password/g" src/server/LowPressureZone.Aspire.Migrations/appsettings.Development.json

cp src/server/LowPressureZone.Api/appsettings-template.Development.json src/server/LowPressureZone.Api/appsettings.Development.json
sed -i "s/{AdminEmail}/$admin_email/g" src/server/LowPressureZone.Api/appsettings.Development.json

rm -r tools/mounts/azuracast
rm -r tools/mounts/icecast2
cp -r tools/mounts/init/* tools/mounts

echo "Configuration is complete. Run the application using the LowPressureZone.Aspire project located in src/server/LowPressureZone.Aspire"