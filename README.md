# mod1
Terrain generation and water simulation in Unity C#. (42 Silicon Valley)

<p float="left">
  <img src="https://github.com/ashih42/mod1/blob/master/Screenshots/voxel_rain.png" width="420" />
  <img src="https://github.com/ashih42/mod1/blob/master/Screenshots/particle_rain.png" width="420" />
</p>

Made in Unity version 2018.3.12f1
Recommended screen resolution: 1024 x 768

After building the game in Unity, copy `Assets/Maps` to the build directory as `Assets/Maps`.

## Releases

* [Windows Build](https://github.com/ashih42/mod1/releases/download/v00/mod1-Windows-v00.zip)

## Controls

### Camera Controls
* `W`, `A`, `S`, `D`, `Q`, `E` Move around.
* `Left Shift` + `Move Mouse` Look around.

### Terrain Controls
* Enter terrain file name in text field, and then click `Load Terrain` to build height map in Unity Terrain object.
  * demo1.mod1
  * demo2.mod1
  * demo3.mod1
  * demo4.mod1
  * demo5.mod1
* Click `Random Terrain` to generate and texturize a 3D mesh from layered Perlin noises.
  * Adapted from Sebastian Lague's [Procedural Terrain Generation](https://www.youtube.com/playlist?list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3) tutorial.

### Voxel Water Controls
* `Voxel Size` Adjust water size.
* `Numpad 1` Initialize water around 4 edges of terrain.
* `Numpad 2` Initialize water along 1 edge of terrain.
* Tap `Numpad 3` Spawn rain.
* Hold `Numpad 6` Spawn rain.
* Tap `Numpad .` Take 1 step in water-filling simulation.
* Hold `Numpad 0` Fast-forward water-filling simulation.
* Hold `M` Activate Moses.

### Physics-based Water Controls
* `Particle Size` Adjust water size.
* `1` Spawn water around 4 edges of terrain.
* `2` Spawn water along 1 edge of terrain.
* `3` Spawn rain.
* Hold `M` Activate Moses.

### Monster Controls
* `P` spawn a toon witch, who does not like water.
* `R` Remove all water and monsters.


