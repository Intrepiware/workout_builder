import { useState, useEffect } from "react";
import Autocomplete from "../Components/AutoComplete";
import { Workout, getWorkout } from "../apis/workout";
import React from "react";

function HomeIndex() {
  const [timing, setTiming] = useState("");
  const [focus, setFocus] = useState("Hybrid");
  const [isFocusLocked, setIsFocusLocked] = useState(false);
  const [isTimingLocked, setIsTimingLocked] = useState(false);
  const [timings, setTimings] = useState([]);
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
    getWorkout("", "").then(
      (result: Workout) => {
        setWorkout(result);
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
  }, []);

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

  const getCustomizedWorkout = () => {
    const timingParam: string = isTimingLocked ? timing : "";
    const focusParam: string = isFocusLocked ? focus : "";
    getWorkout(timingParam, focusParam).then(
      (result: Workout) => {
        setWorkout(result);
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
    </>
  );
}

export default HomeIndex;
