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
