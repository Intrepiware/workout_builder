import { useEffect, useState } from "react";
import { ExerciseListItem, getExercises } from "../apis/exerciseListItem";

interface UiElements {
  name?: string;
  focus?: string;
  equipment?: string;
}

interface QueryCriteria {
  name?: string;
  focus?: string;
  equipment?: string;
  take: number;
  skip: number;
}

function ExercisesIndex() {
  const [uiElements, setUiElements] = useState<UiElements>({});
  const [query, setQuery] = useState<QueryCriteria>({ take: 26, skip: 0 });
  const [exercises, setExercises] = useState<ExerciseListItem[]>([]);
  const [equipment, setEquipment] = useState<string[]>([]);

  useEffect(() => {
    getExercises(
      query.skip,
      query.take,
      query.name,
      query.focus,
      query.equipment
    ).then((data: ExerciseListItem[]) => setExercises(data));
  }, [query]);

  useEffect(() => {
    fetch("/Home/Equipment")
      .then((res) => res.json())
      .then((result: string[]) => {
        setEquipment(result);
      });
  }, []);

  const handleSearchClick = () => {
    setQuery({
      skip: 0,
      take: 26,
      focus: uiElements.focus,
      equipment: uiElements.equipment,
      name: uiElements.name,
    });
  };

  return (
    <section className="section">
      <div className="container is-max-desktop">
        <div className="columns">
          <div className="column">
            <label className="label">Name Search</label>
            <div className="field">
              <p className="control">
                <input
                  type="text"
                  placeholder="Workout Name"
                  className="input"
                  value={uiElements.name}
                  onChange={(e) =>
                    setUiElements((x) => ({ ...x, name: e.target.value }))
                  }
                />
              </p>
            </div>
          </div>
          <div className="column">
            <label className="label">Focus</label>
            <div className="field">
              <p className="select is-fullwidth">
                <select
                  value={uiElements.focus}
                  onChange={(e) =>
                    setUiElements((x) => ({ ...x, focus: e.target.value }))
                  }
                >
                  <option></option>
                  <option>Abs</option>
                  <option>Cardio</option>
                  <option>Hybrid</option>
                  <option>Strength</option>
                </select>
              </p>
            </div>
          </div>
          <div className="column">
            <label className="label">Equipment</label>
            <div className="field">
              <p className="select is-fullwidth">
                <select
                  value={uiElements.equipment}
                  onChange={(e) =>
                    setUiElements((x) => ({ ...x, equipment: e.target.value }))
                  }
                >
                  <option></option>
                  {equipment.map((x) => (
                    <option key={x}>{x}</option>
                  ))}
                </select>
              </p>
            </div>
          </div>
          <div className="column" style={{ marginTop: "auto" }}>
            <div className="field">
              <p className="control">
                <button
                  className="button is-info"
                  onClick={() => handleSearchClick()}
                >
                  Search
                </button>
              </p>
            </div>
          </div>
        </div>
        <p className="buttons is-right">
          <a className="button" data-tooltip="Add Favorite">
            <span className="icon is-small">
              <span className="material-symbols-outlined ">star</span>
            </span>
          </a>
          <a className="button" title="Copy Link" data-tooltip="Copy URL">
            <span className="icon is-small">
              <span className="material-symbols-outlined">link</span>
            </span>
          </a>
          <a className="button" data-tooltip="Advanced Options">
            <span className="icon is-small">
              <span className=" material-symbols-outlined">settings</span>
            </span>
          </a>
        </p>
        <div className="table-container">
          <table className="table is-fullwidth">
            <thead>
              <tr>
                <th>Name</th>
                <th>Focus</th>
              </tr>
            </thead>
            <tbody>
              {exercises.map((x) => (
                <tr key={x.id}>
                  <td>
                    {x.editUrl && <a href={x.editUrl}>{x.name}</a>}
                    {!x.editUrl && x.name}
                  </td>
                  <td>{x.focus}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        <div className="buttons is-right">
          <button
            className="button is-info"
            disabled={query.skip == 0}
            onClick={() => {
              setQuery((x) => ({ ...x, skip: x.skip - 25 }));
              window.scrollTo(0, 0);
            }}
          >
            Prev
          </button>
          <button
            className="button is-info"
            disabled={exercises.length < 26}
            onClick={() => {
              setQuery((x) => ({ ...x, skip: x.skip + 25 }));
              window.scrollTo(0, 0);
            }}
          >
            Next
          </button>
        </div>
      </div>
    </section>
  );
}

export default ExercisesIndex;
