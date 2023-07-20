dropShadow
-------------------------------------
[Asset Store Link](http://u3d.as/L2d)  
© 2017 Justin Garza

PLEASE LEAVE A REVIEW OR RATE THE PACKAGE IF YOU FIND IT USEFUL!
Enjoy! :)

Contact  
-------------------------------------
Questions, suggestions, help needed?  
Contact me at:  
Email: jgarza9788@gmail.com  
Cell: 1-818-251-0647  
Contact Info: [justingarza.net/contact](http://justingarza.net/contact/)
  
Description/Features
-------------------------------------
Allows you to create a drop shadow effect.* standard or long shadow* works on mobile
	1. tested on Nexus9 (Android) and iPhone6s (iOS)

Terms of Use
-------------------------------------
You are free to add this asset to any game you’d like
However:  
please put my name in the credits, or in the special thanks section. :)  
please do not re-distribute.  

Table of Contents 
-------------------------------------
1. How it Works
2. Ways to Tweek and Get Other Effects
3. Cons

  
How it Works
-------------------------------------
below are the steps that take place to make this asset work.

1. A second camera will use the [Culling Mask](https://goo.gl/MsP743) and dropShadow.cs to create the _GlobalShadowTex (a texture).
2. In the dropShadowLayer (GameObject) the Shader will take the _GlobalShadowTex and blur it and render it on the spriteRenderer.

![Imgur](http://i.imgur.com/aNl3iv6.png)

![Imgur](http://i.imgur.com/MCiYyda.png)


Ways to Tweek and Get Other Effects 
-------------------------------------
Obviously changing the standard variables will allow you to change the effects a bit.

**_distance**  
The distance or spread of the shadow.

**_color**  
The color of the shadow (locked to black in the demo).

**_offsetX**  
The offset in the X direction.
you can also move the second camera for the same effect.

**_offsetY**
The offset in the Y direction.
you can also move the second camera for the same effect.

**_longShadow**
Whether or not to just render the shadow in just one direction.

**_longShadowAngle**
the Angle of the long shadow.

![Imgur](http://i.imgur.com/syPTvfv.png)

In addition to changing the inputs above you can also doings like change the rotation, distance, and position of the second camera.

Cons
-------------------------------------

* All objects will have their shadows rendered together.
* does not work on WebGL (for unknown reasons).





