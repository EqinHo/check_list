﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Checklist_API.Features.Users.Entity;

namespace Checklist_API.Features.JWT.Entity;

public readonly record struct JwtUserRoleId(Guid jwtUserRoleId)
{
    public static JwtUserRoleId NewId => new(Guid.NewGuid());
    public static JwtUserRoleId Empty => new(Guid.Empty);
}

public class JWTUserRole
{
    public JwtUserRoleId Id { get; set; }
    public UserId UserId { get; set; }
    public JwtRoleId JwtRoleId { get; set; }
    public DateTime DateCreated { get; init; }
    public DateTime DateUpdated { get; set; }



    public virtual User? User { get; set; }
    public virtual JWTRole? JWTRole { get; set; }
}