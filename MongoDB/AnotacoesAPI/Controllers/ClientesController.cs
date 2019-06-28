using MongoDB.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MongoDB.Controllers
{
    public class ClientesController : Controller
    {
        MongoDbContext _mongoDbContext = new MongoDbContext();

        private readonly SQLDbContext _sQLDbContext;

        public ClientesController(SQLDbContext sQLDbContext)
        {
            _sQLDbContext = sQLDbContext;
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            List<Cliente> listaClientes = _mongoDbContext.Clientes.Find(m => true).ToList();
            return View(listaClientes);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var entity = _mongoDbContext.Clientes.Find(m => m.Id == id).FirstOrDefault();
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Cliente cliente)
        {
            if (CPFValidation.Validate(cliente.CPF) && EmailValidation.Validate(cliente.Email))
            {

                if (ModelState.IsValid)
                {
                    Cliente existeCpf = _mongoDbContext.Clientes.Find(m => m.CPF == cliente.CPF).FirstOrDefault();
                    Cliente existeEmail = _mongoDbContext.Clientes.Find(m => m.Email == cliente.Email).FirstOrDefault();

                    if (existeCpf == null && existeEmail == null)
                    {
                        cliente.Id = Guid.NewGuid();

                        _sQLDbContext.Add(cliente);
                        await _sQLDbContext.SaveChangesAsync();

                        _mongoDbContext.Clientes.InsertOne(cliente);

                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return RedirectToAction("Add", "Clientes");
                    }
                }
            }
            return View(cliente);
        }

        [HttpGet]
        public IActionResult Delete(Guid id)
        {
            _mongoDbContext.Clientes.DeleteOne(m => m.Id == id);
            return RedirectToAction("Index", "Clientes");
        }

    }
}
