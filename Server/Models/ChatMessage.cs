public class ChatMessage
{
    public ChatMessage(int senderId, string text)
    {
        SenderId = senderId;
        Text = text;
        CreatedDate = DateTime.Now;
        IsDeleted = 0;
    }

    [Key]
    public int Id { get; set; }
    [Required]
    public int SenderId { get; set; }
    [Required]
    public string? Text { get; set; }
    public string? OldText { get; set; }
    [Required]
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime DeletedDate { get; set; }
    [Required]
    public int IsDeleted { get; set; }
    public int DeleteFromAccountId { get; set; }
}