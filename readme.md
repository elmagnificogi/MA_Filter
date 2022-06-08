# MapAssist 过滤器

A GUI drop items filter editor for MapAssist, it's easy to change to other language, now just for Chinese

主要是用来修改MapAssist的掉落过滤文件的，GUI的版本，比手动改yaml方便多了，MapAssist的自带过滤只是一个示例参考，实际没什么用。



本项目

> https://github.com/elmagnificogi/MA_Filter



MapAssist 原项目

> https://github.com/OneXDeveloper/MapAssist



MapAssist 中文版

> https://github.com/elmagnificogi/MapAssist



由于项目本身并不会改变游戏内物品名称的显示，可以配合我的内置mod一起使用。

> https://github.com/elmagnificogi/diablo2_resurrected_filter



**增加过滤流程**

![image-20220506020722397](http://img.elmagnifico.tech:9514/static/upload/elmagnifico/202205060207504.png)

**修改过滤流程**

![image-20220506020817516](http://img.elmagnifico.tech:9514/static/upload/elmagnifico/202205060208566.png)



## 视频

https://www.bilibili.com/video/BV1R44y1u79x?share_source=copy_web



## 功能

- 过滤孔数
- 过滤物品质量
- 过滤无形
- 符文过滤
- 宝石过滤
- 精华等特殊物品过滤
- 词缀过滤
- 技能过滤
- 物品种类过滤



## TODO

- 过滤文件冗余



## 代码

代码很丑，主要是MapAssist下面很多地方写的很蠢逼，为了兼容只能写得更蠢



## 使用指南

地图过滤，总的来说是用来快速筛选底材和商店物品的，简单说一下大体结构：

- 过滤是白名单，也就是符合过滤条件的物品会被提示

- 每一种物品都可以创建一个过滤
- 一个过滤中含有若干个规则
- 生成的过滤文件，需要替换原本`MapAssist下`的`ItemFilter.yaml`，并且重启`MapAssist`才能生效
- 多个`ItemFilter.yaml`可以通过合并，来将其他人的规则合并到一起，然后再导出使用



**关键原则：**

**地面掉落物，只有在未鉴定的情况能看到的属性，才能被过滤提示，鉴定以后的属性是没办法通过过滤提前知道的。**

商店物品，只有在`MapAssist下`勾选了过滤商店功能才能生效并且过滤规则中启用了商店过滤，**商店过滤需要对话显示交易框才行**

已鉴定物品，同商店物品。

- 暗金物品的词缀目前无法过滤



#### 使用示例

我以手斧为例，先选择斧类，然后选择手斧，然后添加一个新规则，启用质量过滤，选择白色和蓝色，然后点击保存，这个时候这条规则就被记录下来了。

![image-20220507145642688](http://img.elmagnifico.tech:9514/static/upload/elmagnifico/202205071456764.png)

同理，如果还是这个手斧，还能添加第二条规则，我们选择过滤白色并且是2孔或者3孔的有孔条件，点击保存。这样手斧就有了2条规则。

![image-20220507145907285](http://img.elmagnifico.tech:9514/static/upload/elmagnifico/202205071459320.png)

地图内只要掉落了手斧符合任何一条规则，就会提示给你。再也不用满地东西里捡起来看了，可以快速筛选到不错的底材。



需要用到什么过滤器，就在对应的功能上面打勾就行了，然后选择条件，填写参数即可。**技能和词条大部分都只能适用于装备天生具有这个技能或者属性（比如骑士专属盾带抗性）或者是商店已知所有属性**，大部分掉落装备是不具有的，谨慎选择。

- 指定技能时，计算的是该技能在这个装备的总和，比如天堂拳，如果过滤是3，那么3战斗技能加成也会触发天堂拳3级的过滤条件

剩下就看你对物品的熟悉程度，来应用各种条件的过滤了。当所有过滤都修改或者设置完了，那就导出就行了，会有对应的提示，按操作执行就行了。

![image-20220507160808897](http://img.elmagnifico.tech:9514/static/upload/elmagnifico/202205071608935.png)



#### 种类过滤

还是以斧类为例，首先使用物品种类过滤，这个时候就不需要选择具体哪一个底材了，然后就是物品等级是必须要选择的，否则种类过滤无法生效

其他过滤选项也都能在物品过滤中生效，按需勾选就行了

![image-20220510104251776](http://img.elmagnifico.tech:9514/static/upload/elmagnifico/image-20220510104251776.png)

- 种类过滤适合职业类装备或者某一个大类全都需要的情况下进行局部过滤



#### 商店过滤

只有当以下2条同时生效时，商店过滤才起作用，勾选了检测商店，地面掉落也是检测的

- MapAssist中勾选了过滤商店
- 过滤规则中勾选了检测商店

![image-20220608132331568](http://img.elmagnifico.tech:9514/static/upload/elmagnifico/202206081323656.png)



#### 合并

选择两个需要合并的过滤，然后打开即可

![image-20220507163815996](http://img.elmagnifico.tech:9514/static/upload/elmagnifico/202205071638040.png)

合并完成会显示对应的规则总数，接下来修改或者导出都可以

![image-20220507163849351](http://img.elmagnifico.tech:9514/static/upload/elmagnifico/202205071638387.png)



## 高级功能

比如全局过滤，局部过滤，过滤模板，这些都需要参考MapAssit的过滤示例，就是原生的那个`ItemFilter.yaml`文件，书写较为复杂，自行研究。



## 过滤测试

- 开启地图时，自己身上具有的东西是不会被过滤器识别的

先把符合过滤条件的东西（建议未鉴定的或者底材）仍在地上，然后重启地图，切回游戏，就能看到提示了。如果没有提示，建议检查过滤条件，大部分情况都是写了地面掉落无法识别到的属性（商店或者自己可以识别到）



## 快速使用

直接下别人下好的，比如群里，这都是经过验证的，替换，并且重启`MapAssist`生效，进游戏刷就是了。



## Summary

其实有了过滤以后，大部分瞎眼Mod都可以不用了，只要用一个比较干净清爽的mod即可。



## 交流

暗黑2重置交流群：941746977，edit by elmagnifico

暗黑2社区，请使用手机QQ扫码加入

> https://qun.qq.com/qqweb/qunpro/share?_wv=3&_wwv=128&appChannel=share&inviteCode=1Sfn6V&appChannel=share&businessType=9&from=246610&biz=ka

![community](http://img.elmagnifico.tech:9514/static/upload/elmagnifico/202205151608035.png)
