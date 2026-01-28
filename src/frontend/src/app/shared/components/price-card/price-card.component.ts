import { Component, Input } from '@angular/core';
import { DecimalPipe, PercentPipe } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-price-card',
  imports: [RouterLink, DecimalPipe, PercentPipe],
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
