using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApi.Controllers;
using MyWebApi.Data;
using MyWebApi.Models;
using MyWebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWebApiTests.Controllers
{
    public class BieresControllerTests
    {
        private AppDbContext _context;
        private BieresController _controller;
        private BieresService _service; 

        [SetUp]
        public void Setup()
        {
            // Configurer le DbContext en mémoire
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "BiereTestDB")
                .Options;

            _context = new AppDbContext(options);
            _context.Database.EnsureDeleted();  // Efface la base de données en mémoire avant chaque test
            _context.Database.EnsureCreated();  // Crée une nouvelle base de données en mémoire avant chaque test

            _service = new BieresService(_context);  
            _controller = new BieresController(_service);
        }

        [Test]
        public async Task GetAllBieres_RetourneTouteLesBieres()
        {
            //on ajoute 2 bieres
            _context.Brasseries.Add(new Brasserie() { Id = 1, Nom = "Abbaye de Leffe" });
            _context.Bieres.Add(new Biere() { Id = 1, Nom = "Leffe Blonde", DegreAlcool = 6.60m, Prix = 2.20m, BrasserieId = 1 });
            _context.Bieres.Add(new Biere() { Id = 2, Nom = "Leffe Blonde 2", DegreAlcool = 6.60m, Prix = 2.20m, BrasserieId = 1 });
            _context.SaveChanges();

            //on appel la fonction
            var result = await _controller.GetAllBieres();
            var okResult = result as OkObjectResult;

            //on test
            Assert.IsNotNull(okResult);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            var bieres = okResult.Value as List<Biere>;
            Assert.That(bieres!.Count, Is.EqualTo(2));
        }
      
    }
}
