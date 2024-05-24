<template>
  <div class="w-8">
    <Toolbar class="mb-4 border-none">
      <template #start>
        <Dropdown v-model="selectedPaymentType" :options="paymentTypes" optionLabel="name" optionValue="value"
                  @change="fetchPayments" class="w-full md:w-14rem"/>
      </template>
      <template #end v-if="userRole === UserRole.Admin && selectedPaymentType !== PaymentType.Crypto">
        <Button label="Добавить платеж" icon="pi pi-plus" severity="success" class="mr-2"
                @click="isModalVisible = true"/>
      </template>
    </Toolbar>

    <DataTable :value="payments" v-model:expandedRows="expandedRows" dataKey="id"
               @rowExpand="fetchShares">
      <Column v-if="userRole === UserRole.Admin" expander/>
      <Column v-if="userRole === UserRole.Admin" field="isCompleted" header="Статус">
        <template v-slot:body="slotProps">
          <Tag :value="isCompletedHandle(slotProps.data)"
               :severity="getPaymentStatusSeverity(slotProps.data)"/>
        </template>
      </Column>
      <Column v-if="userRole !== UserRole.Admin" field="status" header="Статус">
        <template v-slot:body="slotProps">
          <Tag :value="getStatus(slotProps.data)" :severity="getShareStatusSeverity(slotProps.data)"/>
        </template>
      </Column>
      <Column v-if="userRole !== UserRole.Admin" header="Ваша сумма">
        <template v-slot:body="slotProps">
          {{
            getTruncatedAmount(slotProps.data.sharedAmount, slotProps.data.currency.precision) + ' ' + slotProps.data.currency.shortName
          }}
        </template>
      </Column>
      <Column v-if="userRole !== UserRole.Admin" header="Ваша доля">
        <template v-slot:body="slotProps">
          {{
            getTruncatedAmount(slotProps.data.share, 2) + ' ' + "%"
          }}
        </template>
      </Column>
      <Column header="Общая сумма">
        <template v-slot:body="slotProps">
          {{
            getTruncatedAmount(slotProps.data.amount, slotProps.data.currency.precision) + ' ' + slotProps.data.currency.shortName
          }}
        </template>
      </Column>
      <Column field="date" header="Дата">
        <template v-slot:body="slotProps">
          {{ getDateOnly(slotProps.data.date) }}
        </template>
      </Column>
      <Column
          v-if="(userRole === UserRole.Admin && selectedPaymentType !== PaymentType.Crypto) || selectedPaymentType === PaymentType.Purchase"
          field="caption"
          header="Комментарий"></Column>
      <Column v-if="userRole !== UserRole.Admin && selectedPaymentType !== PaymentType.Crypto">
        <template v-slot:body="sharedSlotProps">
          <div v-if="sharedSlotProps.data.status === ShareStatus.Created">
            <ConfirmPopup group="templating">
              <template #message="sharedSlotProps">
                <div class="flex align-items-center w-full gap-2 border-bottom-1 surface-border p-3 mb-3 pb-1">
                  <i :class="sharedSlotProps.message.icon" class="text-2xl"></i>
                  <p>{{ sharedSlotProps.message.message }}</p>
                </div>
              </template>
            </ConfirmPopup>
            <div class="card flex justify-content-center">
              <Button @click="showTemplate($event, sharedSlotProps.data, sharedSlotProps.data)"
                      label="Подтвердить оплату"
                      severity="success"></Button>
            </div>
          </div>
        </template>
      </Column>
      <template v-if="userRole === UserRole.Admin" v-slot:expansion="slotProps">
        <div class="p-3">
          <DataTable :value="paymentSharesMap[slotProps.data.id]">
            <Column header="ФИО">
              <template v-slot:body="slotProps">
                {{
                  slotProps.data.user.lastName + ' ' + slotProps.data.user.firstName + ' ' + slotProps.data.user.patronymic
                }}
              </template>
            </Column>
            <Column field="amount" header="Сумма">
              <template v-slot:body="shareSlotProps">
                {{
                  getTruncatedAmount(shareSlotProps.data.amount, slotProps.data.currency.precision) + ' ' + slotProps.data.currency.shortName
                }}
              </template>
            </Column>
            <Column header="Доля">
              <template v-slot:body="slotProps">
                {{
                  getTruncatedAmount(slotProps.data.share, 2) + ' ' + "%"
                }}
              </template>
            </Column>
            <Column field="status" header="Статус">
              <template v-slot:body="slotProps">
                <Tag :value="getStatus(slotProps.data)" :severity="getShareStatusSeverity(slotProps.data)"/>
              </template>
            </Column>
            <Column>
              <template v-slot:body="sharedSlotProps">
                <div v-if="sharedSlotProps.data.status !== ShareStatus.Completed">
                  <ConfirmPopup group="templating">
                    <template #message="sharedSlotProps">
                      <div class="flex align-items-center w-full gap-2 border-bottom-1 surface-border p-3 mb-3 pb-1">
                        <i :class="sharedSlotProps.message.icon" class="text-2xl"></i>
                        <p>{{ sharedSlotProps.message.message }}</p>
                      </div>
                    </template>
                  </ConfirmPopup>
                  <div class="card flex justify-content-center">
                    <Button @click="showTemplate($event, sharedSlotProps.data, slotProps.data)"
                            :label="getButtonLabel(sharedSlotProps.data)"
                            :severity="getButtonSeverity(sharedSlotProps.data)"></Button>
                  </div>
                </div>
              </template>
            </Column>
          </DataTable>
        </div>
      </template>
    </DataTable>
    <Paginator :rows="pageSize" :totalRecords="totalPaymentsCount" :rowsPerPageOptions="[5, 10, 15]"
               @page="pageChange"></Paginator>
  </div>

  <Dialog v-if="userRole === UserRole.Admin" v-model:visible="isModalVisible" modal header="Добавление платежа"
          :draggable="false" :dismissableMask="true">
    <div class="flex align-items-center gap-3 mb-3">
      <label for="amount" class="font-semibold w-6rem">Сумма</label>
      <InputNumber v-model="newPayment.amount" id="amount" class="flex-auto" autocomplete="off"/>
    </div>
    <div class="flex align-items-center gap-3 mb-3">
      <label for="date" class="font-semibold w-6rem">Дата</label>
      <Calendar id="date" v-model="newPayment.date" date-format="dd.mm.yy" show-icon class="flex-auto"
                autocomplete="off"/>
    </div>
    <div class="flex align-items-center gap-3 mb-5">
      <label for="caption" class="font-semibold w-6rem">Комментарий</label>
      <InputText id="caption" v-model="newPayment.caption" class="flex-auto" autocomplete="off"/>
    </div>
    <div class="flex justify-content-end gap-2">
      <Button type="submit" label="Сохранить" @click="createPayment"></Button>
    </div>
  </Dialog>
