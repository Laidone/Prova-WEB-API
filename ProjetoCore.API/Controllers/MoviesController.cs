using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dominio.Models;
using Infra;
using AutoMapper;
using ProjetoCore.API.ViewModels;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace ProjetoCore.API.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext movieDb;
        private readonly IMapper _mapper;
        public MoviesController(ApplicationDbContext context, IMapper mapper)

        {
            movieDb = context;
            _mapper = mapper;
        }
        // GET: Movie
        public ViewResult Index(string searchString, int? SelectedGenre, string sortOrder)
        {
            var genres = movieDb.Genre.OrderBy(g => g.Name).ToList();
            ViewBag.SelectedGenre = new SelectList(genres, "GenreID", "Name", SelectedGenre);
            int genreID = SelectedGenre.GetValueOrDefault();

            var movies = movieDb.Movie
                .Where(c => !SelectedGenre.HasValue || c.GenreID == genreID);

            if (!String.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.Title.Contains(searchString) || s.Director.Contains(searchString));
            }
            ViewBag.RatingSortParm = sortOrder == "Rating" ? "rating_asc" : "Rating";
            switch (sortOrder)
            {
                case "Rating":
                    movies = movies.OrderByDescending(s => s.Rating);
                    break;
                case "rating_asc":
                    movies = movies.OrderBy(s => s.Rating);
                    break;
            }
            return View(movies);
        }
       
        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var movie = await movieDb.Movie
                .Include(m => m.Genre)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }
        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await movieDb.Movie.FindAsync(id);
            movieDb.Movie.Remove(movie);
            await movieDb.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
         public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Movie movie = movieDb.Movie.Find(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }
        //public ActionResult Edit(long? id)
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Movie movie = movieDb.Movie.Find(id);
            if (movie == null)
            {
                return NotFound();
            }
            MovieViewModel movieVM = new MovieViewModel();
            movieDb.Add(_mapper.Map<Movie>(movieVM));
            ViewBag.GenreID = new SelectList(movieDb.Genre, "GenreID", "Name");
            return View(movieVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MovieViewModel movieMV)
        {
            if (ModelState.IsValid)
            {
                if (movieMV.ImageUpload != null)
                {
                    movieMV.ImageFile = ConvertToByte(movieMV.ImageUpload);
                }
                movieDb.Update(_mapper.Map<Movie>(movieMV));
                movieDb.SaveChanges();
            }
            return View(movieMV);
        }
        private bool MovieExists(long id)
        {
            return movieDb.Movie.Any(e => e.ID == id);
        }
        [Authorize]
        public ActionResult Create()
        {
            var movie = new Movie();
            ViewBag.GenreID = new SelectList(movieDb.Genre, "GenreID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieViewModel movieMV)
        {
            if (ModelState.IsValid)
            {

                if (movieMV.ImageUpload != null)
                {
                    movieMV.ImageFile = ConvertToByte(movieMV.ImageUpload);
                }
                movieDb.Add(_mapper.Map<Movie>(movieMV));
                await movieDb.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(movieMV);
        }

        private byte[] ConvertToByte(IFormFile imagem)
        {
            if (imagem.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    imagem.CopyTo(ms);
                    return ms.ToArray();
                }

            }
            return null;
        }
    }
}