export interface AdminPayment{
    id: string | null;
    caption: string | null;
    date: Date;
    amount: number;
    isCompleted: boolean;
}


