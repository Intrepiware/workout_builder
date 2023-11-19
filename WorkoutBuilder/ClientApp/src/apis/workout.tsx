export interface Workout {
  name: string;
  focus: string;
  stations: number;
  timing: string;
  notes: null | string;
  exercises: Exercise[];
  version: string;
}

export interface Exercise {
  station: string;
  exercise: string;
  focus: string;
  equipment: string;
  notes: null | string;
}

export function getWorkout(
  timing: null | string,
  focus: null | string
): Promise<Workout> {
  timing = encodeURIComponent(timing || "");
  focus = encodeURIComponent(focus || "");
  return fetch(`/Home/Workout?timing=${timing}&focus=${focus}`)
    .then((res) => res.json())
    .then((res) => {
      res.version = "v1";
      return res;
    });
}
