using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutBuilder.Services.Models
{
    public class WorkoutGenerationRequestModel
    {
        public string? Timing { get; set; }
        public Focus? Focus { get; set; }

    }
}
