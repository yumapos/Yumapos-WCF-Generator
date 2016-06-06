using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;

namespace FirstRoslynApp
{
    class WCFGenerator
    {
        private static List<EndPoint> _methods;
        private static List<EndPoint> _competedArgsMethods;

        private static readonly MSBuildWorkspace _workspace = MSBuildWorkspace.Create();

        private static Solution _solution = null;
        private static Project _project = null;

        private static string _usings = "";

        private static List<Tuple<string, SourceText, string[], string>> tasks;

        public string ProjectName { get; set; }
        public string SolutionPath { get; set; }
        public List<string> Services { get; set; }
        public string ProjectApi { get; set; }

        private void CheckValues(string[] args)
        {
            SolutionPath = (args.Length > 0 && args[0] != null) ? args[0] : SolutionPath;
            ProjectName = (args.Length > 1 && args[1] != null) ? args[1] : ProjectName;
        }

        public async Task Start(string[] args)
        {
            _solution = await _workspace.OpenSolutionAsync(SolutionPath);
            _project = _solution.Projects.First(x => x.Name == ProjectName);

            CheckValues(args);

            GenerateBaseFiles(ProjectName);

            foreach (var service in Services)
            {
                await GenerateService(service);
                GenerateApi(service);
            }

            GenerateCompletedEventArgs(ProjectName);

            ApplyChanges();
        }


        private async Task GenerateService(string serviceName)
        {
            var svc = GetService(_solution, serviceName);

            if (svc != null)
            {
                var svcName = svc.Name.Remove(svc.Name.IndexOf(".", StringComparison.Ordinal));

                GenerateInterfaceAndChannel(svcName, ProjectName);
                GenerateServiceClient(svcName, ProjectName);
            }
        }

        private void GenerateApi(string svcName)
        {
            svcName = svcName.Remove(svcName.IndexOf(".", StringComparison.Ordinal));
            svcName = svcName.IndexOf("I", StringComparison.Ordinal) == 0 ? svcName.Remove(0, 1) : svcName;

            var sb = new StringBuilder();

            sb.Append(_usings);
            sb.Append("\n");

            sb.Append(" namespace " + ProjectName + "\n { \n");
            sb.Append("\t public partial class " + svcName + "Api\n\t { \n");

            foreach (var method in _methods)
            {
                var parameters = method.ParametersList.ToString().Replace("(", "").Replace(")", "");
                var paramList = method.ParametersList;
                var types = "";
                var paramNames = "";

                if (paramList != null)
                {
                    types = paramList.Parameters.Aggregate(types, (current, param) => current + param.Type + ", ");
                    types = types != "" ? "<" + types.Remove(types.Length - 2) + ">" : "";

                    paramNames = paramList.Parameters.Aggregate(paramNames, (current, param) => current + param.Identifier + ", ");
                }

                var returnType = method.ReturnTypeApi != "void" && method.ReturnTypeApi != "ResponseDto" ? ("<" + method.ReturnTypeApi + ">") : "";

                sb.Append("\t\t public async System.Threading.Tasks.Task" + returnType + " " + method.Name + "(" + parameters + ")\n");
                sb.Append("\t\t {\n");
                sb.Append("\t\t\t OperationContextScope scope = null;\n");
                sb.Append("\t\t\t I" + svcName + "Client client = null;\n");
                sb.Append("\t\t\t try\n");
                sb.Append("\t\t\t {\n");
                sb.Append("\t\t\t\t client = ClientFactory<" + svcName + "Client>.CreateClient(\"" + svcName + "Api\").Client;\n");
                sb.Append("\t\t\t\t scope = new OperationContextScope(((" + svcName + "Client)client).InnerChannel);\n");
                sb.Append("\t\t\t\t AddClientInformationHeader();\n");
                sb.Append("\t\t\t\t var res = System.Threading.Tasks.Task" + (method.ReturnType != "void" ? ("<" + method.ReturnType + ">") : "") + ".Factory.FromAsync" + types + "(client.Begin" + method.Name + ", client.End" + method.Name + ", " + paramNames + " null);\n");
                sb.Append("\t\t\t\t await res.ConfigureAwait(false);\n");

                if (method.ReturnTypeApi != "void")
                {
                    sb.Append("\t\t\t\t return await GetValue<" + method.ReturnTypeApi + ">(res);\n");
                }

                sb.Append("\t\t\t }\n");
                sb.Append("\t\t\t finally\n");
                sb.Append("\t\t\t {\n");
                sb.Append("\t\t\t\t var disposable = client as IDisposable;\n");
                sb.Append("\t\t\t\t if (disposable != null) disposable.Dispose();\n\n");
                sb.Append("\t\t\t\t disposable = scope as IDisposable;\n");
                sb.Append("\t\t\t\t if (disposable != null) disposable.Dispose();\n");
                sb.Append("\t\t\t }\n");
                sb.Append("\t\t }\n\n");

            }

            sb.Append("\t }");
            sb.Append(" }");

            CreateDocument(sb.ToString(), ProjectName, svcName + "Api.cs");

            sb.Clear();

            sb.Append(_usings);
            sb.Append("\n");

            sb.Append(" namespace " + ProjectApi + "\n { \n");
            sb.Append("\t public partial interface I" + svcName + "Api\n\t { \n");

            foreach (var method in _methods)
            {
                var parameters = method.ParametersList.ToString().Replace("(", "").Replace(")", "");
                var returnType = method.ReturnTypeApi != "void" && method.ReturnTypeApi != "ResponseDto" ? ("<" + method.ReturnTypeApi + ">") : "";

                sb.Append("\t\t System.Threading.Tasks.Task" + returnType + " " + method.Name + "(" + parameters + ");\n");
            }

            sb.Append("\t }");
            sb.Append(" }");

            CreateDocument(sb.ToString(), ProjectApi, "API/I" + svcName + "Api.cs");

        }

