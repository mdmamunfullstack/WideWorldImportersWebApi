namespace Entities.RequestFeatures
{
    public class SupplierParameters : RequestParameters
    {
        public SupplierParameters() { OrderBy = "SupplierID"; }

        public uint MinPaymentMethod { get; set; } = 1;
        public uint MaxPaymentMethod { get; set; } = 4;

        public bool ValidPaymentMethodRange => MinPaymentMethod <= MaxPaymentMethod;

        public string SupplierInvoiceNumber { get; set; }
        public int SupplierId { get; set; }
    }
}