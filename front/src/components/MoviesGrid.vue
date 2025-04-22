<script setup>
import { ref, reactive, onMounted, watch } from "vue";
import axios from "axios";
import MovieCard from "@/components/MovieCard.vue";
import PulseLoader from "vue-spinner/src/PulseLoader.vue";

const props = defineProps({
  limit: { type: Number, default: 3 },
  paginationEnabled: { type: Boolean, default: false },
  searchEnabled: { type: Boolean, default: false },
});

const state = reactive({
  movies: [],
  isLoading: false,
  currentPage: 1,
  totalPages: 1,
  searchInupt: '',
  selectedGenreId: null
});

const genres = ref([])

async function loadGenres(params) {
  try {
    const res = await axios.get('https://localhost:7214/api/GetAllGenres');
    genres.value = res.data;
  } catch (error) {
    console.error('Error loading genres')
  }
}

async function loadMovies() {
  state.isLoading = true;
  try {
    const res = await axios.get(`https://localhost:7214/api/GetMovies`, {
      params: {
        pageId: state.currentPage,
        pageSize: props.limit,
        titleFilter: state.searchInupt,
        genreId: state.selectedGenreId
      },
    });
    state.movies = res.data.movies;
    state.totalPages = res.data.pagesCount;
  } catch (err) {
    console.error("Error loading movies:", err);
  } finally {
    state.isLoading = false;
  }
}

// initial load
onMounted(async () => {
  await loadGenres()
  await loadMovies()
});

// whenever page changes, reload
watch(
  () => [state.currentPage, state.selectedGenreId],
  () => loadMovies()
)

function prevPage() {
  if (state.currentPage > 1) {
    state.currentPage--;
  }
}
function nextPage() {
  if (state.currentPage < state.totalPages) {
    state.currentPage++;
  }
}
</script>

<template>
  <section class="bg-yellow-50 px-4 py-10">
    <div class="container-xl lg:container m-auto">
      <h2 class="text-3xl font-bold text-yellow-400 mb-6 text-center">
        Browse Movies
      </h2>

      <div v-if="searchEnabled" class="mb-5">
        <form class="max-w-xl mx-auto" @submit.prevent="loadMovies">
  <div class="flex rounded-lg shadow-sm">
    <!-- Dropdown -->
    <div class="relative flex-shrink-0">
      <select v-model="state.selectedGenreId" class="py-2 pl-3 pr-8 text-sm font-medium text-gray-600 bg-gray-50 border border-r-0 border-gray-300 rounded-l-lg hover:bg-gray-100 focus:z-10 focus:outline-none focus:ring-1 focus:ring-blue-500 focus:border-blue-500">
        <option value="" :selected="true">All Categories</option>
        <option v-for="g in genres" :key="g.genreId" :value="g.genreId">{{ g.title }}</option>
      </select>
    </div>

    <!-- Search Input -->
    <div class="relative flex-1">
      <input v-model="state.searchInupt"
        type="text" 
        placeholder="Search..." 
        class="block w-full py-2 pl-4 pr-10 text-sm border border-l-0 border-gray-300 rounded-r-lg focus:outline-none focus:ring-1 focus:ring-blue-500 focus:border-blue-500"
      >
      <!-- Search Icon -->
      <button class="absolute inset-y-0 right-0 flex items-center px-3 bg-red-100 rounded" type="submit">
        <svg class="w-5 h-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"/>
        </svg>
      </button>
    </div>
  </div>
</form>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div v-if="state.isLoading" class="col-span-full text-center">
          <PulseLoader />
        </div>
        <div
          v-else
          v-for="movie in state.movies"
          :key="movie.id"
          class="bg-white rounded-xl shadow-md"
        >
          <MovieCard :movie="movie" />
        </div>
      </div>

      <!-- Pagination controls -->
      <div
        v-if="paginationEnabled"
        class="flex justify-center items-center space-x-4 mt-8"
      >
        <button
          @click="prevPage"
          :disabled="state.currentPage === 1 || state.isLoading"
          class="px-4 py-2 rounded bg-gray-200 disabled:opacity-50"
        >
          Previous
        </button>

        <span> Page {{ state.currentPage }} of {{ state.totalPages }} </span>

        <button
          @click="nextPage"
          :disabled="state.currentPage === state.totalPages || state.isLoading"
          class="px-4 py-2 rounded bg-gray-200 disabled:opacity-50"
        >
          Next
        </button>
      </div>
    </div>
  </section>
</template>
