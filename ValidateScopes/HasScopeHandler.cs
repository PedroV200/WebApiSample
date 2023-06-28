using Microsoft.AspNetCore.Authorization;

namespace App.ValidateScopes;
public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
    {
        // If user does not have the scope claim, get out of here
        if (!context.User.HasClaim(c => c.Type == "scope" && c.Issuer == requirement.Issuer))
            return Task.CompletedTask;

        // Split the scopes string into an array
        //var scopes = context.User.FindFirst(c => c.Type == "permissions" && c.Issuer == requirement.Issuer).Value.Split(' ');
        var x = context.User.FindFirst(c => c.Type == "permissions" && c.Issuer == requirement.Issuer);
        if(x != null){
            var scopes = x.Value.Split(' ');
            // Succeed if the scope array contains the required scope
            if (scopes.Any(s => s == requirement.Scope))
                context.Succeed(requirement);    
        }

        return Task.CompletedTask;
    }
}