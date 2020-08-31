using CarDealer.Models;
using System.Collections;
using System.Collections.Generic;

namespace CarDealer.DTO
{
    public class CarPartExportTDO
    {
        public CarExportTDO car { get; set; }

        public ICollection<PartExportTDO> parts { get; set; } = new HashSet<PartExportTDO>();
    }
}
