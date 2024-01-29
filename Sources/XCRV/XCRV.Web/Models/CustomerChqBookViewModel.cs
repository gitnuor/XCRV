using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Web.Models
{
    public class CustomerChqBookViewModel
    {
        public CardChqEntity ChqBookInfo { get; set; }
        public List<CardChqEntity> ChqBookDetails { get; set; }
    }

    public class ChqActionStatus
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public ChqActionStatus()
        {
            
        }


        public ChqActionStatus(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public IList<ChqActionStatus> GetChqActionStatuses(bool forFullBook )
        {
            var data = new List<ChqActionStatus>()
            {
                new ChqActionStatus("-99","Select Action"),
                new ChqActionStatus("1","Cancel"),
                new ChqActionStatus("2","Stop"),
                
            };

            if(!forFullBook)
            {
                data.Add(new ChqActionStatus("3", "Cheque Lost"));
                data.Add(new ChqActionStatus("4", "Cheque bounce"));
            }

            return data;
        }
            
    }
}
