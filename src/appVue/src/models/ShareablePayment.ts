import { CurrencyCode } from '@/enums/CurrencyCode';
import { PaymentType } from '@/enums/PaymentType';

export interface ShareablePayment{
    caption: string | null;
    currencyCode: CurrencyCode;
    paymentType: PaymentType;
    date: Date;
    amount: number;
}


