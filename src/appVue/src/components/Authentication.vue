<script setup lang="ts">
import {ref, computed} from 'vue';
import {LoginUser} from '@/models/LoginUser.ts';
import {usersService} from '@/services/users.api';
import InputGroup from "primevue/inputgroup";
import InputGroupAddon from "primevue/inputgroupaddon";

const user = ref<LoginUser>({
  email: '',
  password: '',
});

const isValid = computed(() => {
  return user.value.email && user.value.password;
});

async function login() {
  await usersService.loginUser(user.value);
}
</script>

<template>
  <div class="card flex justify-content-center">
    <InputGroup>

      <InputGroupAddon>
        <label for="email">EMail</label>
        <InputText id="email" v-model="user.email"/>
      </InputGroupAddon>

      <InputGroupAddon>
        <label for="password">Пароль</label>
        <Password id="password" v-model="user.password" toggleMask/>
      </InputGroupAddon>

      <InputGroupAddon>
        <Button :disabled="!isValid" type="submit" @click="login">Войти</Button>
      </InputGroupAddon>
    </InputGroup>
  </div>
</template>