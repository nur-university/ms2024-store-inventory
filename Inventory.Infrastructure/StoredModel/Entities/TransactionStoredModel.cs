using Inventory.Domain.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.StoredModel.Entities;

[Table("transaction")]
internal class TransactionStoredModel 
{
    [Key]
    [Column("transactionId")]
    public Guid Id { get; set; }

    [Required]
    [Column("userCreatorId")]
    public Guid UserCreatorId { get; set; }

    public User UserCreator { get; set; }

    [Required]
    [Column("transactionType")]
    [MaxLength(25)]
    public string TransactionType { get; set; }

    [Required]
    [Column("CreationDate")]
    public DateTime CreationDate { get; set; }

    [Column("CompletedDate")]
    public DateTime? CompletedDate { get; set; }

    [Column("CancelDate")]
    public DateTime? CancelDate { get; set; }

    [Required]
    [Column("totalCost", TypeName = "decimal(18,2)")]
    public decimal TotalCost { get; set; }

    [Required]
    [Column("status")]
    [MaxLength(25)]
    public string Status { get; set; }


    public List<TransactionItemStoredModel> Items { get; set; }

}
