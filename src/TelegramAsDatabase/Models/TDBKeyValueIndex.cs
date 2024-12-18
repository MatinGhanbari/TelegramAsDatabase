using Newtonsoft.Json;

namespace TelegramAsDatabase.Models;

[Serializable]
internal class TDBKeyValueIndex
{
    public Dictionary<string, int> IndexIds { get; set; } = [];

    public static implicit operator string(TDBKeyValueIndex data) => JsonConvert.SerializeObject(data)!;
    public static implicit operator TDBKeyValueIndex(string data) => JsonConvert.DeserializeObject<TDBKeyValueIndex>(data)!;
}