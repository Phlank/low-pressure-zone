# Low Pressure Zone

[Low Pressure Zone](https://lowpressurezone.com) is an online radio and DJing site focused on communities rather than
hosting strong performers. Think of it as an online version of you and your friends spinning tunes in the garage rather
than an online version of a concert. The name comes from a [Clubroot song](https://www.youtube.com/watch?v=0nctcPI-_nI)
that I used to use as a set opener.

## Table of Contents

- [History](#history)
- [Technical information](#technical-information)
    - [Top level](#top-level)
    - [Client](#client)
    - [Server](#server)
- [Contributing](#contributing)

## History

Around ~2012 or so, some members of the /r/realdubstep subreddit took it upon themselves to set up weekly streaming
events for community members to participate in (subreddit get-togethers, basically). This was called C9 (or Clouwdnine)
and was run through a simple website. People would show up in the chat, one person would send audio and people would
listen directly from the server.

Eventually this gained traction and people wanted to do more than just the weekly shows, so they set up a soundclash
event. Two DJs/producers would take turns streaming audio over an hour and a half, spinning for three rounds, 15 minutes
each per round. After each round, there would be a vote in the chat and by the end of it you had a "winner" of that
soundclash. This caught the attention of FatKidOnFire, which eventually led to a soundclash event between C9 and FKOF.

I discovered C9 in 2013 and after becoming involved in the community, I was given the chance to contribute to the site.
The site was a home for me, a third space that shaped many experiences of my life since I found it. However, I had to
leave C9 behind in 2016. The last shows on C9 took place in 2017 after a script for automating Google Sheets scheduling
stopped working.

Low Pressure Zone is the successor to Clouwdnine, and is built with the same spirit of community in mind. I wanted to
create a space where people can share their productions, discover new tunes, and connect with others who share their
passion for soundsystem culture music.

## Technical information

Low Pressure Zone is a web application. The server is a VPS which, alongside a managed database, serves all consumers of
the application. The VPS hosts the static client files, the site API, and an Icecast instance where DJs can test their
stream setup and audio quality. Another VPS hosts AzuraCast, the main radio portion of the site.

### Top level

The top level of the repository contains a `package.json` that is used for executing different repository commands. In
the `tools` folder are a couple of deployment scripts to be run on the VPS when upgrading, and a `docker` directory
containing all the needed volume directories and environment setup for the app. A `docker-compose.yml` file makes
setting up the app for development straightforward. These tools can all be accessed simply at the top level using the
provided `npm run` commands in the top-level `package.json` file.

### Client

The client is a VueJS application written using Typescript and SCSS. The client app is held in the `src/client` folder,
with separate directories for API functions, UI components, composable functions, stores, utility functions, and
validation. The components are organized by use case; shared components reside in one of the subdirectories there, while
all route-specific components are held in `src/components/views`. Packages are managed with Yarn 4. PrimeVue is used as
a component library, with a custom form layout and validation setup used for making form management simpler.

### Server

The server component is a .NET application utilizing AspNetCore. The API services the client app,
including identity management, scheduling, news, and prerecorded DJ set uploads. The API also holds some admin-specific
endpoints used for disconnecting DJs from the radio and doing other management of the AzuraCast deployment.

FastEndpoints is used for building out API endpoints. Entity Framework Core is used for database interaction and
migrations. Hangfire is used to manage scheduled and delayed tasks.

## Contributing

The code is available to fork, though I haven't written any documentation for development setup. If you are interested
in working on the site, please reach out to me.

If you want to support me and the site maintenance, feel free to make a donation at https://coff.ee/phlank :)
