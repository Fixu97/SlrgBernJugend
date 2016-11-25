using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using BusinessLayer.DbHandler;
using Shared.Models;
using Shared.Models.db;
using ExcelLibrary.SpreadSheet;
using ExcelLibrary.CompoundDocumentFormat;
using QiHe.CodeLib;

namespace BusinessLayer.Excel
{
    public class ExcelWriter
    {
        #region Private readonly fields

        private readonly List<PersonDTO> _people;
        private readonly DisciplineDTO _discipline;
        private readonly List<DateTime> _dates;

        #endregion

        #region Private properties

        private static string _folderLocation => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static string _fileLocation => _folderLocation + "ExcelImport.xls";

        #endregion

        #region Constructors

        /// <summary>
        /// Use this constructor if you want to use all people
        /// </summary>
        /// <param name="discipline">The discipline the times should belong to</param>
        /// <param name="dates">The dates for the times</param>
        public ExcelWriter(DisciplineDTO discipline, List<DateTime> dates)
        {
            this._discipline = discipline;
            this._dates = dates;

            // get all people
            var bu = new DbTimeHandler<TimeDTO>();
            var dbObj = bu.GetAll();

            dbObj.ForEach(t => _people.Add(t.Person));


            CreateInputExcel();
        }

        /// <summary>
        /// Use this constructor if you do not want to use all people in the input file
        /// </summary>
        /// <param name="discipline">The discipline the times should belong to</param>
        /// <param name="people">A list of people to insert the times for</param>
        /// <param name="dates">The dates for the times</param>
        public ExcelWriter(DisciplineDTO discipline, List<PersonDTO> people, List<DateTime> dates)
        {
            this._discipline = discipline;
            this._people = people;
            this._dates = dates;

            CreateInputExcel();
        }

        #endregion

        #region Public methods

        public string GetExcelFilePath()
        {
            return _fileLocation;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Creates an Excel that can be used to insert times into the DB
        /// </summary>
        /// <returns>The relative path to the file</returns>
        private void CreateInputExcel()
        {
            //create new xls file 
            Workbook workbook = new Workbook();
            Worksheet worksheet = new Worksheet("First Sheet");

            #region Bug-fix
            // Fill in some data see http://stackoverflow.com/questions/8107610/cant-open-excel-file-generated-with-excellibrary for reason
            for (var i = 0; i < 100; i++)
                worksheet.Cells[i, 0] = new Cell("");
            #endregion


            worksheet.Cells[0, 0] = new Cell(_discipline.Pk);
            worksheet.Cells[0, 1] = new Cell(_discipline.DisplayName);
            worksheet.Cells[1, 0] = new Cell("#");
            worksheet.Cells[1, 1] = new Cell("Prename");
            worksheet.Cells[1, 2] = new Cell("Lastname");

            var j = 4;
            foreach (var date in _dates)
            {
                worksheet.Cells[1, j] = new Cell(date.ToString("dd.MM.yyyy"));
                j++;
            }

            j = 2;
            foreach (var person in _people)
            {
                worksheet.Cells[j, 0] = new Cell(person.Pk);
                worksheet.Cells[j, 1] = new Cell(person.Prename);
                worksheet.Cells[j, 2] = new Cell(person.LastName);
                j++;
            }

            // Add the Worksheet to the Workbook
            workbook.Worksheets.Add(worksheet);

            // Make sure the directory exists
            Directory.CreateDirectory(_folderLocation);

            // Save the whole thing
            workbook.Save(_fileLocation);
        }

        #endregion
    }
}
