using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using WCFGenerator.Common;

namespace WCFGenerator.WcfClientGeneration
{
    internal class WcfGenerator
    {
        private readonly GeneratorWorkspace _generatorWorkspace;

        #region Fields

        private static List<string> _serviceUsings = new List<string>();
        private static List<string> _allUsings = new List<string>();

        #endregion

        public WcfGenerator(GeneratorWorkspace generatorWorkspace, List<ServiceDetail> srvs)
        {
            _generatorWorkspace = generatorWorkspace;
            Services = srvs;
        }

        #region Properties

        public List<ServiceDetail> Services { get; set; }

        #endregion

        /// <summary>
        ///     Start generation and save files
        /// </summary>
        public async Task Start()
        {
            foreach (var service in Services)
            {
                // add infirmation for generate
                var serviceDetails = await GetServiceDetails(service);

                #region Generate Api

                // Set target project - for save client
                _generatorWorkspace.SetTargetProject(service.ProjectName);
                
                // Genarate common code
                var apiProjectFiles = GenerateBaseFiles(serviceDetails.ProjectName);
                
                if (service.ClientInterfaceName != null)
                {
                    GenerateInterfaceAndChannel(serviceDetails, serviceDetails.ProjectName, serviceDetails.ServiceMethods);
                    GenerateServiceClient(serviceDetails, serviceDetails.ProjectName, serviceDetails.ServiceMethods);
                }

                // generate api
                var generatedApi = GenerateApi(serviceDetails, serviceDetails.ServiceMethods);
                apiProjectFiles.Add(generatedApi);

                var eventArgs = GenerateCompletedEventArgs(serviceDetails.ProjectName, serviceDetails.ServiceMethods);
                apiProjectFiles.AddRange(eventArgs);

                // Save all files
                _generatorWorkspace.UpdateFileInTargetProject(apiProjectFiles);
                await _generatorWorkspace.ApplyTargetProjectChanges();

                #endregion

                #region Generate api interface

                // Set target project - for save client interface
                _generatorWorkspace.SetTargetProject(service.ProjectApi);

                // Generate api interface
                var generatedApiInterface = GenerateApiInterface(serviceDetails, serviceDetails.ServiceMethods);
                _generatorWorkspace.UpdateFileInTargetProject(generatedApiInterface);
                await _generatorWorkspace.ApplyTargetProjectChanges();

                #endregion
            }
        }

        #region Generate

        private SrcFile GenerateApi(ServiceDetail service, List<EndPoint> methods)
        {
            var svcName = service.ClientInterfaceName;
            var prjName = service.ProjectName;

            var extIndex = svcName.IndexOf(".", StringComparison.Ordinal);
            svcName = extIndex > 0 ? svcName.Remove(extIndex) : svcName;
            svcName = svcName.IndexOf("I", StringComparison.Ordinal) == 0 ? svcName.Remove(0, 1) : svcName;

            var sb = new StringBuilder();

            sb.Append(string.Join("; \r\n", _serviceUsings) + "; \r\n\r\n");

            sb.Append(" namespace " + prjName + "\r\n { \r\n");
            sb.Append("\t public partial class " + svcName + "Api\r\n\t { \r\n");

            sb.Append("\t\t private ChannelContainer<T> CreateChannel<T>() where T : class, IProperter, new()\r\n\t\t { \r\n");
            sb.Append("\t\t\t var clientContainer = ClientFactory<T>.CreateClient(_endPoint.ToString(), _binding, _endPoint); \r\n");
            sb.Append("\t\t\t return clientContainer; \r\n\t\t } \r\n\r\n");

            foreach (var method in methods)
            {
                if (!service.ImplementedMethods.Any(x => x.Identifier.ToString() == method.Name
                                                 && x.ParameterList.Parameters.ToString() == method.ParametersList.Parameters.ToString()))
                {
                    var parameters = GenerateParameters(method.ParametersList);

                    var paramList = method.ParametersList;
                    var types = "";
                    var paramNames = "";

                    if (paramList != null)
                    {
                        types = paramList.Parameters.Aggregate(types, (current, param) => current + param.Type + ", ");
                        types = types != "" ? "<" + types.Remove(types.Length - 2) + ">" : "";

                        paramNames = paramList.Parameters.Aggregate(paramNames,
                            (current, param) => current + param.Identifier + ", ");
                    }

                    var returnType = method.ReturnTypeApi != "void"
                        ? ("<" + method.ReturnTypeApi + ">")
                        : "";

                    sb.Append("\t\t public async System.Threading.Tasks.Task" + returnType + " " + method.Name + "(" + parameters + ")\r\n");
                    sb.Append("\t\t {\r\n");
                    sb.Append("\t\t\t var channelContainer = CreateChannel<" + svcName + "Client> ();\r\n");
                    sb.Append("\t\t\t var scope = new FlowOperationContextScope(channelContainer.Client.InnerChannel);\r\n\r\n");
                    sb.Append("\t\t\t try\r\n");
                    sb.Append("\t\t\t {\r\n");
                    sb.Append("\t\t\t\t AddClientInformationHeader();\r\n");
                    sb.Append("\t\t\t\t");

                    if (method.ReturnTypeApi != "void")
                        sb.Append(" return");

                    sb.Append(" await System.Threading.Tasks.Task" +
                              (method.ReturnType != "void" ? ("<" + method.ReturnType + ">") : "") +
                              ".Factory.FromAsync" + types + "(channelContainer.Client.Begin" + method.Name +
                              ", channelContainer.Client.End" + method.Name + ", " + paramNames +
                              " null).ContinueOnScope(scope);\r\n");

                    sb.Append("\t\t\t }\r\n");
                    sb.Append("\t\t\t finally\r\n");
                    sb.Append("\t\t\t {\r\n");
                    sb.Append("\t\t\t\t var disposable = channelContainer as IDisposable; \r\n");
                    sb.Append("\t\t\t\t if (disposable != null) disposable.Dispose();\r\n\r\n");
                    sb.Append("\t\t\t\t disposable = scope as IDisposable;\r\n");
                    sb.Append("\t\t\t\t if (disposable != null) disposable.Dispose();\r\n");
                    sb.Append("\t\t\t }\r\n");
                    sb.Append("\t\t }\r\n\r\n");
                }
            }

            sb.Append("\t }");
            sb.Append(" }");

            return new SrcFile(svcName + "Api.g.cs", "", sb.ToString());
        }

