using System.Collections.Generic;

namespace SoftJail.DataProcessor.ImportDto
{
    public class DepartmentImportDTO
    {
        public string Name { get; set; }

        public ICollection<CellImportDTO> Cells { get; set; } = new List<CellImportDTO>();
    }
}
