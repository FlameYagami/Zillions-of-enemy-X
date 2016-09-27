using System;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;

namespace CardEditor.Utils
{
    internal class ExcelHelper
    {
        public static bool ExportPackToExcel(string filePath, DataSet dataSet)
        {
            foreach (DataTable dt in dataSet.Tables)
                try
                {
                    var icolIndex = 0;
                    var iRowIndex = 1;
                    var iCellIndex = 0;
                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet(dt.TableName);
                    var headerRow = sheet.CreateRow(0);
                    foreach (DataColumn item in dt.Columns)
                    {
                        var cell = headerRow.CreateCell(icolIndex);
                        cell.SetCellValue(item.ColumnName);
                        icolIndex++;
                    }
                    foreach (DataRow rowitem in dt.Rows)
                    {
                        var dataRow = sheet.CreateRow(iRowIndex);
                        foreach (DataColumn colitem in dt.Columns)
                        {
                            var cell = dataRow.CreateCell(iCellIndex);
                            cell.SetCellValue(rowitem[colitem].ToString());
                            iCellIndex++;
                        }
                        iCellIndex = 0;
                        iRowIndex++;
                    }
                    var file = new FileStream(filePath, FileMode.OpenOrCreate);
                    workbook.Write(file);
                    file.Flush();
                    file.Close();
                    file.Dispose();
                }
                catch (Exception)
                {
                    return false;
                }
            return true;
        }
    }
}