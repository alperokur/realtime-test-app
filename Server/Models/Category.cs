public class Category
{
    public Category(string name)
    {
        Name = name;
        CreatedDate = DateTime.Now;
        IsDeleted = 0;
    }

    [Key]
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }
    [Required]
    public string? OldName { get; set; }
    [Required]
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime DeletedDate { get; set; }
    public int IsDeleted { get; set; }
    public int DeleteFromAccountId { get; set; }
}