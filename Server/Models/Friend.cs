public class Friend
{
    public Friend(int senderId, int receiverId)
    {
        SenderId = senderId;
        ReceiverId = receiverId;
        IsSeen = 0;
        IsAccepted = 0;
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
    public int IsSeen { get; set; }
    [Required]
    public int IsAccepted { get; set; }
    [Required]
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime DeletedDate { get; set; }
    [Required]
    public int IsDeleted { get; set; }
    public int DeleteFromAccountId { get; set; }
}