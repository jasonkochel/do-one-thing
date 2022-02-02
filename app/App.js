import { NavigationContainer } from "@react-navigation/native";
import { createStackNavigator } from "@react-navigation/stack";
import * as WebBrowser from "expo-web-browser";
import React, { useEffect, useState } from "react";
import api from "./api";
import Auth from "./Auth";
import TaskLists from "./TaskLists";
import Tasks from "./Tasks";

WebBrowser.maybeCompleteAuthSession();

const Stack = createStackNavigator();

export default function App() {
  const [loggedIn, setLoggedIn] = useState(false);

  useEffect(() => {
    /*
    This will test the validity of the stored access token (if any),
    get a new one from the refresh token (if necessary and one exists),
    try again with the new access token, and finally reject if all fails
    */
    api
      .getUserInfo()
      .then(() => setLoggedIn(true))
      .catch(() => setLoggedIn(false));
  }, []);

  return (
    <NavigationContainer>
      <Stack.Navigator>
        {loggedIn ? (
          <>
            <Stack.Screen name="TaskLists" component={TaskLists} />
            <Stack.Screen name="Tasks" component={Tasks} />
          </>
        ) : (
          <Stack.Screen name="Login">
            {(props) => <Auth {...props} setLoggedIn={setLoggedIn} />}
          </Stack.Screen>
        )}
      </Stack.Navigator>
    </NavigationContainer>
  );
}
