<template>
    <div class="mt-3 ms-2 me-2">
        <CForm>
            <CRow>
                <CCol xs="9">
                    <CFormInput class="mb-2 ms-1"
                        placeholder="Search in images..."/>
                </CCol>
                <CCol xs="1">
                    <CButton class="btn btn-primary">Search</CButton>
                </CCol>
                <CCol xs="2" class="mt-2">
                    <CFormSwitch label="Search with ChatGPT"/>
                </CCol>
            </CRow>
        </CForm>
        <div class="mt-2 ms-2 me-2">
            <multi-file-viewer v-model="images"></multi-file-viewer>
        </div>
    </div>
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
