Slapper.AutoMapper
=================
*Slap your data into submission.*

Slapper.AutoMapper maps dynamic data to static types.

<a href="https://www.nuget.org/packages/Slapper.AutoMapper"><img src="https://img.shields.io/nuget/v/Slapper.AutoMapper.svg" alt="NuGet Version" /></a> 
<a href="https://www.nuget.org/packages/Slapper.AutoMapper"><img src="https://img.shields.io/nuget/dt/Slapper.AutoMapper.svg" alt="NuGet Download Count" /></a>

###What is it?###

Slapper.AutoMapper ( Pronounced Slapper-Dot-Automapper ) is a mapping library that can convert dynamic data into 
static types and populate complex nested child objects.

It primarily converts C# dynamics and `IDictionary<string, object>` to strongly typed objects and supports
populating an entire object graph by using underscore notation to underscore into nested objects.

Why use an IDictionary? Because a C# dynamic ( well really an ExpandoObject ) can easily be cast to one allowing 
this library to be used in a variety of ways not only with dictionaries of property names and values but with dynamics as well.

Okay, so what... doesn't other ORMs do this?

Answer: Yes and no but the philosophy of this project is much different. This small library is meant to be used as a 
building block in a larger solution and puts a great emphasis on its ability to map to complex nested properties such as mapping 
a Customer and it's list of Orders and it's list of OrderDetails.

###Is this an ORM?###

No, this is not an ORM in itself but can easily be extended to create one. This library can be thought of as a building 
block of an ORM or used as an extension to an existing ORM or Micro-ORM.

ORMs typically query the database and then map the data into objects. Slapper just handles the mapping part and essentially
only has one input: a dictionary of property names and values.

###What problems does this solve?###

Simply put, it allows you to convert dynamic data into strongly typed objects with ease and populating complex nested child 
objects in your object hierarchy comes free out of the box --something severely lacking in almost every Micro-ORM solution!

###Auto mapping?###

Yep, Slapper.AutoMapper stays true to its name and allows auto-mapping between dynamic data and static types by using
conventions to find a given classes identifier ( the property that gives the class uniqueness ). This allows Slapper to 
figure out how to effectively group objects together so that you do not get duplicate results. You can even supply your 
own conventions or manually specify the identifiers by either calling a simple API method or decorating your types with 
an attribute.

And yes, multiple identifiers aka Composite Primary Keys are supported out of the box!

###Some more ramblings...###

Micro-ORMs have been springing up left and right but many of them are quite basic in their functionality. Many have also 
been opting for either very basic mapping to strongly typed objects or skipping it completely and opting for a completely 
dynamic solution.

Dynamics are super cool and have their place but strongly typed objects have their place too and that is what this library 
focuses on... converting dynamic data into strongly typed objects with strong support for populating nested child properties. 

###Target Audience###

The target audience is C# developers looking to enhance an ORM or write their own. Slapper.AutoMapper
can take care of a lot of the hard work of mapping back to strongly typed objects.

Because Slapper.AutoMappers primary input is simply a dictionary of property names and values, as long as you can get your data
into that form, you're good to go. One thing to note is that the values must be the same data types as the strongly typed object's properties/fields
you are wishing to populate. Slapper.AutoMapper does not handle data type conversions, that is up to you the consumer to feed the proper
data into the library.

And that's it, feel free to explore the examples below and the unit tests and hack away. This library is licensed with the MIT license
so feel free to re-use the code in your own projects any way you please. I only ask that you keep the license comment at the top of the
file or any file that uses significant portions of this projects code for proper attribution.

Slapper.AutoMapper is also available on NuGet as a compiled dll if you prefer that. Check it out at: http://www.nuget.org/packages/Slapper.AutoMapper/

Now let the slapping commence! :)


Usage - Mapping
===============

###Simple Example Using a Dictionary###

The following simple example maps a dictionary of property names and values to a Person class.

```csharp
public class Person
{
	public int Id;
	public string FirstName;
	public string LastName;
}
		
[Test]
public void Can_Map_Matching_Field_Names_With_Ease()
{
    // Arrange
    var dictionary = new Dictionary<string, object>
                            {
                                { "Id", 1 },
                                { "FirstName", "Clark" },
                                { "LastName", "Kent" }
                            };

    // Act
    var person = Slapper.AutoMapper.Map<Person>( dictionary );

    // Assert
    Assert.NotNull( person );
    Assert.That( person.Id == 1 );
    Assert.That( person.FirstName == "Clark" );
    Assert.That( person.LastName == "Kent" );
}
```

###Simple Example Using Dynamic###

The following simple example maps a dynamic object to a Person class.

When mapping dynamics use the `MapDynamic<T>()` method instead of the `Map<T>()` method.

