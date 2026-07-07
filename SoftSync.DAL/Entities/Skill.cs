using System.ComponentModel.DataAnnotations;

namespace SoftSync.DAL.Entities;

public class Skill
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    [MaxLength(50)]
    public string IconName { get; set; } = string.Empty;

    public ICollection<CaseStudy> CaseStudies { get; set; } = new List<CaseStudy>();
}
