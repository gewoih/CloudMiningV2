import {Payment} from "@/models/Payment.ts";
export interface PaymentList {
    items: Payment[];
    totalCount: number;
}