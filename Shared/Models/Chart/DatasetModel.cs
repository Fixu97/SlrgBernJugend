using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Shared.Models.Chart
{
    [DataContract(Name="dataset")]
    public class DatasetModel
    {
        [DataMember(Name="label")]
        public string Label { get; set; }

        [DataMember(Name = "fillColor")]
        public string FillColor { get; set; }

        [DataMember(Name = "strokeColor")]
        public string StrokeColor { get; set; }

        [DataMember(Name = "pointColor")]
        public string PointColor { get; set; }

        [DataMember(Name = "pointStrokeColor")]
        public string PointStrokeColor { get; set; }

        [DataMember(Name = "pointHighlightFill")]
        public string PointHighlightFill { get; set; }

        [DataMember(Name = "pointHighlightStroke")]
        public string PointHighlightStroke { get; set; }

        [DataMember(Name = "data")]
        public List<float> Data { get; set; }
    }
}