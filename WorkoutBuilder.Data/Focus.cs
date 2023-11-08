using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutBuilder.Data
{
    [Table("Focus")]
    public class Focus
    {
        [Key]
        public long Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }
    }
}