        private Document GetService(Solution solution, string iService)
        {
            _methods = new List<EndPoint>();
            Document svc = null;

            foreach (var project in solution.Projects)
            {
                foreach (var document in project.Documents)
                {
                    if (document.Name == iService)
                    {
                        svc = document;
                        break;
                    }
                }
            }

            if (svc != null)
            {
                CancellationToken cancellationToken = default(CancellationToken);

                SyntaxNode documentRoot = svc.GetSyntaxRootAsync(cancellationToken).Result;
                var rootCompUnit = (CompilationUnitSyntax)documentRoot;
                var defaultUsings = rootCompUnit.Usings.Select(x => x.Name.ToString());

                var syntaxTree = svc.GetSyntaxTreeAsync(cancellationToken).Result;
                var syntaxRoot = syntaxTree.GetRoot();
                var syntaxMethods = syntaxRoot.DescendantNodes().OfType<MethodDeclarationSyntax>();

                var methodDeclarationSyntaxs = syntaxMethods as IList<MethodDeclarationSyntax> ?? syntaxMethods.ToList();
                var nodeReturn = methodDeclarationSyntaxs.First().ReturnType.DescendantNodes().OfType<IdentifierNameSyntax>().First();
                var candidateUsing = SymbolFinder.FindDeclarationsAsync(svc.Project, nodeReturn.Identifier.ValueText, ignoreCase: false, cancellationToken: cancellationToken).Result.First().ContainingNamespace.ToString();

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
                    var nodes = method.ParameterList.DescendantNodes().OfType<IdentifierNameSyntax>();
                    if (nodes.Any())
                    {
                        var newUsing = SymbolFinder.FindDeclarationsAsync(svc.Project, nodes.First().Identifier.ValueText, ignoreCase: false, cancellationToken: cancellationToken).Result.First().ContainingNamespace.ToString();

                        if (!usingsCollection.Contains(newUsing) && !newUsing.Contains("Microsoft.") && defaultUsings.Contains(newUsing))
                        {
                            usingsCollection.Add(newUsing);
                        }
                    }
                }

                var newUsings = defaultUsings.Intersect(usingsCollection).ToList();
                newUsings = newUsings.Select(x => "using " + x).ToList();
                usingsCollection = newUsings;

                usingsCollection.Add("using " + candidateUsing);

                _usings = String.Join("; \n", usingsCollection);
                _usings = _usings + "; \n";

                _methods.AddRange(methodDeclarationSyntaxs.Select(sm => new EndPoint()
                {
                    Name = sm.Identifier.ToString(),
                    ReturnType = sm.ReturnType.ToString(),
                    ReturnTypeApi = sm.ReturnType.ToString().Contains("Response") ? GetProperty(sm.ReturnType, "Value") : sm.ReturnType.ToString(),
                    InterfaceReturnType = sm.ReturnType.ToString(),
                    ParametersList = sm.ParameterList,
                }));

                foreach (var method in _methods)
                {
                    method.ReturnType = ChangeType(method.ReturnType);
                    method.ReturnTypeApi = ChangeType(method.ReturnTypeApi);
                }

                return svc;
            }

