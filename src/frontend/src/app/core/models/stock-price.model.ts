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
