using System.Text.Json.Serialization;
using WebApplication1.Endpoint;
using WebApplication1.Service;

/// <summary>
/// No arquivo de contexto do serializador JSON para a aplicação.
/// Tdos os serviços tem que estar listados aqui para que o serializador funcione corretamente.
/// a serialização ocoera de forma otimizada em tempo de compilação e automaticamente feita pelo .NET 9.0+
/// </summary>

namespace WebApplication1.DTO
{
    [JsonSerializable(typeof(RegisterUserDTO))]
    [JsonSerializable(typeof(UserDTO))]
    [JsonSerializable(typeof(List<UserDTO>))]
    [JsonSerializable(typeof(ErrorReport))]
    [JsonSerializable(typeof(Endpoints))]
    [JsonSerializable(typeof(bool))]
    public partial class AppJsonSerializerContext : JsonSerializerContext
    {
    }
}
