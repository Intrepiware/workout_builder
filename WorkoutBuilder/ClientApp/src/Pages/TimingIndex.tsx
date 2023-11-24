import { useState, useRef, useEffect } from "react";
import "./TimingIndex.css";

function TimingIndex() {
  const emptyTiming = { stations: 0, work: 0, rest: 0, hydration: 0 };
  const [timings, setTimings] = useState([emptyTiming]);
  const nextInput = useRef(null);

  // Set input focus when the add/rem buttons are pressed
  useEffect(() => nextInput?.current?.focus(), [timings.length]);

  const regExNumber = new RegExp("^[0-9]*$");
  const onNumberFieldChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (regExNumber.test(e.target.value)) {
      setTimings((old) => {
        const state = [...old];
        const newValue =
          e.target.value === "" ? "" : parseInt(e.target.value, 10);

        const stateIndex: number = +e.target.dataset.row;
        if (!isNaN(stateIndex) && e.target.dataset.name)
          state[stateIndex][e.target.dataset.name] = newValue;
        return state;
      });
    }
  };

  const addRow = () =>
    setTimings((old) => {
      return [...old, emptyTiming];
    });

  const remRow = (idx: number) => {
    setTimings((old) => {
      if (old.length == 1) return [emptyTiming];
      const state = [...old];
      state.splice(idx, 1);
      return state;
    });
  };

  const calcTotalTime = () => {
    let total = 0,
      rest = 0,
      work = 0;
    timings.forEach((x) => {
      total += x.stations * (x.rest + x.work);
      work += x.stations * x.work;
      rest += x.stations * x.rest;
      if (x.hydration > 0) {
        rest += -x.rest + x.hydration;
        total += -x.rest + x.hydration;
      }
    });
    // No rest on the very last station
    rest -= timings[timings.length - 1].rest;
    total -= timings[timings.length - 1].rest;
    return {
      rest: new Date(rest * 1000).toISOString().substring(14, 19),
      work: new Date(work * 1000).toISOString().substring(14, 19),
      total: new Date(total * 1000).toISOString().substring(14, 19),
    };
  };

  const { rest, work, total } = calcTotalTime();

  return (
    <section className="section">
      <div className="container is-max-desktop">
        <div className="content">
          <p>
            This tool can be used to calculate the total work and rest time for
            a workout. Use it to try new ideas for workout timings.
          </p>
          <p>
            <strong>Total Work:</strong> {work}
            &nbsp;&middot;&nbsp;
            <strong>Total Rest:</strong> {rest}
            &nbsp;&middot;&nbsp;
            <strong>Total Time:</strong> {total}
          </p>
        </div>
        <div className="table-container">
          <table className="table">
            <thead>
              <tr className="is-hidden-touch">
                <th>Stations</th>
                <th>Work</th>
                <th>Rest</th>
                <th>Hydration</th>
                <th></th>
              </tr>
              <tr className="is-hidden-desktop">
                <th>
                  <span className="material-symbols-outlined" title="Stations">
                    location_on
                  </span>
                </th>
                <th>
                  <span className="material-symbols-outlined" title="Work">
                    sprint
                  </span>
                </th>
                <th>
                  <span className="material-symbols-outlined" title="Rest">
                    hotel
                  </span>
                </th>
                <th>
                  <span className="material-symbols-outlined" title="Hydration">
                    water_drop
                  </span>
                </th>
              </tr>
            </thead>
            <tbody>
              {timings.map((t, idx) => (
                <tr key={idx}>
                  <td>
                    <input
                      className="input"
                      type="text"
                      data-row={idx}
                      data-name="stations"
                      onChange={onNumberFieldChange}
                      value={t.stations}
                      ref={idx == timings.length - 1 ? nextInput : null}
                    ></input>
                  </td>
                  <td>
                    <input
                      className="input"
                      type="text"
                      data-row={idx}
                      data-name="work"
                      onChange={onNumberFieldChange}
                      value={t.work}
                    ></input>
                  </td>
                  <td>
                    <input
                      className="input"
                      type="text"
                      data-row={idx}
                      data-name="rest"
                      onChange={onNumberFieldChange}
                      value={t.rest}
                    ></input>
                  </td>
                  <td>
                    <input
                      className="input"
                      type="text"
                      data-row={idx}
                      data-name="hydration"
                      onChange={onNumberFieldChange}
                      value={t.hydration}
                    ></input>
                  </td>
                  <td>
                    <button
                      className="button is-danger"
                      onClick={() => remRow(idx)}
                    >
                      <span className="icon is-small">
                        <span className="material-symbols-outlined">
                          remove
                        </span>
                      </span>
                    </button>
                    {idx == timings.length - 1 && (
                      <button className="button is-success" onClick={addRow}>
                        <span className="icon is-small">
                          <span className="material-symbols-outlined">add</span>
                        </span>
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </section>
  );
}

export default TimingIndex;
