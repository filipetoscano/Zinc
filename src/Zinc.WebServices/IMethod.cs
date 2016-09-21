namespace Zinc.WebServices
{
    public interface IMethod<Rq,Rp>
    {
        Rp Run( ExecutionContext context, Rq request );
    }
}
