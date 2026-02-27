# Developing Low Pressure Zone

This document contains information regarding the local environment setup needed for developing Low Pressure Zone.

# Setup guide

## Prerequisites

Development for Low Pressure Zone requires the following to be installed on your machine:

1. Git.
2. Any functional container
   host ([Docker Desktop](https://www.docker.com/products/docker-desktop/), [Podman](https://podman.io/), etc.).
3. [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0).
4. [Node.js](https://nodejs.org/en) with a major version of 22 or greater.
    1. This must be accessible via the PATH.
        1. [Known challenge (Linux)](https://github.com/dotnet/aspire/issues/5571): Using `nvm` to manage the Node.js
           version installed on your machine does not add Node.js binaries to the PATH, but appends them when starting
           the terminal (using .bashrc or any other terminal config). This will result in an error when running the
           AppHost from within JetBrains Rider, as the AppHost doesn't use the typical terminal environment for
           executing commands.
    2. Ensure you have executed `corepack enable` to enable `yarn`, the package manager for the client project.
5. [JetBrains Rider](https://www.jetbrains.com/rider/) for the IDE.
    1. Free for open-source use.
    2. Cross-platform, formatting and cleanup profiles will be applied uniformly between development environments.
    3. Handles both server and client code extremely well.

## Setup steps

### Fork

1. [Fork](https://github.com/Phlank/low-pressure-zone/fork) the repository to your account.
2. On your machine, clone the fork repository using `git clone <your-fork-repo-url>`.

### Initialize development files

The development runtime requires some initial setup. This is automated via an `npm` script.

1. In the top-level of the repository, run `npm run initialize-development`.
2. When prompted, provide information for an admin user's information. These credentials will be how you log into the
   site when running it locally.

This does two things:

1. Copies the template container mounts from the `tools/mounts/init` directory into the `tools/mounts` directory. This
   contains files needed for the Icecast2 container and the AzuraCast container. These are to automatically do the
   initial work of configuring both Icecast2 and AzuraCast to match the Low Pressure Zone deployed environment.
2. Creates the initial `appsettings.Development.json` files in the LowPressureZone.Aspire.Migrations and
   LowPressureZone.Api projects.

### Running the application

1. Open JetBrains Rider.
2. Using the menu at the top left, do **File** > **Open** > **Open** and navigate to the repository location.
3. Select the `LowPressureZone.sln` file and then select **OK**.
4. Using the Explorer button (or **View** > **Tool Windows** > **Explorer**), open the Explorer tool and view the files
   in the solution.
    1. Expand the **LowPressureZone** menu item, then expand the **0. Development Runtime** solution folder.
    2. Right click the **LowPressureZone.Aspire** item and select **Run 'LowPressureZone.Aspire'** in the context menu
       that appears.

### View the AppHost dashboard

At this point, orchestration should have begun. In Rider,

1. Open the Services view (accessible via the **Services** button or **View** > **Tool Windows** > **Services**)
2. View the output for LowPressureZone.Aspire. It should contain the console output for the running AppHost.
3. **Click the link that follows the text *"Login to the dashboard at"*.**

At this page, you can view the status of all components of the application running, view their logs, etc.

### Log into the web client

To ensure database information was seeded correctly, we will log into the application. On the AppHost dashboard:

1. On the AppHost dashboard, open the web client in a new tab using the first URL listed for the `lpz-client` resource
   on the AppHost dashboard. Log into the site as usual using the admin credentials specified earlier.
2. On the AppHost dashboard, open the mailpit dashboard in a new tab using the http URL listed for the `mailpit`
   resource. You should see an email for the two-factor authentication token needed to complete login. Use this token to
   complete the login on the web client tab.