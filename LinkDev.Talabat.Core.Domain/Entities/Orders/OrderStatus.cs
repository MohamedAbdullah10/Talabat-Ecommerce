using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Core.Domain.Entities.Orders
{
    public enum OrderStatus
    {
        [EnumMember(Value = "Pending")]
        Pending = 0,
        [EnumMember( Value = "PaymentReceived")]
        PaymentReceived = 1,
        [EnumMember(Value = "Payment Failed")]
        PaymentFailed = 2,
    }
}
