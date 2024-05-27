<template>
  <div class="h-screen overflow-auto flex flex-column">
    <div class="m-3">
      <Menubar :model="items">
        <template #start>
          <i class="pi pi-bitcoin ml-2" style="font-size: 2.5rem"/>
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
          <div class="flex justify-content-center mr-2">
            <Button aria-controls="menu" aria-haspopup="true" icon="pi pi-user" outlined rounded
                    type="button" @click="toggleMenu"/>
            <Menu id="menu" ref="menu" :model="settingsMenuItems" :popup="true"></Menu>
          </div>
        </template>
      </Menubar>
    </div>

    <div class="flex flex-1 align-items-center justify-content-center">
      <router-view></router-view>
    </div>
  </div>
</template>

<script lang="ts" setup>
import {ref} from "vue";
import router from "@/router.ts";
import Menu from "primevue/menu";

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

const settingsMenuItems = ref([
  {
    label: 'Регистрация', icon: 'pi pi-user-plus', command: () => {
      router.push({name: "register"});
    }
  },
  {
    label: 'Вход', icon: 'pi pi-sign-in', command: () => {
      router.push({name: "login"});
    }
  },
  // {
  //   label: 'Профиль', icon: 'pi pi-user-edit', command: () => {
  //     router.push({name: "register"});
  //   }
  // },
  // {
  //   label: 'Настройки', icon: 'pi pi-cog', command: () => {
  //     router.push({name: "register"});
  //   }
  // },
  // {
  //   label: 'Выйти', icon: 'pi pi-sign-out', command: () => {
  //     router.push({name: "register"});
  //   }
  // }
]);

const menu = ref();
const toggleMenu = (event) => {
  menu.value.toggle(event);
};
</script>
