using MemoryPack;
using Newtonsoft.Json;

namespace TelegramAsDatabase.Models;

[Serializable]
[MemoryPackable]
public partial class TDBData<T>
{
    public Guid Id { get; set; }
    public T Data { get; set; }

    public static implicit operator string(TDBData<T> data) => JsonConvert.SerializeObject(data)!;
    public static implicit operator TDBData<T>(string data) => JsonConvert.DeserializeObject<TDBData<T>>(data)!;
}