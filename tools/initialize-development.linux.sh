echo "This process will delete appsettings.Development.json files if they exist. Ctrl+C if this is not desired."
echo "The development environment requires some configuration. These variables will be set in the corresponding appsettings.Development.json files."
echo "Admin user email:"
read admin_email
echo "Admin username:"
read admin_username
echo "Admin display name:"
read admin_displayname

rm src/server/LowPressureZone.Api/appsettings.Development.json

cp src/LowPressureZone.Aspire/appsettings-template.Development.json src/LowPressureZone.Aspire/appsettings.Development.json
sed -i "s/{AdminUsername}/$admin_username/g" src/LowPressureZone.Aspire/appsettings.Development.json
sed -i "s/{AdminDisplayName}/$admin_displayname/g" src/LowPressureZone.Aspire/appsettings.Development.json
sed -i "s/{AdminEmail}/$admin_email/g" src/LowPressureZone.Aspire/appsettings.Development.json

rm -r tools/mounts/azuracast
rm -r tools/mounts/icecast2
cp -r tools/mounts/init/* tools/mounts

echo "Configuration is complete. Run the application using the LowPressureZone.Aspire project located in src/server/LowPressureZone.Aspire"