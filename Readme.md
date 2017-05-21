# EntityFramework Core and DDD

The sample solution to demonstrate EF Core capabilities to store encapsulated DDD models.

### Solution structure

#### Src

* [Domain](/src/Domain). Domain models.
* [Application](/src/Application). Application queries and commands.
* [Infrastructure](/src/Infrastructure). EF Core DbContexts where Domain models are mapped into MS LocalDB.

#### Tests

* [Application.Tests](/tests/Application.Tests)
* [Infrastructure.Tests](/tests/Infrastructure.Tests)

### Domain

There are two aggregate roots here. ``PurchaseApproval`` and ``PurchaseApproval2``.
They are mainly the same but are targeted to demonstrate different approach to tackle some EF core mapping challenges.

From the point of encapsulation ``PurchaseApproval2`` exposing interfaces for the related objects when ``PurchaseApproval`` exposes concrete classes.

``PurchaseApproval``

* Has read only collection property ``Decisions`` exposed as ``IEnumerable<Decision>`` with backing private field.
* Has reference property ``Data`` as ``ApprovalData`` with protected setter.

``PurchaseApproval2``

* Has read only collection property ``Decisions`` exposed as ``IReadOnlyCollection<IDecision>`` with backing protected read only property.
* Has reference read only property ``Data`` as ``IApprovalData`` with backing protected property.

Additionally ``IApprovalRepository`` repository interface is exposed here as an example to use with ``PurchaseApproval`` aggregate root.

### Infrastructure

Here in ``Persistence`` folder 3 examples of EF Core DbContext are implemented and ``IApprovalRepository``

#### ``DomainDbContext``

Demonstrates how EF Core can help to map domain model ``PurchaseApproval``:
* Using [shadow properties](https://docs.microsoft.com/en-us/ef/core/modeling/shadow-properties) to map not existent model properties which are needed for persistence like related entities primary keys, related entities foreign keys.
* [Relationships](https://docs.microsoft.com/en-us/ef/core/modeling/relationships) to use shadow properties as foreign keys.

  The read only ``IEnumerable<Decision>`` property ``Decisions`` is successfully mapped by EF Core. For this to work backing collection should be created in the default constructor.

* Here related objects (``Decisions`` and ``Data``) have foreign keys as their primary keys.

The usage is demonstrated in the [test](/tests/Infrastructure.Tests/Persistence/ApprovalPersistenceTest.cs).

 #### ``DomainDbContext2``

The same as ``DomainDbContext`` only related entities use foreign keys as not primary keys. Here primary keys are DB generated IDENTITY fields. This is the case to demonstrate what should be done to ensure that removing of related objects will be correctly translated to DB ``delete`` statements.

The usage is demonstrated in the [test](/tests/Infrastructure.Tests/Persistence/ApprovalPersistence2Test.cs).

 #### ``Domain2DbContext``

More complicated mapping of model ``PurchaseApproval2`` where related objects are interfaces not concrete classes.

To ensure proper EF mapping the backing properties with concrete class types are used here together with their names as (magic) strings to map correctly.

The usage is demonstrated in the [test](/tests/Infrastructure.Tests/Persistence/Approval2PersistenceTest.cs).

### Application

* Demonstrates how Commands use domain model repository and model behaviour to implement commands
* Demonstrates how queries directly uses EF Db context to query and project to Dto objects.
* The [test](/tests/Application.Tests/QueriesTest.cs) demonstrate how easy to test those things.