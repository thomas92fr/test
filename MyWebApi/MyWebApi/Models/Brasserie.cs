using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyWebApi.Models
{
    /// <summary>
    /// Représente une brasserie 
    /// </summary>
    public class Brasserie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Nom { get; set; }

        /// <summary>
        /// Liste des bieres de la braserie
        /// </summary>
        public ICollection<Biere>? Bieres { get; set; }
    }
}
