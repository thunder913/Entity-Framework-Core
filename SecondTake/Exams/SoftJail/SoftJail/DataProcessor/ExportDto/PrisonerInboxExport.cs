using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ExportDto
{
    [XmlType("Prisoner")]
    public class PrisonerExport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IncarcerationDate { get; set; }
        
        [XmlArray("EncryptedMessages")]
        public Message[] Messages { get; set; }
    }

    [XmlType("Message")]
    public class Message
    {
        public string Description { get; set; }
    }
}
