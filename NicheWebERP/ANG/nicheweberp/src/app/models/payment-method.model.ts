export interface PaymentMethod {
  id: string;
  description: string | null;
  paymentMethodType: number;
  surchargePercentage: number;
}
