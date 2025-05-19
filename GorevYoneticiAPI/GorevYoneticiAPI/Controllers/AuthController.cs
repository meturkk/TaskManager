using GorevYoneticiAPI.Data;
using GorevYoneticiAPI.DTOs;
using GorevYoneticiAPI.Models;
using GorevYoneticiAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace GorevYoneticiAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthController(AppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // Kullanıcı kayıt işlemi
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto request)
        {
            // Aynı kullanıcı adından daha önce kayıt yapılmış mı kontrol et
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return BadRequest("Kullanıcı adı zaten mevcut.");

            // Şifreyi hash'le ve salt oluştur
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            // Yeni kullanıcı oluştur
            var user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            // Veritabanına ekle
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Kayıt başarılı");
        }

        // Kullanıcı giriş işlemi
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
            // Kullanıcı adı ile kullanıcıyı bul
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
                return BadRequest("Kullanıcı bulunamadı.");

            // Şifreyi doğrula
            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                return BadRequest("Şifre yanlış.");

            // JWT token oluştur ve döndür
            var token = _tokenService.CreateToken(user);
            return Ok(new { token });
        }

        // Şifreyi hash'leyen yardımcı metot
        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key; // Rastgele bir salt oluştur
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)); // Şifreyi hash'le
        }

        // Şifre doğrulayan yardımcı metot
        private bool VerifyPasswordHash(string password, byte[] hash, byte[] salt)
        {
            using var hmac = new HMACSHA512(salt); // Salt ile aynı algoritmayı kullan
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)); // Şifreyi tekrar hash'le
            return computedHash.SequenceEqual(hash); // Hash'ler aynı mı kontrol et
        }
    }
}
