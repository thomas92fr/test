using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Models
{
    /// <summary>
    /// Représente un grossiste
    /// </summary>
    public class Grossiste
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Nom { get; set; }

        /// <summary>
        /// Liste des bieres vendu par le grossiste , les quantités en stock
        /// </summary>
        public ICollection<GrossisteBiere>? GrossisteBieres { get; set; }
    }
}
