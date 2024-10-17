import {Statistics} from "@/models/Statistics.ts";
import {Purchase} from "@/models/Purchase.ts";

export interface StatisticsPage{
    statisticsDtoList: Statistics[];
    purchaseDtoList: Purchase[];
}