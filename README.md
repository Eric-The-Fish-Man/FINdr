# FINdr
Program for organizing the datasheet from fish tracking (the process which is included in the description). This is a basis for setting up a machine learning process that can identify the patterns of the fish.


THIS IS NOT A FULLY POLISHED PROGRAM. IT HAS INCOMPLETE FEATURES AND THUS IS NOT BUG FREE. IF ANYONE WISHES TO TAKE THIS PROGRAM AND BUILD ON IT, MODIFY IT, OR IMPROVE IT, THEY ARE FREE AND ENCOURAGED TO DO SO.



Instructions for how to process footage for tracking.


1. Prepare the footage for tracking
  The footage used for testing this program were top-down videos of zebrafish (one male, one female). To accurately process them the first step was to simplify the footage into a white background with black fish. This was done using a background subtraction method in the 3D modeling & compositing software Blender. A screenshot of the node output is included, to demonstrate the detailed process of how this works.

2. Track the footage
  The program used for tracking the footage in our example case was imagej fiji. In imagej I:
    set the image type to 8-bit
    limited the threshold
    used Mtrack2 to track (with min size at about 350, max size at infinity, and velocity and track length depended on the footage)
  This resulted in a csv file sorted like the following:
    |Fish_1_X|Fish_1_Y|-|Fish_2_X|Fish_2_Y|-|...
    |Fish_1_X|Fish_1_Y|-|Fish_2_X|Fish_2_Y|-|...
  etc. Noted is in cases where the behaviour has been previously recorded, the entries can be put into the spots with a dash
  
3. Run the program.
  When the program is run, there needs to be a csv file organized as shown above titled 'Results.csv'. Inside the file Program.cs at the top, there are three variables that keep track of the resolution, tank count (this program can track multiple tanks simultaneously), and the frame count. Make sure these agree with the processed footage.
  
  
  
4. Enjoy!
  This is by no means a perfect program, but it forms a good basis for adding a tracking algorithm. Feel free to edit, add, or improve this code!
