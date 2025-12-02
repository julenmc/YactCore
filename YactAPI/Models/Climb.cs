using System.ComponentModel.DataAnnotations;

namespace YactAPI.Models
{
    public class Climb
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Path { get; set; }
        [Required]
        public double LongitudeInit { get; set; }
        [Required]
        public double LongitudeEnd { get; set; }
        [Required]
        public double LatitudeInit { get; set; }
        [Required]
        public double LatitudeEnd { get; set; }
        [Required]
        public double AltitudeInit { get; set; }
        [Required]
        public double AltitudeEnd { get; set; }
        [Required]
        public double InitRouteDistance { get; set; } // Distance from the start of the route to the climb start point
        [Required]
        public double EndRouteDistance { get; set; } // Distance from the start of the route to the climb end point
        [Required]
        public double Distance { get; set; }
        [Required]
        public double AverageSlope { get; set; }
        [Required]
        public double MaxSlope { get; set; }
        [Required]
        public double HeightDiff { get; set; }
    }
}
