using System;
using System.Linq;
using Autofac;
using CoDHVKDecompiler.Common;
using CoDHVKDecompiler.Decompiler;

namespace CoDHVKDecompiler.CLI
{
    public class Startup
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("CoD Havok Decompiler made from JariK's CoDHVKDecompiler");

            // setup dependency injection
            var builder = new ContainerBuilder();

            // CoDHavokTool.Common
            builder.Register((context, parameters) =>
            {
                if (parameters.Count() != 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return LuaFileFactory.Create(parameters.Positional<string>(0));
            }).As<ILuaFile>().InstancePerLifetimeScope();
            
            // CoDHavokTool.LuaDecompiler
            builder.RegisterType<Decompiler.Decompiler>().As<IDecompiler>().SingleInstance();

            // CodHavokTool
            builder.RegisterType<Program>().SingleInstance();


            var container = builder.Build();
            container.Resolve<Program>().Main(args);
        }
    }
}