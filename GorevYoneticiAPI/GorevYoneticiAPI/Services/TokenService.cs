using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GorevYoneticiAPI.Models;
using GorevYoneticiAPI.Services;

/// TokenService, kullanıcı için JWT (JSON Web Token) oluşturur.
/// Bu sınıf, kimlik doğrulama sırasında access token üretmekten sorumludur.

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    
    // Uygulama ayarlarını okumak için yapılandırma nesnesi alınır. 
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

 
    public string CreateToken(User user)
    {
        // JWT içine eklenecek kullanıcıya ait bilgiler (claim'ler)
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username), // Kullanıcı adı
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) // Kullanıcı ID
        };

        // Uygulama ayarlarında tanımlı olan "Jwt:Key" değeriyle güvenlik anahtarı oluşturulur
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        // Anahtar ve algoritma kullanılarak imzalama bilgisi tanımlanır
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // JWT token nesnesi oluşturulur
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],             // Token'ı oluşturan taraf
            audience: _configuration["Jwt:Audience"],         // Token'ın hedef kitlesi
            claims: claims,                                   // Token içeriği
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])), // Token geçerlilik süresi
            signingCredentials: creds                         // İmzalama bilgisi
        );

        // Token string olarak dönülür
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
