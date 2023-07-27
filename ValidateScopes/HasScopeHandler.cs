using Microsoft.AspNetCore.Authorization;

namespace App.ValidateScopes;
public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
{   // BUG en el FindFirst, en lla linea 21.
    // Devuelve el primer claim del tipo permissions concidente con el issuer.
    // Si el array con tiene 3 premisos, ej create, read, update ... SOLO DEVOLVIA CREATE !!!!
    // Este middleware es llamado por el contrioller. El bug no permiti el ingreso al metodo get, con permiso
    // read, por que el findfirst SIEMPRE devuelve la primera ocurrencia, que en el ejmplo es create.
    // Se agrega en el findfirst ademas del tipo, issuer concida el valor del claim.
    // LISTED: 27_7_2023 14:47 
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
    {
        // If user does not have the scope claim, get out of here
        if (!context.User.HasClaim(c => c.Type == "scope" && c.Issuer == requirement.Issuer))
            return Task.CompletedTask;

        // Split the scopes string into an array
        //var scopes = context.User.FindFirst(c => c.Type == "permissions" && c.Issuer == requirement.Issuer).Value.Split(' ');
        var x = context.User.FindFirst(c => c.Type == "permissions" && c.Issuer == requirement.Issuer && c.Value==requirement.Scope);
        if(x != null){
            var scopes = x.Value.Split(' ');
            // Succeed if the scope array contains the required scope
            if (scopes.Any(s => s == requirement.Scope))
                context.Succeed(requirement);    
        }

        return Task.CompletedTask;
    }
}