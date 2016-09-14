﻿using System;
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

        public ExtensionBaseEx(IDeserializeMessage Deserializer)
        {
            MessageDecompressor = Deserializer;
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

        public void SendResponse(ICSharpServerSession Session, SendMessage Message)
        {
            ((ICSharpServerExtension)ServerExtensionBase).SendResponse(Session, Message);
        }

        public void SendResponseToUsers(IEnumerable<ICSharpServerUser> Users, SendMessage Message)
        {
            ((ICSharpServerExtension)ServerExtensionBase).SendResponseToUsers(Users, Message);
        }

        public bool RedirectMessage(string ExtensionName, int CommandId, ICSharpServerSession Session, dynamic msg)
        {
            return ((IExtensionMessageRedirect)ServerExtensionBase).RedirectMessage(ExtensionName, CommandId, Session, msg);
        }

        public bool RedirectMessage(string ExtensionName, string CommandName, ICSharpServerSession Session, dynamic msg)
        {
            return ((IExtensionMessageRedirect)ServerExtensionBase).RedirectMessage(ExtensionName, CommandName, Session, msg);
        }

        public abstract void Init();

        public void Log(string msg)
        {
            ((IExtensionLog)ServerExtensionBase).Log(msg);
        }

        public void CloseSession(ICSharpServerSession Session)
        {
            ((IExtensionSessionManage)ServerExtensionBase).CloseSession(Session);
        }
    }

    public interface ICSharpServerExtension
    {
        string ExtensionName { get; }
        IDeserializeMessage MessageDecompressor { get; }
        void SendResponse(ICSharpServerSession Session, SendMessage Message);
        void SendResponseToUsers(IEnumerable<ICSharpServerUser> Users, SendMessage Message);
    }

    public interface IExtensionMessageRedirect
    {
        bool RedirectMessage(string ExtensionName, int CommandId, ICSharpServerSession Session, dynamic msg);
        bool RedirectMessage(string ExtensionName, string CommandName, ICSharpServerSession Session, dynamic msg);
    }

    public interface IExtensionSessionManage
    {
        void CloseSession(ICSharpServerSession Session);
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
