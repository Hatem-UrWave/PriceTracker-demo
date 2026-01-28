# Price Tracker Infrastructure & API - Complete Implementation Plan v2

## Project Overview

Build a complete DevOps learning project with:
- **Angular 18 Frontend** - Modern responsive UI for price tracking
- **.NET 8 Price Tracker API** - Tracks crypto, stocks, and forex prices
- **Nginx Reverse Proxy** - Serves static files and proxies API requests
- **Hetzner Infrastructure** - 2 CX22 servers provisioned via Terraform
- **Cloudflare Zero Trust** - Tunnels, DNS, Access policies
- **Monitoring Stack** - Portainer, Grafana, Prometheus, Uptime Kuma
- **CI/CD Pipeline** - GitHub Actions for infrastructure and application deployment

### Target URLs
- Frontend + API: `tracker.urwave.dev`
- Management: `mgmt.urwave.dev`

### URL Routing (Nginx)
```
tracker.urwave.dev/           → Angular SPA (static files)
tracker.urwave.dev/api/*      → .NET API (proxy)
tracker.urwave.dev/swagger    → API Documentation (proxy)
tracker.urwave.dev/hangfire   → Hangfire Dashboard (proxy)
```

### Infrastructure
- Location: Nuremberg (nbg1)
- App Server: CX22 (2 vCPU, 4GB RAM, 40GB SSD) ~€4.35/month
- Management Server: CX22 (2 vCPU, 4GB RAM, 40GB SSD) ~€4.35/month

---

## Updated Architecture

```
┌─────────────────────────────────────────────────────────────────────────┐
│                        Cloudflare Zero Trust                            │
│           ┌──────────────────────┬──────────────────┐                   │
│           │  tracker.urwave.dev  │ mgmt.urwave.dev  │                   │
│           └──────────┬───────────┴────────┬─────────┘                   │
│                      │ Tunnel             │ Tunnel                      │
└──────────────────────┼────────────────────┼─────────────────────────────┘
                       ▼                    ▼
┌──────────────────────────────────┐  ┌──────────────────────────────────┐
│      App Server (CX22)           │  │   Management Server (CX22)       │
│      ~€4.35/month                │  │   ~€4.35/month                   │
├──────────────────────────────────┤  ├──────────────────────────────────┤
│                                  │  │                                  │
│  ┌────────────────────────────┐  │  │  ┌────────────┐ ┌─────────────┐  │
│  │      Nginx :80             │  │  │  │ Portainer  │ │ Prometheus  │  │
│  │  ┌───────────┬──────────┐  │  │  │  │ :9000      │ │ :9090       │  │
│  │  │  /        │  /api/*  │  │  │  │  └────────────┘ └─────────────┘  │
│  │  │  Static   │  Proxy   │  │  │  │  ┌────────────┐ ┌─────────────┐  │
│  │  │  Angular  │  to API  │  │  │  │  │ Grafana    │ │ Uptime Kuma │  │
│  │  └───────────┴──────────┘  │  │  │  │ :3000      │ │ :3001       │  │
│  └────────────────────────────┘  │  │  └────────────┘ └─────────────┘  │
│               │                  │  │  ┌─────────────────────────────┐  │
│               ▼                  │  │  │ Loki + Promtail             │  │
│  ┌────────────────────────────┐  │  │  └─────────────────────────────┘  │
│  │    .NET API :8080          │  │  │                                  │
│  └────────────────────────────┘  │  │  ┌─────────────────────────────┐  │
│               │                  │  │  │ cloudflared tunnel          │  │
│       ┌───────┴───────┐         │  │  └─────────────────────────────┘  │
│       ▼               ▼         │  └──────────────────────────────────┘
│  ┌─────────┐    ┌─────────┐     │
│  │PostgreSQL│    │  Redis  │     │
│  │ :5432   │    │ :6379   │     │
│  └─────────┘    └─────────┘     │
│                                  │
│  ┌────────────────────────────┐  │
│  │   cloudflared tunnel       │  │
│  └────────────────────────────┘  │
└──────────────────────────────────┘
```

---

## Updated Project Structure

```
price-tracker/
├── .github/
│   └── workflows/
│       ├── infra-plan.yml           # Terraform plan on PR
│       ├── infra-apply.yml          # Terraform apply on merge to main
│       ├── api-build.yml            # Build and push API Docker image
│       ├── api-deploy.yml           # Deploy API to app server
│       ├── frontend-build.yml       # Build Angular and push to server
│       ├── frontend-deploy.yml      # Deploy frontend to app server
│       └── management-deploy.yml    # Deploy management stack
├── infrastructure/
│   └── terraform/
│       ├── main.tf                  # Provider configuration
│       ├── variables.tf             # Input variables
│       ├── outputs.tf               # Output values
│       ├── hetzner-network.tf       # VPC and firewall
│       ├── hetzner-servers.tf       # Server provisioning
│       ├── hetzner-ssh.tf           # SSH key management
│       ├── cloudflare-dns.tf        # DNS records
│       ├── cloudflare-tunnels.tf    # Zero Trust tunnels
│       ├── cloudflare-access.tf     # Access policies
│       └── terraform.tfvars.example # Example variables file
├── src/
│   ├── frontend/                    # Angular 18 Application
│   │   ├── angular.json
│   │   ├── package.json
│   │   ├── tsconfig.json
│   │   ├── tailwind.config.js
│   │   ├── Dockerfile
│   │   └── src/
│   │       ├── index.html
│   │       ├── main.ts
│   │       ├── styles.scss
│   │       └── app/
│   │           ├── app.component.ts
│   │           ├── app.config.ts
│   │           ├── app.routes.ts
│   │           ├── core/
│   │           │   ├── services/
│   │           │   │   ├── api.service.ts
│   │           │   │   ├── crypto.service.ts
│   │           │   │   ├── stocks.service.ts
│   │           │   │   ├── forex.service.ts
│   │           │   │   └── alerts.service.ts
│   │           │   ├── models/
│   │           │   │   ├── crypto-price.model.ts
│   │           │   │   ├── stock-price.model.ts
│   │           │   │   ├── forex-rate.model.ts
│   │           │   │   └── alert.model.ts
│   │           │   └── interceptors/
│   │           │       └── error.interceptor.ts
│   │           ├── shared/
│   │           │   ├── components/
│   │           │   │   ├── navbar/
│   │           │   │   ├── footer/
│   │           │   │   ├── price-card/
│   │           │   │   ├── price-chart/
│   │           │   │   ├── loading-spinner/
│   │           │   │   └── alert-badge/
│   │           │   └── pipes/
│   │           │       ├── currency.pipe.ts
│   │           │       └── percent-change.pipe.ts
│   │           └── features/
│   │               ├── dashboard/
│   │               │   ├── dashboard.component.ts
│   │               │   └── dashboard.component.html
│   │               ├── crypto/
│   │               │   ├── crypto-list/
│   │               │   └── crypto-detail/
│   │               ├── stocks/
│   │               │   ├── stocks-list/
│   │               │   └── stock-detail/
│   │               ├── forex/
│   │               │   └── forex-list/
│   │               └── alerts/
│   │                   ├── alerts-list/
│   │                   └── alert-create/
│   └── api/                         # .NET 8 API (renamed from PriceTracker.Api)
│       ├── PriceTracker.Api.csproj
│       ├── Program.cs
│       ├── appsettings.json
│       ├── appsettings.Production.json
│       ├── Dockerfile
│       ├── Endpoints/
│       ├── Services/
│       ├── Jobs/
│       ├── Data/
│       ├── Models/
│       └── Clients/
├── deployment/
│   ├── app/
│   │   ├── docker-compose.yml       # Nginx + API + PostgreSQL + Redis
│   │   ├── .env.example
│   │   ├── nginx/
│   │   │   ├── nginx.conf
│   │   │   └── conf.d/
│   │   │       └── default.conf
│   │   └── cloudflared/
│   │       └── config.yml
│   └── management/
│       ├── docker-compose.yml
│       ├── .env.example
│       ├── prometheus/
│       │   └── prometheus.yml
│       ├── grafana/
│       │   └── provisioning/
│       └── loki/
│           └── loki-config.yml
├── scripts/
│   ├── setup-server.sh
│   ├── deploy-app.sh
│   └── deploy-management.sh
├── docs/
│   ├── SETUP.md
│   ├── ARCHITECTURE.md
│   └── RUNBOOK.md
├── .gitignore
├── .dockerignore
└── README.md
```

