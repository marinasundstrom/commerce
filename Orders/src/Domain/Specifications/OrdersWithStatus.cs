﻿using System;
using YourBrand.Orders.Domain.Entities;
using YourBrand.Orders.Domain.Enums;

namespace YourBrand.Orders.Domain.Specifications;

public class OrdersWithStatus : BaseSpecification<Order>
{
    public OrdersWithStatus(OrderStatus status)
    {
        Criteria = order => order.Status == status;
    }
}

