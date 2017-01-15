using System.Threading.Tasks;

namespace Zinc.WebServices
{
    public interface IMethod<Rq,Rp>
    {
        Task<Rp> RunAsync( ExecutionContext context, Rq request );
    }
}
