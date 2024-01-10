import { useEffect, useState } from "react";
import { WorkoutListItem, getUserWorkouts } from "../apis/workoutListItem";

interface QueryCriteria {
  take: number;
  skip: number;
  favorites: boolean;
}

function WorkoutsIndex() {
  const [query, setQuery] = useState<QueryCriteria>({
    skip: 0,
    take: 26,
    favorites: true,
  });
  const [workouts, setWorkouts] = useState<WorkoutListItem[]>([]);

  useEffect(() => {
    getUserWorkouts(query.skip, query.take, query.favorites).then(
      (data: WorkoutListItem[]) => setWorkouts(data)
    );
  }, [query]);

  const setFavorite = (id: string) => {
    if (!!id) {
      fetch(`/Workouts/Favorite/${id}`, {
        method: "POST",
        credentials: "include",
      })
        .then((res) => res.json())
        .then((json) => {
          const workoutsClone = [...workouts];
          const index = workoutsClone.findIndex((x) => x.publicId == id);
          workoutsClone[index].isFavorite = json.isFavorite;
          setWorkouts(workoutsClone);
        });
    }
  };

  const formatDate = (dateString: string) => {
    const date = new Date(`${dateString}Z`);
    const hoursSince =
      (new Date().getTime() - date.getTime()) / (1000 * 60 * 60);
    return hoursSince >= 24
      ? date.toLocaleDateString()
      : date.toLocaleTimeString();
  };

  return (
    <section className="section">
      <div className="container is-max-desktop">
        <div className="buttons has-addons">
          <button
            className={`button ${query.favorites ? "is-info" : ""}`}
            onClick={() => setQuery((x) => ({ ...x, favorites: true }))}
          >
            Favorites
          </button>
          <button
            className={`button ${query.favorites ? "" : "is-info"}`}
            onClick={() => setQuery((x) => ({ ...x, favorites: false }))}
          >
            All
          </button>
        </div>
        <div className="table-container">
          <table className="table is-fullwidth">
            <thead>
              <tr>
                <th>Name</th>
                <th>Created</th>
                <th className="has-text-centered">Favorite</th>
              </tr>
            </thead>
            <tbody>
              {workouts &&
                workouts.slice(0, 25).map((x) => (
                  <tr key={x.id}>
                    <td>
                      <a href={`${document.location.origin}?id=${x.publicId}`}>
                        {x.name}
                      </a>
                    </td>
                    <td>{formatDate(x.createDate)}</td>
                    <td className="has-text-centered">
                      <a
                        className="button"
                        onClick={() => setFavorite(x.publicId)}
                      >
                        <span className="icon is-small">
                          <span
                            className={`material-symbols-outlined ${
                              x.isFavorite ? "filled-star" : ""
                            }`}
                          >
                            star
                          </span>
                        </span>
                      </a>
                    </td>
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
            disabled={workouts.length < 26}
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

export default WorkoutsIndex;
