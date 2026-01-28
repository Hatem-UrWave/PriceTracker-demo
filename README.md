# Price Tracker - DevOps Learning Project

A complete full-stack application for tracking cryptocurrency, stock, and forex prices with real-time alerts.

## Live URLs

| Service | URL | Description |
|---------|-----|-------------|
| **Application** | https://tracker.urwave.dev | Angular frontend |
| **API** | https://tracker.urwave.dev/api | .NET REST API |
| **API Docs** | https://tracker.urwave.dev/openapi/v1.json | OpenAPI specification |
| **Hangfire** | https://tracker.urwave.dev/hangfire | Background jobs dashboard |
| **Grafana** | https://mgmt.urwave.dev/grafana | Metrics & logs visualization |
| **Prometheus** | https://mgmt.urwave.dev/prometheus | Metrics collection |
| **Portainer** | https://mgmt.urwave.dev/portainer | Container management |
| **Uptime Kuma** | https://mgmt.urwave.dev/status | Uptime monitoring |

### Infrastructure
- **Hetzner Cloud** - 2x CX22 servers (Nuremberg datacenter)
  - App Server: `46.225.56.58`
  - Management Server: `46.225.50.149`
- **Cloudflare Zero Trust** - Tunnels for secure access (no exposed ports)
- **Terraform** - Infrastructure as Code
- **Docker & Docker Compose** - Containerization
- **Nginx** - Reverse proxy

### Monitoring Stack
- **Prometheus** - Metrics collection
- **Grafana** - Metrics & logs visualization
- **Loki + Promtail** - Log aggregation
- **Portainer** - Container management UI
- **Uptime Kuma** - Uptime monitoring

### CI/CD
- **GitHub Actions** - Automated build and deployment
- **GitHub Container Registry (ghcr.io)** - Docker image storage

## Docker Registry

Images are stored in GitHub Container Registry:

```
ghcr.io/hatem-urwave/pricetracker-demo/price-tracker-api:latest
```

View packages: https://github.com/Hatem-UrWave/PriceTracker-demo/pkgs/container/pricetracker-demo%2Fprice-tracker-api

## Architecture

```
                    ┌─────────────────────────────────────────┐
                    │         Cloudflare Zero Trust           │
                    │  tracker.urwave.dev │ mgmt.urwave.dev   │
                    └──────────┬──────────────┬───────────────┘
                               │              │
                    ┌──────────▼────────┐  ┌──▼─────────────────┐
                    │    App Server     │  │   Mgmt Server      │
                    │   46.225.56.58    │  │   46.225.50.149    │
                    │                   │  │                    │
                    │  ┌─────────────┐  │  │  ┌──────────────┐  │
                    │  │    Nginx    │  │  │  │    Nginx     │  │
                    │  └──────┬──────┘  │  │  └──────┬───────┘  │
                    │         │         │  │         │          │
                    │  ┌──────▼──────┐  │  │  ┌──────▼───────┐  │
                    │  │  .NET API   │  │  │  │  Portainer   │  │
                    │  │  (Hangfire) │  │  │  │  Grafana     │  │
                    │  └──┬──────┬───┘  │  │  │  Prometheus  │  │
                    │     │      │      │  │  │  Loki        │  │
                    │  ┌──▼──┐ ┌─▼───┐  │  │  │  Uptime Kuma │  │
                    │  │ PG  │ │Redis│  │  │  │  Promtail    │  │
                    │  └─────┘ └─────┘  │  │  └──────────────┘  │
                    │                   │  │                    │
                    │  ┌─────────────┐  │  │  ┌──────────────┐  │
                    │  │ cloudflared │  │  │  │ cloudflared  │  │
                    │  └─────────────┘  │  │  └──────────────┘  │
                    └───────────────────┘  └────────────────────┘
```

## Terraform Infrastructure

All infrastructure is managed via Terraform in `infrastructure/terraform/`.

### Providers

| Provider | Version | Purpose |
|----------|---------|---------|
| `hetznercloud/hcloud` | ~> 1.45 | Hetzner Cloud resources |
| `cloudflare/cloudflare` | ~> 4.0 | Cloudflare DNS & Tunnels |

### Resources Created

#### Hetzner Cloud

| Resource | Name | Description |
|----------|------|-------------|
| **Server** | `price-tracker-app` | Application server (CX22, Ubuntu 24.04) |
| **Server** | `price-tracker-mgmt` | Management server (CX22, Ubuntu 24.04) |
| **Network** | `price-tracker-network` | Private network (10.0.0.0/16) |
| **Subnet** | - | Cloud subnet (10.0.1.0/24, eu-central) |
| **Firewall** | `price-tracker-app-firewall` | App server firewall (SSH, HTTP, HTTPS, ICMP) |
| **Firewall** | `price-tracker-mgmt-firewall` | Mgmt server firewall (SSH, HTTP, HTTPS, ICMP) |
| **SSH Key** | `price-tracker-deploy` | SSH key for server access |

