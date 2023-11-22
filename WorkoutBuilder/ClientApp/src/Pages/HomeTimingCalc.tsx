import { useState, useEffect } from "react";
import React from "react";

function HomeTimingCalc() {
  const emptyTiming = { stations: 0, work: 0, rest: 0, hydration: 0 };
  const [timings, setTimings] = useState([emptyTiming]);

  const regExNumber = new RegExp("^[0-9]*$");
  const onNumberFieldChange = (e) => {
    if (regExNumber.test(e.target.value)) {
      setTimings((old) => {
        const state = [...old];
        const newValue =
          e.target.value === "" ? "" : parseInt(e.target.value, 10);
        state[e.target.dataset.row][e.target.dataset.name] = newValue;
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
    let seconds = 0;
    timings.forEach((x) => {
      seconds += x.stations * (x.rest + x.work);
      if (x.hydration > 0) seconds += -x.rest + x.hydration;
    });
    return new Date(seconds * 1000).toISOString().substring(14, 19);
  };

  return (
    <section className="section">
      <p>
        <strong>Total Time:</strong> {calcTotalTime()}
      </p>
      <div className="table-container">
        <table className="table">
          <thead>
            <tr>
              <th>Stations</th>
              <th>Work</th>
              <th>Rest</th>
              <th>Hydration</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {timings.map((t, idx) => (
              <tr>
                <td>
                  <input
                    className="input"
                    type="text"
                    data-row={idx}
                    data-name="stations"
                    onChange={onNumberFieldChange}
                    value={t.stations}
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
                  <a
                    className="button is-danger"
                    onClick={() => remRow(idx)}
                    href="javascript:void(0)"
                  >
                    <span className="material-symbols-outlined">remove</span>
                  </a>
                  {idx == timings.length - 1 && (
                    <a
                      className="button is-success"
                      onClick={addRow}
                      href="javascript:void(0)"
                    >
                      <span className="material-symbols-outlined">add</span>
                    </a>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  );
}

export default HomeTimingCalc;
