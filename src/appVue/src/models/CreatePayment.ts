import {CurrencyCode} from "@/enums/CurrencyCode.ts";
import {PaymentType} from "@/enums/PaymentType.ts";

export interface CreatePayment{
    caption: string | null;
    currencyCode: CurrencyCode;
    paymentType: PaymentType;
    date: Date;
    amount: number;
    isCompleted: boolean;
}