# MonteEngine

A poormans synchronous 2D game engine for simple game products and jam games.

Crossplatform (Windows and Linux) game engine built using SDL2 as the backbone for rendering. 
Uses Sayers.SDL2.Core nuget package for now. 

X+ right, Y+ down. 

Provides collections based systems and components for basic game entities and systems. 

I.E. 
Entity, Canvas, Scene, SceneManager and etc. 


# TODO:

Update the content manager to be more async operated and more request oriented. 
    - Scene loads should manage if a resource is in use all ready before deloading and loading again
    - make scene changing a coroutine that returns the % of loaded objects. 

Move from synchronous do-as-it-is-deemed to async do-when-able kind of loop. 

Create own bindings for updated SDL2 (or sdl3 dev?)

Create documentation

Build with flag 'debug' or 'release' flag for debug systems to be enabled or not. 
Add generated .dll to the project. 
