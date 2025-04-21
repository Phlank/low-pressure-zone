# Low Pressure Zone

[Low Pressure Zone](https://lowpressurezone.com) is an online radio and DJing site focused on communities rather than hosting strong performers. Think of it as an online version of you and your friends spinning tunes in the garage rather than an online version of a concert. The name comes from a [Clubroot song](https://www.youtube.com/watch?v=0nctcPI-_nI) that I used to use as a set opener.

## Why I built this

When I was in college for Computer Science, the /r/realdubstep subreddit started the online radio site ClouwdNine so that subreddit members could DJ and chat with one another. Over time, the site grew significantly and larger events were held. In the early days of C9, I stumbled across it and started performing there before I had ever DJ'd a live show. The site maintainer was busy with work and family life and couldn't make contributions to the site as often as they wished, and allowed me to improve the site.

I was given SSH access to the server (something I can't believe even to this day) and I started a git repository for the site's code. Over time, I made many contributions. The site was a home for me, a third space that shaped many experiences of my life since I found it. However, I had to leave C9 behind to go and live some of my own life's stories in 2016. The last shows on C9 took place in 2017 after a script for automating google sheets scheduling stopped working.

The tech stack of C9 was messy, built on Wordpress, Google Sheets, some cronjobs, a few python scripts, and some really bad javascript. I built Low Pressure Zone as a successor to C9, another third space for myself and (hopefully) many others to bring more life into their communities.

## Tech stack

Low Pressure Zone is a web application deployed on the web through Digital Ocean. The server is a VPS which, alongside a managed database, serve all consumers of the application. As software updates occur, scripts are executed on the VPS manually to deploy changes in the software to the consumers. Especially in the early stages of the app, there is zero need to containerize everything. It would just make configuration updates more difficult with no perceived benefit.

The app client is a Typescript/VueJS frontend built and deployed statically. The web API is a .NET application managing identity and data. Nginx serves the static content and reverse-proxies API requests to the .NET app's Kestrel server. The site audio is served using an [Icecast 2 server](https://icecast.org/). SSL certificates are managed through Certbot.

All portions of the site are built using open-source software libraries and possible.

## Contributing

For the next while, especially in the infancy of the app, I wish to be the sole site maintainer. As things stabilize, this will probably change. I hope to eventually support students in college and give them a way to contribute to OSS prior to finding work, hopefully passing forward to others some of the experiences I had with C9.