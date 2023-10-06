namespace MyWebApi.Models
{
    /// <summary>
    /// Représente la réponse d'un devis demandé a un grossiste
    /// </summary>
    public class Devis
    {
        /// <summary>
        /// Montant total du devis
        /// </summary>
        public decimal Prix { get; set; }

        /// <summary>
        /// Texte représentant le contenu de la commande
        /// </summary>
        public string? Recapitulatif { get; set; }
    }
}
