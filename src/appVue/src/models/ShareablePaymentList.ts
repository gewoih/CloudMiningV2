import {Payment} from "@/models/Payment.ts";
export interface ShareablePaymentList{
    payments: Payment[];
    totalRecords: number;
}