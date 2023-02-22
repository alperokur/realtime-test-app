public class Page
{
    public Page(string name, int category)
    {
        Name = name;
        Category = category;
        IsDeleted = 0;
        CreatedDate = DateTime.Now;
    }

    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public int Category { get; set; }
    public string? Description { get; set; }
    [Required]
    public int IsDeleted { get; set; }
    public int DeleteFromAccountId { get; set; }
    [Required]
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime DeletedDate { get; set; }
}