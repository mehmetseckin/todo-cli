namespace Todo.Core.Repository;

using Microsoft.Kiota.Abstractions.Authentication;

public abstract class RepositoryBase
{
    protected IAuthenticationProvider AuthenticationProvider { get; }

    protected RepositoryBase(IAuthenticationProvider authenticationProvider)
    {
        AuthenticationProvider = authenticationProvider;
    }
}