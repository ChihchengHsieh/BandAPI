using BandAPI.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BandAPI.Models
{
    [TitleAndDescription(ErrorMessage = "Title must be different from description")]

    public abstract class AlbumManipulationDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(400)]
        public virtual string Description { get; set; }
    }
}
