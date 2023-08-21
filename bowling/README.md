# Next Level Bowling Kata

## Introduction

Everyone that has done coding katas probably knows the [bowling kata (PowerPoint)][bowling] [🇩🇪][bowling-de]. I wanted to take this to the next level and implement a fully fledged web version of this kata with an angular frontend and a .NET WebApi as a backend, backed by Auth0 and SQL Server Express, runnable with `docker compose`. By now, I'm in the process of scaffolding the solution, therefor most parts are not yet in a usable state.

## Structure

- `docker-compose.yml` - the YAML definition for `docker compose`

### `bowling-frontend`

This folder contains the Angular frontend of the solution.

### `bowling-backend`

This folder contains the .NET Core WebApi backend.

## Running the sample

### Requirements

#### Auth0

For obvious reasons I do not include my Auth0 configuration, therefor you'll have to create your own account and set it up for the solution. I will **not** explain how to set up Auth0 here, but only how to configure this solution when you've already set up your Auth0 App.

The Angular application is configured to load the Auth0 configuration from `src/environments/.env` and automatically create a `environment.ts` file in that folder if `npm run config`, `npm run build` or `npm run start` is executed. Since `npm run build` is used to build the dockerized Angular application, all you have to do is to add a `src/environments/.env` file with the following contents

```
AUTH0_CLIENT_ID={Insert your Auth0 client ID here}
AUTH0_DOMAIN={Insert your Auth0 domain here}
AUTH0_AUDIENCE={Insert your Auth0 APIs identifier here}
```

Unfortunately these values have to be set for the ASP.NET Core backend separately. Therefor you have to create a file `appsettings.local.json` in the `bowling-backend-api` folder, that contains the follwing JSON

```json
{
    "auth0": {
        "Audience": "{Insert your Auth0 APIs identifier here}",
        "Authority": "{Insert your Auth0 domain here}"
    }
}
```

You can find the values in the settings page of your Auth0 App.

Probably this process will be streamlined in the future, but that's no a priority whatsoever.

#### Docker

Docker with docker compose v2 has to be installed.

### Running with `docker compose`

After Auth0 has been set up, running the solution should be as easy as calling

```cmd
docker compose build
docker compose up
```

[bowling]: http://butunclebob.com/files/downloads/Bowling%20Game%20Kata.ppt
[bowling-de]: https://ccd-school.de/coding-dojo/class-katas/bowling/