using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ProxomoWP7Demo
{
    public class Custom_TrainingRec
    {
        public Custom_TrainingRec()
        {
        }

        public Custom_TrainingRec(string theTableName, string theID, string theTransDate, string theFinInst, double theAmount, string theDetails, bool theIsRetAcct, bool theIsJointAcct, int theAcctType)
        {
            tableName = theTableName; // required
            ID = theID; // optional

            
            transactionDate = Convert.ToDateTime(theTransDate);
            financialInstitutionName = theFinInst;
            amount = theAmount;
            details = theDetails;
            isRetirementAccount = theIsRetAcct;
            isJointAccount = theIsJointAcct;
            accountType = theAcctType;

            


            //PartitionKey = DateTime.UtcNow.ToString("MMddyyyy");

            //// Row key allows sorting, so we make sure the rows come back in time order.
            //RowKey = string.Format("{0:10}_{1}", DateTime.MaxValue.Ticks - DateTime.Now.Ticks, Guid.NewGuid());
        }

        //Dictionary<string, object> _properties = new Dictionary<string, object>();

        public string tableName { get; set; }
        public string ID { get; set; }

        public DateTime transactionDate { get; set; }
        public string financialInstitutionName { get; set; }
        public double amount { get; set; }
        public string details { get; set; }
        public bool isRetirementAccount { get; set; }
        public bool isJointAccount { get; set; }
        public int accountType { get; set; }

    }
}
