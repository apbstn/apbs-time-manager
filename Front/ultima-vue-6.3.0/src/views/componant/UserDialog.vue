<!-- UserDialog.vue -->
<template>
  <Dialog 
    :visible="visible" 
    :header="isEdit ? 'Edit User' : 'Add User'" 
    :modal="true" 
    :style="{ width: '400px' }" 
    @update:visible="$emit('update:visible', $event)"
  >
    <div class="p-field">
      <label for="email">Email</label>
      <InputText 
        id="email" 
        v-model="form.email" 
        class="p-inputtext w-full" 
        :class="{ 'p-invalid': errors.email }" 
      />
      <small v-if="errors.email" class="p-error">{{ errors.email }}</small>
    </div>
    <div class="p-field">
      <label for="username">Username</label>
      <InputText 
        id="username" 
        v-model="form.username" 
        class="p-inputtext w-full" 
        :class="{ 'p-invalid': errors.username }" 
      />
      <small v-if="errors.username" class="p-error">{{ errors.username }}</small>
    </div>
    <div class="p-field">
      <label for="phoneNumber">Phone Number (Optional)</label>
      <InputText 
        id="phoneNumber" 
        v-model="form.phoneNumber" 
        class="p-inputtext w-full" 
      />
    </div>
    <template #footer>
      <Button label="Cancel" icon="pi pi-times" class="p-button-text" @click="$emit('close')" />
      <Button 
        :label="isEdit ? 'Save' : 'Add'" 
        icon="pi pi-check" 
        class="p-button-primary" 
        @click="submit" 
        :loading="loading"
      />
    </template>
  </Dialog>
</template>

<script setup>
import { ref, watch } from 'vue';
import axios from 'axios';
import Dialog from 'primevue/dialog';
import InputText from 'primevue/inputtext';
import Button from 'primevue/button';

defineProps({
  visible: Boolean,
  isEdit: Boolean,
  user: Object,
  currentId: [Number, String, null]
});

const emit = defineEmits(['update:visible', 'refresh', 'close']);

const form = ref({
  email: '',
  username: '',
  phoneNumber: ''
});
const errors = ref({});
const loading = ref(false);

watch(
  () => props.user,
  (newUser) => {
    if (newUser) {
      form.value = { ...newUser };
    } else {
      form.value = { email: '', username: '', phoneNumber: '' };
    }
    errors.value = {};
  },
  { immediate: true }
);

const validate = () => {
  errors.value = {};
  if (!form.value.email) errors.value.email = 'Email is required';
  else if (!/\S+@\S+\.\S+/.test(form.value.email)) errors.value.email = 'Invalid email format';
  if (!form.value.username) errors.value.username = 'Username is required';
  return Object.keys(errors.value).length === 0;
};

const submit = async () => {
  if (!validate()) return;

  try {
    loading.value = true;
    const token = localStorage.getItem('accessToken');
    if (!token) throw new Error('No access token found');

    if (props.isEdit) {
      await axios.put(`http://localhost:58169/api/user/${props.currentId}`, form.value, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
    } else {
      await axios.post('http://localhost:58169/api/user', form.value, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
    }
    emit('refresh');
    emit('close');
  } catch (err) {
    console.error('Error saving user:', err);
    errors.value.form = 'Failed to save user: ' + 
      (err.response?.data?.message || err.message || 'Unknown error');
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.p-field {
  margin-bottom: 1rem;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
}

.p-inputtext {
  width: 100%;
}

.p-error {
  color: #dc3545;
}
</style>