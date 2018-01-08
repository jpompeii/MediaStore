using MongoDB.Bson;

namespace MediaContentService.Model
{
    public class Library
    {   
        public string Id { get; private set; }
        public string AccountId { get; private set; }
        public string LibraryName { get; set; }
        public string AssetCollection { get; set; }
        private Account _account;

        public Account Account
        {
            get {
                if (_account == null && AccountId != null)
                {
                    _account = MediaStoreContext.GetContext().FindObjectById<Account>(AccountId);
                }
                return _account;
            }
            set {
                _account = value;
                AccountId = _account.Id;
            }
        }


        public Library()
        {
            _account = null;
        }

        public void SetAccount(Account account)
        {
            
        }
    }
}
