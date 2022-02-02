import "dotenv/config";

export default ({ config }) => ({
  ...config,
  extra: {
    webClientId: process.env.GOOGLE_WEB_CLIENT_ID,
    baseUrl: process.env.API_BASE_URL,
  },
});
