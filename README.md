# Localization Codegen
This is a simple package to automatically generate accessor code for the [Localization](https://docs.unity3d.com/Packages/com.unity.localization@1.4/manual/index.html) package's [SharedTableData](https://docs.unity3d.com/Packages/com.unity.localization@1.4/api/UnityEngine.Localization.Tables.SharedTableData.html) assets.
## Installation
Install the prerequisite [Codegen](https://github.com/LurkingNinja/com.lurking-ninja.codegen) package and after that, this package from the Package Manager by selecting the plus icon and clicking the Add package from Git URL option.
You can choose manually installing the package or from GitHub source
### Using direct git source code
Use the Package Manager's ```+/Add package from git URL``` function.
The URL you should use is this:
```
https://github.com/LurkingNinja/com.lurking-ninja.localization-codegen.git?path=Packages/com.lurking-ninja.localization-codegen
```
### Manual install
1. Download the latest ```.zip``` package from the [Release](https://github.com/LurkingNinja/com.lurking-ninja.localization-codegen/releases) section.
2. Unpack the ```.zip``` file into your project's ```Packages``` folder.
3. Open your project and check if it is imported properly.
## Usage
After the package installed, all you need to do is to set up your localization, create your Locales. 
When you SAVE(!) a new Shared Table asset this package generates a file in the ```Assets/Plugins/LurkingNinja/LocalizationCodegen``` folder with the same name you named your asset with the exception of all spaces will be replaced with underscore character (_) and all special characters will be omitted. Please be careful with filenames from now on, they will serve as struct name in code as well.

Actual use of the new code if fairly simple, here is an example usage code:
```csharp
using UnityEngine;
using LurkingNinja.Localization;

public class LocalizationTest : MonoBehaviour
{
    private void Start() => Debug.Log(I18N.New_Table.Test);
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