---

## Phase 1-2: Prerequisites & Terraform

[Same as v1 - See previous implementation plan]

---

## Phase 3: Angular Frontend

### 3.1 package.json

```json
{
  "name": "price-tracker-frontend",
  "version": "1.0.0",
  "scripts": {
    "ng": "ng",
    "start": "ng serve",
    "build": "ng build",
    "build:prod": "ng build --configuration=production",
    "watch": "ng build --watch --configuration development",
    "test": "ng test",
    "lint": "ng lint"
  },
  "private": true,
  "dependencies": {
    "@angular/animations": "^18.0.0",
    "@angular/common": "^18.0.0",
    "@angular/compiler": "^18.0.0",
    "@angular/core": "^18.0.0",
    "@angular/forms": "^18.0.0",
    "@angular/platform-browser": "^18.0.0",
    "@angular/platform-browser-dynamic": "^18.0.0",
    "@angular/router": "^18.0.0",
    "chart.js": "^4.4.0",
    "ng2-charts": "^6.0.0",
    "rxjs": "~7.8.0",
    "tslib": "^2.6.0",
    "zone.js": "~0.14.0"
  },
  "devDependencies": {
    "@angular-devkit/build-angular": "^18.0.0",
    "@angular/cli": "^18.0.0",
    "@angular/compiler-cli": "^18.0.0",
    "@types/jasmine": "~5.1.0",
    "autoprefixer": "^10.4.0",
    "jasmine-core": "~5.1.0",
    "karma": "~6.4.0",
    "karma-chrome-launcher": "~3.2.0",
    "karma-coverage": "~2.2.0",
    "karma-jasmine": "~5.1.0",
    "karma-jasmine-html-reporter": "~2.1.0",
    "postcss": "^8.4.0",
    "tailwindcss": "^3.4.0",
    "typescript": "~5.4.0"
  }
}
```

### 3.2 angular.json

```json
{
  "$schema": "./node_modules/@angular/cli/lib/config/schema.json",
  "version": 1,
  "newProjectRoot": "projects",
  "projects": {
    "price-tracker": {
      "projectType": "application",
      "schematics": {
        "@schematics/angular:component": {
          "style": "scss",
          "standalone": true
        }
      },
      "root": "",
      "sourceRoot": "src",
      "prefix": "app",
      "architect": {
        "build": {
          "builder": "@angular-devkit/build-angular:application",
          "options": {
            "outputPath": "dist/price-tracker",
            "index": "src/index.html",
            "browser": "src/main.ts",
            "polyfills": ["zone.js"],
            "tsConfig": "tsconfig.app.json",
            "inlineStyleLanguage": "scss",
            "assets": [
              "src/favicon.ico",
              "src/assets"
            ],
            "styles": [
              "src/styles.scss"
            ],
            "scripts": []
          },
          "configurations": {
            "production": {
              "budgets": [
                {
                  "type": "initial",
                  "maximumWarning": "500kb",
                  "maximumError": "1mb"
                },
                {
                  "type": "anyComponentStyle",
                  "maximumWarning": "2kb",
                  "maximumError": "4kb"
                }
              ],
              "outputHashing": "all",
              "optimization": true
            },
            "development": {
              "optimization": false,
              "extractLicenses": false,
              "sourceMap": true
            }
          },
          "defaultConfiguration": "production"
        },
        "serve": {
          "builder": "@angular-devkit/build-angular:dev-server",
          "configurations": {
            "production": {
              "buildTarget": "price-tracker:build:production"
            },
            "development": {
              "buildTarget": "price-tracker:build:development"
            }
          },
          "defaultConfiguration": "development",
          "options": {
            "proxyConfig": "proxy.conf.json"
          }
        },
        "test": {
          "builder": "@angular-devkit/build-angular:karma",
          "options": {
            "polyfills": ["zone.js", "zone.js/testing"],
            "tsConfig": "tsconfig.spec.json",
            "inlineStyleLanguage": "scss",
            "assets": [
              "src/favicon.ico",
              "src/assets"
            ],
            "styles": [
              "src/styles.scss"
            ],
            "scripts": []
          }
        }
      }
    }
  }
}
```

### 3.3 tailwind.config.js

```javascript
/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#eff6ff',
          100: '#dbeafe',
          200: '#bfdbfe',
          300: '#93c5fd',
          400: '#60a5fa',
          500: '#3b82f6',
          600: '#2563eb',
          700: '#1d4ed8',
          800: '#1e40af',
          900: '#1e3a8a',
        },
        success: '#10b981',
        danger: '#ef4444',
        warning: '#f59e0b',
      }
    },
  },
  plugins: [],
}
```

### 3.4 proxy.conf.json (for local development)

```json
{
  "/api": {
    "target": "http://localhost:5000",
    "secure": false,
    "changeOrigin": true
  }
}
```

### 3.5 src/index.html

```html
<!doctype html>
<html lang="en" class="h-full">
<head>
  <meta charset="utf-8">
  <title>Price Tracker - Crypto, Stocks & Forex</title>
  <base href="/">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <meta name="description" content="Track cryptocurrency, stock, and forex prices in real-time with alerts">
  <link rel="icon" type="image/x-icon" href="favicon.ico">
  <link rel="preconnect" href="https://fonts.googleapis.com">
  <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
  <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap" rel="stylesheet">
</head>
<body class="h-full bg-gray-50">
  <app-root></app-root>
</body>
</html>
```

### 3.6 src/styles.scss

```scss
@tailwind base;
@tailwind components;
@tailwind utilities;

:root {
  --font-family: 'Inter', system-ui, sans-serif;
}

body {
  font-family: var(--font-family);
  @apply antialiased;
}

// Custom utility classes
@layer components {
  .btn {
    @apply inline-flex items-center justify-center px-4 py-2 border border-transparent 
           text-sm font-medium rounded-md shadow-sm focus:outline-none focus:ring-2 
           focus:ring-offset-2 transition-colors duration-200;
  }
  
  .btn-primary {
    @apply btn bg-primary-600 text-white hover:bg-primary-700 focus:ring-primary-500;
  }
  
  .btn-secondary {
    @apply btn bg-white text-gray-700 border-gray-300 hover:bg-gray-50 focus:ring-primary-500;
  }
  
  .btn-danger {
    @apply btn bg-red-600 text-white hover:bg-red-700 focus:ring-red-500;
  }
  
  .card {
    @apply bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden;
  }
  
  .input {
    @apply block w-full rounded-md border-gray-300 shadow-sm 
           focus:border-primary-500 focus:ring-primary-500 sm:text-sm;
  }
  
  .price-up {
    @apply text-green-600;
  }
  
  .price-down {
    @apply text-red-600;
  }
}
```

### 3.7 src/main.ts

```typescript
import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';

bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));
```

### 3.8 src/app/app.config.ts

```typescript
import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';

import { routes } from './app.routes';
import { errorInterceptor } from './core/interceptors/error.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withInterceptors([errorInterceptor])),
    provideAnimations(),
    provideCharts(withDefaultRegisterables()),
  ]
};
```

### 3.9 src/app/app.routes.ts

