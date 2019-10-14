NSMutableDictionary* g_Dict = nullptr;
int g_NextId = 1;

int CallClass(const char* class, const char* method, id* args, int num);
int CallInstance(int objId, const char* method, id* args, int num);

void InitOcDict(void)
{
    g_Dict = [NSMutableDictionary dictionary];
}

int CallClass(const char* class, const char* method, id* args, int num)
{
    NSString* _class = [NSString stringWithCString:class];
    NSString* _method = [NSString stringWithCString:method];

    CLASS cls = NSClassFromString(_class);
    SEL selector = NSSelectorFromString(_method);
    IMP imp = [self methodForSelector:selector];
    void (*func)(id, SEL) = (void *)imp;
    func(self, selector);
    
    //1、创建签名对象
    NSMethodSignature*signature = [cls methodSignatureForSelector:selector];
    //2、判断传入的方法是否存在
    if (!signature) {
        //传入的方法不存在 就抛异常
        NSString*info = [NSString stringWithFormat:@"-[%@ %@]:unrecognized selector sent to instance",cls,NSStringFromSelector(aSelector)];
        @throw [[NSException alloc] initWithName:@"方法没有" reason:info userInfo:nil];
        return nil;
    }
    //3、创建NSInvocation对象
    NSInvocation*invocation = [NSInvocation invocationWithMethodSignature:signature];
    //4、保存方法所属的对象
    invocation.target = cls;           // index 0
    invocation.selector = selector;    // index 1

    //5、设置参数
    NSUInteger count = [signature numberOfArguments];
    for (int index = 0; index < num && index + 2 < count; ++index) {
        id tmp = args[index];
        if([tmp isKindOfClass:[NSNull class]])
            tmp = nil;
        [invocation setArgument:&tmp atIndex:index];
    }

    //6、调用NSinvocation对象
    [invocation invoke];

    //7、获取返回值
    id res = nil;
    if (signature.methodReturnLength ==0) return nil;
    //getReturnValue获取返回值
    [invocation getReturnValue:&res];
    if(nil==res){
        return 0;
    } else {
        int newId = g_NextId++;
        g_Dict[newId] = res;
        return newId;
    }
}

int CallInstance(int objId, const char* method, ...)
{
    ID self = g_Dict[objId];
    NSString* _method = [NSString stringWithCString:method];

    CLASS cls = NSClassFromString(_class);
    SEL selector = NSSelectorFromString(_method);
    IMP imp = [self methodForSelector:selector];
    void (*func)(id, SEL) = (void *)imp;
    func(self, selector);
    
    //1、创建签名对象
    NSMethodSignature*signature = [[self class] instanceMethodSignatureForSelector:selector];
    //2、判断传入的方法是否存在
    if (!signature) {
        //传入的方法不存在 就抛异常
        NSString*info = [NSString stringWithFormat:@"-[%@ %@]:unrecognized selector sent to instance",[self class],NSStringFromSelector(aSelector)];
        @throw [[NSException alloc] initWithName:@"方法没有" reason:info userInfo:nil];
        return nil;
    }
    //3、创建NSInvocation对象
    NSInvocation*invocation = [NSInvocation invocationWithMethodSignature:signature];
    //4、保存方法所属的对象
    invocation.target = self;           // index 0
    invocation.selector = selector;    // index 1

    //5、设置参数
    NSUInteger count = [signature numberOfArguments];
    for (int index = 0; index < num && index + 2 < count; ++index) {
        id tmp = args[index];
        if([tmp isKindOfClass:[NSNull class]])
            tmp = nil;
        [invocation setArgument:&tmp atIndex:index];
    }

    //6、调用NSinvocation对象
    [invocation invoke];

    //7、获取返回值
    id res = nil;
    if (signature.methodReturnLength ==0) return nil;
    //getReturnValue获取返回值
    [invocation getReturnValue:&res];
    if(nil==res){
        return 0;
    } else {
        int newId = g_NextId++;
        g_Dict[newId] = res;
        return newId;
    }
}