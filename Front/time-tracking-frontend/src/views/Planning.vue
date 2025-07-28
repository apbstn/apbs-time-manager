<template>
  <h2 style="font-size: 22px; color: #6B7280;">Plan Of Work</h2>
  <div class="max-w-4xl mx-auto p-6 bg-white rounded-2xl shadow-2xl border border-gray-100 mt-10">
    <!-- Header -->
    <div class="flex justify-between items-center mb-6">
      <div class="space-y-1">
        <InputText v-if="isEditing" v-model="scheduleName" placeholder="Nom de l’horaire"
          class="text-xl font-bold text-slate-700 border-b border-slate-300 focus:outline-none focus:border-blue-500" />
        <h2 v-else class="text-2xl font-bold text-slate-700">
          {{ scheduleName }}
          <span class="ml-2 text-xs bg-slate-200 text-slate-600 px-2 py-0.5 rounded-full">Default</span>
        </h2>
        <div v-if="!isEditing && savedDate" class="text-xs text-gray-400">Created At {{ formatDate(savedDate) }}</div>
      </div>
      <Button v-if="isEditing" @click="saveSchedule" label="Save" icon="pi pi-check" class="add-button" iconPos="right" />
      <Button v-else @click="toggleEdit" label="Update" icon="pi pi-refresh" class="add-button1" iconPos="right" />
    </div>

    <!-- Work Type Selector -->
    <div class="mb-4 text-sm text-slate-600">
      <span class="font-medium">Working conditions :</span>
      <span v-if="!isEditing" class="ml-1 text-slate-800 font-semibold">{{ workType }}</span>
      <Dropdown v-else v-model="workType" :options="['Fixe', 'Flexible', 'Hebdomadaire']"
        class="ml-2 px-2 py-1 border rounded focus:outline-none focus:ring-2 focus:ring-blue-400" />
    </div>

    <!-- Day Selector -->
    <div v-if="isEditing && workType !== 'Hebdomadaire'" class="flex gap-2 mb-4">
      <Button v-for="day in days" :key="day.key" @click="toggleDay(day.key)" :label="day.key" :class="['w-10 h-10 rounded-lg font-medium',
        selectedDays.includes(day.key)
          ? 'bg-green-500 text-white'
          : 'bg-white border text-slate-600 hover:bg-orange-100'
      ]" class="day-button" />
    </div>

    <!-- Time or Hours Input -->
    <div class="space-y-3 text-sm">
      <div v-if="workType === 'Hebdomadaire'" class="flex items-center gap-3">
        <label class="text-slate-700 font-medium">Hours per week :</label>
        <InputNumber v-if="isEditing" v-model="weeklyHours" :min="0" class="flex items-center" />
        <span v-else class="text-slate-800 font-semibold">{{ weeklyHours }} h</span>
      </div>

      <div v-for="day in days" :key="day.key" v-if="workType !== 'Hebdomadaire'"
        class="flex items-center justify-between px-4 py-3 rounded-lg"
        :class="selectedDays.includes(day.key) ? 'bg-gray-50 border border-gray-200' : ''">
        <div class="flex items-center">{{ day.name }}</div>

        <template v-if="selectedDays.includes(day.key)">
          <!-- FLEXIBLE MODE: input hours & minutes -->
          <div v-if="workType === 'Flexible' && isEditing" class="flex items-center justify-end">
            <div class="flex items-center">
              <InputNumber v-model="flexibleHours[day.key].hours" :min="0" :max="23" suffix=" h" class="" />
            </div>
            <div class="flex items-center ml-2">
              <InputNumber v-model="flexibleHours[day.key].minutes" :min="0" :max="59" suffix=" m" class="" />
            </div>
          </div>

          <div v-else-if="workType === 'Flexible'" class="text-slate-800 font-semibold">
            {{ flexibleHours[day.key].hours }}h {{ flexibleHours[day.key].minutes }}m
          </div>

          <!-- FIXE MODE: start & end time -->
          <div v-else-if="workType === 'Fixe' && isEditing" class="flex gap-2 items-center">
            <div class="flex items-center ml-2">
              <InputText type="time" v-model="fixedHours[day.key].start" class="" />
            </div>
            <div class="flex items-center ml-2">
              <span class="text-slate-500">to</span>
            </div>
            <div class="flex items-center ml-2">
              <InputText type="time" v-model="fixedHours[day.key].end" class="" />
            </div>
          </div>
          <div v-else-if="workType === 'Fixe'" class="text-slate-800 font-semibold">
            {{ fixedHours[day.key].start }} – {{ fixedHours[day.key].end }}
          </div>
        </template>

        <template v-else>
          <div class="text-slate-400 italic">Day of rest</div>
        </template>
      </div>
    </div>

    <!-- Footer -->
    <div class="mt-6 pt-4 border-t text-sm text-slate-600">
      <span class="font-medium text-slate-700">Payroll hours :</span>
      Include time tracked before the scheduled start time
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import api from '@/api';



