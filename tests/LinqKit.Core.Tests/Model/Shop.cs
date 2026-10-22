using System;
using System.Collections.Generic;

namespace LinqKitCoreTests.Model;

public enum SalesChannel
{
    Store,
    Web
}

public class Customer
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Country { get; set; }

    public string Currency { get; set; }

    public List<Purchase> Purchases { get; set; } = new();
}

public class Purchase
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string Country { get; set; }

    public string Currency { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public SalesChannel Channel { get; set; }

    public DateTime Date { get; set; }
}

public class Shipment
{
    public int Id { get; set; }

    public int PurchaseId { get; set; }

    public int Quantity { get; set; }
}

public static class Shop
{
    public static List<Customer> Customers()
    {
        var purchases = Purchases();
        var customers = new List<Customer>
        {
            new() { Id = 1, Name = "Alice", Country = "FI", Currency = "EUR" },
            new() { Id = 2, Name = "Bob", Country = "SE", Currency = "SEK" },
            new() { Id = 3, Name = "Carol", Country = "FI", Currency = "EUR" }
        };
        customers.ForEach(c => c.Purchases = purchases.FindAll(p => p.CustomerId == c.Id));
        return customers;
    }

    public static List<Purchase> Purchases() =>
    [
        new() { Id = 10, CustomerId = 1, Country = "FI", Currency = "EUR", Description = "laptop", Price = 1500m, Quantity = 1, Channel = SalesChannel.Web, Date = new DateTime(2026, 1, 1) },
        new() { Id = 11, CustomerId = 1, Country = "FI", Currency = "EUR", Description = "mouse", Price = 20m, Quantity = 2, Channel = SalesChannel.Store, Date = new DateTime(2026, 1, 1) },
        new() { Id = 12, CustomerId = 2, Country = "SE", Currency = "EUR", Description = "desk", Price = 400m, Quantity = 1, Channel = SalesChannel.Store, Date = new DateTime(2026, 1, 2) },
        new() { Id = 13, CustomerId = 2, Country = "NO", Currency = "SEK", Description = "chair", Price = 150m, Quantity = 0, Channel = SalesChannel.Web, Date = new DateTime(2026, 1, 2) },
        new() { Id = 14, CustomerId = 3, Country = "FI", Currency = "EUR", Description = "monitor", Price = 300m, Quantity = 2, Channel = SalesChannel.Web, Date = new DateTime(2026, 1, 3) }
    ];

    public static List<Shipment> Shipments() =>
    [
        new() { Id = 100, PurchaseId = 10, Quantity = 1 },
        new() { Id = 101, PurchaseId = 11, Quantity = 1 },
        new() { Id = 102, PurchaseId = 11, Quantity = 1 },
        new() { Id = 103, PurchaseId = 14, Quantity = 1 }
    ];
}
