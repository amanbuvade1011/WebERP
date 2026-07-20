export interface ProductListItem {
  styleCode: string;
  garment: string;
  category: string | null;
  label: string | null;
}

export interface PricePoint {
  id: string;
  name: string | null;
}

export interface StylePrice {
  pricePointId: string;
  pricePointName: string | null;
  localUnitPriceExTax1: number;
  localUnitPriceTax1: number;
  internationalUnitPriceExTax1: number;
  internationalUnitPriceTax1: number;
}

export interface StylePriceLine {
  pricePointId: string;
  localUnitPriceExTax1: number;
  localUnitPriceTax1: number;
  internationalUnitPriceExTax1: number;
  internationalUnitPriceTax1: number;
}

export interface StyleSellLocation {
  locationId: string;
  locationName: string | null;
  allowRetail: boolean;
  allowWebRetail: boolean;
  allowRental: boolean;
  allowWholesaleIndent: boolean;
}

export interface GenerateProductsResult {
  productsCreated: number;
  productLocationsCreated: number;
  totalProductsForColor: number;
}

export interface StockGridRow {
  productId: string;
  sizeId: string;
  sizeDescription: string;
  locationId: string;
  locationName: string | null;
  held: number;
  allocated: number;
  available: number;
}