```typescript
import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/dashboard/dashboard.component')
      .then(m => m.DashboardComponent),
    title: 'Dashboard - Price Tracker'
  },
  {
    path: 'crypto',
    children: [
      {
        path: '',
        loadComponent: () => import('./features/crypto/crypto-list/crypto-list.component')
          .then(m => m.CryptoListComponent),
        title: 'Cryptocurrencies - Price Tracker'
      },
      {
        path: ':symbol',
        loadComponent: () => import('./features/crypto/crypto-detail/crypto-detail.component')
          .then(m => m.CryptoDetailComponent),
        title: 'Crypto Details - Price Tracker'
      }
    ]
  },
  {
    path: 'stocks',
    children: [
      {
        path: '',
        loadComponent: () => import('./features/stocks/stocks-list/stocks-list.component')
          .then(m => m.StocksListComponent),
        title: 'Stocks - Price Tracker'
      },
      {
        path: ':symbol',
        loadComponent: () => import('./features/stocks/stock-detail/stock-detail.component')
          .then(m => m.StockDetailComponent),
        title: 'Stock Details - Price Tracker'
      }
    ]
  },
  {
    path: 'forex',
    loadComponent: () => import('./features/forex/forex-list/forex-list.component')
      .then(m => m.ForexListComponent),
    title: 'Forex Rates - Price Tracker'
  },
  {
    path: 'alerts',
    children: [
      {
        path: '',
        loadComponent: () => import('./features/alerts/alerts-list/alerts-list.component')
          .then(m => m.AlertsListComponent),
        title: 'Price Alerts - Price Tracker'
      },
      {
        path: 'create',
        loadComponent: () => import('./features/alerts/alert-create/alert-create.component')
          .then(m => m.AlertCreateComponent),
        title: 'Create Alert - Price Tracker'
      }
    ]
  },
  {
    path: '**',
    redirectTo: ''
  }
];
```

### 3.10 src/app/app.component.ts

```typescript
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './shared/components/navbar/navbar.component';
import { FooterComponent } from './shared/components/footer/footer.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent, FooterComponent],
  template: `
    <div class="min-h-full flex flex-col">
      <app-navbar />
      <main class="flex-1">
        <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
          <router-outlet />
        </div>
      </main>
      <app-footer />
    </div>
  `
})
export class AppComponent {}
```

### 3.11 src/app/core/models/crypto-price.model.ts

```typescript
export interface CryptoPrice {
  id: number;
  symbol: string;
  name: string;
  priceUsd: number;
  priceEur: number;
  marketCapUsd: number;
  volume24hUsd: number;
  changePercent24h: number;
  lastUpdated: Date;
}
```

### 3.12 src/app/core/models/stock-price.model.ts

```typescript
export interface StockPrice {
  id: number;
  symbol: string;
  name: string;
  exchange: string;
  price: number;
  dayHigh: number;
  dayLow: number;
  open: number;
  previousClose: number;
  changePercent: number;
  volume: number;
  lastUpdated: Date;
}
```

### 3.13 src/app/core/models/forex-rate.model.ts

```typescript
export interface ForexRate {
  id: number;
  baseCurrency: string;
  targetCurrency: string;
  rate: number;
  lastUpdated: Date;
}
```

### 3.14 src/app/core/models/alert.model.ts

```typescript
export interface Alert {
  id: number;
  assetType: 'crypto' | 'stock' | 'forex';
  symbol: string;
  condition: 'above' | 'below';
  targetPrice: number;
  isActive: boolean;
  isTriggered: boolean;
  triggeredAt?: Date;
  createdAt: Date;
}

export interface CreateAlertRequest {
  assetType: string;
  symbol: string;
  condition: string;
  targetPrice: number;
  webhookUrl?: string;
  email?: string;
}
```

### 3.15 src/app/core/services/api.service.ts

```typescript
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api';

  get<T>(endpoint: string): Observable<T> {
    return this.http.get<T>(`${this.baseUrl}${endpoint}`);
  }

  post<T>(endpoint: string, data: any): Observable<T> {
    return this.http.post<T>(`${this.baseUrl}${endpoint}`, data);
  }

  put<T>(endpoint: string, data: any): Observable<T> {
    return this.http.put<T>(`${this.baseUrl}${endpoint}`, data);
  }

  delete<T>(endpoint: string): Observable<T> {
    return this.http.delete<T>(`${this.baseUrl}${endpoint}`);
  }
}
```

### 3.16 src/app/core/services/crypto.service.ts

```typescript
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { CryptoPrice } from '../models/crypto-price.model';

@Injectable({
  providedIn: 'root'
})
export class CryptoService {
  private readonly api = inject(ApiService);

  getAll(): Observable<CryptoPrice[]> {
    return this.api.get<CryptoPrice[]>('/crypto');
  }

  getBySymbol(symbol: string): Observable<CryptoPrice> {
    return this.api.get<CryptoPrice>(`/crypto/${symbol}`);
  }

  getTop(count: number = 10): Observable<CryptoPrice[]> {
    return this.api.get<CryptoPrice[]>(`/crypto/top/${count}`);
  }
}
```

### 3.17 src/app/core/services/stocks.service.ts

```typescript
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { StockPrice } from '../models/stock-price.model';

@Injectable({
  providedIn: 'root'
})
export class StocksService {
  private readonly api = inject(ApiService);

  getAll(): Observable<StockPrice[]> {
    return this.api.get<StockPrice[]>('/stocks');
  }

  getBySymbol(symbol: string): Observable<StockPrice> {
    return this.api.get<StockPrice>(`/stocks/${symbol}`);
  }
}
```

### 3.18 src/app/core/services/forex.service.ts

```typescript
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { ForexRate } from '../models/forex-rate.model';

@Injectable({
  providedIn: 'root'
})
export class ForexService {
  private readonly api = inject(ApiService);

  getAll(): Observable<ForexRate[]> {
    return this.api.get<ForexRate[]>('/forex');
  }

  getRate(base: string, target: string): Observable<ForexRate> {
    return this.api.get<ForexRate>(`/forex/${base}/${target}`);
  }
}
```

### 3.19 src/app/core/services/alerts.service.ts

```typescript
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Alert, CreateAlertRequest } from '../models/alert.model';

@Injectable({
  providedIn: 'root'
})
export class AlertsService {
  private readonly api = inject(ApiService);

  getAll(): Observable<Alert[]> {
    return this.api.get<Alert[]>('/alerts');
  }

  getById(id: number): Observable<Alert> {
    return this.api.get<Alert>(`/alerts/${id}`);
  }

  create(request: CreateAlertRequest): Observable<Alert> {
    return this.api.post<Alert>('/alerts', request);
  }

  delete(id: number): Observable<void> {
    return this.api.delete<void>(`/alerts/${id}`);
  }
}
```

### 3.20 src/app/core/interceptors/error.interceptor.ts

```typescript
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'An error occurred';
      
      if (error.error instanceof ErrorEvent) {
        // Client-side error
        errorMessage = error.error.message;
      } else {
        // Server-side error
        switch (error.status) {
          case 0:
            errorMessage = 'Unable to connect to server';
            break;
          case 404:
            errorMessage = 'Resource not found';
            break;
          case 500:
            errorMessage = 'Internal server error';
            break;
          default:
            errorMessage = error.error?.message || `Error: ${error.status}`;
        }
      }
      
      console.error('API Error:', errorMessage);
      return throwError(() => new Error(errorMessage));
    })
  );
};
```

### 3.21 src/app/shared/components/navbar/navbar.component.ts

