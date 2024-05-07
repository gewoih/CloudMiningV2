import { CurrencyCode } from '@/enums/CurrencyCode';
import { PaymentType } from '@/enums/PaymentType';

export interface Payment{
    id: string | null;
    caption: string | null;
    currencyCode: CurrencyCode;
    paymentType: PaymentType;
    date: Date;
    amount: number;
    isCompleted: boolean;
}


