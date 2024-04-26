<template>
  <Menu :model="items">
    <template #item="{ item, props }">
      <router-link v-slot="{ href, navigate }" :to="{ name: item.route }" custom>
        <a :href="href" v-bind="props.action" @click="navigate">
          <span :class="item.icon"/>
          <span class="ml-2">{{ item.label }}</span>
        </a>  
      </router-link>
    </template>
  </Menu>

  <div class="content">
    <router-view></router-view>
  </div>
</template>

<script setup lang="ts">
import {ref} from "vue";

const items = ref([
  {
    label: 'Создание аккаунта',
    icon: 'pi pi-plus',
    route: 'register',
  },
  {
    label: 'Вход',
    icon: 'pi pi-search',
    route: 'login',
  },
  {
    label: 'Платежи',
    icon: 'pi pi-search',
    route: 'payments',
  }
]);

</script>