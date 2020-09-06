using System.Collections.Generic;
using System.Xml.Serialization;
namespace VaporStore.DataProcessor.Dto.Export
{
   [XmlType("User")]
    public class UserExportDTO
    {
        [XmlAttribute("username")]
        public string Username { get; set; }
        public List<PurchaseExportDTO> Purchases { get; set; } = new List<PurchaseExportDTO>();
        
        public decimal TotalSpent { get; set; }
    }
}
