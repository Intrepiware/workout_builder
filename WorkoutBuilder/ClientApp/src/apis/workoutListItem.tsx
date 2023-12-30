export interface WorkoutListItem {
  id: number;
  publicId: string;
  createDate: string;
  isFavorite: boolean;
  name: string;
}

export function getUserWorkouts(
  skip: number,
  take: number,
  favorites: boolean
): Promise<WorkoutListItem[]> {
  const takeParam = `?take=${take}`;
  const skipParam = skip > 0 ? `&skip=${skip}` : "";
  const favoritesParam = favorites ? "&onlyFavorites=true" : "";
  return fetch(`/Workouts${takeParam}${skipParam}${favoritesParam}`, {
    credentials: "include",
    headers: { "X-Requested-With": "XMLHttpRequest" },
  }).then((res) => res.json());
}
