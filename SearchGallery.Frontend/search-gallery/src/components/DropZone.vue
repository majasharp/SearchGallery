<template>
    <div
      @dragenter.prevent="toggleActive"
      @dragleave.prevent="toggleActive"
      @dragover.prevent
      @drop.prevent="toggleActive"
      @drop="onDragged"
      :class="{ 'active-dropzone': active }"
      class="dropzone"
    >
      <span>Drop files here</span>
      <span>or</span>
      <label class="btn btn-primary" for="dropzoneFile">Choose file(s)</label>
      <input id="dropzoneFile" type="file" accept="image/*" class="dropzoneFile" @input="onEventFilesPicked" multiple="multiple" ref="files" />
    </div>
  </template>
  
<script>
  import { defineComponent, ref } from 'vue';
  export default defineComponent({
    name: 'DropZone',
    props: {},
    emits: ['filesloaded'],
    setup(props, { emit }) {
      const active = ref(false);
      const files = ref(null);
      const toggleActive = () => {
        active.value = !active.value;
      };
  
      const onEventFilesPicked = () => {
        emit('filesloaded', [...files.value.files]);
        files.value.value = '';
      };
  
      const onDragged = () => {
        emit('filesloaded', [...files.value.files]);
        files.value.value = '';
      };
  
      return {
        active,
        toggleActive,
        onEventFilesPicked,
        onDragged,
        files
      };
    }
  });
</script>
  
<style scoped lang="scss">
  .dropzone {
    height: 150px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    row-gap: 8px;
    border: 2px dashed #321fdb;
    background-color: #fff;
    transition: 0.3s ease all;
    label {
      transition: 0.3s ease all;
    }
    input {
      display: none;
    }
  }
  .active-dropzone {
    color: #fff;
    border-color: #fff;
    background-color: #321fdb;
    label {
      background-color: #fff;
      color: #321fdb;
    }
  }
</style>