import { useState, useEffect } from "react";
import Autocomplete from "../Components/AutoComplete";
import { WorkoutRootObject, getWorkout, getWorkoutById } from "../apis/workout";
import React from "react";
import "@creativebulma/bulma-tooltip/dist/bulma-tooltip.min.css";
import "./HomeIndex.css";
import { AdvancedOptionsDialog } from "../Components/HomeIndex/AdvancedOptionsDialog";

interface Timing {
  id: bigint;
  name: string;
  stations: number;
  stationTiming: string | null;
  notes: string | null;
  customGenerator: string | null;
}

interface UiElements {
  isFocusLocked: boolean;
  isTimingLocked: boolean;
  isAdvancedModalShown: boolean;
  isFavorite: boolean;
  youtubeUrl: null | string;
  lastClick: string;
  timing: string;
  focus: string;
  selectedEquipment: string[];
  equipmentPreset: string;
}

function HomeIndex(props: any) {
  const [uiElements, setUiElements] = useState<UiElements>({
    isAdvancedModalShown: false,
    isFavorite: false,
    isFocusLocked: false,
    isTimingLocked: false,
    lastClick: "",
    timing: "",
    focus: "Hybrid",
    selectedEquipment: [],
    equipmentPreset: "All",
    youtubeUrl: null,
  });
  const [timings, setTimings] = useState<string[]>([]);
  const [allEquipment, setAllEquipment] = useState<string[]>([]);
  const [, setError] = useState(null);
  const [workout, setWorkout] = useState<WorkoutRootObject | null>(null);

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
        setUiElements((x) => ({ ...x, selectedEquipment: result }));
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
        setWorkout(savedWorkout);
        setUiElements((x) => ({
          ...x,
          timing: workout.name,
          focus: workout.focus,
        }));
      }
    } catch (err) {
      console.error(
        "An error occurred while loading/rendering the saved workout",
        err
      );
    }
  }, []);

  const handleTimingChange = (item: string) => {
    setWorkout(null);
    setUiElements((x) => ({ ...x, isTimingLocked: true, timing: item }));
  };

  const handleFocusChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setWorkout(null);
    setUiElements((x) => ({
      ...x,
      isFocusLocked: true,
      focus: e.target.value,
    }));
  };

  const getCustomizedWorkout = () => {
    const timingParam: string = uiElements.isTimingLocked
      ? uiElements.timing
      : "";
    const focusParam: string = uiElements.isFocusLocked ? uiElements.focus : "";
    const equipmentParam: string = uiElements.selectedEquipment.join("|");
    getWorkout(timingParam, focusParam, equipmentParam).then(
      (result: WorkoutRootObject) => {
        const { workout } = result;
        setWorkout(result);
        setUiElements((x) => ({
          ...x,
          timing: workout.name,
          focus: workout.focus,
          isFavorite: result.isFavorite,
        }));
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
        setWorkout(result);
        setUiElements((x) => ({
          ...x,
          isFavorite: result.isFavorite,
          timing: workout.name,
          focus: workout.focus,
        }));
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
    if (!!workout) {
      fetch(`/Workouts/Favorite/${workout.publicId}`, {
        method: "POST",
        credentials: "include",
      })
        .then((res) => {
          if (res.headers.has("Location"))
            window.location.href = res.headers.get("Location") || "";
          else {
            return res.json();
          }
        })
        .then((json) => {
          setUiElements((x) => ({ ...x, isFavorite: json.isFavorite }));
        });
    }
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
    setUiElements((x) => ({
      ...x,
      isAdvancedModalShown: !x.isAdvancedModalShown,
    }));
  };

  const handleFavoriteClick = () => {
    setFavorite();
    setUiElements((x) => ({ ...x, lastClick: "favorite" }));
  };

  const handleCopyClick = () => {
    if (workout && workout.publicId) {
      navigator.clipboard
        .writeText(`${window.location.origin}?id=${workout.publicId}`)
        .then(
          () => setUiElements((x) => ({ ...x, lastClick: "copy" })),
          () => {
            console.error("Failed to copy");
          }
        );
    }
  };

  return (
    <>
      <AdvancedOptionsDialog
        allEquipment={allEquipment}
        onClose={() =>
          setUiElements((x) => ({ ...x, isAdvancedModalShown: false }))
        }
        onSelectedChange={(eqp) =>
          setUiElements((x) => ({ ...x, selectedEquipment: eqp }))
        }
        selectedEquipment={uiElements.selectedEquipment}
        visible={uiElements.isAdvancedModalShown}
      />
      <section className="section">
        <div className="container is-max-desktop">
          <div className="columns">
            <div className="column is-three-fifths">
              <label className="label">Workout Name</label>
              <div className="field has-addons">
                <span className="control">
                  <a
                    className={`button ${
                      uiElements.isTimingLocked ? "is-success" : ""
                    }`}
                    onClick={() =>
                      setUiElements((x) => ({
                        ...x,
                        isTimingLocked: !x.isTimingLocked,
                      }))
                    }
                  >
                    <span className="material-symbols-outlined">lock</span>
                  </a>
                </span>
                <Autocomplete
                  key={uiElements.timing}
                  placeholder="Search for timing"
                  data={timings.sort()}
                  value={uiElements.timing}
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
                    className={`button ${
                      uiElements.isFocusLocked ? "is-success" : ""
                    }`}
                    onClick={() =>
                      setUiElements((x) => ({
                        ...x,
                        isFocusLocked: !x.isFocusLocked,
                      }))
                    }
                  >
                    <span className="material-symbols-outlined">lock</span>
                  </a>
                </span>
                <div className="control">
                  <div className="select">
                    <select
                      value={uiElements.focus}
                      onChange={handleFocusChange}
                    >
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
                <strong>Name:</strong> {workout.workout.name} &middot;{" "}
                <strong>Stations:</strong> {workout.workout.stations} &middot;{" "}
                <strong>Timing:</strong> {workout.workout.timing}
              </p>
            )}
            {workout?.workout?.notes && (
              <p className="is-italic">Note: {workout.workout.notes}</p>
            )}
            <p className="buttons is-right">
              {workout?.publicId && props.userid > 0 && (
                <>
                  <a
                    className="button"
                    onClick={handleFavoriteClick}
                    data-tooltip={
                      uiElements.lastClick == "favorite"
                        ? uiElements.isFavorite
                          ? "Favorited!"
                          : "Unfavorited!"
                        : "Add Favorite"
                    }
                    onMouseOut={() =>
                      setUiElements((x) => ({ ...x, lastClick: "" }))
                    }
                  >
                    <span className="icon is-small">
                      <span
                        className={`material-symbols-outlined ${
                          uiElements.isFavorite ? "filled-star" : ""
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
                    data-tooltip={
                      uiElements.lastClick == "copy" ? "Copied!" : "Copy URL"
                    }
                    onMouseOut={() =>
                      setUiElements((x) => ({ ...x, lastClick: "" }))
                    }
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
              {workout?.workout.exercises.map((row) => (
                <tr key={row.station}>
                  <td>{row.station}</td>
                  <td>
                    {row.exercise}
                    {row.youtubeUrl && (
                      <span className="material-symbols-sharp filled-help">
                        help
                      </span>
                    )}
                  </td>
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
