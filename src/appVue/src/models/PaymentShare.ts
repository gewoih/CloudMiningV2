import {ShareStatus} from "@/enums/ShareStatus.ts";

export interface PaymentShare {
    user: {
        id: string;
        firstName: string;
        lastName: string;
        patronymic: string;
    };
    id: string;
    amount: number;
    share: number;
    status: ShareStatus;
}