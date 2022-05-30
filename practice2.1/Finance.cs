using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practice2._1
{
    public static class Finance
    {
        public static AccountBalance GetAccountBalance(int accountid)
        {
            using (var db = new dbDataContext())
            {
                var balance = db.Journals.Where(x=> x.AccountID == accountid).Select(x=>  new {x.Debit,x.Credit});
                var totalCredit = balance.Sum(x => (double?)x.Credit ) ?? 0;
                var totalDebit = balance.Sum(x => (double?)x.Debit ) ?? 0;
                var account = db.Accounts.SingleOrDefault(x => x.ID == accountid);
                if (account == null) return null ;
                return new AccountBalance(accountid, account.Name,Math.Abs( totalCredit - totalDebit), (totalCredit > totalDebit) ? AccountBalance.BalanceTypes.Credit : AccountBalance.BalanceTypes.Debit,totalCredit,totalDebit);
               
            }
        }
        public static AccountBalance GetAccountBalance(this Account account,int accountid)
        {
            return GetAccountBalance(account.ID);
        }
    
        public class AccountBalance
        {

            public AccountBalance(int id , string name , double amount , BalanceTypes balanceType , double totalCredit , double totalDebit)
            {
                ID = id;
                Name = name;    
                BalanceAmount = amount; 
                BalanceType = balanceType;  
                TotalCredit = totalCredit;
                TotalDebit = totalDebit;
            }
            public int ID { get; }
            public string Name { get; }
            public double BalanceAmount { get;  }
            public double TotalCredit { get; }
            public double TotalDebit { get; }
            public BalanceTypes BalanceType { get; }
            public string Balance
            {
                get
                {
                    return BalanceAmount.ToString() + " " + ((BalanceType == BalanceTypes.Credit) ? "دائن" : "مدين");
                }
            }
           
            public enum BalanceTypes
            {
                Credit =1,
                Debit
            }
        }
    }
}
