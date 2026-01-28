export interface ForexRate {
  id: number;
  baseCurrency: string;
  targetCurrency: string;
  rate: number;
  lastUpdated: Date;
}
