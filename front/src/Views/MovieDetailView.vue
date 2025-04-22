<script setup>
import { onMounted, reactive, ref, computed } from "vue";
import { useRoute, RouterLink, useRouter } from "vue-router";
import axios from "axios";
import PulseLoader from "vue-spinner/src/PulseLoader.vue";
import { useToast } from "vue-toastification";
import { authApi, isAuthenticated, isAdmin } from "@/services/auth";
import Rating from 'primevue/rating';


const state = reactive({
  movie: {},
  isLoading: true,
});

const route = useRoute();
const router = useRouter();
const toast = useToast();

const populateMovie = async () => {
  try {
    const res = await axios.get(`https://localhost:7214/api/GetMovies/${route.params.id}`);
    state.movie = res.data;
  } catch (error) {
    console.error("Error from Mount JobDetailView.vue: " + error);
  } finally {
    state.isLoading = false;
  }
}
let existingScore = ref(0);
const getExistingScore = async () => {
  try {
    const res = await authApi.get(`/api/GetUserMovieScore?movieId=${route.params.id}`)
    if (res.status == 404) toast.error('Movie Not Found')
    existingScore.value = Number(res.data);
  } catch (error) {
    toast.error('Faild to get user score for this movie')
    console.error('Error in getExistingScore from MovieDetailView.vue: ', error)
  }
}

onMounted(async () => {
  await populateMovie();
  if (isAuthenticated.value) {
    await getExistingScore();
  }
});
const scoreVal = computed(() => existingScore.value)
const handleScoreSubmit = async () => {
  try {
    console.log(scoreVal.value)
    const res = await authApi.post('/api/ScoreMovie', { movieId: state.movie.movieId, score: scoreVal.value })
    if (res.status == 200) {
      toast.success('Successfully Submitted score')
    }
  } catch (error) {
    toast.error('Faild to submit score')
    console.error('Error from handleScoreSubmit MovieDetailView.vue: ', error)
  }
}

const handleDelete = async () => {
  try {
    const confirm = window.confirm("Are U sure U wanna DELETE this Job?");
    if (confirm) {
      // const res = await axios.delete(`/api/jobs/${route.params.id}`)
      // if (res.status >= 200 && res.status < 400) {
      //   toast.success('successfully deleted job')
      //   router.push('/jobs')
      // }
      // else {
      // }
      toast.error("Not Implemented");
    }
  } catch (error) {
    toast.error("Deleting job faild");
    console.log("Error from Delete JobDetailView.vue: ", error);
  }
};

