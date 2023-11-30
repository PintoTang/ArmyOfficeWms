using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.UserService.DataMode
{
    public sealed class UserInformationMode
    {
        private AccountMode _account=new AccountMode();
        
        private GroupMode _group=new GroupMode();
        private PersonMode _person =new PersonMode();

        public AccountMode Account
        {
            get { return _account; }
            set { _account = value; }
        }

        public GroupMode Group
        {
            get { return _group; }
            set { _group = value; }
        }

        public PersonMode Person
        {
            get { return _person; }
            set { _person = value; }
        }
    }
}
