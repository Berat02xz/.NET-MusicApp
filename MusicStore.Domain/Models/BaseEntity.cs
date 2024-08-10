using System.ComponentModel.DataAnnotations;

namespace MusicStore.Domain.Models
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