        internal SrcFile GenerateApiInterface(ServiceDetail service, List<EndPoint> methods)
        {
            var svcName = service.ClientInterfaceName;

            var extIndex = svcName.IndexOf(".", StringComparison.Ordinal);
            svcName = extIndex > 0 ? svcName.Remove(extIndex) : svcName;
            svcName = svcName.IndexOf("I", StringComparison.Ordinal) == 0 ? svcName.Remove(0, 1) : svcName;

            var sb = new StringBuilder();

            sb.Append(string.Join("; \r\n", _serviceUsings) + "; \r\n\r\n");

            sb.Append(" namespace " + service.ProjectApi + "\r\n { \r\n");
            sb.Append("\t public partial interface I" + svcName + "Api\r\n\t { \r\n");

            foreach (var method in methods)
            {
                if (!service.ImplementedMethods.Any(x => x.Identifier.ToString() == method.Name && x.ParameterList.Parameters.ToString() == method.ParametersList.Parameters.ToString()))
                {
                    var parameters = GenerateParameters(method.ParametersList);

                    var returnType = method.ReturnTypeApi != "void" ? ("<" + method.ReturnTypeApi + ">") : "";

                    sb.Append("\t\t System.Threading.Tasks.Task" + returnType + " " + method.Name + "(" + parameters + ");\r\n");
                }
            }

            sb.Append("\t }");
            sb.Append(" }");

            return new SrcFile("I" + svcName + "Api.g.cs", string.Join("/", service.ProjectApiFolders), sb.ToString());
        }

        private string GenerateParameters(ParameterListSyntax parametersList)
        {
            var parameters = "";
            var size = parametersList.Parameters.Count;
            var count = 1;

            foreach (var parm in parametersList.Parameters)
            {
                var param = parm.ToString();

                if (param.Contains("IEnumerable<"))
                {
                    param = param.Replace("IEnumerable<", "").Replace(">", "[]");
                }

                parameters += param;

                if (count < size)
                {
                    parameters += ", ";
                    count++;
                }
            }

            return parameters;
        }

