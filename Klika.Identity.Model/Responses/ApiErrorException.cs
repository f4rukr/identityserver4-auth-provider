using System;
using System.Collections.Generic;

namespace Klika.Identity.Model.Responses
{
    public class ApiErrorException : Exception
    {
        public List<ApiError> Errors;

        public ApiErrorException(string code, string description)
        {
            Errors = new List<ApiError>
            {
                new ApiError(code, description)
            };
        }
        
        public ApiErrorException(ApiError error)
        {
            Errors = new List<ApiError>
            {
                error
            };
        }
        
        public ApiErrorException(List<ApiError> errors)
        {
            Errors = errors;
        }
    }
}
