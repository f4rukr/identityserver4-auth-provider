using System;

namespace Klika.Identity.Model.Responses
{
    public class ApiResponse
    {
        public bool Succeeded { get; set; }

        public ApiResponse(bool isSuccesful)
        {
            Succeeded = isSuccesful;
        }
    }
}
