import axios from 'axios';

export default () =>
  axios.create({
    baseURL: 'path/to/backend',
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json'
    }
  });
