import { useState, useEffect } from "react";
import Autocomplete from "../Components/AutoComplete";
import {
  Workout,
  WorkoutRootObject,
  getWorkout,
  getWorkoutById,
} from "../apis/workout";
import React from "react";
import "@creativebulma/bulma-tooltip/dist/bulma-tooltip.min.css";
import "./HomeIndex.css";

interface Timing {
  id: bigint;
  name: string;
  stations: number;
  stationTiming: string | null;
  notes: string | null;
  customGenerator: string | null;
}

function HomeIndex() {
  const [timing, setTiming] = useState("");
  const [focus, setFocus] = useState("Hybrid");
  const [isFocusLocked, setIsFocusLocked] = useState(false);
  const [isTimingLocked, setIsTimingLocked] = useState(false);
  const [isAdvancedModalShown, setIsAdvancedModalShown] = useState(false);
  const [lastClick, setLastClick] = useState("");
  const [timings, setTimings] = useState<string[]>([]);
  const [allEquipment, setAllEquipment] = useState<string[]>([]);
  const [selectedEquipment, setSelectedEquipment] = useState<string[]>([]);
  const [equipmentPreset, setEquipmentPreset] = useState("All");
  const [, setError] = useState(null);
  const [workout, setWorkout] = useState<Workout | null>(null);
  const [publicId, setPublicId] = useState<string | null>(null);
  const [isFavorite, setIsFavorite] = useState(false);

  useEffect(() => {
    fetch("/Home/Timings")
      .then((res) => res.json())
      .then(
        (result: Timing[]) => {
          setTimings(result.map((x) => x.name));
        },
        // Note: it's important to handle errors here
        // instead of a catch() block so that we don't swallow
        // exceptions from actual bugs in components.
        (error) => {
          // TODO: Implement this
          setError(error);
        }
      );
  }, []);

  useEffect(() => {
    fetch("/Home/Equipment")
      .then((res) => res.json())
      .then((result: string[]) => {
        setAllEquipment(result);
        setSelectedEquipment(result);
      });
  }, []);

  useEffect(() => {
    const urlParams = new URLSearchParams(window.location.search);

    if (urlParams.has("id")) {
      getSavedWorkout(urlParams.get("id") || "");
      return;
    }

    try {
      const savedWorkout: WorkoutRootObject = JSON.parse(
        localStorage.getItem("workout") || ""
      );
      if (savedWorkout.version === "v1") {
        const { workout } = savedWorkout;
        setWorkout(workout);
        setTiming(workout.name);
        setFocus(workout.focus);
        setPublicId(savedWorkout.publicId);
      }
    } catch (err) {
      console.error(
        "An error occurred while loading/rendering the saved workout",
        err
      );
    }
  }, []);

  useEffect(() => {
    switch (equipmentPreset) {
      case "None":
        setSelectedEquipment([]);
        break;
      case "Bootcamp":
        setSelectedEquipment([
          "Bodyweight",
          "Cones",
          "Space to Run",
          "Dumbbells",
        ]);
        break;
      case "Calisthenics":
        setSelectedEquipment([
          "Bodyweight",
          "Dip Bars",
          "Plyo Box",
          "Pull Up Bar",
          "Step Riser",
          "TRX",
        ]);
        break;
      case "Just Weights":
        setSelectedEquipment(["Dumbbells", "Barbell"]);
        break;
      case "Odd Object Training":
        setSelectedEquipment([
          "Kettlebell",
          "Med Ball",
          "Sandbell",
          "Sandbag",
          "Slamball",
          "Sledge Hammer",
          "Tire",
          "Small Plates",
          "Large Plates",
        ]);
        break;
      case "All":
      default:
        setSelectedEquipment(allEquipment);
        break;
    }
  }, [equipmentPreset]);

  const handleTimingChange = (item: string) => {
    setWorkout(null);
    setTiming(item);
    setIsTimingLocked(true);
  };

  const handleFocusChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setWorkout(null);
    setFocus(e.target.value);
    setIsFocusLocked(true);
  };

  const handleEquipmentToggle = (label: string): void => {
    const selected: string[] = [...selectedEquipment];
    if (selected.includes(label)) {
      selected.splice(selected.indexOf(label), 1);
    } else {
      selected.push(label);
    }
    setSelectedEquipment(selected);
  };

  const getCustomizedWorkout = () => {
    const timingParam: string = isTimingLocked ? timing : "";
    const focusParam: string = isFocusLocked ? focus : "";
    const equipmentParam: string = selectedEquipment.join("|");
    getWorkout(timingParam, focusParam, equipmentParam).then(
      (result: WorkoutRootObject) => {
        const { workout } = result;
        setWorkout(workout);
        setTiming(workout.name);
        setFocus(workout.focus);
        setPublicId(result.publicId);
        localStorage.setItem("workout", JSON.stringify(result));
      },
      // Note: it's important to handle errors here
      // instead of a catch() block so that we don't swallow
      // exceptions from actual bugs in components.
      (error) => {
        setError(error);
      }
    );
  };

  const getSavedWorkout = (id: string) => {
    getWorkoutById(id).then(
      (result: WorkoutRootObject) => {
        const { workout } = result;
        setWorkout(workout);
        setTiming(workout.name);
        setFocus(workout.focus);
        setPublicId(result.publicId);
        localStorage.setItem("workout", JSON.stringify(result));
      },
      // Note: it's important to handle errors here
      // instead of a catch() block so that we don't swallow
      // exceptions from actual bugs in components.
      (error) => {
        setError(error);
      }
    );
  };

  const setFavorite = () => {
    fetch(`/Home/Favorite/${publicId}`, {
      method: "POST",
      credentials: "include",
    }).then((res) => {
      if (res.headers.has("Location"))
        window.location.href = res.headers.get("Location") || "";
      else {
        setIsFavorite(!isFavorite);
        return res.json();
      }
    });
  };

  const equipment: string[] = [];
  function uniqueEquipment(item: string) {
    if (equipment.indexOf(item) == -1) {
      equipment.push(item);
      return true;
    }
    return false;
  }

  const toggleAdvancedOptions = () => {
    setIsAdvancedModalShown((old) => !old);
  };

  const handleFavoriteClick = () => {
    setFavorite();
    setLastClick("favorite");
  };

  const handleCopyClick = () => {
    navigator.clipboard
      .writeText(`${window.location.origin}?id=${publicId}`)
      .then(
        () => {
          setLastClick("copy");
        },
        () => {
          console.error("Failed to copy");
        }
      );
  };

  return (
    <>
      <section className="section">
        <div className="container is-max-desktop">
          <div className="columns">
            <div className="column is-three-fifths">
              <label className="label">Workout Name</label>
              <div className="field has-addons">
                <span className="control">
                  <a
                    className={`button ${isTimingLocked ? "is-success" : ""}`}
                    onClick={() => setIsTimingLocked(!isTimingLocked)}
                  >
                    <span className="material-symbols-outlined">lock</span>
                  </a>
                </span>
                <Autocomplete
                  key={timing}
                  placeholder="Search for timing"
                  data={timings.sort()}
                  value={timing}
                  onSelect={handleTimingChange}
                  controlClassName="is-expanded"
                />
              </div>
            </div>
            <div className="column">
              <label className="label">Focus</label>
              <div className="field has-addons">
                <span className="control">
                  <a
                    className={`button ${isFocusLocked ? "is-success" : ""}`}
                    onClick={() => setIsFocusLocked(!isFocusLocked)}
                  >
                    <span className="material-symbols-outlined">lock</span>
                  </a>
                </span>
                <div className="control">
                  <div className="select">
                    <select value={focus} onChange={handleFocusChange}>
                      <option>Cardio</option>
                      <option>Strength</option>
                      <option>Hybrid</option>
                    </select>
                  </div>
                </div>
              </div>
            </div>
            <div className="column" style={{ marginTop: "auto" }}>
              <div className="field">
                <p className="control">
                  <a
                    className={`button is-info ${!workout && "is-pulse"}`}
                    onClick={getCustomizedWorkout}
                  >
                    {!!workout ? "Regenerate" : "Generate!"}
                  </a>
                </p>
              </div>
            </div>
          </div>
          <div className="content">
            {workout && (
              <p>
                <strong>Name:</strong> {workout.name} &middot;{" "}
                <strong>Stations:</strong> {workout.stations} &middot;{" "}
                <strong>Timing:</strong> {workout.timing}
              </p>
            )}
            {workout?.notes && (
              <p className="is-italic">Note: {workout.notes}</p>
            )}
            <p className="buttons is-right">
              {publicId && (
                <>
                  <a
                    className="button"
                    onClick={handleFavoriteClick}
                    data-tooltip={
                      lastClick == "favorite" ? "Favorited!" : "Add Favorite"
                    }
                    onMouseOut={() => setLastClick("")}
                  >
                    <span className="icon is-small">
                      <span
                        className={`material-symbols-outlined ${
                          isFavorite ? "filled-star" : ""
                        }`}
                      >
                        star
                      </span>
                    </span>
                  </a>
                  <a
                    className="button"
                    onClick={handleCopyClick}
                    title="Copy Link"
                    data-tooltip={lastClick == "copy" ? "Copied!" : "Copy URL"}
                    onMouseOut={() => setLastClick("")}
                  >
                    <span className="icon is-small">
                      <span className="material-symbols-outlined">link</span>
                    </span>
                  </a>
                </>
              )}
              <a
                className="button"
                onClick={toggleAdvancedOptions}
                data-tooltip="Advanced Options"
              >
                <span className="icon is-small">
                  <span className=" material-symbols-outlined">settings</span>
                </span>
              </a>
            </p>
          </div>
        </div>
      </section>
      <section>
        <div className="container is-max-desktop">
          <table className="table is-fullwidth is-narrow">
            <thead>
              <tr>
                <th>Station</th>
                <th>Exercise</th>
                <th>Focus</th>
                <th>Equipment</th>
              </tr>
            </thead>
            <tbody>
              {workout &&
                workout.exercises.map((row) => (
                  <tr key={row.station}>
                    <td>{row.station}</td>
                    <td>{row.exercise}</td>
                    <td>{row.focus}</td>
                    <td
                      className={
                        uniqueEquipment(row.equipment)
                          ? "has-background-warning"
                          : ""
                      }
                    >
                      {row.equipment}
                    </td>
                  </tr>
                ))}
            </tbody>
          </table>
        </div>
      </section>

      <div
        className={`modal ${isAdvancedModalShown ? "is-active" : ""}`}
        id="advanced-options"
      >
        <div className="modal-background"></div>
        <div className="modal-content">
          <div className="box">
            <h3 className="title is-4 is-spaced">Advanced Options</h3>
            <label className="label">Randomization</label>
            <div className="field has-addons">
              <p className="control">
                <button className="button" disabled>
                  <span className="icon is-small">
                    <span className="material-symbols-outlined">exercise</span>
                  </span>
                  <span>By Equipment</span>
                </button>
              </p>
              <p className="control">
                <button className="button is-primary is-selected">
                  <span className="material-symbols-outlined"> sprint </span>
                  <span>&nbsp;By Exercise</span>
                </button>
              </p>
            </div>
            <div className="field">
              <label className="label">Equipment Presets</label>
              <div className="control">
                <div className="select">
                  <select
                    value={equipmentPreset}
                    onChange={(e) => setEquipmentPreset(e.target.value)}
                  >
                    <option>All</option>
                    <option>None</option>
                    <option>Bootcamp</option>
                    <option>Calisthenics</option>
                    <option>Just Weights</option>
                    <option>Odd Object Training</option>
                  </select>
                </div>
              </div>
            </div>
            <label className="label">Available Equipment</label>
            <div className="buttons">
              <EquipmentButton
                onClick={handleEquipmentToggle}
                label="Bodyweight"
                selectedEquipment={selectedEquipment}
              ></EquipmentButton>
              <EquipmentButton
                onClick={handleEquipmentToggle}
                label="Dumbbells"
                selectedEquipment={selectedEquipment}
              ></EquipmentButton>
              <EquipmentButton
                onClick={handleEquipmentToggle}
                label="Space to Run"
                selectedEquipment={selectedEquipment}
              ></EquipmentButton>
              {allEquipment
                .filter(
                  (x) =>
                    !["Bodyweight", "Space to Run", "Dumbbells"].includes(x)
                )
                .map((x) => (
                  <EquipmentButton
                    key={x}
                    onClick={handleEquipmentToggle}
                    label={x}
                    selectedEquipment={selectedEquipment}
                  ></EquipmentButton>
                ))}
            </div>
          </div>
        </div>
        <button
          className="modal-close is-large"
          aria-label="close"
          onClick={toggleAdvancedOptions}
        ></button>
      </div>
    </>
  );
}

function EquipmentButton({
  label,
  selectedEquipment,
  onClick,
}: {
  label: string;
  selectedEquipment: string[];
  onClick: (label: string) => void;
}) {
  return (
    <button
      className={`button ${
        selectedEquipment.includes(label) ? "is-primary" : ""
      }`}
      onClick={() => onClick(label)}
    >
      {label}
    </button>
  );
}

export default HomeIndex;