</template>

<script setup lang="ts">
import {computed, ref} from 'vue';
import {paymentsService} from "@/services/payments.api.ts";
import {format} from 'date-fns'
import {CurrencyCode} from "@/enums/CurrencyCode.ts";
import {PaymentType} from "@/enums/PaymentType.ts";
import {CreatePayment} from "@/models/CreatePayment.ts";
import {PaymentShare} from "@/models/PaymentShare.ts";
import {UserRole} from "@/enums/UserRole.ts";
import {AdminPayment} from "@/models/AdminPayment.ts";
import {Payment} from "@/models/Payment.ts";
import {ShareStatus} from "@/enums/ShareStatus.ts";
import {useConfirm} from "primevue/useconfirm";
import ConfirmPopup from 'primevue/confirmpopup';

const isModalVisible = ref(false);
const expandedRows = ref({});
const paymentSharesMap = ref<{ [key: string]: PaymentShare[] }>({});
const selectedPaymentType = ref(PaymentType.Electricity);
const payments = ref<AdminPayment[] | Payment[]>();
const paymentShares = ref<PaymentShare[]>();
const totalPaymentsCount = ref(0);
const pageSize = ref(10);
const pageNumber = ref(1);
const userRole = ref(UserRole.User);
const confirm = useConfirm();

