using Microsoft.Build.Execution;

namespace Eticaret.ViewModels
{
    public class ShoppingRemoveViewModel
    {
        public string Mesage { get; set; }
        public decimal CartTotal { get; set; }
        public int CartCount { get; set; }
        public int ItemCount { get; set; }
        public int DeleteId { get; set; }
    }
}
