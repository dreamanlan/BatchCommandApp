<!-- @import "[TOC]" {cmd="toc" depthFrom=1 depthTo=6 orderedList=false} -->

<!-- code_chunk_output -->

- [一、语法](#一语法)
- [二、基本用法](#二基本用法)
- [三、GM脚本命令与函数](#三gm脚本命令与函数)
- [四、GM脚本文件](#四gm脚本文件)
- [五、变量](#五变量)
- [六、启动脚本](#六启动脚本)
- [七、调试UI](#七调试ui)
- [八、源码位置](#八源码位置)
- [九、api参考](#九api参考)
	- [A、基础api---语句与异步机制](#a基础api---语句与异步机制)
	- [B、基础api---消息与执行机制](#b基础api---消息与执行机制)
	- [C、基础api---运算](#c基础api---运算)
	- [D、基础api---类型转换](#d基础api---类型转换)
	- [E、基础api---反射调用](#e基础api---反射调用)
	- [F、基础api---字符串](#f基础api---字符串)
	- [G、基础api---列表与哈希表](#g基础api---列表与哈希表)
	- [H、基础api---数学](#h基础api---数学)
	- [I、基础api---文件操作](#i基础api---文件操作)
	- [J、基础api---其它](#j基础api---其它)
	- [K、unity通用api---对象与组件](#kunity通用api---对象与组件)
	- [L、unity通用api---对象空间位置](#lunity通用api---对象空间位置)
	- [M、unity通用api---几个特殊值对象构造](#munity通用api---几个特殊值对象构造)
	- [N、unity通用api---随机](#nunity通用api---随机)
	- [O、unity通用api---时间](#ounity通用api---时间)
	- [P、unity通用api---调试等](#punity通用api---调试等)
	- [Q、调试UI](#q调试ui)
	- [R、性能分级脚本](#r性能分级脚本)
	- [S、游戏功能api---外部系统交互](#s游戏功能api---外部系统交互)
	- [T、游戏功能api---wetest调用](#t游戏功能api---wetest调用)
	- [U、游戏功能api---设备查询](#u游戏功能api---设备查询)
	- [V、游戏功能api---场景查询](#v游戏功能api---场景查询)
	- [W、游戏功能api---UI操作](#w游戏功能api---ui操作)
	- [X、游戏功能api---材质参数](#x游戏功能api---材质参数)
	- [Y、游戏功能api---PlayerPrefs](#y游戏功能api---playerprefs)
	- [Z、游戏功能api---内存查询](#z游戏功能api---内存查询)

<!-- /code_chunk_output -->


## 一、语法

**【一句话语法】**

***简单说就是c/c#的函数体内的语法，但是即便是语句，结尾也需要加分号，除非语句是所属块内的最后一个语句。***

## 二、基本用法

1. 入口在DebugTool的开发自用页签，同时提供了从剪贴板与adb命令输入的方式
2. 安卓profiler版本默认启动会监听adb命令，release版本需要先输入
```
setdebug(1);listenandroid();
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
setdebug(1);listenclipboard(100);
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
1.  GM脚本是在DebugConsole命令的基础上扩展的，输入/?会显示DebugConsole的命令列表（比较少，除了开关DebugConsole外主要就是用来执行gm命令或脚本的命令）
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
2.  GM脚本有一个命令cmd("命令串")，这个命令串是提交给DebugConsole来执行的，所以也能使用这里列出的各条命令

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
	log(deviceinfo())
	<=>
	log deviceinfo()
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
	loop(3600){log("loop {0}",$$);logprofiler();wait(1000);};
	```
	- looplist命令，用来遍历一个IList，循环体内可以使用\$\$来访问当前遍历的元素
	```
	looplist(list变量){命令列表};
	```
	比如下面循环遍历数组并显示各数组元素(gm脚本支持数组的直接写法，实际是c#的List)：
	```
	looplist([1,2,3,4]){log("looplist {0}",$$);};
	```
	再比如下面循环遍历hash表并显示各对元素(gm脚本里的哈希表的key可以是常量或变量，所以字符串常量作为key时必须加引号)：
	```
	looplist({1=>13,3=>25,9=>345}){log("pair {0}:{1}",$$.Key,$$.Value);};
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
			localmessage("proc1");
			localmessage("proc2");
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
			localmessage("proc",123);
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
				logprofiler();
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
	propset(name, val);
	propget(name);
	propget(name, defval);
```
## 六、启动脚本

1. 启动时GM脚本配置
2. 安卓系统启动时会检查/data/local/tmp目录下是否有initgm.txt的文本文件（不是安卓系统检查Application.persistentDataPath目录下是否有initgm.txt），如果有则把initgm.txt的每一行当作一行GM脚本或DebugConsole命令进行处理，这些命令作为一个命令列表执行，所以中间执行的命令应该不能是执行命令或GM脚本的命令（因为会重置GM脚本解释器），我们可以在这个文件里配置要加载执行的GM脚本文件，然后后续就交给GM脚本文件处理了，比如下面的内容就是启动时打开调试开关（会影响GM脚本的日志输出），然后根据apk来决定加载执行不同的GM脚本文件
```
	setdebug(1);
	if(appid()=='DefaultCompany.Test'){cmd('scp init.dsl');};
	if(appid()!='DefaultCompany.Test'){cmd('scp init0.dsl');};
```
3. 对于不需要使用GM脚本文件的情形，initgm.txt里一般配置监听adb命令或剪贴板方便后续输入命令
```
	setdebug(1);
	if(isandroid()){listenandroid();};
	if(!isandroid()){listenclipboard(100);};
```
4. 启动时性能分级设置脚本配置（目前仅用于性能实验）
5. 性能分级脚本的主要考虑是对游戏分档的实验与补充，一方面是对新增机型的分档实验，另一方面是对部分特殊机型可能需要在不同档位做一些特殊的设置。考虑到性能分级通常由性能测试发现，是一个不断迭代的过程，所以考虑提供一个脚本来方便在不打包的情况下修改设置，并避免每次启动都要重复进行设置操作
6. 性能分级脚本不像GM脚本那样自由，主要考虑到这些脚本在实验确定后应该翻译为c#固化为启动逻辑的一部分，为了方便翻译到c#，需要尽量符合c#的风格。
7. 与GM脚本配置类似，在安卓启动时会检查/data/local/tmp目录下是否perf0.dsl - perf31.dsl，这32个文件中的任何文件，如果存在则加载执行。（不是安卓系统检查Application.persistentDataPath目录下是否有这些脚本文件）
8. 性能分级脚本分为init、grade、default_grade、setting四个部分
	- 首先会整理所有性能分级脚本里的这些部分，归类为4个集合，然后先执行init集合里的脚本，这里一般设置根据机器model或gpu能直接确定的分档（优先级最高，匹配到分档后就不再检查后面的grade与default_grade）
	- 如果init部分没有确定分档，则开始执行grade集合里的脚本，这里是按grade值的升序执行检查（0是最高级，4是目前的最低级），一旦确定分档就不再继续执行
	- 如果所有grade仍然没能确定分档，则开始执行default_grade集合里的脚本，这里的脚本与grade类似也是按grade值升序执行检查（0是最高级，4是目前的最低级），default_grade一般是根据一些通用的指标来确定分档，也就是没那么精确，但基本上所有手机都能确定一个分档
	- 经过上面3步确定了grade后，接下来执行grade值对应的setting部分的代码
	- grade与default_grade里面的代码每一行是一个条件判断，行与行之间的条件的关系是and
9. 我们用于实验的一个perf0.dsl内容如下：（还很初级，主要针对几个实验机型，逻辑不完全）
```
	perf_grade(0)
	{
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
10. 性能分级脚本的api都在Assets/PerfGrade/PerfGrade.cs里实现，这些api不用注册，脚本解释器采用reflection来自动查找相应的api
11. 每个性能分级脚本的api都需要二个原型，一个是脚本api方法，供脚本解释器调用，一个是api实现，供脚本翻译到的c#代码直接调用。脚本api方法一般会调用api实现来实现api功能，或者二者都调用共用的内部实现方法。api实现的名称必须是api名称加上"_impl"，参数需要与脚本里的写法匹配（下面代码里set_custom_fps是脚本api方法，set_custom_fps_impl是api实现方法）
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
12. 性能分级脚本的api也可以在GM脚本里调用，但是没法提供文档查询

## 七、调试UI

1. 输GM命令或执行GM脚本文件的方式适用于无法预见的功能，对于一些明确的功能，固定的UI还是更方便一些，与DebugTool类似，我们也实现了一套调试UI的机制
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
	loadui("UI资源");
	showui();
	hideui();
```
8. 调试UI的资源就是一个DSL文本文件，目前放在Assets/Resources目录下
9. GM脚本的启动脚本以及调试UI的Canvas的prefab也在这个目录下：GmScript.prefab
10. 在安卓手机上，我们可以直接输GM命令来加载显示隐藏调试UI，也可以通过adb命令来操作
```
显示当前加载的调试ui：
	adb shell am broadcast -a com.unity3d.command -e cmd 'showui()'
隐藏调试ui:
	adb shell am broadcast -a com.unity3d.command -e cmd 'hideui()'
加载调试ui（调试UI的资源文件名不包含空格时，可以不加引号，这个是在loadui API实现时专门支持的写法）:
	adb shell am broadcast -a com.unity3d.command -e cmd 'loadui(TestUI)'
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
Assets\GmCommands\PerfGradeGm.cs //性能分级脚本解释器部分，GM脚本可以直接调用性能分级的api，不过这部分api使用"/? 过滤串"时查询不到

除GM脚本外，我们实现了一套简单的调试ui工具，用DSL来描述ui，然后逻辑在c#里提供，使用gm命令来加载、显示及隐藏这些ui
Assets\GmCommands\UiHanlder.cs //调试ui的逻辑处理部分

GM脚本是一个异步执行的脚本，没办法像通常的脚本在启动时立即执行（可能滞后一帧），为了支持性能分级实验，我们实现了性能分级脚本（性能分级脚本支持翻译为c#代码，从而不再依赖脚本解释器），这里是api与纯c#的逻辑框架部分
Assets\PerfGrade\PerfGrade.cs //性能分级的api与性能分级逻辑框架
```

## 九、api参考

***更好的查阅api的办法是使用ilspy或类似工具，加载项目工程的Assembly-CSharp.dll（在Library/ScriptAssemblies目录下）与StoryScript.dll，然后搜索查看实现***

### A、基础api---语句与异步机制

- 语句列表，语句用法见前面介绍部分
```
= [foreach]:foreach(a,b,...){command_list;}; statement, iterator is $$
= [if]:if(condition){command_list;}elseif(condition){command_list;}else{command_list;}; statement, elseif can be zero or more, else can be zero or one
= [loop]:loop(count){command_list;}; statement, iterator is $$
= [looplist]:looplist(list){command_list;}; statement, iterator is $$
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
= [propset]:propset(name, val) command
```
- 全局key/value读函数
```
= [propget]:propget(name[,defval] function
```
- ~~应该不会用到的命令，请勿使用~~
```
= [clearglobals]:clearglobals() command
= [clearmessage]:clearmessage(msgid1,msgid2,...) command
= [clearnamespacedmessage]:clearnamespacedmessage(msgid1,msgid2,...) command

= [localconcurrentmessage]:localconcurrentmessage(msgid,arg1,arg2,...) command
= [localconcurrentnamespacedmessage]:localconcurrentnamespacedmessage(msgid,arg1,arg2,...) command
= [localmessage]:localmessage(msgid,arg1,arg2,...) command
= [localnamespacedmessage]:localnamespacedmessage(msgid,arg1,arg2,...) command

= [resumelocalmessagehandler]:resumelocalmessagehandler(msgid1,msgid2,...) command
= [resumelocalnamespacedmessagehandler]:resumelocalnamespacedmessagehandler(msgid1,msgid2,...) command

= [suspend]:suspend command
= [suspendlocalmessagehandler]:suspendlocalmessagehandler(msgid1,msgid2,...) command
= [suspendlocalnamespacedmessagehandler]:suspendlocalnamespacedmessagehandler(msgid1,msgid2,...) command

= [waitlocalmessage]:waitlocalmessage(msgid1,msgid2,...)[set(var,val)timeoutset(timeout,var,val)] command
= [waitlocalmessagehandler]:waitlocalmessagehandler(msgid1,msgid2,...)[set(var,val)timeoutset(timeout,var,val)] command
= [waitlocalnamespacedmessage]:waitlocalnamespacedmessage(msgid1,msgid2,...)[set(var,val)timeoutset(timeout,var,val)] command
= [waitlocalnamespacedmessagehandler]:waitlocalnamespacedmessagehandler(msgid1,msgid2,...)[set(var,val)timeoutset(timeout,var,val)] command

= [pause]:pause command
= [terminate]:terminate command

= [clearcmdsubsts]:clearcmdsubsts() command
= [clearfuncsubsts]:clearfuncsubsts() command
= [substcmd]:substcmd(id,substId) command
= [substfunc]:substfunc(id,substId) command
```
- ~~应该不会用到的函数，请勿使用~~
```
= [countcommand]:countcommand(level) function
= [counthandlercommand]:counthandlercommand() function
= [namespace]:namespace() function
= [storyid]:storyid() function
= [messageid]:messageid() function

= [getcmdsubst]:getcmdsubst(id) function
= [getfuncsubst]:getfuncsubst(id) function
```
### C、基础api---运算

- 命令类
```
= [assign]:assign(var, val) command，赋值操作符的函数样式写法
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
- 函数类
```
= [collectioncall]:collectioncall function, internal implementation, using csharp object syntax
= [collectionget]:collectionget function, internal implementation, using csharp object syntax
= [dotnetcall]:dotnetcall function, internal implementation, using csharp object syntax
= [dotnetget]:dotnetget function, internal implementation, using csharp object syntax
```
- 反射工具函数，主要用于类型变换或根据字符串获取Type对象
```
= [changetype]:changetype(obj,type_obj_or_str) function，类似Convert.ChangeType，另外考虑了BoxedValue与object继承情形的转型
= [gettype]:gettype(type_name_str) function，Type.GetType(typeName)
= [getunitytype]:getunitytype(type_str) function，Type.GetType($"UnityEngine.{typeName},UnityEngine")
= [getunityuitype]:getunityuitype(type_str) function，Type.GetType($"UnityEngine.UI.{typeName},UnityEngine.UI")
= [getusertype]:getusertype(type_str) function，Type.GetType($"{typeName},Assembly-CSharp")
= [typeof]:typeof(type) or typeof(type,assembly) function，Type.GetType，Automatically try gettype/getunitytype/getunityuitype/getusertype
= [gettypeassemblyname]:gettypeassemblyname(obj) function，get Type.AssemblyQualifiedName
= [gettypefullname]:gettypefullname(obj) function，get Type.FullName
= [gettypename]:gettypename(obj) function，get Type.Name
= [parseenum]:parseenum(type_obj_or_str,enum_val) function，Enum.Parse

= [linq]:linq(obj,method,arg1,arg2,...) function, internal implementation, using obj.method(arg1,arg2,...) syntax, method can be orderby/orderbydesc/where/top, iterator is $$
```
### F、基础api---字符串

- 命令类
```
= [appendformat]:appendformat(sb,fmt,arg1,arg2,...) command
= [appendlineformat]:appendformatline(sb,fmt,arg1,arg2,...) command
```
- 函数类
```
= [hex2int]:hex2int(str) function
= [hex2long]:hex2long(str) function
= [hex2uint]:hex2uint(str) function
= [hex2ulong]:hex2ulong(str) function
= [str2double]:str2double(str) function
= [str2long]:str2long(str) function
= [str2uint]:str2uint(str) function
= [str2ulong]:str2ulong(str) function
= [str2float]:str2float(str) function
= [str2int]:str2int(str) function

= [datetimestr]:datetimestr(fmt) function
= [shortdatestr]:shortdatestr() function
= [shorttimestr]:shorttimestr() function
= [longdatestr]:longdatestr() function
= [longtimestr]:longtimestr() function

= [isnullorempty]:isnullorempty(str) function
= [makestring]:makestring(char1_as_str_or_int,char2_as_str_or_int,...) function
= [newstringbuilder]:newstringbuilder() function
= [stringjoin]:stringjoin(sep,list) function
= [stringreplace]:stringreplace(str,key,rep_str) function
= [stringreplacechar]:stringreplacechar(str,key,char_as_str) function
= [stringsplit]:stringsplit(str,sep_list) function
= [stringtrim]:stringtrim(str) function
= [stringtrimend]:stringtrimend(str) function
= [stringtrimstart]:stringtrimstart(str) function

= [str2lower]:str2lower(str) function, use cache
= [str2upper]:str2upper(str) function, use cache
= [stringcontains]:stringcontains(str,str1,str2,...) function
= [stringcontainsany]:stringcontainsany(str,str1,str2,...) function
= [stringlist]:stringlist(str_split_by_sep) function
= [stringnotcontains]:stringnotcontains(str,str1,str2,...) function
= [stringnotcontainsany]:stringnotcontainsany(str,str1,str2,...) function
= [stringtolower]:stringtolower(str) function
= [stringtoupper]:stringtoupper(str) function
= [substring]:substring(str[,start,len]) function

= [format]:format(fmt[,arg1,...]) function
```
### G、基础api---列表与哈希表

- 命令类
```
= [hashtableadd]:hashtableadd(hashtable,key,val) command
= [hashtableclear]:hashtableclear(hashtable) command
= [hashtableremove]:hashtableremove(hashtable,key) command
= [hashtableset]:hashtableset(hashtable,key,val) command

= [listadd]:listadd(list,value) command
= [listclear]:listclear(list) command
= [listenandroid]:listenandroid() command
= [listenclipboard]:listenclipboard(interval) command
= [listinsert]:listinsert(list,index,value) command
= [listremove]:listremove(list,value) command
= [listremoveat]:listremoveat(list,index) command
= [listset]:listset(list,index,value) command
```
- 函数类
```
= [hashtable]:hashtable(k1=>v1,k2=>v2,...) function
= [hashtableget]:hashtableget(hash_obj,key[,defval]) function
= [hashtablekeys]:hashtablekeys(hash_obj) function
= [hashtablesize]:hashtablesize(hash_obj) function
= [hashtablevalues]:hashtablevalues(hash_obj) function

= [list]:list(v1,v2,...) function
= [listget]:listget(list,index[,defval]) function
= [listindexof]:listindexof(list,val) function
= [listsize]:listsize(list) function

= [array]:array(v1,v2,...) function
= [toarray]:toarray(list) function

= [floatlist]:floatlist(str_split_by_sep) function
= [intlist]:intlist(str_split_by_sep) function
= [vector2list]:vector2list(str_split_by_sep) function, vector2 per 2 elements
= [vector3list]:vector3list(str_split_by_sep) function, vector3 per 3 elements
```
### H、基础api---数学

- 所有数学api都是函数
```
= [abs]:abs(val) function
= [acos]:acos(v) function
= [approximately]:approximately(v1,v2) function
= [asin]:asin(v) function
= [atan]:atan(v) function
= [atan2]:atan2(v1,v2) function
= [ceiling]:ceiling(val) function
= [ceilingtoint]:ceilingtoint(v) function
= [clamp]:clamp(v,v1,v2) function
= [clamp01]:clamp01(v) function
= [closestpoweroftwo]:closestpoweroftwo(v) function
= [cos]:cos(val) function
= [cosh]:cosh(val) function
= [deg2rad]:deg2rad(deg) function
= [rad2deg]:rad2deg(rad) function
= [dist]:dist(x1,y1,x2,y2) function
= [distsqr]:distsqr(x1,y1,x2,y2) function
= [exp]:exp(v) function
= [exp2]:exp2(v) function
= [floor]:floor(val) function
= [floortoint]:floortoint(v) function
= [ispoweroftwo]:ispoweroftwo(v) function
= [lerp]:lerp(a,b,t) function
= [lerpangle]:lerpangle(a,b,t) function
= [lerpunclamped]:lerpunclamped(a,b,t) function
= [log]:log(x) or log(x,y) function
= [log10]:log10(v) function
= [log2]:log2(v) function
= [max]:max(v1,v2,...) function
= [min]:min(v1,v2,...) function
= [nextpoweroftwo]:nextpoweroftwo(v) function
= [pow]:pow(x) or pow(x,y) function
= [exp]:exp(x) function
= [round]:round(val) function
= [roundtoint]:roundtoint(v) function
= [smoothstep]:smoothstep(from,to,t) function
= [sin]:sin(val) function
= [sinh]:sinh(val) function
= [sqrt]:sqrt(val) function
= [tan]:tan(v) function
= [tanh]:tanh(v) function

= [vector2dist]:vector2dist(pt1,pt2) function
= [vector3dist]:vector3dist(pt1,pt2) function
```
### I、基础api---文件操作

- 命令类
```
= [writealllines]:writealllines(file,lines) command
= [writefile]:writefile(file,txt) command
```
- 函数类
```
= [readalllines]:readalllines(file) function
= [readfile]:readfile(file) function
= [combinepath]:combinepath(path1,path2) function
= [getdirname]:getdirname(path) function
= [getextension]:getextension(path) function
= [getfilename]:getfilename(path) function
```
### J、基础api---其它

- 命令类
```
= [help]:help() command，执行后打开浏览器并跳转到本文档
= [log]:log(fmt,args) command，使用warning日志输出到logcat同时输出到DebugConsole，格式化串与C#的string.Format相同
```
- 函数类
```
= [setdebug]:setdebug(1_or_0) command

= [eval]:eval(exp1,exp2,...) function
= [time]:time() function

= [fromjson]:fromjson(json_str) function
= [tojson]:tojson(obj) function
```
### K、unity通用api---对象与组件

- 命令类
```
= [addcomponent]:addcomponent(obj_or_path,type_or_str)[obj("varname")] command
= [removecomponent]:removecomponent(obj_or_path,type_or_str) command

= [addtransform]:addtransform(name,local0_or_world1){position(vector3(x,y,z));rotation(vector3(x,y,z));scale(vector3(x,y,z));} command

= [creategameobject]:creategameobject(name,prefab[,parent])[obj("varname")]{position(vector3(x,y,z));rotation(vector3(x,y,z));scale(vector3(x,y,z));loadtimeout(1000);disable("typename", "typename", ...);remove("typename", "typename", ...);} command
= [destroygameobject]:destroygameobject(path) command

= [gameobjectanimation]:gameobjectanimation(obj, anim[, normalized_time]) command
= [gameobjectanimationparam]:gameobjectanimationparam(obj){float(name,val);int(name,val);bool(name,val);trigger(name,val);} command

= [setparent]:setparent(obj_or_path,parent,int_stay_world_pos) command
= [setactive]:setactive(obj_or_path,1_or_0) command
= [setvisible]:setvisible(obj_or_path,1_or_0) command

= [sendmessage]:sendmessage(objname,msg,arg1,arg2,...) command
= [sendmessagewithgameobject]:sendmessagewithgameobject(gameobject,msg,arg1,arg2,...) command
= [sendmessagewithtag]:sendmessagewithtag(tagname,msg,arg1,arg2,...) command
```
- 函数类
```
= [equalsnull]:equalsnull(obj) function
= [isnull]:isnull(obj) function
= [null]:null() function

= [getcomponent]:getcomponent(obj_or_path,type_or_str) function
= [getcomponentinchildren]:getcomponentinchildren(obj_or_path,type_or_str[,int_inc_inactive]) function
= [getcomponentinparent]:getcomponentinparent(obj_or_path,type_or_str[,int_inc_inactive]) function
= [getcomponents]:getcomponents(obj_or_path,type_or_str) function
= [getcomponentsinchildren]:getcomponentsinchildren(obj_or_path,type_or_str[,int_inc_inactive]) function
= [getcomponentsinparent]:getcomponentsinparent(obj_or_path,type_or_str[,int_inc_inactive]) function
= [getgameobject]:getgameobject(obj_or_path) or getgameobject(obj_or_path){disable(comp1,comp2,...);remove(comp1,comp2,...);} function
= [getparent]:getparent(obj_or_path) function
= [getchild]:getchild(obj_or_path,child_path) function

= [isactive]:isactive(obj_or_path) function
= [isreallyactive]:isreallyactive(obj_or_path) function
= [isvisible]:isvisible(obj_or_path) function
```
### L、unity通用api---对象空间位置

- 命令类
```
= [settransform]:settransform(name,local0_or_world1){position(vector3(x,y,z));rotation(vector3(x,y,z));scale(vector3(x,y,z));} command
```
- 函数类
```
= [getposition]:getposition(obj_or_path[,local0_or_world1]) function
= [getpositionx]:getpositionx(obj_or_path[,local0_or_world1]) function
= [getpositiony]:getpositiony(obj_or_path[,local0_or_world1]) function
= [getpositionz]:getpositionz(obj_or_path[,local0_or_world1]) function
= [getrotation]:getrotation(obj_or_path[,local0_or_world1]) function
= [getrotationx]:getrotationx(obj_or_path[,local0_or_world1]) function
= [getrotationy]:getrotationy(obj_or_path[,local0_or_world1]) function
= [getrotationz]:getrotationz(obj_or_path[,local0_or_world1]) function
= [getscale]:getscale(obj_or_path) function
= [getscalex]:getscalex(obj_or_path) function
= [getscaley]:getscaley(obj_or_path) function
= [getscalez]:getscalez(obj_or_path) function
```
### M、unity通用api---几个特殊值对象构造

- 函数类
```
= [color]:color(r,g,b,a) function
= [color32]:color32(r,g,b,a) function
= [eular]:eular(x,y,z) function
= [position]:position(x,y,z) function
= [quaternion]:quaternion(x,y,z,w) function
= [rotation]:rotation(x,y,z) function
= [scale]:scale(x,y,z) function
= [vector2]:vector2(x,y) function
= [vector2int]:vector2int(x,y) function
= [vector2to3]:vector2to3(pt) function
= [vector3]:vector3(x,y,z) function
= [vector3int]:vector3int(x,y,z) function
= [vector3to2]:vector3to2(pt) function
= [vector4]:vector4(x,y,z,w) function
```
### N、unity通用api---随机

- 函数类
```
= [rndfloat]:rndfloat() function
= [rndfromlist]:rndfromlist(list[,defval]) function
= [rndint]:rndint(min,max) function
= [rndvector2]:rndvector2(pt,radius) function
= [rndvector3]:rndvector3(pt,radius) function
```
### O、unity通用api---时间

- 函数类
```
= [gettime]:gettime() function
= [gettimescale]:gettimescale() function
```
### P、unity通用api---调试等

- 调试
```
= [logcodenum]:logcodenum() command，输出代码里的一个整数，用来确认版本是否符合，打包时先更新此值，运行时对比

= [debugbreak]:debugbreak() command，Debug.DebugBreak
= [editorbreak]:editorbreak() command，Debug.Break
```
- 打开UI
```
= [openurl]:openurl(url) command
```
- 退出
```
= [quit]:quit() command
```
### Q、调试UI

- 命令类
```
= [loadui]:loadui(ui_name_dsl) command，加载指定的调试ui
= [showui]:showui() command，显示当前加载的调试ui
= [hideui]:hideui() command，隐藏当前调试ui
```
### R、性能分级脚本

- 命令类
```
= [compileperf]:compileperf(perf_dsl_file) command，将性能分级脚本翻译到C#代码
= [reloadperfs]:reloadperfs() command，重新执行全部性能分级脚本（/data/local/tmp/perf*.dsl）
= [runperf]:runperf(perf_dsl_file) command，运行指定的性能分级脚本
= [logcperfs]:logcperfs() command，打印编译好的性能分级脚本id
```
### S、游戏功能api---外部系统交互

- 命令类
```
= [shell]:shell(cmd) command，异步执行系统命令
= [shelltimeout]:shelltimeout(cmd,ms) command，异步执行系统命令，带超时时间
= [cleanupcompletedtasks]:cleanupcompletedtasks() command，清理底层用于异步执行命令的Task
= [usejavatask]:usejavatask(is_java) command，设置是否使用java任务机制，默认使用c#的Task

= [startactivity]:startactivity(package_name[[,class_name[,flags]],extra_list_or_dict]) command，启动一个activity

目前GM脚本系统带了一个安卓插件（接收adb命令也是通过这个插件），里面有一个用来重启应用的activity，可如下使用
startactivity("com.DefaultCompany.Test","com.unity3d.broadcastlib.RestartActivity",0,["package","com.DefaultCompany.Test","class","com.unity3d.player.UnityPlayerActivity","flags",0]);
或
startactivity("com.DefaultCompany.Test","com.unity3d.broadcastlib.RestartActivity",0,{"package":"com.DefaultCompany.Test","class":"com.unity3d.player.UnityPlayerActivity","flags":0});

常用的flags:
	0x00008000 Intent.FLAG_ACTIVITY_CLEAR_TASK
	0x04000000 Intent.FLAG_ACTIVITY_CLEAR_TOP
	0x08000000 Intent.FLAG_ACTIVITY_MULTIPLE_TASK
	0x10000000 Intent.FLAG_ACTIVITY_NEW_TASK
	0x20000000 Intent.FLAG_ACTIVITY_SINGLE_TOP

= [finishactivity]:finishactivity() command，停止当前activity，相当于退出应用

= [startservice]:startservice(srv_class, extra_name, extra_val) command，启动一个service
= [stopservice]:stopservice(srv_class) command，停止一个service

= [setclipboard]:setclipboard(text) command，写系统剪贴板
```
- 函数类
```
= [shell]:shell(cmd) function, return string，同步执行系统命令
= [shelltimeout]:shelltimeout(cmd,ms) function, return string，同步执行系统命令，带超时时间
= [isjavatask]:isjavatask() function, return int，是否使用的java任务机制
= [gettaskcount]:gettaskcount() function, return int，当前Task数量

= [getclipboard]:getclipboard() function, get system clipboard content，读系统剪贴板

获取unity apk启动参数的函数（使用adb命令来传递参数）
= [getbool]:getbool(str) function
= [getboolarray]:getboolarray(str) function
= [getbyte]:getbyte(str) function
= [getbytearray]:getbytearray(str) function
= [getchar]:getchar(str) function
= [getchararray]:getchararray(str) function
= [getdouble]:getdouble(str) function
= [getdoublearray]:getdoublearray(str) function
= [getfloat]:getfloat(str) function
= [getfloatarray]:getfloatarray(str) function
= [getint]:getint(str) function
= [getintarray]:getintarray(str) function
= [getlong]:getlong(str) function
= [getlongarray]:getlongarray(str) function
= [getshort]:getshort(str) function
= [getshortarray]:getshortarray(str) function
= [getstring]:getstring(str) function
= [getstringarray]:getstringarray(str) function

= [getactivityclass]:getactivityclass() function
```
### T、游戏功能api---wetest调用

- 命令类
```
= [wetesttouch]:wetesttouch(action,x,y) command, simulate touch event with WeTest
```
- 函数类
```
= [wetestheight]:wetestheight() function, WeTest GetHeight
= [wetestwidth]:wetestwidth() function, WeTest GetWidth
= [wetestx]:wetestx() function, WeTest GetX
= [wetesty]:wetesty() function, WeTest GetY
```
### U、游戏功能api---设备查询

- 命令类
```
= [logprofiler]:logprofiler() command，输出性能数据日志（主要是内存信息）
= [logresolutions]:logresolutions() command, print supported resolutions

= [devicesupports]:devicesupports() command, print unsupported feature
= [supportsgf]:supportsgf() command, print unsupported graphics format
= [supportsrt]:supportsrt() command, print unsupported rt
= [supportstex]:supportstex() command, print unsupported tex
= [supportsva]:supportsva() command, print unsupported vertex attribute format
```

- 函数类
```
= [app]:app() function, return typeof(Application)
= [appid]:appid() function, return Application.identifier
= [appname]:appname() function, return Application.productName

= [deviceinfo]:deviceinfo() function, get device name/model and gpu model

= [getcompatibleformat]:getcompatibleformat(fmt_str,usage_str) function
= [getgraphicsformat]:getgraphicsformat(def_fmt) function
= [getmsaasamplecount]:getmsaasamplecount(w,h,color_fmt,depth_bit,mip_ct) function
= [istexsupported]:istexsupported(fmt_str,usage_str) function

= [getpersistentpath]:getpersistentpath() function
= [getstreamingassets]:getstreamingassets() function

= [isandroid]:isandroid() function, return Application.platform==Android
= [isconsole]:isconsole() function, return Application.isConsolePlatform
= [isdebug]:isdebug() function
= [isdev]:isdev() function
= [iseditor]:iseditor() function, return Application.isEditor
= [isiphone]:isiphone() function, return Application.platform==IPhone
= [ismobile]:ismobile() function, return Application.isMobilePlatform
= [ispc]:ispc() function, return not mobile and not console
= [platform]:platform() function, return Application.platform

= [sysinfo]:sysinfo() function, return typeof(SystemInfo)
= [shader]:shader() function, return typeof(Shader)
= [screen]:screen() function, return typeof(Screen)

= [screendpi]:screendpi() function, return Screen.dpi
= [screenheight]:screenheight() function, return Screen.height
= [screenwidth]:screenwidth() function, return Screen.width
```

### V、游戏功能api---场景查询

- 命令类
```
= [logcomps]:logcomps(root_name,[name1,name2,...],type,up_level,include_inactive) command
= [logscenepath]:logscenepath([prefixs],obj,up_level) command
```
- 函数类
```
= [searchcomps]:searchcomps(root_name,[name1,name2,...],type,include_inactive) function
= [getscenepath]:getscenepath([prefixs],obj,up_level) function, return partial scene path
```
### W、游戏功能api---UI操作

- 命令类
```
= [click]:click(uiobj) command
= [clickonpos]:clickonpos(x,y) command
= [clickonptr]:clickonptr() command
= [clickui]:clickui(name1,name2,...) command

= [toggle]:toggle(uiobj) command
= [toggleon]:toggleon(name1,name2,...) command
= [toggleonpos]:toggleonpos(x,y) command
= [toggleonptr]:toggleonptr() command
```
- 函数类
```
= [findbutton]:findbutton(name1,name2,...) function
= [findcomp]:findcomp(root_name,[name1,name2,...],type,include_inactive) function
= [finddropdown]:finddropdown(name1,name2,...) function
= [findinput]:findinput(name1,name2,...) function
= [findrawimg]:findrawimg(name1,name2,...) function
= [findslider]:findslider(name1,name2,...) function
= [findtmpdropdown]:findtmpdropdown(name1,name2,...) function
= [findtmpinput]:findtmpinput(name1,name2,...) function
= [findtoggle]:findtoggle(name1,name2,...) function
= [finduiimg]:finduiimg(name1,name2,...) function

= [getpointer]:getpointer() function, return Input.mousePosition
= [getptrcomps]:getptrcomps(type,include_inactive) function, return List<Component>
= [getptruis]:getptruis() function, return List<UnityEngine.EventSystems.RaycastResult>

= [raycastcomps]:raycastcomps(x,y,type,include_inactive) function, return List<Component>
= [raycastuis]:raycastuis(x,y) function, return List<UnityEngine.EventSystems.RaycastResult>
```

### X、游戏功能api---材质参数

- 命令类
```
= [logmesh]:logmesh(mesh_or_path) command

= [setmat]:setmat(renderer_or_path,mat_or_path) or setmat(renderer_or_path,ix,mat_or_path) or setmat(renderer_or_path,ix,mats_or_path,ix) command
= [setmats]:setmats(mesh_or_path,mats_or_path) command
= [setsmat]:setsmat(renderer_or_path,mat_or_path) or setsmat(renderer_or_path,ix,mat_or_path) or setsmat(renderer_or_path,ix,mats_or_path,ix) command
= [setsmats]:setsmats(mesh_or_path,mats_or_path) command

= [matsetcolor]:matsetcolor(mat_or_path,key,val) command, Material.SetColor
= [matsetfloat]:matsetfloat(mat_or_path,key,val) command, Material.SetFloat
= [matsetint]:matsetint(mat_or_path,key,val) command, Material.SetInt
= [matsetinteger]:matsetinteger(mat_or_path,key,val) command, Material.SetInteger
= [matsetvector]:matsetvector(mat_or_path,key,val) command, Material.SetVector
```
- 函数类
```
= [getmat]:getmat(mat_or_path[,index]) function, renderer.material[s[ix]]
= [getsmat]:getsmat(mat_or_path[,index]) function, renderer.sharedMaterial[s[ix]]

= [matgetcolor]:matgetcolor(mat_or_path,key) function, Material.GetColor
= [matgetfloat]:matgetfloat(mat_or_path,key) function, Material.GetFloat
= [matgetint]:matgetint(mat_or_path,key) function, Material.GetInt
= [matgetinteger]:matgetinteger(mat_or_path,key) function, Material.GetInteger
= [matgetvector]:matgetvector(mat_or_path,key) function, Material.GetVector
```
### Y、游戏功能api---PlayerPrefs

- 命令类
```
= [prefbyjava]:prefbyjava(key,val) command
= [preffloat]:preffloat(key,val) command
= [prefint]:prefint(key,val) command
= [prefstr]:prefstr(key,val) command
```
- 函数类
```
= [prefbyjava]:prefbyjava(key,defval) function
= [preffloat]:preffloat(key,defval) function
= [prefint]:prefint(key,defval) function
= [prefstr]:prefstr(key,defval) function
```
### Z、游戏功能api---内存查询

- 命令类
```
= [allocmemory]:allocmemory(key,size) command
= [freememory]:freememory(key) command
= [gc]:gc() command, force Garbage Collect
= [consumecpu]:consumecpu(time) command
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
  - [R、性能分级脚本](#r-性能分级脚本)
  - [S、游戏功能api---外部系统交互](#s-游戏功能api-外部系统交互)
  - [T、游戏功能api---wetest调用](#t-游戏功能api-wetest调用)
  - [U、游戏功能api---设备查询](#u-游戏功能api-设备查询)
  - [V、游戏功能api---场景查询](#v-游戏功能api-场景查询)
  - [W、游戏功能api---UI操作](#w-游戏功能api-ui操作)
  - [X、游戏功能api---材质参数](#x-游戏功能api-材质参数)
  - [Y、游戏功能api---PlayerPrefs](#y-游戏功能api-playerprefs)
  - [Z、游戏功能api---内存查询](#z-游戏功能api-内存查询)

<!-- /code_chunk_output -->

