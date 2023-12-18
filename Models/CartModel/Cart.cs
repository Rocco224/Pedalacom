using Pedalacom.Models.CustomerModel;
using Pedalacom.Models.ProductModel;
using System;

namespace Pedalacom.Models.CartModel;

public partial class Cart
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int CustomerId { get; set; }
    public int Quantity { get; set; }

    public virtual Customer ?Customer { get; set; }

    public virtual Product ?Product { get; set; }
}
