import { useEffect, useState } from "react";
import GoogleLogin from "react-google-login";
import api from "./api";
import "./App.css";

function App() {
  const [loggedIn, setLoggedIn] = useState(false);
  const [userInfo, setUserInfo] = useState();
  const [taskLists, setTaskLists] = useState();

  useEffect(() => {
    const accessToken = localStorage.getItem("access_token");
    if (!!accessToken) {
      setLoggedIn(true);
    }
  }, []);

  useEffect(() => {
    if (loggedIn) {
      api.getUserInfo().then((res) => setUserInfo(res));
      api.getTaskLists().then((res) => setTaskLists(res));
    }
  }, [loggedIn]);

  const responseGoogle = async (response) => {
    const authorizationCode = response.code;

    if (authorizationCode) {
      try {
        const data = await api.getAccessToken(authorizationCode);
        setLoggedIn(true);
        localStorage.setItem("access_token", data.access_token);
        localStorage.setItem("refresh_token", data.refresh_token);
      } catch (err) {
        console.error(err);
        setLoggedIn(false);
      }
    }
  };

  return (
    <div className="App">
      <header className="App-header">
        {loggedIn ? (
          <>
            <div>Hi {userInfo?.name}</div>
            <ul>
              {taskLists?.map((list) => (
                <li key={list.id}>{list.title}</li>
              ))}
            </ul>
          </>
        ) : (
          <GoogleLogin
            clientId="621831603836-ru5ohclu0c4frateq2cqn3ekofe686k5.apps.googleusercontent.com"
            buttonText="Login"
            onSuccess={responseGoogle}
            onFailure={responseGoogle}
            cookiePolicy={"single_host_origin"}
            accessType="offline"
            responseType="code"
            prompt="consent"
            scope="https://www.googleapis.com/auth/tasks"
          />
        )}
      </header>
    </div>
  );
}

export default App;
