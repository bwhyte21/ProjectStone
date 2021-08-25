namespace ProjectStone_Utility.BrainTree
{
  public class BrainTreeSettings // These names can be found in appsettings.json as we are using Dependency Injection to use the items from there.
    {
        public string Environment { get; set; }
        public string MerchantId { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }
}