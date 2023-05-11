namespace HaWeb.Models;
using System.ComponentModel.DataAnnotations;
public class YearSetting {
    [Required]
    public int EndYear { get; set; }
}