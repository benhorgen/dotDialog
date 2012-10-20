

[*** This Repo utilizes submodules... read the info at the bottom ***]


dotDialog
=========

A Dialog based micro framework: Platform bindings translate Element based objects into native UI objects


<b>Mission</b>:  Unite a dialog framework that translates 'elements' into native UI objects.  This enables developers to utilize a small <i>portion</i> of their UI code across multiple targeted platforms (such as iOS, Android, ASP.NET web apps, Windows 8, etc).  

<b>Key Notes</b>:  The three dialog repos are submodules to <i>other developers</i> github repos (they deserve the credit).  The direction of each of those dialog frameworks includes friendly collaboration and doesn't always keep compatibility between them.  We can manage that via github and a desire to be API compatible.  :-)

<b>Example</b>:  This repo explores how the three frameworks can be utilized for writing portable application code. This repo contains a rudimentary sample application which should sufficiently demonstrate how Dialog UI can be utilized for some cross platform UI.  This repo does not contain the source code for the different Dialog frameworks (it uses git submodules to pull it in for you).

This is a work in progress, but one that was/is heavily contributed to by Sam Lippert.  Sam created Android.Dialog (or forked it) when we collaborated on a previous MonoCross project.  Sam proved this concept to me and the sample code demonstrates how I've since been leveraging it.


The Sample App
---------------
The Personal Information Manager (PIM) sample app is located in the sample subfolder.  You must have Xamarin's Mono for Android toolset installed before opening either solution (in either Visual Studion or MonoDevelop).  The 'PersonalInfoManager.sln' file includes the MonoTouch sample also, so you'll need Xamarin's Mono for iOS installed too.

This sample utilizes the MonoCross cross platform mobile application framework to help create a larger shared code base. An important note, this dialog framework can help accelerate development of basic screens, this is not a replacement for the rich UI created when developers directly harness the native UI APIs.  MonoCross allows us to share UI on some screens and write platform specific UI for others.

This and other topics are covered in a book I coauthored,  "Professional Cross-Platform Mobile Development using C#" (Published Feb. 2012)



This repo includes 3 submodules
--------------------------------
Android.Dialog by githubber @sam-lippert

MonoTouch.Dialg by githubber  @migueldeicaza

MonoCross hosted by githubber @toejam

**After cloning the repo, don't forget to clone the submodules.  

Use the following commands from the root folder:
>'git submodule init'
>'git submodule update'




