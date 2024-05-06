<template>
<div class="w-8">
  
  <Toolbar class="mb-4">
    <template #start>
      <Button label="Добавить платеж" icon="pi pi-plus" severity="success" class="mr-2" @click="isModalVisible = true" />
    </template>

    <template #end>
      <Dropdown v-model="selectedPaymentType" :options="paymentTypes" optionLabel="name" optionValue="value" @change="fetchPayments" class="w-full md:w-14rem" />
    </template>
  </Toolbar>
  
  <DataTable :value="payments" @page="handlePageChange" paginator :pageCount="totalPages" :rows="10" :total-records="records"  dataKey="id">
    <Column expander/>
    <Column field="isCompleted" header="Статус">
      <template #body="slotProps">
        <Tag :value="slotProps.data.isCompleted ? 'Завершен' : 'Ожидание'" :severity="getPaymentStatusSeverity(slotProps.data.isCompleted)" />
      </template>
    </Column>
    <Column field="amount" header="Сумма"/>
    <Column field="date" header="Дата">
      <template #body="slotProps">
        {{ getDateOnly(slotProps.data.date) }}
      </template>
    </Column>
    <Column field="caption" header="Комментарий"></Column>
  </DataTable>
</div>
  
  <Dialog v-model:visible="isModalVisible" modal header="Добавление платежа" :draggable="false" :dismissableMask="true">
    <div class="flex align-items-center gap-3 mb-3">
      <label for="amount" class="font-semibold w-6rem">Сумма</label>
      <InputNumber v-model="newPayment.amount" id="amount" class="flex-auto" autocomplete="off" />
    </div>
    <div class="flex align-items-center gap-3 mb-3">
      <label for="date" class="font-semibold w-6rem">Дата</label>
      <Calendar id="date" v-model="newPayment.date" date-format="dd.mm.yy" show-icon class="flex-auto" autocomplete="off" />
    </div>
    <div class="flex align-items-center gap-3 mb-5">
      <label for="caption" class="font-semibold w-6rem">Комментарий</label>
      <InputText id="caption" v-model="newPayment.caption" class="flex-auto" autocomplete="off" />
    </div>
    <div class="flex justify-content-end gap-2">
      <Button type="submit" label="Сохранить" @click="createPayment"></Button>
    </div>
  </Dialog>
</template>

<script setup lang="ts">
import {ref} from 'vue';
import {ShareablePayment} from "@/models/ShareablePayment.ts";
import {paymentsService} from "@/services/payments.api.ts";
import {format} from 'date-fns'
import {CurrencyCode} from "@/enums/CurrencyCode.ts";
import {PaymentType} from "@/enums/PaymentType.ts";

const isModalVisible = ref(false);
const selectedPaymentType = ref(PaymentType.Electricity);
const paymentTypes = ref([
  { name: 'Электричество', value: 'Electricity' },
  { name: 'Покупки', value: 'Purchase' }
]);
const payments = ref<ShareablePayment[]>();
const newPayment = ref<ShareablePayment>({
  caption: null,
  currencyCode: CurrencyCode.RUB,
  paymentType: PaymentType.Electricity,
  date: new Date(),
  amount: 0,
  isCompleted: false
});
const records = ref(0);
const currentPage = ref(1);
const totalPages = ref(0);

const handlePageChange = (event) => {
  currentPage.value = event.page + 1;
}

const getPaymentStatusSeverity = (isCompleted: boolean) => {
   return isCompleted ? 'success' : 'danger';
}

const getDateOnly = (date) => {
  return format(date, 'dd.MM.yyyy');
};

const fetchPayments = async () => {
  const response = await paymentsService.getPayments(currentPage.value, selectedPaymentType.value);
  payments.value = response.payments;
  records.value = response.totalRecords;
  totalPages.value = Math.ceil(records.value / 10);
  console.log(totalPages.value);
};

const createPayment = async () => {
  newPayment.value.paymentType = selectedPaymentType.value;
  await paymentsService.createPayment(newPayment.value);
  //TODO: Заменить на получение созданного платежа
  await fetchPayments();
  newPayment.value = {
    caption: null,
    currencyCode: CurrencyCode.RUB,
    paymentType: selectedPaymentType.value,
    date: new Date(),
    amount: 0,
    isCompleted: false
  };
  isModalVisible.value = false;
};

fetchPayments();
console.log(totalPages.value)

</script>

