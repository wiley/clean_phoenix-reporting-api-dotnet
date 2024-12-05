using System.ComponentModel.DataAnnotations;

namespace Reporting.API.InputSerializer 
{
  public class InputPowerBi 
  {
    [Required]
    public string groupId {get; set;}
    [Required]
    public string reportId {get; set;}
    [Required]
    public string reportSectionId {get; set;}
  }
}