
using Newtonsoft.Json;

namespace AChat.Application.Common.Extensions;

public static class CommonExtension
{
  public static T Copy<T>(this T self)
  {
    var serialized = JsonConvert.SerializeObject(self);
    return JsonConvert.DeserializeObject<T>(serialized);
  }
}