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

For obvious reasons I do not include my Auth0 configuration, therefor you'll have to create your own account and set it up for the solution.

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