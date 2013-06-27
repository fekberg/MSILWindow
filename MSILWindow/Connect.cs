using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using Extensibility;
using EnvDTE;
using EnvDTE80;

namespace MSILWindow
{
    /// <summary>The object for implementing an Add-in.</summary>
    /// <seealso class='IDTExtensibility2' />
    public class Connect : IDTExtensibility2
    {
        private Events events;
        private BuildEvents buildEvents;
        private TextEditorEvents textEditorEvents;

        private EnvDTE80.Windows2 toolWins;
        private EnvDTE.Window toolWin;
        private DTE2 applicationObject;
        private AddIn addInInstance;

        private static ICommandService serviceProxy;
        private static readonly string pipeName = "UserControl1Service";
        private static readonly Uri serviceUri = new Uri("net.pipe://localhost/Pipe");
        private static readonly EndpointAddress serviceAddress = new EndpointAddress(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", serviceUri.OriginalString, pipeName));
        
        /// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
        public Connect()
        {
        }

        /// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
        /// <param term='application'>Root object of the host application.</param>
        /// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
        /// <param term='addInInst'>Object representing this Add-in.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {
            try
            {
                applicationObject = (DTE2)application;
                addInInstance = (AddIn)addInInst;

                object tempObject = null;

                var controlName = "MSILWindow.Window.WindowControl";

                // Change this to your path
                var assemblyPath = @"C:\Users\filip.ekberg\Documents\GitHub\MSILWindow\MSILWindow.Window\bin\Debug\MSILWindow.Window.dll";
                var controlGuid = "{6d0f6084-69ef-4100-92c5-5e7e3a557e05}";

                toolWins = (Windows2)applicationObject.Windows;

                toolWin = toolWins.CreateToolWindow2(addInInstance,
                    assemblyPath, controlName, "Real-time MSIL Viewer", controlGuid,
                    ref tempObject);

                if (toolWin != null)
                {
                    toolWin.Visible = true;
                }

                events = applicationObject.Events;
                buildEvents = events.BuildEvents;
                buildEvents.OnBuildDone += BuildEvents_OnBuildDone;
                textEditorEvents = events.get_TextEditorEvents();
                textEditorEvents.LineChanged += Connect_LineChanged;

                serviceProxy = ChannelFactory<ICommandService>.CreateChannel(new NetNamedPipeBinding(), serviceAddress);
            }
            catch { }
        }

        private void BuildEvents_OnBuildDone(vsBuildScope scope, vsBuildAction action)
        {
            // Process the MSIL on each build
            try
            {
                Process(((TextSelection)applicationObject.ActiveDocument.ActiveWindow.Document.Selection).ActivePoint, ((TextSelection)applicationObject.ActiveDocument.ActiveWindow.Document.Selection).ActivePoint);
            }
            catch { }
        }
        private void Connect_LineChanged(TextPoint startPoint, TextPoint endPoint, int hint)
        {
            Process(startPoint, endPoint);
        }

        private void Process(TextPoint startPoint, TextPoint endPoint)
        {
            try
            {
                var result = "";

                var methodName = GetMethodName(startPoint, endPoint);
                var className = GetClassName(startPoint, endPoint);

                result = string.Format("Processing method {0} in class {1}{2}{3}{2}{2}", methodName, className, Environment.NewLine, "-----------------------------------");

                serviceProxy.Execute(result);

                var executable = GetProjectExecutable(applicationObject.ActiveDocument.ProjectItem.ContainingProject, applicationObject.ActiveDocument.ProjectItem.ContainingProject.ConfigurationManager.ActiveConfiguration);

                if (!File.Exists(executable)) return;
                try
                {
                    AppDomainSetup appDomainSetup = new AppDomainSetup();
                    string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    appDomainSetup.ApplicationBase = Path.GetFullPath(path);
                    var ad = AppDomain.CreateDomain("New Domain", null, appDomainSetup);
                    AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

                    var type = typeof(AssemblyProxy);
                    var proxy = (AssemblyProxy)ad.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
                    result += proxy.LoadInfoFromAssembly(executable, className, methodName);

                    AppDomain.Unload(ad);
                    serviceProxy.Execute(result);
                }
                catch
                {
                }
            }
            catch { }
        }
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                Assembly assembly = System.Reflection.Assembly.Load(args.Name);
                if (assembly != null)
                    return assembly;
                string[] Parts = args.Name.Split(',');
                string File = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + Parts[0].Trim() + ".exe";

                return System.Reflection.Assembly.LoadFrom(File);
            }
            catch { }

            return null;
        }

        public void OnAddInsUpdate(ref Array custom)
        {
        }
        public void OnBeginShutdown(ref Array custom)
        {
        }
        public void OnDisconnection(ext_DisconnectMode removeMode, ref Array custom)
        {
        }
        public void OnStartupComplete(ref Array custom)
        {
        }
        private static string GetProjectExecutable(Project startupProject, Configuration config)
        {
            try
            {
                string projectFolder = Path.GetDirectoryName(startupProject.FileName);
                string outputPath = (string)config.Properties.Item("OutputPath").Value;
                string assemblyFileName = (string)startupProject.Properties.Item("AssemblyName").Value + ".exe";
                return Path.Combine(new[] {
                                      projectFolder,
                                      outputPath,
                                      assemblyFileName
                                  });
            }
            catch { }

            return "";
        }
        private string GetName(TextPoint startPoint, TextPoint endPoint, vsCMElement vsCMElement)
        {
            try
            {
                var element = startPoint.get_CodeElement(vsCMElement);
                TextPoint start = element.GetStartPoint();
                TextPoint end = element.GetEndPoint();
                string raw = start.CreateEditPoint().GetText(end);

                return raw;
            }
            catch { }

            return "";
        }
        private string GetMethodName(TextPoint startPoint, TextPoint endPoint)
        {
            try
            {
                var methodRawText = GetName(startPoint, endPoint, vsCMElement.vsCMElementFunction);
                var methodName = methodRawText.Split(Environment.NewLine.ToCharArray()).FirstOrDefault();
                var paranthesisIndex = methodName.IndexOf('(');
                methodName = methodName.Substring(0, paranthesisIndex);
                methodName = methodName.Split(' ').LastOrDefault();

                return methodName;
            }
            catch { }

            return "";
        }
        private string GetClassName(TextPoint startPoint, TextPoint endPoint)
        {
            try
            {
                var classRawText = GetName(startPoint, endPoint, vsCMElement.vsCMElementClass);

                var classRow = classRawText.Split(Environment.NewLine.ToCharArray()).FirstOrDefault();
                var classSegments = classRow.Split(':');
                classSegments = (classSegments.Length == 0) ? classRow.Split(' ') : classSegments.FirstOrDefault().Split(' ');

                var className = classSegments.LastOrDefault();

                return className;
            }
            catch { }

            return "";
        }
    }
}