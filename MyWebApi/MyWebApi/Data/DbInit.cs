using MyWebApi.Models;

namespace MyWebApi.Data
{
    public static class DbInit
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            context.Brasseries.Add(new Brasserie() {Id=1, Nom = "Abbaye de Leffe" });
            context.Bieres.Add(new Biere() { Id = 1, Nom = "Leffe Blonde", DegreAlcool = 6.6m, Prix = 2.20M , BrasserieId = 1});
            context.Grossistes.Add(new Grossiste() { Id = 1, Nom = "GeneDrinks" });
            context.GrossisteBieres.Add(new GrossisteBiere() { BiereId = 1 , GrossisteId = 1, QuantiteEnStock = 20 });

            context.Brasseries.Add(new Brasserie() { Id = 2, Nom = "Brasseries Georges" });
            context.Bieres.Add(new Biere() { Id = 2, Nom = "George Blonde", DegreAlcool = 7.6m, Prix = 4.20M, BrasserieId = 2 });
            context.GrossisteBieres.Add(new GrossisteBiere() { BiereId = 2, GrossisteId = 1, QuantiteEnStock = 40 });

            context.SaveChanges();
        }
    }
}
