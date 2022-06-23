using Braintree;

namespace ProjectStone_Utility.BrainTree;

// BrainTree Interface Gateway.
public interface IBrainTreeGate
{
    // User BrainTree's IBrainTreeGateway as the return type for these methods to return gateway transactions.
    IBraintreeGateway CreateGateway();

    IBraintreeGateway GetGateway();
}