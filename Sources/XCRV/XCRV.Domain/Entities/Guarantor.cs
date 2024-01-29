using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class Guarantor
    {
        public string cust_id { get; set; }
        public string cust_name { get; set; }
        public string guarantor_cif { get; set; }

        public string foracid { get; set; }
        public string schm_code { get; set; }
        public string schm_desc { get; set; }
        public string acct_poa_as_rec_type { get; set; }
        public string cust_reltn_code { get; set; }
        public string acct_poa_as_name { get; set; }
        public string addr1 { get; set; }
        public string addr2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string cntry { get; set; }
        public string post_code { get; set; }
        public string telephone { get; set; }
        public string telexno { get; set; }
        public string fax { get; set; }
        public string del_flg { get; set; }

        public string address { get; set; }


        public string srl { get; set; }
        public string g_cif_id { get; set; }
        public string g_name { get; set; }
        public string date_of_birth { get; set; }
        public string father_name { get; set; }
        public string mother_name { get; set; }
        public string spouse_name { get; set; }
        public string present_adress { get; set; }
        public string permanent_adress { get; set; }
        public string nid { get; set; }
        public string contact_no { get; set; }
        public string profession { get; set; }
    }    
}
