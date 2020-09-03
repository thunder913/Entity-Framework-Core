using System.Collections.Generic;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.Dto.Export
{
    [XmlType("User")]
    public class ExportUserPurchaseDTO
    {
        [XmlAttribute("username")]
        public string Username{get;set;}

        [XmlArray("Purchases")]
        public List<PurchaseExportDTO> Purchases { get; set; }
    }
}
