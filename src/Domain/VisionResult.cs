using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace azure.Domain
{
    public class VisionResult
    {
        [JsonProperty("categories")]
        public List<Category> Categories { get; set; }

        [JsonProperty("adult")]
        public object Adult { get; set; }

        [JsonProperty("tags")]
        public List<Tag> Tags { get; set; }

        [JsonProperty("description")]
        public Description Description { get; set; }

        [JsonProperty("requestId")]
        public Guid RequestId { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }

        [JsonProperty("faces")]
        public List<Face> Faces { get; set; }

        [JsonProperty("color")]
        public Color Color { get; set; }

        [JsonProperty("imageType")]
        public ImageType ImageType { get; set; }
    }

    public class Category
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("detail")]
        public Detail Detail { get; set; }
    }

    public class Detail
    {
        [JsonProperty("celebrities")]
        public List<Celebrity> Celebrities { get; set; }

        [JsonProperty("landmarks")]
        public object Landmarks { get; set; }
    }

    public class Celebrity
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("faceRectangle")]
        public FaceRectangle FaceRectangle { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }
    }

    public class FaceRectangle
    {
        [JsonProperty("left")]
        public long Left { get; set; }

        [JsonProperty("top")]
        public long Top { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }
    }

    public class Color
    {
        [JsonProperty("dominantColorForeground")]
        public string DominantColorForeground { get; set; }

        [JsonProperty("dominantColorBackground")]
        public string DominantColorBackground { get; set; }

        [JsonProperty("dominantColors")]
        public List<string> DominantColors { get; set; }

        [JsonProperty("accentColor")]
        public string AccentColor { get; set; }

        [JsonProperty("isBWImg")]
        public bool IsBwImg { get; set; }
    }

    public class Description
    {
        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("captions")]
        public List<Caption> Captions { get; set; }
    }

    public class Caption
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }
    }

    public class Face
    {
        [JsonProperty("age")]
        public long Age { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("faceRectangle")]
        public FaceRectangle FaceRectangle { get; set; }
    }

    public class ImageType
    {
        [JsonProperty("clipArtType")]
        public long ClipArtType { get; set; }

        [JsonProperty("lineDrawingType")]
        public long LineDrawingType { get; set; }
    }

    public class Metadata
    {
        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }
    }

    public class Tag
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }
    }
}
