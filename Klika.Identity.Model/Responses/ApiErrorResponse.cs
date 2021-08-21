using System;
using System.Collections.Generic;
using System.Text;

namespace Klika.Identity.Model.Responses
{
    public class ApiErrorResponse : ApiResponse
    {
        public List<ApiError> Errors { get; set; }

        public ApiErrorResponse() : base (false)
        {
            Errors = new List<ApiError> { };
        }

        public ApiErrorResponse(string type, string description) : base(false)
        {
            Errors = new List<ApiError>
            {
                new ApiError(type, description)
            };
        }

        public ApiErrorResponse(List<ApiError> errors) : base(false)
        {
            Errors = errors;
        }

        public ApiErrorResponse(ApiErrorException ex) : base(false)
        {
            Errors = ex.Errors;
        }
    }
}
