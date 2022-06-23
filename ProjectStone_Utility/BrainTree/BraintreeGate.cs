using Braintree;
using Microsoft.Extensions.Options;

namespace ProjectStone_Utility.BrainTree;

public class BrainTreeGate : IBrainTreeGate
{
    public BrainTreeSettings Options { get; set; }
    private IBraintreeGateway BrainTreeGateway { get; set; }

    /// <summary>
    /// Sets BrainTree options to be used for the gateway.
    /// </summary>
    /// <param name="options"></param>
    public BrainTreeGate(IOptions<BrainTreeSettings> options)
    {
        Options = options.Value;
    }

    /// <summary>
    /// Adds a new BrainTree Gateway using the keys in the appSettings.json via _options.
    /// </summary>
    /// <returns></returns>
    public IBraintreeGateway CreateGateway()
    {
        return new BraintreeGateway(Options.Environment, Options.MerchantId, Options.PublicKey, Options.PrivateKey);
    }

    /// <summary>
    /// Creates a new BrainTree Gateway or returns the one that exists.
    /// </summary>
    /// <returns></returns>
    public IBraintreeGateway GetGateway()
    {
        // Null coalescing compound assignment.
        return BrainTreeGateway ??= CreateGateway();
    }
}