import React from "react";
import ReactDOM from "react-dom/client";
import ExercisesIndex from "./Pages/ExercisesIndex";

const root = document.getElementById("root");
ReactDOM.createRoot(root!).render(
  <React.StrictMode>
    <ExercisesIndex {...root?.dataset} />
  </React.StrictMode>
);
