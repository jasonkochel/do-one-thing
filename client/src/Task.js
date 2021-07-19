import { useEffect, useState } from "react";
import api from "./api";

export function Task({ listId, task, tags }) {
  const [taskTags, setTaskTags] = useState();

  const computeTaskTags = (tags, taskTags) => {
    if (tags && Array.isArray(tags)) {
      const tagList = tags.map((t) => ({
        tag: t,
        isOn: taskTags.includes(t),
      }));
      setTaskTags(tagList);
    }
  };

  useEffect(() => {
    api.getTaskTags(listId, task.id).then((res) => computeTaskTags(tags, res));
  }, [listId, task.id, tags]);

  const handleToggleTag = (i) => {
    const { tag, isOn } = taskTags[i];

    const method = isOn ? api.deleteTaskTag : api.addTaskTag;
    method(listId, task.id, tag).then((res) => computeTaskTags(tags, res));
  };

  return (
    <div>
      {task.title}:
      {taskTags &&
        taskTags.map((t, i) => (
          <span
            key={t.tag}
            style={{ margin: "0 15px 0 15px", color: t.isOn ? "red" : "white" }}
            onClick={() => handleToggleTag(i)}
          >
            {t.tag}
          </span>
        ))}
    </div>
  );
}
