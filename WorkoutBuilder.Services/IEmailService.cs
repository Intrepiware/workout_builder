using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutBuilder.Services
{
    public interface IEmailService
    {
        void Send(string toAddress, string subject, string body);
    }
}
