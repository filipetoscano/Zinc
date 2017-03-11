using System.Threading.Tasks;

namespace Zinc.WebServices
{
    /// <summary>
    /// Decorator interface, which describes a service method.
    /// </summary>
    /// <typeparam name="Rq">Request message type.</typeparam>
    /// <typeparam name="Rp">Response message type.</typeparam>
    public interface IMethod<Rq,Rp>
    {
        /// <summary>
        /// Executes the current method.
        /// </summary>
        /// <param name="context">Execution context.</param>
        /// <param name="request">Request message.</param>
        /// <returns>Response message.</returns>
        Task<Rp> RunAsync( ExecutionContext context, Rq request );
    }
}
