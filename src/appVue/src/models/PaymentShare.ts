export interface PaymentShare{
    user: {
        id: string;
        firstName: string;
        lastName: string;
        patronymic: string;
    };
    amount: number;
    share: number;
    isCompleted: boolean;
}