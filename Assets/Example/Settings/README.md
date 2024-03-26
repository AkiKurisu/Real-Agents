# 打包前必读

`AITurboEditorSetting`为Editor模式下使用的Setting,不会被打包出去，因此你可以在里面填写你的APIKey。

`RuntimeAISetting`为Runtime模式下使用的Setting，会被打包出去！因此请不要在里面填写APIKey和你的服务器地址，而是在游戏中通过本地存档读取。