using System.ComponentModel.DataAnnotations;
using Base.Domain;
using Domain.Identity;

namespace Domain;

public class ProductExistence : AbstractIdDatabaseEntity
{
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    [MaxLength(512)] public string? Location { get; set; }
    public float Amount { get; set; }
}