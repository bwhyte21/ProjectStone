using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ProjectStone_Utility
{
    // This class MUST be static.
  public static class SessionExtensions
  {
      // Serialize value.
      public static void Set<T>(this ISession session, string key, T value)
      {
          // Serialize the object and store it as a string using System.Text.Json's serializer.
          session.SetString(key, JsonSerializer.Serialize(value));
      }
      
      // Deserialize value.
      public static T Get<T>(this ISession session, string key)
      {
          // Deserialize the object and store it as a string using System.Text.Json's deserializer.
          var value = session.GetString(key);

          return value == null ? default : JsonSerializer.Deserialize<T>(value);
      }
  }
}
