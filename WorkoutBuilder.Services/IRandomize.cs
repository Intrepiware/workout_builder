using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutBuilder.Services
{
    public interface IRandomize
    {
        T GetRandomItem<T>(IEnumerable<T> items);
        double NextDouble();
    }
}
