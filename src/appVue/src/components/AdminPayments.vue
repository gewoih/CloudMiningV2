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
  
  <DataTable :value="payments" v-model:expandedRows="expandedRows" dataKey="id"
             @rowExpand="fetchShares">
    <Column expander/>
    <Column field="isCompleted" header="Статус">
      <template #body="slotProps">
        <Tag :value="slotProps.data.isCompleted ? 'Завершен' : 'Ожидание'" :severity="getStatusSeverity(slotProps.data.isCompleted)" />
      </template>
    </Column>
    <Column field="amount" header="Сумма"/>
    <Column field="date" header="Дата">
      <template #body="slotProps">
        {{ getDateOnly(slotProps.data.date) }}
      </template>
    </Column>
    <Column field="caption" header="Комментарий"></Column>
    <template #expansion="slotProps">
      <div class="p-3">
        <DataTable :value="paymentSharesMap[slotProps.data.id]">
          <Column header="ФИО">
            <template #body="slotProps">
              {{ slotProps.data.user.lastName + ' ' + slotProps.data.user.firstName + ' ' + slotProps.data.user.patronymic }}
            </template>
          </Column>
          <Column field="amount" header="Сумма" ></Column>
          <Column field="share" header="Доля" ></Column>
          <Column field="isCompleted" header="Статус">
            <template #body="slotProps">
              <Tag :value="slotProps.data.isCompleted ? 'Завершен' : 'Ожидание'" :severity="getStatusSeverity(slotProps.data.isCompleted)" />
            </template>
          </Column>
        </DataTable>
      </div>
    </template>
  </DataTable>
  <Paginator :rows="pageSize" :totalRecords="totalPaymentsCount" :rowsPerPageOptions="[5, 10, 15]" @page="pageChange"></Paginator>
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
import {format} from 'date-fns'
import {CurrencyCode} from "@/enums/CurrencyCode.ts";
import {PaymentType} from "@/enums/PaymentType.ts";
import {Payment} from "@/models/Payment.ts";
import {CreatePayment} from "@/models/CreatePayment.ts";
import {PaymentShare} from "@/models/PaymentShare.ts";
import {adminService} from "@/services/admin.api.ts";

const isModalVisible = ref(false);
const expandedRows = ref({});
const paymentSharesMap = ref<{ [key: string]: PaymentShare[] }>({});
const selectedPaymentType = ref(PaymentType.Electricity);
const payments = ref<Payment[]>();
const paymentShares = ref<PaymentShare[]>();
const totalPaymentsCount = ref(0);
const pageSize = ref(10);
const pageNumber = ref(1);

const newPayment = ref<CreatePayment>({
  caption: null,
  currencyCode: CurrencyCode.RUB,
  paymentType: PaymentType.Electricity,
  date: new Date(),
  amount: 0,
  isCompleted: false
});

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

const fetchShares = async (event) => {
  const paymentId = event.data.id;
  if (!paymentSharesMap.value[paymentId]) {
    paymentSharesMap.value[paymentId] = await adminService.getShares(paymentId);
  }
  paymentShares.value = paymentSharesMap.value[paymentId];
}

const fetchPayments = async () => {
  const response = await adminService.getPayments(pageNumber.value, pageSize.value, selectedPaymentType.value);
  payments.value = response.items;
  totalPaymentsCount.value = response.totalCount;
};

const createPayment = async () => {
  newPayment.value.paymentType = selectedPaymentType.value;
  await adminService.createPayment(newPayment.value);
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

</script>

