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
