<!-- src/components/CelebrityMultiselect.vue -->
<script setup>
import { ref, watch, onMounted } from 'vue'
import Multiselect from 'vue-multiselect'
import 'vue-multiselect/dist/vue-multiselect.min.css'
import { authApi } from '@/services/auth'

// --- Props & Emits
const props = defineProps({
  modelValue: {
    type: Array,
    default: () => []
  },
  pageSize: {
    type: Number,
    default: 10
  }
})
const emit = defineEmits(['update:modelValue'])

// --- Local state
const searchQuery = ref('')            // what the user types
const options     = ref([])            // array of { celebrityId, fullName, birthYear }
const isLoading   = ref(false)
const pageId      = ref(1)
const pagesCount  = ref(0)
const selected    = ref([])            // array of full objects

// Mirror selected.ids ↔ selected objects
watch(selected, (newList) => {
  emit('update:modelValue', newList.map(x => x.celebrityId))
})

// When parent changes the v-model of IDs, rehydrate objects (optional)
watch(() => props.modelValue, (ids) => {
  selected.value = options.value.filter(o => ids.includes(o.celebrityId))
})

// --- Fetch function
async function fetchCelebrities(name, page = 1) {
  isLoading.value = true
  try {
    const res = await authApi.get('/api/GetAllCelebrities', {
      params: { filterName: name, pageId: page, pageSize: props.pageSize }
    })
    options.value   = res.data.celebrities
    pagesCount.value = res.data.pagesCount
    pageId.value     = page
  } finally {
    isLoading.value = false
  }
}

// --- Trigger fetch when searchQuery changes (debounced if you want)
watch(searchQuery, (q) => {
  fetchCelebrities(q, 1)
})

// initial load (empty search)
onMounted(() => fetchCelebrities('', 1))

// --- Pagination handlers (if you want “load more”)
function nextPage() {
  if (pageId.value < pagesCount.value) {
    fetchCelebrities(searchQuery.value, pageId.value + 1)
  }
}
</script>

<template>
  <div class="space-y-2">
    <Multiselect
      v-model="selected"
      :options="options"
      :multiple="true"
      :loading="isLoading"
      :searchable="true"
      :close-on-select="false"
      placeholder="Search celebrities..."
      label="fullName"
      track-by="celebrityId"
      @search-change="searchQuery = $event"
    />

    <!-- optional “load more” if you want infinite scroll or manual -->
    <div class="flex justify-center">
      <button
        class="px-2 py-1 bg-gray-200 rounded disabled:opacity-50"
        @click="nextPage"
        :disabled="isLoading || pageId === pagesCount"
      >
        Load More ({{ pageId }} / {{ pagesCount }})
      </button>
    </div>
  </div>
</template>
