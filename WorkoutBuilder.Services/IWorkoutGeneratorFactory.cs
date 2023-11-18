﻿using WorkoutBuilder.Data;
using WorkoutBuilder.Services.Models;

namespace WorkoutBuilder.Services
{
    public interface IWorkoutGeneratorFactory
    {
        Timing GetTiming(string timingName);
        IWorkoutService GetGenerator(Timing timing);
    }
}
