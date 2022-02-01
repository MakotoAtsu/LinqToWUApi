# LinqToWuapi

Support to use .Net Linq expression to Search Windows Update object

How to use it:

1. Import Windows Update Agent API (WUAPI)
```
using WUApiLib;
```

2. Use Where Method to Search IUpdate object
```
// Create IUpdateSearcher 
var search = new UpdateSession().CreateUpdateSearcher();

// Use Where() to instead Search()
// Now you can enjoy in strong type hint
var result = search.Where(x => !x.IsInstalled && x.Type == UpdateType.utSoftware);
```

