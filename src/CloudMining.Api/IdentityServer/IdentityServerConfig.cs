using Duende.IdentityServer.Models;
using Duende.IdentityServer;
using IdentityModel;

namespace CloudMining.Api.IdentityServer
{
	public static class IdentityServerConfig
	{
		public static IEnumerable<IdentityResource> GetIdentityResources()
		{
			return
			[
				new IdentityResources.OpenId(),
			];
		}

		public static IEnumerable<ApiScope> GetApiScopes()
		{
			return
			[
				new ApiScope("all", "CloudMining API")
			];
		}

		public static IEnumerable<ApiResource> GetApiResources()
		{
			return new List<ApiResource>
			{
				new("cloud-mining-api", "CloudMining API")
				{
					Scopes = { "all" }
				}
			};
		}

		public static IEnumerable<Client> GetClients()
		{
			return
			[
				new Client
				{
					ClientId = "web_app",
					ClientName = "CloudMiningApp",
					AllowedGrantTypes = GrantTypes.Code,
					RequireClientSecret = false,
					AllowedScopes =
					{
						IdentityServerConstants.StandardScopes.OpenId, "all"
					},
					RequirePkce = true,
					RedirectUris = { "http://localhost:8080" },
					AllowedCorsOrigins = { "http://localhost:8080" },
					PostLogoutRedirectUris = { "http://localhost:8080" },
					AllowOfflineAccess = true,
					AllowAccessTokensViaBrowser = true,
					AccessTokenLifetime = 60 * 60 * 24 * 7 //1 неделя
				}
			];
		}
	}
}