```csharp
public class Person
{
	public int Id;
	public string FirstName;
	public string LastName;
}
		
[Test]
public void Can_Map_Matching_Field_Names_Using_Dynamic()
{
    // Arrange
    dynamic dynamicPerson = new ExpandoObject();
    dynamicPerson.Id = 1;
    dynamicPerson.FirstName = "Clark";
    dynamicPerson.LastName = "Kent";

    // Act
    var person = Slapper.AutoMapper.MapDynamic<Person>( dynamicPerson ) as Person;
            
    // Assert
    Assert.NotNull( person );
    Assert.That( person.Id == 1 );
    Assert.That( person.FirstName == "Clark" );
    Assert.That( person.LastName == "Kent" );
}
```

###Mapping Nested Types Using a Dictionary###

The following example maps a list of dictionaries of property names and values to a Customer class and using underscore notation ("_"), 
Slapper.AutoMapper properly populates the nested child types. This is really what I would consider this libraries secret sauce.
You can just as easily use a list of dynamics which is demonstrated below too which is what is typically returned back from Micro ORMs.

As an example, the following SQL would return similar results to what is in the dictionaries in the example below ( Note the use of SQL aliases ).

*Now it may not seem immediately obvious but what we are really achieving here is something very interesting... we are effectively combining
SQL and the mapping to C# objects at the same time by use of SQL aliases.*

```sql
SELECT	c.CustomerId,
		c.FirstName,
		c.LastName,
		o.OrderId AS Orders_OrderId,
		o.OrderTotal AS Orders_OrderTotal,
		od.OrderDetailId AS Orders_OrderDetails_OrderId,
		od.OrderDetailId AS Orders_OrderDetails_OrderDetailId,
		od.OrderDetailTotal AS Orders_OrderDetails_OrderDetailTotal
FROM	Customer c
		JOIN Order o ON c.CustomerId = o.CustomerId
		JOIN OrderDetail od ON o.OrderId = od.OrderId
```

This example is indicative of the results you would commonly encounter when querying a database and joining on an Orders
and OrderDetails table --you would get back duplicate results in some fields. Notice how the CustomerId in both dictionaries
are the same. Because of Slapper.AutoMapper's default conventions, it will identify the CustomerId field as being the 
identifier ( or primary key so to speak ). This means that when it attempts to convert the second dictionary to a Customer 
object, it will see that it has already created a Customer object with an CustomerId of 1 and will simply re-use the previous 
instance resulting in only one Customer object being returned back. This is how Slapper.AutoMapper effectively groups results
together and is the key to this libraries awesomeness.


```csharp
public class Customer
{
	public int CustomerId;
	public string FirstName;
	public string LastName;
	public IList<Order> Orders;
}

public class Order
{
	public int OrderId;
	public decimal OrderTotal;
	public IList<OrderDetail> OrderDetails;
}

public class OrderDetail
{
	public int OrderDetailId;
	public decimal OrderDetailTotal;
}
		
[Test]
public void I_Can_Map_Nested_Types_And_Resolve_Duplicate_Entries_Properly()
{
	// Arrange
	var dictionary = new Dictionary<string, object>
						 {
							 { "CustomerId", 1 },
							 { "FirstName", "Bob" },
							 { "LastName", "Smith" },
							 { "Orders_OrderId", 1 },
							 { "Orders_OrderTotal", 50.50m },
							 { "Orders_OrderDetails_OrderDetailId", 1 },
							 { "Orders_OrderDetails_OrderDetailTotal", 25.00m }
						 };

	var dictionary2 = new Dictionary<string, object>
						 {
							 { "CustomerId", 1 },
							 { "FirstName", "Bob" },
							 { "LastName", "Smith" },
							 { "Orders_OrderId", 1 },
							 { "Orders_OrderTotal", 50.50m },
							 { "Orders_OrderDetails_OrderDetailId", 2 },
							 { "Orders_OrderDetails_OrderDetailTotal", 25.50m }
						 };

	var list = new List<IDictionary<string, object>> { dictionary, dictionary2 };

	// Act
	var customers = Slapper.AutoMapper.Map<Customer>( list );

	// Assert
	
	// There should only be a single customer
	Assert.That( customers.Count() == 1 );

	// There should only be a single Order
	Assert.That( customers.FirstOrDefault().Orders.Count == 1 );

	// There should be two OrderDetails
	Assert.That( customers.FirstOrDefault().Orders.FirstOrDefault().OrderDetails.Count == 2 );
}

[Test]
public void I_Can_Map_Nested_Types_And_Resolve_Duplicate_Entries_Properly_Using_Dynamics()
{
    // Arrange
    dynamic customer1 = new ExpandoObject();
    customer1.CustomerId = 1;
    customer1.FirstName = "Bob";
    customer1.LastName = "Smith";
    customer1.Orders_OrderId = 1;
    customer1.Orders_OrderTotal = 50.50m;
    customer1.Orders_OrderDetails_OrderDetailId = 1;
    customer1.Orders_OrderDetails_OrderDetailTotal = 25.00m;

    dynamic customer2 = new ExpandoObject();
    customer2.CustomerId = 1;
    customer2.FirstName = "Bob";
    customer2.LastName = "Smith";
    customer2.Orders_OrderId = 1;
    customer2.Orders_OrderTotal = 50.50m;
    customer2.Orders_OrderDetails_OrderDetailId = 2;
    customer2.Orders_OrderDetails_OrderDetailTotal = 25.50m;

    var customerList = new List<dynamic> { customer1, customer2 };

    // Act
    var customers = Slapper.AutoMapper.MapDynamic<Customer>( customerList );

    // Assert

    // There should only be a single customer
    Assert.That( customers.Count() == 1 );

    // There should only be a single Order
    Assert.That( customers.FirstOrDefault().Orders.Count == 1 );

    // There should be two OrderDetails
    Assert.That( customers.FirstOrDefault().Orders.FirstOrDefault().OrderDetails.Count == 2 );
}
```

