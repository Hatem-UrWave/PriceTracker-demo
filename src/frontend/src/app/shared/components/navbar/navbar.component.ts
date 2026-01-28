import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, RouterLinkActive],
  template: `
    <nav class="bg-white shadow-sm border-b border-gray-200">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16">
          <div class="flex">
            <div class="flex-shrink-0 flex items-center">
              <a routerLink="/" class="text-xl font-bold text-primary-600">
                📈 PriceTracker
              </a>
            </div>

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

          <div class="flex items-center">
            <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
              <span class="w-2 h-2 mr-1 bg-green-400 rounded-full animate-pulse"></span>
              Live
            </span>
          </div>
        </div>
      </div>

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
