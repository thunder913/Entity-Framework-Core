using VaporStore.Data.Models.Enums;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class CardDTO
    {
        public string Number { get; set; }
        public string Cvc { get; set; }
        public CardType Type { get; set; }
    }
}
