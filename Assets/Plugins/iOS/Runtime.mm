#if defined(__cplusplus)
extern "C" {
#endif
    
bool g_ObjectDictInited = false;
NSMutableDictionary* g_Dict = nullptr;
int g_NextId = 1;

enum ArgTypeEnum
{
    ArgType_Int8 = 0,
    ArgType_Uint8,
    ArgType_Int16,
    ArgType_Uint16,
    ArgType_Int32,
    ArgType_Uint32,
    ArgType_Int64,
    ArgType_Uint64,
    ArgType_Bool,
    ArgType_Float,
    ArgType_Decimal,
    ArgType_Double,
    ArgType_String
};

typedef struct
{
    int ArgType;
    long LongVal;
    double DoubleVal;
    const char* StringVal;
} ArgTypeInfo;

id ArgType2Id(ArgTypeInfo arg)
{
    switch(arg.ArgType){
        case ArgType_Int8:
            return [NSNumber numberWithChar: (char)arg.LongVal];
        case ArgType_Uint8:
            return [NSNumber numberWithUnsignedChar: (unsigned char)arg.LongVal];
        case ArgType_Int16:
            return [NSNumber numberWithShort: (short)arg.LongVal];
        case ArgType_Uint16:
            return [NSNumber numberWithUnsignedShort: (unsigned short)arg.LongVal];
        case ArgType_Int32:
            return [NSNumber numberWithInt: (int)arg.LongVal];
        case ArgType_Uint32:
            return [NSNumber numberWithUnsignedInt: (unsigned int)arg.LongVal];
        case ArgType_Int64:
            return [NSNumber numberWithLongLong: (long long)arg.LongVal];
        case ArgType_Uint64:
            return [NSNumber numberWithUnsignedLongLong: (unsigned long long)arg.LongVal];
        case ArgType_Bool:
            return [NSNumber numberWithBool: (bool)arg.LongVal];
        case ArgType_Float:
            return [NSNumber numberWithFloat: (float)arg.DoubleVal];
        case ArgType_Decimal:
            return [NSNumber numberWithFloat: (float)arg.DoubleVal];
        case ArgType_Double:
            return [NSNumber numberWithDouble: (double)arg.DoubleVal];
        case ArgType_String:
            return [NSString stringWithUTF8String: (const char*)arg.StringVal];
        default:
            return [NSNumber numberWithInt: (int)arg.LongVal];
    }
}
    
ArgTypeInfo Id2ArgType(id val)
{
    ArgTypeInfo info;
    if([val isKindOfClass: [NSNumber class]]){
        double v1 = [val doubleValue];
        long long v2 = [val longLongValue];
        if(abs(v1-v2)<0.0001f){
            info.ArgType = ArgType_Int64;
            info.LongVal = v2;
        } else {
            info.ArgType = ArgType_Double;
            info.DoubleVal = v1;
        }
    } else if([val isKindOfClass: [NSString class]]){
        info.ArgType = ArgType_String;
        info.StringVal = [val UTF8String];
    }
    return info;
}
    
void PrepareObjectDict(void)
{
    if(!g_ObjectDictInited){
        g_ObjectDictInited = true;
        g_Dict = [NSMutableDictionary dictionary];
    }
}
    
ArgTypeInfo ObjectGet(int objId)
{
    PrepareObjectDict();
    id rkey = [NSNumber numberWithInt:objId];
    id val = [g_Dict objectForKey:rkey];
    return Id2ArgType(val);
}

int CallClass(const char* class0, const char* method, ArgTypeInfo* args, int num)
{
    PrepareObjectDict();
    NSString* _class = [NSString stringWithUTF8String:class0];
    NSString* _method = [NSString stringWithUTF8String:method];

    Class cls = NSClassFromString(_class);
    SEL selector = NSSelectorFromString(_method);
    
    //1、创建签名对象
    NSMethodSignature*signature = [cls methodSignatureForSelector:selector];
    //2、判断传入的方法是否存在
    if (!signature) {
        //传入的方法不存在 就抛异常
        //NSString*info = [NSString stringWithFormat:@"-[%@ %@]:unrecognized selector sent to instance",cls,NSStringFromSelector(selector)];
        //@throw [[NSException alloc] initWithName:@"方法没有" reason:info userInfo:nil];
        return 0;
    }
    //3、创建NSInvocation对象
    NSInvocation*invocation = [NSInvocation invocationWithMethodSignature:signature];
    //4、保存方法所属的对象
    invocation.target = cls;           // index 0
    invocation.selector = selector;    // index 1

    //5、设置参数
    NSUInteger count = [signature numberOfArguments];
    for (int index = 0; index < num && index + 2 < count; ++index) {
        id tmp = ArgType2Id(args[index]);
        if([tmp isKindOfClass:[NSNull class]])
            tmp = nil;
        const char* type = [signature getArgumentTypeAtIndex:index+2];
        if(strlen(type)>1 || type[0]=='@' || type[0]=='#' || type[0]==':' || type[0]=='?'){
            id okey = [NSNumber numberWithInt:[tmp longLongValue]];
            tmp = [g_Dict objectForKey:okey];
        }
        [invocation setArgument:&tmp atIndex:index+2];
    }

    //6、调用NSinvocation对象
    [invocation invoke];

    //7、获取返回值
    id res = nil;
    if (signature.methodReturnLength ==0)
        return 0;
    //getReturnValue获取返回值
    [invocation getReturnValue:&res];
    if(nil==res){
        return 0;
    } else {
        int newId = g_NextId++;
        id key = [NSNumber numberWithInt:newId];
        [g_Dict setObject:res forKey:key];
        return newId;
    }
}

int CallInstance(int objId, const char* method, ArgTypeInfo* args, int num)
{
    PrepareObjectDict();
    id rkey = [NSNumber numberWithInt:objId];
    id _self = [g_Dict objectForKey:rkey];
    NSString* _method = [NSString stringWithUTF8String:method];

    SEL selector = NSSelectorFromString(_method);
    
    //1、创建签名对象
    NSMethodSignature*signature = [[_self class] instanceMethodSignatureForSelector:selector];
    //2、判断传入的方法是否存在
    if (!signature) {
        //传入的方法不存在 就抛异常
        //NSString*info = [NSString stringWithFormat:@"-[%@ %@]:unrecognized selector sent to instance",[self class],NSStringFromSelector(aSelector)];
        //@throw [[NSException alloc] initWithName:@"方法没有" reason:info userInfo:nil];
        return 0;
    }
    //3、创建NSInvocation对象
    NSInvocation*invocation = [NSInvocation invocationWithMethodSignature:signature];
    //4、保存方法所属的对象
    invocation.target = _self;           // index 0
    invocation.selector = selector;    // index 1

    //5、设置参数
    NSUInteger count = [signature numberOfArguments];
    for (int index = 0; index < num && index + 2 < count; ++index) {
        id tmp = ArgType2Id(args[index]);
        if([tmp isKindOfClass:[NSNull class]])
            tmp = nil;
        [invocation setArgument:&tmp atIndex:index+2];
    }

    //6、调用NSinvocation对象
    [invocation invoke];

    //7、获取返回值
    id res = nil;
    if (signature.methodReturnLength ==0)
        return 0;
    //getReturnValue获取返回值
    [invocation getReturnValue:&res];
    if(nil==res){
        return 0;
    } else {
        int newId = g_NextId++;
        id key = [NSNumber numberWithInt:newId];
        [g_Dict setObject:res forKey:key];
        return newId;
    }
}

#if defined(__cplusplus)
}
#endif
