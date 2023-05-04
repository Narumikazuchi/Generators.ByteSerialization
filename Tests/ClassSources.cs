namespace Tests;

static public class ClassSources
{
    public const String STOREMAP_FILE_SOURCE = @"using System;

public sealed class StoreMap
{
    public Guid StoreId { get; set; }

    public Guid ProductId { get; set; }
}";

    public const String PERSON_FILE_SOURCE = @"using System;

public sealed class Person
{
    public String FirstName { get; set; }

    public String LastName { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public Guid CustomerId { get; set; }
}";

    public const String PRODUCT_FILE_SOURCE = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public sealed class Product
{
    public Product([SerializeFromMember(nameof(Id))] Guid id)
    {
        Id = id;
    }

    public Guid Id { get; init; }

    public String Name { get; set; }

    public Decimal Price { get; set; }
}";
}