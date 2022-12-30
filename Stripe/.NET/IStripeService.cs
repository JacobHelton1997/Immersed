    public interface IStripeService
    {
        string CreateSession();
        string CreateSubscriptionSession(string priceId, string customerId);
        List<StripeProduct> GetSubscriptionsProducts();
        Paged<Subscription> GetSubscriptions(int pageIndex, int pageSize);
        CurrentSubscription GetSubscriptionByCreatedById(int createdById);
        int CreateSubscription(SubscriptionAddRequest model, int userId, string productId);
        int AddSubscription(string sessionId, int userId);
        object GetInvoice(string customerId);
        Stripe.Subscription UpdateStripeSubscription(string subscriptionId, string priceId);
        void UpdateSubscription(string invoiceId, string subscriptionId, string productId);
        Paged<Transaction> GetTransactionsByUserId(int pageIndex, int pageSize, int userId);
        Transaction GetTransactionById(int id);
        int CreateTransaction(TransactionAddRequest model, int userId);
        int AddTrasaction(string sessionId, int userId);
        int CreatePaymentAccount(PaymentAccountAddRequest model, int userId);
        void UpdatePaymentAccount(PaymentAccountUpdateRequest model, int userId);
        Paged<PaymentAccount> GetPaymentAccounts(int pageIndex, int pageSize);
        Paged<PaymentAccount> GetPaymentAccountsByCreatedBy(int pageIndex, int pageSize, int createdBy);
        PaymentAccount GetPaymentAccountById(int id);
        void DeletePaymentAccount(int id);
    }
