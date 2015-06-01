  * ### Right click on your project, and select 'Unload' ###
![http://clip2net.com/clip/m27927/1277504623-clip-18kb.png](http://clip2net.com/clip/m27927/1277504623-clip-18kb.png)

  * ### Right click again, and select 'Edit' ###
![http://clip2net.com/clip/m27927/1277504732-clip-16kb.png](http://clip2net.com/clip/m27927/1277504732-clip-16kb.png)

  * ### Make changes, for example, like this one at the end of file ###
```
  </ItemGroup>
  <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
  <Import Project="$(ProjectDir)..\ThirdParty\MetaCreator.targets" />
</Project>
```
(assumes, that `.\ThirdParty\MetaCreator.targets` and `.\ThirdParty\MetaCreator.dll` are exists)

  * ### Reload project again ###
![http://clip2net.com/clip/m27927/1277504900-clip-20kb.png](http://clip2net.com/clip/m27927/1277504900-clip-20kb.png)

  * ### Disable security warning ###
![http://clip2net.com/clip/m27927/1277505402-clip-22kb.png](http://clip2net.com/clip/m27927/1277505402-clip-22kb.png)<br>
If you change your csproj manually, than this message appears.<br>
This message informs your, that some third party code (and probably malicious) going to execute at compile time.<br>
If you choise highlited options, this message never appears. Decision is going to save at your <code>*.suo</code> file. if you remove your <code>*.suo</code> file (near you <code>*.sln</code>) this message appears again.<br>
<br>
<BR><br>
<br>
<br>
Another way to going on this message, is<br>
<img src='http://clip2net.com/clip/m27927/1277505896-clip-24kb.png' />