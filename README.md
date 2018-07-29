# Nested Switches Builder

A class to build dynamically nested switches in C# .Net 

## Use case

Basic nested switches :

```
String path = "ABC";
String result = null;
switch (path[0])
{
    case 'A':
        switch (path[1])
        {
            case 'B':
                switch (path[1])
                {
                    case 'C':
                        result = "abc"
                        break;
                    case 'D':
                        result = "abd"
                        break;
                }
                break;
        }
        break;
}

Console.WriteLine(result); // "abc"
```

Using the NestedSwitchesBuilder :

```
NestedSwitchesBuilder<char, Func<String>> builder = new NestedSwitchesBuilder<char, Func<String>>();
builder.Add("ABC".AsEnumerable(), () => "abc");
builder.Add("ABD".AsEnumerable(), () => "abd");
var nestedSwitches = builder.Build();

Console.WriteLine(nestedSwitches("ABC".AsEnumerable())()); // "abc"
```

## Contribution

Contribution is highly welcome!

All contributions are assumed to be dual-licensed under MIT/Apache-2.

## License

`nested-switches-builder` is distributed under the terms of both the MIT license and the Apache License (Version 2.0).