Usage - Auto Mapping and Identifiers
====================================

###Auto Mapping###

Auto mapping allows Slapper to figure out how to effectively group objects together so that you do not get 
duplicate results. Now internally, no actual grouping is happening but this is the easiest way to conceptualize
how it works.

*For the curious, the actual implementation relies upon an instance cache implemented as a Dictionary where the key is the all of
the identifier's hashes summed together and the value being the instance.*

A classes identifier(s) play an important role in the ability of the mapper to effectively group objects together. If no
identifiers are found, the mapper will still map the results to the requested type but there will be duplicates in the results.


####Default Convention####
Slapper.AutoMapper uses three different conventions in an attempt to locate/match a requested types
identifier:
- Id
- TypeName + Id
- TypeName + Nbr

For example, if your Customer object has any of the following properties or fields, it will use it as the identifier:
- Id
- CustomerId
- CustomerNbr

####Creating Your Own Convention####

You can specify your own conventions very easily. The following example creates a convention of TypeName + _Id:

```csharp
Slapper.AutoMapper.Configuration.IdentifierConventions.Add( type => type.Name + "_Id" );
````

####Manually Specifying the Identifier(s)####

Slapper allows you to manually specify a classes identifiers. 1 through N number of identifiers are supported.

The following example specifies two identifiers for the Customer object by using the AddIdentifiers() method:


```csharp
public class Customer
{
	public int CustomerId;

	public string CustomerType;

	public string FirstName;

	public string LastName;
}

Slapper.AutoMapper.Configuration.AddIdentifiers( typeof( Customer ), new List<string> { "CustomerId", "CustomerType" } );
````

####Attribute-based Identifiers####

Slapper.AutoMapper also supports attribute-based identifiers.

By default, the library uses it's own Id attribute that allows you to simply decorate the identifiers on your class with
a `[Slapper.AutoMapper.Id]` attribute.

If you wish to use your own attribute instead of the default one, just set the Type to use on the following field:

```csharp
Slapper.AutoMapper.Configuration.IdentifierAttributeType = typeof( YourCustomAttribute );
``` 

The following example specifies two identifiers for the Customer object:

```csharp
public class Customer
{
	[Slapper.AutoMapper.Id]
	public int CustomerId;

	[Slapper.AutoMapper.Id]
	public string CustomerType;

	public string FirstName;

	public string LastName;
}
````

Usage - Caching
===============

####Caching Explained####

Slapper.AutoMapper internally maintains a cache of every object it creates, referred to as the instance cache.
This cache plays an important role in Slapper's ability to easily lookup existing objects and ultimately assists
in the ability for Slapper.AutoMapper to populate complex nested types.

Slapper.AutoMapper itself never removes an instance from this cache, so if you tell it to create 50,000 objects, 
then there are going to be 50,000 objects in the cache for the lifetime of the current thread or Http context. 

The instance cache exists for the lifetime of the current thread and each of your application's threads will
get it's own unique cache making use of this library thread safe.

####Cache Backing Store####

The instance cache backing store will either make use of the HttpContext if one exists or the CallContext of the
executing thread. The library makes use of reflection in order to persist the cache in the HttpContext when
neccessary so that the library does not require a dependency on the System.Web library.

####Clearing the Cache###

Slapper never clears the cache because we feel that it should be the consumer of this library that should have that 
responsibility.

If you would like to clear this cache, you can do so at any time like so:

```csharp
Slapper.AutoMapper.Cache.ClearInstanceCache();
````


###License###

MIT License:

Copyright (c) 2016, Randy Burden and contributors.
All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the "Software"), to deal in the Software without restriction, including 
without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the 
following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial 
portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN 
NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 

Description:

Slapper.AutoMapper maps dynamic data to static types. Slap your data into submission!

Slapper.AutoMapper ( Pronounced Slapper-Dot-Automapper ) is a mapping library that can convert 
dynamic data into static types and populate complex nested child objects.
It primarily converts C# dynamics and IDictionary<string, object> to strongly typed objects and supports
populating an entire object graph by using underscore notation to underscore into nested objects.
