<h1 align="center">Welcome to com.gameframe.async 👋</h1>
<p>
  <img alt="Version" src="https://img.shields.io/badge/version-1.0.3-blue.svg?cacheSeconds=2592000" />
  <a href="https://openupm.com/packages/com.gameframe.async/"><img src="https://img.shields.io/npm/v/com.gameframe.async?label=openupm&amp;registry_uri=https://package.openupm.com" /></a>
  <a href="https://twitter.com/coryleach">
    <img alt="Twitter: coryleach" src="https://img.shields.io/twitter/follow/coryleach.svg?style=social" target="_blank" />
  </a>
</p>

> Async task utility package for Unity</br>
> Helper methods for starting tasks on the Unity thread.</br>
> Start and await coroutines from any thread.</br>

## Quick Package Install

#### Using UnityPackageManager (for Unity 2019.3 or later)
Open the package manager window (menu: Window > Package Manager)<br/>
Select "Add package from git URL...", fill in the pop-up with the following link:<br/>
https://github.com/coryleach/UnityAsync.git#1.0.3<br/>

#### Using OpenUPM

The package is available on the [openupm registry](https://openupm.com). It's recommended to install it via [openupm-cli](https://github.com/openupm/openupm-cli).

```
openupm add com.gameframe.async
```

#### Using UnityPackageManager (for Unity 2019.2 or earlier)
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

## Author

👤 **Cory Leach**

* Twitter: [@coryleach](https://twitter.com/coryleach)
* Github: [@coryleach](https://github.com/coryleach)

## Show your support

Give a ⭐️ if this project helped you!

***
_This README was generated with ❤️ by [readme-md-generator](https://github.com/kefranabg/readme-md-generator)_