        private List<SrcFile> GenerateBaseFiles(string defaultNamespace)
        {
            var sb = new StringBuilder();

            var files = new List<SrcFile>();

            #region Create IProperter 

            sb.Append("namespace " + defaultNamespace + " \r\n{");
            sb.Append("\r\n\t public interface IProperter \r\n\t {\r\n");
            sb.Append("\t\t bool IsCaughtException { get; set; } \r\n");
            sb.Append("\t } \r\n}");

            files.Add(new SrcFile("IProperter.g.cs", "ServiceReferences", sb.ToString()));

            #endregion

            #region Create ChannelContainer 
            sb.Clear();

            sb.Append("using System;\r\n\r\n");

            sb.Append("namespace " + defaultNamespace + "\r\n");
            sb.Append("{\r\n\t public class ChannelContainer<TClient> : IDisposable\r\n");
            sb.Append("\t{ \r\n ");

            sb.Append("\t\t private readonly TClient _client; \r\n\r\n");
            sb.Append("\t\t public ChannelContainer(TClient client)\r\n");
            sb.Append("\t\t { \r\n \t\t\t _client = client;  \r\n \t\t } \r\n\r\n");

            sb.Append("\t\t public TClient Client\r\n");
            sb.Append("\t\t { \r\n \t\t\t get \r\n \t\t\t {\r\n \t\t\t\t return _client;  \r\n \t\t\t } \r\n \t\t } \r\n\r\n");

            sb.Append("\t\t public string Address { get; set; }\r\n\r\n");
            sb.Append("\t\t public event EventHandler Disposing;\r\n\r\n");

            sb.Append("\t\t public void Dispose()\r\n \t\t { \r\n \t\t\t var dispose = Disposing; \r\n \t\t\t if (dispose != null) dispose(this, new EventArgs()); \r\n");
            sb.Append("\t\t } \r\n \t } \r\n } ");

            files.Add(new SrcFile("ChannelContainer.g.cs", "", sb.ToString()));

            #endregion

            #region Create ClientFactory 

            sb.Clear();

            sb.Append("using System;" + " \r\n\r\n");
            sb.Append("using System.Collections.Concurrent;" + " \r\n\r\n");
            sb.Append("using System.ServiceModel;" + " \r\n\r\n");

            sb.Append("namespace " + defaultNamespace + "\r\n");
            sb.Append("{\r\n\t public static class ClientFactory<TClient> where TClient : class, IProperter , new()\r\n \t { \r\n");
            sb.Append("\t\t private static ConcurrentDictionary<string, ConcurrentBag<TClient>> FreeChannelsChannels { get; set; } \r\n");
            sb.Append("\t\t private static ConcurrentDictionary<int, ChannelContainer<TClient>> UsedChannels { get; set; } \r\n\r\n");

            sb.Append("\t\t static ClientFactory() \r\n\t\t { \r\n");
            sb.Append("\t\t\t FreeChannelsChannels = new ConcurrentDictionary<string, ConcurrentBag<TClient>>();\r\n");
            sb.Append("\t\t\t UsedChannels = new ConcurrentDictionary<int, ChannelContainer<TClient>>();\r\n \t\t } \r\n\r\n");

            sb.Append("\t\t public static ChannelContainer<TClient> CreateClient(string address, BasicHttpBinding binding, EndpointAddress enAddress)\r\n \t\t { \r\n");
            sb.Append("\t\t\t ConcurrentBag<TClient> currentChannels = GetFreeChannels(address); \r\n\r\n");
            sb.Append("\t\t\t TClient client = null; \r\n\r\n");

            sb.Append("\t\t\t for (int i=0; currentChannels.Count > 0  && client == null && i< 10; i++ ) \r\n \t\t\t { \r\n");
            sb.Append("\t\t\t\t currentChannels.TryTake(out client); \r\n \t\t\t } \r\n");

            sb.Append("\t\t\t client = client ?? (TClient)Activator.CreateInstance(typeof(TClient), new object[] {binding, enAddress}); \r\n\r\n");
            sb.Append("\t\t\t var container = new ChannelContainer<TClient>(client) { Address = address }; \r\n\r\n");

            sb.Append("\t\t\t UsedChannels.TryAdd(container.GetHashCode(), container); \r\n");
            sb.Append("\t\t\t container.Disposing += ContainerOnDisposing; \r\n\r\n");

            sb.Append("\t\t\t return container; \r\n \t\t } \r\n\r\n");

            sb.Append("\t\t public static ConcurrentBag<TClient> GetFreeChannels(string address) \r\n \t\t { \r\n");
            sb.Append("\t\t\t return FreeChannelsChannels.GetOrAdd(address, arg => new ConcurrentBag<TClient>()); \r\n \t\t } \r\n\r\n");

            sb.Append("\t\t private static void ContainerOnDisposing(object sender, EventArgs eventArgs) \r\n \t\t { \r\n");
            sb.Append("\t\t\t var container = (ChannelContainer<TClient>) sender;\r\n");
            sb.Append("\t\t\t container.Disposing -= ContainerOnDisposing; \r\n\r\n");
            sb.Append("\t\t\t UsedChannels.TryRemove(container.GetHashCode(), out container); \r\n\r\n");
            sb.Append("\t\t\t if(!container.Client.IsCaughtException) \r\n \t\t\t { \r\n");
            sb.Append("\t\t\t\t var freeChannels = GetFreeChannels(container.Address); \r\n");
            sb.Append("\t\t\t\t freeChannels.Add(container.Client); \r\n \t\t\t } else \r\n \t\t\t { \r\n ");
            sb.Append("\t\t\t\t ((IDisposable)container.Client).Dispose(); \r\n ");
            sb.Append("\t\t\t\t System.Diagnostics.Debug.WriteLine(\"Client has an exception\"); \r\n \t\t\t }\r\n");
            sb.Append("\t\t } \r\n \t } \r\n }");

            files.Add(new SrcFile("ClientFactory.g.cs", "", sb.ToString()));

            #endregion

            #region Create FlowOperationContextScope 

            sb.Clear();

            sb.Append("using System;\r\n");
            sb.Append("using System.ServiceModel;\r\n\r\n");

            sb.Append("namespace " + defaultNamespace + "\r\n");
            sb.Append("{\r\n\t public sealed class FlowOperationContextScope : IDisposable\r\n");
            sb.Append("\t { \r\n");
            sb.Append("\t\t private bool _inFlight; \r\n");
            sb.Append("\t\t private bool _disposed; \r\n");
            sb.Append("\t\t private OperationContext _thisContext; \r\n");
            sb.Append("\t\t private OperationContext _originalContext; \r\n\r\n");

            sb.Append("\t\t public FlowOperationContextScope(IContextChannel channel)\r\n \t\t\t: this(new OperationContext(channel)) {} \r\n\r\n");
            sb.Append("\t\t public FlowOperationContextScope(OperationContext context)\r\n \t\t { \r\n \t\t\t _originalContext = OperationContext.Current; \r\n");
            sb.Append("\t\t\t _thisContext = context;\r\n");
            sb.Append("\t\t\t OperationContext.Current = context;\r\n");
            sb.Append("\t\t\t while (OperationContext.Current == null) \r\n\t\t\t {\r\n\t\t\t\t OperationContext.Current = context;\r\n\t\t\t }\r\n\t\t}\r\n\r\n");

            sb.Append("\t\t public void Dispose()\r\n \t\t { \r\n \t\t\t if (_disposed) return; \r\n");
            sb.Append("\t\t\t if (_inFlight || OperationContext.Current != _thisContext)\r\n \t\t\t { \r\n \t\t\t\t throw new InvalidOperationException();\r\n \t\t\t } \r\n");
            sb.Append("\t\t\t _disposed = true; \r\n");
            sb.Append("\t\t\t OperationContext.Current = _originalContext; \r\n");
            sb.Append("\t\t\t _thisContext = null; \r\n");
            sb.Append("\t\t\t _originalContext = null; \r\n \t\t } \r\n\r\n");

            sb.Append("\t\t internal void BeforeAwait() \r\n");
            sb.Append("\t\t { \r\n");
            sb.Append("\t\t\t if (_inFlight) \r\n");
            sb.Append("\t\t\t { \r\n \t\t\t\t return; \r\n \t\t\t } \r\n");
            sb.Append("\t\t\t _inFlight = true; \r\n");
            sb.Append("\t\t } \r\n\r\n");

            sb.Append("\t\t internal void AfterAwait() \r\n");
            sb.Append("\t\t { \r\n");
            sb.Append("\t\t\t if (!_inFlight) \r\n");
            sb.Append("\t\t\t { \r\n ");
            sb.Append("\t\t\t\t throw new InvalidOperationException(); \r\n ");
            sb.Append("\t\t\t } \r\n ");
            sb.Append("\t\t\t _inFlight = false; \r\n");
            sb.Append("\t\t\t OperationContext.Current = _thisContext; \r\n");
            sb.Append("\t\t } \r\n \t } \r\n }");

            files.Add(new SrcFile("FlowOperationContextScope.g.cs","", sb.ToString()));

            #endregion

            #region Create SimpleAwaiter

            sb.Clear();

            sb.Append("using System;\r\n");
            sb.Append("using System.Threading.Tasks;\r\n");
            sb.Append("using System.Runtime.CompilerServices;\r\n");
            sb.Append("using System.Threading;\r\n\r\n");

            sb.Append("\r\n\r\n namespace " + defaultNamespace);
            sb.Append("\r\n {\r\n\t public class SimpleAwaiter : INotifyCompletion");
            sb.Append("\r\n\t {");

            sb.Append("\r\n\t\t #region Fields\r\n");
            sb.Append("\r\n\t\t protected readonly Task task;");
            sb.Append("\r\n\t\t protected readonly Action beforeAwait;");
            sb.Append("\r\n\t\t protected readonly Action afterAwait;");
            sb.Append("\r\n\r\n\t\t #endregion\r\n");

            sb.Append("\r\n\t\t public SimpleAwaiter(Task task, Action beforeAwait, Action afterAwait)");
            sb.Append("\r\n\t\t {");
            sb.Append("\r\n\t\t\t this.task = task;");
            sb.Append("\r\n\t\t\t this.beforeAwait = beforeAwait;");
            sb.Append("\r\n\t\t\t this.afterAwait = afterAwait;");
            sb.Append("\r\n\t\t }\r\n");

            sb.Append("\r\n\t\t public SimpleAwaiter GetAwaiter()");
            sb.Append("\r\n\t\t {");
            sb.Append("\r\n\t\t\t return this;");
            sb.Append("\r\n\t\t }\r\n");

            sb.Append("\r\n\t\t public void GetResult()");
            sb.Append("\r\n\t\t {");
            sb.Append("\r\n\t\t\t // FUCK YEAH! DO NOT REMOVE!");
            sb.Append("\r\n\t\t\t task.GetAwaiter().GetResult();");
            sb.Append("\r\n\t\t }\r\n");

            sb.Append("\r\n\t\t public bool IsCompleted");
            sb.Append("\r\n\t\t {");
            sb.Append("\r\n\t\t\t get \r\n\t\t\t {");
            sb.Append("\r\n\t\t\t\t // don't do anything if the task completed synchronously");
            sb.Append("\r\n\t\t\t\t // (we're on the same thread)");
            sb.Append("\r\n\t\t\t\t if (task.IsCompleted)");
            sb.Append("\r\n\t\t\t\t {");
            sb.Append("\r\n\t\t\t\t\t return true;");
            sb.Append("\r\n\t\t\t\t }");
            sb.Append("\r\n\t\t\t\t beforeAwait();");
            sb.Append("\r\n\t\t\t\t return false;");
            sb.Append("\r\n\t\t\t }");
            sb.Append("\r\n\t\t }\r\n");

            sb.Append("\r\n\t\t public void OnCompleted(Action continuation)");
            sb.Append("\r\n\t\t {");
            sb.Append("\r\n\t\t\t task.ContinueWith(t =>");
            sb.Append("\r\n\t\t\t { ");
            sb.Append("\r\n\t\t\t\t afterAwait();");
            sb.Append("\r\n\t\t\t\t continuation();");
            sb.Append("\r\n\t\t\t },");
            sb.Append("\r\n\t\t\t CancellationToken.None,");
            sb.Append("\r\n\t\t\t TaskContinuationOptions.ExecuteSynchronously,");
            sb.Append("\r\n\t\t\t SynchronizationContext.Current != null");
            sb.Append("\r\n\t\t\t\t ? TaskScheduler.FromCurrentSynchronizationContext()");
            sb.Append("\r\n\t\t\t\t : TaskScheduler.Current);");
            sb.Append("\r\n\t\t}\r\n\t }\r\n");
            sb.Append("\r\n\t public class SimpleAwaiter<TResult> : SimpleAwaiter");
            sb.Append("\r\n\t {\r\n\t\t #region Fields\r\n");
            sb.Append("\r\n\t\t private readonly Task<TResult> _task;");
            sb.Append("\r\n\r\n\t\t #endregion\r\n");
            sb.Append("\r\n\t\t public SimpleAwaiter(Task<TResult> task, Action beforeAwait, Action afterAwait)");
            sb.Append("\r\n\t\t\t : base(task, beforeAwait, afterAwait)");
            sb.Append("\r\n\t\t {");
            sb.Append("\r\n\t\t\t _task = task;");

            sb.Append("\r\n\t\t }");
            sb.Append("\r\n\r\n\t\t public new SimpleAwaiter<TResult> GetAwaiter()");
            sb.Append("\r\n\t\t {");
            sb.Append("\r\n\t\t\t return this;");
            sb.Append("\r\n\t\t }\r\n");
            sb.Append("\r\n\t\t public new TResult GetResult()");
            sb.Append("\r\n\t\t {");
            sb.Append("\r\n\t\t\t return _task.Result;");

            sb.Append("\r\n\t\t }\r\n\t }\r\n}\r\n");

            files.Add(new SrcFile("SimpleAwaiter.g.cs", "", sb.ToString()));

            #endregion

            #region Create TaskExt 

            sb.Clear();

            sb.Append("using System;\r\n");
            sb.Append("using System.Threading.Tasks;\r\n\r\n");

            sb.Append("namespace " + defaultNamespace + "\r\n");
            sb.Append("{\r\n\t public static class TaskExt\r\n");
            sb.Append("\t {\r\n");
            sb.Append("\t\t public static SimpleAwaiter<TResult> ContinueOnScope<TResult>(this Task<TResult> @this, FlowOperationContextScope scope) \r\n");
            sb.Append("\t\t {\r\n");
            sb.Append("\t\t\t return new SimpleAwaiter<TResult>(@this, scope.BeforeAwait, scope.AfterAwait);\r\n");
            sb.Append("\t\t }\r\n\r\n");
            sb.Append("\t\t public static SimpleAwaiter ContinueOnScope(this Task @this, FlowOperationContextScope scope) \r\n");
            sb.Append("\t\t {\r\n");
            sb.Append("\t\t\t return new SimpleAwaiter(@this, scope.BeforeAwait, scope.AfterAwait);\r\n");
            sb.Append("\t\t }\r\n\t }\r\n}");

            files.Add(new SrcFile("TaskExt.g.cs", "", sb.ToString()));

            #endregion

            #region EndpointConfiguration


            sb.Clear();

            sb.Append("using System; \r\n\r\n");
            sb.Append("namespace " + defaultNamespace + "\r\n { \r\n");
            sb.Append("\t public enum EndpointConfiguration \r\n\t { \r\n");
            sb.Append("\t\t BasicHttpBinding_Client \r\n");
            sb.Append("\t } \r\n }");

            files.Add(new SrcFile("EndpointConfiguration.g.cs", "ServiceReferences", sb.ToString()));

            #endregion

            return files;
        }