            Console.WriteLine("Service wasn't found");

            return null;
        }

        private string ChangeType(string method)
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

            returnType = returnType == "Stream" ? "byte[]" : returnType;
            return returnType;
        }

        private string GetProperty(TypeSyntax returnType, string value)
        {
            var responseDto = returnType.ToString().Replace("Task<", "").Replace(">", "");
            var project = _solution.Projects.First(x => x.Name == ProjectApi);
            var responseClass = project?.Documents.FirstOrDefault(x => x.Name == responseDto + ".cs");

            var result = responseDto;

            if (responseClass != null)
            {
                var syntaxRoot = responseClass.GetSyntaxTreeAsync().Result.GetRoot();
                var syntaxproperties = syntaxRoot.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();
                var propertyDeclarationSyntax = syntaxproperties.FirstOrDefault(x => x.Identifier.ToString() == value);

                if (propertyDeclarationSyntax == null)
                {
                    if (returnType.ToString() == "ResponseDto")
                    {
                        result = "void";
                    }
                }
                else
                {
                    result = propertyDeclarationSyntax?.Type.ToString();
                }
            }
            else
            {
                var responsesDocument = project?.Documents.FirstOrDefault(x => x.Name == "GenerateResponseDto.cs");
                var responseSyntaxRoot = responsesDocument?.GetSyntaxTreeAsync().Result.GetRoot();
                var responseClasses = responseSyntaxRoot?.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

                var repClass = responseClasses?.FirstOrDefault(x => x.Identifier.ToString() == responseDto);

                if (repClass != null)
                {
                    var syntaxproperties = repClass.Members.OfType<PropertyDeclarationSyntax>().ToList();
                    var propertyDeclarationSyntax = syntaxproperties.FirstOrDefault(x => x.Identifier.ToString() == value);

                    if (propertyDeclarationSyntax != null)
                    {
                        result = propertyDeclarationSyntax.Type.ToString();
                    }
                }
            }

            if (result.Contains("IEnumerable"))
            {
                result = result.Replace("IEnumerable<", "").Replace(">", "");
                result = result + "[]";
            }

            return result;
        }

        private static void CreateDocument(string code, string projectName, string fileName)
        {
            if (fileName != null && projectName != null)
            {
                var folders = fileName.Split('/');
                fileName = folders[folders.Length - 1];
                folders = folders.Where((val, idx) => idx != folders.Length - 1).ToArray();

                var sourceText = SourceText.From(code);
                tasks.Add(Tuple.Create(fileName, sourceText, folders, projectName));
            }
        }

        private static void ApplyChanges()
        {
            foreach (var doc in tasks)
            {
                _project = _solution?.Projects.FirstOrDefault(x => x.Name == doc.Item4);

                var oldDocument = _project?.Documents.FirstOrDefault(x => x.Name == doc.Item1);

                if (oldDocument != null)
                {
                    bool isEqual = oldDocument.Folders.SequenceEqual(doc.Item3);

                    if (isEqual)
                    {
                        _project = _project.RemoveDocument(oldDocument.Id);
                    }
                }

                var newDocument = _project?.AddDocument(doc.Item1, doc.Item2, doc.Item3);
                _project = newDocument?.Project;
                _solution = _project?.Solution;
            }

            _workspace.TryApplyChanges(_project.Solution);
        }

        private static void GenerateBaseFiles(string projectName)
        {
            var sb = new StringBuilder();
            tasks = new List<Tuple<string, SourceText, string[], string>>();

            #region Create IProperter 

            sb.Append("namespace " + projectName + " \n{");
            sb.Append("\n\t public interface IProperter \n\t {\n");
            sb.Append("\t\t bool IsCaughtException { get; set; } \n");
            sb.Append("\t } \n}");

            CreateDocument(sb.ToString(), projectName, "Service References/IProperter.cs");

            #endregion

            #region Create ChannelContainer 
            sb.Clear();

            sb.Append("using System;\n\n");

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

            sb.Append("using System;" + " \n\n");
            sb.Append("using System.Collections.Concurrent;" + " \n\n");

            sb.Append("namespace " + projectName + "\n");
            sb.Append("{\n\t public static class ClientFactory<TClient> where TClient : class, IProperter , new()\n \t { \n");
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

            sb.Append("using System;\n");
            sb.Append("using System.ServiceModel;\n\n");

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

            sb.Append("using System;\n");
            sb.Append("using System.Threading.Tasks;\n");
            sb.Append("using System.Runtime.CompilerServices;\n");
            sb.Append("using System.Threading;\n\n");

            sb.Append("\n\n namespace " + projectName);
            sb.Append("\n {\n\t public class SimpleAwaiter : INotifyCompletion");
            sb.Append("\n\t {");

            sb.Append("\n\t\t #region Fields\n");
            sb.Append("\n\t\t protected readonly Task task;");
            sb.Append("\n\t\t protected readonly Action beforeAwait;");
            sb.Append("\n\t\t protected readonly Action afterAwait;");
            sb.Append("\n\n\t\t #endregion\n");

            sb.Append("\n\t\t public SimpleAwaiter(Task task, Action beforeAwait, Action afterAwait)");
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

            sb.Append("using System;\n");
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

            #region EndpointConfiguration


            sb.Clear();

            sb.Append("using System; \n\n");
            sb.Append("namespace " + projectName + "\n { \n");
            sb.Append("\t public enum EndpointConfiguration \n\t { \n");
            sb.Append("\t\t BasicHttpBinding_Client \n");
            sb.Append("\t } \n }");

            CreateDocument(sb.ToString(), projectName, "Service References/EndpointConfiguration.cs");

            #endregion
        }

        private void GenerateInterfaceAndChannel(string iService, string projectName)
        {
            var sb = new StringBuilder();

            sb.Append(_usings + " \n\n");

            sb.Append("namespace " + projectName + "\n { \n");
            sb.Append("\t [System.CodeDom.Compiler.GeneratedCodeAttribute(\"System.ServiceModel\", \"4.0.0.0\")]\n");
            sb.Append("\t [System.ServiceModel.ServiceContractAttribute(ConfigurationName = \"" + iService + "Client\")]\n");
            sb.Append("\t public interface " + iService + "Client\n \t{ \n");

            foreach (var method in _methods)
            {
                var parameters = method.ParametersList.ToString().Replace("(", "").Replace(")", "");
                var pm = parameters != "" ? parameters + ", " : "";

                sb.Append("\t\t [System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = \"http://tempuri.org/" + iService + "Client/" + method.Name + "\", ReplyAction = \"http://tempuri.org/" + iService + "iService/" + method.Name + "Response\")]\n");
                sb.Append("\t\t System.IAsyncResult Begin" + method.Name + "(" + pm + "System.AsyncCallback callback, object asyncState);\n\n");
                sb.Append("\t\t " + method.ReturnType + " End" + method.Name + "(System.IAsyncResult result);\n\n");
            }

            sb.Append("\t } \n\n");
            sb.Append("\t [System.CodeDom.Compiler.GeneratedCodeAttribute(\"System.ServiceModel\", \"4.0.0.0\")]\n");
            sb.Append("\t public interface " + iService + "Channel : " + iService + "Client, System.ServiceModel.IClientChannel{} \n }");

            CreateDocument(sb.ToString(), projectName, "Service References/" + iService + "Client.cs");
        }

        private static void GenerateCompletedEventArgs(string projectName)
        {
            var sb = new StringBuilder();

            foreach (var method in _competedArgsMethods)
            {
                if (method.InterfaceReturnType.ToString() != "void")
                {
                    sb.Clear();

                    sb.Append(_usings + " \n");

                    sb.Append(" namespace " + projectName + "\n { \n");
                    sb.Append("\t [System.CodeDom.Compiler.GeneratedCodeAttribute(\"System.ServiceModel\", \"4.0.0.0\")] \n");
                    sb.Append("\t public partial class " + method.Name + "CompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs \n");
                    sb.Append("\t { \n\n");
                    sb.Append("\t   private object[] results; \n\n");
                    sb.Append("\t   public " + method.Name + "CompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :\n");
                    sb.Append("\t   base(exception, cancelled, userState) {\n");
                    sb.Append("\t       this.results = results;\n");
                    sb.Append("\t   } \n\n");
                    sb.Append("\t   public " + method.InterfaceReturnType + " Result { \n");
                    sb.Append("\t       get {\n");
                    sb.Append("\t           base.RaiseExceptionIfNecessary();\n");
                    sb.Append("\t           return ((" + method.InterfaceReturnType + ")(this.results[0]));\n");
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

            sb.Append(_usings + " \n\n");

            sb.Append(" namespace " + projectName + "\n { \n");
            sb.Append("\t public partial class " + channelName + " : System.ServiceModel.ClientBase<" + svcName + "Client>, " + svcName + "Client, IProperter\n ");
            sb.Append("\t {");

            sb.Append(GeneratePropertiesAndConstructors(svcName));
            sb.Append(GenerateMethods(svcName + "Client"));
            sb.Append(GenerateAdditionMathods(svcName + "Client"));
            sb.Append(GenerateClientChannel(svcName + "Client"));

            sb.Append("    } \n");
            sb.Append("}");

            CreateDocument(sb.ToString(), projectName, "Service References/" + channelName + ".cs");

            _competedArgsMethods = _competedArgsMethods == null ? _methods
                                                                : _methods.Concat(_competedArgsMethods)
                                                                          .GroupBy(item => item.Name)
                                                                          .Select(group => group.First())
                                                                          .ToList();
        }

        private static string GenerateClientChannel(string svcClient)
        {
            var sb = new StringBuilder();
            var channelName = (svcClient.IndexOf("I", StringComparison.Ordinal) == 0 ? svcClient.Remove(0, 1) : svcClient) + "Channel";

            sb.Append("\t\t private class " + channelName + " : ChannelBase<" + svcClient + ">, " + svcClient + ", IProperter \n \t\t { \n");
            sb.Append("\t\t\t public bool IsCaughtException { get; set; } \n\n");

            sb.Append("\t\t public " + channelName + "(System.ServiceModel.ClientBase<" + svcClient + "> client) : base(client) { } \n\n");

            foreach (var method in _methods)
            {
                var parameters = "";

                foreach (var parameter in method.ParametersList.Parameters)
                {
                    parameters += parameter.Type + " " + parameter.Identifier + ", ";
                }

                sb.Append("\t\t public System.IAsyncResult Begin" + method.Name + "(" + parameters + "System.AsyncCallback callback, object asyncState) \n \t\t { \n");
                sb.Append("\t\t\t object[] _args = new object[" + method.ParametersList.Parameters.Count + "]; \n");

                foreach (var parameter in method.ParametersList.Parameters)
                {
                    sb.Append("\t\t\t _args[" + method.ParametersList.Parameters.IndexOf(parameter) + "] = " + parameter.Identifier + ";\n");
                }

                sb.Append("\t\t\t System.IAsyncResult _result = base.BeginInvoke(\"" + method.Name + "\", _args, callback, asyncState);\n");
                sb.Append("\t\t\t return _result;\n");
                sb.Append("\t\t } \n\n");

                sb.Append("\t\t public " + method.ReturnType + " End" + method.Name + "(System.IAsyncResult result) \n \t\t { \n");
                sb.Append("\t\t\t object[] _args = new object[0];\n");

                if (method.ReturnType.ToString() == "void")
                {
                    sb.Append("\t\t\t base.EndInvoke(\"" + method.Name + "\", _args, result);\n");
                }
                else
                {
                    sb.Append("\t\t\t " + method.ReturnType + " _result = ((" + method.ReturnType + ")(base.EndInvoke(\"" + method.Name + "\", _args, result)));\n");
                }

                if (method.ReturnType != "void")
                {
                    sb.Append("\t\t\t return _result;\n");
                }

                sb.Append("\t\t } \n\n");
            }
            sb.Append("\t } \n");

            return sb.ToString();
        }

        private static string GenerateAdditionMathods(string svcName)
        {
            var sb = new StringBuilder();
            var client = (svcName.IndexOf("I", StringComparison.Ordinal) == 0 ? svcName.Remove(0, 1) : svcName);

            sb.Append("\t\t protected override " + svcName + " CreateChannel() \n \t\t {\n");
            sb.Append("\t\t\t return new " + client + "Channel(this); \n \t\t } \n\n");

            sb.Append("\t\t private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration) \n \t\t { \n");
            sb.Append("\t\t\t if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_Client)) \n \t\t\t { \n");
            sb.Append("\t\t\t\t System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding(); \n");
            sb.Append("\t\t\t\t result.MaxBufferSize = int.MaxValue; \n");
            sb.Append("\t\t\t\t result.MaxReceivedMessageSize = int.MaxValue; \n");
            sb.Append("\t\t\t\t return result; \n \t\t\t } \n ");
            sb.Append("\t\t\t throw new System.InvalidOperationException(string.Format(\"Could not find endpoint with name \'{0}\'.\", endpointConfiguration));\n \t\t } \n\n");

            sb.Append("\t\t private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration) \n \t\t { \n");
            sb.Append("\t\t\t if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_Client)) \n \t\t\t { \n");
            sb.Append("\t\t\t\t return new System.ServiceModel.EndpointAddress(_remoteAddress); \n \t\t\t } \n\n");
            sb.Append("\t\t\t throw new System.InvalidOperationException(string.Format(\"Could not find endpoint with name \'{0}\'.\", endpointConfiguration));\n \t\t } \n\n");

            sb.Append("\t\t private static System.ServiceModel.Channels.Binding GetDefaultBinding() \n \t\t { \n");
            sb.Append("\t\t\t return " + client + ".GetBindingForEndpoint(EndpointConfiguration.BasicHttpBinding_Client); \n \t\t } \n");

            sb.Append("\t\t private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress() \n \t\t { \n");
            sb.Append("\t\t\t return " + client + ".GetEndpointAddress(EndpointConfiguration.BasicHttpBinding_Client); \n \t\t } \n\n");

            return sb.ToString();
        }

        private static string GeneratePropertiesAndConstructors(string svcName)
        {
            var properties = " \n\t\t private static string _remoteAddress = \"\"; \n";
            properties += " \n\t\t public bool IsCaughtException { get; set; } \n\n";

            var client = (svcName.IndexOf("I", StringComparison.Ordinal) == 0 ? svcName.Remove(0, 1) : svcName);

            return properties +
                   "\t\t public " + client + "Client() : base(" + client + "Client.GetDefaultBinding(), " + client + "Client.GetDefaultEndpointAddress()) { } \n\n" +
                   "\t\t public " + client + "Client(EndpointConfiguration endpointConfiguration) : base(" + client + "Client.GetBindingForEndpoint(endpointConfiguration), " + client + "Client.GetEndpointAddress(endpointConfiguration)) {   } \n\n" +
                   "\t\t public " + client + "Client(EndpointConfiguration endpointConfiguration, string remoteAddress) : base(" + client + "Client.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress)) { \n \t\t\t _remoteAddress = remoteAddress; \n \t\t } \n\n" +
                   "\t\t public " + client + "Client(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : base(" + client + "Client.GetBindingForEndpoint(endpointConfiguration), remoteAddress)  { \n \t\t\t _remoteAddress =  remoteAddress.Uri.AbsolutePath; \n \t\t } \n\n" +
                   "\t\t public " + client + "Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(binding, remoteAddress) {\n \t\t\t _remoteAddress =  remoteAddress.Uri.AbsolutePath; \n \t\t  } \n\n";
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

                var parms = method.ParametersList.Parameters.Count == 0 ? "" : (method.ParametersList.Parameters + ",");

                result.Append("\t\t [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]\n");
                result.Append("\t\t public System.IAsyncResult Begin" + method.Name + "(" + parms + " System.AsyncCallback callback, object asyncState)\n");
                result.Append("\t\t {\n");
                result.Append("\t\t   return base.Channel.Begin" + method.Name + "(");

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
                result.Append("\t\t       " + (method.ReturnType != "void" ? "return" : "") + " base.Channel.End" + method.Name + "(result);\n");
                result.Append("\t\t   }\n");
                result.Append("\t\t   catch (Exception)\n");
                result.Append("\t\t   {\n");
                result.Append("\t\t       IsCaughtException = true;\n");

                if (method.ReturnType != "void")
                {
                    result.Append("\t\t       return " + GetBadResponse(method.ReturnType) + ";\n");
                }

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

                if (method.ReturnType != "void")
                {
                    result.Append("\t\t   " + method.ReturnType + " retVal = ((" + client + ")(this)).End" + method.Name + "(result);\n");
                    result.Append("\t\t   return new object[] { retVal };\n");
                }
                else
                {
                    result.Append("\t\t   ((" + client + ")(this)).End" + method.Name + "(result);\n");
                    result.Append("\t\t   return null;\n");
                }


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

                result.Append("\t\t public void " + method.Name + "Async(" + (method.ParametersList.Parameters.Count > 0 ? method.ParametersList.Parameters + ", " : "") + " object userState)\n");
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
                result.Append("\t\t }\n\n");
                #endregion
            }

            return result.ToString();
        }

        private static string GetBadResponse(string returnType)
        {
            if (returnType == "bool" || returnType == "Boolean")
            {
                return "false";
            }

            if (returnType == "Guid")
            {
                return "Guid.Empty";
            }

            if (returnType == "long" || returnType == "int")
            {
                return "0";
            }

            return "null";
        }
    }


    public class EndPoint
    {
        public string Name { get; set; }
        public string ReturnTypeApi { get; set; }
        public string ReturnType { get; set; }
        public string InterfaceReturnType { get; set; }
        public ParameterListSyntax ParametersList { get; set; }
    }
}
