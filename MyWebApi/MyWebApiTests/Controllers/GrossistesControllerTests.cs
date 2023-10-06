using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApi.Controllers;
using MyWebApi.Data;
using MyWebApi.Models;
using MyWebApi.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWebApiTests.Controllers
{
    public class GrossistesControllerTests
    {

        private AppDbContext _context;
        private GrossistesController _controller;
        private GrossistesService _service; 

        [SetUp]
        public void Setup()
        {

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "BiereTestDB")
                .Options;

            _context = new AppDbContext(options);
            _context.Database.EnsureDeleted();  // Efface la base de données en mémoire avant chaque test
            _context.Database.EnsureCreated();  // Crée une nouvelle base de données en mémoire avant chaque test

            _service = new GrossistesService(_context); 
            _controller = new GrossistesController(_service);
        }

        [Test]
        public async Task GetAllGrossistes_RetourneTouslesGrossistes()
        {
            //on ajoute 2 grossistes
            _context.Grossistes.Add(new Grossiste { Id = 1, Nom = "TestGrossiste" });
            _context.Grossistes.Add(new Grossiste { Id = 2, Nom = "TestGrossiste 2" });
            _context.SaveChanges();

            //on appel la fonction
            var result = await _controller.GetAllGrossistes();
            var okResult = result as OkObjectResult;

            //on test
            Assert.IsNotNull(okResult);        
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            var grossistes = okResult.Value as List<Grossiste>;
            Assert.That(grossistes!.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task Devis_ErreurSiCommandeVide()
        {
            //on ajoute les données
            _context.Brasseries.Add(new Brasserie() { Id = 1, Nom = "Abbaye de Leffe" });
            _context.Bieres.Add(new Biere() { Id = 1, Nom = "Leffe Blonde", DegreAlcool = 6.60m, Prix = 2.20M, BrasserieId = 1 });
            _context.Grossistes.Add(new Grossiste() { Id = 1, Nom = "GeneDrinks" });
            _context.GrossisteBieres.Add(new GrossisteBiere() { BiereId = 1, GrossisteId = 1, QuantiteEnStock = 20 });
            _context.SaveChanges();


            int testGrossisteId = 1;
            var devisLignes = new List<DevisLigne>(); //commande vide

            //on appel la fonction
            var result = await _controller.DemanderDevis(testGrossisteId, devisLignes);
            var badRequestResult = result as BadRequestObjectResult;

            // test
            Assert.IsNotNull(badRequestResult);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));

            var json = JsonConvert.SerializeObject(badRequestResult.Value);
            var errorMessage = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            Assert.IsTrue(errorMessage!["message"] == "La commande ne peut pas être vide.");
        }

        [Test]
        public async Task Devis_ErreurSiGrossiteInexistant()
        {
            //on ajoute les données
            _context.Brasseries.Add(new Brasserie() { Id = 1, Nom = "Abbaye de Leffe" });
            _context.Bieres.Add(new Biere() { Id = 1, Nom = "Leffe Blonde", DegreAlcool = 6.6m, Prix = 2.20M, BrasserieId = 1 });
            _context.Grossistes.Add(new Grossiste() { Id = 1, Nom = "GeneDrinks" });
            _context.GrossisteBieres.Add(new GrossisteBiere() { BiereId = 1, GrossisteId = 1, QuantiteEnStock = 20 });
            _context.SaveChanges();


            int testGrossisteId = 99; //grossiste inexistant
            var devisLignes = new List<DevisLigne>()
            {                  
                new DevisLigne { BiereId = 1, Quantite = 1 }
            };

            //on appel la fonction
            var result = await _controller.DemanderDevis(testGrossisteId, devisLignes);
            var badRequestResult = result as BadRequestObjectResult;

            // test
            Assert.IsNotNull(badRequestResult);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));

            var json = JsonConvert.SerializeObject(badRequestResult.Value);
            var errorMessage = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            Assert.IsTrue(errorMessage!["message"] == "Le grossiste 99 n'existe pas.");
        }

        [Test]
        public async Task Devis_ErreurSiDoublonLignes()
        {
            //on ajoute les données
            _context.Brasseries.Add(new Brasserie() { Id = 1, Nom = "Abbaye de Leffe" });
            _context.Bieres.Add(new Biere() { Id = 1, Nom = "Leffe Blonde", DegreAlcool = 6.6m, Prix = 2.20M, BrasserieId = 1 });
            _context.Grossistes.Add(new Grossiste() { Id = 1, Nom = "GeneDrinks" });
            _context.GrossisteBieres.Add(new GrossisteBiere() { BiereId = 1, GrossisteId = 1, QuantiteEnStock = 20 });
            _context.SaveChanges();


            int testGrossisteId = 1;
            var devisLignes = new List<DevisLigne>()
            {
                new DevisLigne { BiereId = 1, Quantite = 1 },
                new DevisLigne { BiereId = 1, Quantite = 2 } //2x la même biere dans la commande 
            };

            //on appel la fonction
            var result = await _controller.DemanderDevis(testGrossisteId, devisLignes);
            var badRequestResult = result as BadRequestObjectResult;

            // test
            Assert.IsNotNull(badRequestResult);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));

            var json = JsonConvert.SerializeObject(badRequestResult.Value);
            var errorMessage = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            Assert.IsTrue(errorMessage!["message"] == "Il ne peut pas y avoir de doublon dans la commande.");
        }

        [Test]
        public async Task Devis_ErreurSiBiereNonVendue()
        {
            //on ajoute les données
            _context.Brasseries.Add(new Brasserie() { Id = 1, Nom = "Abbaye de Leffe" });
            _context.Bieres.Add(new Biere() { Id = 1, Nom = "Leffe Blonde", DegreAlcool = 6.6m, Prix = 2.20M, BrasserieId = 1 });
            _context.Bieres.Add(new Biere() { Id = 2, Nom = "Leffe Blonde non vendue", DegreAlcool = 6.6m, Prix = 2.20M, BrasserieId = 1 });
            _context.Grossistes.Add(new Grossiste() { Id = 1, Nom = "GeneDrinks" });
            _context.GrossisteBieres.Add(new GrossisteBiere() { BiereId = 1, GrossisteId = 1, QuantiteEnStock = 20 });
            _context.SaveChanges();


            int testGrossisteId = 1;
            var devisLignes = new List<DevisLigne>()
            {
                new DevisLigne { BiereId = 2, Quantite = 1 } //pas de stock/vente de la biere 2 chez le grossiste
            };

            //on appel la fonction
            var result = await _controller.DemanderDevis(testGrossisteId, devisLignes);
            var badRequestResult = result as BadRequestObjectResult;

            // test
            Assert.IsNotNull(badRequestResult);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));

            var json = JsonConvert.SerializeObject(badRequestResult.Value);
            var errorMessage = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            Assert.IsTrue(errorMessage!["message"] == "La bière avec l'ID 2 n'est pas vendue par ce grossiste.");
        }

        [Test]
        public async Task Devis_ErreurSiQuantiteTropGrande()
        {
            //on ajoute les données
            _context.Brasseries.Add(new Brasserie() { Id = 1, Nom = "Abbaye de Leffe" });
            _context.Bieres.Add(new Biere() { Id = 1, Nom = "Leffe Blonde", DegreAlcool = 6.6m, Prix = 2.20M, BrasserieId = 1 });
            _context.Grossistes.Add(new Grossiste() { Id = 1, Nom = "GeneDrinks" });
            _context.GrossisteBieres.Add(new GrossisteBiere() { BiereId = 1, GrossisteId = 1, QuantiteEnStock = 20 });
            _context.SaveChanges();


            int testGrossisteId = 1;
            var devisLignes = new List<DevisLigne>()
            {
                new DevisLigne { BiereId = 1, Quantite = 21 } //quantité en stock 20
            };

            //on appel la fonction
            var result = await _controller.DemanderDevis(testGrossisteId, devisLignes);
            var badRequestResult = result as BadRequestObjectResult;

            // test
            Assert.IsNotNull(badRequestResult);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));

            var json = JsonConvert.SerializeObject(badRequestResult.Value);
            var errorMessage = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            Assert.IsTrue(errorMessage!["message"] == "Le nombre de bières commandé est supérieur au stock du grossiste pour la bière 1.");
        }

        /// <summary>
        /// Test les diférents paliers de réductions : jusqu'a 10 pas de réduc, de 11 a 20 10% de réduc, puis a partir de 21 20% de réduc
        /// </summary>
        [Test]
        public async Task Devis_Reductions()
        {
            //on ajoute les données
            _context.Brasseries.Add(new Brasserie() { Id = 1, Nom = "Abbaye de Leffe" });
            _context.Bieres.Add(new Biere() { Id = 1, Nom = "Leffe Blonde", DegreAlcool = 6.6m, Prix = 2.0M, BrasserieId = 1 });
            _context.Grossistes.Add(new Grossiste() { Id = 1, Nom = "GeneDrinks" });
            _context.GrossisteBieres.Add(new GrossisteBiere() { BiereId = 1, GrossisteId = 1, QuantiteEnStock = 500 });
            _context.SaveChanges();


            int testGrossisteId = 1;
            var devisLignes = new List<DevisLigne>()
            {
                new DevisLigne { BiereId = 1, Quantite = 10 } // jusqu'a 10 pas de réduc
            };

            //on appel la fonction pour une quantité de 10
            var result = await _controller.DemanderDevis(testGrossisteId, devisLignes);
            var okResult = result as OkObjectResult;

            //on test
            Assert.IsNotNull(okResult);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            var json = JsonConvert.SerializeObject(okResult.Value);
            var results = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            var prix = Convert.ToDecimal(results!["Prix"], CultureInfo.InvariantCulture);

            Assert.That(prix, Is.EqualTo((10 * 2.0M * 1))); //pas de réduction


            testGrossisteId = 1;
            devisLignes = new List<DevisLigne>()
            {
                new DevisLigne { BiereId = 1, Quantite = 11 } // 10% de réduc
            };

            //on appel la fonction pour une quantité de 11
            result = await _controller.DemanderDevis(testGrossisteId, devisLignes);
            okResult = result as OkObjectResult;

            //on test
            Assert.IsNotNull(okResult);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            json = JsonConvert.SerializeObject(okResult.Value);
            results = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            prix = Convert.ToDecimal(results!["Prix"], CultureInfo.InvariantCulture);

            Assert.That(prix, Is.EqualTo((11 * 2.0M * 0.9M))); //réduction de 10%

            testGrossisteId = 1;
            devisLignes = new List<DevisLigne>()
            {
                new DevisLigne { BiereId = 1, Quantite = 20 } //10% de réduc
            };

            //on appel la fonction pour une quantité de 20
            result = await _controller.DemanderDevis(testGrossisteId, devisLignes);
            okResult = result as OkObjectResult;

            //on test
            Assert.IsNotNull(okResult);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            json = JsonConvert.SerializeObject(okResult.Value);
            results = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            prix = Convert.ToDecimal(results!["Prix"], CultureInfo.InvariantCulture);

            Assert.That(prix, Is.EqualTo((20 * 2.0M * 0.9M))); //réduction de 10%

            testGrossisteId = 1;
            devisLignes = new List<DevisLigne>()
            {
                new DevisLigne { BiereId = 1, Quantite = 21 } // 20% de réduc
            };

            //on appel la fonction pour une quantité de 21
            result = await _controller.DemanderDevis(testGrossisteId, devisLignes);
            okResult = result as OkObjectResult;

            //on test
            Assert.IsNotNull(okResult);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            json = JsonConvert.SerializeObject(okResult.Value);
            results = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            prix = Convert.ToDecimal(results!["Prix"], CultureInfo.InvariantCulture);

            Assert.That(prix, Is.EqualTo((21 * 2.0M * 0.8M))); //réduction de 20%

        }

       
    }
}
