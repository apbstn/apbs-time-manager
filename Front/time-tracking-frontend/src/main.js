import { createApp } from 'vue';
import App from './App.vue';
import router from './router';

import { definePreset } from '@primevue/themes';
import Material from '@primevue/themes/material';
import PrimeVue from 'primevue/config';

import 'primeicons/primeicons.css'

import InputText from 'primevue/inputtext';
import InputNumber from 'primevue/inputnumber';
import Button from 'primevue/button';

import VueTelInput from 'vue-tel-input';
import 'vue-tel-input/dist/vue-tel-input.css';

import DatePicker from 'primevue/datepicker';

import '@/assets/styles.scss';
import '@/assets/tailwind.css';
import Dropdown from 'primevue/dropdown'

import Select from 'primevue/select';

import Textarea from 'primevue/textarea';

import FloatLabel from 'primevue/floatlabel';


import Menu from 'primevue/menu';
import Vue3Toastify from 'vue3-toastify'
import 'vue3-toastify/dist/index.css'

import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import ColumnGroup from 'primevue/columngroup';   // optional
import Row from 'primevue/row';   

// Then register it:


const app = createApp(App);
app.use(router);
app.use(Vue3Toastify, {
  autoClose: 3000,
  position: 'top-right'
})
const MyPreset = definePreset(Material, {
    semantic: {
        primary: {
            50: '{indigo.50}',
            100: '{indigo.100}',
            200: '{indigo.200}',
            300: '{indigo.300}',
            400: '{indigo.400}',
            500: '{indigo.500}',
            600: '{indigo.600}',
            700: '{indigo.700}',
            800: '{indigo.800}',
            900: '{indigo.900}',
            950: '{indigo.950}'
        }
    }
});

app.use(PrimeVue, {
    ripple: true,
    inputVariant: 'filled',
    theme: {
        preset: MyPreset,
        options: {
            darkModeSelector: '.app-dark'
        }
    }
});

app.use(VueTelInput);

// ✅ Register PrimeVue components
app.component('InputText', InputText);
app.component('InputNumber', InputNumber);
app.component('Button', Button);
app.component('datapicker', DatePicker);
app.component('Dropdown', Dropdown);
app.component('Select', Select);
app.component('Textarea', Textarea);
app.component('FloatLabel', FloatLabel);
app.component('DataTable', DataTable);
app.component('Column', Column);
app.component('ColumnGroup', ColumnGroup);
app.component('Row', Row);
app.component('Menu', Menu);







app.mount('#app');
