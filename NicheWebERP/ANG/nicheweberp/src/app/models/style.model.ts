export interface StyleListItem {
  id: string;
  code: string;
  description: string;
  categoryName: string | null;
  labelName: string | null;
  rangeName: string | null;
  inactive: boolean;
}

export type { PagedResult } from './paged-result.model';

export interface StyleColor {
  id: string;
  color: string;
  rgbValue: string | null;
  inactive: boolean;
}

export interface StyleDetail {
  id: string;
  code: string;
  description: string;
  webDescription: string | null;
  weight: number;
  sizewayId: string;
  sizewayDescription: string | null;
  categoryId: string | null;
  categoryName: string | null;
  labelId: string | null;
  labelName: string | null;
  rangeId: string;
  rangeName: string | null;
  inactive: boolean;
  nonStock: boolean;
  allowManualPrice: boolean;
  deliveryPeriod: string | null;
  colors: StyleColor[];
}

export interface CreateStyleRequest {
  code: string;
  description: string;
  webDescription: string | null;
  weight: number;
  sizewayId: string;
  categoryId: string | null;
  labelId: string | null;
  rangeId: string;
  deliveryPeriod: string | null;
}

export interface UpdateStyleRequest {
  description: string;
  webDescription: string | null;
  weight: number;
  sizewayId: string;
  categoryId: string | null;
  labelId: string | null;
  rangeId: string;
  deliveryPeriod: string | null;
  inactive: boolean;
}

export interface AddColorRequest {
  color: string;
  rgbValue: string | null;
}
