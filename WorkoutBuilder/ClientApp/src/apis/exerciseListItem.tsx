export interface ExerciseListItem {
  id: number;
  name: string;
  focus: string;
  editUrl: string | null;
}

export function getExercises(
  skip: number,
  take: number,
  name?: string | null,
  focus?: string | null,
  equipment?: string | null
): Promise<ExerciseListItem[]> {
  const takeParam = `?take=${take}`;
  const skipParam = skip > 0 ? `&skip=${skip}` : "";
  const nameParam = !!name ? `&name=${name}` : "";
  const focusParam = !!focus ? `&focus=${focus}` : "";
  const equipmentParam = !!equipment ? `&equipment=${equipment}` : "";
  return fetch(
    `/Exercises${takeParam}${skipParam}${nameParam}${focusParam}${equipmentParam}`,
    {
      credentials: "include",
      headers: { "X-Requested-With": "XMLHttpRequest" },
    }
  ).then((res) => res.json());
}
