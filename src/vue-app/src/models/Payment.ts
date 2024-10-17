import {ShareStatus} from "@/enums/ShareStatus.ts";
import {CurrencyCode} from "@/enums/CurrencyCode.ts";

export interface Payment {
    id: string | null;
    caption: string | null;
    date: Date;
    amount: number;
    currency: {
        shortName: string;
        code: CurrencyCode;
        precision: number;
    };
    sharedAmount: number;
    share: number;
    status: ShareStatus;
}


