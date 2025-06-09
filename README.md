# Low Pressure Zone

[Low Pressure Zone](https://lowpressurezone.com) is an online radio and DJing site focused on communities rather than hosting strong performers. Think of it as an online version of you and your friends spinning tunes in the garage rather than an online version of a concert. The name comes from a [Clubroot song](https://www.youtube.com/watch?v=0nctcPI-_nI) that I used to use as a set opener.

## Why I built this

When I was in college for Computer Science, the /r/realdubstep subreddit started the online radio site ClouwdNine so that subreddit members could DJ and chat with one another. Over time, the site grew significantly and larger events were held. In the early days of C9, I stumbled across it and started performing there before I had ever DJ'd a live show. The site maintainer couldn't make contributions to the site as often as they wished, and allowed me to improve the site.

I was given SSH access to the server (something I can't believe even to this day) and I started a git repository for the site's code. Over time, I made many contributions. The site was a home for me, a third space that shaped many experiences of my life since I found it. However, I had to leave C9 behind to go and live some of my own life's stories in 2016. The last shows on C9 took place in 2017 after a script for automating google sheets scheduling stopped working.

The tech stack of C9 was messy, built on Wordpress, Google Sheets, some cronjobs, a few python scripts, and some really bad javascript. I built Low Pressure Zone as a successor to C9, another third space for myself and (hopefully) many others to bring more life into their communities.

## Technical information

Low Pressure Zone is a web application deployed through Digital Ocean. The server is a VPS which, alongside a managed database, serves all consumers of the application. 

### Tech stack

The browser client is a Typescript/VueJS frontend built and deployed statically. Yarn is used for package management, and Vite is used as the development server.

The web API is a C#/AspNetCore application managing identity and data.

Nginx serves the static content and reverse-proxies API requests to the .NET app's Kestrel server. The site audio is served using an [Icecast 2 server](https://icecast.org/). SSL certificates are managed through Certbot.

### Deployment

Scripts are executed on the VPS manually to deploy changes in the software to the consumers. Especially now in the early stages of the app, I don't see a need to containerize the whole setup. However, when I eventually do, I will be setting up a CI/CD pipeline for it.

## Contributing

The code is available to fork, though I haven't written any documentation for development setup. If you are interested in working on the site, please reach out to me.

If you want to support me and the site maintainence, feel free to make a donation at https://coff.ee/phlank :)