const scheduleId = ref(null)
const isEditing = ref(false)
const scheduleName = ref('Horaire de travail')
const savedDate = ref('')
const workType = ref('Fixe')
const weeklyHours = ref(40)

const days = [
  { key: 'M', name: 'Monday' },
  { key: 'T', name: 'Tuesday' },
  { key: 'W', name: 'Wednesday' },
  { key: 'Th', name: 'Thursday' },
  { key: 'F', name: 'Friday' },
  { key: 'S', name: 'Saturday' },
  { key: 'Su', name: 'Sunday' }
]

const selectedDays = ref(['M', 'T', 'W', 'Th', 'F'])

const flexibleHours = reactive({
  M: { hours: 8, minutes: 0 },
  T: { hours: 8, minutes: 0 },
  W: { hours: 8, minutes: 0 },
  Th: { hours: 8, minutes: 0 },
  F: { hours: 8, minutes: 0 },
  S: { hours: 0, minutes: 0 },
  Su: { hours: 0, minutes: 0 }
})

const fixedHours = reactive({
  M: { start: '08:00', end: '16:00' },
  T: { start: '08:00', end: '16:00' },
  W: { start: '08:00', end: '16:00' },
  Th: { start: '08:00', end: '16:00' },
  F: { start: '08:00', end: '16:00' },
  S: { start: '', end: '' },
  Su: { start: '', end: '' }
})

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString()
}

const fetchSchedule = async () => {
  try {
    const response = await api.get('/api/schedules/1')
    const data = response.data
    console.log("reponse : ---------------------------------", response.data)
    if (data) {
      workType.value= data.workType
      console.log("work type : --------------------", workType.value)
      scheduleId.value = data.id
      scheduleName.value = data.name
      selectedDays.value = data.selectedDays
      weeklyHours.value = data.weeklyHours
      savedDate.value = data.createdAt
      Object.assign(flexibleHours, data.flexibleHours)
      Object.assign(fixedHours, data.fixedHours)
    }
  } catch (error) {
    console.error('Error fetching schedule:', error)
  }
}

const saveSchedule = async () => {
  const payload = {
    name: scheduleName.value,
    workType: workType.value,
    selectedDays: selectedDays.value,
    flexibleHours,
    fixedHours,
    weeklyHours: weeklyHours.value
  }

  try {
    let response
    if (scheduleId.value) {
      response = await api.put(`/api/schedules/${scheduleId.value}`, payload)
    } else {
      response = await api.post('/api/schedules', payload)
      scheduleId.value = response.data.id
    }
    savedDate.value = response.data.createdAt || new Date().toISOString()
    isEditing.value = false
    console.log('Saved schedule:', response.data)
  } catch (error) {
    console.error('Error saving schedule:', error)
  }
}

const toggleEdit = () => {
  isEditing.value = !isEditing.value
}

const toggleDay = (dayKey) => {
  const index = selectedDays.value.indexOf(dayKey)
  if (index === -1) {
    selectedDays.value.push(dayKey)
  } else {
    selectedDays.value.splice(index, 1)
  }
}

onMounted(fetchSchedule)
</script>

<style scoped>
.add-button {
  border-radius: 6px;
  padding: 0.5rem 1rem;
  font-weight: 500;
  transition: background-color 0.2s, transform 0.1s;
  border: 1px solid #35D300;
  color: #35D300;
  background-color: transparent !important;
}

.add-button:hover {
  transform: translateY(-1px);
  background-color: #35D300 !important;
  color: white !important;
  border-color: white !important;
}

.add-button1 {
  border-radius: 6px;
  padding: 0.5rem 1rem;
  font-weight: 500;
  transition: background-color 0.2s, transform 0.1s;
  border: 1px solid #ff0000 !important;
  color: #ff0000 !important;
  background-color: transparent !important;
}

.add-button1:hover {
  transform: translateY(-1px);
  background-color: #ff0000 !important;
  color: white !important;
  border-color: white !important;
}

.day-button {
  border-radius: 6px;
  font-weight: 500;
  transition: background-color 0.2s, transform 0.1s;
  border: 1px solid #35D300 !important;
  color: #35D300 !important;
  background-color: transparent !important;
}

.day-button:hover:not(.bg-green-500) {
  transform: translateY(-1px);
  background-color: #35D300 !important;
  color: white !important;
  border-color: white !important;
}

.day-button.bg-green-500 {
  background-color: #35D300 !important;
  color: white !important;
  border-color: white !important;
}
</style>