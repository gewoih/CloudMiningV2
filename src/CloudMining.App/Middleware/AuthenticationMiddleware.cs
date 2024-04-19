namespace CloudMining.App.Middleware
{
	public class AuthenticationMiddleware : IMiddleware
	{
		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			const string authPath = "/user/login";
			if (context.User.Identity?.IsAuthenticated == false && 
			    !context.Request.Path.StartsWithSegments("/user"))
			{
				context.Response.Redirect(authPath);
				return;
			}

			await next.Invoke(context);
		}
	}
}
