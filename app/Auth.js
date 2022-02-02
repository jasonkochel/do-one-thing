import AsyncStorage from "@react-native-async-storage/async-storage";
import * as AuthSession from "expo-auth-session";
import * as Google from "expo-auth-session/providers/google";
import Constants from "expo-constants";
import * as SecureStore from "expo-secure-store";
import { StatusBar } from "expo-status-bar";
import React, { memo, useEffect } from "react";
import { Button, StyleSheet, View } from "react-native";
import api from "./api";

const redirectUri = AuthSession.makeRedirectUri({ useProxy: true });

const authConfig = {
  expoClientId: Constants.manifest.extra.webClientId,
  webClientId: Constants.manifest.extra.webClientId,
  //iosClientId: "GOOGLE_GUID.apps.googleusercontent.com",
  //androidClientId: "GOOGLE_GUID.apps.googleusercontent.com",
  responseType: "code",
  prompt: "consent",
  scopes: ["https://www.googleapis.com/auth/tasks"],
  shouldAutoExchangeCode: false,
  usePKCE: false,
  redirectUri,
  extraParams: {
    access_type: "offline",
  },
};

const Auth = ({ setLoggedIn }) => {
  const [request, response, promptAsync] = Google.useAuthRequest(authConfig);

  useEffect(() => {
    const handleResponse = async (code) => {
      try {
        const res = await api.getAccessToken(code, redirectUri);
        await AsyncStorage.setItem("access_token", res.access_token);
        if (await SecureStore.isAvailableAsync()) {
          await SecureStore.setItemAsync("refresh_token", res.refresh_token);
        } else {
          await AsyncStorage.setItem("refresh_token", res.refresh_token);
        }
        setLoggedIn(true);
      } catch (e) {
        console.error(e);
        setLoggedIn(false);
      }
    };

    if (response?.type === "success") {
      handleResponse(response.params.code);
    }
  }, [response]);

  return (
    <View style={styles.container}>
      <Button
        disabled={!request}
        title="Login"
        onPress={() => {
          promptAsync();
        }}
      />
      <StatusBar style="auto" />
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#fff",
    alignItems: "center",
    justifyContent: "center",
  },
});

export default memo(Auth);
