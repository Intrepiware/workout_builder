import React from "react";
import ReactDOM from "react-dom/client";
import { FocusAndActivationGroup } from "./Components/ExerciseDetails/FocusAndActivationGroup";

const root = document.getElementById("focus-and-activation-group");
ReactDOM.createRoot(root!).render(
  <React.StrictMode>
    <FocusAndActivationGroup {...root?.dataset} />
  </React.StrictMode>
);
