namespace CloudMining.App.Middleware
{
	public class AuthenticationMiddleware : IMiddleware
	{
		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			const string authPath = "/user/login";
			const string registerPath = "/user/register";
			if (context.User.Identity?.IsAuthenticated == false && 
			    context.Request.Path != authPath && 
			    context.Request.Path != registerPath)
			{
				context.Response.Redirect(authPath);
				return;
			}

			await next.Invoke(context);
		}
	}
}
