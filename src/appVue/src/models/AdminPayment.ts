import {CurrencyCode} from "@/enums/CurrencyCode.ts";

export interface AdminPayment {
    id: string | null;
    caption: string | null;
    date: Date;
    amount: number;
    currency: {
        shortName: string;
        code: CurrencyCode;
        precision: number;
    };
    isCompleted: boolean;
}


