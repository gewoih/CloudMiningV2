<template>
	<div class="container mt-5">
	  <div class="row justify-content-center">
		<div class="col-md-6">
		  <form @submit.prevent="register" class="card p-4 shadow">
			<h2 class="mb-4">Регистрация</h2>
  
			<div class="mb-2">
			  <label for="firstName" class="form-label">Имя</label>
			  <input type="text" class="form-control" id="firstName" v-model="user.firstName" required>
			</div>
  
			<div class="mb-2">
			  <label for="lastName" class="form-label">Фамилия</label>
			  <input type="text" class="form-control" id="lastName" v-model="user.lastName" required>
			</div>
  
			<div class="mb-2">
			  <label for="patronymic" class="form-label">Отчество</label>
			  <input type="text" class="form-control" id="patronymic" v-model="user.patronymic" required>
			</div>
  
			<div class="mb-2">
			  <label for="email" class="form-label">Email</label>
			  <input type="email" class="form-control" id="email" v-model="user.email" required>
			</div>
  
			<div class="mb-2">
			  <label for="password" class="form-label">Пароль</label>
			  <input type="password" class="form-control" id="password" v-model="user.password" required>
			</div>
  
			<div class="mb-4">
			  <label for="confirmPassword" class="form-label">Подтверждение пароля</label>
			  <input type="password" class="form-control" id="confirmPassword" v-model="user.confirmPassword" required>
			</div>
  
			<button type="submit" :disabled="!isValid" class="btn btn-primary">Создать аккаунт</button>
		  </form>
		</div>
	  </div>
	</div>
</template>

<script lang="ts">
	export default {
		name: 'Registration',
		data() {
			return {
				user: {
					firstName: '',
					lastName: '',
					patronymic: '',
					email: '',
					password: '',
					confirmPassword: ''					
				}
			};
		},
		
		computed: {
			isValid() {
				return this.user.firstName && 
				this.user.lastName && 
				this.user.patronymic && 
				this.user.email && 
				this.user.password && 
				this.user.password === this.user.confirmPassword;
			}
		},
		
		methods: {
			register() {
				alert('Пользователь зарегестрирован!' + JSON.stringify(this.user));
			}
		}
	}
</script>
