using System.Collections.Generic;

namespace SoftJail.DataProcessor.ImportDto
{
    public class DepartmentDto
    {
        public string Name { get; set; }
        public CellDto[] Cells { get; set; }
    }

    public class CellDto
    {
        public int CellNumber { get; set; }

        public bool HasWindow { get; set; }
    }
}
