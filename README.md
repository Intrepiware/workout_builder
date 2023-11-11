# Workout Builder

**Working Example:** [https://mdb-workoutbuilder.azurewebsites.net/](https://mdb-workoutbuilder.azurewebsites.net/)https://mdb-workoutbuilder.azurewebsites.net/)

## Coding Patterns

There are two unusual coding patterns in this project that are worth pointing out.

- **Property Injection:** Dependencies are injected via properties on the services/controllers, versus the constructor.
- **MVC with React Components:** The best of both worlds. Each Razor view loads a page-specific javascript file, which contains a React component. As more Views are added, each one will have its own page-specific javascript file. This is accomplished by tweaking webpack to have multiple entry points (see below)

https://github.com/Intrepiware/workout_builder/blob/160727a60edc97e1ea527e4155edfd70c2e5d856/WorkoutBuilder/ClientApp/config/webpack.config.js#L8
