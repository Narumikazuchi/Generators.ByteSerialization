using Narumikazuchi.Generators.ByteSerialization;

namespace Tests.Functionality.ManagedWithParameterizedConstructor;

public sealed record class Person
{
    public Person([SerializeFromMember(nameof(FirstName))] String firstName,
                  [SerializeFromMember(nameof(LastName))] String lastName)
    {
        this.FirstName = firstName;
        this.LastName = lastName;
    }

    public String FirstName { get; set; } = String.Empty;

    public String LastName { get; set; } = String.Empty;

    public DateOnly DateOfBirth { get; set; }

    public Guid CustomerId { get; set; }
}