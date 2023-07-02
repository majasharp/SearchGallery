<template>
    <multi-file-viewer v-model="images"></multi-file-viewer>
</template>

<script>
import ApiClient from '../services/ApiClient';
import MultiFileViewer from '../components/MultiFileViewer.vue';
import { ref, onMounted } from 'vue';

export default {
    components: {
        MultiFileViewer
    },
    setup() {
        const images = ref([]);

        function retrieveImages() {
            console.log("Hello");
            ApiClient().post('gallery', {})
            .then(response => {
                images.value = response.data;
            })    
        }

        onMounted(() => {
            retrieveImages();
        })
        return {
            images
        }
    },
}
</script>
