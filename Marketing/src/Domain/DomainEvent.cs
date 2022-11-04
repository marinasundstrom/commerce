﻿using MediatR;

namespace YourBrand.Marketing.Domain
{
    public abstract record DomainEvent : INotification
    {
        public Guid Id { get; } = Guid.NewGuid();
    }
}