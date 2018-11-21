using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymRat.Models
{
    public class GymImageItem
    {
        public string Name { get; set; }
        public string BigPart { get; set; }
        public IFormFile Image { get; set; }
    }
}
