<template>

  <div class="w-8">

    <Toolbar class="mb-4">
      <template #end>
        <Dropdown v-model="selectedPaymentType" :options="paymentTypes" optionLabel="name" optionValue="value" @change="fetchPayments" class="w-full md:w-14rem" />
      </template>
    </Toolbar>

    <DataTable :value="payments" dataKey="id">
      <Column field="isCompleted" header="Статус">
        <template #body="slotProps">
          <Tag :value="slotProps.data.isCompleted ? 'Завершен' : 'Ожидание'" :severity="getStatusSeverity(slotProps.data.isCompleted)" />
        </template>
      </Column>
      <Column field="amount" header="Сумма"/>
      <Column field="share" header="Доля"></Column>
      <Column field="totalAmount" header="Общая сумма"/>
      <Column field="date" header="Дата">
        <template #body="slotProps">
          {{ getDateOnly(slotProps.data.date) }}
        </template>
      </Column>
    </DataTable>
    <Paginator :rows="pageSize" :totalRecords="totalPaymentsCount" :rowsPerPageOptions="[5, 10, 15]" @page="pageChange"></Paginator>
  </div>

</template>

<script setup lang="ts">
import {ref} from 'vue';
import {paymentsService} from "@/services/payments.api.ts";
import {format} from 'date-fns';
import {PaymentType} from "@/enums/PaymentType.ts";
import {Payment} from "@/models/Payment.ts";

const selectedPaymentType = ref(PaymentType.Electricity);
const payments = ref<Payment[]>();
const totalPaymentsCount = ref(0);
const pageSize = ref(10);
const pageNumber = ref(1);

const paymentTypes = ref([
  { name: 'Электричество', value: 'Electricity' },
  { name: 'Покупки', value: 'Purchase' }
]);

const pageChange = async (event) => {
  pageNumber.value = event.page+1;
  pageSize.value = event.rows;
  await fetchPayments();
}

const getStatusSeverity = (isCompleted: boolean) => {
  return isCompleted ? 'success' : 'danger';
}

const getDateOnly = (date) => {
  return format(date, 'dd.MM.yyyy');
};

const fetchPayments = async () => {
  const response = await paymentsService.getPayments(pageNumber.value, pageSize.value, selectedPaymentType.value);
  payments.value = response.items;
  totalPaymentsCount.value = response.totalCount;
};

fetchPayments();

</script>