```typescript
import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  template: `
    <nav class="bg-white shadow-sm border-b border-gray-200">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16">
          <div class="flex">
            <!-- Logo -->
            <div class="flex-shrink-0 flex items-center">
              <a routerLink="/" class="text-xl font-bold text-primary-600">
                📈 PriceTracker
              </a>
            </div>
            
            <!-- Navigation Links -->
            <div class="hidden sm:ml-8 sm:flex sm:space-x-4">
              <a routerLink="/" routerLinkActive="border-primary-500 text-gray-900"
                 [routerLinkActiveOptions]="{exact: true}"
                 class="border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700 
                        inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium">
                Dashboard
              </a>
              <a routerLink="/crypto" routerLinkActive="border-primary-500 text-gray-900"
                 class="border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700 
                        inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium">
                Crypto
              </a>
              <a routerLink="/stocks" routerLinkActive="border-primary-500 text-gray-900"
                 class="border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700 
                        inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium">
                Stocks
              </a>
              <a routerLink="/forex" routerLinkActive="border-primary-500 text-gray-900"
                 class="border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700 
                        inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium">
                Forex
              </a>
              <a routerLink="/alerts" routerLinkActive="border-primary-500 text-gray-900"
                 class="border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700 
                        inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium">
                Alerts
              </a>
            </div>
          </div>
          
          <!-- Status indicator -->
          <div class="flex items-center">
            <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
              <span class="w-2 h-2 mr-1 bg-green-400 rounded-full animate-pulse"></span>
              Live
            </span>
          </div>
        </div>
      </div>
      
      <!-- Mobile menu -->
      <div class="sm:hidden border-t border-gray-200">
        <div class="pt-2 pb-3 space-y-1">
          <a routerLink="/" routerLinkActive="bg-primary-50 border-primary-500 text-primary-700"
             [routerLinkActiveOptions]="{exact: true}"
             class="border-transparent text-gray-600 hover:bg-gray-50 hover:border-gray-300 
                    block pl-3 pr-4 py-2 border-l-4 text-base font-medium">
            Dashboard
          </a>
          <a routerLink="/crypto" routerLinkActive="bg-primary-50 border-primary-500 text-primary-700"
             class="border-transparent text-gray-600 hover:bg-gray-50 hover:border-gray-300 
                    block pl-3 pr-4 py-2 border-l-4 text-base font-medium">
            Crypto
          </a>
          <a routerLink="/stocks" routerLinkActive="bg-primary-50 border-primary-500 text-primary-700"
             class="border-transparent text-gray-600 hover:bg-gray-50 hover:border-gray-300 
                    block pl-3 pr-4 py-2 border-l-4 text-base font-medium">
            Stocks
          </a>
          <a routerLink="/forex" routerLinkActive="bg-primary-50 border-primary-500 text-primary-700"
             class="border-transparent text-gray-600 hover:bg-gray-50 hover:border-gray-300 
                    block pl-3 pr-4 py-2 border-l-4 text-base font-medium">
            Forex
          </a>
          <a routerLink="/alerts" routerLinkActive="bg-primary-50 border-primary-500 text-primary-700"
             class="border-transparent text-gray-600 hover:bg-gray-50 hover:border-gray-300 
                    block pl-3 pr-4 py-2 border-l-4 text-base font-medium">
            Alerts
          </a>
        </div>
      </div>
    </nav>
  `
})
export class NavbarComponent {}
```

### 3.22 src/app/shared/components/footer/footer.component.ts

```typescript
import { Component } from '@angular/core';

@Component({
  selector: 'app-footer',
  standalone: true,
  template: `
    <footer class="bg-white border-t border-gray-200">
      <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
        <div class="flex flex-col md:flex-row justify-between items-center">
          <div class="text-gray-500 text-sm">
            © 2024 PriceTracker. Built for learning DevOps.
          </div>
          <div class="flex space-x-6 mt-4 md:mt-0">
            <a href="/api/status" target="_blank" 
               class="text-gray-400 hover:text-gray-500 text-sm">
              API Status
            </a>
            <a href="/swagger" target="_blank" 
               class="text-gray-400 hover:text-gray-500 text-sm">
              API Docs
            </a>
          </div>
        </div>
      </div>
    </footer>
  `
})
export class FooterComponent {}
```

### 3.23 src/app/shared/components/price-card/price-card.component.ts

```typescript
import { Component, Input } from '@angular/core';
import { CommonModule, DecimalPipe, PercentPipe } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-price-card',
  standalone: true,
  imports: [CommonModule, RouterLink, DecimalPipe, PercentPipe],
  template: `
    <a [routerLink]="link" class="card block hover:shadow-md transition-shadow duration-200">
      <div class="p-5">
        <div class="flex items-center justify-between">
          <div class="flex items-center">
            <div class="flex-shrink-0 w-10 h-10 rounded-full bg-gray-100 flex items-center justify-center">
              <span class="text-lg font-bold text-gray-600">{{ symbol.charAt(0) }}</span>
            </div>
            <div class="ml-3">
              <h3 class="text-sm font-medium text-gray-900">{{ name }}</h3>
              <p class="text-xs text-gray-500">{{ symbol }}</p>
            </div>
          </div>
          <div class="text-right">
            <p class="text-lg font-semibold text-gray-900">
              {{ currencySymbol }}{{ price | number:'1.2-2' }}
            </p>
            <p [class]="changePercent >= 0 ? 'price-up' : 'price-down'" 
               class="text-sm font-medium flex items-center justify-end">
              <span>{{ changePercent >= 0 ? '↑' : '↓' }}</span>
              <span class="ml-1">{{ changePercent | percent:'1.2-2' }}</span>
            </p>
          </div>
        </div>
      </div>
    </a>
  `
})
export class PriceCardComponent {
  @Input({ required: true }) symbol!: string;
  @Input({ required: true }) name!: string;
  @Input({ required: true }) price!: number;
  @Input({ required: true }) changePercent!: number;
  @Input() currencySymbol: string = '$';
  @Input() link: string = '';
}
```

### 3.24 src/app/shared/components/loading-spinner/loading-spinner.component.ts

```typescript
import { Component } from '@angular/core';

@Component({
  selector: 'app-loading-spinner',
  standalone: true,
  template: `
    <div class="flex justify-center items-center py-12">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
    </div>
  `
})
export class LoadingSpinnerComponent {}
```

### 3.25 src/app/features/dashboard/dashboard.component.ts

