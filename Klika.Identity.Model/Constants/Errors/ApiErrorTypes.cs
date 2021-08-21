using Klika.Identity.Model.Responses;

namespace Klika.Identity.Model.Constants.Errors
{
    public class ApiErrorTypes
    {
        public static ApiError UserNotFoundId { get { return new ApiError(ErrorCodes.UserNotFound, ErrorDescriptions.UserDoesNotExistWithId); }}
        public static ApiError UserNotFoundEmail { get { return new ApiError(ErrorCodes.UserNotFound, ErrorDescriptions.UserDoesNotExistWithEmail); }}
        public static ApiError UserAlreadyVerified { get { return new ApiError(ErrorCodes.UserAlreadyVerified, ErrorDescriptions.UserAlreadyVerified); }}
        public static ApiError FailedToDeliverMail { get { return new ApiError(ErrorCodes.FailedToDeliverMail, ErrorDescriptions.FailedToDeliverMail); }}
    }
}