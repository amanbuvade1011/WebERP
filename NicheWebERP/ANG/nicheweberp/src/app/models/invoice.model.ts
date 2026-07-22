// No 'Draft' - GenerateInvoice is a one-click, one-shot action (see
// docs/ai-plan/sprints/sprint-06-invoicing-payments.md), status is computed server-side from
// paid amount vs due date, never stored.
export type InvoiceStatus = 'Paid' | 'Pending' | 'Overdue';

export const INVOICE_STATUS_VALUES: Record<InvoiceStatus, number> = {
  Paid: 0,
  Pending: 1,
  Overdue: 2
};

export const INVOICE_STATUS_NAMES: readonly InvoiceStatus[] = ['Paid', 'Pending', 'Overdue'];

export interface InvoiceListItem {
  id: string;
  documentNumber: number;
  firmId: string;
  firmName: string;
  salesOrderId: string | null;
  salesOrderDocumentNumber: number | null;
  issueDate: string | null;
  dueDate: string | null;
  totalAmount: number;
  paidAmount: number;
  status: number;
  statusName: InvoiceStatus;
}

export interface InvoiceLine {
  lineId: string;
  productId: string;
  styleCode: string;
  color: string;
  sizeDescription: string;
  quantity: number;
  unitPriceExTax: number;
  unitPriceTax: number;
  lineTotalExTax: number;
  lineTotalTax: number;
}

export interface PaymentSummary {
  paymentId: string;
  documentNumber: number;
  paymentDate: string | null;
  amount: number;
  paymentMethodName: string | null;
  narration: string | null;
}

export interface InvoiceDetail {
  id: string;
  documentNumber: number;
  firmId: string;
  firmName: string;
  salesOrderId: string | null;
  salesOrderDocumentNumber: number | null;
  locationId: string;
  locationName: string | null;
  issueDate: string | null;
  dueDate: string | null;
  customerReferenceNo: string | null;
  narration: string | null;
  subTotalExTax: number;
  taxAmount: number;
  totalAmount: number;
  paidAmount: number;
  remainingAmount: number;
  status: number;
  statusName: InvoiceStatus;
  lines: InvoiceLine[];
  payments: PaymentSummary[];
}

export interface RecordPaymentRequest {
  invoiceId: string;
  amount: number;
  paymentMethodId: string;
  narration: string | null;
}
