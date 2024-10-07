using Checklist_API.Features.Checklists.Entity;
using System.ComponentModel.DataAnnotations;

namespace Checklist_API.Features.JWT.Entity;

public class JWTRole
{
    public string RoleName { get; init; } = string.Empty;    

    public virtual ICollection<JWTUserRole>? JWTUserRoles { get; set; } = new List<JWTUserRole>();
}