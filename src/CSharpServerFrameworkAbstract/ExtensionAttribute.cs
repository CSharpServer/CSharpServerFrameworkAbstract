using System;

namespace CSharpServerFramework.Extension
{
    /// <summary>
    /// 作用于ExtensionBase类的Attribute，指定Extension的名，必须保证ExtensionLoader返回的所有Extension的Name不一样
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ExtensionInfoAttribute : Attribute
    {
        readonly string name;

        public ExtensionInfoAttribute(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }

    }

    /// <summary>
    /// 把Extension 指定为Validation类型
    /// Session认证前所有的请求都会转发到包含该标签的Extension
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ValidateExtensionAttribute : Attribute
    {
    }

    /// <summary>
    /// Extension的命令Attribute，用于指定Extension子类的方法的命令Id，该Attribute只能作用于方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]   
    public sealed class CommandInfoAttribute : Attribute
    {
        readonly int Id;
        readonly string name;
        readonly bool isAsyncInvoke;
        readonly bool isOrderCommand;
        readonly bool isAcceptRawData;

        /// <summary>
        /// 特征构造
        /// isAcceptRawData为true，方法定义为 void fun(ICSharpServerSession session, byte[] buffer)，buffer 为Client发送的完整数据包（包括包头，route信息）
        /// isAcceptRawData为false，方法定义为 void fun(ICSharpServerSession session, dynamic msg)
        /// </summary>
        /// <param name="commandId">分配的Id，同一个Extension里CommandId必须不同</param>
        /// <param name="name">命令名，要求同上</param>
        /// <param name="asyncInvoke">指定Command是否使用异步方式调用，默认使用同方式调用。如果Extension子类的方法存在死循环或方法阻塞时间长，请设置改值为真，改为异步调用</param>
        /// <param name="orderCommand">有顺序的命令，先请求的先响应,AsyncInvoke参数必须是false有效</param>
        /// <param name="isAcceptRawData">方法是否接受byte[]类型的参数</param>
        public CommandInfoAttribute(int commandId, string name = null, bool asyncInvoke = true, bool orderCommand = false,bool isAcceptRawData = false)
        {
            this.Id = commandId;
            this.name = name;
            this.isAsyncInvoke = asyncInvoke;
            this.isOrderCommand = orderCommand;
            this.isAcceptRawData = isAcceptRawData;
        }

        /// <summary>
        /// 特征构造
        /// isAcceptRawData为true，方法定义为 void fun(ICSharpServerSession session, byte[] buffer)，buffer 为Client发送的完整数据包（包括包头，route信息）
        /// isAcceptRawData为false，方法定义为 void fun(ICSharpServerSession session, dynamic msg)
        /// </summary>
        /// <param name="commandId">分配的Id，同一个Extension里CommandId必须不同</param>
        /// <param name="asyncInvoke">指定Command是否使用异步方式调用，默认使用同方式调用。如果Extension子类的方法存在死循环或方法阻塞时间长，请设置改值为真，改为异步调用</param>
        /// <param name="orderCommand">有顺序的命令，先请求的先响应,AsyncInvoke参数必须是false有效</param>
        /// <param name="isAcceptRawData">方法是否接受byte[]类型的参数</param>
        public CommandInfoAttribute(int commandId, bool asyncInvoke = true, bool orderCommand = false, bool isAcceptRawData = false) :
            this(commandId,null,asyncInvoke,orderCommand, isAcceptRawData)
        {
        }

        /// <summary>
        /// 特征构造
        /// isAcceptRawData为true，方法定义为 void fun(ICSharpServerSession session, byte[] buffer)，buffer 为Client发送的完整数据包（包括包头，route信息）
        /// isAcceptRawData为false，方法定义为 void fun(ICSharpServerSession session, dynamic msg)
        /// </summary>
        /// <param name="name">命令名，同一个Extension里CommandId必须不同</param>
        /// <param name="asyncInvoke">指定Command是否使用异步方式调用，默认使用同方式调用。如果Extension子类的方法存在死循环或方法阻塞时间长，请设置改值为真，改为异步调用</param>
        /// <param name="orderCommand">有顺序的命令，先请求的先响应,AsyncInvoke参数必须是false有效</param>
        /// <param name="isAcceptRawData">方法是否接受byte[]类型的参数</param>
        public CommandInfoAttribute(string name, bool asyncInvoke = true, bool orderCommand = false, bool isAcceptRawData = false):
            this(-1,name,asyncInvoke,orderCommand,isAcceptRawData)
        {
        }

        public bool IsOrderCommand { get { return isOrderCommand; } }

        public bool IsAsyncInvoke
        {
            get { return isAsyncInvoke; }
        } 

        public int CommandId
        {
            get { return Id; }
        }
        public string CommandName
        {
            get { return name; }
        }

        public bool IsAcceptRawDataCommand
        {
            get { return isAcceptRawData; }
        }
    }
}
