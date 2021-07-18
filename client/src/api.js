import axios from "axios";

const client = axios.create({
  baseURL: "https://localhost:5001/api",
});

client.interceptors.request.use((config) => {
  config.headers.Authorization =
    "Bearer " + localStorage.getItem("access_token");
  return config;
});

client.interceptors.response.use(
  (response) => {
    return response;
  },
  async (error) => {
    const originalRequest = error.config;
    if (error.response.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      const res = await axios.post(
        "https://localhost:5001/api/auth/refresh",
        new URLSearchParams({
          refreshToken: localStorage.getItem("refresh_token"),
        })
      );
      if (res.status === 200) {
        const token = res.data.access_token;
        localStorage.setItem("access_token", token);
        originalRequest.headers.Authorization = "Bearer " + token;
        return axios(originalRequest);
      }
    }
  }
);

const getAccessToken = (authorizationCode) =>
  client
    .post(
      "/auth/token",
      new URLSearchParams({
        authorizationCode,
      })
    )
    .then((res) => res.data);

const getUserInfo = () => {
  return client
    .get(`https://openidconnect.googleapis.com/v1/userinfo`)
    .then((res) => res.data);
};

const getTaskLists = () => client.get("/tasks").then((res) => res.data);
const getTasks = (listId) =>
  client.get(`/tasks/${listId}`).then((res) => res.data);

const getTags = (listId) =>
  client.get(`/lists/${listId}/tags`).then((res) => res.data);
const createTag = (listId, tag) =>
  client.post(`/lists/${listId}/tags/${tag}`).then((res) => res.data);
const deleteTag = (listId, tag) =>
  client.delete(`/lists/${listId}/tags/${tag}`).then((res) => res.data);

const api = {
  getAccessToken,
  getUserInfo,

  getTaskLists,
  getTasks,

  getTags,
  createTag,
  deleteTag,
};

export default api;
