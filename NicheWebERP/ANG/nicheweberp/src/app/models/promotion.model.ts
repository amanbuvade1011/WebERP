export interface Promotion {
  id: string;
  description: string | null;
  isCoupon: boolean;
  couponCode: string | null;
  couponDiscountPrintedValue: number;
  couponIsDollar: boolean;
  couponDiscountMinimumSpend: number;
  couponMaxUses: number;
  couponCurrentUses: number;
  couponMaxUsesPerson: number;
  startDate: string;
  endDate: string | null;
}

export interface CreatePromotionRequest {
  description: string;
  startDate: string;
  endDate: string | null;
  isCoupon: boolean;
  couponCode: string | null;
  couponDiscountPrintedValue: number;
  couponIsDollar: boolean;
  couponDiscountMinimumSpend: number;
  couponMaxUses: number;
  couponMaxUsesPerson: number;
}

export interface ValidateCouponRequest {
  couponCode: string;
  firmId: string;
  orderSubTotal: number;
}

export interface ValidateCouponResult {
  valid: boolean;
  message: string | null;
  discountAmount: number;
  promotionId: string | null;
}