const handleAddToWatchlist = async () => {
  try {
    const res = await authApi.post(`/api/AddMovieToWatchlist?movieId=${state.movie.movieId}`)
    toast.success('Successfully Added Movie to Watchlist')
  } catch (error) {
    if (error.response?.status == 404) {
      toast.error('Movie not found in Database')
    }
    else {
      console.log('Error Adding Movie to Watchlist: ' + error.response?.data)
    }
  }
}
</script>
<template>
  <!-- Go Back -->
  <section class="bg-yellow-100">
    <div class="container m-auto py-6 px-6 bg-yellow-50 flex justify-between">
      <RouterLink
        to="/movies"
        class="hover:text-gray-500 text-black flex items-center"
      >
        <i class="fas fa-arrow-left mr-2"></i> Back to Movie Listings
      </RouterLink>


      <button type="button"
      @click="handleAddToWatchlist"
        class="text-black flex items-center bg-orange-500 hover:bg-orange-200 py-2 px-2 rounded" 
      >
        <i class="fas fa-arrow-left mr-2"></i> Add To Watchlist
      </button >
    </div>
  </section>

  <section class="bg-red-50" v-if="!state.isLoading">
    <div class="container m-auto py-10 px-6">
      <div :class="`grid ${isAdmin ? 'grid-cols-[minmax(0,3fr)_minmax(0,1fr)]' : 'grid-cols-1'} md:grid-cols-70/30 w-full gap-6`">
        <main>
          <div class="bg-white p-6 rounded-lg shadow-md text-center md:text-left grid grid-cols-2">
            <div>              
              <div class="text-gray-500 mb-4">{{ state.movie.year }}</div>
              <h1 class="text-3xl font-bold mb-4">{{ state.movie.title }}</h1>
              <div class="text-gray-500 mb-4 flex align-middle justify-center md:justify-start">
              <i class="pi pi-star-fill text-lg text-yellow-400 mr-2 mt-1"></i>
                <p class="text-yellow-500">{{ state.movie.score }}</p>
              </div>
            </div>
            <div v-if="isAuthenticated" class="ms-10">
              <form @submit.prevent="handleScoreSubmit" class="flex flex-col gap-4 w-40 mt-5">
                <label class="text-xl">Score this movie</label>
                  <div class="flex flex-col items-center gap-4">
                      <Rating :disabled="existingScore > 0" name="score" v-model="scoreVal" :stars="10" class="[&_.p-rating-icon]:text-[var(--p-rating-icon-size)]"/>
                  </div>
                  <button v-if="existingScore == 0" type="submit" class="focus:outline-none text-white bg-yellow-400 hover:bg-yellow-500 focus:ring-4 focus:ring-yellow-300 font-medium rounded-lg text-sm px-5 py-2.5 me-2 mb-2 dark:focus:ring-yellow-900">Submit Score</button>
              </form>
            </div>
          </div>

          <div class="bg-white p-6 rounded-lg shadow-md mt-6">
            <h3 class="text-orange-800 text-lg font-bold mb-3">
              Movie Genres
            </h3>

            <span v-for="g in state.movie.genres" :key="g" class="mx-1">{{
              g
            }}</span>
          </div>
          
          <div class="bg-white p-6 rounded-lg shadow-md mt-6">
            <h3 class="text-xl font-bold mb-6">Directors</h3>
            
            <h2 v-for="d in state.movie.directors" :key="d.celebrityId" class="mx-2">
              {{ d.fullName }}, 
            </h2>
            
            <h3 class="text-xl font-bold mb-6">Actors</h3>
            
            <span class="mx-1" v-for="a in state.movie.actors" :key="a.celebrityId">
              {{ a.fullName }}, 
            </span>
          </div>

          <div class="bg-white p-6 rounded-lg shadow-md mt-6">
            <h3 class="text-orange-800 text-lg font-bold mb-3">
              Comments
            </h3>

            <span v-for="c in state.movie.comments" :key="c.commentId" class="mx-1">{{
              c.text
            }}</span>
          </div>
        </main>
        
        <!-- Sidebar -->
        <aside>
          <!-- Manage -->
          <div v-if="isAdmin" class="bg-white p-6 rounded-lg shadow-md mt-6">
            <h3 class="text-xl font-bold mb-6">Manage Movie</h3>
            <RouterLink
              :to="`/movies/edit/${state.movie.id}`"
              class="bg-green-500 hover:bg-green-600 text-white text-center font-bold py-2 px-4 rounded-full w-full focus:outline-none focus:shadow-outline mt-4 block"
              >Edit movie</RouterLink
            >
            <button
              @click="handleDelete"
              class="bg-red-500 hover:bg-red-600 text-white font-bold py-2 px-4 rounded-full w-full focus:outline-none focus:shadow-outline mt-4 block"
            >
              Delete m]Movie
            </button>
          </div>
        </aside>
      </div>
    </div>
  </section>

  <section class="bg-green-50" v-else>
    <div class="container m-auto py-10 px-6">
      <div class="grid grid-cols-1 md:grid-cols-70/30 w-full gap-6">
        <PulseLoader />
      </div>
    </div>
  </section>
</template>

<style scoped>
/* Root rating container */
:deep(.p-rating) {
  --p-rating-icon-size: 2rem;            /* Icon size */
  --p-rating-icon-color: #facc15;        /* Default yellow-400 */
  --p-rating-icon-hover-color: #eab308;  /* Hover yellow-500 */
  --p-rating-icon-active-color: #facc15; /* Active state color */
  --p-rating-gap: 0.5rem;                /* Space between stars */
}

/* Individual rating options */
:deep(.p-rating-option) {
  transition: all var(--p-rating-transition-duration, 0.2s) ease;
}

/* Active (selected) stars */
:deep(.p-rating-on-icon) {
  color: var(--p-rating-icon-active-color);
}

/* Inactive stars */
:deep(.p-rating-off-icon) {
  color: var(--p-rating-icon-color);
  opacity: 0.4;
}

/* Hover states */
:deep(.p-rating-option:hover) .p-rating-off-icon {
  color: var(--p-rating-icon-hover-color);
  opacity: 0.7;
}

/* Focus ring customization */
:deep(.p-rating:focus-within) {
  --p-rating-focus-ring-color: #fde047;  /* yellow-300 */
  --p-rating-focus-ring-width: 2px;
}
</style>