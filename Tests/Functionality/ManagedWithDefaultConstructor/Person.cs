namespace Tests.Functionality.ManagedWithDefaultConstructor;

public sealed record class Person
{
    public String FirstName { get; set; } = String.Empty;

    public String LastName { get; set; } = String.Empty;

    public DateOnly DateOfBirth { get; set; }

    public Guid CustomerId { get; set; }
}