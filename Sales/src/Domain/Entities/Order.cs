﻿using System.Collections.Generic;
using YourBrand.Sales.Domain.Enums;
using YourBrand.Sales.Domain.Events;

namespace YourBrand.Sales.Domain.Entities;

public class Order : AuditableEntity, IAggregateRoot
{
    HashSet<OrderItem> _items = new HashSet<OrderItem>();

    protected Order()
    {
    }

    public Order(OrderStatus status = OrderStatus.Draft)
    {
        Status = status;
    }

    public string Id { get; private set; } = Guid.NewGuid().ToString();


    public OrderStatus Status { get; private set; }

    public bool UpdateStatus(OrderStatus status)
    {
        var oldStatus = Status;
        if (status != oldStatus)
        {
            Status = status;

            AddDomainEvent(new OrderUpdated(Id));
            AddDomainEvent(new OrderStatusUpdated(Id, status, oldStatus));

            return true;
        }

        return false;
    }

    public User? AssigneeId { get; private set; }

    public string? AssigneeIdId { get; private set; }

    public bool UpdateAssigneeId(string? userId)
    {
        var oldAssigneeIdId = AssigneeIdId;
        if (userId != oldAssigneeIdId)
        {
            AssigneeIdId = userId;
            AddDomainEvent(new OrderAssignedUserUpdated(Id, userId, oldAssigneeIdId));

            return true;
        }

        return false;
    }

    public string? CustomerId { get; set; } 

    public double VatRate { get; set; } 

    public decimal Vat{ get; set; }

    public decimal SubTotal { get; set; } 

    public decimal Total { get; set; } 

    public ValueObjects.Address BillingAddress { get; set; } = null!;

    public ValueObjects.Address? ShippingAddress { get; set; }


    public IReadOnlyCollection<OrderItem> Items => _items;

    public OrderItem AddOrderItem(string description, string? itemId, decimal price, double quantity, decimal total) 
    {
        var orderItem = new OrderItem(description, itemId, price, quantity, total);
        _items.Add(orderItem);
        return orderItem;
    }

    public void RemoveOrderItem(OrderItem orderItem) => _items.Remove(orderItem);
}
