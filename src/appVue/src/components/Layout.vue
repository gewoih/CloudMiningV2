<template>
  <div class="m-4">
    <Menubar :model="items">
      <template #start>
        <i class="pi pi-bitcoin" style="font-size: 2.5rem"/>
      </template>

      <template #item="{ item, props }">
        <router-link v-slot="{ navigate }" :to="{ name: item.route }" custom>
          <a class="flex align-items-center" v-bind="props.action" @click="navigate">
            <span :class="item.icon"/>
            <span class="ml-2">{{ item.label }}</span>
          </a>
        </router-link>
      </template>

      <template #end>
          <div>
            <Button icon="pi pi-user" severity="" rounded outlined dropdown>
              <template #dropdown>
                  <Menu>
                    <menubar command="" @click="">Регистрация</menubar>
                    <menubar command="" @click="">Вход</menubar>
                    <menubar command="" @click="">Профиль</menubar>
                    <menubar command="" @click="">Настройки</menubar>
                    <menubar command="" @click="">Выход</menubar>
                  </Menu>
              </template>
            </Button>
          </div>
      </template>
    </Menubar>
  </div>

  <div class="content">
    <router-view></router-view>
  </div>
</template>

<script setup lang="ts">
import {ref} from "vue";
import router from "@/router";

const items = ref([
  {
    label: 'Статистика',
    icon: 'pi pi-chart-line',
    route: 'payments',
  },
  {
    label: 'Участники',
    icon: 'pi pi-users',
    route: 'payments',
  },
  {
    label: 'Платежи',
    icon: 'pi pi-wallet',
    route: 'payments',
  }
]);
</script>
