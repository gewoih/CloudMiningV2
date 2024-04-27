<template>
  <DataTable :value="payments" dataKey="id">
    <Column expander style="width: 5rem"/>
    <Column field="caption" header="Комментарий"></Column>
    <Column field="date" header="Дата">
      <template #body="slotProps">
        {{ getDateOnly(slotProps.data.date) }}
      </template>
    </Column>
    <Column field="amount" header="Сумма"/>
    <Column field="isCompleted" header="Статус">
      <template #body="slotProps">
        <Tag :value="slotProps.data.isCompleted ? 'Завершен' : 'Ожидание'" :severity="getPaymentStatusSeverity(slotProps.data.isCompleted)" />
      </template>
    </Column>
  </DataTable>
</template>

<script setup lang="ts">
import {ref} from 'vue';
import {ShareablePayment} from "@/models/ShareablePayment.ts";
import {paymentsService} from "@/services/payments.api.ts";
import { format } from 'date-fns'
import {CurrencyCode} from "@/enums/CurrencyCode.ts";
import {PaymentType} from "@/enums/PaymentType.ts";

const selectedPaymentType = ref(PaymentType.Electricity);
const payments = ref<ShareablePayment[]>([]);
const newPayment = ref<ShareablePayment>({
  caption: null,
  currencyCode: CurrencyCode.RUB,
  paymentType: PaymentType.Electricity,
  date: new Date(),
  amount: 0,
  isCompleted: false
});
const isModalOpen = ref(false);

const getPaymentStatusSeverity = (isCompleted: boolean) => {
   return isCompleted ? 'success' : 'danger';
}

const getDateOnly = (date) => {
  return format(date, 'dd.MM.yyyy');
};

const fetchPayments = async () => {
  payments.value = await paymentsService.getPayments(selectedPaymentType.value);
};

const showModal = () => {
  isModalOpen.value = true;
};

const closeModal = () => {
  isModalOpen.value = false;
};
const createPayment = async () => {
  newPayment.value.paymentType = selectedPaymentType.value;
  await paymentsService.createPayment(newPayment.value);
  await fetchPayments();
  newPayment.value = {
    caption: null,
    currencyCode: CurrencyCode.RUB,
    paymentType: selectedPaymentType.value,
    date: new Date(),
    amount: 0,
    isCompleted: false
  };
  closeModal();
};

fetchPayments();

</script>

