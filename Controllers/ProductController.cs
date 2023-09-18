﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.Models.ViewModels;

namespace WebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ProductController(ApplicationDbContext _db, IWebHostEnvironment _webHostEnvironment)
        {
            db = _db;
            webHostEnvironment = _webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> objList = db.Products;
            foreach (var obj in objList)
            {
                obj.Category = db.Categories.FirstOrDefault(x => x.CategoryId == obj.CategoryId);
            }
            return View(objList);
        }

        //GET - UpdateAndInsert
        public IActionResult UpdateAndInsert(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new Product(),
                CategorySelectList = db.Categories.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.CategoryId.ToString()
                })
            };
            if (id == null)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = db.Products.Find(id);
                if (productVM.Product == null) return NotFound();
                return View(productVM);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAndInsert(ProductVM productVM)
        {

            var files = HttpContext.Request.Form.Files;
            string webRootPath = webHostEnvironment.WebRootPath; 

            if (productVM.Product.ProductId == 0)
            {
                string upload = webRootPath + WebConstants.ImagePath;
                string fileName = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(files[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                productVM.Product.Image = fileName + extension;
                db.Products.Add(productVM.Product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {

            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        //GET - Delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? CategoryId)
        {
            var category = db.Categories.Find(CategoryId);
            if (category == null)
            {
                return NotFound();
            }
            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
