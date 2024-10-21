using Checklist_API.Features.Checklists.Entity;
using System.ComponentModel.DataAnnotations;

namespace Checklist_API.Features.JWT.Entity;

public class Role
{
    public string RoleName { get; init; } = string.Empty;    

    public virtual ICollection<UserRole>? UserRoles { get; set; } = new List<UserRole>();
}