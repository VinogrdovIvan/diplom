using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Shared.Requests
{
    public class VerifyPasswordRequest
    {
        public string CurrentPassword { get; set; }
    }
}
