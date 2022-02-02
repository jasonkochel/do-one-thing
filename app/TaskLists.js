import { StatusBar } from "expo-status-bar";
import React, { useEffect, useState } from "react";
import { FlatList, StyleSheet, Text, View } from "react-native";
import api from "./api";

const TaskLists = ({ navigation }) => {
  const [taskLists, setTaskLists] = useState();

  useEffect(() => {
    api
      .getTaskLists()
      .then((res) => setTaskLists(res))
      .catch((e) => {
        console.error(e);
      });
  }, []);

  const Item = ({ item }) => (
    <View style={styles.item}>
      <Text
        style={styles.title}
        onPress={() => navigation.navigate("Tasks", { listId: item.id })}
      >
        {item.title}
      </Text>
    </View>
  );

  const renderTaskListItem = ({ item }) => <Item item={item} />;

  return (
    <View style={styles.container}>
      <FlatList
        data={taskLists}
        renderItem={renderTaskListItem}
        keyExtractor={(item) => item.id}
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
  item: {
    backgroundColor: "#fff",
    borderColor: "#000",
    borderWidth: 1,
    padding: 20,
    marginHorizontal: 16,
  },
  title: {
    fontSize: 32,
  },
});

export default TaskLists;
