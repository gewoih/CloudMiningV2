export interface Member{
    user: {
        id: string;
        firstName: string;
        lastName: string;
        patronymic: string;
    };
    share: number;
    amount: number;
    date: Date;
}