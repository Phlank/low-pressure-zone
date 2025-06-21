# Azuracast setup

1. Install to new droplet on DigitalOcean
2. When IP becomes public, load and add account
3. Set up basic information and then navigate to dashboard
4. Manage radio at bottom of site
5. Enable streamers/djs
6. Navigate to Broadcasting (left menu) and go to Mount Points
    1. Edit mount point Radio
        1. Set to 320kbps
        2. Turn AutoDJ off
        3. Do not show on public pages
        4. Fallback: /live
    2. Add mount point, /live
        1. Do not show on public pages
        2. Do not publish to yellow pages
        3. Fallback mount: /offline
        4. Set quality to 320kbps
    3. Add mount point, /offline
       1. AutoDJ on

# What stays the same?

1. Streaming password stays the same. The URL and port will be different.
    1. New port: 8005
    2. I would love to have "radio.lowpressurezone.com" as the URL for the AzuraCast instance.