        private void GenerateInterfaceAndChannel(ServiceDetail iService, string projectName, List<EndPoint> methods)
        {
            var sb = new StringBuilder();
            var serviceFileName = iService.FileName.Remove(iService.FileName.IndexOf(".", StringComparison.Ordinal));

            sb.Append(String.Join("; \r\n", _serviceUsings) + "; \r\n" + " \r\n\r\n");

            sb.Append("namespace " + projectName + "\r\n { \r\n");
            sb.Append("\t [System.CodeDom.Compiler.GeneratedCodeAttribute(\"System.ServiceModel\", \"4.0.0.0\")]\r\n");
            sb.Append("\t [System.ServiceModel.ServiceContractAttribute(ConfigurationName = \"" + serviceFileName + "Client\")]\r\n");
            sb.Append("\t public interface " + iService.ClientInterfaceName + "Client\r\n \t{ \r\n");
            sb.Append("\t\t bool IsCaughtException { get; set; } \r\n\r\n");

            foreach (var method in methods)
            {
                var parameters = method.ParametersList.ToString().Replace("(", "").Replace(")", "");
                var pm = parameters != "" ? parameters + ", " : "";

                sb.Append("\t\t [System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = \"http://tempuri.org/" + serviceFileName + "/" + method.Name + "\", ReplyAction = \"http://tempuri.org/" + serviceFileName + "/" + method.Name + "Response\")]\r\n");

                foreach (var fault in method.Faults)
                {
                    var faultType = fault.Attributes.First().ArgumentList.Arguments.First().Expression.ToString().Replace("typeof(", "").Replace(")", "");
                    sb.Append("\t\t [System.ServiceModel.FaultContractAttribute(typeof(" + (iService.FaultProject ?? iService.ProjectApi) +
                               "." + faultType + "), Action=\"http://tempuri.org/" + serviceFileName + "/" + method.Name + faultType +
                               "Fault\", Name=\"" + faultType + "\", Namespace=\"http://schemas.datacontract.org/2004/07/YumaPos.Shared.API.Faults\")]\r\n");
                }

                sb.Append("\t\t System.IAsyncResult Begin" + method.Name + "(" + pm + "System.AsyncCallback callback, object asyncState);\r\n\r\n");
                sb.Append("\t\t " + method.ReturnType + " End" + method.Name + "(System.IAsyncResult result);\r\n\r\n");
            }
            sb.Append("\t } \r\n\r\n");
            sb.Append("\t [System.CodeDom.Compiler.GeneratedCodeAttribute(\"System.ServiceModel\", \"4.0.0.0\")]\r\n");
            sb.Append("\t public interface " + iService.ClientInterfaceName + "Channel : " + iService.ClientInterfaceName + "Client, System.ServiceModel.IClientChannel{} \r\n }");

            _generatorWorkspace.UpdateFileInTargetProject(iService.ClientInterfaceName + "Client.g.cs", "ServiceReferences", sb.ToString());
        }

