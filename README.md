解析从[Sockets Services for Vizzy](https://www.simplerockets.com/Mods/View/298478/Sockets-service-for-Vizzy)获得的结构化信息，编写时用于解析来自[Droid Pad](https://github.com/umer0586/DroidPad)的信息以实现用手机或平板控制JNO的载具。

算得上是头一次写Unity，非常初步，在添加Vizzy块上借用了很多[Vizzy++](https://github.com/sflanker/sr2-vizzyplusplus)的代码（没搞明白如何利用Juno Harmony，没有0Harmony.dll就无法打包，有的话会和Juno Harmony冲突😅）。使用了[Json.NET](https://github.com/JamesNK/Newtonsoft.Json)

用于过于早期，暂请自行构建。

获得代码后要先去unity打开，版本错误是正常的（我用的版本多了个c1，不过没事），载入ModTools包，然后您可以从Assets/Scripts/Mod.cs的AppendedVizzyBlocks（被添加的Vizzy块）、Assets/Scripts/Vizzy/DataProcessingHelper 里的一切（添加的Vizzy块的程序）、Assets/Content/XML UI/Vizzy/HelperToolbox.xml(将被添加的Vizzy块、颜色、样式等在游戏中被显示的样子)开始修改。

# in English (Machnine translation)

Parse structured data from [Sockets Services for Vizzy](https://www.simplerockets.com/Mods/View/298478/Sockets-service-for-Vizzy), written to parse data sended by [Droid Pad]( https://github.com/umer0586/DroidPad) to control vehicles in Juno:New Origins with a mobile phone or tablet.

It was my first time writing Unity, and copied lots of codes from [Vizzy++](https://github.com/sflanker/sr2-vizzyplusplus) to make it works to add Vizzy blocks (I didn't figure out how to use Juno Harmony, I couldn't package the mod without 0Harmony.dll, and if I put it into the mod, it conflicts with Juno Harmony😅). I used [Json.NET](https://github.com/JamesNK/Newtonsoft.Json) to parse JSON data.
For too early use, please build it yourself.

I used an version with extra c1 in its version code so it's okay to see different editor version waring. You can start editing from _Dictionnary AppendedVizzyBlock_ in Assets/Scripts/Mod.cs , anything in Assets/Scripts/Vizzy/DataProcessingHelper (program for appended Vizzy blocks), and Assets/Content/XML UI/Vizzy/HelperToolbox.xml (how the added Vizzy blocks, colors, styles, etc. will be displayed in the game).
