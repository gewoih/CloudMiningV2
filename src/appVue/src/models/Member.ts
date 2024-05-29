import {CurrencyCode} from "@/enums/CurrencyCode.ts";

export interface Member{
    user: {
        id: string;
        firstName: string;
        lastName: string;
        patronymic: string;
    };
    share: number;
    amount: number;
    currency: {
        shortName: string;
        code: CurrencyCode;
        precision: number;
    };
    date: Date;
}