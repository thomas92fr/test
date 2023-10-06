using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyWebApi.Models
{
     /// <summary>
     /// Représente une biere
     /// </summary>
    public class Biere
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Nom { get; set; }

        public decimal DegreAlcool { get; set; }

        public decimal Prix { get; set; }

        /// <summary>
        /// Id unique de la brasserie qui réalise cette biere
        /// </summary>
        [Required]
        public int BrasserieId { get; set; }

        /// <summary>
        /// Brasserie qui réalise cette biere
        /// </summary>        
        [JsonIgnore]
        public Brasserie? Brasserie { get; set; }

        /// <summary>
        /// Liste des grossistes
        /// </summary>        
        public ICollection<GrossisteBiere>? GrossisteBieres { get; set; }
    }
}
