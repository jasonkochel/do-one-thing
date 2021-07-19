import { useEffect, useState } from "react";
import api from "./api";
import { Task } from "./Task";

export function Tasks({ listId }) {
  const [tags, setTags] = useState();
  const [selectedTag, setSelectedTag] = useState();
  const [tasks, setTasks] = useState();
  const [taskId, setTaskId] = useState();

  useEffect(() => {
    api.getTags(listId).then((res) => setTags(res));
  }, [listId]);

  useEffect(() => {
    api.getTasks(listId, selectedTag).then((res) => {
      setTaskId();
      setTasks(res);
    });
  }, [listId, selectedTag]);

  const handleSelectTag = (tag) => {
    if (selectedTag === tag) {
      setSelectedTag();
    } else {
      setSelectedTag(tag);
    }
  };

  return (
    <>
      <hr />
      {tags &&
        tags.map((t) => (
          <span
            key={t}
            style={{
              margin: "0 15px 0 15px",
              color: t === selectedTag ? "red" : "white",
            }}
            onClick={() => handleSelectTag(t)}
          >
            {t}
          </span>
        ))}
      <hr />
      {tasks &&
        tasks.map((task) => (
          <div key={task.id} onClick={() => setTaskId(task.id)}>
            {task.title}
          </div>
        ))}
      <hr />
      {taskId && (
        <Task
          listId={listId}
          task={tasks.find((t) => t.id === taskId)}
          tags={tags}
        />
      )}
    </>
  );
}
