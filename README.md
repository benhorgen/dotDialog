

[*** This Repo utilizes submodules... read the info at the bottom ***]


dotDialog
=========

A Dialog based micro framework: Platform bindings translate Element based objects into native UI objects


Mission:  Unite a dialog framework that translates 'elements' into native UI objects.  This enables developers to utilize a small <b>portion<b/> of their UI code across multiple targeted platforms (such as iOS, Android, ASP.NET web apps, Windows 8, etc).  

Key Notes:  The three dialog repos are submodules to other developers github repos.  The direction of each of those dialog frameworks includes friendly collaboration and doesn't always keep compatibility between them.  We can manage that via github and a desire to be API compatible.  :-)

Example:  This repo explores how the three frameworks can be utilized for writing portable application code, this repo does not contain the source code for the different Dialog frameworks.  This repo contains on the sample application, which is rudimentary, but should sufficiently demonstrate how Dialog UI can be utilized for some cross platform UI.

This is a work in progress, but one that was/is heavily contributed to by Sam Lippert.  Sam created Android.Dialog (or forked it) when we collaborated on a previous MonoCross project.  Sam proved this concept to me.  The sample code demonstrates how I've since been leveraging it.



This repo includes 3 submodules
--------------------------------
Android.Dialog by githubber @sam-lippert

MonoTouch.Dialg by githubber  @migueldeicaza

MonoCross hosted by githubber @toejam



The Personal Information Manager sample (PIM) is located in the sample subfolder.  You must have Xamarin's Mono for Android toolset installed before opening either solution (in either Visual Studion or MonoDevelop).  The 'PersonalInfoManager.sln' file includes the MonoTouch sample also, so you'll need Xamarin's Mono for iOS installed too.




**After cloning the repo, don't forget to clone the submodules.  

Use the following commands from the root folder:
>'git submodule init'
>'git submodule update'




