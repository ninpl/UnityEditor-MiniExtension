# UnityEditor-MiniExtension
Mini extension for Unity3D editor

Description: Small added to the Unity3D engine, adding functionality to the base Editor.

## Project Setup

1. Need Unity3D free or Pro (Tested on 4.6.1+)
2. You need the Visual Studio extension for Unity3D

### Resources

1. [Visual Studio Tools](https://visualstudiogallery.msdn.microsoft.com/20b80b8c-659b-45ef-96c1-437828fe7cf2)

## Features [V. 1.0.12]

> Current
> 
> - Clean PlayerPrefs and EditorPrefs
> - Create a screenshot of the scene display (Unity3d PRO only)
> - Added a tool for Debug with Raycast
> - Added different languages for the (English / Spanish) editor
> - Added Manager ScriptableObject
> - Added About.
> - Mini Localization

> Future
>
> - Console

### Screenshot

Clean PlayerPrefs and EditorPrefs
![Screenshot software](https://raw.githubusercontent.com/lPinchol/UnityEditor-MiniExtension/master/Resources/Img/ClearEditExt.png "ClearEditExt")

Take screenshot
![Screenshot software](https://raw.githubusercontent.com/lPinchol/UnityEditor-MiniExtension/master/Resources/Img/ScreenShotGOExt.png "ScreenShotGOExt")

Create a Debug Raycast
![Screenshot software](https://raw.githubusercontent.com/lPinchol/UnityEditor-MiniExtension/master/Resources/Img/RayCastDebugExt.png "RayCastDebugExt")

Create an asset (ScriptableObject)
![Screenshot software](https://raw.githubusercontent.com/lPinchol/UnityEditor-MiniExtension/master/Resources/Img/ScriptableObjectExt.png "ScriptableObjectExt")
Create new template use
```csharp
[Serializable]
public class ClassTemplateTest : ScriptableObject 
{

}
```

Use localization
![Screenshot software](https://raw.githubusercontent.com/lPinchol/UnityEditor-MiniExtension/master/Resources/Img/LocalizationExt.png "LocalizationExt")