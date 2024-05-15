export interface Payment{
    id: string | null;
    amount: number;
    sharedAmount: number;
    share: number;
    date: Date;
    isCompleted: boolean;
}


