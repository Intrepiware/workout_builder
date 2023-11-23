import { useState, useEffect } from "react";
import Autocomplete from "../Components/AutoComplete";
import { Workout, getWorkout } from "../apis/workout";
import React from "react";

function HomeIndex() {
  const [timing, setTiming] = useState("");
  const [focus, setFocus] = useState("Hybrid");
  const [isFocusLocked, setIsFocusLocked] = useState(false);
  const [isTimingLocked, setIsTimingLocked] = useState(false);
  const [isAdvancedModalShown, setIsAdvancedModalShown] = useState(false);
  const [timings, setTimings] = useState([]);
  const [allEquipment, setAllEquipment] = useState([]);
  const [selectedEquipment, setSelectedEquipment] = useState([]);
  const [equipmentPreset, setEquipmentPreset] = useState("All");
  const [, setError] = useState(null);
  const [workout, setWorkout] = useState<Workout | null>(null);

  useEffect(() => {
    fetch("/Home/Timings")
      .then((res) => res.json())
      .then(
        (result) => {
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
      .then((result) => {
        setAllEquipment(result);
        setSelectedEquipment(result);
      });
  }, []);

  useEffect(() => {
    try {
      const savedWorkout: Workout = JSON.parse(localStorage.getItem("workout"));
      if (savedWorkout.version === "v1") {
        setWorkout(savedWorkout);
        setTiming(savedWorkout.name);
        setFocus(savedWorkout.focus);
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
          "Room to Run",
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
        setSelectedEquipment(["Dumbbells", "Olympic Barbell", "Small Barbell"]);
        break;
      case "Odd Object Training":
        setSelectedEquipment([
          "Kettlebell",
          "Med Ball",
          "Sandbell",
          "Sandbag - Lg (84.4 lb)",
          "Sandbag - Sm (61.2 lb)",
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
    const selected = [...selectedEquipment];
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
      (result: Workout) => {
        setWorkout(result);
        localStorage.setItem("workout", JSON.stringify(result));
        setTiming(result.name);
        setFocus(result.focus);
      },
      // Note: it's important to handle errors here
      // instead of a catch() block so that we don't swallow
      // exceptions from actual bugs in components.
      (error) => {
        setError(error);
      }
    );
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
                    className="button is-primary"
                    onClick={getCustomizedWorkout}
                  >
                    Regenerate
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
            <p className="is-size-7 has-text-right">
              <a onClick={toggleAdvancedOptions}>Advanced Options</a>
            </p>
          </div>
        </div>
      </section>
      <section>
        <div className="container is-max-desktop">
          <table className="table is-fullwidth">
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
                label="Room to Run"
                selectedEquipment={selectedEquipment}
              ></EquipmentButton>
              <EquipmentButton
                onClick={handleEquipmentToggle}
                label="Dumbbells"
                selectedEquipment={selectedEquipment}
              ></EquipmentButton>
              {allEquipment
                .filter(
                  (x) => !["Bodyweight", "Room to Run", "Dumbbells"].includes(x)
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