        private List<SrcFile> GenerateCompletedEventArgs(string projectName, List<EndPoint> competedArgsMethods)
        {
            var sb = new StringBuilder();
            var ret = new List<SrcFile>();

            foreach (var method in competedArgsMethods)
            {
                sb.Clear();

                sb.Append(String.Join("; \r\n", _allUsings) + "; \r\n\r\n");

                sb.Append(" namespace " + projectName + "\r\n { \r\n");
                sb.Append("\t [System.Diagnostics.DebuggerStepThroughAttribute()] \r\n");
                sb.Append("\t [System.CodeDom.Compiler.GeneratedCodeAttribute(\"System.ServiceModel\", \"4.0.0.0\")] \r\n");
                sb.Append("\t public partial class " + method.Service + "_" + method.Name + "CompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs \r\n");
                sb.Append("\t { \r\n\r\n");
                sb.Append("\t   private object[] results; \r\n\r\n");
                sb.Append("\t   public " + method.Service + "_" + method.Name + "CompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :\r\n");
                sb.Append("\t   base(exception, cancelled, userState) {\r\n");
                sb.Append("\t       this.results = results;\r\n");
                sb.Append("\t   } \r\n\r\n");

                if (method.InterfaceReturnType != "void")
                {
                    sb.Append("\t   public " + method.InterfaceReturnType + " Result { \r\n");
                    sb.Append("\t       get {\r\n");
                    sb.Append("\t           base.RaiseExceptionIfNecessary();\r\n");
                    sb.Append("\t           return ((" + method.InterfaceReturnType + ")(this.results[0]));\r\n");
                    sb.Append("\t       }\r\n");
                    sb.Append("\t   }\r\n");
                }

                sb.Append("\t }\r\n");
                sb.Append(" }");

               ret.Add(new SrcFile(method.Service + "_" + method.Name + "CompletedEventArgs.g.cs", "ServiceReferences\\CompletedEventArgs", sb.ToString()));
            }

            return ret;
        }

        private void GenerateServiceClient(ServiceDetail sd, string projectName, List<EndPoint> methods)
        {
            var sb = new StringBuilder();
            var svcName = sd.ClientInterfaceName;

            var channelName = (svcName.IndexOf("I", StringComparison.Ordinal) == 0 ? svcName.Remove(0, 1) : svcName) + "Client";

            sb.Append(string.Join("; \r\n", _allUsings) + "; \r\n");

            sb.Append(" namespace " + projectName + "\r\n { \r\n");
            sb.Append("\t public partial class " + channelName + " : System.ServiceModel.ClientBase<" + svcName + "Client>, " + svcName + "Client, IProperter\r\n ");
            sb.Append("\t {");

            sb.Append(GeneratePropertiesAndConstructors(svcName));
            sb.Append(GenerateMethods(svcName + "Client", methods));
            sb.Append(GenerateAdditionMathods(svcName + "Client"));
            sb.Append(GenerateClientChannel(svcName + "Client", methods));

            sb.Append("    } \r\n");
            sb.Append("}");

            _generatorWorkspace.UpdateFileInTargetProject(channelName + ".g.cs", "ServiceReferences", sb.ToString());
        }

