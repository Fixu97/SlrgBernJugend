using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BusinessLayer.DbHandler;
using ExcelLibrary.SpreadSheet;
using Shared.Models;
using Shared.Models.db;

namespace BusinessLayer.Excel
{
    public class ExcelReader
    {
        #region Private fields

        private readonly string FileName;    // The path to the file
        private readonly static string FolderLocation = Path.GetTempPath();

        private int _disciplineId;   // The PK of the discipline (which has to be in the Cell (r1,c1)

        private const int FirstDataRow = 2; // third row => Discipline name, headers, data
        private const int FirstDataCol = 3; // third column => PK, Prename, Lastname, data

        private readonly ResultModel _result = new ResultModel();

        #endregion

        #region Constructors

        public ExcelReader(string fileName)
        {
            FileName = fileName;
            Init();
        }

        public ExcelReader(HttpPostedFileBase file)
        {
            FileName = FolderLocation + "\\" + Guid.NewGuid().ToString();
            file.SaveAs(FileName);
            Init();
        }

        #endregion

        #region Private methods

        private void Init()
        {
            ReadExcel();
            if (_result.SuccessfullyRead)
            {
                Insert();
            }
        }

        private void ReadExcel()
        {
            try
            {
                // reset variables
                _result.SuccessfullyRead = false;
                _result.Warnings = new List<string>();
                _result.Errors = new List<Exception>();
                _result.TimesInserted = new List<TimeDTO>();

                // open xls file 
                var book = Workbook.Load(FileName);
                var sheet = book.Worksheets[0];

                #region Discipline

                if (!int.TryParse(sheet.Cells[0, 0].StringValue, out _disciplineId))
                {
                    throw new InvalidDataException($"Could not read discipline id in (r1/c1)");
                }

                #endregion

                // Worksheet dimensions
                var rowIndex = sheet.Cells.LastRowIndex;
                var colIndex = sheet.Cells.LastColIndex;

                for (var col = FirstDataCol; col <= colIndex; col++)
                {
                    #region Date

                    DateTime date = sheet.Cells[1, col].DateTimeValue;

                    // check if it is an empty Date
                    if (date.Ticks == 0)
                    {
                        break;
                    }

                    #endregion

                    for (var row = FirstDataRow; row <= rowIndex; row++)
                    {
                        #region Person

                        int personId;

                        if (!int.TryParse(sheet.Cells[row, 0].StringValue, out personId))
                        {
                            _result.Warnings.Add($"Could not read person id in (r{row + 1}/c1)!");
                            continue;
                        }

                        #endregion

                        #region Seconds

                        decimal seconds;
                        // do not insert empty values
                        if (!decimal.TryParse(sheet.Cells[row, col].StringValue, out seconds))
                        {
                            _result.Warnings.Add($"Could not read time in (r{row + 1}/c{col + 1})!");
                            continue;
                        }

                        #endregion

                        var newTime = new TimeDTO()
                        {
                            FK_P = personId,
                            FK_D = _disciplineId,
                            Seconds = seconds,
                            Date = date
                        };

                        _result.TimesInserted.Add(newTime);

                    }
                }

                _result.SuccessfullyRead = true;

            }
            catch (Exception e)
            {
                _result.SuccessfullyRead = false;
                _result.Errors.Add(e);
            }
        }

        private void Insert()
        {
            try
            {
                var bu = new DbTimeHandler<TimeDTO>();
                foreach (var time in _result.TimesInserted)
                {
                    bu.Insert(time);
                }
                _result.SuccessfullyInserted = true;
            }
            catch (Exception e)
            {
                _result.SuccessfullyInserted = false;
                _result.Errors.Add(e);
            }
        }

        #endregion

        public ResultModel GetResult()
        {
            return _result;
        }
    }
}
