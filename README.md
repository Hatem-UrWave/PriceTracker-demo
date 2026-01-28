# Price Tracker - DevOps Learning Project

A complete full-stack application for tracking cryptocurrency, stock, and forex prices with real-time alerts.

## 🚀 Tech Stack

### Frontend
- **Angular 21** - Modern TypeScript framework with standalone components
- **Tailwind CSS 3.4** - Utility-first CSS framework
- **ng2-charts** - Beautiful charts powered by Chart.js

### Backend
- **.NET 10** - Latest .NET with minimal APIs
- **PostgreSQL 16** - Relational database
- **Redis 7** - Caching and session management
- **Hangfire** - Background job processing

### Infrastructure
- **Hetzner Cloud** - 2x CX23 servers (Nuremberg datacenter)
- **Cloudflare Zero Trust** - Tunnels, DNS, and access control
- **Terraform** - Infrastructure as Code
- **Docker & Docker Compose** - Containerization
- **Nginx** - Reverse proxy and static file serving

### Monitoring
- **Prometheus** - Metrics collection
- **Grafana** - Metrics visualization
- **Portainer** - Container management
- **Uptime Kuma** - Uptime monitoring
- **Loki + Promtail** - Log aggregation

## 📋 Features

- Real-time price tracking for cryptocurrencies, stocks, and forex
- Price alerts with webhook notifications
- RESTful API with Swagger documentation
- Hangfire dashboard for job monitoring
- Responsive Angular SPA
- Redis caching for improved performance
- Prometheus metrics endpoint
- Health check endpoints

## 🏗️ Architecture

```
┌─────────────────────────────────────────┐
│       Cloudflare Zero Trust            │
│   tracker.urwave.dev | mgmt.urwave.dev  │
└──────────┬──────────────┬───────────────┘
           │              │
    ┌──────▼────────┐  ┌──▼─────────────┐
    │  App Server   │  │  Mgmt Server   │
    │  (CX23)       │  │  (CX23)        │
    │               │  │                │
    │  ┌─────────┐  │  │  ┌──────────┐ │
    │  │  Nginx  │  │  │  │ Portainer│ │
    │  └────┬────┘  │  │  │ Grafana  │ │
    │       │       │  │  │Prometheus│ │
    │  ┌────▼────┐  │  │  │  Loki    │ │
    │  │ .NET API│  │  │  └──────────┘ │
    │  └─┬───┬───┘  │  └────────────────┘
    │    │   │      │
    │  ┌─▼┐ ┌▼───┐  │
    │  │PG│ │Redis│ │
    │  └──┘ └────┘  │
    └────────────────┘
```

## 🚦 Getting Started

### Prerequisites

- Node.js 22+
- .NET 10 SDK
- Docker & Docker Compose
- Terraform
- GitHub account
- Hetzner Cloud account
- Cloudflare account

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/Hatem-UrWave/PriceTracker-demo.git
   cd PriceTracker-demo
   ```

2. **Start the API**
   ```bash
   cd src/api
   dotnet restore
   dotnet run
   ```

3. **Start the Frontend**
   ```bash
   cd src/frontend
   npm install
   npm start
   ```

   The frontend will be available at `http://localhost:4200`
   The API will be available at `http://localhost:5000`

### Production Deployment

1. **Configure Terraform**
   ```bash
   cd infrastructure/terraform
   cp terraform.tfvars.example terraform.tfvars
   # Edit terraform.tfvars with your values
   ```

2. **Deploy Infrastructure**
   ```bash
   terraform init
   terraform plan
   terraform apply
   ```

3. **Configure GitHub Secrets**
   Add the following secrets to your GitHub repository:
   - `HETZNER_API_TOKEN`
   - `CLOUDFLARE_API_TOKEN`
   - `CLOUDFLARE_ACCOUNT_ID`
   - `CLOUDFLARE_ZONE_ID`
   - `SSH_PRIVATE_KEY`
   - `SSH_PUBLIC_KEY`
   - `APP_SERVER_IP`
   - `DB_PASSWORD`
   - `REDIS_PASSWORD`
   - `APP_TUNNEL_TOKEN`
   - `ADMIN_EMAIL`

4. **Deploy Application**
   Push to main branch to trigger GitHub Actions workflows:
   - API build and deployment
   - Frontend build and deployment

## 📊 Monitoring

- **Application**: https://tracker.urwave.dev
- **API Docs**: https://tracker.urwave.dev/swagger
- **Hangfire**: https://tracker.urwave.dev/hangfire
- **Management**: https://mgmt.urwave.dev

## 💰 Cost Breakdown

| Resource | Monthly Cost |
|----------|--------------|
| Hetzner CX23 × 2 | ~€8.70 |
| Cloudflare (Free) | €0 |
| GitHub Actions (Free) | €0 |
| **Total** | **~€8.70/month** |

## 📝 API Endpoints

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

## 🔒 Security

- Cloudflare Zero Trust for secure access
- Environment variables for sensitive data
- Docker secrets management
- Firewall rules on Hetzner servers
- HTTPS via Cloudflare

## 📚 Documentation

- [Setup Guide](docs/SETUP.md)
- [Architecture](docs/ARCHITECTURE.md)
- [Runbook](docs/RUNBOOK.md)

## 🤝 Contributing

This is a learning project. Feel free to fork and experiment!

## 📄 License

MIT License - See LICENSE file for details

## 🙏 Acknowledgments

Built for learning DevOps practices including:
- Infrastructure as Code
- CI/CD pipelines
- Container orchestration
- Monitoring and observability
- Cloud deployments

---

**Project**: Price Tracker
**Hetzner Project**: Price Tracker
**Server Type**: CX23 (2 vCPU, 4GB RAM, 40GB SSD)
**Location**: Nuremberg (nbg1)