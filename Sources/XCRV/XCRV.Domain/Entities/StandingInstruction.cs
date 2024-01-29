using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class StandingInstruction
    {
        public string id { get; set; }
        public string foracid { get; set; }
        public string sol_id { get; set; }
        public string si_srl_num { get; set; }
        public string si_start_date { get; set; }
        public string frequency { get; set; }
        public string part_tran_type { get; set; }
        public string fixed_amt { get; set; }
        public string auto_pstd_flg { get; set; }
        public string carry_for_alwd_flg { get; set; }
        public string deleted { get; set; }
    }
}
