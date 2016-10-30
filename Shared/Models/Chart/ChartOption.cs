using System.Runtime.Serialization;

namespace Shared.Models.Chart
{
    [DataContract(Name="options")]
    public class ChartOption
    {

        /// <summary>
        /// If we should show the scale at all
        /// </summary>
        [DataMember(Name = "showScale")]
        public bool ShowScale { get; set; }

        /// <summary>
        /// Interpolated JS string - can access value
        /// </summary>
        [DataMember(Name = "scaleLabel")]
        public string ScaleLabel { get; set; }

        /// <summary>
        /// Whether grid lines are shown across the chart
        /// </summary>
        [DataMember(Name="scaleShowGridLines")]
        public bool ScaleShowGridLines { get; set; }

        /// <summary>
        /// Colour of the grid lines
        /// </summary>
        [DataMember(Name = "scaleGridLineColor")]
        public string ScaleGridLineColor { get; set; }

        /// <summary>
        /// Width of the grid lines
        /// </summary>
        [DataMember(Name = "ScaleGridLineWidth")]
        public float ScaleGridLineWidth { get; set; }

        /// <summary>
        /// Whether to show horizontal lines (except X axis)
        /// </summary>
        [DataMember(Name = "scaleShowHorizontalLines")]
        public bool ScaleShowHorizontalLines { get; set; }

        /// <summary>
        /// Whether to show vertical lines (except Y axis)
        /// </summary>
        [DataMember(Name = "scaleShowVerticalLines")]
        public bool ScaleShowVerticalLines { get; set; }

        /// <summary>
        /// Whether the line is curved between points
        /// </summary>
        [DataMember(Name = "bezierCurve")]
        public bool BezierCurve { get; set; }

        /// <summary>
        /// Tension of the bezier curve between points
        /// </summary>
        [DataMember(Name = "bezierCurveTension")]
        public float BezierCurveTension { get; set; }

        /// <suWhether to show a dot for each pointmmary>
        /// 
        /// </summary>
        [DataMember(Name = "pointDot")]
        public bool PointDot { get; set; }

        /// <summary>
        /// Radius of each point dot in pixels
        /// </summary>
        [DataMember(Name = "pointDotRadius")]
        public int PointDotRadius { get; set; }

        /// <summary>
        /// Pixel width of point dot stroke
        /// </summary>
        [DataMember(Name = "pointDotStrokeWidth")]
        public int PointDotStrokeWidth { get; set; }

        /// <summary>
        /// amount extra to add to the radius to cater for hit detection outside the drawn point
        /// </summary>
        [DataMember(Name = "pointHitDetectionRadius")]
        public int PointHitDetectionRadius { get; set; }

        /// <summary>
        /// Whether to show a stroke for datasets
        /// </summary>
        [DataMember(Name = "datasetStroke")]
        public bool DatasetStroke { get; set; }

        /// <summary>
        /// Pixel width of dataset stroke
        /// </summary>
        [DataMember(Name = "datasetStrokeWidth")]
        public int DatasetStrokeWidth { get; set; }

        /// <summary>
        /// Whether to fill the dataset with a colour
        /// </summary>
        [DataMember(Name = "datasetFill")]
        public bool DatasetFill { get; set; }

        /// <summary>
        /// A legend template
        /// </summary>
        [DataMember(Name = "legendTemplate")]
        public string LegendTemplate { get; set; }

        /// <summary>
        /// Template string for single tooltips
        /// </summary>
        [DataMember(Name = "tooltipTemplate")]
        public string TooltipTemplate { get; set; }

        /// <summary>
        /// Template string for multiple tooltips
        /// </summary>
        [DataMember(Name = "multiTooltipTemplate")]
        public string MultiTooltipTemplate { get; set; }

    }
}