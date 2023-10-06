using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Models;

namespace MyWebApi.Services
{
    /// <summary>
    /// Service chargée des opération sur les données des grossistes
    /// </summary>
    public class GrossistesService : IGrossistesService
    {
        private readonly AppDbContext _context;

        public GrossistesService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retourne la liste de tous les grossistes
        /// </summary>
        public async Task<IEnumerable<Grossiste>> GetAllGrossistesAsync()
        {
            return await _context.Grossistes.ToListAsync();
        }

        /// <summary>
        /// Retourne les données du grossiste portant l'id en parametre 
        /// </summary>
        public async Task<Grossiste?> GetGrossisteByIdAsync(int id)
        {
            return await _context.Grossistes.FindAsync(id);
        }

        /// <summary>
        /// Calcul le montant d'une commande de bieres pour un grossiste
        /// </summary>
        /// <param name="grossisteId">Identifiant unique du grossiste concerné par le devis</param>
        /// <param name="commande">Liste des bieres commandées avec la quantité</param>
        public async Task<Devis> DemanderDevisAsync(int grossisteId, List<DevisLigne> commande)
        {
            var grossiste = await _context.Grossistes.FindAsync(grossisteId);

            if (grossiste == null)
            {
                throw new ArgumentException($"Le grossiste {grossisteId} n'existe pas.");
            }

            if (commande == null || !commande.Any())
            {
                throw new ArgumentException("La commande ne peut pas être vide.");
            }

            if (commande.GroupBy(l => l.BiereId).Where(g => g.Count() > 1 /* On regroupe les lignes par BiereId et on compte le nombre de ligne par groupe */).Any())
            {
                throw new ArgumentException("Il ne peut pas y avoir de doublon dans la commande.");
            }
          

            decimal prixTotal = 0m;
            int nombreTotalBoissons = 0;

            var recap = new System.Text.StringBuilder("Récapitulatif de la commande: \n");

            foreach (DevisLigne ligne in commande)
            {
                var biereId = ligne.BiereId;
                var quantite = ligne.Quantite;

                var stock = await _context.GrossisteBieres
                    .Include(gb => gb.Biere)
                    .Where(gb => gb.GrossisteId == grossisteId && gb.BiereId == biereId)
                    .FirstOrDefaultAsync();

                if (stock == null)
                {
                    throw new ArgumentException($"La bière avec l'ID {biereId} n'est pas vendue par ce grossiste.");
                }

                if (stock.QuantiteEnStock < quantite)
                {
                    throw new ArgumentException($"Le nombre de bières commandé est supérieur au stock du grossiste pour la bière {biereId}.");
                }

                prixTotal += stock.Biere!.Prix * quantite;
                nombreTotalBoissons += quantite;

                recap.AppendLine($"- {stock.Biere.Nom}: {quantite} unités");
            }

            //on calcul la réduction par raport au nombre total de bieres commandé

            if (nombreTotalBoissons > 20)
            {
                prixTotal *= 0.8m;
            }
            else if (nombreTotalBoissons > 10)
            {
                prixTotal *= 0.9m;
            }

            return new Devis
            {
                Prix = prixTotal,
                Recapitulatif = recap.ToString()
            };
        }

        /// <summary>
        /// Permet de mettre en vente une quantité d'une biere chez un grossiste, ou de modifier la quantité en stock si la biere est déja en stock
        /// </summary>
        /// <param name="grossisteId">Identifiant unique du grossiste concerné</param>
        /// <param name="biereId">Identifiant unique de la biere concernée</param>
        /// <param name="quantiteEnStock">Quantité en stock a créer/modifier</param>
        public async Task MajGrossisteBiereQuantiteEnStockAsync(int grossisteId, int biereId, int quantiteEnStock)
        {
            var grossiste = await _context.Grossistes.FindAsync(grossisteId);

            if (grossiste == null)
            {
                throw new ArgumentException($"Le grossiste {grossisteId} n'existe pas.");
            }

            Biere? biere = await _context.Bieres.FindAsync(biereId);

            if (biere == null)
            {
                throw new ArgumentException($"La biere {biereId} n'existe pas.");
            }

            var stock = await _context.GrossisteBieres.FirstOrDefaultAsync(gb => gb.GrossisteId == grossisteId && gb.BiereId == biereId);

            if(stock == null)
            {
                await _context.GrossisteBieres.AddAsync(new GrossisteBiere() { 
                    GrossisteId = grossisteId,
                    BiereId = biereId,
                    QuantiteEnStock = quantiteEnStock   
                });
            }
            else
            {
                stock.QuantiteEnStock = quantiteEnStock;
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Ajout d'un grossiste dans la BDD
        /// </summary>
        public async Task AddGrossisteAsync(Grossiste newGrossiste)
        {
            _context.Grossistes.Add(newGrossiste);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Modification d'un grossiste dans la BDD
        /// </summary>
        public async Task UpdateGrossisteAsync(Grossiste updatedGrossiste)
        {
            _context.Grossistes.Update(updatedGrossiste);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Suppresion d'un grossiste dans la BDD
        /// </summary>
        public async Task DeleteGrossisteAsync(int id)
        {
            Grossiste? grossiste = await _context.Grossistes.FindAsync(id);
            if (grossiste != null)
            {
                _context.GrossisteBieres.RemoveRange(_context.GrossisteBieres.Where(gb => gb.GrossisteId == grossiste.Id).ToArray());
                _context.Grossistes.Remove(grossiste);
                await _context.SaveChangesAsync();
            }
        }
    }
}
