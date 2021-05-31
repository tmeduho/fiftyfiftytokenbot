using System;
using System.Text.Json.Serialization;

namespace FiftyFiftyBot.DataTransferObjects
{
  public class PancakeApiResult
  {
    [JsonPropertyName("updated_at")]
    public long UpdatedAt { get; set; }

    [JsonPropertyName("data")]
    public PancakeData Data { get; set; }
  }

  public class PancakeData
  {
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("symbol")]
    public string Symbol { get; set; }

    [JsonPropertyName("price")]
    public string Price { get; set; }

    [JsonPropertyName("price_BNB")]
    public string PriceInBnb { get; set; }
  }
}