        private static string GenerateClientChannel(string svcClient, List<EndPoint> methods)
        {
            var sb = new StringBuilder();
            var channelName = (svcClient.IndexOf("I", StringComparison.Ordinal) == 0 ? svcClient.Remove(0, 1) : svcClient) + "Channel";

            sb.Append("\t\t private class " + channelName + " : ChannelBase<" + svcClient + ">, " + svcClient + ", IProperter \r\n \t\t { \r\n");
            sb.Append("\t\t\t public bool IsCaughtException { get; set; } \r\n\r\n");

            sb.Append("\t\t public " + channelName + "(System.ServiceModel.ClientBase<" + svcClient + "> client) : base(client) { } \r\n\r\n");

            foreach (var method in methods)
            {
                var parameters = "";

                foreach (var parameter in method.ParametersList.Parameters)
                {
                    parameters += parameter.Type + " " + parameter.Identifier + ", ";
                }

                sb.Append("\t\t public System.IAsyncResult Begin" + method.Name + "(" + parameters + "System.AsyncCallback callback, object asyncState) \r\n \t\t { \r\n");
                sb.Append("\t\t\t object[] _args = new object[" + method.ParametersList.Parameters.Count + "]; \r\n");

                foreach (var parameter in method.ParametersList.Parameters)
                {
                    sb.Append("\t\t\t _args[" + method.ParametersList.Parameters.IndexOf(parameter) + "] = " + parameter.Identifier + ";\r\n");
                }

                sb.Append("\t\t\t System.IAsyncResult _result = base.BeginInvoke(\"" + method.Name + "\", _args, callback, asyncState);\r\n");
                sb.Append("\t\t\t return _result;\r\n");
                sb.Append("\t\t } \r\n\r\n");

                sb.Append("\t\t public " + method.ReturnType + " End" + method.Name + "(System.IAsyncResult result) \r\n \t\t { \r\n");
                sb.Append("\t\t\t object[] _args = new object[0];\r\n");

                if (method.ReturnType.ToString() == "void")
                {
                    sb.Append("\t\t\t base.EndInvoke(\"" + method.Name + "\", _args, result);\r\n");
                }
                else
                {
                    sb.Append("\t\t\t " + method.ReturnType + " _result = ((" + method.ReturnType + ")(base.EndInvoke(\"" + method.Name + "\", _args, result)));\r\n");
                }

                if (method.ReturnType != "void")
                {
                    sb.Append("\t\t\t return _result;\r\n");
                }

                sb.Append("\t\t } \r\n\r\n");
            }
            sb.Append("\t } \r\n");

            return sb.ToString();
        }

        private static string GenerateAdditionMathods(string svcName)
        {
            var sb = new StringBuilder();
            var client = (svcName.IndexOf("I", StringComparison.Ordinal) == 0 ? svcName.Remove(0, 1) : svcName);

            sb.Append("\t\t protected override " + svcName + " CreateChannel() \r\n \t\t {\r\n");
            sb.Append("\t\t\t return new " + client + "Channel(this); \r\n \t\t } \r\n\r\n");

            sb.Append("\t\t private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration) \r\n \t\t { \r\n");
            sb.Append("\t\t\t if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_Client)) \r\n \t\t\t { \r\n");
            sb.Append("\t\t\t\t System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding(); \r\n");
            sb.Append("\t\t\t\t result.MaxBufferSize = int.MaxValue; \r\n");
            sb.Append("\t\t\t\t result.MaxReceivedMessageSize = int.MaxValue; \r\n");
            sb.Append("\t\t\t\t return result; \r\n \t\t\t } \r\n ");
            sb.Append("\t\t\t throw new System.InvalidOperationException(string.Format(\"Could not find endpoint with name \'{0}\'.\", endpointConfiguration));\r\n \t\t } \r\n\r\n");

            sb.Append("\t\t private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration) \r\n \t\t { \r\n");
            sb.Append("\t\t\t if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_Client)) \r\n \t\t\t { \r\n");
            sb.Append("\t\t\t\t return new System.ServiceModel.EndpointAddress(_remoteAddress); \r\n \t\t\t } \r\n\r\n");
            sb.Append("\t\t\t throw new System.InvalidOperationException(string.Format(\"Could not find endpoint with name \'{0}\'.\", endpointConfiguration));\r\n \t\t } \r\n\r\n");

            sb.Append("\t\t private static System.ServiceModel.Channels.Binding GetDefaultBinding() \r\n \t\t { \r\n");
            sb.Append("\t\t\t return " + client + ".GetBindingForEndpoint(EndpointConfiguration.BasicHttpBinding_Client); \r\n \t\t } \r\n");

            sb.Append("\t\t private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress() \r\n \t\t { \r\n");
            sb.Append("\t\t\t return " + client + ".GetEndpointAddress(EndpointConfiguration.BasicHttpBinding_Client); \r\n \t\t } \r\n\r\n");

            return sb.ToString();
        }

        private static string GeneratePropertiesAndConstructors(string svcName)
        {
            var properties = " \r\n\t\t private static string _remoteAddress = \"\"; \r\n";
            properties += " \r\n\t\t public bool IsCaughtException { get; set; } \r\n\r\n";

            var client = (svcName.IndexOf("I", StringComparison.Ordinal) == 0 ? svcName.Remove(0, 1) : svcName);

            return properties +
                   "\t\t public " + client + "Client() : base(" + client + "Client.GetDefaultBinding(), " + client + "Client.GetDefaultEndpointAddress()) { } \r\n\r\n" +
                   "\t\t public " + client + "Client(EndpointConfiguration endpointConfiguration) : base(" + client + "Client.GetBindingForEndpoint(endpointConfiguration), " + client + "Client.GetEndpointAddress(endpointConfiguration)) {   } \r\n\r\n" +
                   "\t\t public " + client + "Client(EndpointConfiguration endpointConfiguration, string remoteAddress) : base(" + client + "Client.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress)) { \r\n \t\t\t _remoteAddress = remoteAddress; \r\n \t\t } \r\n\r\n" +
                   "\t\t public " + client + "Client(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : base(" + client + "Client.GetBindingForEndpoint(endpointConfiguration), remoteAddress)  { \r\n \t\t\t _remoteAddress =  remoteAddress.Uri.AbsoluteUri; \r\n \t\t } \r\n\r\n" +
                   "\t\t public " + client + "Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(binding, remoteAddress) {\r\n \t\t\t _remoteAddress =  remoteAddress.Uri.AbsoluteUri; \r\n \t\t  } \r\n\r\n";
        }

