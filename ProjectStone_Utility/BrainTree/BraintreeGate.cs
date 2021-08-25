using Braintree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace ProjectStone_Utility.BrainTree
{
    public class BrainTreeGate : IBrainTreeGate
    {
        public BrainTreeSettings _options { get; set; }
        private IBraintreeGateway _brainTreeGateway { get; set; }

        /// <summary>
        /// Sets Braintree options to be used for the gateway.
        /// </summary>
        /// <param name="options"></param>
        public BrainTreeGate(IOptions<BrainTreeSettings> options)
        {
            _options = options.Value;
        }

        /// <summary>
        /// Adds a new Braintree Gateway using the keys in the appsettings.json via _options.
        /// </summary>
        /// <returns></returns>
        public IBraintreeGateway CreateGateway()
        {
            return new BraintreeGateway(_options.Environment, _options.MerchantId, _options.PublicKey, _options.PrivateKey);
        }

        /// <summary>
        /// Creates a new Braintree Gateway or returns the one that exists.
        /// </summary>
        /// <returns></returns>
        public IBraintreeGateway GetGateway()
        {
            // Null coalescing compound assignment.
            return _brainTreeGateway ??= CreateGateway();
        }
    }
}