# Development Guidelines for .NET

## Core Philosophy

I follow a belief that Event Sourcing, CQRS, and Event-Driven Architecture can simplify what many software developers may consider a "complex domain" by unraveling the complexity by identifying specific, step-by-step business operations. That is, persisting the decisions made through the logic with events to the event store and publishing related messages. The event store is managed by Marten and the event-driven aspect with messages being published is handled by Wolverine. 

This places a strong emphasis on the behavior of components, with logic encapsulated in small functions (methods in C#). With these loosely coupled pieces along with thoughtful naming of code and events, ideally even a non-technical stakeholder could understand what the code is doing. 

We are leveraging the "Critter Stack", AKA Marten and Wolverine, as much as possible throughout this system. We are building a reference architecture on how event sourcing and event-driven systems are built and how the Critter Stack can simplify much of the work through its low-ceremony code.

## Quick Reference

**Key Principles:**

- C# 12+ features and .NET 9
- Nullable reference types enabled
- Immutable data only
- Prefer pure functions for business logic
- Command Query Responsibility Segregation (CQRS) tp segregate reads amd write operations
- Event Sourcing preferred, not CRUD, for database storage. Empowered through use of CQRS.
- Test behavior, not implementation
- Domain-Driven Design influenced concepts, in a pragmatic way, not dogmatic
- Validate incoming data before it enters a domain entity, event, service from edges like an API or messaging

**Preferred Tools:**

- **Language**: C# 12+ (.NET 9)
- **Testing**: xUnit + Shouldly
- **State Management**: Prefer immutable patterns and records
- **Validation**: FluentValidation
- **Serialization**: System.Text.Json
- **Database**: Postgres
- **Event Sourcing**: Marten 8+
- **Document Store**: Marten 8+
- **Command Execution**: Wolverine 4+
- **Event-Driven Framework**: Wolverine 4+
- **Messaging Tool**: RabbitMQ as the message-broker using the AMQP to communicate between components in different value streams

## Prefer Pure Functions for Business Logic

As much as possible, we recommend that you try to create pure functions for any business logic or workflow routing logic that is responsible for "deciding" what to do next. The goal here is to make that code relatively easy to test inside of isolated unit tests that are completely decoupled from infrastructure. Moreover, using pure functions allows you to largely eschew the usage of mock objects inside of unit tests which can become problematic when overused.

Wolverine has a lot of specific functionality to move infrastructure concerns out of the way of your business or workflow logic. For tips on how to create pure functions for your Wolverine message handlers or HTTP endpoints, see:

- [A-Frame Architecture with Wolverine](https://jeremydmiller.com/2023/07/19/a-frame-architecture-with-wolverine/)
- [Testing Without Mocks: A Pattern Language by Jim Shore](https://www.jamesshore.com/v2/projects/nullables/testing-without-mocks)
- [Compound Handlers in Wolverine](https://jeremydmiller.com/2023/03/07/compound-handlers-in-wolverine/)
- [Isolating Side Effects from Wolverine Handlers](https://jeremydmiller.com/2023/04/24/isolating-side-effects-from-wolverine-handlers/)

## Testing Principles

### Behavior-Driven Testing

- **No "unit tests"** - this term is not helpful. Tests should verify expected behavior, treating implementation as a black box
- Test through the public API exclusively - internals should be invisible to tests
- Create tests in a separate project directory underneath the `tests` directory
- Tests that examine internal implementation details are wasteful and should be avoided
- Tests must document expected business behaviour

### Testing Tools

- **xUnit** for testing framework
- **Shouldly** for readable assertions
- **Moq** for mocking when absolutely necessary (prefer real implementations)
- All test code must follow the same C# standards as production code

### Test Organization

```
src/
  Payments/
    PaymentProcessorApplication/
      PaymentProcessorApplication.csproj
      PaymentProcessor.cs
      PaymentValidator.cs
tests/
  Payments/
    PaymentProcessorApplication.Tests/
      PaymentProcessor.Tests.cs // The validator is an implementation detail. Validation is fully covered, but by testing the expected business behaviour
```

### Test Data Pattern

Use builder pattern with fluent interfaces for test data:

```csharp
public class PaymentRequestBuilder
{
    private decimal _amount = 100m;
    private string _currency = "GBP";
    private string _cardId = "card_123";
    private string _customerId = "cust_456";
    private string? _description;
    private Dictionary<string, object>? _metadata;
    private string? _idempotencyKey;
    private AddressDetails _addressDetails = new AddressDetailsBuilder().Build();
    private PayingCardDetails _payingCardDetails = new PayingCardDetailsBuilder().Build();

    public PaymentRequestBuilder WithAmount(decimal amount)
    {
        _amount = amount;
        return this;
    }

    public PaymentRequestBuilder WithCurrency(string currency)
    {
        _currency = currency;
        return this;
    }

    public PaymentRequestBuilder WithCardId(string cardId)
    {
        _cardId = cardId;
        return this;
    }

    public PaymentRequestBuilder WithCustomerId(string customerId)
    {
        _customerId = customerId;
        return this;
    }

    public PaymentRequestBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public PaymentRequestBuilder WithMetadata(Dictionary<string, object> metadata)
    {
        _metadata = metadata;
        return this;
    }

    public PaymentRequestBuilder WithIdempotencyKey(string idempotencyKey)
    {
        _idempotencyKey = idempotencyKey;
        return this;
    }

    public PaymentRequestBuilder WithAddressDetails(AddressDetails addressDetails)
    {
        _addressDetails = addressDetails;
        return this;
    }

    public PaymentRequestBuilder WithPayingCardDetails(PayingCardDetails payingCardDetails)
    {
        _payingCardDetails = payingCardDetails;
        return this;
    }

    public PaymentRequest Build()
    {
        return new PaymentRequest(
            _amount,
            _currency,
            _cardId,
            _customerId,
            _description,
            _metadata,
            _idempotencyKey,
            _addressDetails,
            _payingCardDetails
        );
    }
}

public class AddressDetailsBuilder
{
    private string _houseNumber = "123";
    private string? _houseName = "Test House";
    private string _addressLine1 = "Test Address Line 1";
    private string? _addressLine2 = "Test Address Line 2";
    private string _city = "Test City";
    private string _postcode = "SW1A 1AA";

    public AddressDetailsBuilder WithHouseNumber(string houseNumber)
    {
        _houseNumber = houseNumber;
        return this;
    }

    public AddressDetailsBuilder WithHouseName(string? houseName)
    {
        _houseName = houseName;
        return this;
    }

    public AddressDetailsBuilder WithAddressLine1(string addressLine1)
    {
        _addressLine1 = addressLine1;
        return this;
    }

    public AddressDetailsBuilder WithAddressLine2(string? addressLine2)
    {
        _addressLine2 = addressLine2;
        return this;
    }

    public AddressDetailsBuilder WithCity(string city)
    {
        _city = city;
        return this;
    }

    public AddressDetailsBuilder WithPostcode(string postcode)
    {
        _postcode = postcode;
        return this;
    }

    public AddressDetails Build()
    {
        return new AddressDetails(
            _houseNumber,
            _houseName,
            _addressLine1,
            _addressLine2,
            _city,
            _postcode
        );
    }
}

// Usage in tests
var paymentRequest = new PaymentRequestBuilder()
    .WithAmount(250m)
    .WithCurrency("USD")
    .WithMetadata(new Dictionary<string, object> { ["orderId"] = "order_789" })
    .WithAddressDetails(new AddressDetailsBuilder()
        .WithCity("London")
        .WithPostcode("E1 6AN")
        .Build())
    .Build();
```

Key principles:

- Always return complete objects with sensible defaults
- Use fluent interfaces for readable test setup
- Build incrementally - extract nested object builders as needed
- Compose builders for complex objects
- Make builders immutable by returning new instances

## C# and .NET Guidelines

### Project Structure

### Project Structure

```
src/
  YourApp/                          # The core project with value objects and other primitives. Minimal dependencies.
  YourApp.Api/                      # Web API project along with application logic (use cases, services), domain models, entities, events, commands, queries. Almost always utilizes the Marten and Wolverin dependencies.
  YourApp.BackgroundWorkers/        # This project is only required if the service performs asynchronous work. It is seperate to run any background workers independently from a synchronous API
tests/
  YourApp.Api.Tests/                # Most the tests. Tests concerning API, application logic, domain model, etc.
  YourApp.Tests/                    # Tests for ensuring the value objects and other low-level primitives function exactly as expected.
```

Inside the .Core library, DO NOT create folders based on technical feature (Entities, DTO's, Services). Instead, create folders based on the actual business value that grouped set of code performs. Loosely following a vertical slice architecture style. A new developer should be able to look at the files/folders inside a project and understand what is is that the application does.

### C# Language Features

#### Records and Immutability

Use records for data transfer objects and value objects:

```csharp
// Good - Immutable record
public record PaymentRequest(
    decimal Amount,
    string Currency,
    string CardId,
    string CustomerId,
    string? Description = null,
    Dictionary<string, object>? Metadata = null,
    string? IdempotencyKey = null,
    AddressDetails AddressDetails,
    PayingCardDetails PayingCardDetails
);

public record AddressDetails(
    string HouseNumber,
    string? HouseName,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string Postcode
);

// For domain entities with behavior
public sealed class Payment
{
    public Guid Id { get; }
    public decimal Amount { get; }
    public string Currency { get; }
    public PaymentStatus Status { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? ProcessedAt { get; private set; }

    // A private constructor, often only used by the Create method or in test projects when assembly is exposed
    private Payment(Guid id, decimal amount, string currency)
    {
        Id = id;
        Amount = amount;
        Currency = currency;
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }
    
    // A common convention for Marten
    public static Payment Create(Guid id, decimal amount, string currency)
    {
        return new Payment(id, ammount, currency);
    }

    public void MarkAsProcessed()
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new InvalidOperationException("Payment can only be processed when pending");
        }

        Status = PaymentStatus.Processed;
        ProcessedAt = DateTime.UtcNow;
    }
}
```

#### Nullable Reference Types

Always enable nullable reference types:

```xml
<!-- In .csproj -->
<PropertyGroup>
  <Nullable>enable</Nullable>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <WarningsAsErrors />
</PropertyGroup>
```

```csharp
// Good - Explicit nullability
public class PaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        IPaymentRepository paymentRepository,
        ILogger<PaymentService> logger)
    {
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Payment?> GetPaymentAsync(PaymentId id, CancellationToken cancellationToken = default)
    {
        return await _paymentRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Payment> CreatePaymentAsync(PaymentRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var payment = new Payment(PaymentId.New(), request.Amount, request.Currency);
        await _paymentRepository.AddAsync(payment, cancellationToken);
        
        return payment;
    }
}
```

### Validation with FluentValidation

```csharp
public class PaymentRequestValidator : AbstractValidator<PaymentRequest>
{
    public PaymentRequestValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero")
            .LessThanOrEqualTo(10000)
            .WithMessage("Amount cannot exceed £10,000");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Must(BeValidCurrency)
            .WithMessage("Currency must be a valid ISO currency code");

        RuleFor(x => x.CardId)
            .NotEmpty()
            .Length(16)
            .WithMessage("Card ID must be 16 characters");

        RuleFor(x => x.AddressDetails)
            .NotNull()
            .SetValidator(new AddressDetailsValidator());

        RuleFor(x => x.PayingCardDetails)
            .NotNull()
            .SetValidator(new PayingCardDetailsValidator());
    }

    private static bool BeValidCurrency(string currency)
    {
        var validCurrencies = new[] { "GBP", "USD", "EUR" };
        return validCurrencies.Contains(currency);
    }
}

public class AddressDetailsValidator : AbstractValidator<AddressDetails>
{
    public AddressDetailsValidator()
    {
        RuleFor(x => x.HouseNumber)
            .NotEmpty()
            .WithMessage("House number is required");

        RuleFor(x => x.AddressLine1)
            .NotEmpty()
            .WithMessage("Address line 1 is required");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City is required");

        RuleFor(x => x.Postcode)
            .NotEmpty()
            .Matches(@"^[A-Z]{1,2}\d[A-Z\d]? ?\d[A-Z]{2}$")
            .WithMessage("Postcode must be a valid UK postcode");
    }
}
```

#### Model Validation in Tests

**CRITICAL**: Tests must use real validators and models from the main project, not redefine their own.

```csharp
// ❌ WRONG - Defining validators in test files
public class TestPaymentRequestValidator : AbstractValidator<PaymentRequest>
{
    public TestPaymentRequestValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0);
        // ... other rules
    }
}

// ✅ CORRECT - Import validators from the main project
using YourApp.Application.Validators;

[Fact]
public async Task ProcessPayment_ShouldFailValidation_WhenAmountIsNegative()
{
    // Arrange
    var paymentRequest = new PaymentRequestBuilder()
        .WithAmount(-100m)
        .Build();

    var validator = new PaymentRequestValidator();
    var paymentService = new PaymentService(_paymentRepository, validator, _logger);

    // Act
    var result = await paymentService.ProcessPaymentAsync(paymentRequest);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Should().BeOfType<Failure<Payment>>();
    var failure = (Failure<Payment>)result;
    failure.Error.Should().Contain("Amount must be greater than zero");
}
```

## Code Style

### Code Structure

- **No nested if/else statements** - use early returns, guard clauses, or pattern matching
- **Avoid deep nesting** in general (max 2 levels)
- Keep methods small and focused on a single responsibility
- Prefer flat, readable code over clever abstractions

### Naming Conventions

- **Methods**: `PascalCase`, verb-based (e.g., `CalculateTotal`, `ValidatePayment`)
- **Properties**: `PascalCase` (e.g., `PaymentAmount`, `CustomerDetails`)
- **Fields**: `_camelCase` for private fields
- **Constants**: `PascalCase` for public constants, `_camelCase` for private constants
- **Types**: `PascalCase` (e.g., `PaymentRequest`, `UserProfile`)
- **Files**: `PascalCase.cs` for all C# files
- **Test files**: `*.Tests.cs`

### No Comments in Code

Code should be self-documenting through clear naming and structure. Comments indicate that the code itself is not clear enough.

```csharp
// Avoid: Comments explaining what the code does
public static decimal CalculateDiscount(decimal price, Customer customer)
{
    // Check if customer is premium
    if (customer.Tier == CustomerTier.Premium)
    {
        // Apply 20% discount for premium customers
        return price * 0.8m;
    }
    // Regular customers get 10% discount
    return price * 0.9m;
}

// Good: Self-documenting code with clear names
private const decimal PremiumDiscountMultiplier = 0.8m;
private const decimal StandardDiscountMultiplier = 0.9m;

private static bool IsPremiumCustomer(Customer customer)
{
    return customer.Tier == CustomerTier.Premium;
}

public static decimal CalculateDiscount(decimal price, Customer customer)
{
    var discountMultiplier = IsPremiumCustomer(customer)
        ? PremiumDiscountMultiplier
        : StandardDiscountMultiplier;

    return price * discountMultiplier;
}
```

### API Endpoints and Command Handlers

Use the conventions that Wolverine and Marten try to push for instead of the verbose layering of API, Application, and Domain through methodologies like Clean Architecture. We sometimes called this A-Frame Architecture, as coined by Jim Shore.

Ideally classes and methods are static. There does not need to be a constructor with constructor injection, as Wolverine supports method injection, which is the preferred style. If one needs an `IDocumentSession` from Marten, just add it to the method after the command, entity, or other service.

Leverage the fact Wolverine will generate code around the handler, so our "business logic" can be as concise as possible. Below we are returning all the events (being persisted at the end of the event stream through Marten) and the messages (to be published by Wolverine). 

```csharp
public static class ScheduleFreightShipmentEndpoint
{
    public static ProblemDetails Validate(FreightShipment shipment)
    {
        return shipment.Status == FreightShipmentStatus.Scheduled
            ? new ProblemDetails { Status = StatusCodes.Status400BadRequest, Detail = "Shipment has already been scheduled" }
            : WolverineContinue.NoProblems;
    }

    [WolverinePost("/api/freight-shipments/schedule")]
    public static (CreationResponse<Guid>, IStartStream) Post(ScheduleShipment command, IDocumentSession session)
    {
        var (origin, destination) = command;

        // Can optimize with batched queries, compiled queries, or batched compiled queries! :)
        var originLocation =  session.Query<Location>().FirstOrDefault(x => x.Name == command.Origin);
        var destinationLocation = session.Query<Location>().FirstOrDefault(x => x.Name == command.Destination);

        if (originLocation is null)
            throw new InvalidOperationException($"Cannot locate Origin of '{command.Origin}' in our records");

        if (destinationLocation is null)
            throw new InvalidOperationException($"Cannot locate Origin of '{command.Destination}' in our records");

        var id = Guid.NewGuid();
        var scheduledAt = DateTime.UtcNow;
        var scheduled = new FreightShipmentScheduled(id, origin, destination, scheduledAt);
        var start = MartenOps.StartStream<FreightShipment>(scheduled);

        var response = new CreationResponse<Guid>("/api/freight-shipments/" + start.StreamId, start.StreamId);

        return (response, start);
    }
}

```
