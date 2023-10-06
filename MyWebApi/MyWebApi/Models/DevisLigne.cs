using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Models
{
    /// <summary>
    /// Représente une ligne de contenu d'une commande 
    /// </summary>
    public class DevisLigne 
    {
        /// <summary>
        /// Identifiant unique de la biere commandée
        /// </summary>
        [Required]
        public int BiereId { get; set; }

        /// <summary>
        /// Quantité commandée
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "La quantité commandée doit être supérieure ou égale à zéro.")]
        public int Quantite{ get; set; }
    }
}
