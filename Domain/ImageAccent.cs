using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Domain
{
    public class ImageAccent
    {
        [Required]
        public string ImagePath { get; set; }
        [Required]
        public string LowAccent { get; set; }
        [Required]
        public string MiddleAccent { get; set; }
        [Required]
        public string HighAccent { get; set; }
    }
}