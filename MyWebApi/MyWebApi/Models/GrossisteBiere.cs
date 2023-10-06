using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyWebApi.Models
{
    /// <summary>
    /// Représente une biere en stock (et vendu) par un grossiste
    /// </summary>
    public class GrossisteBiere
    {
        /// <summary>
        /// Identifiant unique de la biere vendue
        /// </summary>
        [Required]
        public int BiereId { get; set; }

        /// <summary>
        /// biere vendue
        /// </summary>
        [JsonIgnore]
        public Biere? Biere { get; set; }

        /// <summary>
        /// Identifiant unique du grossiste
        /// </summary>
        [Required]
        public int GrossisteId { get; set; }


        [JsonIgnore]
        public Grossiste? Grossiste { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La quantité en stock doit être supérieure ou égale à zéro.")]
        public int QuantiteEnStock { get; set; }
    }
}
