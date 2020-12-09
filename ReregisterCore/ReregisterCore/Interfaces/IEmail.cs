using ReregisterCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReregisterCore.Interfaces
{
    public interface IEmail
    {
        Task SendAsync(string emailTo, string body, string subject, EmailOptionsDTO options);
    }
}
