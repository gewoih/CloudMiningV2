import {PriceBar} from "@/models/PriceBar.ts";
import {Expense} from "@/models/Expense.ts";

export interface Statistics {
    user: {
        id: string;
        firstName: string;
        lastName: string;
        patronymic: string;
    } | null;
    totalIncome: number;
    monthlyIncome: number;
    totalExpense: number;
    electricityExpense: number;
    depositAmount: number;
    totalProfit: number;
    monthlyProfit: number;
    paybackPercent: number;
    incomes: PriceBar[];
    profits: PriceBar[];
    expenses: Expense[];
}