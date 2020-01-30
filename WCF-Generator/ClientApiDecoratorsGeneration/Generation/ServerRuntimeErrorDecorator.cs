using System.Text;
using Microsoft.CodeAnalysis;
using WCFGenerator.Common;

namespace WCFGenerator.ClientApiDecoratorsGeneration.Generation
{
    public class ServerRuntimeErrorDecorator : ClientApiDecorator
    {
        public override string ClassName => "ServerRuntimeErrorDecorator";
        protected override void GenerateMethodBody(StringBuilder sb, IMethodSymbol toDecorate)
        {
            sb.AppendLine("try");
            sb.AppendLine("{");
            sb.AppendLine("return await _actor." + toDecorate.GetMethodCall() + ";");
            sb.AppendLine("}");
            var returnTemplate = @"
			catch (FaultException<ExceptionDetail> ex)
			{
				throw new ServerRuntimeException(ex.Detail.Message, ex.Detail);
			}
			catch (FaultException ex)
			{
				throw new ServerRuntimeException(ex.Message, ex);
			}
			catch (ActionNotSupportedException ex)
			{
               throw new ServerRuntimeException(ex.Message, ex);
			}
            catch (AggregateException ex)
            {
                foreach (var innerException in ex.Flatten().InnerExceptions)
                {
					var e = innerException as FaultException;
                    if (e == null) continue;
                    var exc = e as FaultException<ExceptionDetail>;
                    if (exc?.Detail.InnerException != null)
                    {
                        throw new ServerRuntimeException(exc.Detail.InnerException.Message, exc.Detail.InnerException);
                    }
                    throw new ServerRuntimeException(innerException.Message, innerException);
                }
                throw;
            }";

            sb.AppendLine(returnTemplate);
        }

        protected override void GenerateUsings(StringBuilder sb)
        {
            var usings = @"using System;
            using System.ServiceModel;
            using YumaPos.Shared.Terminal.Infrastructure;
            using YumaPos.FrontEnd.Infrastructure.CommandProcessing;";
            sb.AppendLine(usings);
        }
    }
}
