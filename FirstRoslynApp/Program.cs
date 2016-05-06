using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using Nito.AsyncEx;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FirstRoslynApp
{
    class Program
    {
        private static List<EndPoint> _methods;
        private static string _iService = "IService.cs";
        private static string _serviceReference = "ServiceReference.cs";
        private static string _serviceLink = "http://localhost:41310/Service1.svc";

        public Program(List<EndPoint>  methods)
        {
            _methods = methods;
        }

        public Program() : base()
        {
        }

        static void Main(string[] args)
        {
            var projectName = "Bleu.Web";

            var sb = new StringBuilder();
            string solutionPath = @"C:\Users\denis.ev\Documents\visual studio 2015\Projects\FirstRoslynApp\FirstRoslynApp.sln";
            var msWorkspace = MSBuildWorkspace.Create();
            var solution = msWorkspace.OpenSolutionAsync(solutionPath).Result;

            var svc = GetService(solution);

            if (svc != null)
            {
                var svcName = svc.Name.Remove(svc.Name.IndexOf(".", StringComparison.Ordinal));

                GenerateBaseFiles(projectName);
                GenerateInterfaceAndChannel(projectName);
                GenerateCompletedEventArgs(projectName);
                GenerateServiceClient(svcName, projectName);

                //AsyncContext.Run(() => CreateDocument(sb.ToString(), projectName, "Service References/TestFile.cs"));
                //Console.WriteLine(sb);
            }

            Console.ReadLine();
        }

        private static Document GetService(Solution solution)
        {
            _methods = new List<EndPoint>();
            Document svc = null;

            foreach (var project in solution.Projects)
            {
                foreach (var document in project.Documents)
                {
                    if (document.Name == _iService)
                    {
                        svc = document;
                    }
                }
            }

            if (svc != null)
            {
                var syntaxTree = svc.GetSyntaxTreeAsync().Result;

                var syntaxRoot = syntaxTree.GetRoot();
                var syntaxMethods = syntaxRoot.DescendantNodes().OfType<MethodDeclarationSyntax>();

                _methods.AddRange(syntaxMethods.Select(sm => new EndPoint()
                {
                    Name = sm.Identifier.ToString(),
                    ReturnType = sm.ReturnType,
                    ParametersList = sm.ParameterList,
                }));

                //foreach (var method in _methods)
                //{
                //    Console.WriteLine(method.ReturnType + "  :  " + method.Parameters + "(" + method.Parameters.Count + ")");
                //}

                return svc;
            }

            Console.WriteLine("Service wsan't found");

            return null;
        }

        private static async void CreateDocument(string code, string projectName, string fileName )
        {
            if (fileName != null && projectName != null)
            {
                var folders = fileName.Split('/');
                fileName = folders[folders.Length - 1];
                folders = folders.Where((val, idx) => idx != folders.Length - 1).ToArray();

                string solutionPath = @"C:\Users\denis.ev\Documents\Demo\Bleu.Web.sln";
                var workspace = MSBuildWorkspace.Create();
                await workspace.OpenSolutionAsync(solutionPath);

                var sourceText = SourceText.From(code);

                var originalSolution = workspace.CurrentSolution;
                var project = originalSolution.GetProject(originalSolution.Projects.First(x => x.AssemblyName == projectName).Id);
                var result = project.AddDocument(fileName, sourceText, folders);

                workspace.TryApplyChanges(result.Project.Solution);
            }
        }

        private static void GenerateBaseFiles(string projectName)
        {
            var sb = new StringBuilder();
            var serviceReference = _serviceReference.Remove(_serviceReference.IndexOf(".", StringComparison.Ordinal));
            var iService = _iService.Remove(_iService.IndexOf(".", StringComparison.Ordinal));

            #region Create ChannelContainer 

            sb.Append("using System; \n\n");
            sb.Append("namespace " + projectName + "\n");
            sb.Append("{\n\t public class ChannelContainer<TClient> : IDisposable\n");
            sb.Append("\t{ \n ");

            sb.Append("\t\t private readonly TClient _client; \n\n");
            sb.Append("\t\t public ChannelContainer(TClient client)\n");
            sb.Append("\t\t { \n \t\t\t _client = client;  \n \t\t } \n\n");

            sb.Append("\t\t public TClient Client\n");
            sb.Append("\t\t { \n \t\t\t get \n \t\t\t {\n \t\t\t\t return _client;  \n \t\t\t } \n \t\t } \n\n");

            sb.Append("\t\t public string Address { get; set; }\n\n");
            sb.Append("\t\t public event EventHandler Disposing;\n\n");

            sb.Append("\t\t public void Dispose()\n \t\t { \n \t\t\t var dispose = Disposing; \n \t\t\t if (dispose != null) dispose(this, new EventArgs()); \n");
            sb.Append("\t\t } \n \t } \n } ");

            CreateDocument(sb.ToString(), projectName, "ChannelContainer.cs");

            #endregion

            #region Create ClientFactory 

            sb.Clear();

            sb.Append("using System; \n");
            sb.Append("using System.Collections.Concurrent; \n");
            sb.Append("using " + projectName + "." + serviceReference + "\n\n");
            sb.Append("namespace " + projectName + "\n");
            sb.Append("{\n\t public static class ClientFactory<TClient> where TClient : class, " + iService + ", new()\n \t { \n");
            sb.Append("\t\t private static ConcurrentDictionary<string, ConcurrentBag<TClient>> FreeChannelsChannels { get; set; } \n");
            sb.Append("\t\t private static ConcurrentDictionary<int, ChannelContainer<TClient>> UsedChannels { get; set; } \n\n");

            sb.Append("\t\t static ClientFactory() \n\t\t { \n");
            sb.Append("\t\t\t FreeChannelsChannels = new ConcurrentDictionary<string, ConcurrentBag<TClient>>();\n");
            sb.Append("\t\t\t UsedChannels = new ConcurrentDictionary<int, ChannelContainer<TClient>>();\n \t\t } \n\n");

            sb.Append("\t\t public static ChannelContainer<TClient> CreateClient(string address)\n \t\t { \n");
            sb.Append("\t\t\t ConcurrentBag<TClient> currentChannels = GetFreeChannels(address); \n\n");
            sb.Append("\t\t\t TClient client = null; \n\n");

            sb.Append("\t\t\t for (int i=0; currentChannels.Count > 0  && client == null && i< 10; i++ ) \n \t\t\t { \n");
            sb.Append("\t\t\t\t currentChannels.TryTake(out client); \n \t\t\t } \n");

            sb.Append("\t\t\t client = client ?? new TClient(); \n\n");
            sb.Append("\t\t\t var container = new ChannelContainer<TClient>(client) { Address = address }; \n\n");

            sb.Append("\t\t\t UsedChannels.TryAdd(container.GetHashCode(), container); \n");
            sb.Append("\t\t\t container.Disposing += ContainerOnDisposing; \n\n");
            
            sb.Append("\t\t\t return container; \n \t\t } \n\n");

            sb.Append("\t\t public static ConcurrentBag<TClient> GetFreeChannels(string address) \n \t\t { \n");
            sb.Append("\t\t\t return FreeChannelsChannels.GetOrAdd(address, arg => new ConcurrentBag<TClient>()); \n \t\t } \n\n");

            sb.Append("\t\t private static void ContainerOnDisposing(object sender, EventArgs eventArgs) \n \t\t { \n");
            sb.Append("\t\t\t var container = (ChannelContainer<TClient>) sender;\n");
            sb.Append("\t\t\t container.Disposing -= ContainerOnDisposing; \n\n");
            sb.Append("\t\t\t UsedChannels.TryRemove(container.GetHashCode(), out container); \n\n");
            sb.Append("\t\t\t if(!container.Client.IsCaughtException) \n \t\t\t { \n");
            sb.Append("\t\t\t\t var freeChannels = GetFreeChannels(container.Address); \n");
            sb.Append("\t\t\t\t freeChannels.Add(container.Client); \n \t\t\t } else \n \t\t\t { \n ");
            sb.Append("\t\t\t\t ((IDisposable)container.Client).Dispose(); \n ");
            sb.Append("\t\t\t\t System.Diagnostics.Debug.WriteLine(\"Client has an exception\"); \n \t\t\t }\n");
            sb.Append("\t\t } \n \t } \n }");

            CreateDocument(sb.ToString(), projectName, "ClientFactory.cs");

            #endregion

            #region Create FlowOperationContextScope 

            sb.Clear();

            sb.Append("using System; \n");
            sb.Append("using System.ServiceModel; \n\n");
            sb.Append("namespace " + projectName + "\n");
            sb.Append("{\n\t public sealed class FlowOperationContextScope : IDisposable\n");
            sb.Append("\t { \n");
            sb.Append("\t\t private bool _inFlight; \n");
            sb.Append("\t\t private bool _disposed; \n");
            sb.Append("\t\t private OperationContext _thisContext; \n");
            sb.Append("\t\t private OperationContext _originalContext; \n\n");

            sb.Append("\t\t public FlowOperationContextScope(IContextChannel channel)\n \t\t\t: this(new OperationContext(channel)) {} \n\n");
            sb.Append("\t\t public FlowOperationContextScope(OperationContext context)\n \t\t { \n \t\t\t _originalContext = OperationContext.Current; \n");
            sb.Append("\t\t\t OperationContext.Current = _thisContext = context;\n \t\t } \n\n");

            sb.Append("\t\t public void Dispose()\n \t\t { \n \t\t\t if (_disposed) return; \n");
            sb.Append("\t\t\t if (_inFlight || OperationContext.Current != _thisContext)\n \t\t\t { \n \t\t\t\t throw new InvalidOperationException();\n \t\t\t } \n");
            sb.Append("\t\t\t _disposed = true; \n");
            sb.Append("\t\t\t OperationContext.Current = _originalContext; \n");
            sb.Append("\t\t\t _thisContext = null; \n");
            sb.Append("\t\t\t _originalContext = null; \n \t\t } \n\n");

            sb.Append("\t\t internal void BeforeAwait() \n");
            sb.Append("\t\t { \n");
            sb.Append("\t\t\t if (_inFlight) \n");
            sb.Append("\t\t\t { \n \t\t\t\t return; \n \t\t\t } \n");
            sb.Append("\t\t\t _inFlight = true; \n");
            sb.Append("\t\t } \n\n");

            sb.Append("\t\t internal void AfterAwait() \n");
            sb.Append("\t\t { \n");
            sb.Append("\t\t\t if (!_inFlight) \n");
            sb.Append("\t\t\t { \n ");
            sb.Append("\t\t\t\t throw new InvalidOperationException(); \n ");
            sb.Append("\t\t\t } \n ");
            sb.Append("\t\t\t _inFlight = false; \n");
            sb.Append("\t\t\t OperationContext.Current = _thisContext; \n");
            sb.Append("\t\t } \n \t } \n }");

            CreateDocument(sb.ToString(), projectName, "FlowOperationContextScope.cs");
            #endregion

            #region Create SimpleAwaiter

            sb.Clear();

            sb.Append("using System;");
            sb.Append("\n using System.Threading;");
            sb.Append("\n using System.Threading.Tasks;");
            sb.Append("\n using System.Runtime.CompilerServices;");
            sb.Append("\n\n namespace " + projectName);
            sb.Append("\n {\n\t public class SimpleAwaiter : INotifyCompletion");
            sb.Append("\n\t {");

            sb.Append("\n\t\t #region Fields\n");
            sb.Append("\n\t\t protected readonly Task task;");
            sb.Append("\n\t\t protected readonly Action beforeAwait;");
            sb.Append("\n\t\t protected readonly Action afterAwait;");
            sb.Append("\n\n\t\t #endregion\n");

            sb.Append("\n\t\t public SimpleAwaiter(Task task, Action beforeAwait, Action afterAwait");
            sb.Append("\n\t\t {");
            sb.Append("\n\t\t\t this.task = task;");
            sb.Append("\n\t\t\t this.beforeAwait = beforeAwait;");
            sb.Append("\n\t\t\t this.afterAwait = afterAwait;");
            sb.Append("\n\t\t }\n");

            sb.Append("\n\t\t public SimpleAwaiter GetAwaiter()");
            sb.Append("\n\t\t {");
            sb.Append("\n\t\t\t return this;");
            sb.Append("\n\t\t }\n");

            sb.Append("\n\t\t public void GetResult()");
            sb.Append("\n\t\t {");
            sb.Append("\n\t\t\t // FUCK YEAH! DO NOT REMOVE!");
            sb.Append("\n\t\t\t task.GetAwaiter().GetResult();");
            sb.Append("\n\t\t }\n");

            sb.Append("\n\t\t public bool IsCompleted");
            sb.Append("\n\t\t {");
            sb.Append("\n\t\t\t get \n\t\t\t {");
            sb.Append("\n\t\t\t\t // don't do anything if the task completed synchronously");
            sb.Append("\n\t\t\t\t // (we're on the same thread)");
            sb.Append("\n\t\t\t\t if (task.IsCompleted)");
            sb.Append("\n\t\t\t\t {");
            sb.Append("\n\t\t\t\t\t return true;");
            sb.Append("\n\t\t\t\t }");
            sb.Append("\n\t\t\t\t beforeAwait();");
            sb.Append("\n\t\t\t\t return false;");
            sb.Append("\n\t\t\t }");
            sb.Append("\n\t\t }\n");

            sb.Append("\n\t\t public void OnCompleted(Action continuation)");
            sb.Append("\n\t\t {");
            sb.Append("\n\t\t\t task.ContinueWith(t =>");
            sb.Append("\n\t\t\t { ");
            sb.Append("\n\t\t\t\t afterAwait();");
            sb.Append("\n\t\t\t\t continuation();");
            sb.Append("\n\t\t\t },");
            sb.Append("\n\t\t\t CancellationToken.None,");
            sb.Append("\n\t\t\t TaskContinuationOptions.ExecuteSynchronously,");
            sb.Append("\n\t\t\t SynchronizationContext.Current != null");
            sb.Append("\n\t\t\t\t ? TaskScheduler.FromCurrentSynchronizationContext()");
            sb.Append("\n\t\t\t\t : TaskScheduler.Current);");
            sb.Append("\n\t\t}\n\t }\n");
            sb.Append("\n\t public class SimpleAwaiter<TResult> : SimpleAwaiter");
            sb.Append("\n\t {\n\t\t #region Fields\n");
            sb.Append("\n\t\t private readonly Task<TResult> _task;");
            sb.Append("\n\n\t\t #endregion\n");
            sb.Append("\n\t\t public SimpleAwaiter(Task<TResult> task, Action beforeAwait, Action afterAwait)");
            sb.Append("\n\t\t\t : base(task, beforeAwait, afterAwait)");
            sb.Append("\n\t\t {");
            sb.Append("\n\t\t\t _task = task;");

            sb.Append("\n\t\t }");
            sb.Append("\n\n\t\t public new SimpleAwaiter<TResult> GetAwaiter()");
            sb.Append("\n\t\t {");
            sb.Append("\n\t\t\t return this;");
            sb.Append("\n\t\t }\n");
            sb.Append("\n\t\t public new TResult GetResult()");
            sb.Append("\n\t\t {");
            sb.Append("\n\t\t\t return _task.Result;");

            sb.Append("\n\t\t }\n\t }\n}\n");

            CreateDocument(sb.ToString(), projectName, "SimpleAwaiter.cs");

            #endregion

            #region Create TaskExt 

            sb.Clear();

            sb.Append("using System.Threading.Tasks;\n\n");
            sb.Append("namespace " + projectName + "\n");
            sb.Append("{\n\t public static class TaskExt\n");
            sb.Append("\t {\n");
            sb.Append("\t\t public static SimpleAwaiter<TResult> ContinueOnScope<TResult>(this Task<TResult> @this, FlowOperationContextScope scope) \n");
            sb.Append("\t\t {\n");
            sb.Append("\t\t\t return new SimpleAwaiter<TResult>(@this, scope.BeforeAwait, scope.AfterAwait);\n");
            sb.Append("\t\t }\n\n");
            sb.Append("\t\t public static SimpleAwaiter ContinueOnScope(this Task @this, FlowOperationContextScope scope) \n");
            sb.Append("\t\t {\n");
            sb.Append("\t\t\t return new SimpleAwaiter(@this, scope.BeforeAwait, scope.AfterAwait);\n");
            sb.Append("\t\t }\n\t }\n}");

            CreateDocument(sb.ToString(), projectName, "TaskExt.cs");

            #endregion
        }

        private static void GenerateInterfaceAndChannel(string projectName)
        {
            var sb = new StringBuilder();
            var iService = _iService.Remove(_iService.IndexOf(".", StringComparison.Ordinal));

            sb.Append("using System;\n\n");
            sb.Append("namespace " + projectName + "\n { \n");
            sb.Append("\t [System.CodeDom.Compiler.GeneratedCodeAttribute(\"System.ServiceModel\", \"4.0.0.0\")]\n");
            sb.Append("\t [System.ServiceModel.ServiceContractAttribute(ConfigurationName = \""+ iService + "\")]\n");
            sb.Append("\t public interface " + iService + "\n \t{ \n");
            sb.Append("\t\t bool IsCaughtException { get; set; } \n\n");

            foreach (var method in _methods)
            {
                var parameters = method.ParametersList.ToString().Replace("(", "").Replace(")", "");

                sb.Append("\t\t [System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = \"http://tempuri.org/" + iService + "/" + method.Name + "\", ReplyAction = \"http://tempuri.org/" + iService + "/" + method.Name + "Response\")]\n");
                sb.Append("\t\t System.IAsyncResult Begin" + method.Name + "(" + parameters != "" ? parameters + ", " : "" + "System.AsyncCallback callback, object asyncState);\n\n");
                sb.Append("\t\t " + method.ReturnType + " End" + method.Name + "(System.IAsyncResult result);\n\n");
            }

            sb.Append("\t } \n\n");
            sb.Append("\t [System.CodeDom.Compiler.GeneratedCodeAttribute(\"System.ServiceModel\", \"4.0.0.0\")]\n");
            sb.Append("\t public interface " + iService + "Channel : " + iService + ", System.ServiceModel.IClientChannel{}");

            CreateDocument(sb.ToString(), projectName, "Service References/"+ iService + ".cs");
        }

        private static void GenerateCompletedEventArgs(string projectName)
        {
            var sb = new StringBuilder();

            foreach (var method in _methods)
            {
                if (method.ReturnType.ToString() != "void")
                {
                    sb.Clear();

                    sb.Append(" using System; \n\n");
                    sb.Append(" namespace " + projectName + "\n { \n");
                    sb.Append("\t [System.CodeDom.Compiler.GeneratedCodeAttribute(\"System.ServiceModel\", \"4.0.0.0\")] \n");
                    sb.Append("\t public partial class" + method.Name + "CompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs \n");
                    sb.Append("\t { \n\n");
                    sb.Append("\t   private object[] results; \n\n");
                    sb.Append("\t   public " + method.Name + "CompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :\n");
                    sb.Append("\t   base(exception, cancelled, userState) {\n");
                    sb.Append("\t       this.results = results;\n");
                    sb.Append("\t   } \n\n");
                    sb.Append("\t   public " + method.ReturnType + " Result { \n");
                    sb.Append("\t       get {\n");
                    sb.Append("\t           base.RaiseExceptionIfNecessary();\n");
                    sb.Append("\t           return ((" + method.ReturnType + ")(this.results[0]));\n");
                    sb.Append("\t       }\n");
                    sb.Append("\t   }\n");
                    sb.Append("\t }\n");
                    sb.Append(" }");
                }

                CreateDocument(sb.ToString(), projectName, "Service References/CompletedEventArgs/" + method.Name + "CompletedEventArgs.cs");
            }
        }

        private static void GenerateServiceClient(string svcName, string projectName)
        {
            var sb = new StringBuilder();
            var channelName = (svcName.IndexOf("I", StringComparison.Ordinal) == 0 ? svcName.Remove(0, 1) : svcName) + "Client";

            sb.Append(" using System; \n\n");
            sb.Append(" namespace " + projectName + "\n { \n");
            sb.Append(@"\t public partial class " + channelName + " : System.ServiceModel.ClientBase<" + svcName + ">, " + svcName + "\n ");
            sb.Append("\t {");

            sb.Append(GeneratePropertiesAndConstructors());
            sb.Append(GenerateMethods(svcName));

            sb.Append(@"    }");
            sb.Append(@"}");

            CreateDocument(sb.ToString(), projectName, "Service References/" + channelName + ".cs");
        }

        private static string GeneratePropertiesAndConstructors()
        {
            var properties = " \n\t\t public bool IsCaughtException { get; set; } \n\n";

            return properties +
                   "\t\t public Service1Client() : base(Service1Client.GetDefaultBinding(), Service1Client.GetDefaultEndpointAddress()) \n\t { \n\n\t } \n\n" +
                   "\t\t public Service1Client(EndpointConfiguration endpointConfiguration) : base(Service1Client.GetBindingForEndpoint(endpointConfiguration), Service1Client.GetEndpointAddress(endpointConfiguration)) \n\t { \n\n\t } \n\n" +
                   "\t\t public Service1Client(EndpointConfiguration endpointConfiguration, string remoteAddress) : base(Service1Client.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress)) \n\t { \n\n\t } \n\n" +
                   "\t\t public Service1Client(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : base(Service1Client.GetBindingForEndpoint(endpointConfiguration), remoteAddress) \n\t { \n\n\t } \n\n" +
                   "\t\t public Service1Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(binding, remoteAddress) \n\t { \n\n\t } \n ";
        }

        private static string GenerateMethods(string client)
        {
            var result = new StringBuilder();

            result.Append("\n");

            #region Delegates And Event

            foreach (var method in _methods)
            {

                result.Append("\t\t private BeginOperationDelegate onBegin" + method.Name + "Delegate; \n");
                result.Append("\t\t private EndOperationDelegate onEnd" + method.Name + "Delegate; \n");
                result.Append("\t\t private System.Threading.SendOrPostCallback on" + method.Name + "CompletedDelegate; \n\n");

                if (method.ReturnType.ToString() == "void")
                {
                    result.Append("\t\t public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> " + method.Name + "Completed; \n\n");
                }
                else
                {
                    result.Append("\t\t public event System.EventHandler<" + method.Name + "CompletedEventArgs> " + method.Name + "Completed; \n\n");
                }
            }

            #endregion

            foreach (var method in _methods)
            {
                #region Begin-End
                result.Append("\t\t [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]\n");
                result.Append("\t\t public System.IAsyncResult Begin" + method.Name + "(" + method.ParametersList.Parameters + ",  System.AsyncCallback callback, object asyncState)\n");
                result.Append("\t\t {\n");
                result.Append("\t\t   return base.Channel.BeginGetData(");

                foreach (var parameter in method.ParametersList.Parameters)
                {
                    result.Append(parameter.Identifier + ", ");
                }

                result.Append(" callback, asyncState); \n");

                result.Append("\t\t }\n\n");

                result.Append("\t\t [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]\n");
                result.Append("\t\t public " + method.ReturnType + " End" + method.Name + "(System.IAsyncResult result)\n");
                result.Append("\t\t {\n");
                result.Append("\t\t   try\n");
                result.Append("\t\t   {\n");
                result.Append("\t\t       return base.Channel.End" + method.Name + "(result);\n");
                result.Append("\t\t   }\n");
                result.Append("\t\t   catch (Exception)\n");
                result.Append("\t\t   {\n");
                result.Append("\t\t       IsCaughtException = true;\n");
                result.Append("\t\t   }\n");
                result.Append("\t\t }\n\n");
                #endregion

                #region OnBegin-OnEnd-OnCompleted
                result.Append("\t\t private System.IAsyncResult OnBegin" + method.Name + "(object[] inValues, System.AsyncCallback callback, object asyncState)\n");
                result.Append("\t\t {\n");

                var counter = 0;
                foreach (var parameter in method.ParametersList.Parameters)
                {
                    result.Append("\t\t   " + parameter.Type + " " + parameter.Identifier + " = ((" + parameter.Type + ")(inValues[" + counter + "]));\n");
                    counter++;
                }

                result.Append("\t\t   return ((" + client + ")(this)).Begin" + method.Name + "(");

                foreach (var parameter in method.ParametersList.Parameters)
                {
                    result.Append(parameter.Identifier + ", ");
                }

                result.Append("callback, asyncState);\n");
                result.Append("\t\t }\n\n");

                result.Append("\t\t private object[] OnEnd" + method.Name + "(System.IAsyncResult result)\n");
                result.Append("\t\t {\n");
                result.Append("\t\t   string retVal = ((" + client + ")(this)).End" + method.Name + "(result);\n");
                result.Append("\t\t   return new object[] { retVal };\n");
                result.Append("\t\t }\n\n");

                result.Append("\t\t private void On" + method.Name + "Completed(object state)\n");
                result.Append("\t\t {\n");
                result.Append("\t\t   if ((this." + method.Name + "Completed != null))\n");
                result.Append("\t\t   {\n");
                result.Append("\t\t      InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));\n");
                result.Append("\t\t      this." + method.Name + "Completed(this, new " + method.Name + "CompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));\n");
                result.Append("\t\t   }\n");
                result.Append("\t\t }\n\n");
                #endregion

                #region AsyncMethod
                result.Append("\t\t public void " + method.Name + "Async(" + method.ParametersList.Parameters + ")\n");
                result.Append("\t\t {\n");
                result.Append("\t\t   this." + method.Name + "Async(");

                foreach (var parameter in method.ParametersList.Parameters)
                {
                    result.Append(parameter.Identifier + ", ");
                }

                result.Append("null);\n");
                result.Append("\t\t }\n\n");

                result.Append("\t\t public void " + method.Name + "Async(" + method.ParametersList.Parameters + ", object userState)\n");
                result.Append("\t\t {\n");
                result.Append("\t\t   if ((this.onBegin" + method.Name + "Delegate == null))\n");
                result.Append("\t\t   {\n");
                result.Append("\t\t       this.onBegin" + method.Name + "Delegate = new BeginOperationDelegate(this.OnBegin" + method.Name + ");\n");
                result.Append("\t\t   }\n");
                result.Append("\t\t   if ((this.onEnd" + method.Name + "Delegate == null))\n");
                result.Append("\t\t   {\n");
                result.Append("\t\t       this.onEnd" + method.Name + "Delegate = new EndOperationDelegate(this.OnEnd" + method.Name + ");\n");
                result.Append("\t\t   }\n");
                result.Append("\t\t   if ((this.on" + method.Name + "CompletedDelegate == null))\n");
                result.Append("\t\t   {\n");
                result.Append("\t\t       this.on" + method.Name + "CompletedDelegate = new System.Threading.SendOrPostCallback(this.On" + method.Name + "Completed);\n");
                result.Append("\t\t   }\n");
                result.Append("\t\t   base.InvokeAsync(this.onBegin" + method.Name + "Delegate, new object[] {");

                counter = 0;
                foreach (var parameter in method.ParametersList.Parameters)
                {
                    result.Append(parameter.Identifier);
                    counter++;

                    if (method.ParametersList.Parameters.Count != counter)
                    {
                        result.Append(", ");
                    }
                }

                result.Append("}, this.onEnd" + method.Name + "Delegate, this.on" + method.Name + "CompletedDelegate, userState);\n");
                result.Append("\t\t       this.on" + method.Name + "CompletedDelegate = new System.Threading.SendOrPostCallback(this.On" + method.Name + "Completed);\n");
                result.Append("\t\t   }\n");
                result.Append("\t\t }\n\n");
                #endregion
            }

            return result.ToString();
        }
    }

    public class EndPoint
    {
        public string Name { get; set; }
        public TypeSyntax ReturnType { get; set; }
        public ParameterListSyntax ParametersList { get; set; }
    } 

}