```typescript
import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CryptoService } from '../../core/services/crypto.service';
import { ForexService } from '../../core/services/forex.service';
import { CryptoPrice } from '../../core/models/crypto-price.model';
import { ForexRate } from '../../core/models/forex-rate.model';
import { PriceCardComponent } from '../../shared/components/price-card/price-card.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, PriceCardComponent, LoadingSpinnerComponent],
  template: `
    <div class="space-y-8">
      <!-- Header -->
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Dashboard</h1>
        <p class="mt-1 text-sm text-gray-500">Real-time price tracking for crypto, stocks, and forex</p>
      </div>
      
      <!-- Crypto Section -->
      <section>
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-lg font-semibold text-gray-900">Top Cryptocurrencies</h2>
          <a routerLink="/crypto" class="text-sm text-primary-600 hover:text-primary-700">
            View all →
          </a>
        </div>
        
        @if (loadingCrypto()) {
          <app-loading-spinner />
        } @else if (cryptoError()) {
          <div class="text-center py-8 text-red-600">
            {{ cryptoError() }}
          </div>
        } @else {
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            @for (crypto of topCrypto(); track crypto.symbol) {
              <app-price-card
                [symbol]="crypto.symbol"
                [name]="crypto.name"
                [price]="crypto.priceUsd"
                [changePercent]="crypto.changePercent24h / 100"
                [link]="'/crypto/' + crypto.symbol"
              />
            }
          </div>
        }
      </section>
      
      <!-- Forex Section -->
      <section>
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-lg font-semibold text-gray-900">Forex Rates (USD)</h2>
          <a routerLink="/forex" class="text-sm text-primary-600 hover:text-primary-700">
            View all →
          </a>
        </div>
        
        @if (loadingForex()) {
          <app-loading-spinner />
        } @else if (forexError()) {
          <div class="text-center py-8 text-red-600">
            {{ forexError() }}
          </div>
        } @else {
          <div class="card overflow-hidden">
            <table class="min-w-full divide-y divide-gray-200">
              <thead class="bg-gray-50">
                <tr>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Currency
                  </th>
                  <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Rate
                  </th>
                </tr>
              </thead>
              <tbody class="bg-white divide-y divide-gray-200">
                @for (rate of forexRates(); track rate.targetCurrency) {
                  <tr class="hover:bg-gray-50">
                    <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                      USD / {{ rate.targetCurrency }}
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 text-right">
                      {{ rate.rate | number:'1.4-4' }}
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        }
      </section>
      
      <!-- Quick Actions -->
      <section>
        <h2 class="text-lg font-semibold text-gray-900 mb-4">Quick Actions</h2>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <a routerLink="/alerts/create" 
             class="card p-6 hover:shadow-md transition-shadow duration-200 text-center">
            <div class="text-3xl mb-2">🔔</div>
            <h3 class="font-medium text-gray-900">Create Alert</h3>
            <p class="text-sm text-gray-500 mt-1">Set price notifications</p>
          </a>
          <a routerLink="/alerts" 
             class="card p-6 hover:shadow-md transition-shadow duration-200 text-center">
            <div class="text-3xl mb-2">📋</div>
            <h3 class="font-medium text-gray-900">My Alerts</h3>
            <p class="text-sm text-gray-500 mt-1">View active alerts</p>
          </a>
          <a href="/swagger" target="_blank"
             class="card p-6 hover:shadow-md transition-shadow duration-200 text-center">
            <div class="text-3xl mb-2">📚</div>
            <h3 class="font-medium text-gray-900">API Docs</h3>
            <p class="text-sm text-gray-500 mt-1">Explore the API</p>
          </a>
        </div>
      </section>
    </div>
  `
})
export class DashboardComponent implements OnInit {
  private readonly cryptoService = inject(CryptoService);
  private readonly forexService = inject(ForexService);
  
  topCrypto = signal<CryptoPrice[]>([]);
  forexRates = signal<ForexRate[]>([]);
  
  loadingCrypto = signal(true);
  loadingForex = signal(true);
  
  cryptoError = signal<string | null>(null);
  forexError = signal<string | null>(null);
  
  ngOnInit() {
    this.loadCrypto();
    this.loadForex();
  }
  
  private loadCrypto() {
    this.cryptoService.getTop(6).subscribe({
      next: (data) => {
        this.topCrypto.set(data);
        this.loadingCrypto.set(false);
      },
      error: (err) => {
        this.cryptoError.set(err.message);
        this.loadingCrypto.set(false);
      }
    });
  }
  
  private loadForex() {
    this.forexService.getAll().subscribe({
      next: (data) => {
        this.forexRates.set(data.slice(0, 6));
        this.loadingForex.set(false);
      },
      error: (err) => {
        this.forexError.set(err.message);
        this.loadingForex.set(false);
      }
    });
  }
}
```

### 3.26 src/app/features/crypto/crypto-list/crypto-list.component.ts

```typescript
import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CryptoService } from '../../../core/services/crypto.service';
import { CryptoPrice } from '../../../core/models/crypto-price.model';
import { PriceCardComponent } from '../../../shared/components/price-card/price-card.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-crypto-list',
  standalone: true,
  imports: [CommonModule, PriceCardComponent, LoadingSpinnerComponent],
  template: `
    <div class="space-y-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Cryptocurrencies</h1>
        <p class="mt-1 text-sm text-gray-500">Track real-time cryptocurrency prices</p>
      </div>
      
      @if (loading()) {
        <app-loading-spinner />
      } @else if (error()) {
        <div class="text-center py-8 text-red-600">
          {{ error() }}
        </div>
      } @else {
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          @for (crypto of cryptos(); track crypto.symbol) {
            <app-price-card
              [symbol]="crypto.symbol"
              [name]="crypto.name"
              [price]="crypto.priceUsd"
              [changePercent]="crypto.changePercent24h / 100"
              [link]="'/crypto/' + crypto.symbol"
            />
          }
        </div>
        
        @if (cryptos().length === 0) {
          <div class="text-center py-12 text-gray-500">
            No cryptocurrency data available
          </div>
        }
      }
    </div>
  `
})
export class CryptoListComponent implements OnInit {
  private readonly cryptoService = inject(CryptoService);
  
  cryptos = signal<CryptoPrice[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  
  ngOnInit() {
    this.cryptoService.getAll().subscribe({
      next: (data) => {
        this.cryptos.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.message);
        this.loading.set(false);
      }
    });
  }
}
```

### 3.27 src/app/features/crypto/crypto-detail/crypto-detail.component.ts

```typescript
import { Component, OnInit, Input, inject, signal } from '@angular/core';
import { CommonModule, DecimalPipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CryptoService } from '../../../core/services/crypto.service';
import { CryptoPrice } from '../../../core/models/crypto-price.model';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-crypto-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, LoadingSpinnerComponent, DecimalPipe, DatePipe],
  template: `
    <div class="space-y-6">
      <!-- Back link -->
      <a routerLink="/crypto" class="inline-flex items-center text-sm text-gray-500 hover:text-gray-700">
        ← Back to Cryptocurrencies
      </a>
      
      @if (loading()) {
        <app-loading-spinner />
      } @else if (error()) {
        <div class="text-center py-8 text-red-600">
          {{ error() }}
        </div>
      } @else if (crypto()) {
        <div class="card">
          <div class="p-6">
            <!-- Header -->
            <div class="flex items-center justify-between mb-6">
              <div class="flex items-center">
                <div class="w-16 h-16 rounded-full bg-gray-100 flex items-center justify-center">
                  <span class="text-2xl font-bold text-gray-600">{{ crypto()!.symbol.charAt(0) }}</span>
                </div>
                <div class="ml-4">
                  <h1 class="text-2xl font-bold text-gray-900">{{ crypto()!.name }}</h1>
                  <p class="text-gray-500">{{ crypto()!.symbol }}</p>
                </div>
              </div>
              <div class="text-right">
                <p class="text-3xl font-bold text-gray-900">
                  \${{ crypto()!.priceUsd | number:'1.2-2' }}
                </p>
                <p [class]="crypto()!.changePercent24h >= 0 ? 'price-up' : 'price-down'" 
                   class="text-lg font-medium">
                  {{ crypto()!.changePercent24h >= 0 ? '+' : '' }}{{ crypto()!.changePercent24h | number:'1.2-2' }}%
                </p>
              </div>
            </div>
            
            <!-- Stats Grid -->
            <div class="grid grid-cols-2 md:grid-cols-4 gap-6">
              <div>
                <p class="text-sm text-gray-500">Market Cap</p>
                <p class="text-lg font-semibold text-gray-900">
                  \${{ crypto()!.marketCapUsd | number:'1.0-0' }}
                </p>
              </div>
              <div>
                <p class="text-sm text-gray-500">24h Volume</p>
                <p class="text-lg font-semibold text-gray-900">
                  \${{ crypto()!.volume24hUsd | number:'1.0-0' }}
                </p>
              </div>
              <div>
                <p class="text-sm text-gray-500">Price (EUR)</p>
                <p class="text-lg font-semibold text-gray-900">
                  €{{ crypto()!.priceEur | number:'1.2-2' }}
                </p>
              </div>
              <div>
                <p class="text-sm text-gray-500">Last Updated</p>
                <p class="text-lg font-semibold text-gray-900">
                  {{ crypto()!.lastUpdated | date:'short' }}
                </p>
              </div>
            </div>
            
            <!-- Actions -->
            <div class="mt-6 pt-6 border-t border-gray-200">
              <a [routerLink]="['/alerts/create']" 
                 [queryParams]="{type: 'crypto', symbol: crypto()!.symbol}"
                 class="btn-primary">
                Create Price Alert
              </a>
            </div>
          </div>
        </div>
      }
    </div>
  `
})
export class CryptoDetailComponent implements OnInit {
  @Input() symbol!: string;
  
  private readonly cryptoService = inject(CryptoService);
  
  crypto = signal<CryptoPrice | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  
  ngOnInit() {
    this.cryptoService.getBySymbol(this.symbol).subscribe({
      next: (data) => {
        this.crypto.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.message);
        this.loading.set(false);
      }
    });
  }
}
```

### 3.28 src/app/features/alerts/alerts-list/alerts-list.component.ts

```typescript
import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AlertsService } from '../../../core/services/alerts.service';
import { Alert } from '../../../core/models/alert.model';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-alerts-list',
  standalone: true,
  imports: [CommonModule, RouterLink, LoadingSpinnerComponent, DatePipe],
  template: `
    <div class="space-y-6">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900">Price Alerts</h1>
          <p class="mt-1 text-sm text-gray-500">Get notified when prices hit your targets</p>
        </div>
        <a routerLink="/alerts/create" class="btn-primary">
          + Create Alert
        </a>
      </div>
      
      @if (loading()) {
        <app-loading-spinner />
      } @else if (error()) {
        <div class="text-center py-8 text-red-600">
          {{ error() }}
        </div>
      } @else if (alerts().length === 0) {
        <div class="card p-12 text-center">
          <div class="text-4xl mb-4">🔔</div>
          <h3 class="text-lg font-medium text-gray-900">No alerts yet</h3>
          <p class="text-gray-500 mt-2">Create your first price alert to get started</p>
          <a routerLink="/alerts/create" class="btn-primary mt-4 inline-block">
            Create Alert
          </a>
        </div>
      } @else {
        <div class="card overflow-hidden">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Asset</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Condition</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Target</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Status</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Created</th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Actions</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              @for (alert of alerts(); track alert.id) {
                <tr class="hover:bg-gray-50">
                  <td class="px-6 py-4 whitespace-nowrap">
                    <div class="flex items-center">
                      <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                            [class]="getAssetTypeClass(alert.assetType)">
                        {{ alert.assetType }}
                      </span>
                      <span class="ml-2 font-medium text-gray-900">{{ alert.symbol }}</span>
                    </div>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    Price {{ alert.condition }}
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    \${{ alert.targetPrice | number:'1.2-2' }}
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap">
                    @if (alert.isTriggered) {
                      <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                        ✓ Triggered
                      </span>
                    } @else if (alert.isActive) {
                      <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
                        Active
                      </span>
                    } @else {
                      <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-800">
                        Inactive
                      </span>
                    }
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {{ alert.createdAt | date:'short' }}
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-right text-sm">
                    <button (click)="deleteAlert(alert.id)" 
                            class="text-red-600 hover:text-red-900">
                      Delete
                    </button>
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      }
    </div>
  `
})
export class AlertsListComponent implements OnInit {
  private readonly alertsService = inject(AlertsService);
  
  alerts = signal<Alert[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  
  ngOnInit() {
    this.loadAlerts();
  }
  
  loadAlerts() {
    this.alertsService.getAll().subscribe({
      next: (data) => {
        this.alerts.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.message);
        this.loading.set(false);
      }
    });
  }
  
  deleteAlert(id: number) {
    if (confirm('Are you sure you want to delete this alert?')) {
      this.alertsService.delete(id).subscribe({
        next: () => {
          this.alerts.update(alerts => alerts.filter(a => a.id !== id));
        },
        error: (err) => {
          alert('Failed to delete alert: ' + err.message);
        }
      });
    }
  }
  
  getAssetTypeClass(type: string): string {
    switch (type) {
      case 'crypto': return 'bg-orange-100 text-orange-800';
      case 'stock': return 'bg-purple-100 text-purple-800';
      case 'forex': return 'bg-teal-100 text-teal-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  }
}
```

### 3.29 src/app/features/alerts/alert-create/alert-create.component.ts

```typescript
import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AlertsService } from '../../../core/services/alerts.service';

@Component({
  selector: 'app-alert-create',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule],
  template: `
    <div class="max-w-2xl mx-auto space-y-6">
      <a routerLink="/alerts" class="inline-flex items-center text-sm text-gray-500 hover:text-gray-700">
        ← Back to Alerts
      </a>
      
      <div class="card">
        <div class="p-6">
          <h1 class="text-xl font-bold text-gray-900 mb-6">Create Price Alert</h1>
          
          <form [formGroup]="form" (ngSubmit)="onSubmit()" class="space-y-6">
            <!-- Asset Type -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Asset Type</label>
              <select formControlName="assetType" class="input">
                <option value="">Select type...</option>
                <option value="crypto">Cryptocurrency</option>
                <option value="stock">Stock</option>
                <option value="forex">Forex</option>
              </select>
            </div>
            
            <!-- Symbol -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Symbol</label>
              <input type="text" formControlName="symbol" class="input" 
                     placeholder="e.g., BTC, AAPL, EUR">
              <p class="mt-1 text-xs text-gray-500">Enter the asset symbol (e.g., BTC for Bitcoin)</p>
            </div>
            
            <!-- Condition -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Condition</label>
              <div class="flex space-x-4">
                <label class="flex items-center">
                  <input type="radio" formControlName="condition" value="above" 
                         class="h-4 w-4 text-primary-600 focus:ring-primary-500">
                  <span class="ml-2 text-sm text-gray-700">Price goes above</span>
                </label>
                <label class="flex items-center">
                  <input type="radio" formControlName="condition" value="below" 
                         class="h-4 w-4 text-primary-600 focus:ring-primary-500">
                  <span class="ml-2 text-sm text-gray-700">Price goes below</span>
                </label>
              </div>
            </div>
            
            <!-- Target Price -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Target Price (USD)</label>
              <div class="relative">
                <span class="absolute inset-y-0 left-0 pl-3 flex items-center text-gray-500">$</span>
                <input type="number" formControlName="targetPrice" class="input pl-7" 
                       placeholder="0.00" step="0.01">
              </div>
            </div>
            
            <!-- Webhook URL (optional) -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Webhook URL <span class="text-gray-400">(optional)</span>
              </label>
              <input type="url" formControlName="webhookUrl" class="input" 
                     placeholder="https://your-webhook-url.com">
              <p class="mt-1 text-xs text-gray-500">We'll POST to this URL when the alert triggers</p>
            </div>
            
            <!-- Submit -->
            <div class="flex items-center justify-end space-x-3 pt-4 border-t">
              <a routerLink="/alerts" class="btn-secondary">Cancel</a>
              <button type="submit" [disabled]="!form.valid || submitting()" class="btn-primary">
                @if (submitting()) {
                  Creating...
                } @else {
                  Create Alert
                }
              </button>
            </div>
            
            @if (error()) {
              <div class="p-3 bg-red-50 text-red-700 rounded-md text-sm">
                {{ error() }}
              </div>
            }
          </form>
        </div>
      </div>
    </div>
  `
})
export class AlertCreateComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly alertsService = inject(AlertsService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  
  submitting = signal(false);
  error = signal<string | null>(null);
  
  form: FormGroup = this.fb.group({
    assetType: ['', Validators.required],
    symbol: ['', [Validators.required, Validators.maxLength(10)]],
    condition: ['above', Validators.required],
    targetPrice: [null, [Validators.required, Validators.min(0)]],
    webhookUrl: ['']
  });
  
  ngOnInit() {
    // Pre-fill from query params if available
    this.route.queryParams.subscribe(params => {
      if (params['type']) {
        this.form.patchValue({ assetType: params['type'] });
      }
      if (params['symbol']) {
        this.form.patchValue({ symbol: params['symbol'] });
      }
    });
  }
  
  onSubmit() {
    if (!this.form.valid) return;
    
    this.submitting.set(true);
    this.error.set(null);
    
    this.alertsService.create(this.form.value).subscribe({
      next: () => {
        this.router.navigate(['/alerts']);
      },
      error: (err) => {
        this.error.set(err.message);
        this.submitting.set(false);
      }
    });
  }
}
```

### 3.30 src/frontend/Dockerfile

```dockerfile
# Build stage
FROM node:20-alpine AS build
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm ci

# Copy source
COPY . .

# Build for production
RUN npm run build:prod

# Production stage - just the static files
FROM nginx:alpine AS production
COPY --from=build /app/dist/price-tracker/browser /usr/share/nginx/html

# This is just for building the static files
# In deployment, we copy to the main nginx container
```

---

## Phase 4: Nginx Configuration

### 4.1 deployment/app/nginx/nginx.conf

```nginx
user nginx;
worker_processes auto;
error_log /var/log/nginx/error.log warn;
pid /var/run/nginx.pid;

events {
    worker_connections 1024;
    use epoll;
    multi_accept on;
}

http {
    include /etc/nginx/mime.types;
    default_type application/octet-stream;

    # Logging
    log_format main '$remote_addr - $remote_user [$time_local] "$request" '
                    '$status $body_bytes_sent "$http_referer" '
                    '"$http_user_agent" "$http_x_forwarded_for" '
                    'rt=$request_time uct="$upstream_connect_time" '
                    'uht="$upstream_header_time" urt="$upstream_response_time"';

    access_log /var/log/nginx/access.log main;

    # Performance
    sendfile on;
    tcp_nopush on;
    tcp_nodelay on;
    keepalive_timeout 65;
    types_hash_max_size 2048;

    # Gzip compression
    gzip on;
    gzip_vary on;
    gzip_proxied any;
    gzip_comp_level 6;
    gzip_types text/plain text/css text/xml application/json application/javascript 
               application/xml application/xml+rss text/javascript application/wasm;

    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;

    # Hide nginx version
    server_tokens off;

    # Include server configurations
    include /etc/nginx/conf.d/*.conf;
}
```

### 4.2 deployment/app/nginx/conf.d/default.conf

```nginx
# Upstream for .NET API
upstream api_backend {
    server api:8080;
    keepalive 32;
}

server {
    listen 80;
    server_name _;
    
    root /usr/share/nginx/html;
    index index.html;

    # Health check endpoint for Cloudflare
    location /nginx-health {
        access_log off;
        return 200 "healthy\n";
        add_header Content-Type text/plain;
    }

    # API proxy
    location /api/ {
        proxy_pass http://api_backend;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
        
        # Timeouts
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
    }

    # Swagger UI
    location /swagger {
        proxy_pass http://api_backend;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    # Hangfire Dashboard (protected by Zero Trust)
    location /hangfire {
        proxy_pass http://api_backend;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    # Prometheus metrics endpoint
    location /metrics {
        proxy_pass http://api_backend;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
    }

    # Health check endpoint
    location /health {
        proxy_pass http://api_backend;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
    }

    # Angular static files
    location / {
        try_files $uri $uri/ /index.html;
        
        # Cache static assets
        location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
            expires 1y;
            add_header Cache-Control "public, immutable";
        }
        
        # Don't cache index.html
        location = /index.html {
            expires -1;
            add_header Cache-Control "no-store, no-cache, must-revalidate";
        }
    }

    # Deny access to hidden files
    location ~ /\. {
        deny all;
        access_log off;
        log_not_found off;
    }
}
```

---

## Phase 5: Updated Docker Compose - App Server

### 5.1 deployment/app/docker-compose.yml

```yaml
version: '3.8'

services:
  nginx:
    image: nginx:alpine
    container_name: price-tracker-nginx
    restart: unless-stopped
    ports:
      - "80:80"
    volumes:
      - ./nginx/nginx.conf:/etc/nginx/nginx.conf:ro
      - ./nginx/conf.d:/etc/nginx/conf.d:ro
      - ./frontend:/usr/share/nginx/html:ro
    depends_on:
      - api
    networks:
      - app-network
    healthcheck:
      test: ["CMD", "wget", "--no-verbose", "--tries=1", "--spider", "http://localhost/nginx-health"]
      interval: 30s
      timeout: 10s
      retries: 3

  api:
    image: ghcr.io/${GITHUB_REPOSITORY}/price-tracker-api:${IMAGE_TAG:-latest}
    container_name: price-tracker-api
    restart: unless-stopped
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Host=db;Database=pricetracker;Username=postgres;Password=${DB_PASSWORD}
      - ConnectionStrings__HangfireConnection=Host=db;Database=pricetracker_hangfire;Username=postgres;Password=${DB_PASSWORD}
      - ConnectionStrings__Redis=redis:6379,password=${REDIS_PASSWORD}
    depends_on:
      db:
        condition: service_healthy
      redis:
        condition: service_healthy
    networks:
      - app-network
    healthcheck:
      test: ["CMD", "wget", "--no-verbose", "--tries=1", "--spider", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s
    labels:
      - "prometheus.scrape=true"
      - "prometheus.port=8080"
      - "prometheus.path=/metrics"

  db:
    image: postgres:16-alpine
    container_name: price-tracker-db
    restart: unless-stopped
    environment:
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=${DB_PASSWORD}
      - POSTGRES_MULTIPLE_DATABASES=pricetracker,pricetracker_hangfire
    volumes:
      - postgres-data:/var/lib/postgresql/data
      - ./init-db.sh:/docker-entrypoint-initdb.d/init-db.sh
    networks:
      - app-network
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 5

  redis:
    image: redis:7-alpine
    container_name: price-tracker-redis
    restart: unless-stopped
    command: redis-server --requirepass ${REDIS_PASSWORD} --appendonly yes
    volumes:
      - redis-data:/data
    networks:
      - app-network
    healthcheck:
      test: ["CMD", "redis-cli", "-a", "${REDIS_PASSWORD}", "ping"]
      interval: 10s
      timeout: 5s
      retries: 5

  cloudflared:
    image: cloudflare/cloudflared:latest
    container_name: cloudflared-app
    restart: unless-stopped
    command: tunnel --no-autoupdate run --token ${CLOUDFLARE_TUNNEL_TOKEN}
    networks:
      - app-network
    depends_on:
      - nginx

  node-exporter:
    image: prom/node-exporter:latest
    container_name: node-exporter
    restart: unless-stopped
    command:
      - '--path.procfs=/host/proc'
      - '--path.sysfs=/host/sys'
      - '--collector.filesystem.mount-points-exclude=^/(sys|proc|dev|host|etc)($$|/)'
    volumes:
      - /proc:/host/proc:ro
      - /sys:/host/sys:ro
      - /:/rootfs:ro
    networks:
      - app-network
    ports:
      - "9100:9100"

networks:
  app-network:
    driver: bridge

volumes:
  postgres-data:
  redis-data:
```

---

## Phase 6: Updated Terraform - Tunnel Configuration

### 6.1 Update cloudflare-tunnels.tf

```hcl
# App Tunnel - now points to Nginx
resource "cloudflare_tunnel_config" "app" {
  account_id = var.cloudflare_account_id
  tunnel_id  = cloudflare_tunnel.app.id

  config {
    ingress_rule {
      hostname = "${var.app_subdomain}.${var.domain}"
      service  = "http://nginx:80"  # Changed from localhost:8080 to nginx:80
    }
    ingress_rule {
      service = "http_status:404"
    }
  }
}
```

---

## Phase 7: Updated GitHub Actions

### 7.1 .github/workflows/frontend-build.yml

```yaml
name: Build Frontend

on:
  push:
    branches:
      - main
    paths:
      - 'src/frontend/**'
      - '.github/workflows/frontend-build.yml'
  pull_request:
    paths:
      - 'src/frontend/**'

jobs:
  build:
    name: Build Angular
    runs-on: ubuntu-latest

    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup Node.js
        uses: actions/setup-node@v4
        with:
          node-version: '20'
          cache: 'npm'
          cache-dependency-path: src/frontend/package-lock.json

      - name: Install dependencies
        working-directory: src/frontend
        run: npm ci

      - name: Lint
        working-directory: src/frontend
        run: npm run lint --if-present

      - name: Build
        working-directory: src/frontend
        run: npm run build:prod

      - name: Upload build artifact
        if: github.event_name != 'pull_request'
        uses: actions/upload-artifact@v4
        with:
          name: frontend-dist
          path: src/frontend/dist/price-tracker/browser
          retention-days: 7
```

### 7.2 .github/workflows/frontend-deploy.yml

```yaml
name: Deploy Frontend

on:
  workflow_run:
    workflows: ["Build Frontend"]
    types:
      - completed
    branches:
      - main
  workflow_dispatch:

jobs:
  deploy:
    name: Deploy to App Server
    runs-on: ubuntu-latest
    if: ${{ github.event.workflow_run.conclusion == 'success' || github.event_name == 'workflow_dispatch' }}
    environment: production

    steps:
      - name: Download artifact
        uses: actions/download-artifact@v4
        with:
          name: frontend-dist
          path: dist
          run-id: ${{ github.event.workflow_run.id }}
          github-token: ${{ secrets.GITHUB_TOKEN }}

      - name: Setup SSH
        run: |
          mkdir -p ~/.ssh
          echo "${{ secrets.SSH_PRIVATE_KEY }}" > ~/.ssh/id_ed25519
          chmod 600 ~/.ssh/id_ed25519
          ssh-keyscan -H ${{ secrets.APP_SERVER_IP }} >> ~/.ssh/known_hosts

      - name: Deploy to server
        run: |
          # Create directory if not exists
          ssh root@${{ secrets.APP_SERVER_IP }} "mkdir -p /opt/price-tracker/frontend"
          
          # Sync files
          rsync -avz --delete dist/ root@${{ secrets.APP_SERVER_IP }}:/opt/price-tracker/frontend/
          
          # Verify deployment
          ssh root@${{ secrets.APP_SERVER_IP }} "ls -la /opt/price-tracker/frontend/"

      - name: Reload Nginx
        run: |
          ssh root@${{ secrets.APP_SERVER_IP }} "docker exec price-tracker-nginx nginx -s reload"
```

### 7.3 Updated .github/workflows/api-deploy.yml

```yaml
name: Deploy API

on:
  workflow_run:
    workflows: ["Build and Push API"]
    types:
      - completed
    branches:
      - main
  workflow_dispatch:
    inputs:
      image_tag:
        description: 'Docker image tag to deploy'
        required: false
        default: 'latest'

jobs:
  deploy:
    name: Deploy to App Server
    runs-on: ubuntu-latest
    if: ${{ github.event.workflow_run.conclusion == 'success' || github.event_name == 'workflow_dispatch' }}
    environment: production

    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup SSH
        run: |
          mkdir -p ~/.ssh
          echo "${{ secrets.SSH_PRIVATE_KEY }}" > ~/.ssh/id_ed25519
          chmod 600 ~/.ssh/id_ed25519
          ssh-keyscan -H ${{ secrets.APP_SERVER_IP }} >> ~/.ssh/known_hosts

      - name: Copy deployment files
        run: |
          scp -r deployment/app/* root@${{ secrets.APP_SERVER_IP }}:/opt/price-tracker/

      - name: Deploy
        run: |
          ssh root@${{ secrets.APP_SERVER_IP }} << 'EOF'
            cd /opt/price-tracker
            
            # Create .env file
            cat > .env << 'ENVEOF'
          DB_PASSWORD=${{ secrets.DB_PASSWORD }}
          REDIS_PASSWORD=${{ secrets.REDIS_PASSWORD }}
          CLOUDFLARE_TUNNEL_TOKEN=${{ secrets.APP_TUNNEL_TOKEN }}
          GITHUB_REPOSITORY=${{ github.repository }}
          IMAGE_TAG=${{ github.event.inputs.image_tag || 'latest' }}
          ENVEOF
            
            # Ensure frontend directory exists
            mkdir -p frontend
            
            # Login to GitHub Container Registry
            echo "${{ secrets.GITHUB_TOKEN }}" | docker login ghcr.io -u ${{ github.actor }} --password-stdin
            
            # Pull and deploy
            docker compose pull
            docker compose up -d
            
            # Cleanup old images
            docker image prune -f
            
            # Health check
            sleep 30
            curl -f http://localhost/health || exit 1
          EOF

      - name: Verify deployment
        run: |
          sleep 10
          curl -f https://tracker.urwave.dev/api/status || echo "API not yet accessible via tunnel"
```

---

## Phase 8: Additional Files

### 8.1 src/frontend/tsconfig.json

```json
{
  "compileOnSave": false,
  "compilerOptions": {
    "outDir": "./dist/out-tsc",
    "strict": true,
    "noImplicitOverride": true,
    "noPropertyAccessFromIndexSignature": true,
    "noImplicitReturns": true,
    "noFallthroughCasesInSwitch": true,
    "skipLibCheck": true,
    "esModuleInterop": true,
    "sourceMap": true,
    "declaration": false,
    "experimentalDecorators": true,
    "moduleResolution": "bundler",
    "importHelpers": true,
    "target": "ES2022",
    "module": "ES2022",
    "lib": ["ES2022", "dom"],
    "paths": {
      "@core/*": ["src/app/core/*"],
      "@shared/*": ["src/app/shared/*"],
      "@features/*": ["src/app/features/*"]
    }
  },
  "angularCompilerOptions": {
    "enableI18nLegacyMessageIdFormat": false,
    "strictInjectionParameters": true,
    "strictInputAccessModifiers": true,
    "strictTemplates": true
  }
}
```

### 8.2 src/frontend/tsconfig.app.json

```json
{
  "extends": "./tsconfig.json",
  "compilerOptions": {
    "outDir": "./out-tsc/app",
    "types": []
  },
  "files": ["src/main.ts"],
  "include": ["src/**/*.d.ts"]
}
```

---

## Execution Order for Claude Code

### Step 1: Create Project Structure
Create all directories as specified.

### Step 2: Terraform Files
Create all files in `infrastructure/terraform/`.

### Step 3: .NET API (Phase from v1)
Create the complete API project as specified in v1.

### Step 4: Angular Frontend
1. Initialize Angular project: `ng new price-tracker --style=scss --routing --standalone`
2. Install dependencies: Tailwind, ng2-charts
3. Create all components, services, and models
4. Build and test locally

### Step 5: Nginx Configuration
Create nginx.conf and default.conf.

### Step 6: Docker Compose Files
Create updated docker-compose.yml with nginx.

### Step 7: GitHub Actions
Create all workflow files.

### Step 8: Documentation
Update README.md with frontend information.

---

## Updated Cost Summary

| Resource | Monthly Cost |
|----------|-------------|
| Hetzner CX22 × 2 | ~€8.70 |
| Cloudflare (Free tier) | €0 |
| GitHub Actions (Free tier) | €0 |
| **Total** | **~€8.70/month** |

---

## Post-Implementation Checklist

- [ ] All v1 checklist items
- [ ] Angular frontend builds successfully
- [ ] Nginx serves static files
- [ ] Nginx proxies /api/* to .NET API
- [ ] Frontend accessible at https://tracker.urwave.dev
- [ ] API accessible at https://tracker.urwave.dev/api
- [ ] Swagger at https://tracker.urwave.dev/swagger
- [ ] Frontend CI/CD pipeline working
- [ ] Frontend hot reloads on deployment
