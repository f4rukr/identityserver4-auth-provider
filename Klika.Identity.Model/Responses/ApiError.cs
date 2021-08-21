using System;
using System.Collections.Generic;
using System.Text;

namespace Klika.Identity.Model.Responses
{
    public class ApiError
    {
        public string Code { get; set; }
        public string Description { get; set; }

        public ApiError(string code, string description)
        {
            Code = code;
            Description = description;
        }
    }
}
