import {ExpenseType} from "@/enums/ExpenseType.ts";
import {PriceBar} from "@/models/PriceBar.ts";

export interface Expense{
    Type: ExpenseType;
    PriceBars: PriceBar[] | null;
}