#### Cloudflare

| Resource | Name | Description |
|----------|------|-------------|
| **Tunnel** | `price-tracker-app` | Tunnel for tracker.urwave.dev |
| **Tunnel** | `price-tracker-management` | Tunnel for mgmt.urwave.dev |
| **DNS Record** | `tracker` | CNAME pointing to app tunnel |
| **DNS Record** | `mgmt` | CNAME pointing to management tunnel |
| **Access App** | `Price Tracker Management` | Zero Trust access for mgmt subdomain |
| **Access Policy** | `Management Access Policy` | Email-based access control |

### Server Configuration

Both servers are provisioned with:
- Ubuntu 24.04 LTS
- Docker & Docker Compose plugin
- Application directory (`/opt/price-tracker` or `/opt/management`)
- Useful bash aliases (`dc`, `dps`, `dlogs`)

### Private Network

```
Network: 10.0.0.0/16
Subnet:  10.0.1.0/24 (eu-central)

App Server:        10.0.1.10
Management Server: 10.0.1.20
```

### Terraform Variables

| Variable | Default | Description |
|----------|---------|-------------|
| `hetzner_api_token` | - | Hetzner Cloud API token (sensitive) |
| `cloudflare_api_token` | - | Cloudflare API token (sensitive) |
| `cloudflare_account_id` | - | Cloudflare account ID |
| `cloudflare_zone_id` | - | Cloudflare zone ID |
| `domain` | `urwave.dev` | Base domain |
| `app_subdomain` | `tracker` | Application subdomain |
| `management_subdomain` | `mgmt` | Management subdomain |
| `server_location` | `nbg1` | Hetzner datacenter (Nuremberg) |
| `server_type` | `cx22` | Server type (2 vCPU, 4GB RAM) |
| `ssh_public_key` | - | SSH public key for access |
| `admin_email` | - | Email for Cloudflare Access |

### Terraform Outputs

