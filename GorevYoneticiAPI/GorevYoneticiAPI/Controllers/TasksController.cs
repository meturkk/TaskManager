using GorevYoneticiAPI.Data;
using GorevYoneticiAPI.DTOs;
using GorevYoneticiAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GorevYoneticiAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Sadece yetkili (token'lı) kullanıcılar erişebilir
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        // Yeni görev oluşturur
        [HttpPost]
        public async Task<IActionResult> CreateTask(TaskCreateDto request)
        {
            // JWT token içinden kullanıcı ID'si alınır
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Yeni görev nesnesi oluşturulur
            var task = new TaskItem
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                IsCompleted = request.IsCompleted, // tamamlandı durumu destekleniyor
                UserId = userId
            };

            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();

            return Ok(task);
        }

        // Görevleri listele (isteğe bağlı olarak tamamlanmış filtrelemesi yapılabilir)
        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] bool? isCompleted = null)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var query = _context.TaskItems.Where(t => t.UserId == userId);

            // Eğer tamamlanma durumu belirtilmişse ona göre filtreleme yapılır
            if (isCompleted.HasValue)
            {
                query = query.Where(t => t.IsCompleted == isCompleted.Value);
            }

            var tasks = await query.ToListAsync();
            return Ok(tasks);
        }

        // Bir görevi tamamlandı olarak işaretler
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> MarkAsCompleted(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var task = await _context.TaskItems
                                     .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null)
                return NotFound();

            task.IsCompleted = true;
            await _context.SaveChangesAsync();

            return Ok(task);
        }

        // Belirli bir görevi ID'ye göre getirir
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var task = await _context.TaskItems
                                     .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null)
                return NotFound("Görev bulunamadı.");

            return Ok(task);
        }

        // Var olan görevi günceller
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskUpdateDto request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var task = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null)
                return NotFound("Görev bulunamadı.");

            // Görev güncelleniyor
            task.Title = request.Title;
            task.Description = request.Description;
            task.DueDate = request.DueDate;
            task.IsCompleted = request.IsCompleted;

            await _context.SaveChangesAsync();

            return Ok(task);
        }

        // Görev silme işlemi
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var task = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null)
                return NotFound("Görev bulunamadı.");

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();

            return Ok("Görev silindi.");
        }

        // Görevleri günlük, haftalık, aylık olarak gruplar
        [HttpGet("grouped")]
        public async Task<IActionResult> GetGroupedTasks()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tasks = await _context.TaskItems
                                      .Where(t => t.UserId == userId)
                                      .ToListAsync();

            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek); // haftanın başı (Pazar)
            var startOfMonth = new DateTime(today.Year, today.Month, 1); // ayın ilk günü

            var grouped = new
            {
                Daily = tasks.Where(t => t.DueDate.Date == today).ToList(),
                Weekly = tasks.Where(t => t.DueDate.Date >= startOfWeek && t.DueDate.Date < startOfWeek.AddDays(7)).ToList(),
                Monthly = tasks.Where(t => t.DueDate.Date >= startOfMonth && t.DueDate.Date < startOfMonth.AddMonths(1)).ToList()
            };

            return Ok(grouped);
        }
    }
}
