3/10/21
- Drastically improve `DebugCommandHandler`
- Add  commands for `Help`, `Goto`, `Speed`, `Test`, `God`
- Add `goto` spawn data for FluffyFields, CandyCoast, and FancyRuins
- Move all classes under `ModStuff.Utility` namespace to root `ModStuff` namespace
- Split `Core` into separate classes
- Add `VarHelper` to hold variables integral to the mod that would otherwise require a lot of ugly `GameObject.Find()` searches
- Improve debugging a lot

1/27/21
- Add UI stuff
- Make custom debug commands work

1/26/21
- Create `Core` to hold integral functions for the entire mod
-  Create `SaveManager` to handle saving data to PlayerPrefs, Entity, save files (todo), and custom files (todo)
- Create `DebugManager` to output debug info to `output.txt` file. Output a test "GAME STARTED" in `LudositySplash.Awake()`
- Create project. Realized I took a wrong turn in life that has led me to rewrite 18k+ lines of code for a mod that a handful of people would use :)