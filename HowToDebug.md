Inside msbuild task use Log("asd") or base.Log.Message(...) with different priorities.

Configure your visual studio to aprooritate level of message during build process.

![http://clip2net.com/clip/m27927/1277545107-clip-18kb.png](http://clip2net.com/clip/m27927/1277545107-clip-18kb.png)

Or you can uncomment Debugger.Launch() line inside Execute() method of task. Do not forget that you should run another instance of VS using 'user launch' handler.