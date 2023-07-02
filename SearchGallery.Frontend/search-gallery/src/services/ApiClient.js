import axios from 'axios';

export default () =>
  axios.create({
    baseURL: 'https://localhost:7242/',
    headers: {
      Accept: 'application/json',
    }
  });
