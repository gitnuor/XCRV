using System.Collections.Generic;
using XCRV.Domain.Entities;

namespace XCRV.Web.Models
{
    public class RelatedPartyViewModel
    {
        public string AccountNumber { get; set; }
        public InterestDetails RelatedPartyInfo { get; set; }
        public IEnumerable<InterestDetails> RelatedPartyDetails { get; set; }
    }
}
