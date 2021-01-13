# url-shortener-csharp

Sample URL Shortener written in C#.

This is a toy project to simply go from 0 and some spec to AWS prod using C# ecosystem.

This is not the code i write in prod, please don't PR me with cleaner version 🤣. A LOT of corners where cut. The bits that i felt especially bad about are commented accordingly, but some aren't, so if you have a sensitive nose, you were warned.

Links to all the similar repos are in [here](https://github.com/alanmynah/url-shorteners)

Optimising for speed of development.

## Requirements:

- .Net 5 web api
- docker
- ef core
- local mssql server, hosted on docker

I used mac + rider

## Installation

Never tried from scratch myself, but i imagine:

- clone:

```sh
git clone this-repo
cd /to/this/proj
```

- copy-paste `.env.example` to `.env` file for local dev.

- and run:

```sh
docker-compose up --build
# it now runs, head to example.http file and make some calls
```

If you want to run just dependencies via docker and the API separately in VS or Rider, run:

```sh
docker-compose -f docker-compose.yml -f docker-compose.local.yml up --build
```
