using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCRV.Domain.Entities
{
    public class PremimumAsset
    {
        public string foracid { get; set; }
        public string schm_code { get; set; }
        public string bal { get; set; }
        public string ovrdu { get; set; }
        public string disb_amt { get; set; }
        public string int_rate { get; set; }
        public string sys_classification { get; set; }
        public string usr_classification { get; set; }
        public string next_int_dmd_date { get; set; }

        public string tot_trhom { get; set; }
        public string tot_trcar { get; set; }
        public string tot_trsln { get; set; }
        public string tot_cacla { get; set; }
        public string total { get; set; }
        public string avgbalance { get; set; }
        public string avgbalance2 { get; set; }


    }
}
