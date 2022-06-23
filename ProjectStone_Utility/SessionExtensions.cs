using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ProjectStone_Utility;

// This class MUST be static.
public static class SessionExtensions
{
    // Serialize value.
    public static void Set<T>(this ISession session, string key, T value)
    {
        // Serialize the object and store it as a string using System.Text.Json serializer.
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    // Deserialize value.
    public static T Get<T>(this ISession session, string key)
    {
        // Deserialize the object and store it as a string using System.Text.Json deserializer.
        var value = session.GetString(key);

        return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }
}