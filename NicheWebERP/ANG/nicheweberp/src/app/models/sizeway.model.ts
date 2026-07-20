export interface SizewayItem {
  sizeId: string;
  sizeDescription: string;
  sequence: number;
}

export interface Sizeway {
  id: string;
  description: string;
  excludeRetailSearch: boolean;
  sizes: SizewayItem[];
}

export interface CreateSizewayRequest {
  description: string;
  excludeRetailSearch: boolean;
  sizeIds: string[];
}

export interface UpdateSizeSequenceRequest {
  sizeIds: string[];
}

export interface Size {
  id: string;
  description: string;
}