        private static string GenerateMethods(string client, List<EndPoint> methods)
        {
            var result = new StringBuilder();

            result.Append("\r\n");

            #region Delegates And Event

            foreach (var method in methods)
            {
                result.Append("\t\t private BeginOperationDelegate onBegin" + method.Name + "Delegate; \r\n");
                result.Append("\t\t private EndOperationDelegate onEnd" + method.Name + "Delegate; \r\n");
                result.Append("\t\t private System.Threading.SendOrPostCallback on" + method.Name + "CompletedDelegate; \r\n\r\n");

                if (method.ReturnType.ToString() == "void")
                {
                    result.Append("\t\t public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> " + method.Name + "Completed; \r\n\r\n");
                }
                else
                {
                    result.Append("\t\t public event System.EventHandler<" + method.Service + "_" + method.Name + "CompletedEventArgs> " + method.Name + "Completed; \r\n\r\n");
                }
            }

            #endregion

            foreach (var method in methods)
            {
                #region Begin-End

                var parms = method.ParametersList.Parameters.Count == 0 ? "" : (method.ParametersList.Parameters + ",");

                result.Append("\t\t [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]\r\n");
                result.Append("\t\t public System.IAsyncResult Begin" + method.Name + "(" + parms + " System.AsyncCallback callback, object asyncState)\r\n");
                result.Append("\t\t {\r\n");
                result.Append("\t\t   return base.Channel.Begin" + method.Name + "(");

                foreach (var parameter in method.ParametersList.Parameters)
                {
                    result.Append(parameter.Identifier + ", ");
                }

                result.Append(" callback, asyncState); \r\n");

                result.Append("\t\t }\r\n\r\n");

                result.Append("\t\t [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]\r\n");
                result.Append("\t\t public " + method.ReturnType + " End" + method.Name + "(System.IAsyncResult result)\r\n");
                result.Append("\t\t {\r\n");
                result.Append("\t\t   try\r\n");
                result.Append("\t\t   {\r\n");
                result.Append("\t\t       " + (method.ReturnType != "void" ? "return" : "") + " base.Channel.End" + method.Name + "(result);\r\n");
                result.Append("\t\t   }\r\n");
                result.Append("\t\t   catch (Exception)\r\n");
                result.Append("\t\t   {\r\n");
                result.Append("\t\t       IsCaughtException = true;\r\n");
                result.Append("\t\t       throw;\r\n");
                result.Append("\t\t   }\r\n");
                result.Append("\t\t }\r\n\r\n");
                #endregion

                #region OnBegin-OnEnd-OnCompleted
                result.Append("\t\t private System.IAsyncResult OnBegin" + method.Name + "(object[] inValues, System.AsyncCallback callback, object asyncState)\r\n");
                result.Append("\t\t {\r\n");

                var counter = 0;
                foreach (var parameter in method.ParametersList.Parameters)
                {
                    result.Append("\t\t   " + parameter.Type + " " + parameter.Identifier + " = ((" + parameter.Type + ")(inValues[" + counter + "]));\r\n");
                    counter++;
                }

                result.Append("\t\t   return ((" + client + ")(this)).Begin" + method.Name + "(");

                foreach (var parameter in method.ParametersList.Parameters)
                {
                    result.Append(parameter.Identifier + ", ");
                }

                result.Append("callback, asyncState);\r\n");
                result.Append("\t\t }\r\n\r\n");

                result.Append("\t\t private object[] OnEnd" + method.Name + "(System.IAsyncResult result)\r\n");
                result.Append("\t\t {\r\n");

                if (method.ReturnType != "void")
                {
                    result.Append("\t\t   " + method.ReturnType + " retVal = ((" + client + ")(this)).End" + method.Name + "(result);\r\n");
                    result.Append("\t\t   return new object[] { retVal };\r\n");
                }
                else
                {
                    result.Append("\t\t   ((" + client + ")(this)).End" + method.Name + "(result);\r\n");
                    result.Append("\t\t   return null;\r\n");
                }

                result.Append("\t\t }\r\n\r\n");

                result.Append("\t\t private void On" + method.Name + "Completed(object state)\r\n");
                result.Append("\t\t {\r\n");
                result.Append("\t\t   if ((this." + method.Name + "Completed != null))\r\n");
                result.Append("\t\t   {\r\n");
                result.Append("\t\t      InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));\r\n");
                result.Append("\t\t      this." + method.Name + "Completed(this, new " + method.Service + "_" + method.Name + "CompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));\r\n");
                result.Append("\t\t   }\r\n");
                result.Append("\t\t }\r\n\r\n");
                #endregion

                #region AsyncMethod
                result.Append("\t\t public void " + method.Name + "Async(" + method.ParametersList.Parameters + ")\r\n");
                result.Append("\t\t {\r\n");
                result.Append("\t\t   this." + method.Name + "Async(");

                foreach (var parameter in method.ParametersList.Parameters)
                {
                    result.Append(parameter.Identifier + ", ");
                }

                result.Append("null);\r\n");
                result.Append("\t\t }\r\n\r\n");

                result.Append("\t\t public void " + method.Name + "Async(" + (method.ParametersList.Parameters.Count > 0 ? method.ParametersList.Parameters + ", " : "") + " object userState)\r\n");
                result.Append("\t\t {\r\n");
                result.Append("\t\t   if ((this.onBegin" + method.Name + "Delegate == null))\r\n");
                result.Append("\t\t   {\r\n");
                result.Append("\t\t       this.onBegin" + method.Name + "Delegate = new BeginOperationDelegate(this.OnBegin" + method.Name + ");\r\n");
                result.Append("\t\t   }\r\n");
                result.Append("\t\t   if ((this.onEnd" + method.Name + "Delegate == null))\r\n");
                result.Append("\t\t   {\r\n");
                result.Append("\t\t       this.onEnd" + method.Name + "Delegate = new EndOperationDelegate(this.OnEnd" + method.Name + ");\r\n");
                result.Append("\t\t   }\r\n");
                result.Append("\t\t   if ((this.on" + method.Name + "CompletedDelegate == null))\r\n");
                result.Append("\t\t   {\r\n");
                result.Append("\t\t       this.on" + method.Name + "CompletedDelegate = new System.Threading.SendOrPostCallback(this.On" + method.Name + "Completed);\r\n");
                result.Append("\t\t   }\r\n");
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

                result.Append("}, this.onEnd" + method.Name + "Delegate, this.on" + method.Name + "CompletedDelegate, userState);\r\n");
                result.Append("\t\t       this.on" + method.Name + "CompletedDelegate = new System.Threading.SendOrPostCallback(this.On" + method.Name + "Completed);\r\n");
                result.Append("\t\t }\r\n\r\n");
                #endregion
            }

            return result.ToString();
        }

