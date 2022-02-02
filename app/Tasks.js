import React, { useEffect, useState } from "react";
import { FlatList, StyleSheet, Text, View } from "react-native";
import api from "./api";

const Tasks = ({ route, navigation }) => {
  const [tasks, setTasks] = useState();

  const { listId } = route.params;

  useEffect(() => {
    api
      .getTasks(listId)
      .then((res) => setTasks(res))
      .catch((e) => {
        console.error(e);
      });
  }, [listId]);

  const Item = ({ title }) => (
    <View style={styles.item}>
      <Text style={styles.title}>{title}</Text>
    </View>
  );

  const renderItem = ({ item }) => <Item title={item.title} />;

  return (
    <View style={styles.container}>
      <FlatList
        data={tasks}
        renderItem={renderItem}
        keyExtractor={(item) => item.id}
      />
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

export default Tasks;
