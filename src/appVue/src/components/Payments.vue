<script setup lang="ts">
import {ref} from 'vue';
import {ShareablePayment} from "@/models/ShareablePayment.ts";
import {shareablePaymentsService} from "@/services/payments.api.ts";
import {CurrencyCode} from "@/enums/CurrencyCode.ts";
import {PaymentType} from "@/enums/PaymentType.ts";

const selectedPaymentType = ref(PaymentType.Electricity);
const isModalShown = ref(false);
const payments = ref<ShareablePayment[]>([]);
const newPayment = ref<ShareablePayment>({
  caption: null,
  currencyCode: CurrencyCode.RUB,
  paymentType: PaymentType.Electricity,
  date: new Date(),
  amount: 0,
  isCompleted: false
});

const fetchPayments = async () => {
  payments.value = await shareablePaymentsService.getPayments(selectedPaymentType.value);
  console.log(payments.value);
};

const createPayment = async () => {
  newPayment.value.paymentType = selectedPaymentType.value;
  await shareablePaymentsService.createPayment(newPayment.value);
  await fetchPayments();
};

fetchPayments();

</script>

<template>
  <div class="container-fluid">
    <div class="row justify-content-end">
      <div class="col-auto">
        <select class="form-select" v-model="selectedPaymentType" @change="fetchPayments">
          <option :value="PaymentType.Electricity">Электричество</option>
          <option :value="PaymentType.Purchase">Покупки</option>
        </select>
      </div>
    </div>
    <div class="row">
      <div class="col">
        <div class="table-responsive m-3">
          <table class="table table-striped table-hover display">
            <thead class="sticky-top">
            <tr>
              <th>Сумма</th>
              <th>Дата</th>
              <th>Комментарий</th>
            </tr>
            </thead>
            <tbody>
            <tr v-for="payment in payments"
                :class="{ 'table-success': payment.isCompleted, 'table-danger': !payment.isCompleted }">
              <td>{{ payment.amount }}</td>
              <td>{{ payment.date }}</td>
              <td>{{ payment.caption }}</td>
            </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
    <div class="row justify-content-end">
      <div class="col-auto">
        <button type="button" class="btn btn-primary m-3" v-on:click="isModalShown = !isModalShown">Добавить платеж</button>
      </div>
    </div>
  </div>

  <!-- Modal -->
  <BModal id="modal-center" centered title="Создание нового платежа" v-model="isModalShown">
    <BForm @submit="createPayment">
      <BFormGroup label="Название платежа" description="Укажите комментарий к платежу">
        <BFormInput type="text" v-model="newPayment.caption" />
      </BFormGroup>

      <BFormGroup label="Дата">
        <BFormInput type="date" v-model="newPayment.date" required/>
      </BFormGroup>

      <BFormGroup label="Сумма">
        <BFormInput type="number" v-model="newPayment.amount" required/>
      </BFormGroup>
    </BForm>
  </BModal>
</template>
