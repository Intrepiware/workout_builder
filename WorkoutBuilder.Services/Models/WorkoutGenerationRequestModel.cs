using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutBuilder.Data;

namespace WorkoutBuilder.Services.Models
{
    public class WorkoutGenerationRequestModel
    {
        public string? TimingName { get; set; }
        public Focus? Focus { get; set; }
        public Timing Timing { get; set; }
    }
}
