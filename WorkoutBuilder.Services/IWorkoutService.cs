using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutBuilder.Services.Models;

namespace WorkoutBuilder.Services
{
    public interface IWorkoutService
    {
        WorkoutGenerationResponseModel Generate(WorkoutGenerationRequestModel request);
    }
}
