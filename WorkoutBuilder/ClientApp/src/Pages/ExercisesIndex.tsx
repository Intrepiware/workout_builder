// interface QueryCriteria {
//   take: number;
//   skip: number;
//   favorites: boolean;
// }

function ExercisesIndex() {
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
              <tr>
                <td>
                  <a href="#">Exercise Name</a>
                </td>
                <td>Hybrid</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </section>
  );
}

export default ExercisesIndex;
