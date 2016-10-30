using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Shared.Models.Chart
{
    [DataContract(Name="data")]
    public class ChartDataModel
    {
        [DataMember(Name="labels")]
        public List<string> Dates { get; set; } 

        [DataMember(Name="datasets")]
        public List<DatasetModel> Datasets { get; set; }

        [DataMember(Name="options")]
        public ChartOption Options { get; set; }

        [DataMember(Name="title")]
        public string Title { get; set; }

    }
}