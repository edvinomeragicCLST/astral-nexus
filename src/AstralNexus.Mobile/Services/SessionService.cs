using AstralNexus.Mobile.Models;

namespace AstralNexus.Mobile.Services;

public sealed class SessionService
{
    public UserEntity? CurrentUser { get; private set; }
    public bool IsLoggedIn => CurrentUser is not null;
    public void SignIn(UserEntity user) => CurrentUser = user;
    public void SignOut() => CurrentUser = null;
}
