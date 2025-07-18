## Quick start

The easiest way to start the Dify server is through Docker Compose. Before running Dify with the following commands, make sure that [Docker](https://docs.docker.com/get-docker/) and [Docker Compose](https://docs.docker.com/compose/install/) are installed on your machine:

#### Start Dev
```bash
docker-compose -f docker-compose.dev.yml up -d --force-recreate --build
```

#### Start Prod
```bash
docker-compose -f docker-compose.prod.yml up -d --force-recreate --build
```

Once the services are running, you can access them in your browser using the following URLs:
- identityserver: http://localhost:5295
- aspnet-client-app: http://localhost:5006
- nextjs-client-app: http://localhost:3000