<template>
    <div class="mt-3 ms-2 me-2">
        <div>
            <CRow>
                <CCol xs="7">
                    <CFormInput class="mb-2 ms-1" placeholder="Search in images..." @keyup.enter="retrieveImagesBySearch" v-model="searchText"/>
                </CCol>
                <CCol xs="1">
                    <CButton class="btn btn-primary" @click="retrieveImagesBySearch">Search</CButton>
                </CCol>
                <CCol xs="2">
                    <CFormSelect v-model="numberOfResults">
                        <option value="3">Number of results</option>
                        <option value="1">1</option>
                        <option value="3">3</option>
                        <option value="5">5</option>
                        <option value="10">10</option>
                        <option value="20">20</option>
                    </CFormSelect>
                </CCol>
                <CCol xs="2" class="mt-2">
                    <CFormSwitch v-model="smartSearchEnabled" label="Search with ChatGPT"/>
                </CCol>
            </CRow>
        </div>
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
    MultiFileViewer,
},
    setup() {
        const images = ref([]);
        const searchText = ref("");
        const smartSearchEnabled = ref(false);
        const numberOfResults = ref("3");

        function retrieveImages() {
            ApiClient().post('gallery', {})
            .then(response => {
                images.value = response.data;
            })
        }

        function retrieveImagesBySearch() {
            ApiClient().post('gallery', {
                freeText: searchText.value,
                smartSearchEnabled: smartSearchEnabled.value,
                pageCount: numberOfResults.value
            })
            .then(response => {
                images.value = response.data;
            })
        }

        onMounted(() => {
            retrieveImages();
        })
        return {
            images,
            searchText,
            retrieveImagesBySearch,
            smartSearchEnabled,
            numberOfResults
        }
    },
}
</script>