| Output | Description |
|--------|-------------|
| `app_server_ip` | Application server public IP |
| `management_server_ip` | Management server public IP |
| `app_tunnel_token` | Cloudflare tunnel token for app (sensitive) |
| `management_tunnel_token` | Cloudflare tunnel token for mgmt (sensitive) |
| `app_url` | Application URL (https://tracker.urwave.dev) |
| `management_url` | Management URL (https://mgmt.urwave.dev) |

### Terraform Commands

```bash
cd infrastructure/terraform

# Initialize
terraform init

# Preview changes
terraform plan

# Apply changes
terraform apply

# Get outputs
terraform output

# Get sensitive outputs
terraform output -json app_tunnel_token
```

### Terraform Files

```
infrastructure/terraform/
├── main.tf                 # Provider configuration
├── variables.tf            # Input variables
├── outputs.tf              # Output values
├── locals.tf               # Local values
├── hetzner-servers.tf      # Server resources
├── hetzner-network.tf      # Network & firewall
├── hetzner-ssh.tf          # SSH key
├── cloudflare-tunnels.tf   # Cloudflare tunnels
├── cloudflare-dns.tf       # DNS records
└── cloudflare-access.tf    # Zero Trust access
```

## GitHub Secrets Required

### Infrastructure Secrets
| Secret | Description |
|--------|-------------|
| `HETZNER_API_TOKEN` | Hetzner Cloud API token |
| `CLOUDFLARE_API_TOKEN` | Cloudflare API token |
| `CLOUDFLARE_ACCOUNT_ID` | Cloudflare account ID |
| `CLOUDFLARE_ZONE_ID` | Cloudflare zone ID for urwave.dev |

### SSH Access
| Secret | Description |
|--------|-------------|
| `SSH_PRIVATE_KEY` | SSH private key for deployment |

### App Server Secrets
| Secret | Description |
|--------|-------------|
| `APP_SERVER_IP` | App server IP (46.225.56.58) |
| `DB_PASSWORD` | PostgreSQL password |
| `REDIS_PASSWORD` | Redis password |
| `APP_TUNNEL_TOKEN` | Cloudflare tunnel token for app |

### Management Server Secrets
| Secret | Description |
|--------|-------------|
| `MGMT_SERVER_IP` | Management server IP (46.225.50.149) |
| `MGMT_TUNNEL_TOKEN` | Cloudflare tunnel token for management |
| `GRAFANA_PASSWORD` | Grafana admin password |
| `PORTAINER_PASSWORD` | Portainer admin password (optional) |

## GitHub Actions Workflows

| Workflow | Trigger | Description |
|----------|---------|-------------|
| `api-build.yml` | Push to `src/api/**` | Build and push API Docker image |
| `api-deploy.yml` | Manual / After build | Deploy API to app server |
| `frontend-build.yml` | Push to `src/frontend/**` | Build Angular app |
| `frontend-deploy.yml` | Manual / After build | Deploy frontend to app server |
| `management-deploy.yml` | Manual | Deploy management stack |
| `infrastructure.yml` | Push to `infrastructure/**` | Apply Terraform changes |

## API Endpoints

### Cryptocurrency
- `GET /api/crypto` - Get all cryptocurrencies
- `GET /api/crypto/top/{count}` - Get top N cryptocurrencies
- `GET /api/crypto/{symbol}` - Get specific cryptocurrency

### Stocks
- `GET /api/stocks` - Get all stocks
- `GET /api/stocks/{symbol}` - Get specific stock

### Forex
- `GET /api/forex` - Get all forex rates
- `GET /api/forex/{base}/{target}` - Get specific forex rate

### Alerts
- `GET /api/alerts` - Get all alerts
- `POST /api/alerts` - Create new alert
- `DELETE /api/alerts/{id}` - Delete alert

### Health & Metrics
- `GET /health` - Health check
- `GET /health/ready` - Readiness check
- `GET /health/live` - Liveness check
- `GET /metrics` - Prometheus metrics

## Local Development

### Prerequisites
- Node.js 22+
- .NET 8 SDK
- Docker & Docker Compose
- PostgreSQL 16 (or use Docker)
- Redis 7 (or use Docker)

### Run with Docker Compose

```bash
cd deployment/app
docker compose up -d
```

### Run Locally

1. **Start the API**
   ```bash
   cd src/api
   dotnet restore
   dotnet run
   ```

2. **Start the Frontend**
   ```bash
   cd src/frontend
   npm install
   npm start
   ```

   - Frontend: http://localhost:4200
   - API: http://localhost:5000

## Monitoring Access

### Grafana
- URL: https://mgmt.urwave.dev/grafana
- Username: `admin`
- Password: Set via `GRAFANA_PASSWORD` secret

**Pre-configured Data Sources:**
- Prometheus (metrics)
- Loki (logs)

**Useful Dashboard IDs to Import:**
- `1860` - Node Exporter Full
- `3662` - Prometheus Stats
- `13639` - Loki Logs

### Portainer
- URL: https://mgmt.urwave.dev/portainer
- Create admin account on first access

### Prometheus
- URL: https://mgmt.urwave.dev/prometheus
- Direct access to PromQL queries

### Uptime Kuma
- URL: https://mgmt.urwave.dev/status
- Create admin account on first access

## Server Access

```bash
# App Server
ssh -i ~/.ssh/price-tracker-deploy root@46.225.56.58

# Management Server
ssh -i ~/.ssh/price-tracker-deploy root@46.225.50.149
```

### Useful Commands

```bash
# View running containers
docker ps

# View logs
docker logs price-tracker-api --tail 100 -f

# Restart a service
cd /opt/price-tracker && docker compose restart api

# View database
docker exec -it price-tracker-db psql -U postgres -d pricetracker
```

## Cost Breakdown

| Resource | Monthly Cost |
|----------|--------------|
| Hetzner CX22 x 2 | ~€9.00 |
| Cloudflare (Free) | €0 |
| GitHub Actions (Free) | €0 |
| **Total** | **~€9.00/month** |

## Security

- **No exposed ports** - All traffic goes through Cloudflare Tunnels
- **Cloudflare Zero Trust** - Secure access without VPN
- **Environment variables** - Secrets managed via GitHub Secrets
- **Docker networks** - Isolated container networking
- **HTTPS everywhere** - SSL termination at Cloudflare

## Project Structure

```
PriceTracker-demo/
├── .github/
│   └── workflows/           # GitHub Actions CI/CD
├── deployment/
│   ├── app/                 # App server Docker Compose
│   └── management/          # Management server Docker Compose
├── infrastructure/
│   └── terraform/           # Terraform IaC
├── src/
│   ├── api/                 # .NET 8 API
│   └── frontend/            # Angular 19 SPA
└── README.md
```

## Troubleshooting

### API Returns 500 Error
Check database tables exist:
```bash
docker exec price-tracker-db psql -U postgres -d pricetracker -c '\dt'
```

### Container Won't Start
Check logs:
```bash
docker logs <container-name> --tail 50
```

### Cloudflare Tunnel Issues
Verify tunnel is running:
```bash
docker logs cloudflared-app --tail 20
```

## License

MIT License - See LICENSE file for details

---

**Built for learning DevOps practices including:**
- Infrastructure as Code (Terraform)
- CI/CD pipelines (GitHub Actions)
- Container orchestration (Docker Compose)
- Monitoring and observability (Prometheus, Grafana, Loki)
- Secure cloud deployments (Cloudflare Zero Trust)
