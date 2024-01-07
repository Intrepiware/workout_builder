import { useEffect, useState } from "react";
import { ExerciseListItem, getExercises } from "../apis/exerciseListItem";
interface QueryCriteria {
  take: number;
  skip: number;
  name?: string;
  focus?: string;
  equipment?: string;
}

function ExercisesIndex() {
  const [query, setQuery] = useState<QueryCriteria>({
    take: 26,
    skip: 0,
  });
  const [exercises, setExercises] = useState<ExerciseListItem[]>([]);

  useEffect(() => {
    getExercises(
      query.skip,
      query.take,
      query.name,
      query.focus,
      query.equipment
    ).then((data: ExerciseListItem[]) => setExercises(data));
  }, [query]);

  return (
    <section className="section">
      <div className="container is-max-desktop">
        <div className="buttons has-addons"></div>

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
