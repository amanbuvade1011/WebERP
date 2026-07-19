export type InvoiceStatus = 'Paid' | 'Pending' | 'Overdue' | 'Draft';

export interface Invoice {
  invoiceNo: string;
  customer: string;
  orderRef: string;
  issueDate: string;
  dueDate: string;
  amount: number;
  status: InvoiceStatus;
}
