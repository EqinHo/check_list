using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Checklist_API.Features.Users.Entity;

namespace Checklist_API.Features.JWT.Entity;

public class JWTUserRole
{
    public string RoleName { get; set; } = string.Empty;
    public UserId UserId { get; set; }   
    public DateTime DateCreated { get; init; }
    public DateTime DateUpdated { get; set; }


    public virtual User? User { get; set; }
    public virtual JWTRole? JWTRole { get; set; }
}