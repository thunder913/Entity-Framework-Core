using System.ComponentModel.DataAnnotations;

namespace FastFood.Core.ViewModels.Orders
{
    public class CreateOrderInputModel
    {
        public string Customer { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
