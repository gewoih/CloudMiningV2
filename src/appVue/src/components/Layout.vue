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
import {computed, ref} from "vue";
import router from "@/router.ts";
import Menu from "primevue/menu";
import {useUserStore} from "@/stores/user.ts";

const menu = ref();
const userStore = useUserStore();

const items = computed(() => {
  return [
    {
      label: 'Главная',
      icon: 'pi pi-chart-line',
      route: 'home',
      visible: userStore.isAuthenticated
    },
    {
      label: 'Участники',
      icon: 'pi pi-users',
      route: 'members',
      visible: userStore.isAdmin && userStore.isAuthenticated
    },
    {
      label: 'Платежи',
      icon: 'pi pi-wallet',
      route: 'payments',
      visible: userStore.isAuthenticated
    }
  ].filter(item => item.visible);
});

const settingsMenuItems = computed(() => {
  return [
    {
      label: 'Добавить пользователя',
      icon: 'pi pi-user-plus',
      command: () => {
        router.push({name: "register"});
      },
      visible: userStore.isAdmin
    },
    {
      label: enterButtonLabels,
      icon: 'pi pi-sign-in',
      command: () => {
        router.push({name: "login"});
        userStore.setToken(null);
      },
      visible: true
    }
  ].filter(item => item.visible);
});

const toggleMenu = (event: any) => {
  menu.value.toggle(event);
};
const enterButtonLabels = () => {
  if (userStore.isAuthenticated) {
    return "Сменить пользователя"
  } else
    return "Вход"
}
</script>
