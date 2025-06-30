# MonteEngine

V 0.0.1

MonteEngine is a simplistic 2D engine with some Unity like collection handling. MonteEngine does provide some builtin solutions for most common 2D usecases. 

Monte Engine uses SDL2 as its backbone to access windows, renderers, controllers, audio systems, etc, through MonteSDL2Binds. 

MonteSDL2Binds.dll is required to build with MonteEngine in your projects. 

Files to have in the projects root and in the end built release products .exe root.

```
MonteSDL2Binds.dll
MonteEngine.dll
SDL2.dll
SDL2_image.dll
SDL2_ttf.dll
SDL2_mixer.dll
```

you can getthe SDL2 dll files from here. 
https://github.com/mmozeiko/build-sdl2 

Add this to your .csproj

```xml
<ItemGroup>
  <Reference Include="MonteEngine.dll" />
  <Reference Include="MonteSDL2Binds.dll" />
  <Reference Include="SDL2.dll" />
  <Reference Include="SDL2_image.dll" />
  <Reference Include="SDL2_ttf.dll" />
  <Reference Include="SDL2_mixer.dll" />
</ItemGroup>
```

Check the Documentation folder for more information and the Getting Started section for a quick launch project.
