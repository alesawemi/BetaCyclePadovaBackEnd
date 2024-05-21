namespace BetaCycle_Padova.Models.LTWorks
{
    public class CustomerAddressFrontEnd
    {
        /// <summary>
        /// Primary key. Foreign key to Customer.CustomerID.
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Primary key. Foreign key to Address.AddressID.
        /// </summary>
        public int AddressId { get; set; }

        /// <summary>
        /// The kind of Address. One of: Archive, Billing, Home, Main Office, Primary, Shipping
        /// </summary>
        public string AddressType { get; set; } = null!;
    }
}
