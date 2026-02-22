namespace ModuloNet.Application.Auth;

public static class AuthPolicies
{
    public const string AdminOnly = nameof(AdminOnly);
    public const string ParentOnly = nameof(ParentOnly);
    public const string StudentOnly = nameof(StudentOnly);
    public const string ParentOrAdmin = nameof(ParentOrAdmin);
}
