<p align="center">
<img align="center" src="https://raw.githubusercontent.com/coryleach/UnityPackages/master/Documentation/GameframeFace.gif" />
</p>
<h1 align="center">Gameframe.Async 👋</h1>

<!-- BADGE-START -->
[![Build Status](https://travis-ci.org/coryleach/UnityAsync.svg?branch=master)](https://travis-ci.org/coryleach/UnityAsync)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/d2749fdbc70f422a9d1efccb56d48bff)](https://www.codacy.com/manual/coryleach/UnityAsync?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=coryleach/UnityAsync&amp;utm_campaign=Badge_Grade)
![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/coryleach/UnityAsync?include_prereleases)
[![openupm](https://img.shields.io/npm/v/com.gameframe.async?label=openupm&amp;registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.gameframe.async/)
[![license](https://img.shields.io/github/license/coryleach/UnityAsync)](https://github.com/coryleach/UnityAsync/blob/master/LICENSE)

[![twitter](https://img.shields.io/twitter/follow/coryleach.svg?style=social)](https://twitter.com/coryleach)
<!-- BADGE-END -->

> Async task utility package for Unity    
> Helper methods for starting tasks on the Unity thread.    
> Start and await coroutines from any thread.

## Quick Package Install

#### Using UnityPackageManager (for Unity 2019.3 or later)
Open the package manager window (menu: Window > Package Manager)<br/>
Select "Add package from git URL...", fill in the pop-up with the following link:<br/>
https://github.com/coryleach/UnityAsync.git#1.0.4<br/>

#### Using UnityPackageManager (for Unity 2019.1 or later)

Find the manifest.json file in the Packages folder of your project and edit it to look like this:
```js
{
  "dependencies": {
    "com.gameframe.async": "https://github.com/coryleach/UnityAsync.git#1.0.4",
    ...
  },
}
```

<!-- DOC-START -->
<!-- 
Changes between 'DOC START' and 'DOC END' will not be modified by readme update scripts
-->

## Quick Package Install

### Using UnityPackageManager (for Unity 2019.3 or later)
Open the package manager window (menu: Window > Package Manager)<br/>
Select "Add package from git URL...", fill in the pop-up with the following link:<br/>
[https://github.com/coryleach/UnityAsync.git#1.0.3](https://github.com/coryleach/UnityAsync.git#1.0.3)<br/>

### Using OpenUPM

The package is available on the [openupm registry](https://openupm.com). It's recommended to install it via [openupm-cli](https://github.com/openupm/openupm-cli).

```console
openupm add com.gameframe.async
```

### Using UnityPackageManager (for Unity 2019.2 or earlier)
Find the manifest.json file in the Packages folder of your project and edit it to look like this:
```js
{
  "dependencies": {
    "com.gameframe.async": "https://github.com/coryleach/UnityAsync.git#1.0.3",
    ...
  },
}
```

## Quick Start

### Start and Await a coroutine from anywhere
```c#
//Start a coroutine from anywhere without a monobehaviour
//Your coroutine will run on the main thread
var task = CoroutineRunner.RunAsync(MyCoroutine());
//You can await the returned task which will complete when the coroutine is done
await task;
```

### Start a Task on the main unity thread from anywhere
```c#
//This will execute MyMethod on the main Unity thread
var task = UnityTaskUtil.RunOnUnityThreadAsync(MyMethod);
//A task is returned that you can await
await task;
```

### Switch between Main thread and Background thread in any task
```c#

// Currently on main thread
await Awaiters.BackgroundThread;
//You should now be on a background thread

//Await one frame
await Awaiters.NextFrame;

//Currently on a background thread
await Awaiters.MainUnityThread;
//Task is now running on the Unity main thread

```

<!-- DOC-END -->

## Author

👤 **Cory Leach**

* Twitter: [@coryleach](https://twitter.com/coryleach)
* Github: [@coryleach](https://github.com/coryleach)


## Show your support

Give a ⭐️ if this project helped you!

***
_This README was generated with ❤️ by [Gameframe.Packages](https://github.com/coryleach/unitypackages)_
