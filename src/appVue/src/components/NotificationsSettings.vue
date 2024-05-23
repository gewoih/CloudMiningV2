<script setup lang="ts">
import { ref } from "vue";
import { NotificationSettings } from "@/models/NotificationSettings.ts";

const notificationSettings = ref<NotificationSettings>({
  isTelegramNotificationsEnabled: false,
  newPayoutNotification: false,
  newElectricityPaymentNotification: false,
  newPurchaseNotification: false,
  unpaidElectricityPaymentReminder: false,
  unpaidPurchasePaymentReminder: false,
});
</script>

<template>
 
  <div>
    <h2>Настройки уведомлений</h2>

    <div>
      <h4>Источники уведомлений</h4>

      <div class="field-checkbox">
        <Checkbox v-model="notificationSettings.isTelegramNotificationsEnabled" :binary="true" input-id="isTelegramEnabled"/>
        <label for="isTelegramEnabled">Telegram</label>
      </div>
    </div>

    <div>
      <p class="font-bold">Уведомления по новым платежам</p>

      <div class="field-checkbox">
        <Checkbox v-model="notificationSettings.newPayoutNotification" :binary="true" input-id="newPayout"/>
        <label for="newPayout">Выплаты в криптовалюте</label>
      </div>

      <div class="field-checkbox">
        <Checkbox v-model="notificationSettings.newElectricityPaymentNotification" :binary="true" input-id="newElectricity"/>
        <label for="newElectricity">Платежи по электричеству</label>
      </div>

      <div class="field-checkbox">
        <Checkbox v-model="notificationSettings.newPurchaseNotification" :binary="true" input-id="newPurchase"/>
        <label for="newPurchase">Покупки</label>
      </div>
    </div>

    <div>
      <p class="font-bold">Напоминания о просроченных платежах</p>

      <div class="field-checkbox">
        <Checkbox v-model="notificationSettings.unpaidElectricityPaymentReminder" :binary="true" input-id="electricityReminder"/>
        <label for="electricityReminder">По электричеству</label>
      </div>

      <div class="field-checkbox">
        <Checkbox :model-value="notificationSettings.unpaidPurchasePaymentReminder" :binary="true" input-id="purchaseReminder"/>
        <label for="purchaseReminder">По покупкам</label>
      </div>
    </div>
  </div>
</template>

<style scoped>
.field-checkbox {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}
</style>