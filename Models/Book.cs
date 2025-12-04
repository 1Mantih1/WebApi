using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Models;

[Index("Isbn", Name = "UQ__Books__447D36EA6CB1C7DB", IsUnique = true)]
public partial class Book
{
    [Key]
    [Column("BookID")]
    public int BookId { get; set; }

    [StringLength(255)]
    public string Title { get; set; } = null!;

    [Column("AuthorID")]
    public int AuthorId { get; set; }

    [Column("ISBN")]
    [StringLength(20)]
    public string? Isbn { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    public int? Pages { get; set; }

    public int StockQuantity { get; set; }

    public bool IsAvailable { get; set; }

    [ForeignKey("AuthorId")]
    [InverseProperty("Books")]
    public virtual Author Author { get; set; } = null!;

    [InverseProperty("Book")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
