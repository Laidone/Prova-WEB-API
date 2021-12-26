using Dominio.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace ProjetoCore.API.ViewModels
{
    public class MovieViewModel
    {
        public int ID { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime ReleaseDate { get; set; }


        [StringLength(60, MinimumLength = 3)]
        public string Director { get; set; }

        // [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public decimal Gross { get; set; }

        [Range(0, 10)]
        public double Rating { get; set; }

        public int GenreID { get; set; }

        public virtual Genre Genre { get; set; }
        public IFormFile ImageUpload { get; set; }
        public byte[] ImageFile { get; set; }

    }
}