using Newtonsoft.Json;

namespace TelegramAsDatabase.Models;

[Serializable]
internal class TDBIndex
{
    public Dictionary<Guid, string> Data { get; set; } = [];

    public static implicit operator string(TDBIndex data) => JsonConvert.SerializeObject(data)!;
    public static implicit operator TDBIndex(string data) => JsonConvert.DeserializeObject<TDBIndex>(data)!;
}