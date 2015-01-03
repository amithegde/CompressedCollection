Welcome to CompressedCollection
=============================

This is a POC data structure to show how a collection which contains objects with repeating string property values can be compressed in memory for purposes such as caching.

Written in C#.

Typical Usecases:
---------------------
- While caching a large collection in memory for repeated access
- While caching a large collection in some sort of persistent cache such as a file/blob

Asymptotic Performance:
-----------------------
To Compress - O(n)
To Decompress - O(n)

Auxiliary space store redundant strings as a string array.

Idea:
-----
If a class has bunch of string properties and such string properties have repeating values in a collection, these redundant values take a lot of memory. If the collection is large (say 100k rows) and it needs to be cached in memory, lot of system memory gets clogged up.

This datastructure takes a stab at compressing such a collection, by iterating over all the properties, collecting strings and mapping them to a dictionary. Map index will be replaced on the property value, there by reducing the string size required to storing longer strings. The dictionary is further converted to an array of string, with the dictionary key being the array index.

To decompress, it goes through the dictionary (the array which has all the strings) and replaces all the string properties with their corresponding values, and returns the same.

Fast Reflection:
----------------
Since this needs to perform get/set operations on object properties, it becomes very important to get/set property values fast. For this purpose, it uses `Reflection.Emit` get dynamically emit Getters and Setters for the object, cache them for reuse. Take a look at the code [here][1]. This method works almost as fast as direct propery access.

![](https://raw.githubusercontent.com/amithegde/CompressedCollection/master/img/dyanamicGetterSetterPerfCheck.png)

Feel free to fork and use the code.

[1]:https://github.com/amithegde/CompressedCollection/blob/master/src/CompressedCollection/DynamicGetterSetter.cs
