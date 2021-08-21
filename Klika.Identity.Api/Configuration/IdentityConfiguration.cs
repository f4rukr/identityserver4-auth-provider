using IdentityServer4;
using IdentityServer4.Models;
using Klika.Identity.Model.Constants.IdentityConfig;
using System;
using System.Collections.Generic;

namespace Klika.Identity.Api.Configuration
{
    public class IdentityConfiguration
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetResourceApis()
        {
            return new List<ApiResource>
            {
                new ApiResource(name: InternalApis.DineroApi, displayName: "Dinero Resource API") { Scopes = new List<string>() { InternalApis.DineroApi } },
                new ApiResource(name: InternalApis.IdentityServer, displayName: "Identity Server API") { Scopes = new List<string>() { InternalApis.IdentityServer } }
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new[]
            {
                new ApiScope(name: InternalApis.DineroApi,   displayName: "Dinero Resource Api Access"),
                new ApiScope(name: InternalApis.IdentityServer,   displayName: "Identity Server Api Access")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client()
                {
                    ClientId = InternalClients.DineroCcFlow,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret(Environment.GetEnvironmentVariable("dinero_cc_flow_secret").Sha256()) },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        InternalApis.IdentityServer
                    }
                },
                new Client()
                {
                    ClientId = InternalClients.Mobile,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = { new Secret(Environment.GetEnvironmentVariable("mobile_client_secret").Sha256()) },
                    AllowedScopes = new List<string>
                    {
                        InternalApis.DineroApi,
                        InternalApis.IdentityServer,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                },
                new Client()
                {
                    ClientId = InternalClients.Web,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = { new Secret(Environment.GetEnvironmentVariable("web_client_secret").Sha256()) },
                    AllowedScopes = new List<string>
                    {
                        InternalApis.DineroApi,
                        InternalApis.IdentityServer,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                }
            };
        }
    }
}
