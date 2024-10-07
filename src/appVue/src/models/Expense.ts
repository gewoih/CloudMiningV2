import {ExpenseType} from "@/enums/ExpenseType.ts";
import {PriceBar} from "@/models/PriceBar.ts";

export interface Expense{
    type: ExpenseType;
    priceBars: PriceBar[] | null;
}