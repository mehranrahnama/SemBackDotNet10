using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SemBackDotNet10.SecurityDomain.Model.SemUserDetail;

/// <summary>
/// ASP.NET Core equivalent of Spring's @CurrentUser annotation.
/// Use [FromServices] or HttpContext.User for controller action parameters.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class CurrentUserAttribute : Attribute, IBindingSourceMetadata
{
    public BindingSource? BindingSource => BindingSource.Custom;
}
