import React from "react";
import ReactDOM from "react-dom/client";
import HomeIndex from "./Pages/HomeIndex";

const root = document.getElementById("root");
ReactDOM.createRoot(root!).render(
  <React.StrictMode>
    <HomeIndex {...root?.dataset} />
  </React.StrictMode>
);
