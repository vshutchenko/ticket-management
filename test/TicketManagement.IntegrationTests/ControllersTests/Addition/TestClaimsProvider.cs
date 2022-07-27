using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace TicketManagement.IntegrationTests.ControllersTests.Addition
{
    public class TestClaimsProvider
    {
        public TestClaimsProvider(IList<Claim> claims)
        {
            Claims = claims;
        }

        public TestClaimsProvider()
        {
            Claims = new List<Claim>();
        }

        public IList<Claim> Claims { get; }

        public static TestClaimsProvider WithEventManagerClaims()
        {
            var provider = new TestClaimsProvider();
            provider.Claims.Add(new Claim(ClaimTypes.Email, "eventManager@gmail.com"));
            provider.Claims.Add(new Claim(ClaimTypes.Role, "Event manager"));
            provider.Claims.Add(new Claim("id", "ae6af83f-d680-4a71-9af5-6ec65c06f5b6"));
            provider.Claims.Add(new Claim("timezoneId", "Eastern Standard Time"));
            provider.Claims.Add(new Claim("culture", "en-US"));

            return provider;
        }

        public static TestClaimsProvider WithUserClaims()
        {
            var provider = new TestClaimsProvider();
            provider.Claims.Add(new Claim(ClaimTypes.Email, "user@gmail.com"));
            provider.Claims.Add(new Claim(ClaimTypes.Role, "User"));
            provider.Claims.Add(new Claim("id", "d33655d7-af47-49c7-a004-64969e5b651f"));
            provider.Claims.Add(new Claim("timezoneId", "Eastern Standard Time"));
            provider.Claims.Add(new Claim("culture", "en-US"));

            return provider;
        }
    }
}
