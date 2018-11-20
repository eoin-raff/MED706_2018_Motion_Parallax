# p7

Git Repository for semester project for MED7: Sensing Media


## How to make and run a scene with a Motion Parallax
1. Ensure that the Main Camera has the `Body Source Manager`, `Body Source View`, and `Translate Camera` script components
2. In `BodySourceView` should take the `Body Source Manager`component in the `M Body Source Manager` field, and the `HeadPosition` prefab in the `M Joint Object` field
3. The `Translate Camera`script should be set up as follows:
   1. Z_Map Factor should be between 0 and 1
   2. The Camera should be attached in the Main Camera field
   3. Screen Size Inches is the diagonal size of the display
   4. Aspect ration A and B should match the displays Aspect Ratio (e.g. a 16:9 display would have `Aspect Ratio A = 16` and `Aspect Ration B = 9` 
4. When running the scene, use the Up and Down arrow keys to adjust the Camera's Vertical Position so that objects located at the Origin (0, 0, 0) in the scene are directly in line with the user's viewpoint.

## Using Splines
Bezier Splines used in this project are taken from [CatLikeCoding tutorial on Curves and Splines](https://catlikecoding.com/unity/tutorials/curves-and-splines/)

To use these splines in a scene:
1. Add an empty Game object and give it the `BezierSpline` component
2. Add and adjust Curves as necessary to maker the desired Spline
3. Add the `Spline Walker` component to the object which should travel the spline.
4. Ensure the spline walker has a reference to the desired spline, and set the parameters for looping and duration


## Developer Notes:

Use the following blog post to understand Git structure: 
https://nvie.com/posts/a-successful-git-branching-model/

This Repo should contain a Unity Project, along with various files for implementing Headtracking via Kinect, Wiimote, or other methods.

It should also contain files for data collection (e.g. via biometrics) and analysis (either via MATLAB or Python)

Reddit comment with MED7 paper on Parallax: https://www.reddit.com/r/Unity3D/comments/3upfqf/need_your_help_creating_a_frustum/cxh4ht4 from thread https://www.reddit.com/r/Unity3D/comments/3upfqf/need_your_help_creating_a_frustum/
