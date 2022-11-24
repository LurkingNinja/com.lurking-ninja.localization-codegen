# localization-codegen

This is a simple package to automatically generate accessor code for the Localization package's SharedTable assets.
The usage is simple. Install this package from the Package Manager by selecting the plus icon and clicking the Add package from Git URL option.
After the package installed, all you need to do is to set up your localization, create your Locales. 
When you SAVE(!) a new Shared Table asset this package generates a file in the Assets/Plugins/LurkingNinja/LocalizationCodegen folder with the same name you named your asset with the exception of all spaces will be replaced with underscore character (_) and all special characters will be omitted. Please be careful with filenames from now on, they will serve as struct name in code as well.

Actual use of the new code if fairly simple, here is an example usage code:
```csharp
using UnityEngine;
using LurkingNinja.Localization;

public class LocalizationTest : MonoBehaviour
{
    private void Start()
    {
        Debug.Log(I18N.New_Table.Test);
    }
}
```
This example carries a couple of assumptions:
- created the new Table with the default name of "New Table"
- created a new entry in it called "Test"
- SAVED your changes (the AssetPostProcessor only run when you save your assets)

In order to make this example work
- copy the code above and paste into a file called "LocalizationTest.cs" in your project
- attach it to something in your scene

When you hit play, you should be able see in the console the string you assigned to the Test entry with your default locale.

From here, it's up to you how you organize your code and feel free to fork this package to your heart's content.
Hope this is useful.
