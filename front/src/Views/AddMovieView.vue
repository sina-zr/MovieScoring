<script setup>
import { authApi, isAdmin } from "@/services/auth";
import axios from "axios";
import { reactive, onBeforeMount, onMounted, ref, watch } from "vue";
import { useRouter } from "vue-router";
import { useToast } from "vue-toastification";
import Multiselect from "vue-multiselect";
import "vue-multiselect/dist/vue-multiselect.min.css";
import CelebrityMultiselect from "@/components/CelebrityMultiselect.vue";

const router = useRouter();
const toast = useToast();
onBeforeMount(() => {
  if (!isAdmin.value) {
    router.push("/");
  }
});

// Genre options from your API/data
const genreOptions = ref([])
// Selected genres (full objects)
const selectedGenres = ref([])
// Output: Array of genreIds
const selectedGenreIds = ref([])
// Update genreIds whenever selected genres change
watch(selectedGenres, (newVal) => {
  selectedGenreIds.value = newVal.map(g => g.genreId)
})
const loadGenres = async () => {
  try {
    const res = await axios.get('https://localhost:7214/api/GetAllGenres')
    genreOptions.value = res.data
    console.log(genreOptions.value)
  } catch (error) {
    console.error('Error fetching genres')
  }
}
onMounted(async () => {
  await loadGenres()
})

const form = reactive({
  title: "",
  year: 0
});

const handleSubmit = async () => {
  const requestBody = {
    title: form.title,
    year: form.year,
    genres: selectedGenreIds.value
  };

  try {
    const res = await authApi.post("/api/Admin/AddMovie", requestBody);

    var newMovieId = Number(res.data)

    if (directorsIdArray.value && directorsIdArray.value.length) {
      try {
        const directorsRes = await authApi.post(`/api/Admin/UpsertMovieDirectors?movieId=${newMovieId}`, directorsIdArray.value) 
      } catch (error) {
        console.error('Error in setting the movie directors: ', error)
      }
    } 
    if (actorsIdArray.value && actorsIdArray.value.length) {
      try {
        const actorsRes = await authApi.post(`/api/Admin/UpsertMovieActors?movieId=${newMovieId}`, actorsIdArray.value) 
      } catch (error) {
        console.error('Error in setting the movie actors: ', error)
      }
    }

    router.push(`/movies/${res.data}`);
    toast.success("Successfully added a movie");
  } catch (error) {
    console.error("Error from AddMovieView: " + error);
    toast.error("Faild to add movie");
  }
};

const directorsIdArray = ref([])
const actorsIdArray = ref([])
</script>
<template>
  <section class="bg-green-50">
    <div class="container m-auto max-w-2xl py-24">
      <div
        class="bg-white px-6 py-8 mb-4 shadow-md rounded-md border m-4 md:m-0"
      >
        <form @submit.prevent="handleSubmit">
          <h2 class="text-3xl text-center font-semibold mb-6">Add Movie</h2>

          <div class="mb-4">
            <label for="genres" class="block text-gray-700 font-bold mb-2">
              Movie Genres
            </label>
            <Multiselect v-model="selectedGenres" :options="genreOptions" :multiple="true"
              :close-on-select="true" :clear-on-select="false" :preserve-search="true"
              placeholder="Select genres" label="title" track-by="genreId"
            />
          </div>

          <div class="mb-4">
            <label class="block text-gray-700 font-bold mb-2"
              >Movie Title</label
            >
            <input
              v-model="form.title"
              type="text"
              id="title"
              class="border rounded w-full py-2 px-3 mb-2"
              placeholder="eg. Harry Potter"
              required
            />
          </div>          

          <div class="mb-4">
            <label class="block text-gray-700 font-bold mb-2"> Year </label>
            <input
              v-model="form.year"
              type="number"
              id="year"
              name="year"
              class="border rounded w-full py-2 px-3 mb-2"
              placeholder="like 2012"
              required
            />
          </div>

          <h3 class="text-2xl mb-5">Other Info</h3>

          <div class="mb-4">
            <label for="actors" class="block text-gray-700 font-bold mb-2">
              Movie Directors
            </label>
            <CelebrityMultiselect v-model="directorsIdArray" />
            <p class="mt-2">You picked IDs: {{ directorsIdArray }}</p>
          </div>

          <div class="mb-4">
            <label for="actors" class="block text-gray-700 font-bold mb-2">
              Movie Actors
            </label>
            <CelebrityMultiselect v-model="actorsIdArray" />
            <p class="mt-2">You picked IDs: {{ actorsIdArray }}</p>
          </div>
          <div class="mb-3 flex justify-center">
            <button type="submit" class="py-2 px-3 rounded bg-green-300 hover:bg-green-100">Submit</button>
          </div>
        </form>
      </div>
    </div>
  </section>
</template>