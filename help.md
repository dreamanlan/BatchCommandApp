<!-- @import "[TOC]" {cmd="toc" depthFrom=1 depthTo=6 orderedList=false} -->

<!-- code_chunk_output -->

- [一、语法](#一-语法)
- [二、基本用法](#二-基本用法)
- [三、GM脚本命令与函数](#三-gm脚本命令与函数)
- [四、GM脚本文件](#四-gm脚本文件)
- [五、变量](#五-变量)
- [六、启动脚本](#六-启动脚本)
- [七、调试UI](#七-调试ui)
- [八、源码位置](#八-源码位置)
- [九、api参考](#九-api参考)
  - [A、基础api---语句与异步机制](#a-基础api-语句与异步机制)
  - [B、基础api---消息与执行机制](#b-基础api-消息与执行机制)
  - [C、基础api---运算](#c-基础api-运算)
  - [D、基础api---类型转换](#d-基础api-类型转换)
  - [E、基础api---反射调用](#e-基础api-反射调用)
  - [F、基础api---字符串](#f-基础api-字符串)
  - [G、基础api---列表与哈希表](#g-基础api-列表与哈希表)
  - [H、基础api---数学](#h-基础api-数学)
  - [I、基础api---文件操作](#i-基础api-文件操作)
  - [J、基础api---其它](#j-基础api-其它)
  - [K、unity通用api---对象与组件](#k-unity通用api-对象与组件)
  - [L、unity通用api---对象空间位置](#l-unity通用api-对象空间位置)
  - [M、unity通用api---几个特殊值对象构造](#m-unity通用api-几个特殊值对象构造)
  - [N、unity通用api---随机](#n-unity通用api-随机)
  - [O、unity通用api---时间](#o-unity通用api-时间)
  - [P、unity通用api---调试等](#p-unity通用api-调试等)
  - [Q、调试UI](#q-调试ui)
  - [R、启动时设置脚本](#r-启动时设置脚本)
  - [S、游戏功能api---外部系统交互](#s-游戏功能api-外部系统交互)
  - [T、游戏功能api---wetest调用](#t-游戏功能api-wetest调用)
  - [U、游戏功能api---设备查询](#u-游戏功能api-设备查询)
  - [V、游戏功能api---场景查询](#v-游戏功能api-场景查询)
  - [W、游戏功能api---UI操作](#w-游戏功能api-ui操作)
  - [X、游戏功能api---材质参数](#x-游戏功能api-材质参数)
  - [Y、游戏功能api---PlayerPrefs](#y-游戏功能api-playerprefs)
  - [Z、游戏功能api---内存查询](#z-游戏功能api-内存查询)

<!-- /code_chunk_output -->


## 一、语法

**【一句话语法】**

***简单说就是c/c#的函数体内的语法，但是即便是语句，结尾也需要加分号，除非语句是所属块内的最后一个语句。***

## 二、基本用法

1. 入口在DebugConsole窗口（使用`按键或屏幕中上点3下打开），同时提供了从剪贴板与adb命令输入的方式

2. 安卓profiler版本默认启动会监听adb命令，release版本需要先输入
```
set_debug(1);listen_android();
```
后才开始监听

3. adb命令的发送方法
```
adb shell am broadcast -a com.unity3d.command -e cmd '命令串'
```
当命令串里有引号时，需要使用\转义，否则引号会被console去掉导致命令执行时语法错误

4. 在手机shell上执行时直接从am开始，此时命令串里的引号不需要使用转义(比如在MFQ网页的终端里输入命令)
```
am broadcast -a com.unity3d.command -e cmd '命令串'
```

5. 也支持发给指定apk，这在同时有多个接收命令的应用时用于区分发给哪一个
```
adb shell am broadcast -a com.unity3d.command -e com.DefaultCompany.Test '命令串'
```

6. 可以使用adb命令来打开DebugConsole，或者关闭（命令改为close）
```
adb shell am broadcast -a com.unity3d.command -e cmd 'open'
```

7. 剪贴板监听默认未开启，需要输入
```
set_debug(1);listen_clipboard(100);
```
后开启以每100ms一次的频率监听

8. 剪贴板命令的格式如下，在文本编辑器里输入，然后选中ctrl+c，就会触发命令执行
与adb命令类似，也有两种方式，一种是不区分应用的
```
[cmd]:命令串
```

9. 还有一种是指定特定应用的，此时使用包名作为关键字，只有这个包名对应的应用才会处理此命令
```
[com.DefaultCompany.Test]:命令串
```

10. 在安卓上通过adb命令也可以远程设置手机剪贴板内容（用于命令时游戏端需要先使用listenclipboard(100)来监听）
- 设置普通的剪贴板内容
```
adb shell am startservice -a com.unity3d.clipboard -e text '剪贴板内容'
```
- 设置剪贴板gm命令
```
adb shell am startservice -a com.unity3d.clipboard -e cmd '命令串'
```
- 设置特定应用的gm命令
```
adb shell am startservice -a com.unity3d.clipboard -e pkgcmd '包名:命令串'
```

11.  GM脚本是在DebugConsole命令的基础上扩展的，输入/?会显示DebugConsole的命令列表（比较少，除了开关DebugConsole外主要就是用来执行gm命令或脚本的命令）
	- open/close用来打开/关闭DebugConsole，没有参数
	- clear用来清空DebugConsole里的显示内容，没有参数
	- sys显示unity的一些系统信息，没有参数
	- quality level，设置QualitySetting的level，没有参数时显示当前level
	- vsync val，设置垂直同步的值，一般就1或2，没有参数时显示当前值
	- resetdsl，重置gm脚本解释器为初始状态
	- script gm_file，加载并执行gm脚本文件
	- scp gm_file，同上
	- command cmd_str，执行gm命令字符串，DebugConsole对未注册的命令（按命令串的第一个空格前的单词检索）会交给gm解释器按gm脚本执行，所以除非与DebugConsole命令名冲突，不用加command关键字来执行gm命令脚本
	- cmd cmd_str，同上
	- gm gm_str，目前等同于执行cmd gm("gm命令")，实际没有效果（未接入后端gm系统）
	- filter 过滤字符串，用来过滤DebugConsole窗口的显示内容，无参数时清空过滤字符串，通常只用于有大量输出信息的时候
	- /? 无参数时显示DebugConsole的命令列表，可以带一个字符串参数，此时显示包含这个字符串的gm命令/函数的使用说明

12.  GM脚本有一个命令cmd("命令串")，这个命令串是提交给DebugConsole来执行的，所以也能使用这里列出的各条命令

## 三、GM脚本命令与函数

1. GM脚本采用命令队列来执行，所以可以直接执行的都是命令

2. 命令没有返回值，可以有参数，参数可以是常量值或函数调用

3. GM函数有返回值，函数调用不能直接执行，必须作为命令或函数的参数

4. 命令的参数都是简单数值或变量时，命令也可以写成命令行的形式，即用空格来分隔命令名与各个参数（默认命令与函数都是写成函数调用的样式），当没有参数时，只写命令名称即可（省略括号）
	```
	log("{0} {1}",1,2)
	<=>
	log "{0} {1}" 1 2
	```

5. 较4进一步，如果命令的参数是函数调用，不能是复合语句样式的函数与运算表达式，命令也可以写成命令行的形式
	```
	log(device_info())
	<=>
	log device_info()
	----------------------------------
	log(1+2)
	则不能写成命令行样式
	log 1+2
	语法解析会解释成(log(1))+2
	----------------------------------
	假设有一个函数是复合语句样式：a(1,2,3)b(4,5,6)
	log(a(1,2,3)b(4,5,6))
	也不能写成命令行样式
	log a(1,2,3)b(4,5,6)
	语法解析会解释成log(a(1,2,3),b(4,5,6))
	```

6. 一般命令行样式只用于书写单个命令，虽然也可以使用分号来分隔多个命令行样式的命令，但看起来可能不太好理解

7. 命令可以跨tick执行，内置的wait命令（别名sleep）接受一个数值参数，用来等待指定的时间（单位是毫秒），在多个命令间插入wait命令就可以实现跨tick来执行命令序列

8. 函数不能跨tick执行，总是立即计算出返回值

9. 同c系语言一样，多个命令放在一行形成命令序列（每个命令以分号结尾，最后一个命令可以不用分号结尾）
	```
	命令1(参数表);命令2(参数表);命令3(参数表);
	```

10. 有几个复合语句性质的命令：if命令、while命令、loop命令、looplist命令、foreach命令
    - if命令，按照GM脚本语法，结尾需要加分号（如果后面没有别的命令可不加），其中elseif可以有多个，else最多有一个也可以没有。
	```
	if(条件){命令列表}elseif(条件){命令列表}else{命令列表};
	```
    - while命令，结尾需要加分号（如果后面没有别的命令可不加）
	```
	while(条件){命令列表};
	```
	- loop命令，结尾需要加分号（如果后面没有别的命令可不加），按指定循环次数循环，循环体内可以使用\$\$来访问当前循环序号（从0开始）
	```
	loop(循环次数){命令列表};
	```
	比如我们要每秒执行一次某个命令，一共执行3600次，如下：
	```
	loop(3600){log("loop {0}",$$);log_profiler();wait(1000);};
	```
	- looplist命令，用来遍历一个IList，循环体内可以使用\$\$来访问当前遍历的元素
	```
	loop_list(list变量){命令列表};
	```
	比如下面循环遍历数组并显示各数组元素(gm脚本支持数组的直接写法，实际是c#的List)：
	```
	loop_list([1,2,3,4]){log("loop_list {0}",$$);};
	```
	再比如下面循环遍历hash表并显示各对元素(gm脚本里的哈希表的key可以是常量或变量，所以字符串常量作为key时必须加引号)：
	```
	loop_list({1=>13,3=>25,9=>345}){log("pair {0}:{1}",$$.Key,$$.Value);};
	```
	- foreach命令，直接指定每次循环的变量值，循环体内可以使用\$\$来访问当前值
	```
	foreach(值1,值2,...){命令列表};
	```
	比如我们对几个奇数分别进行处理：
	```
	foreach(1,3,5,7,9){log("foreach {0}",$$);};
	```

11.  gm脚本支持dotnet反射调用，语法与c#的对象访问写法基本相同，不过在处理方法重载时可能会有问题，所以带重载的方法有可能出现调用不了的情况

12.  为了适应反射调用的类型，gm脚本添加了常用基础类型的转换函数
```
bool(val);
sbyte(val);
byte(val);
char(val);
short(val);
ushort(val);
int(val);
uint(val);
long(val);
ulong(val);
float(val);
double(val);
decimal(val);
```

13.  另外提供了几个按内存在整数与浮点重新解释的函数（类似shader里的那几个）
```
32位内存重解释：
ftoi(val);
itof(val);
ftou(val);
utof(val);
64位内存重解释：
dtol(val);
ltod(val);
dtou(val);
utod(val);
```

## 四、GM脚本文件

1. 在DebugConsole里可以使用scp gm_file的命令来执行一个GM脚本文件。如果gm_file是相对路径，在安卓系统上，gm_file的默认目录是/data/local/tmp（不是安卓系统的默认目录是Application.persistentDataPath）

2. GM脚本文件的写法如下，这部分来着我们以前的剧情脚本，大概是基于消息处理的框架，每一个消息处理就是一个命令队列。实际上在DebugConsole里输入的命令也是包装成一个脚本来执行的
```
	script(main)
	{
		onmessage("start")
		{
			//命令列表
		};
	};
```

3. GM脚本文件支持多个onmessage消息处理块，可以由start块通过localmessage命令来触发其它的消息处理，不过对GM脚本来说一般应该用不着
```
	script(main)
	{
		local
		{
			@vname1(0);
			@vname2(0);
		};
		onmessage("start")
		{
			local_message("proc1");
			local_message("proc2");
		};
		onmessage("proc1")
		{
			$lv=0;
			inc($lv);
			@vname1 = $lv;
			wait(3000);
			inc($lv);
			@vname1 = $lv;
			wait(3000);
			log("proc1 v:{0} {1}",@vname1,@vname2);
		};
		onmessage("proc2")
		{
			$lv=0;
			inc($lv);
			@vname2 = $lv;
			wait(2000);
			inc($lv);
			@vname2 = $lv;
			wait(2000);
			log("proc2 v:{0} {1}",@vname1,@vname2);
		};
	};
```

4. localmessage命令在触发消息处理时可以带参数，消息处理此时需要使用args子句来指明参数列表，也可以不指明局部变量，在消息处理里使用$0,$1,$2,…来访问，$$是参数数量
```
	script(main)
	{
		onmessage("start")
		{
			local_message("proc",123);
		};
		onmessage("proc")args($lv)
		{
			inc($lv);
			wait(2000);
			inc($lv);
			wait(2000);
			log("proc v:{0}",$lv);
		};
	};
```

5. 虽然GM脚本文件里可以有多个名称不同的script块，但我们肯定不需要写这么复杂，这部分就不说明了（我们也没有在GM脚本里提供启动其它脚本的命令，相当于禁用了这个功能）

6. 下面是一个有可能常用的gm脚本文件，在3600秒的时间里，每秒输出一次性能数据
```
	script(main)
	{
		onmessage("start")
		{
			loop(3600){
				log_profiler();
				wait(1000);
			};
		};
	};
```

7. GM脚本执行过程中，如果又执行了GM命令或加载GM脚本的命令，则正在执行的GM脚本会终止执行（也就是GM脚本解释器只能有一段脚本代码在运行），所以GM脚本的执行命令一般应该是最后一条GM命令，如果中间需要执行其他命令，则执行完后需要重新输入执行GM脚本的命令

## 五、变量

1. GM脚本没有提供词法范围的变量机制，代之以名称<=>值的映射来提供简单的变量机制

2. 在脚本里有3种级别的变量，即跨脚本的全局变量，脚本级别的局部变量，还有消息处理级别的局部变量，以名称前缀@@、@、$来区分

3. @@vname 是跨脚本的全局变量，只要不执行resetdsl命令，这些全局变量一直有效，值会保持，这可以在多次命令输入间传递数据，比如前面输入的命令查询到一个对象，可以存到全局变量里，后面输入的命令使用全局变量来访问对象

4. @vname 是脚本级别的局部变量，在脚本的local块里可以指定一个初始值（常量值），这些变量只在当前输入的命令列表间有效，在脚本文件里时是在同一个脚本内跨多个消息处理有效，对于直接输入命令来说与$开头的变量作用相同

5. $vname 是消息处理级别的局部变量，这些变量只在消息处理里有效，不能跨消息处理与脚本

6. 有一个命令propset与一个函数propget，提供了跨脚本的名称<=>值的映射，这个其实与全局变量共用一个map，不过这里直接使用字符串作为key，也能用在多次命令输入间传递数据
```
	prop_set(name, val);
	prop_get(name);
	prop_get(name, defval);
```

## 六、启动脚本

1. 启动时GM脚本配置

	- 安卓系统启动时会检查/data/local/tmp目录下是否有initgm.txt的文本文件（不是安卓系统检查Application.persistentDataPath目录下是否有initgm.txt），如果有则把initgm.txt的每一行当作一行GM脚本或DebugConsole命令进行处理，这些命令作为一个命令列表执行，所以中间执行的命令应该不能是执行命令或GM脚本的命令（因为会重置GM脚本解释器），我们可以在这个文件里配置要加载执行的GM脚本文件，然后后续就交给GM脚本文件处理了，比如下面的内容就是启动时打开调试开关（会影响GM脚本的日志输出），然后根据apk来决定加载执行不同的GM脚本文件
	```
		set_debug(1);
		if(is_android()){listen_android();};
		if(!is_android()){listen_clipboard(100);};
		if(app_id()=='DefaultCompany.Test'){cmd('scp init.dsl');};
		if(app_id()!='DefaultCompany.Test'){cmd('scp init0.dsl');};
	```

	- 对于不需要使用GM脚本文件的情形，initgm.txt里一般配置监听adb命令或剪贴板方便后续输入命令
	```
		set_debug(1);
		if(is_android()){listen_android();};
		if(!is_android()){listen_clipboard(100);};
	```

2. 启动时设置脚本配置

	- 启动时设置脚本的主要考虑是对游戏分档的实验与补充，一方面是对新增机型的分档实验，另一方面是对部分特殊机型可能需要在不同档位做一些特殊的设置。考虑到分级设置通常由性能测试发现，是一个不断迭代的过程，所以考虑提供一个脚本来方便在不打包的情况下修改设置，并避免每次启动都要重复进行设置操作

	- 启动时设置脚本不像GM脚本那样自由，主要考虑到这些脚本在实验确定后应该翻译为c#固化为启动逻辑的一部分，为了方便翻译到c#，需要尽量符合c#的风格。

	- 与GM脚本配置类似，在安卓启动时会检查/data/local/tmp目录下是否startup0.dsl - startup7.dsl，这8个文件中的任何文件，如果存在则加载执行。（不是安卓系统检查Application.persistentDataPath目录下是否有这些脚本文件）

	- 启动时设置脚本用于设置的部分分为init、grade、default_grade、setting四个部分
		- 首先会整理所有启动时设置脚本里的这些部分，归类为4个集合，然后先执行init集合里的脚本，这里一般设置根据机器model或gpu能直接确定的分档（优先级最高，匹配到分档后就不再检查后面的grade与default_grade）
		- 如果init部分没有确定分档，则开始执行grade集合里的脚本，这里是按grade值的升序执行检查（0是最高级，4是目前的最低级），一旦确定分档就不再继续执行
		- 如果所有grade仍然没能确定分档，则开始执行default_grade集合里的脚本，这里的脚本与grade类似也是按grade值升序执行检查（0是最高级，4是目前的最低级），default_grade一般是根据一些通用的指标来确定分档，也就是没那么精确，但基本上所有手机都能确定一个分档
		- 经过上面3步确定了grade后，接下来执行grade值对应的setting部分的代码
		- grade与default_grade里面的代码每一行是一个条件判断，行与行之间的条件的关系是and

	- 我们用于实验的一个startup0.dsl内容如下：（还很初级，主要针对几个实验机型，逻辑不完全）
	```
		startup(0)
		{
			initgm
			{:
				set_debug(1);
				if(is_android()){listen_android();};
				if(!is_android()){listen_clipboard(100);};
			:};
			init
			{
				log_sysinfo();
				//full model or gpu match
				add_grade("Redmi K60 Ultra", 0);
			};
			grade(0)//mali ultra
			{
				is_android();
				gpu_like("G715", "G815", "G915");
				memory_above(6000);
				gpu_memory_above(4000);
			};
			grade(0)//redmi ultra
			{
				is_android();
				device_like("K60", "K70", "K80", "K90");
				memory_above(5000);
				gpu_memory_above(4000);
			};
			grade(0)//pc
			{
				is_pc();
				device_like("HP Z2 Tower G5");
				gpu_like("RTX 3080");
				memory_above(32000);
				gpu_memory_above(6000);
			};
			grade(1)
			{
				gpu_like("G710");
			};
			grade(2)
			{
				memory_above(6000);
				gpu_like("G76",@"Adreno \(TM\) 640");
			};
			grade(3)
			{
				gpu_like(@"Adreno \(TM\) 615");
			};
			grade(4)
			{
				gpu_like("GE8320");
			};
			default_grade(0)
			{
				memory_above(12000);
				gpu_memory_above(8000);
			};
			default_grade(1)
			{
				memory_above(8000);
				gpu_memory_above(6000);
			};
			default_grade(2)
			{
				memory_above(6000);
				gpu_memory_above(5000);
			};
			default_grade(3)
			{
				memory_above(6000);
				gpu_memory_above(3000);
			};
			default_grade(4)
			{
				memory_below(6000);
				gpu_memory_below(3000);
			};
			setting(0)
			{
				set_hardware_level(4);
				set_rendering_mode(1);
				//set_resolution(1920,1080,true,1,1);
			};
			setting(1)
			{
				set_hardware_level(3);
				set_rendering_mode(1);
				//set_resolution(1920,1080,true,1,1);
			};
			setting(2)
			{
				set_hardware_level(2);
				set_rendering_mode(0);
				set_resolution(1280,720,true,1,1);
			};
			setting(3)
			{
				set_hardware_level(1);
				set_rendering_mode(0);
				set_resolution(960,540,true,1,1);
				set_mipmap(1);
			};
			setting(4)
			{
				set_hardware_level(0);
				set_rendering_mode(0);
				set_resolution(640,360,true,1,1);
				set_shader_lod(300);
				set_lod_level(1.5, 1);
				set_mipmap(2);
			};
		};
	```

	- 启动时设置脚本的api都在Assets/StartupApi/StartupApi.cs里实现，这些api不用注册，脚本解释器采用reflection来自动查找相应的api

	- 每个启动时设置脚本的api都需要二个原型，一个是脚本api方法，供脚本解释器调用，一个是api实现，供脚本翻译到的c#代码直接调用。脚本api方法一般会调用api实现来实现api功能，或者二者都调用共用的内部实现方法。api实现的名称必须是api名称加上"_impl"，参数需要与脚本里的写法匹配（下面代码里set_custom_fps是脚本api方法，set_custom_fps_impl是api实现方法）
	```
		private BoxedValue set_custom_fps(BoxedValueList list)
		{
			bool r = false;
			if (list.Count > 0)
			{
				r = set_custom_fps_impl(list[0].GetInt());
			}
			return BoxedValue.From(r);
		}
		private bool set_custom_fps_impl(int val)
		{
			QualityManager.SetCustomFPS(val);
			return true;
		}
	```

	- 启动时设置脚本的api也可以在GM脚本里调用，但是没法提供文档查询

3. 启动时设置脚本也可包含启动GM脚本配置，当同时需要时应优先使用启动时设置脚本配置启动GM代码

	- 在2中列出的启动时设置脚本包含一个initgm块，这个块就是配置的initgm.txt相同的内容
	- initgm块是使用的外部代码块的语法，使用{: :}将代码括起来，当然最后也需要加个分号
	- 除支持initgm的功能外，通过增加启动时设置脚本的api，我们也可以在init代码块里实现启动时开关调试或日志的功能

## 七、调试UI

1. 输GM命令或执行GM脚本文件的方式适用于无法预见的功能，对于一些明确的功能，固定的UI还是更方便一些，因此我们实现了一套调试UI的机制
2. 整个GM脚本系统有一个目标是方便用于单独的测试工程，所以会尽量减少对项目代码的依赖（很难不依赖，但我们尽量减少依赖），调试UI也是类似的思路，所用的控件都是原生UGUI的控件
3. 调试UI是比较简单的UI布局，我们把屏幕分成一个20行6列的表格，通过DSL来描述调试UI的布局，支持在这个表格里嵌入常见的UGUI控件：label、input、button、dropdown、toggle、toggle_group、slider
4. 下面是一个测试调试UI的布局文件内容（所有调试UI的代码都在Assets/GmCommands/UiHandler.cs中）
	- 每个控件有一个id，可以使用@auto，此时加载时会自动生成一个id，这种情形主要用于UI逻辑不需要明确引用控件的情形
	- 控件可以指定一个事件处理方法名，这个方法必须定义在UiHandler类里，事件处理不需要注册（UiHandler采用reflection来自动注册事件处理），原型需要与对应UGUI控件的事件签名一致
	```
	//For relevant C# code, see UiHandler.cs

	label(@auto, 0, 1, "test label:");
	dropdown(@auto, 0, 2, "OnValueChanged", 0){
		"0--Option0",
		"1--Option1",
		"2--Option2"
	};
	toggle(toggle1, 1, 1, "show", "OnCheckedChanged", "true");
	button(@auto, 1, 2, "Press", "OnButton");
	toggle_group(@auto, 1, 3){
		toggle(group_toggle1, "one", "OnOneChanged", "true");
		toggle(group_toggle2, "two", "OnTwoChanged", "true");
	};
	label(@auto, 2, 1, "test slider:");
	slider(slider1, 2, 2, "OnSliderChanged", 0.0, 1.0, 0.5);
	```
5. 每个调试UI都用于特定的模块或特性，这些UI是随着开发逐渐积累的，一般在每次运行时默认会加载一个最近调试的调试UI（在UiHandler.cs的初始化阶段），我们也可以通过命令来切换其它调试UI
6. 调试UI主要来自测试工程，不一定都能与游戏功能相适应，这取决于具体调试UI的开发，一般不适应的是游戏场景里缺少相关的结点（所以调试UI在开发时也要尽量考虑通用性）
7. 目前有3个GM命令用于调试UI的加载与显隐
```
	load_ui("UI资源");
	show_ui();
	hide_ui();
```
8. 调试UI的资源就是一个DSL文本文件，目前放在Assets/Resources目录下
9. GM脚本的启动脚本以及调试UI的Canvas的prefab也在这个目录下：GmScript.prefab
10.  在安卓手机上，我们可以直接输GM命令来加载显示隐藏调试UI，也可以通过adb命令来操作
```
显示当前加载的调试ui：
	adb shell am broadcast -a com.unity3d.command -e cmd 'show_ui()'
隐藏调试ui:
	adb shell am broadcast -a com.unity3d.command -e cmd 'hide_ui()'
加载调试ui（调试UI的资源文件名不包含空格时，可以不加引号，这个是在load_ui API实现时专门支持的写法）:
	adb shell am broadcast -a com.unity3d.command -e cmd 'load_ui(TestUI)'
```
## 八、源码位置

1. 我们的基础部分以dll形式放在插件目录下
```
Assets\Plugins\StoryScript.dll //基础story脚本解释器部分(基于命令队列的解释器)
Assets\Plugins\Dsl.dll //DSL语法解析部分
此目录下的其它dll是GM脚本解释器的一些内置api依赖的dll
```
2. 工程里的DebugConsole与GmScript解释器部分
```
Assets\GmCommands\ClientGmStorySystem.cs //GM脚本系统框架部分
Assets\Scripts\DebugConsole.cs //DebugConsole的功能，交互面板与简单命令处理
Assets\GmCommands\GeneralCommands.cs //通用的GM命令的实现，写法是比较复杂的原始写法，支持复合语句的语法样式（通常不用再添加）
Assets\GmCommands\GeneralFunctions.cs //通用的GM函数的实现，写法也是比较复杂的原始写法，支持复合语句的语法样式（通常不用再添加）
Assets\GmCommands\GmCommands.cs //与游戏相关的GM命令/函数的实现，这里的命令与函数一般都是函数调用样式的（一般新加命令或函数都在这里）
Assets\GmCommands\GmRootScript.cs //GM脚本的根MonoBehaviour对象与启动管理
Assets\GmCommands\Logger.cs //GM脚本的日志系统
Assets\GmCommands\StoryScriptUtility.cs //一些工具函数
Assets\GmCommands\StartupScript.cs //启动时设置脚本解释器部分，GM脚本可以直接调用启动时设置脚本的api，不过这部分api使用"/? 过滤串"时查询不到

除GM脚本外，我们实现了一套简单的调试ui工具，用DSL来描述ui，然后逻辑在c#里提供，使用gm命令来加载、显示及隐藏这些ui
Assets\GmCommands\UiHanlder.cs //调试ui的逻辑处理部分

GM脚本是一个异步执行的脚本，没办法像通常的脚本在启动时立即执行（可能滞后一帧），为了支持分级设置实验，我们实现了启动时设置脚本（启动时设置脚本支持翻译为c#代码，从而不再依赖脚本解释器），这里是api与纯c#的逻辑框架部分
Assets\StartupApi\StartupApi.cs //启动时设置脚本的api与分级设置逻辑框架
```

## 九、api参考

***更好的查阅api的办法是使用ilspy或类似工具，加载项目工程的Assembly-CSharp.dll（在Library/ScriptAssemblies目录下）与StoryScript.dll，然后搜索查看实现***

### A、基础api---语句与异步机制

- 语句列表，语句用法见前面介绍部分
```
= [foreach]:foreach(a,b,...){command_list;}; statement, iterator is $$（DSL 语句语法，非注册 API；对应 API 为 for_each_value）
= [if]:if(condition){command_list;}elseif(condition){command_list;}else{command_list;}; statement, elseif can be zero or more, else can be zero or one
= [loop]:loop(count){command_list;}; statement, iterator is $$
= [loop_list]:loop_list(list){command_list;}; statement, iterator is $$
= [while]:while(condition){command_list;}; statement
```
- 虽然我们也有break/continue/return命令，但这与传统c语言对应语句的涵义不一样，所以不要使用这些命令
- 用于实现异步效果的命令wait与sleep（二者等价），虽然不算语句，但对GM脚本的跨tick执行机制特别重要（单位是ms）
```
= [wait]:wait(ms) command
= [sleep]:sleep(ms) command
```
### B、基础api---消息与执行机制

- 执行命令的命令（如通过cmd来执行DebudConsole的命令，从而可以让DebugConsole命令与其它GM命令一起写成命令列表来执行）
```
= [cmd]:cmd(str) command，调用DebugConsole.Execute来执行命令，支持DebugConsole里可输入的所有命令，也用于将DebugConsole的命令包装成GM命令从而在GM脚本里使用
= [gm]:gm(str) command，计划用于执行后端GM指令，未实现
```
- 全局key/value写命令，这些数据只要不执行clearglobals命令或DebugConsole的resetdsl命令就一直存在
```
= [prop_set]:prop_set(name, val) command
```
- 全局key/value读函数
```
= [prop_get]:prop_get(name[,defval]) function
```
- ~~应该不会用到的命令，请勿使用~~
```
= [clear_globals]:clear_globals() command
= [clear_message]:clear_message(msgid1,msgid2,...) command
= [clear_namespaced_message]:clear_namespaced_message(msgid1,msgid2,...) command

= [local_concurrent_message]:local_concurrent_message(msgid,arg1,arg2,...) command
= [local_concurrent_namespaced_message]:local_concurrent_namespaced_message(msgid,arg1,arg2,...) command
= [local_message]:local_message(msgid,arg1,arg2,...) command
= [local_namespaced_message]:local_namespaced_message(msgid,arg1,arg2,...) command

= [resume_local_message_handler]:resume_local_message_handler(msgid1,msgid2,...) command
= [resume_local_namespaced_message_handler]:resume_local_namespaced_message_handler(msgid1,msgid2,...) command

= [suspend]:suspend command
= [suspend_local_message_handler]:suspend_local_message_handler(msgid1,msgid2,...) command
= [suspend_local_namespaced_message_handler]:suspend_local_namespaced_message_handler(msgid1,msgid2,...) command

= [wait_local_message]:wait_local_message(msgid1,msgid2,...)[set(var,val)timeoutset(timeout,var,val)] command
= [wait_local_message_handler]:wait_local_message_handler(msgid1,msgid2,...)[set(var,val)timeoutset(timeout,var,val)] command
= [wait_local_namespaced_message]:wait_local_namespaced_message(msgid1,msgid2,...)[set(var,val)timeoutset(timeout,var,val)] command
= [wait_local_namespaced_message_handler]:wait_local_namespaced_message_handler(msgid1,msgid2,...)[set(var,val)timeoutset(timeout,var,val)] command

= [pause]:pause command
= [terminate]:terminate command

= [clearcmdsubsts]:clearcmdsubsts() command（旧版内建，当前 StoryScript.dll 已移除，无对应注册）
= [clearfuncsubsts]:clearfuncsubsts() command（旧版内建，当前 StoryScript.dll 已移除，无对应注册）
= [substcmd]:substcmd(id,substId) command（旧版内建，当前 StoryScript.dll 已移除，无对应注册）
= [substfunc]:substfunc(id,substId) command（旧版内建，当前 StoryScript.dll 已移除，无对应注册）
```
- ~~应该不会用到的函数，请勿使用~~（以上 4 个及下方 4 个 subst 系均为旧版残留，当前运行时调用会报 unknown func）
```
= [countcommand]:countcommand(level) function（旧版内建，已移除）
= [counthandlercommand]:counthandlercommand() function（旧版内建，已移除）
= [namespace]:namespace() function
= [story_id]:story_id() function
= [message_id]:message_id() function

= [getcmdsubst]:getcmdsubst(id) function（旧版内建，已移除）
= [getfuncsubst]:getfuncsubst(id) function（旧版内建，已移除）
```
### C、基础api---运算

- 命令类
```
= [assign]:assign(var, val) command，赋值操作符的函数样式写法（DSL 语句语法，非注册 API）
= [inc]:inc(var, val) command，增加一个变量的值，我们没有实现++运算符，用这个命令代替
= [dec]:dec(var, val) command，减少一个变量的值，我们没有实现--运算符，用这个命令代替

赋值操作符是一个命令，所以赋值是可以直接执行的（除赋值操作符外其它操作符都是函数，所以只能作为命令或函数的参数）
= [=]:assignment operator
```
- 函数类
```
这些操作符与c/c#的相应操作符功能相同
= [-]:sub operator
= [!]:not operator
= [!=]:not equal operator
= [%]:mod operator
= [&]:bitand operator
= [&&]:and operator
= [*]:mul operator
= [/]:div operator
= [^]:bitxor operator
= [|]:bitor operator
= [||]:or operator
= [~]:bitnot operator
= [+]:add operator
= [<]:less operator
= [<<]:left shift operator
= [<=]:less equal operator
= [==]:equal operator
= [>]:great operator
= [>=]:great equal operator
= [>>]:right shift operator
```
### D、基础api---类型转换

- 类型转换api全部是函数类
```
c#类型转换语义下的类型转换
= [bool]:bool(v) function
= [byte]:byte(v) function
= [char]:char(v) function
= [decimal]:decimal(v) function
= [double]:double(v) function
= [float]:float(v) function
= [int]:int(v) function
= [long]:long(v) function
= [sbyte]:sbyte(v) function
= [short]:short(v) function
= [uint]:uint(v) function
= [ulong]:ulong(v) function
= [ushort]:ushort(v) function
内存重新新解释（f表示32位float，d表示64位double，i表示32位整数，l表示64位整数，u表示无符号整数，位数与对应的转换源或目的操作数相同）
= [dtol]:dtol(v) function
= [dtou]:dtou(v) function
= [ftoi]:ftoi(v) function
= [ftou]:ftou(v) function
= [itof]:itof(v) function
= [ltod]:ltod(v) function
= [utod]:utod(v) function
= [utof]:utof(v) function
```
### E、基础api---反射调用

- 反射调用在脚本里可以采用对象写法，解释器加载代码时会转换为对相应api的调用。

- 命令类
```
= [collectioncall]:collectioncall command, internal implementation, using csharp object syntax
= [collectionset]:collectionset command, internal implementation, using csharp object syntax
= [dotnetcall]:dotnetcall command, internal implementation, using csharp object syntax
= [dotnetset]:dotnetset command, internal implementation, using csharp object syntax
```
- 函数类，如果类型名称不是已有api名称，解释器会尝试用typeof来获取类型Type对象，名字空间也采用类似处理（需要在首次使用时收集所有加载的assembly的类型的名字空间，首次执行会比较慢），使用import引入或使用typeof来明确指定包含名字空间的类型字符串会更快一些
```
= [collectioncall]:collectioncall function, internal implementation, using csharp object syntax
= [collectionget]:collectionget function, internal implementation, using csharp object syntax
= [dotnetcall]:dotnetcall function, internal implementation, using csharp object syntax
= [dotnetget]:dotnetget function, internal implementation, using csharp object syntax
```
- 反射工具函数，主要用于类型变换或根据字符串获取Type对象
```
= [change_type]:change_type(obj,type_obj_or_str) function，类似Convert.ChangeType，另外考虑了BoxedValue与object继承情形的转型
= [get_type]:get_type(type_name_str) function，Type.GetType(typeName)
= [get_unity_type]:get_unity_type(type_str) function，Type.GetType($"UnityEngine.{typeName},UnityEngine")
= [get_unity_ui_type]:get_unity_ui_type(type_str) function，Type.GetType($"UnityEngine.UI.{typeName},UnityEngine.UI")
= [get_user_type]:get_user_type(type_str) function，Type.GetType($"{typeName},Assembly-CSharp")

= [import_namespace]:import_namespace(ns,assembly) command, import a namespace for typeof
= [un_import_namespace]:un_import_namespace(ns,assembly) command, remove the previously imported namespace
= [typeof]:typeof(type) or typeof(type,assembly) function, Type.GetType, Automatically try get_unity_type/get_unity_ui_type/get_user_type, then try imports, and finally try get_type

= [get_type_assembly_name]:get_type_assembly_name(obj) function，get Type.AssemblyQualifiedName
= [get_type_full_name]:get_type_full_name(obj) function，get Type.FullName
= [get_type_name]:get_type_name(obj) function，get Type.Name
= [parse_enum]:parse_enum(type_obj_or_str,enum_val) function，Enum.Parse

= [linq]:linq(obj,method,arg1,arg2,...) function, internal implementation, using obj.method(arg1,arg2,...) syntax, method can be orderby/orderbydesc/where/top, iterator is $$
```
### F、基础api---字符串

- 命令类
```
= [append_format]:append_format(sb,fmt,arg1,arg2,...) command
= [append_format_line]:append_format_line(sb,fmt,arg1,arg2,...) command
```
- 函数类
```
= [hex_2_int]:hex_2_int(str) function
= [hex_2_long]:hex_2_long(str) function
= [hex_2_uint]:hex_2_uint(str) function
= [hex_2_ulong]:hex_2_ulong(str) function
= [str_2_double]:str_2_double(str) function
= [str_2_long]:str_2_long(str) function
= [str_2_uint]:str_2_uint(str) function
= [str_2_ulong]:str_2_ulong(str) function
= [str_2_float]:str_2_float(str) function
= [str_2_int]:str_2_int(str) function

= [datetime_str]:datetime_str(fmt) function
= [short_date_str]:short_date_str() function
= [short_time_str]:short_time_str() function
= [long_date_str]:long_date_str() function
= [long_time_str]:long_time_str() function

= [is_null_or_empty]:is_null_or_empty(str) function
= [make_string]:make_string(char1_as_str_or_int,char2_as_str_or_int,...) function
= [new_string_builder]:new_string_builder() function
= [string_join]:string_join(sep,list) function
= [string_replace]:string_replace(str,key,rep_str) function
= [string_replace_char]:string_replace_char(str,key,char_as_str) function
= [string_split]:string_split(str,sep_list) function
= [string_trim]:string_trim(str) function
= [string_trim_end]:string_trim_end(str) function
= [string_trim_start]:string_trim_start(str) function

= [string_to_lower]:string_to_lower(str) function, use cache
= [string_to_upper]:string_to_upper(str) function, use cache
= [string_contains]:string_contains(str,str1,str2,...) function
= [string_contains_any]:string_contains_any(str,str1,str2,...) function
= [string_list]:string_list(str_split_by_sep) function
= [string_not_contains]:string_not_contains(str,str1,str2,...) function
= [string_not_contains_any]:string_not_contains_any(str,str1,str2,...) function
= [string_to_lower]:string_to_lower(str) function
= [string_to_upper]:string_to_upper(str) function
= [substring]:substring(str[,start,len]) function

= [format]:format(fmt[,arg1,...]) function
```
### G、基础api---列表与哈希表

- 命令类
```
= [hashtable_add]:hashtable_add(hashtable,key,val) command
= [hashtable_clear]:hashtable_clear(hashtable) command
= [hashtable_remove]:hashtable_remove(hashtable,key) command
= [hashtable_set]:hashtable_set(hashtable,key,val) command

= [list_add]:list_add(list,value) command
= [list_clear]:list_clear(list) command
= [listen_android]:listen_android() command
= [listen_clipboard]:listen_clipboard(interval) command
= [list_insert]:list_insert(list,index,value) command
= [list_remove]:list_remove(list,value) command
= [list_remove_at]:list_remove_at(list,index) command
= [list_set]:list_set(list,index,value) command
```
- 函数类
```
= [hashtable]:hashtable(k1=>v1,k2=>v2,...) function
= [hashtable_get]:hashtable_get(hash_obj,key[,defval]) function
= [hashtable_keys]:hashtable_keys(hash_obj) function
= [hashtable_size]:hashtable_size(hash_obj) function
= [hashtable_values]:hashtable_values(hash_obj) function

= [list]:list(v1,v2,...) function
= [list_get]:list_get(list,index[,defval]) function
= [list_index_of]:list_index_of(list,val) function
= [list_size]:list_size(list) function

= [array]:array(v1,v2,...) function
= [to_array]:to_array(list) function

= [float_list]:float_list(str_split_by_sep) function
= [int_list]:int_list(str_split_by_sep) function
= [vector_2_list]:vector_2_list(str_split_by_sep) function, vector2 per 2 elements
= [vector_3_list]:vector_3_list(str_split_by_sep) function, vector3 per 3 elements
```
### H、基础api---数学

- 所有数学api都是函数
```
= [abs]:abs(val) function
= [acos]:acos(v) function
= [approximately]:approximately(v1,v2) function
= [asin]:asin(v) function
= [atan]:atan(v) function
= [atan_2]:atan_2(v1,v2) function
= [ceiling]:ceiling(val) function
= [ceiling_to_int]:ceiling_to_int(v) function
= [clamp]:clamp(v,v1,v2) function
= [clamp_01]:clamp_01(v) function
= [closest_power_of_two]:closest_power_of_two(v) function
= [cos]:cos(val) function
= [cosh]:cosh(val) function
= [deg2rad]:deg2rad(deg) function
= [rad2deg]:rad2deg(rad) function
= [dist]:dist(x1,y1,x2,y2) function
= [dist_sqr]:dist_sqr(x1,y1,x2,y2) function
= [exp]:exp(v) function
= [exp_2]:exp_2(v) function
= [floor]:floor(val) function
= [floor_to_int]:floor_to_int(v) function
= [is_power_of_two]:is_power_of_two(v) function
= [lerp]:lerp(a,b,t) function
= [lerp_angle]:lerp_angle(a,b,t) function
= [lerp_unclamped]:lerp_unclamped(a,b,t) function
= [log]:log(x) or log(x,y) function
= [log_10]:log_10(v) function
= [log_2]:log_2(v) function
= [max]:max(v1,v2,...) function
= [min]:min(v1,v2,...) function
= [next_power_of_two]:next_power_of_two(v) function
= [pow]:pow(x) or pow(x,y) function
= [exp]:exp(x) function
= [round]:round(val) function
= [round_to_int]:round_to_int(v) function
= [smooth_step]:smooth_step(from,to,t) function
= [sin]:sin(val) function
= [sinh]:sinh(val) function
= [sqrt]:sqrt(val) function
= [tan]:tan(v) function
= [tanh]:tanh(v) function

= [vector_2_dist]:vector_2_dist(pt1,pt2) function
= [vector_3_dist]:vector_3_dist(pt1,pt2) function
```
### I、基础api---文件操作

- 命令类
```
= [write_all_lines]:write_all_lines(file,lines) command
= [write_all_text]:write_all_text(file,txt) command
```
- 函数类
```
= [read_all_lines]:read_all_lines(file) function
= [read_all_text]:read_all_text(file) function
= [combine_path]:combine_path(path1,path2) function
= [get_directory_name]:get_directory_name(path) function
= [get_extension]:get_extension(path) function
= [get_file_name]:get_file_name(path) function
```
### J、基础api---其它

- 命令类
```
= [help]:help() command，执行后打开浏览器并跳转到本文档
= [log]:log(fmt,args) command，使用warning日志输出到logcat同时输出到DebugConsole，格式化串与C#的string.Format相同
```
- 函数类
```
= [set_debug]:set_debug(1_or_0) command

= [eval]:eval(exp1,exp2,...) function
= [time]:time() function

= [from_json]:from_json(json_str) function
= [to_json]:to_json(obj) function
```
### K、unity通用api---对象与组件

- 命令类
```
= [add_component]:add_component(obj_or_path,type_or_str)[obj("varname")] command
= [remove_component]:remove_component(obj_or_path,type_or_str) command

= [add_transform]:add_transform(name,local0_or_world1){position(vector_3(x,y,z));rotation(vector_3(x,y,z));scale(vector_3(x,y,z));} command

= [create_game_object]:create_game_object(name,prefab[,parent])[obj("varname")]{position(vector_3(x,y,z));rotation(vector_3(x,y,z));scale(vector_3(x,y,z));loadtimeout(1000);disable("typename", "typename", ...);remove("typename", "typename", ...);} command
= [destroy_game_object]:destroy_game_object(path) command

= [game_object_animation]:game_object_animation(obj, anim[, normalized_time]) command
= [game_object_animation_param]:game_object_animation_param(obj){float(name,val);int(name,val);bool(name,val);trigger(name,val);} command

= [set_parent]:set_parent(obj_or_path,parent,int_stay_world_pos) command
= [set_active]:set_active(obj_or_path,1_or_0) command
= [set_visible]:set_visible(obj_or_path,1_or_0) command

= [send_message]:send_message(objname,msg,arg1,arg2,...) command
= [send_message_with_game_object]:send_message_with_game_object(gameobject,msg,arg1,arg2,...) command
= [send_message_with_tag]:send_message_with_tag(tagname,msg,arg1,arg2,...) command
```
- 函数类
```
= [equals_null]:equals_null(obj) function
= [is_null]:is_null(obj) function
= [null]:null() function

= [get_component]:get_component(obj_or_path,type_or_str) function
= [get_component_in_children]:get_component_in_children(obj_or_path,type_or_str[,int_inc_inactive]) function
= [get_component_in_parent]:get_component_in_parent(obj_or_path,type_or_str[,int_inc_inactive]) function
= [get_components]:get_components(obj_or_path,type_or_str) function
= [get_components_in_children]:get_components_in_children(obj_or_path,type_or_str[,int_inc_inactive]) function
= [get_components_in_parent]:get_components_in_parent(obj_or_path,type_or_str[,int_inc_inactive]) function
= [get_game_object]:get_game_object(obj_or_path) or get_game_object(obj_or_path){disable(comp1,comp2,...);remove(comp1,comp2,...);} function
= [get_parent]:get_parent(obj_or_path) function
= [get_child]:get_child(obj_or_path,child_path) function

= [is_active]:is_active(obj_or_path) function
= [is_really_active]:is_really_active(obj_or_path) function
= [is_visible]:is_visible(obj_or_path) function
```
### L、unity通用api---对象空间位置

- 命令类
```
= [set_transform]:set_transform(name,local0_or_world1){position(vector_3(x,y,z));rotation(vector_3(x,y,z));scale(vector_3(x,y,z));} command
```
- 函数类
```
= [get_position]:get_position(obj_or_path[,local0_or_world1]) function
= [get_position_x]:get_position_x(obj_or_path[,local0_or_world1]) function
= [get_position_y]:get_position_y(obj_or_path[,local0_or_world1]) function
= [get_position_z]:get_position_z(obj_or_path[,local0_or_world1]) function
= [get_rotation]:get_rotation(obj_or_path[,local0_or_world1]) function
= [get_rotation_x]:get_rotation_x(obj_or_path[,local0_or_world1]) function
= [get_rotation_y]:get_rotation_y(obj_or_path[,local0_or_world1]) function
= [get_rotation_z]:get_rotation_z(obj_or_path[,local0_or_world1]) function
= [get_scale]:get_scale(obj_or_path) function
= [get_scale_x]:get_scale_x(obj_or_path) function
= [get_scale_y]:get_scale_y(obj_or_path) function
= [get_scale_z]:get_scale_z(obj_or_path) function
```
### M、unity通用api---几个特殊值对象构造

- 函数类
```
= [color]:color(r,g,b,a) function
= [color_32]:color_32(r,g,b,a) function
= [eular]:eular(x,y,z) function
= [position]:position(x,y,z) function
= [quaternion]:quaternion(x,y,z,w) function
= [rotation]:rotation(x,y,z) function
= [scale]:scale(x,y,z) function
= [vector_2]:vector_2(x,y) function
= [vector_2_int]:vector_2_int(x,y) function
= [vector_2_to_3]:vector_2_to_3(pt) function
= [vector_3]:vector_3(x,y,z) function
= [vector_3_int]:vector_3_int(x,y,z) function
= [vector_3_to_2]:vector_3_to_2(pt) function
= [vector_4]:vector_4(x,y,z,w) function
```
### N、unity通用api---随机

- 函数类
```
= [rnd_float]:rnd_float() function
= [rnd_from_list]:rnd_from_list(list[,defval]) function
= [rnd_int]:rnd_int(min,max) function
= [rnd_vector_2]:rnd_vector_2(pt,radius) function
= [rnd_vector_3]:rnd_vector_3(pt,radius) function
```
### O、unity通用api---时间

- 命令类
```
= [set_time_scale]:set_time_scale(scale) command
```
- 函数类
```
= [get_time]:get_time() function
= [get_time_scale]:get_time_scale() function
```
### P、unity通用api---调试等

- 调试
```
= [log_code_num]:log_code_num() command，输出代码里的一个整数，用来确认版本是否符合，打包时先更新此值，运行时对比

= [debug_break]:debug_break() command，Debug.DebugBreak
= [editor_break]:editor_break() command，Debug.Break
```
- 打开UI
```
= [open_url]:open_url(url) command
```
- 退出
```
= [quit]:quit() command
```
### Q、调试UI

- 命令类
```
= [load_ui]:load_ui(ui_name_dsl) command，加载指定的调试ui
= [show_ui]:show_ui() command，显示当前加载的调试ui
= [hide_ui]:hide_ui() command，隐藏当前调试ui
```
### R、启动时设置脚本

- 命令类
```
= [compilestartup]:compilestartup(startup_dsl_file) command，将启动时设置脚本翻译到C#代码（旧版内建，当前已移除，无对应注册）
= [reload_startups]:reload_startups(rungm) command，重新执行全部启动时设置脚本（/data/local/tmp/startup*.dsl）
= [run_startup]:run_startup(startup_dsl_file,rungm) command，运行指定的启动时设置脚本
= [logcstartups]:logcstartups() command，打印编译好的启动时设置脚本id（旧版内建，当前已移除，无对应注册）
```
### S、游戏功能api---外部系统交互

- 命令类
```
= [shell]:shell(cmd) command，异步执行系统命令
= [shell_timeout]:shell_timeout(cmd,ms) command，异步执行系统命令，带超时时间
= [cleanup_completed_tasks]:cleanup_completed_tasks() command，清理底层用于异步执行命令的Task
= [use_java_task]:use_java_task(is_java) command，设置是否使用java任务机制，默认使用c#的Task

= [start_activity]:start_activity(package_name[[,class_name[,flags]],extra_list_or_dict]) command，启动一个activity

目前GM脚本系统带了一个安卓插件（接收adb命令也是通过这个插件），里面有一个用来重启应用的activity，可如下使用
start_activity("com.DefaultCompany.Test","com.unity3d.broadcastlib.RestartActivity",0,["package","com.DefaultCompany.Test","class","com.unity3d.player.UnityPlayerActivity","flags",0]);
或
start_activity("com.DefaultCompany.Test","com.unity3d.broadcastlib.RestartActivity",0,{"package":"com.DefaultCompany.Test","class":"com.unity3d.player.UnityPlayerActivity","flags":0});

常用的flags:
	0x00008000 Intent.FLAG_ACTIVITY_CLEAR_TASK
	0x04000000 Intent.FLAG_ACTIVITY_CLEAR_TOP
	0x08000000 Intent.FLAG_ACTIVITY_MULTIPLE_TASK
	0x10000000 Intent.FLAG_ACTIVITY_NEW_TASK
	0x20000000 Intent.FLAG_ACTIVITY_SINGLE_TOP

= [finish_activity]:finish_activity() command，停止当前activity，相当于退出应用

= [start_service]:start_service(srv_class, extra_name, extra_val) command，启动一个service
= [stop_service]:stop_service(srv_class) command，停止一个service

= [set_clipboard]:set_clipboard(text) command，写系统剪贴板
```
- 函数类
```
= [shell]:shell(cmd) function, return string，同步执行系统命令
= [shell_timeout]:shell_timeout(cmd,ms) function, return string，同步执行系统命令，带超时时间
= [is_java_task]:is_java_task() function, return int，是否使用的java任务机制
= [get_task_count]:get_task_count() function, return int，当前Task数量

= [get_clipboard]:get_clipboard() function, get system clipboard content，读系统剪贴板

获取unity apk启动参数的函数（使用adb命令来传递参数）
= [get_bool]:get_bool(str) function
= [get_bool_array]:get_bool_array(str) function
= [get_byte]:get_byte(str) function
= [get_byte_array]:get_byte_array(str) function
= [get_char]:get_char(str) function
= [get_char_array]:get_char_array(str) function
= [get_double]:get_double(str) function
= [get_double_array]:get_double_array(str) function
= [get_float]:get_float(str) function
= [get_float_array]:get_float_array(str) function
= [get_int]:get_int(str) function
= [get_int_array]:get_int_array(str) function
= [get_long]:get_long(str) function
= [get_long_array]:get_long_array(str) function
= [get_short]:get_short(str) function
= [get_short_array]:get_short_array(str) function
= [get_string]:get_string(str) function
= [get_string_array]:get_string_array(str) function

= [get_activity_class_name]:get_activity_class_name() function
```
### T、游戏功能api---wetest调用

- 命令类
```
= [wetest_touch]:wetest_touch(action,x,y) command, simulate touch event with WeTest
```
- 函数类
```
= [wetest_get_height]:wetest_get_height() function, WeTest GetHeight
= [wetest_get_width]:wetest_get_width() function, WeTest GetWidth
= [wetest_get_x]:wetest_get_x() function, WeTest GetX
= [wetest_get_y]:wetest_get_y() function, WeTest GetY
```
### U、游戏功能api---设备查询

- 命令类
```
= [log_profiler]:log_profiler() command，输出性能数据日志（主要是内存信息）
= [log_resolutions]:log_resolutions() command, print supported resolutions

= [device_supports]:device_supports() command, print unsupported feature
= [supports_gfx_format]:supports_gfx_format() command, print unsupported graphics format
= [supports_rt]:supports_rt() command, print unsupported rt
= [supports_tex]:supports_tex() command, print unsupported tex
= [supports_vertex_attribute_format]:supports_vertex_attribute_format() command, print unsupported vertex attribute format
```

- 函数类
```
= [app]:app() function, return typeof(Application)
= [app_id]:app_id() function, return Application.identifier
= [app_name]:app_name() function, return Application.productName

= [device_info]:device_info() function, get device name/model and gpu model

= [get_compatible_format]:get_compatible_format(fmt_str,usage_str) function
= [get_graphics_format]:get_graphics_format(def_fmt) function
= [get_msaa_sample_count]:get_msaa_sample_count(w,h,color_fmt,depth_bit,mip_ct) function
= [is_format_supported]:is_format_supported(fmt_str,usage_str) function

= [get_persistent_path]:get_persistent_path() function
= [get_streaming_assets]:get_streaming_assets() function

= [is_android]:is_android() function, return Application.platform==Android
= [is_console]:is_console() function, return Application.isConsolePlatform
= [is_debug]:is_debug() function
= [is_development]:is_development() function
= [is_editor]:is_editor() function, return Application.isEditor
= [is_iphone]:is_iphone() function, return Application.platform==IPhone
= [is_mobile]:is_mobile() function, return Application.isMobilePlatform
= [is_pc]:is_pc() function, return not mobile and not console
= [platform]:platform() function, return Application.platform

= [system_info]:system_info() function, return typeof(SystemInfo)
= [shader]:shader() function, return typeof(Shader)
= [screen]:screen() function, return typeof(Screen)

= [screen_dpi]:screen_dpi() function, return Screen.dpi
= [screen_height]:screen_height() function, return Screen.height
= [screen_width]:screen_width() function, return Screen.width
```

### V、游戏功能api---场景查询

- 命令类
```
= [log_objects]:log_objects(name_key,type) command
= [log_components]:log_components(root_name,[name1,name2,...],type,up_level,include_inactive) command，其中name1,name2,...用来匹配组件所在对象的场景路径（与下一部分UI操作里相似）
= [log_scene_path]:log_scene_path([prefixs],obj,up_level) command
```
- 函数类
```
= [find_object]:find_object(name_key,type) function
= [search_objects]:search_objects(name_key,type) function
= [find_component]:find_component(root_name,[name1,name2,...],type,include_inactive) function，其中name1,name2,...用来匹配组件所在对象的场景路径（与下一部分UI操作里相似）
= [search_components]:search_components(root_name,[name1,name2,...],type,include_inactive) function，其中name1,name2,...用来匹配组件所在对象的场景路径（与下一部分UI操作里相似）
= [get_scene_path]:get_scene_path([prefixs],obj,up_level) function, return partial scene path
```
### W、游戏功能api---UI操作

**有时候使用MFQ或安卓投屏软件远程操作手机时，有些手机的远程屏幕操作会失效，此时可以使用UI操作类命令与函数来部分代替UI操作**

- 命令类
```
= [click]:click(uiobj) command，触发指定Button的点击事件
= [click_on_pos]:click_on_pos(x,y) command，触发指定屏幕坐标位置的Button的点击事件
= [click_on_pointer]:click_on_pointer() command，触发在鼠标当前位置的点击事件处理（用于Button控件点击）
= [click_ui]:click_ui(name1,name2,...) command，搜索场景路径上包含name1,name2,...的Button控件（name完全匹配某一层级名字，各name不需要是连续层次的名字，只要路径上名称顺序符合，中间可以跳过一些层级的名字），并触发其点击事件

= [toggle]:toggle(uiobj) command，切换指定Toggle的选中状态
= [toggle_on]:toggle_on(name1,name2,...) command，搜索场景路径上包含name1,name2,...的Toggle控件（name完全匹配某一层级名字，各name不需要是连续层次的名字，只要路径上名称顺序符合，中间可以跳过一些层级的名字），并切换其选中状态
= [toggle_on_pos]:toggle_on_pos(x,y) command，切换在指定屏幕坐标位置的Toggle的选中状态
= [toggle_on_pointer]:toggle_on_pointer() command，切换在鼠标当前位置的Toggle的选中状态
```
- 函数类（其中name1,name2,...用来匹配组件所在对象的场景路径，与命令部分相似）
```
= [find_ui_button]:find_ui_button(name1,name2,...) function
= [find_ui_dropdown]:find_ui_dropdown(name1,name2,...) function
= [find_ui_input]:find_ui_input(name1,name2,...) function
= [find_raw_image]:find_raw_image(name1,name2,...) function
= [find_ui_slider]:find_ui_slider(name1,name2,...) function
= [find_tmp_dropdown]:find_tmp_dropdown(name1,name2,...) function
= [find_tmp_input]:find_tmp_input(name1,name2,...) function
= [find_ui_toggle]:find_ui_toggle(name1,name2,...) function
= [find_ui_image]:find_ui_image(name1,name2,...) function

= [get_pointer]:get_pointer() function, return Input.mousePosition
= [get_pointer_components]:get_pointer_components(type,include_inactive) function, return List<Component>，向鼠标位置打射线，返回命中的指定类型的组件列表
= [pointer_raycast_uis]:pointer_raycast_uis() function, return List<UnityEngine.EventSystems.RaycastResult>，向鼠标位置打射线，返回命中的ui控件信息

= [raycast_components]:raycast_components(x,y,type,include_inactive) function, return List<Component>，向指定屏幕位置打射线，返回命中的指定类型的组件列表
= [raycast_uis]:raycast_uis(x,y) function, return List<UnityEngine.EventSystems.RaycastResult>，向指定屏幕位置打射线，返回命中的ui控件信息
```

### X、游戏功能api---材质参数

- 命令类
```
= [log_mesh]:log_mesh(mesh_or_path) command

= [set_material]:set_material(renderer_or_path,mat_or_path) or set_material(renderer_or_path,ix,mat_or_path) or set_material(renderer_or_path,ix,mats_or_path,ix) command
= [set_materials]:set_materials(mesh_or_path,mats_or_path) command
= [set_shared_material]:set_shared_material(renderer_or_path,mat_or_path) or set_shared_material(renderer_or_path,ix,mat_or_path) or set_shared_material(renderer_or_path,ix,mats_or_path,ix) command
= [set_shared_materials]:set_shared_materials(mesh_or_path,mats_or_path) command

= [material_set_color]:material_set_color(mat_or_path,key,val) command, Material.SetColor
= [material_set_float]:material_set_float(mat_or_path,key,val) command, Material.SetFloat
= [material_set_int]:material_set_int(mat_or_path,key,val) command, Material.SetInt
= [material_set_integer]:material_set_integer(mat_or_path,key,val) command, Material.SetInteger
= [material_set_vector]:material_set_vector(mat_or_path,key,val) command, Material.SetVector
```
- 函数类
```
= [get_material]:get_material(mat_or_path[,index]) function, renderer.material[s[ix]]
= [get_shared_material]:get_shared_material(mat_or_path[,index]) function, renderer.sharedMaterial[s[ix]]

= [material_get_color]:material_get_color(mat_or_path,key) function, Material.GetColor
= [material_get_float]:material_get_float(mat_or_path,key) function, Material.GetFloat
= [material_get_int]:material_get_int(mat_or_path,key) function, Material.GetInt
= [material_get_integer]:material_get_integer(mat_or_path,key) function, Material.GetInteger
= [material_get_vector]:material_get_vector(mat_or_path,key) function, Material.GetVector
```
### Y、游戏功能api---PlayerPrefs

- 命令类
```
= [player_pref_set_by_java]:player_pref_set_by_java(key,val) command
= [player_pref_set_float]:player_pref_set_float(key,val) command
= [player_pref_set_int]:player_pref_set_int(key,val) command
= [player_pref_set_string]:player_pref_set_string(key,val) command
```
- 函数类
```
= [player_pref_get_by_java]:player_pref_get_by_java(key,defval) function
= [player_pref_get_float]:player_pref_get_float(key,defval) function
= [player_pref_get_int]:player_pref_get_int(key,defval) function
= [player_pref_get_string]:player_pref_get_string(key,defval) function
```
### Z、游戏功能api---内存查询

- 命令类
```
= [alloc_memory]:alloc_memory(key,size) command
= [free_memory]:free_memory(key) command
= [gc]:gc() command, force Garbage Collect
= [consume_cpu]:consume_cpu(time) command
```
- 函数类
```
= [gfx]:gfx() function, get used gfx memory
= [mono]:mono() function, get total mono memory
= [native]:native() function, get used native memory
= [total]:total() function, get total native memory
= [unused]:unused() function, get unused reserved native memory
```


<!-- @import "[TOC]" {cmd="toc" depthFrom=1 depthTo=6 orderedList=false} -->

<!-- code_chunk_output -->

- [一、语法](#一-语法)
- [二、基本用法](#二-基本用法)
- [三、GM脚本命令与函数](#三-gm脚本命令与函数)
- [四、GM脚本文件](#四-gm脚本文件)
- [五、变量](#五-变量)
- [六、启动脚本](#六-启动脚本)
- [七、调试UI](#七-调试ui)
- [八、源码位置](#八-源码位置)
- [九、api参考](#九-api参考)
  - [A、基础api---语句与异步机制](#a-基础api-语句与异步机制)
  - [B、基础api---消息与执行机制](#b-基础api-消息与执行机制)
  - [C、基础api---运算](#c-基础api-运算)
  - [D、基础api---类型转换](#d-基础api-类型转换)
  - [E、基础api---反射调用](#e-基础api-反射调用)
  - [F、基础api---字符串](#f-基础api-字符串)
  - [G、基础api---列表与哈希表](#g-基础api-列表与哈希表)
  - [H、基础api---数学](#h-基础api-数学)
  - [I、基础api---文件操作](#i-基础api-文件操作)
  - [J、基础api---其它](#j-基础api-其它)
  - [K、unity通用api---对象与组件](#k-unity通用api-对象与组件)
  - [L、unity通用api---对象空间位置](#l-unity通用api-对象空间位置)
  - [M、unity通用api---几个特殊值对象构造](#m-unity通用api-几个特殊值对象构造)
  - [N、unity通用api---随机](#n-unity通用api-随机)
  - [O、unity通用api---时间](#o-unity通用api-时间)
  - [P、unity通用api---调试等](#p-unity通用api-调试等)
  - [Q、调试UI](#q-调试ui)
  - [R、启动时设置脚本](#r-启动时设置脚本)
  - [S、游戏功能api---外部系统交互](#s-游戏功能api-外部系统交互)
  - [T、游戏功能api---wetest调用](#t-游戏功能api-wetest调用)
  - [U、游戏功能api---设备查询](#u-游戏功能api-设备查询)
  - [V、游戏功能api---场景查询](#v-游戏功能api-场景查询)
  - [W、游戏功能api---UI操作](#w-游戏功能api-ui操作)
  - [X、游戏功能api---材质参数](#x-游戏功能api-材质参数)
  - [Y、游戏功能api---PlayerPrefs](#y-游戏功能api-playerprefs)
  - [Z、游戏功能api---内存查询](#z-游戏功能api-内存查询)

<!-- /code_chunk_output -->

