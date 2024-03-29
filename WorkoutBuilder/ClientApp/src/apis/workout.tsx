export interface WorkoutRootObject {
  publicId: null | string;
  workout: Workout;
  version: string;
  isFavorite: boolean;
}
export interface Workout {
  name: string;
  focus: string;
  stations: number;
  timing: string;
  notes: null | string;
  exercises: Exercise[];
}

export interface Exercise {
  station: string;
  exercise: string;
  focus: string;
  equipment: string;
  notes: null | string;
  youtubeUrl: null | string;
}

export function getWorkout(
  timing: null | string,
  focus: null | string,
  equipment: null | string
): Promise<WorkoutRootObject> {
  timing = encodeURIComponent(timing || "");
  focus = encodeURIComponent(focus || "");
  return fetch(
    `/Home/Workout?timing=${timing}&focus=${focus}&equipment=${equipment}`
  )
    .then((res) => res.json())
    .then((res) => {
      res.version = "v1";
      return res;
    });
}

export function getWorkoutById(id: string): Promise<WorkoutRootObject> {
  return fetch(`/Home/Workout?id=${id}`)
    .then((res) => res.json())
    .then((res) => {
      res.version = "v1";
      return res;
    });
}
