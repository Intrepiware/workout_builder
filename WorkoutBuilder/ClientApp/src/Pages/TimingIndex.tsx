import { useState, useRef, useEffect } from "react";
import "./TimingIndex.css";

interface Timing {
  stations: number;
  work: number;
  rest: number;
  hydration: number;
}

function TimingIndex() {
  const emptyTiming: Timing = { stations: 0, work: 0, rest: 0, hydration: 0 };
  const [timings, setTimings] = useState([emptyTiming]);
  const nextInput = useRef<HTMLInputElement>(null);

  // Set input focus when the add/rem buttons are pressed
  useEffect(() => nextInput?.current?.focus(), [timings.length]);

  const onNumberFieldChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setTimings((old) => {
      const state = [...old];
      const newValue = "" + Number(e.target.value);

      const stateIndex: number = parseInt(e.target.dataset.row || "", 10);
      const propIndex: string = e.target.dataset.name || "";
      if (stateIndex > -1 && !!propIndex)
        state[stateIndex] = { ...state[stateIndex], [propIndex]: newValue };
      return state;
    });
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
    let rest: number = 0,
      work: number = 0;
    timings.forEach((x, idx) => {
      work += x.stations * Number(x.work);
      rest += x.stations * Number(x.rest);
      const hydration = Number(x.hydration);
      if (hydration > 0 && idx < timings.length - 1) {
        rest += -x.rest + hydration;
      }
    });
    // No rest on the very last station
    rest -= timings[timings.length - 1].rest || 0;
    return {
      rest: formatTime(rest),
      work: formatTime(work),
      total: formatTime(rest + work),
    };
  };

  const formatTime = (v: number): string =>
    new Date(v * 1000).toISOString().substring(14, 19);

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
          <table className="table is-narrow" id="timing">
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
                      type="number"
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
                      type="number"
                      data-row={idx}
                      data-name="work"
                      onChange={onNumberFieldChange}
                      value={t.work}
                    ></input>
                  </td>
                  <td>
                    <input
                      className="input"
                      type="number"
                      data-row={idx}
                      data-name="rest"
                      onChange={onNumberFieldChange}
                      value={t.rest}
                    ></input>
                  </td>
                  <td>
                    <input
                      className="input"
                      type="number"
                      data-row={idx}
                      data-name="hydration"
                      onChange={onNumberFieldChange}
                      value={t.hydration}
                    ></input>
                  </td>
                  <td>
                    <button
                      className="button is-danger is-small is-responsive"
                      onClick={() => remRow(idx)}
                    >
                      <span className="icon is-small">
                        <span className="material-symbols-outlined">
                          remove
                        </span>
                      </span>
                    </button>
                    {idx == timings.length - 1 && (
                      <button
                        className="button is-success is-small is-responsive"
                        onClick={addRow}
                      >
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
