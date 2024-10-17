import {AdminPayment} from "@/models/AdminPayment.ts";
import {Payment} from "@/models/Payment.ts";

export interface PaymentList {
    items: AdminPayment[] | Payment[];
    totalCount: number;
}