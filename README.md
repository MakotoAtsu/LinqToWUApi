# LinqToWuapi

Support to use .Net Linq expression to Search Windows Update object

<br/>

# How to use it

1. Use NuGet manager to import the library to your project. [LinqToWUApi](https://www.nuget.org/packages/LinqToWUApi)

2. Import Windows Update Agent API (WUApiLib.dll).
   Generally, it,s in the c:\windows\system32 folder
```
using WUApiLib;
```

<br>

3. Use Where Method to Search IUpdate object
```
// Create IUpdateSearcher 
var search = new UpdateSession().CreateUpdateSearcher();

// Use Where() to instead Search()
// Now you can enjoy in strong type hint
var result = search.Where(x => x.IsInstalled && x.Type == sUpdateType.Software);
```


# Note 
if you found something error like below :

> CS1769: Cannot be used across assembly boundaries because it has a generic type parameters that is an embedded interop type

> System.IO.FileNotFoundException: 'Could not load file or assembly 'Interop.WUApiLib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null'.

Please go to your .csproj and change COMReference element EmbedInteropTypes to 'false'
 ```
   <ItemGroup>
	  <COMReference Include="WUApiLib">
		  <VersionMinor>0</VersionMinor>
		  <VersionMajor>2</VersionMajor>
       ...
       ...
      
		  <EmbedInteropTypes>false</EmbedInteropTypes> <-- Change this from True to False
      
	  </COMReference>
	 </ItemGroup>
  ```
  
 # What' next?
 Maybe I will add some feature for this lib , like:
 - **Select()** method support
 - **Lazy loading** support

 #### Pull Request welcome 
 
