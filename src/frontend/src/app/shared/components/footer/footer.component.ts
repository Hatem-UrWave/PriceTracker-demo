import { Component } from '@angular/core';

@Component({
  selector: 'app-footer',
  imports: [],
  template: `
    <footer class="bg-white border-t border-gray-200">
      <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
        <div class="flex flex-col md:flex-row justify-between items-center">
          <div class="text-gray-500 text-sm">
            © 2026 PriceTracker. Built for learning DevOps.
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
