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
