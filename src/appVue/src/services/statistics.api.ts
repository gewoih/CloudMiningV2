import {apiService} from "@/services/api.ts";
import {StrategyType} from "@/enums/StrategyType.ts";
import {StatisticsPage} from "@/models/StatisticsPage.ts";
import {CreatePurchase} from "@/models/CreatePurchase.ts";
import {Purchase} from "@/models/Purchase.ts";

class StatisticsService{

    async getStatistics(statisticsCalculationStrategy: StrategyType): Promise<StatisticsPage> {
        const response = await apiService.axiosInstance
            .get("/statistics", {params: {statisticsCalculationStrategy}});

        return response.data;
    }
    async createPurchase(newPurchase: CreatePurchase): Promise<Purchase> {
        const response = await apiService
            .axiosInstance
            .post("/statistics/purchases", newPurchase);
        return response.data;
    }
}

export const statisticsService = new StatisticsService();