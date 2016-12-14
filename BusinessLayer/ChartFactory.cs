using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shared.Models.Chart;
using Shared.Models.db;

namespace BusinessLayer
{
    //todo: create labels for every date existing.
    //todo: every series has to have the same amount of entries as there are labels
    //todo: fill time of previous entry if not existing on that day. if no previous element exists, take the first occourence
    //todo: take fastest time of person of discipline per day
    public class ChartFactory
    {
        private Random random = new Random();
        public ChartDataModel CreateChartDataModelDiscipline(List<TimeDTO> times)
        {
            if (times == null || !times.Any())
            {
                throw new InvalidDataException("No times given to draw chart!");
            }
            if (times.Any(t => t.FK_D != times[0].FK_D))
            {
                throw new InvalidDataException("Multiple disciplines specified!");
            }

            var dates = CreateDates(times);
            var people = GetAllPeople(times);
            var labels = CreateLabels(dates);
            var options = CreateChartOptions();

            var chart = new ChartDataModel
            {
                Datasets = new List<DatasetModel>(),
                Title = times.First().Discipline.DisplayName,
                Dates = labels,
                Options = options
            };

            foreach (var person in people)
            {
                var serializedTimes = new List<TimeDTO>();
                var timesOfPerson = times.Where(t => t.FK_P == person.Pk).ToList();

                foreach (var date in dates)
                {
                    // if the person has a time on that date
                    if (timesOfPerson.Any(t => t.Date == date))
                    {
                        //serializedTimes.Add(timesOfPerson.First(t => t.Date == date));

                        serializedTimes.Add(                                                    // Add the
                            timesOfPerson.First(                                                // first occourence
                                t1 => t1.Seconds == timesOfPerson.Where(t2 => t2.Date == date)  // on that day
                                .Min(t3 => t3.Seconds)));                                       // of the fastest time                                       
                    }
                    else
                    {
                        // if the person has no time on that date try to use the last existing time
                        if (serializedTimes.Any())
                        {
                            var lastExistingTime = serializedTimes.Last();
                            serializedTimes.Add(new TimeDTO
                            {
                                Date = date,
                                FK_D = lastExistingTime.Discipline.Pk,
                                Discipline = lastExistingTime.Discipline,
                                Person = lastExistingTime.Person,
                                FK_P = lastExistingTime.Person.Pk,
                                Seconds = lastExistingTime.Seconds
                            });
                        }
                        else
                        {
                            // the person has no time on that date and no time has yet been aded.
                            // therefore we take the first time he has.
                            serializedTimes.Add(timesOfPerson.First());
                        }
                    }
                }

                chart.Datasets.Add(CreateDatasetModel(serializedTimes, dates));
            }

            return chart;
        }

        public List<ChartDataModel> CreateChartDataModels_Person(List<TimeDTO> times)
        {
            if (times.Any(t => t.FK_P != times[0].FK_P))
            {
                throw new InvalidDataException("Multiple people specified!");
            }
            if (times.Any(t => t.FK_D != times[0].FK_D))
            {
                throw new InvalidDataException("Multiple disciplines specified!");
            }
            if (times == null || !times.Any())
            {
                throw new InvalidDataException("No times given to draw chart!");
            }

            var disciplines = new Dictionary<int, List<TimeDTO>>();

            foreach (var time in times)
            {
                if (!disciplines.ContainsKey(time.Discipline.Pk))
                {
                    disciplines.Add(time.Discipline.Pk, new List<TimeDTO>());
                }
                disciplines[time.Discipline.Pk].Add(time);
            }

            var charts = new List<ChartDataModel>();
            foreach (var discipline in disciplines)
            {
                charts.Add(CreateChartDataModelPerson(discipline.Value));
            }

            return charts;
        }

        private ChartDataModel CreateChartDataModelPerson(List<TimeDTO> times)
        {
            if (times.Any(t => t.FK_D != times[0].FK_D))
            {
                throw new InvalidDataException("Only times of one single discipline allowed!");
            }
            var dates = CreateDates(times);
            var series = new List<DatasetModel> {CreateDatasetModel(times, dates)};
            var labels = CreateLabels(dates);
            var options = CreateChartOptions();
            var chartDataModel = new ChartDataModel
            {
                Datasets = series,
                Dates = labels,
                Options = options,
                Title = times[0].Discipline.DisplayName
            };

            return chartDataModel;
        }

        public ChartOption CreateChartOptions()
        {
            var option = new ChartOption
            {
                ShowScale = true,
                ScaleLabel = "<%=value%>",
                ScaleShowGridLines = true,
                ScaleGridLineColor = "rgba(0,0,0,0,0.05)",
                ScaleGridLineWidth = 1,
                ScaleShowHorizontalLines = true,
                ScaleShowVerticalLines = true,
                BezierCurve = true,
                BezierCurveTension = 0.4F,
                PointDot = true,
                PointDotRadius = 4,
                PointDotStrokeWidth = 1,
                PointHitDetectionRadius = 20,
                DatasetStroke = true,
                DatasetStrokeWidth = 2,
                DatasetFill = false,
                LegendTemplate = "<ul class=\"chartLegend\"><% for (var i=0; i<datasets.length; i++){%><li><span><div class=\"legendColor\" style=\"background-color:<%=datasets[i].fillColor%>\"></div><%if(datasets[i].label){%><%=datasets[i].label%><%}%></span></li><%}%></ul>",
                TooltipTemplate = "<%if (label){%><%=label%>: <%}%><%= value %>",
                MultiTooltipTemplate = "<%=datasetLabel%> : <%= value %>"
            };
            return option;
        }

        private DatasetModel CreateDatasetModel(List<TimeDTO> times, List<DateTime> dates)
        {
            #region Datacheck
            if (!times.Any())
            {
                throw new InvalidDataException("No data for series specified!");
            }
            if (times.Any(t => t.FK_D != times[0].FK_D))
            {
                throw new InvalidDataException("Multiple disciplines specified!");
            }
            #endregion

            var serieColor = GenerateRandomColor();

            var dataSet = new DatasetModel
            {
                Label = times.First().Person.DisplayName,
                FillColor = serieColor,
                StrokeColor = serieColor,
                PointColor = serieColor,
                PointStrokeColor = serieColor,
                PointHighlightFill = serieColor,
                PointHighlightStroke = "rgba(151,187,205,1)",
                Data = new List<float>()
            };
            foreach (var time in times)
            {
                dataSet.Data.Add((float)time.Seconds);
            }
            return dataSet;
        }

        private List<DateTime> CreateDates(List<TimeDTO> times)
        {
            var dates = new List<DateTime>();

            foreach (var time in times)
            {
                if (dates.Contains(time.Date))
                {
                    continue;
                }
                dates.Add(time.Date);
            }
            return dates;
        }

        private List<string> CreateLabels(List<DateTime> dates)
        {
            var labels = new List<string>();
            foreach (var date in dates)
            {
                labels.Add(date.ToString("dd.MM.yy"));
            }
            return labels;
        }

        private List<PersonDTO> GetAllPeople(List<TimeDTO> times)
        {
            var persons = new List<PersonDTO>();
            foreach (var time in times)
            {
                if (persons.Any(p => p.Pk == time.Person.Pk))
                {
                    continue;
                }
                persons.Add(time.Person);
            }
            return persons;
        }

        private string GenerateRandomColor()
        {
            var red = random.Next(0, 255);
            var green = random.Next(0, 255);
            var blue = random.Next(0, 255);

            return "rgb(" + red + "," + green + "," + blue + ")";
        }
    }
}
