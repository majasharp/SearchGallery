import axios from 'axios';

export default () =>
  axios.create({
    baseURL: 'path/to/backend',
    headers: {
      responseType: 'arraybuffer'
    }
  });
