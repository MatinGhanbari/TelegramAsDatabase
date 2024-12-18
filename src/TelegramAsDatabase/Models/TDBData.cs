using Newtonsoft.Json;

namespace TelegramAsDatabase.Models;

[Serializable]
public class TDBData<T>
{
    public string Key { get; set; }
    public T Value { get; set; }

    public static implicit operator string(TDBData<T> data) => JsonConvert.SerializeObject(data)!;
    public static implicit operator TDBData<T>(string data) => JsonConvert.DeserializeObject<TDBData<T>>(data)!;
}