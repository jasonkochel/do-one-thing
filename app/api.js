import AsyncStorage from "@react-native-async-storage/async-storage";
import axios from "axios";
import Constants from "expo-constants";
import * as SecureStore from "expo-secure-store";

const client = axios.create({
  baseURL: Constants.manifest.extra.baseUrl,
});

client.interceptors.request.use(async (config) => {
  const token = await AsyncStorage.getItem("access_token");
  config.headers.Authorization = "Bearer " + token;
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
      try {
        const newAccessToken = await refreshAccessToken();
        await AsyncStorage.setItem("access_token", newAccessToken);
        originalRequest.headers.Authorization = "Bearer " + newAccessToken;
        return axios(originalRequest);
      } catch (e) {
        return Promise.reject(e);
      }
    } else {
      return Promise.reject(error);
    }
  }
);

const getAccessToken = (authorizationCode, redirectUri) =>
  client
    .post(
      "/auth/token",
      new URLSearchParams({
        authorizationCode,
        redirectUri,
      })
    )
    .then((res) => res.data);

const refreshAccessToken = async () => {
  let refreshToken;
  if (await SecureStore.isAvailableAsync()) {
    refreshToken = await SecureStore.getItemAsync("refresh_token");
  } else {
    refreshToken = await AsyncStorage.getItem("refresh_token");
  }

  if (!refreshToken) return Promise.reject("refresh_token not found");

  // use "axios" instead of "client" here, because this is called within "client"s response interceptor
  // and we don't want to re-use "client" while it is in flight
  const res = await axios.post(
    `${Constants.manifest.extra.baseUrl}/auth/refresh`,
    new URLSearchParams({ refreshToken })
  );

  if (res.status === 200) {
    return res.data.access_token;
  } else {
    return Promise.reject(res);
  }
};

const getUserInfo = () => {
  return client.get(`https://openidconnect.googleapis.com/v1/userinfo`).then((res) => res.data);
};

const getTaskLists = () => client.get("/lists").then((res) => res.data);

const getTags = (listId) => client.get(`/lists/${listId}/tags`).then((res) => res.data);

const createTag = (listId, tag) =>
  client.post(`/lists/${listId}/tags/${tag}`).then((res) => res.data);

const deleteTag = (listId, tag) =>
  client.delete(`/lists/${listId}/tags/${tag}`).then((res) => res.data);

const getTasks = (listId, tag = null) => {
  const qs = tag ? new URLSearchParams({ tag: tag }).toString() : "";
  return client.get(`/lists/${listId}/tasks?${qs}`).then((res) => res.data);
};

const getTaskTags = (listId, taskId) =>
  client.get(`/lists/${listId}/tasks/${taskId}/tags`).then((res) => res.data);

const addTaskTag = (listId, taskId, tag) => {
  const qs = tag ? new URLSearchParams({ tag: tag }).toString() : "";
  return client.post(`/lists/${listId}/tasks/${taskId}/tags?${qs}`).then((res) => res.data);
};

const deleteTaskTag = (listId, taskId, tag) => {
  const qs = tag ? new URLSearchParams({ tag: tag }).toString() : "";
  return client
    .delete(`/lists/${listId}/tasks/${taskId}/tags?${qs}`, new URLSearchParams({ tag }))
    .then((res) => res.data);
};

const api = {
  getAccessToken,
  refreshAccessToken,
  getUserInfo,

  getTaskLists,
  getTasks,

  getTags,
  createTag,
  deleteTag,

  getTaskTags,
  addTaskTag,
  deleteTaskTag,
};

export default api;
