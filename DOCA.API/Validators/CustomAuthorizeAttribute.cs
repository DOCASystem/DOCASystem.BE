using DOCA.API.Enums;
using DOCA.API.Utils;
using Microsoft.AspNetCore.Authorization;

namespace DOCA.API.Validators;

public class CustomAuthorizeAttribute : AuthorizeAttribute
{
    public CustomAuthorizeAttribute(params RoleEnum[] roleEnums)
    {
        var allowedRoleAsString = roleEnums.Select(x => x.GetDescriptionFromEnum());
        Roles = string.Join(",", allowedRoleAsString);
    }
}