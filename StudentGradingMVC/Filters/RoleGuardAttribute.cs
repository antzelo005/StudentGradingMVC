using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class RoleGuardAttribute : ActionFilterAttribute
{
    private readonly string _role;

    public RoleGuardAttribute(string role)
    {
        _role = role ?? "";
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var roleCookie = context.HttpContext.Request.Cookies["UserRole"];

        var actual = (roleCookie ?? "").Trim();
        var expected = _role.Trim();

        if (!string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }
    }
}