        #endregion

        #region Analyze

        private async Task<ServiceDetail> GetServiceDetails(ServiceDetail service)
        {
            // Get target project
            var project = _generatorWorkspace.Solution.Projects.FirstOrDefault(x => x.Name == service.ProjectName);

            // Target project not found
            if (project == null) throw new ArgumentException(service.ProjectName + " project not found.");

            // Find svc document
            var svc = _generatorWorkspace.Solution.Projects
                .SelectMany(p => p.Documents)
                .FirstOrDefault(document => document.Name == service.FileName);

            // service not found
            if (svc == null) throw new ArgumentException(service.FileName + " file not found.");

            // Get service methods
            service.ServiceMethods = await GetServiceMethods(svc, service, project);


            // Get methods for generate
            var api = _generatorWorkspace.Solution.Projects
                .First(p => p.Name == service.ProjectApi).Documents.FirstOrDefault(x => x.Name == service.ClientInterfaceName + "Api.cs");

            if (api == null) throw new ArgumentException(service.ClientInterfaceName + "Api.cs not found.");

            var tree = await api.GetSyntaxTreeAsync();
            var customMethods = GetMethodSyntaxesFromTree(tree);

            service.ImplementedMethods = customMethods;


            return service;
        }

        private static async Task<List<EndPoint>> GetServiceMethods(Document svc, ServiceDetail iService, Project proj)
        {
            var documentRoot = await svc.GetSyntaxRootAsync();
            var rootCompUnit = (CompilationUnitSyntax) documentRoot;
            var defaultUsings = rootCompUnit.Usings.Select(x => x.Name.ToString()).ToList();

            var tree = await svc.GetSyntaxTreeAsync();
            var methodDeclarationSyntaxs = GetMethodSyntaxesFromTree(tree);

            _serviceUsings = GetUsings(svc, methodDeclarationSyntaxs, defaultUsings);
            _allUsings.AddRange(_serviceUsings.Except(_allUsings));


            var methods = new List<EndPoint>();
            methods.AddRange(methodDeclarationSyntaxs.Select(sm => new EndPoint()
            {
                Service = iService.ClientInterfaceName.IndexOf("I", StringComparison.Ordinal) == 0 ? iService.ClientInterfaceName.Remove(0, 1) : iService.ClientInterfaceName,
                Name = sm.Identifier.ToString(),
                ReturnType = GetFullReturnType(sm.ReturnType, proj),
                ReturnTypeApi = GetFullReturnType(sm.ReturnType, proj),
                InterfaceReturnType = GetFullReturnType(sm.ReturnType, proj),
                ParametersList = sm.ParameterList,
                Faults = sm.AttributeLists.Where(x => x.Attributes.Any(a1 => a1.Name.ToString().Contains("FaultContract")))
            }));

            foreach (var method in methods)
            {
                method.ReturnType = ChangeType(method.ReturnType);
                method.ReturnTypeApi = ChangeType(method.ReturnTypeApi);
            }

            methods = methods.OrderBy(o => o.Name).ToList();

            return methods;
        }

        private static string GetFullReturnType(TypeSyntax returnType, Project project)
        {
            var fullReturnType = returnType.ToString();
            var node = returnType.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
            Document cl = null;

            var systemTypes = new List<string>()
            {
                "Guid",
                "Guid?"
            };

            if (node != null && systemTypes.All(x => x != node.Identifier.ToString() && x != returnType.ToString()))
            {
                var nameSpace = SymbolFinder.FindDeclarationsAsync(project, node.Identifier.ValueText, ignoreCase: false).Result.Last().ContainingNamespace.ToString();

                fullReturnType = fullReturnType.Replace(node.Identifier.ToString(), nameSpace + "." + node.Identifier);
            }

            return fullReturnType;
        }

        private static List<string> GetUsings(Document svc, IList<MethodDeclarationSyntax> methodDeclarationSyntaxs, List<string> defaultUsings)
        {
            var usingsCollection = new List<string>()
            {
                "System",
                "System.IO",
                "System.Threading",
                "System.ServiceModel",
                "System.Threading.Tasks",
                "System.Collections.Generic"
            };

            foreach (var method in methodDeclarationSyntaxs)
            {
                var nodes = method.ParameterList.DescendantNodes().OfType<IdentifierNameSyntax>().ToList();
                nodes.AddRange(method.ReturnType.DescendantNodes().OfType<IdentifierNameSyntax>().ToList());

                if (nodes.Any())
                {
                    var dyclarationSyntax =
                        SymbolFinder.FindDeclarationsAsync(svc.Project, nodes.First().Identifier.ValueText,
                            ignoreCase: false).Result;

                    if (dyclarationSyntax != null && dyclarationSyntax.Any())
                    {
                        var newUsing = dyclarationSyntax.First().ContainingNamespace.ToString();

                        if (!usingsCollection.Contains(newUsing) && !newUsing.Contains("Microsoft.") && defaultUsings.Contains(newUsing))
                        {
                            usingsCollection.Add(newUsing);
                        }
                    }
                }
            }

            var newUsings = defaultUsings.Intersect(usingsCollection).ToList();
            newUsings = newUsings.Select(x => "using " + x).ToList();
            usingsCollection = newUsings;

            return usingsCollection;
        }

        private static List<MethodDeclarationSyntax> GetMethodSyntaxesFromTree(SyntaxTree syntaxTree)
        {
            var syntaxRoot = syntaxTree.GetRoot();
            return syntaxRoot.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
        }

        private static string ChangeType(string method)
        {
            var returnType = method;

            if (method.Contains("Task"))
            {
                if (returnType.Contains("<"))
                {
                    returnType = returnType.Replace("Task<", "");
                    returnType = returnType.Remove(returnType.Length - 1);
                }
                else
                {
                    returnType = returnType.Replace("Task", "void");
                }
            }

            if (method.Contains("IEnumerable<"))
            {
                returnType = returnType.Replace("IEnumerable<", "");
                returnType = returnType.Remove(returnType.Length - 1);
                returnType = returnType + "[]";
            }

            returnType = returnType == "System.IO.Stream" ? "byte[]" : returnType;
            return returnType;
        }

        #endregion
    }
}
