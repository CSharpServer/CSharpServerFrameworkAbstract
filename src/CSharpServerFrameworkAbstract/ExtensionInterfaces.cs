using System;
using CSharpServerFramework.Extension;
using CSharpServerFramework.Message;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace CSharpServerFramework
{
    public interface IUseExtension
    {
        void UseExtension(ICSharpServerExtension extension);
    }

    public interface IExtensionInit
    {
        IEnumerable<ExtensionCommand> LoadCommand();
    }

    public interface IExtensionLog
    {
        void Log(string msg);
    }

    public abstract class ExtensionBaseEx : IExtensionInit,ICSharpServerExtension, IExtensionMessageRedirect,IExtensionLog,IExtensionSessionManage
    {
        public object ServerExtensionBase;

        public ExtensionBaseEx(IDeserializeMessage deserializer)
        {
            MessageDecompressor = deserializer;
            ExtensionName = GetExtensionName();
        }

        public string ExtensionName
        {
            get; private set;
        }

        public IDeserializeMessage MessageDecompressor
        {
            get;private set;
        }

        private string GetExtensionName()
        {
            var extensionAttr = this.GetType().GetTypeInfo().GetCustomAttribute<ExtensionInfoAttribute>();
            if (extensionAttr == null)
            {
                throw new ExtensionNameException("Extension Is No Name Attribute:" + this.GetType().ToString());
            }
            else
            {
                ExtensionName = extensionAttr.Name;
            }
            return ExtensionName;
        }

        public IEnumerable<ExtensionCommand> LoadCommand()
        {
            var methods = this.GetType().GetMethods();
            int i = 0;
            List<ExtensionCommand> commands = new List<ExtensionCommand>();
            foreach (var method in methods)
            {
                
                var attr = method.GetCustomAttribute<CommandInfoAttribute>();
                if (attr != null)
                {
                    string commandName = attr.CommandName;
                    if (string.IsNullOrWhiteSpace(commandName))
                    {
                        commandName = ExtensionName + "Command" + attr.CommandId;
                    }
                    ExtensionCommand command = new ExtensionCommand(method, attr);
                    commands.Add(command);
                }
                i++;
            }
            return commands;
        }

        public void SendResponse(ICSharpServerSession session, SendMessage message)
        {
            ((ICSharpServerExtension)ServerExtensionBase).SendResponse(session, message);
        }

        public void SendResponseToUsers(IEnumerable<ICSharpServerUser> users, SendMessage message)
        {
            ((ICSharpServerExtension)ServerExtensionBase).SendResponseToUsers(users, message);
        }

        public bool RedirectMessage(string extensionName, int commandId, ICSharpServerSession session, dynamic msg)
        {
            return ((IExtensionMessageRedirect)ServerExtensionBase).RedirectMessage(extensionName, commandId, session, msg);
        }

        public bool RedirectMessage(string extensionName, string commandName, ICSharpServerSession session, dynamic msg)
        {
            return ((IExtensionMessageRedirect)ServerExtensionBase).RedirectMessage(extensionName, commandName, session, msg);
        }

        public abstract void Init();

        public void Log(string msg)
        {
            ((IExtensionLog)ServerExtensionBase).Log(msg);
        }

        public void CloseSession(ICSharpServerSession session)
        {
            ((IExtensionSessionManage)ServerExtensionBase).CloseSession(session);
        }
    }

    public interface ICSharpServerExtension
    {
        string ExtensionName { get; }
        IDeserializeMessage MessageDecompressor { get; }
        void SendResponse(ICSharpServerSession session, SendMessage message);
        void SendResponseToUsers(IEnumerable<ICSharpServerUser> users, SendMessage message);
    }

    public interface IExtensionMessageRedirect
    {
        bool RedirectMessage(string extensionName, int commandId, ICSharpServerSession session, dynamic msg);
        bool RedirectMessage(string extensionName, string commandName, ICSharpServerSession session, dynamic msg);
    }

    public interface IExtensionSessionManage
    {
        void CloseSession(ICSharpServerSession session);
    }

    public class ExtensionException : Exception
    {
        public ExtensionException() { }
        public ExtensionException(string message) : base(message) { }
        public ExtensionException(string message, Exception inner) : base(message, inner) { }
    }

    public class ExtensionNameException : Exception
    {
        public ExtensionNameException() { }
        public ExtensionNameException(string message) : base(message) { }
        public ExtensionNameException(string message, Exception inner) : base(message, inner) { }
    }
}
