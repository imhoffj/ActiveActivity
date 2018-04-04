ActiveActivity
==============
ActiveActivity is a sample project combining the work of two Xamarin view interaction libraries for Android. These libraries in combination, allow for a consistent development experience and pattern with the Android Lifecycle. Each library address core issues around Android lifecycle development that are uncommon between iOS and general shared development patterns. A key issue that can be problamatic is leveraging the Run on Main Thread to update UI components, and then calling async await inside that thread. By using await inside the Main thread, you can cause freezing and general crashing of the application. Another issue with using Main Thread is that references are scattered throughout the Acitivity and handling Resume / Pause of the lifecycle can create recursive cases of loading, reinitialization. 

This project was created to walk through the possibility of using both components togehter and to understand the primary issues solved and created from these approaches. The Async All The Way pattern does not exisit in the standard Android development lifecycle and forcing that pattern can cause issues without deep support. These libraries create an essential framework for using the AATW pattern with Android development. The framework can be leveraged in a consistent, reusable, definable way to promote development practices and hardening. 


Activity Controller
===================
Original: https://raw.githubusercontent.com/xamarin/android-activity-controller
Author: Jonathan Dick - https://github.com/Redth

The 'ActivityController' makes managing Android Activities more .NET friendly. By introducing the 
ActivityController, you are enabling the use of 'async/await' by starting activities through the 'StartActivityForResultAsync (..)' method.
By leveraging the ActivityController, you can use the same paradigms used in Core with Android Views, creating a consistent workflow. With ActivityController, you can easily encapsulate components and interactions with the driving force being reusability.


Activity Task
=============
Original: https://raw.githubusercontent.com/garuma/LibActivityTask
Author: Jérémie Laval - https://github.com/garuma

Activity Task is a great way to create asynchronous methods in Android that handles two things for you:

- Activity instance re-creation
- Activity lifecycle events

The idea behind ActivityTask is to help manage the Android lifecycle reference to Activity. You can use the Acitivty Scope to manage
UI updates without the UI Main Thread. The ActivityTask also helps handling Pause / Resume events. If the application is added to background, the ActivityTask will schedule the await continuation for when the Activity Resumes. By combining the Tasks with Lifecycle, you can remove the idea of inter activity management of the lifecycle with the idea of removing race comndition bugs and unccessary management.  