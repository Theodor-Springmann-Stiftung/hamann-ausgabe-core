# Docker Deployment for HaWeb

## Quick Start

### 1. Create the external volume
```bash
docker volume create hamann_data
```

### 2. Set webhook secret (optional)
```bash
export WEBHOOK_SECRET="your-secret-here"
```

### 3. Build and run
```bash
docker-compose up -d --build
```

### 4. View logs
```bash
docker-compose logs -f
```

## Configuration

The application runs with the following defaults:
- **HTTP Port**: 5000
- **HTTPS Port**: 5001 (self-signed cert)
- **Data Path**: `/app/data` (mounted to `hamann_data` volume)
- **Repository**: https://github.com/Theodor-Springmann-Stiftung/hamann-xml
- **Branch**: main

### Environment Variables

Override in `docker-compose.yml` or set before running:

- `DOTNET_ENVIRONMENT`: `Production`, `Staging`, or `Development`
- `FileStoragePath`: Base path for data storage (default: `/app/data`)
- `RepositoryBranch`: Git branch to track
- `RepositoryURL`: Git repository URL
- `WebhookSecret`: GitHub webhook secret for signature validation

## Data Structure

Inside the `hamann_data` volume:
```
/app/data/
  ├── GIT/          # Git repository with XML sources
  └── HAMANN/       # Compiled Hamann.xml files
```

## Webhook Setup

Configure GitHub webhook to POST to:
```
http://your-server:5000/api/webhook/git
```

Or with reverse proxy:
```
https://your-domain.com/api/webhook/git
```

Set Content-Type to `application/json` and add your webhook secret.

## Production Deployment

### With Reverse Proxy (Recommended)

Add to your nginx/traefik config:
```nginx
location / {
    proxy_pass http://localhost:5000;
    proxy_http_version 1.1;
    proxy_set_header Upgrade $http_upgrade;
    proxy_set_header Connection "upgrade";
    proxy_set_header Host $host;
    proxy_set_header X-Real-IP $remote_addr;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto $scheme;
}
```

### Update docker-compose.yml

Remove port exposure if using reverse proxy:
```yaml
services:
  web:
    build: .
    volumes:
      - hamann_data:/app/data
    # ports:  # Comment out for reverse proxy
    #   - "5000:5000"
    environment:
      - ASPNETCORE_URLS=http://+:5000
```

## Troubleshooting

### View application logs
```bash
docker-compose logs -f web
```

### Access container shell
```bash
docker-compose exec web /bin/bash
```

### Check data volume
```bash
docker volume inspect hamann_data
```

### Rebuild from scratch
```bash
docker-compose down
docker-compose up -d --build --force-recreate
```

### Manual Git operations
```bash
docker-compose exec web /bin/bash
cd /app/data/GIT
git status
```