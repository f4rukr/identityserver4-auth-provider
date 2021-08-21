using Microsoft.AspNetCore.Builder;

namespace Klika.Identity.Api.Extensions
{
    public static class ApiEndpointsExtension
    {
        public static void UseApiEndpoints(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
