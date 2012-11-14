
using Newtonsoft.Json;
namespace Waveface.Stream.ClientFramework
{
    public class ImageMetaData
    {
        public int width { get; set; }

        public int height { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ThumbnailData small { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ThumbnailData medium { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ThumbnailData large { get; set; }
    }
}
