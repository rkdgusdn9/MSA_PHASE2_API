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
        public string SmallPart { get; set; }
        public string Direction { get; set; }
        public IFormFile Image { get; set; }
    }
}
