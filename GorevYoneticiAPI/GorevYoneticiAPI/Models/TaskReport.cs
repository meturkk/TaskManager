using GorevYoneticiAPI.Models;

public class TaskReport
{
    public int Id { get; set; }

    public string Type { get; set; }  // Günlük / Haftalık / Aylık

    public DateTime ReportDate { get; set; }

    public string Content { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }
}
