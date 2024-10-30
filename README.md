# Prerequisite Packages

There are some packages you will need to install manually.

## FMOD

You can get the FMOD package from
the [Unity Asset Store](https://assetstore.unity.com/packages/tools/audio/fmod-for-unity-161631).

#### FMOD Studio

If you would like to edit the FMOD project, you will need to download FMOD Studio from
the [FMOD website](https://www.fmod.com/download#fmodstudio).

### Setup

After importing the package into Unity, FMOD will open the setup wizard:

1. Open the Sample Scene in `Assets/_Project/Scenes/Sample Scene.unity`.
2. Open the wizard again (`FMOD > Setup Wizard`).
3. In the **Linking** section, click **FMOD Studio Project**, and navigate to `FMOD/FMOD.fspro`.
4. Run through the rest of the wizard, and you're good to go!

## Odin

You can get Odin via the [educational license](https://odininspector.com/educational/ontario-tech-university) or
from the Unity Asset Store ([Inspector & Serializer](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041) and [Validator](https://assetstore.unity.com/packages/tools/utilities/odin-validator-227861)).

You will need the inspector, serializer and validator

### Setup

There's minimal setup. After opening the project, simply import the ```.unitypackage``` and go through the basic setup

## PrimeTween
Pick it up from [here](https://assetstore.unity.com/packages/tools/animation/primetween-high-performance-animations-and-sequences-252960), and import it from the package manager (Window > Package Manager > My Assets > Search PrimeTween).

## Main Shader (Ruidger)   

This shader is split into 4 main componenets to get the effect 
![image](https://github.com/user-attachments/assets/e5f044eb-791f-4c10-ab28-d354b1da19a4)
The first part is for the shader to identify the main texture of the object. In this case due to it being a template shader, I have a general node. 
![image](https://github.com/user-attachments/assets/ea4ed34f-1ec6-42ea-bb2d-9c37b71752a9)
This part of the shader is calculating the toon shading to create the different lights on the object. I calculated the dot product between the MainLight and the normal of the object. 
![image](https://github.com/user-attachments/assets/d6b4e9f7-9f10-4df8-ba16-b9d74d1ad32d)
The MainLight function take the built in light and finds the direction that the light is shining in and takes that output to perform dot product with the normals of the mesh. 




