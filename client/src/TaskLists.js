import { useEffect, useState } from "react";
import api from "./api";
import { Tasks } from "./Tasks";

export function TaskLists({ userInfo }) {
  const [taskLists, setTaskLists] = useState();
  const [listId, setListId] = useState();

  useEffect(() => {
    api.getTaskLists().then((res) => setTaskLists(res));
  }, []);

  return (
    <>
      <div>Hi {userInfo?.name}</div>
      <hr />
      {taskLists?.map((list) => (
        <div key={list.id} onClick={() => setListId(list.id)}>
          {list.title}
        </div>
      ))}
      {listId && <Tasks key={listId} listId={listId} />}
    </>
  );
}
