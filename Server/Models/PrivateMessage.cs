public class PrivateMessage
{
    public PrivateMessage(int senderId, int receiverId, string text)
    {
        SenderId = senderId;
        ReceiverId = receiverId;
        Text = text;
        IsSeen = 0;
        CreatedDate = DateTime.Now;
        IsDeleted = 0;
    }

    [Key]
    public int Id { get; set; }
    [Required]
    public int SenderId { get; set; }
    [Required]
    public int ReceiverId { get; set; }
    [Required]
    public string Text { get; set; }
    public string? OldText { get; set; }
    [Required]
    public int IsSeen { get; set; }
    [Required]
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime DeletedDate { get; set; }
    public int IsDeleted { get; set; }
    public int DeleteFromAccountId { get; set; }

}