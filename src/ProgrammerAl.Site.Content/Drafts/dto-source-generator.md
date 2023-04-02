Title: DTO SOurce Generator
Published: 6/21/2020
Tags: 
- C#
- Source Generator
---

### Some Mansplaining
Immutable objects are great. Some may say amazing. But not me, I say great. Whenever possible, I make every object in my C# code immutable. The upcoming `record` keyword in C# 9 will make working with those types of objects easier with less typing. Something like `public record Person(string Name, int Age)` will compile into something along the lines of (but not exactly):
```C#
public class Person
{
    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }

    public string Name { get; private set; }
    public int Age { get; private set; }
}
```

You know what else is great in C#? Nullable reference types AKA having the compiler remind you to check for null. When you have a class like this one below, the `?` operator on any data type will let the compiler know that it's possible that variable might be set to null.
```C#
public class PersonDTO
{
    public Person(string? name, int? age)
    {
        Name = name;
        Age = age;
    }

    public string? Name { get; set; }
    public int? Age { get; set; }
}
```

### Putting It Together
My preference is to use immutable objects and non-null references whenever possible. But we never control all of inputs to our applications. For example, when receiving a web request, or a request over a socket, or deserializing an object from a file. One strategy around this is to use a Data Transfer Object. A separate class that is used at the edges of your application and van  verify the validity of the given data. A final `PersonDTO` object would look something like this:
```C#
public class PersonDTO
{
    public Person(string? name, int? age)
    {
        Name = name;
        Age = age;
    }

    public string? Name { get; set; }
    public int? Age { get; set; }

    public bool CheckIsValid()
    {
        return !string.IsNullOrWhiteSpace(Name) && Age.HasValue;
    }

    public Person GeneratePerson()
    {
        if(CheckIsValid())
        {
            return new Person(Name!, Age!);
        }

        throw new Exception("TODO: Handle this in your own way");
    }
}
```

### DTOUtilities NuGet Package
You probably noticed how much of the DTO is the same as the original. With the new SOurce Generators feature in .NET, we can create a generator to create the DTOs for us at compile time. 

---

### *Side Quest Rewards*
- *600 xp*
- *Skill Upgrade: YAML Builds*