const newPayment = ref<CreatePayment>({
  caption: null,
  currencyCode: CurrencyCode.RUB,
  paymentType: PaymentType.Electricity,
  date: new Date(),
  amount: 0,
});

const paymentTypes = ref([
  {name: 'Электричество', value: 'Electricity'},
  {name: 'Выплаты', value: 'Crypto'},
  {name: 'Покупки', value: 'Purchase'}
]);

const pageChange = async (event) => {
  pageNumber.value = event.page + 1;
  pageSize.value = event.rows;
  await fetchPayments();
};

const getPaymentStatusSeverity = (data) => {
  return data.isCompleted ? 'success' : 'warning';
};

const getButtonLabel = (data) => {
  if (selectedPaymentType.value != PaymentType.Crypto) {
    switch (data.status) {
      case ShareStatus.Created:
        return 'Завершить оплату';

      case ShareStatus.Pending:
        return 'Подтвердить оплату';
    }
  } else {
    switch (data.status) {
      case ShareStatus.Created:
        return 'Подтвердить перевод';
    }
  }
};

const getButtonSeverity = (data) => {
  if (selectedPaymentType.value != PaymentType.Crypto) {
    switch (data.status) {
      case ShareStatus.Created:
        return 'secondary';

      case ShareStatus.Pending:
        return 'success';
    }
  } else {
    switch (data.status) {
      case ShareStatus.Created:
        return 'success';
    }
  }
};

const getShareStatusSeverity = (payment) => {
  if (selectedPaymentType.value != PaymentType.Crypto) {
    switch (payment.status) {
      case ShareStatus.Created:
        return 'danger';

      case ShareStatus.Pending:
        return 'warning';

      case ShareStatus.Completed:
        return 'success';
    }
  } else {
    switch (payment.status) {
      case ShareStatus.Created:
        return 'warning';

      case ShareStatus.Pending:
        return 'warning';

      case ShareStatus.Completed:
        return 'success';
    }
  }
};
const getStatus = (payment) => {
  if (selectedPaymentType.value != PaymentType.Crypto) {
    if (userRole.value != UserRole.Admin) {
      switch (payment.status) {
        case ShareStatus.Created:
          return "К оплате";

        case ShareStatus.Pending:
          return "Ожидание";

        case ShareStatus.Completed:
          return "Завершен";
      }
    } else {
      switch (payment.status) {
        case ShareStatus.Created:
          return "Не оплачен";

        case ShareStatus.Pending:
          return "Ожидает подтверждения";

        case ShareStatus.Completed:
          return "Оплачен";
      }
    }
  } else {
    switch (payment.status) {
      case ShareStatus.Created:
        return "Ожидание";

      case ShareStatus.Pending:
        return "Ожидание";

      case ShareStatus.Completed:
        return "Завершен";
    }
  }
};

const isCompletedHandle = (data) => {
  return data.isCompleted ? "Завершен" : "Ожидание";
}


