<template>
  <div class="w-8">
  <Toolbar class="mb-4 border-none">
    <template #end>
      <Button class="mr-2" icon="pi pi-plus" label="Добавить депозит" severity="success"
              @click="isModalVisible = true"/>
    </template>
  </Toolbar>
    <DataTable v-model:expandedRows="expandedRows" :value="members" dataKey="user.id"
               @rowExpand="fetchDeposits">
      <Column expander/>
      <Column header="ФИО">
        <template v-slot:body="slotProps">
          {{
            slotProps.data.user.lastName + ' ' + slotProps.data.user.firstName + ' ' + slotProps.data.user.patronymic
          }}
        </template>
      </Column>
      <Column header="Доля участника">
        <template v-slot:body="slotProps">
          {{
            getTruncatedAmount(slotProps.data.share, 2) + ' ' + "%"
          }}
        </template>
      </Column>
      <Column header="Сумма инвестиций">
        <template v-slot:body="slotProps">
          {{
            getTruncatedAmount(slotProps.data.amount, 2) + ' ' + "₽"
          }}
        </template>
      </Column>
      <Column field="registrationDate" header="Дата регистрации">
        <template v-slot:body="slotProps">
          {{ getDateOnly(slotProps.data.registrationDate) }}
        </template>
      </Column>
      <template v-slot:expansion="slotProps">
        <div class="flex justify-content-center p-3">
          <DataTable class="w-6" :value="depositsMap[slotProps.data.user.id]"
                     :sortField="'date'"
                     :sortOrder="-1">
            <Column field="date" header="Дата">
              <template v-slot:body="slotProps">
                {{ getDateOnly(slotProps.data.date) }}
              </template>
            </Column>
            <Column field="amount" header="Сумма">
              <template v-slot:body="slotProps">
                {{
                  getTruncatedAmount(slotProps.data.amount, 2) + ' ' + "₽"
                }}
              </template>
            </Column>
          </DataTable>
        </div>
      </template>
    </DataTable>
  </div>

  <Dialog v-model:visible="isModalVisible" :dismissableMask="true" :draggable="false"
          header="Добавление депозита участнику" modal>
    <div class="flex align-items-center gap-3 mb-5">
      <label class="font-semibold w-6rem" for="caption">Участник</label>
      <Dropdown v-model="selectedMember" :options="members" class="flex-auto" :optionLabel="memberFullName" 
                @change="assignId"/>
    </div>
    <div class="flex align-items-center gap-3 mb-3">
      <label class="font-semibold w-6rem" for="date">Дата</label>
      <Calendar id="date" v-model="newDeposit.date" class="flex-auto" date-format="dd.mm.yy" 
                show-icon showTime/>
    </div>
    <div class="flex align-items-center gap-3 mb-3">
      <label class="font-semibold w-6rem" for="amount">Сумма</label>
      <InputNumber id="amount" v-model="newDeposit.amount" autocomplete="off" class="flex-auto"/>
    </div>
    <div class="flex justify-content-end gap-2">
      <Button label="Сохранить" type="submit" @click="createDeposit"></Button>
    </div>
  </Dialog>
  
</template>

<script setup lang="ts">
import {ref} from "vue";
import {format} from "date-fns";
import {Member} from "@/models/Member.ts";
import {Deposit} from "@/models/Deposit.ts";
import {membersService} from "@/services/members.api.ts";

const isModalVisible = ref(false);
const expandedRows = ref({});
const depositsMap = ref<{ [key: string]: Deposit[] }>({});
const members = ref<Member[]>();
const deposits = ref<Deposit[]>();
const selectedMember = ref<Member>();
const newDeposit = ref<Deposit>({
  userId: "",
  date: new Date(),
  amount: 0
});

const assignId = () => {
  if (selectedMember.value) {
    newDeposit.value.userId = selectedMember.value.user.id;
  } else {
    newDeposit.value.userId = "";
  }
};

const memberFullName = (member: Member) => {
  if (member && member.user) {
    return `${member.user.lastName} ${member.user.firstName} ${member.user.patronymic}`;
  }
  return '';
};

const getDateOnly = (date: Date) => {
  return format(date, 'dd.MM.yyyy');
};

const getTruncatedAmount = (value: number, precision: number) => {
  const factor = 10 ** precision;
  return Math.trunc(value * factor) / factor;
}

const fetchDeposits = async (event: any) => {
  const memberId = event.data.user.id;
  if (!depositsMap.value[memberId]) {
    depositsMap.value[memberId] = await membersService.getDeposits(memberId);
  }
  deposits.value = depositsMap.value[memberId];
};

const fetchMembers = async () => {
  members.value = await membersService.getMembers();
};

const createDeposit = async () => {
  await membersService.createDeposit(newDeposit.value);
  const memberId = newDeposit.value.userId;
  
  await fetchMembers();

  if (!depositsMap.value[memberId]) {
    depositsMap.value[memberId] = [];
  }
  const newDepositData = { ...newDeposit.value };
  depositsMap.value[memberId].unshift(newDepositData);

  deposits.value = depositsMap.value[memberId];
  isModalVisible.value = false;
  
  newDeposit.value = {
    userId: "",
    date: new Date(),
    amount: 0
  };
  selectedMember.value = undefined;
};

fetchMembers();

</script>
