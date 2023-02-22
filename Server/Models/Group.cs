public class Group
{
    public Group(string name, int category)
    {
        Name = name;
        Category = category;
        CreatedDate = DateTime.Now;
        IsDeleted = 0;
    }

    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public int Category { get; set; }
    public string? Description { get; set; }
    public string? OldName { get; set; }
    public int OldCategory { get; set; }
    public string? OldDescription { get; set; }
    [Required]
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime DeletedDate { get; set; }
    [Required]
    public int IsDeleted { get; set; }
    public int DeleteFromAccountId { get; set; }
}