/*
 * Just because the user is authenticated in the frontend doesn’t mean the backend 
 * will automatically trust the frontend to make requests on behalf of the user.
 * We need to make sure we attach the access token as a header on every request 
 * we make to the backend API endpoints that require authorization.
 * We could do this manually in every request, 
 * but it’s easier via a delegating handler like this:
*/

using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace WebAppMVC.Handlers
{
    /// <summary>
    /// Automatically adding the access token from an incomming request to an outgoing request
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    public class AuthorizationHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
           HttpRequestMessage request,
           CancellationToken cancellationToken)
        {
            var httpContext = httpContextAccessor.HttpContext ??
                throw new InvalidOperationException("No HttpContext available from the IHttpContextAccessor!");

            var accessToken = await httpContext.GetTokenAsync("access_token");

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }
     }
}