const showTemplate = (event, data, sharedData) => {
  if (userRole.value == UserRole.Admin) {
    if (data.status == ShareStatus.Created && selectedPaymentType.value != PaymentType.Crypto) {
      confirm.require({
        target: event.currentTarget,
        group: 'templating',
        message: 'Вы хотите завершить платеж для этого участника?',
        icon: 'pi pi-question-circle',
        acceptIcon: 'pi pi-check',
        rejectIcon: 'pi pi-times',
        acceptLabel: 'Да',
        rejectLabel: 'Нет',
        rejectClass: 'p-button-secondary p-button-outlined p-button-sm',
        acceptClass: 'p-button-success p-button-sm',
        accept: async () => {
          await paymentsService.switchPaymentStatus(data.id);
          data.status = ShareStatus.Completed;
          const payment = payments.value?.find(payment => payment.id === sharedData.id) as AdminPayment;
          payment.isCompleted = isSharesCompleted(sharedData.id).value;
        }
      });
    } else if (selectedPaymentType.value == PaymentType.Crypto) {
      confirm.require({
        target: event.currentTarget,
        group: 'templating',
        message: 'Вы хотите подтвердить перевод средств участнику?',
        icon: 'pi pi-question-circle',
        acceptIcon: 'pi pi-check',
        rejectIcon: 'pi pi-times',
        acceptLabel: 'Да',
        rejectLabel: 'Нет',
        rejectClass: 'p-button-secondary p-button-outlined p-button-sm',
        acceptClass: 'p-button-success p-button-sm',
        accept: async () => {
          await paymentsService.switchPaymentStatus(data.id);
          data.status = ShareStatus.Completed;
          const payment = payments.value?.find(payment => payment.id === sharedData.id) as AdminPayment;
          payment.isCompleted = isSharesCompleted(sharedData.id).value;
        }
      });
    } else {
      confirm.require({
        target: event.currentTarget,
        group: 'templating',
        message: 'Вы подтверждаете перевод средств от участника?',
        icon: 'pi pi-question-circle',
        acceptIcon: 'pi pi-check',
        rejectIcon: 'pi pi-times',
        acceptLabel: 'Да',
        rejectLabel: 'Нет',
        rejectClass: 'p-button-secondary p-button-outlined p-button-sm',
        acceptClass: 'p-button-success p-button-sm',
        accept: async () => {
          await paymentsService.switchPaymentStatus(data.id);
          data.status = ShareStatus.Completed;
          const payment = payments.value?.find(payment => payment.id === sharedData.id) as AdminPayment;
          payment.isCompleted = isSharesCompleted(sharedData.id).value;
        }
      });
    }
  } else {
    confirm.require({
      target: event.currentTarget,
      group: 'templating',
      message: 'Вы действительно перевели средства?',
      icon: 'pi pi-question-circle',
      acceptIcon: 'pi pi-check',
      rejectIcon: 'pi pi-times',
      acceptLabel: 'Да',
      rejectLabel: 'Нет',
      rejectClass: 'p-button-secondary p-button-outlined p-button-sm',
      acceptClass: 'p-button-success p-button-sm',
      accept: async () => {
        await paymentsService.switchPaymentStatus(data.id);
        data.status = ShareStatus.Pending;
      }
    });
  }
}
const getDateOnly = (date) => {
  return format(date, 'dd.MM.yyyy');
};
const getTruncatedAmount = (value, precision) => {
  const factor = 10 ** precision;
  return Math.trunc(value * factor) / factor;
}

const getUserRole = () => {
  const jwt = localStorage.getItem('access_token');
  const decodedJwt = jwt && JSON.parse(atob(jwt.split('.')[1]));
  userRole.value = decodedJwt["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
};

const fetchShares = async (event) => {
  const paymentId = event.data.id;
  if (!paymentSharesMap.value[paymentId]) {
    paymentSharesMap.value[paymentId] = await paymentsService.getShares(paymentId);
  }
  paymentShares.value = paymentSharesMap.value[paymentId];

};

const fetchPayments = async () => {
  const response = await paymentsService.getPayments(pageNumber.value, pageSize.value, selectedPaymentType.value);
  payments.value = response.items;
  totalPaymentsCount.value = response.totalCount;
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
  };
  isModalVisible.value = false;
};

const isSharesCompleted = (paymentId) => {
  return computed(() => {
    const shares = paymentSharesMap.value[paymentId];
    if (!shares) {
      return false;
    }
    return shares.every(share => share.status === ShareStatus.Completed);
  });
};

getUserRole();
fetchPayments();


</script>

