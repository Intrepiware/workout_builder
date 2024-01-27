using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using WorkoutBuilder.Models.ListItems;
using WorkoutBuilder.Services.Models;

namespace WorkoutBuilder.Models
{
    public class ExercisesDetailsModel
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Notes { get; set; }
        [Required]
        public string Equipment { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public long FocusId { get; set; }
        public string? YoutubeUrl { get; set; }
        public long? FocusPartId { get; set; }
        public List<long>? ActivationParts { get; set; }
        public List<SelectListItem>? EquipmentOptions { get; set; } = null!;
        public List<SelectListItem>? FocusOptions { get; set; } = null!;
        public List<PartListItem>? Parts { get; set; } = null!;
    }
}
