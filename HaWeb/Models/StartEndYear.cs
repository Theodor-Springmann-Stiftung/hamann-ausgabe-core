namespace HaWeb.Models;
using System.ComponentModel.DataAnnotations;
public class StartEndYear {
    [Required]
    public int StartYear { get; set; }
    
    [Required]
    public int EndYear { get; set; }
}