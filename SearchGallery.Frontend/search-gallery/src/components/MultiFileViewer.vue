<template>
    <CRow>
      <CCol>
        <DropZone @filesloaded="onFilesSelected" />
      </CCol>
    </CRow>
    <CRow class="mt-3" v-for="row in rows" :key="row">
      <CCol class="m-2" v-for="file in row" :key="file">
        <a class="position-relative" v-if="file?.src">
          <CButton class="position-absolute top-0 start-0 translate-middle badge rounded-pill bg-info" @click.prevent="downloadToDisk(file)">
            <CIcon :icon="cilCloudDownload" size="sm" />
          </CButton>
          <CButton class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger" @click.prevent="deleteFile(file)">
            <CIcon :icon="cilTrash" size="sm" />
          </CButton>
          <CRow @click="loadModal(file)">
            <CImage
              style="object-fit: contain"
              class="border border-light"
              height="150"
              :src="file?.src"
              rounded
            />
          </CRow>
          <CRow>
            <label>{{ file?.originalName?.substring(0, 20) ?? 'No filename available' }}</label>
          </CRow>
        </a>
      </CCol>
      <div :style="imageModalActive ? undefined : 'display:none'">
        <CModal
          size="md"
          :visible="imageModalActive"
          @close="
            () => {
              imageModalActive = false;
            }
          "
        >
          <CModalHeader>
            <CModalTitle>{{ modalImage.src ? modalImage.originalName ?? 'No filename available' : 'No image available' }}</CModalTitle>
          </CModalHeader>
          <CModalBody>
            <CImage v-if="modalImage?.src" style="object-fit: contain" rounded class="img-fluid border border-light" :src="modalImage.src" />
          </CModalBody>
        </CModal>
      </div>
    </CRow>
  </template>
  
  <script>
  import DropZone from '@/components/DropZone.vue';
  import ApiClient from '../services/ApiClient';
  import DownloadClient from '../services/DownloadClient';
  import { CIcon } from '@coreui/icons-vue';
  import { cilTrash, cilCloudDownload } from '@coreui/icons';
  import { ref, toRefs, watch, computed } from 'vue';
import { formToJSON } from 'axios';
  
  export default {
    name: 'MultiFileViewer',
    components: {
      DropZone,
      CIcon
    },
    props: {
      modelValue: {
        required: false
      }
    },
    emits: [
      'uploadFailed',
      'downloadFailed',
      'deleteFailed',
      'uploadSucceeded',
      'downloadSucceeded',
      'deleteSucceeded',
      'update:modelValue'
    ],
    setup(props, { emit }) {
      const {
        modelValue: modelValue,
      } = toRefs(props);
  
      const filesToDisplay = ref([]);
      const imageModalActive = ref(false);
      const modalImage = ref({});
  
      const rows = computed(() => {
        let result = [];
        for (let i = 0; i < filesToDisplay.value.length; i = i + 3) {
          result.push([filesToDisplay.value[i], filesToDisplay.value[i + 1], filesToDisplay.value[i + 2]]);
        }
        return result;
      });
  
      watch(
        modelValue,
        () => {
          if (modelValue.value != filesToDisplay.value) {
            if (modelValue.value) {
              modelValue.value.forEach((file) => download(file, true));
            }
          }
        },
        { immediate: true }
      );
  
      function onFilesSelected(files) {
        files.forEach((file) => {
          upload(file);
        });
      }
  
      function showImage(fileDetails, file) {
        if (fileDetails.contentType.includes('image')) {
          const fileReader = new FileReader();
          fileReader.addEventListener('load', () => {
            fileDetails.src = fileReader.result;
            filesToDisplay.value.push(fileDetails);
          });
          fileReader.readAsDataURL(file);
        } else {
          fileDetails.src = '/file-icon.jpg';
          filesToDisplay.value.push(fileDetails);
        }
        emit('update:modelValue', filesToDisplay.value);
      }
  
      function upload(file) {
        const formData = new FormData();
        formData.append('file', file);
        ApiClient()
          .post('gallery/upload', formData)
          .then((response) => {
            showImage(response.data, file);
          })
          .catch((error) => emit('uploadFailed', { model: file, error }));
      }
  
      function download(fileDetails, isThumbnail, forModal) {
        DownloadClient()
          .get(`gallery/${fileDetails.id}?tryDownloadThumbnail=${isThumbnail}`, { responseType: 'arraybuffer' })
          .then((response) => {
            const bytes = new Uint8Array(response.data);
            let binary = '';
            for (let i = 0; i < bytes.length; i++) {
              binary += String.fromCharCode(bytes[i]);
            }
            const imageBinary = 'data:image/.jpeg;base64,' + btoa(binary);
            if (forModal) {
              imageModalActive.value = true;
              modalImage.value.src = imageBinary;
              modalImage.value.actualImageLoaded = true;
            } else {
              fileDetails.src = imageBinary;
              filesToDisplay.value.push(fileDetails);
              emit('update:modelValue', filesToDisplay.value);
            }
          })
          .catch((error) => {
            emit('downloadFailed', { error });
          });
      }
  
      function downloadToDisk(fileDetails) {
        DownloadClient()
          .get(`gallery/${fileDetails.id}`, { responseType: 'arraybuffer' })
          .then((response) => {
            var fileURL = window.URL.createObjectURL(new Blob([response.data], { type: fileDetails.contentType }));
            var fileLink = document.createElement('a');
            fileLink.href = fileURL;
            fileLink.setAttribute('download', fileDetails.originalName);
            document.body.appendChild(fileLink);
            fileLink.click();
          });
      }
  
      function deleteFile(fileDetails) {
        ApiClient()
          .delete(`gallery/${fileDetails.id}`)
          .then((response) => {
            const indexToDelete = filesToDisplay.value.findIndex((file) => file.id === fileDetails.id);
            filesToDisplay.value.splice(indexToDelete, 1);
            emit('deleteSucceeded', { fileDetails, response });
            emit('update:modelValue', filesToDisplay.value);
          })
          .catch((error) => emit('deleteFailed', { fileDetails, error }));
      }
  
      function loadModal(file) {
        modalImage.value = file;

        if (!modalImage.value.actualImageLoaded) {
          download(file, false, true);
        } else {
          imageModalActive.value = true;
        }
      }
  
      return {
        filesToDisplay,
        rows,
        onFilesSelected,
        downloadToDisk,
        deleteFile,
        cilCloudDownload,
        cilTrash,
        imageModalActive,
        modalImage,
        loadModal
      };
    }
  };
  </script>