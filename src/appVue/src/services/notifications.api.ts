import {apiService} from "@/services/api.ts";
import {NotificationSettings} from "@/models/NotificationSettings.ts";

class NotificationsService {
    async updateNotificationSettings(settings: NotificationSettings) {
        const response = await apiService.axiosInstance.patch("/notifications/settings", settings);
        return response.status === 200;
    }

    async getNotificationSettings(): Promise<NotificationSettings> {
        const response = await apiService.axiosInstance.get("/notifications/settings");
        return response.data;
    }
}

export const notificationsService = new NotificationsService();